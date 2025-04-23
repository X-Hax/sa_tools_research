using System;
using System.IO;

namespace splitDecomp
{
    public static class Log
    {
        static TextWriter Writer;

        public static void Init(string file)
        {
            Writer = File.CreateText(file);
        }

        public static void WriteLine(string message, params object[] data)
        {
            Console.WriteLine(message, data);
            Writer.WriteLine(message, data);
            Writer.Flush();
        }

        public static void Write(string message, params object[] data)
        {
            Console.Write(message, data);
            Writer.Write(message, data);
            Writer.Flush();
        }

        public static void Finish()
        {
            Writer.Flush();
            Writer.Close();
        }
    }
}