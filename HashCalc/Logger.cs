// cslog (C# Log)
// Simple application logger.
//
// author: Jared Booker <TheJaydox>
// license: MIT
// https://github.com/TheJaydox/cslog
//

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace cslog
{
    public enum ErrorLevel
    {
        Info,
        Error,
        Exception
    }

    public class Logger
    {
        // Version of the Logger Class
        private static string Version = "1.0";

        // Task & Collection
        private static BlockingCollection<LogEntry> Queue = new BlockingCollection<LogEntry>();
        private static bool TaskCreated = false;
        private static Task WriterTask;

        // Called on startup to populate initial line
        public static string Init = "cslog: " + Version;

        // File to log to
        public static string LogFile = "cslog.log";

        public static void Log(string message, ErrorLevel level = ErrorLevel.Info, [CallerMemberName]string caller = "")
        {
            //Console.WriteLine("Adding to queue: " + entry.Message);
            Queue.Add(new LogEntry(level, caller, message));
            StartWriterTask();
        }

        public static void Object<T>(T obj, ErrorLevel level = ErrorLevel.Info, [CallerMemberName]string caller = "")
        {
            Queue.Add(new LogEntry(level, caller, toJSON<T>(obj)));
            StartWriterTask();
        }

        public static void Exception(Exception ex, [CallerMemberName]string caller = "")
        {
            Queue.Add(new LogEntry(ErrorLevel.Exception, caller, ex.Message + ":" + ex.StackTrace));
            StartWriterTask();
        }

        private static string toJSON<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);

            // Create JSON string for logging
            return Encoding.Default.GetString(ms.ToArray());
        }

        private static void StartWriterTask()
        {
            Console.WriteLine("Start Writer Called!");
            if (TaskCreated == false)
            {
                Console.WriteLine("Logger Task Started!");

                // Update Task Status
                TaskCreated = true;

                WriterTask = Task.Factory.StartNew(() =>
                    Writer(),
                    TaskCreationOptions.LongRunning
                );
            }
        }

        private static void Writer()
        {
            using (var writer = File.AppendText(LogFile))
            {
                foreach (var item in Queue.GetConsumingEnumerable())
                {
                    writer.WriteLine(StringOut(item));
                    writer.Flush();
                }
            }
        }

        private static string StringOut(LogEntry entry)
        {

            Console.WriteLine(entry.Message);

            string output = String.Format("{0} :: {1} [{2}] {3}",
                entry.DT,
                entry.Level,
                entry.Caller,
                entry.Message);

            return output;
        }
    }

    public class LogEntry
    {
        public DateTime DT { get; set; }
        public string Level { get; set; }
        public string Caller { get; set; }
        public string Message { get; set; }

        public LogEntry(ErrorLevel level, string caller, string message)
        {
            this.DT = DateTime.UtcNow;
            this.Level = level.ToString();
            this.Caller = caller;
            this.Message = message;
        }
    }

}
