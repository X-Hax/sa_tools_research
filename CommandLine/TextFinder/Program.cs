using SAModel;
using SplitTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

// This program was made for the purpose of automatic cutscene text extraction from the Korean version of SADXPC.
// It uses the US version as a reference to locate the English text in the Korean EXE, and work up the pointer to find the cutscene text array.
// The resuls are saved as a ready to use split INI file.

namespace TextFinder
{
    class Program
    {
        static int Search(byte[] src, byte[] pattern, int startPos = 0)
        {
            int maxFirstCharSlot = src.Length - pattern.Length + 1;
            for (int i = startPos; i < maxFirstCharSlot; i++)
            {
                if (src[i] != pattern[0]) // compare only first byte
                    continue;

                // found a match on first byte, now try to match rest of the pattern
                for (int j = pattern.Length - 1; j >= 1; j--)
                {
                    if (src[i + j] != pattern[j]) break;
                    if (j == 1) return i;
                }
            }
            return -1;
        }

        static int[] SearchMultiple(byte[] src, byte[] pattern)
        {
            List<int> list = new List<int>();
            int maxFirstCharSlot = src.Length - pattern.Length + 1;
            for (int i = 0; i < maxFirstCharSlot; i++)
            {
                if (src[i] != pattern[0]) // compare only first byte
                    continue;

                // found a match on first byte, now try to match rest of the pattern
                for (int j = pattern.Length - 1; j >= 1; j--)
                {
                    if (src[i + j] != pattern[j]) break;
                    if (j == 1) list.Add(i);
                }
            }
            return list.ToArray();
        }

