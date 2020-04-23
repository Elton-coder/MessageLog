using System;
using System.Data;
using System.IO;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

namespace MessageLog
{
    class Program
    {
        public enum Severity
        {
            Verbose,
            Trace,
            Information,
            Warning,
            Error,
            Critical
        }
        public static class Logger
        {
            public static Action<string> WriteMessage;

            public static Severity Loglevel { get; set } = Severity.Warning;

            public static void LogMessage(Severity s, string component,string msg)
            {
                if (s < Loglevel)
                    return;
                var outputMsg = $"{DateTime.Now}\t{s}\t{component}\t{msg}";
                WriteMessage(outputMsg);
            }

        }
        public class FileLogger
        {
            private readonly string logPath;
            public FileLogger(string path)
            {
                logPath = path;
                Logger.WriteMessage += LogMessage;
            }

            public void DetachLog() => Logger.WriteMessage -= LogMessage;
            // make sure this can't throw
            private void LogMessage(string msg)
            {
                try 
                {
                    using (var log = File.AppendText(logPath))
                    {
                        log.WriteLine(msg);
                        log.Flush();
                    }
                
                }
                catch(Exception)
                {
                    // Hmm. We caught an exception while
                    // logging. We can't really log the
                    // problem (since it's the log that's failing).
                    // So, while normally, catching an exception
                    // and doing nothing isn't wise, it's really the
                    // only reasonable option here.

                }
            }
        }

        public static class LoggingMethods
        {
            public static void LogToConsole(string message)
            {
                Console.Error.WriteLine(message);
            }
        }

        static void Main(string[] args)
        {
            // These two are not mutually exclusive
            Logger.WriteMessage += LoggingMethods.LogToConsole;
            var file = new FileLogger("log.txt");

        }
    }
}
