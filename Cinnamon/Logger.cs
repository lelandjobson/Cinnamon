using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinnamon
{
    internal enum LoggerVerbosity
    {
        Silent,
        Normal,
        Verbose
    }

    internal static class Logger
    {
        public static LoggerVerbosity Verbosity = LoggerVerbosity.Normal;

        internal static void Log(string message, object any = null)
        {
            if(Verbosity == LoggerVerbosity.Silent) { return; }
            Console.WriteLine(message); 
            if(any != null) { Console.WriteLine(any.ToString()); }
        }

        internal static void LogException(Exception e, object any = null)
        {
            if (Verbosity == LoggerVerbosity.Silent) { return; }
            Console.WriteLine($"Exception thrown: {e.Message}. \n \n at {e.StackTrace} \n \n {any ?? String.Empty}");
        }
    }
}
