using log4net;
using System;
using System.IO;
using System.Text;

namespace Cloud.Core
{
    public static class ExceptionLogger
    {
        private static bool mDisabled;

        public static bool DisabledState
        {
            get { return mDisabled; }
            set { mDisabled = value; }
        }

        private static readonly ILog _sqlLogger = LogManager.GetLogger("MySQL");
        private static readonly ILog _threadLogger = LogManager.GetLogger("Thread");
        private static readonly ILog _exceptionLogger = LogManager.GetLogger("Exception");
        private static readonly ILog _criticalExceptionLogger = LogManager.GetLogger("Critical");
        private static readonly ILog _wiredLogger = LogManager.GetLogger("Wired");

        public static void WriteLine(string Line, ConsoleColor Colour = ConsoleColor.Gray)
        {
            if (!mDisabled)
            {
                Console.ForegroundColor = Colour;
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + Line);
            }
        }

        public static void LogQueryError(string query, Exception exception)
        {
            WriteToFile(@"Logs\errors\MySQL_errors.log", "Error in query:\r\n" + query + "\r\n" + exception + "\r\n\r\n");
        }

        public static void LogException(Exception exception)
        {
            WriteToFile(@"Logs\errors\Exception_errors.log", "Exception:\r\n" + exception + "\r\n\r\n");
        }

        public static void LogCriticalException(Exception exception)
        {
            WriteToFile(@"Logs\errors\Critical_errors.log", "Exception:\r\n" + exception + "\r\n\r\n");
        }

        public static void LogThreadException(Exception exception)
        {
            WriteToFile(@"Logs\errors\Thread_errors.log", "Thread Exception:\r\n" + exception + "\r\n\r\n");
        }

        public static void LogWiredException(Exception exception)
        {
            WriteToFile(@"Logs\errors\Wired_errors.log", "Wired Exception:\r\n" + exception + "\r\n\r\n");
        }

        public static void WriteToFile(string path, string content)
        {
            try
            {
                FileStream Writer = new FileStream(path, FileMode.Append, FileAccess.Write);
                byte[] Msg = Encoding.ASCII.GetBytes(Environment.NewLine + content);
                Writer.Write(Msg, 0, Msg.Length);
                Writer.Dispose();
            }
            catch (Exception e)
            {
                WriteLine("Could not write to file: " + e + ":" + content);
            }
        }

        public static void DisablePrimaryWriting(bool ClearConsole)
        {
            mDisabled = true;
            if (ClearConsole)
                Console.Clear();
        }
    }
}
