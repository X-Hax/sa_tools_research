using System;
using System.Collections.Generic;
using System.Globalization;
using SAModel;

namespace ScanAction
{
    partial class Program
    {
        private static List<int> FindActions(byte[] datafile, uint imageBase, SplitTools.FileInfo fileInfo, bool chunk)
        {
            List<int> result = new List<int>();
            // Set motion pointer
            uint motionPointer = imageBase + (uint)fileInfo.Address;
            // Set number of parts
            int numparts = 0;
            if (fileInfo.CustomProperties.ContainsKey("numparts"))
                numparts = int.Parse(fileInfo.CustomProperties["numparts"]);
            // Set reference model
            uint objectPointer = 0;
            if (fileInfo.CustomProperties.ContainsKey("refaddr"))
            {
                objectPointer = imageBase + uint.Parse(fileInfo.CustomProperties["refaddr"], NumberStyles.HexNumber);
            }
            // In most cases the action is right after the motion, so this should be checked first
            if (BitConverter.ToUInt32(datafile, fileInfo.Address + 0x10) == motionPointer)
            {
                // If the reference object is known, compare the pointer to it
                if (objectPointer != 0)
                {
                    if (BitConverter.ToUInt32(datafile, fileInfo.Address + 0xC) == objectPointer)
                        result.Add(fileInfo.Address + 0xC);
                }
                // Otherwise read the pointer and check if the object has the same number of animated parts
                else
                {
                    try
                    {
                        uint objPointer = BitConverter.ToUInt32(datafile, fileInfo.Address + 0xC);
                        NJS_OBJECT testobj = new NJS_OBJECT(datafile, (int)(objPointer - imageBase), imageBase, chunk ? ModelFormat.Chunk : ModelFormat.BasicDX, new Dictionary<int, Attach>());
                        if (testobj.CountAnimated() == numparts)
                            result.Add(fileInfo.Address + 0xC);
                    }
                    catch { }
                }
            }
            // If that didn't work, scan the whole file
            else
            {
                if (objectPointer == 0)
                {
                    Console.WriteLine("No reference model: " + fileInfo.Filename + " (" + (imageBase + fileInfo.Address).ToString("X") + ")");
                }
                // Scan the file
                for (int i = 0; i < datafile.Length - 4; i += 4)
                {
                    // If the motion pointer is detected, try checking if it's an action
                    if (BitConverter.ToUInt32(datafile, i) == motionPointer)
                    {
                        // If the object pointer is known, just check that
                        if (objectPointer != 0)
                        {
                            if (BitConverter.ToUInt32(datafile, i - 4) == objectPointer)
                                result.Add(i - 4);
                        }
                        // Otherwise load the object from the pointer before the motion pointer and compare the number of animated parts
                        else
                        {
                            uint pointer = BitConverter.ToUInt32(datafile, i - 4);
                            if (pointer == 0 || pointer < imageBase)
                                continue;
                            int modeladdr = (int)(pointer - imageBase);
                            try
                            {
                                NJS_OBJECT obj = new NJS_OBJECT(datafile, modeladdr, imageBase, chunk ? ModelFormat.Chunk : ModelFormat.BasicDX, new Dictionary<int, Attach>());
                                if (obj.CountAnimated() == numparts)
                                    result.Add(i - 4);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}