        static void Main(string[] args)
        {
            List<int> addresses = new();
            if (args.Length < 2)
            {
                Console.WriteLine("\nUsage: TextFinder sonic.exe sonic_kr.exe");
                Console.ReadLine();
                return;
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // Binary file
            byte[] sonic_us = File.ReadAllBytes(args[0]);
            byte[] sonic_kr = File.ReadAllBytes(args[1]);
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "out")))
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "out"));
            IniData out_cutscene = new IniData{ DataFilename="sonic.exe", Files = new Dictionary<string, SplitTools.FileInfo>() };
            IniData out_npc = new IniData{ DataFilename="sonic.exe", Files = new Dictionary<string, SplitTools.FileInfo>() };
            IniData out_multi = new IniData{ DataFilename="sonic.exe", Files = new Dictionary<string, SplitTools.FileInfo>() };

            foreach (string inif in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ini"))
            {
                IniData inifile = IniSerializer.Deserialize<IniData>(inif);
                foreach (var file in inifile.Files)
                {
                    switch (file.Value.Type)
                    {
                        case "cutscenetext":
                            bool nomatch = false;
                            int[] pntpointers = new int[0];
                            CutsceneText text = new CutsceneText(sonic_us, file.Value.Address, 0x400000, file.Value.Length);
                            int structaddr = 0;
                            Console.WriteLine("Cutscene text: " + file.Key);
                            for (int tryTextID = 0; tryTextID < text.Text[1].Length; tryTextID++)
                            {
                                Console.WriteLine("\tTrying text: " + text.Text[1][tryTextID]);
                                byte[] engstring = Encoding.GetEncoding(932).GetBytes(text.Text[1][tryTextID]);
                                int[] textaddrs = SearchMultiple(sonic_kr, engstring);
                                Console.WriteLine("Found {0} text addresses", textaddrs.Length);
                                for (int t = 0; t < textaddrs.Length; t++)
                                {
                                    Console.WriteLine("\tText address: " + textaddrs[t].ToString("X"));
                                    int[] textpointers = SearchMultiple(sonic_kr, BitConverter.GetBytes(textaddrs[t] + 0x400000));
                                    for (int p = 0; p < textpointers.Length; p++)
                                    {
                                        Console.WriteLine("\tTrying text pointer: {0}, ID {1}", textpointers[p].ToString("X"), tryTextID);
                                        pntpointers = SearchMultiple(sonic_kr, BitConverter.GetBytes(textpointers[p] + 0x400000 - 4 * tryTextID));
                                        if (pntpointers.Length == 0)
                                        {
                                            Console.WriteLine("No pointers found for pointer address {0}, trying again", p);
                                        }
                                        else
                                            break;
                                    }
                                    if (pntpointers.Length == 0)
                                    {
                                        Console.WriteLine("No pointers found for text address {0}, trying again", t);
                                    }
                                    else
                                        break;
                                }
                                for (int pnt = 0; pnt < pntpointers.Length; pnt++)
                                {
                                    Console.WriteLine("\tTrying pointer pointer: " + pntpointers[pnt].ToString("X"));
                                    structaddr = pntpointers[pnt] - 4;
                                    Console.WriteLine("\tStruct address: {0}, Mem: {1}", structaddr.ToString("X"), (structaddr + 0x400000).ToString("X"));
                                    file.Value.Address = structaddr;
                                    try
                                    {
                                        Console.WriteLine("Length: {0}", file.Value.Length);
                                        CutsceneText text_kr = new CutsceneText(sonic_kr, structaddr, 0x400000, file.Value.Length, true);
                                        nomatch = false;
                                        for (int l = 0; l < file.Value.Length; l++)
                                        {
                                            if (text_kr.Text[1][l].Contains("..."))
                                                continue;
                                            if (text_kr.Text[1][l].Contains("?!"))
                                                continue;
                                            if (text_kr.Text[1][l].Contains("!?"))
                                                continue;
                                            if (text_kr.Text[1][l] != text.Text[1][l])
                                            {
                                                Console.WriteLine("\tEnglish text line {0} doesn't match, trying again", l);
                                                //Console.WriteLine(text.Text[1][l]);
                                                //byte[] bytes_en = System.Text.Encoding.GetEncoding(932).GetBytes(text.Text[1][l]);
                                                //Console.WriteLine(Convert.ToHexString(bytes_en));
                                                //Console.WriteLine("vs");
                                                //byte[] bytes_kr = System.Text.Encoding.GetEncoding(932).GetBytes(text_kr.Text[1][l]);
                                                //Console.WriteLine(text_kr.Text[1][l]);
                                                //Console.WriteLine(Convert.ToHexString(bytes_kr));
                                                nomatch = true;
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        nomatch = true;
                                        Console.WriteLine("\tCatch English text doesn't match, trying again");
                                        //Console.Write(ex.ToString());
                                    }
                                    if (!nomatch)
                                    {
                                        Console.WriteLine("\tEnglish text matches");
                                        break;
                                    }
                                }
                                if (!nomatch)
                                    break;
                            }
                            if (nomatch)
                            {
                                Console.WriteLine("No match found");
                                return;
                            }
                            if (addresses.Contains(structaddr))
                                Console.WriteLine("Address {0} already exists", structaddr.ToString("X"));
                            else
                                addresses.Add(structaddr);
                            Console.WriteLine("\n\n");
                            out_cutscene.Files.Add(file.Key, file.Value);
                            break;
                        case "multistring":
                            bool dpointer = file.Value.CustomProperties.ContainsKey("doublepointer");
                            int countz = file.Value.Length > 1 ? file.Value.Length : 1;
                            var mstring = new MultilingualString(sonic_us, file.Value.Address, 0x400000, countz, dpointer);
                            Console.WriteLine("Multistring: " + file.Key + " : " + mstring.Text[1][0]);
                            byte[] m_engstring = Encoding.GetEncoding(932).GetBytes(mstring.Text[1][0]);
                            if (string.IsNullOrEmpty(mstring.Text[1][0]))
                            {
                                Console.WriteLine("English string is empty\n\n");
                                continue;
                            }
                            int m_textaddr = Search(sonic_kr, m_engstring);
                            Console.WriteLine("\b\bText addr: " + m_textaddr.ToString("X"));
                            int m_textpointer = Search(sonic_kr, BitConverter.GetBytes(m_textaddr + 0x400000));
                            int m_structaddr = m_textpointer - 4;
                            file.Value.Address = m_structaddr;
                            Console.WriteLine("\b\bStruct address: {0}, Mem: {1}", m_structaddr.ToString("X"), (m_structaddr+0x400000).ToString("X"));
                            if (addresses.Contains(m_structaddr))
                                Console.WriteLine("Address {0} already exists", m_structaddr.ToString("X"));
                            else
                                addresses.Add(m_structaddr);
                            Console.WriteLine("\n\n");
                            out_multi.Files.Add(file.Key, file.Value);
                            break;
                        case "npctext":
                            NPCText[][] npc = NPCTextList.Load(sonic_us, file.Value.Address, 0x400000, file.Value.Length);
                            bool found = false;
                            for (int i = 0; i < file.Value.Length; i++)
                            {
                                if (found)
                                    break;
                                if (!npc[1][i].HasText)
                                    continue;
                                foreach (NPCTextGroup group in npc[1][i].Groups)
                                {
                                    if (!group.HasText)
                                        continue;
                                    string n_text = group.Lines[0].Line;
                                    if (string.IsNullOrEmpty(n_text))
                                        Console.WriteLine("String is empty");
                                    Console.WriteLine("NPC Text: " + file.Key + " : " + n_text);
                                    int groupid = npc[1][i].Groups.IndexOf(group);
                                    byte[] n_engstring = Encoding.GetEncoding(932).GetBytes(n_text);
                                    int n_textaddr = Search(sonic_kr, n_engstring);
                                    Console.WriteLine("\b\bText addr: " + n_textaddr.ToString("X"));
                                    int n_textpointer = Search(sonic_kr, BitConverter.GetBytes(n_textaddr + 0x400000));
                                    if (addresses.Contains(n_textpointer))
                                    {
                                        Console.WriteLine("Address {0} already exists, looking again", n_textpointer.ToString("X"));
                                        do
                                        {
                                            n_textpointer = Search(sonic_kr, BitConverter.GetBytes(n_textaddr + 0x400000), n_textpointer + 1);
                                            Console.WriteLine("Trying address {0}", n_textpointer.ToString("X"));
                                        }
                                        while (addresses.Contains(n_textpointer));
                                    }
                                    addresses.Add(n_textpointer);
                                    Console.WriteLine("\bText pointer: " + n_textpointer.ToString("X"));
                                    int n_pntpointer = Search(sonic_kr, BitConverter.GetBytes(n_textpointer + 0x400000));
                                    Console.WriteLine("\bPointer pointer: " + n_pntpointer.ToString("X"));
                                    int n_structaddr = n_pntpointer - 8 * i - 4;
                                    Console.WriteLine("\bEntry addr: " + n_structaddr.ToString("X"));
                                    int n_structpointer = Search(sonic_kr, BitConverter.GetBytes(n_structaddr + 0x400000));
                                    Console.WriteLine("\bEntry pointer: " + n_structpointer.ToString("X"));
                                    int n_final = n_structpointer - 4;
                                    Console.WriteLine("\bFinal address: {0}, Mem: {1}", n_final.ToString("X"), (n_final+0x400000).ToString("X"));
                                    file.Value.Address = n_final;
                                    if (addresses.Contains(n_final))
                                        Console.WriteLine("Address {0} already exists", n_final.ToString("X"));
                                    else
                                        addresses.Add(n_final);
                                    Console.WriteLine("\n\n");
                                    found = true;
                                    break;
                                }
                            }
                            out_npc.Files.Add(file.Key, file.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
            IniSerializer.Serialize(out_npc, Path.Combine(Directory.GetCurrentDirectory(), "out", "npctext.ini"));
            IniSerializer.Serialize(out_multi, Path.Combine(Directory.GetCurrentDirectory(), "out", "multistring.ini"));
            IniSerializer.Serialize(out_cutscene, Path.Combine(Directory.GetCurrentDirectory(), "out", "cutscene.ini"));
        }
    }
}