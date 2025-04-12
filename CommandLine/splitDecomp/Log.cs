using System;
using System.IO;

namespace splitDecomp
{
    partial class Program
    {
        private static TextWriter log;

        private static void WriteLogSingle(string msg)
        {
            log.Write(msg);
            Console.Write(msg);
            log.Flush();
        }

        private static void WriteLogLine(string msg)
        {
            log.WriteLine(msg);
            Console.WriteLine(msg);
            log.Flush();
        }
    }
}
