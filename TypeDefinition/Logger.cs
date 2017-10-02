using System;

namespace Common
{
    public static class Logger
    {
        public static void Debug(string message)
        {
            Console.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), message);
        }

        public static void Info(string message)
        {
            Console.WriteLine("{0}: {1}",DateTime.Now.ToLongTimeString(), message);
        }

        public static void Error(string message)
        {
            Console.WriteLine("ERROR - {0}: {1}", DateTime.Now.ToLongTimeString(), message);
        }
    }
}
