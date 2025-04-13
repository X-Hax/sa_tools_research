using SAModel;
using SplitTools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanAction
{
    partial class Program
    {
        private static List<int> FindActions(byte[] datafile, uint imageBase, SplitTools.FileInfo fileInfo)
        {
            List<int> result = new List<int>();
            uint motionPointer = imageBase + (uint)fileInfo.Address;
            uint objectPointer = 0;
            List<int> numverts_list = new List<int>();
            int[] numverts = new int[0];
            int numparts = 0;
            if (fileInfo.CustomProperties.ContainsKey("refaddr"))
            {
                objectPointer = imageBase + uint.Parse(fileInfo.CustomProperties["refaddr"], NumberStyles.HexNumber);
            }
            else
            {
                if (fileInfo.CustomProperties.ContainsKey("numparts"))
                    numparts = int.Parse(fileInfo.CustomProperties["numparts"]);
                if (fileInfo.CustomProperties.ContainsKey("numverts"))
                {
                    string[] vertlist = fileInfo.CustomProperties["numverts"].Split(',');
                    for (int v = 0; v < vertlist.Length; v++)
                    {
                        numverts_list.Add(int.Parse(vertlist[v]));
                        numverts = numverts_list.ToArray();
                    }
                }
            }
            for (int i = 0; i < datafile.Length - 4; i++)
            {
                if (BitConverter.ToUInt32(datafile, i) == motionPointer)
                {
                    if (objectPointer != 0)
                    {
                        if (BitConverter.ToUInt32(datafile, i - 4) == objectPointer)
                            result.Add(i - 4);
                    }
                    else
                    {
                        uint pointer = BitConverter.ToUInt32(datafile, i - 4);
                        if (pointer == 0 || pointer < imageBase)
                            continue;
                        int modeladdr = (int)(pointer - imageBase);
                        NJS_OBJECT obj = new NJS_OBJECT(datafile, modeladdr, imageBase, ModelFormat.BasicDX, new Dictionary<int, Attach>());
                        if (obj.CountAnimated() == numparts)
                            result.Add(i - 4);
                    }

                }
            }
            return result;
        }
    }
}