using System.IO;

namespace FinanceTracker.Utility
{
    public class Logger
    {
        private static readonly string LOG_FOLDER = "Logs";
        private static readonly string LOG_PATH = "Logs/Log.txt";
        private static readonly string ERROR_LOG_PATH = "Logs/ErrorLog.txt";
        private static readonly string START_MARK= "============================================================================================================";

        static Logger()
        {
            CheckIfLogFolderExists();
            MarkStartOfApp();
        }

        private static void MarkStartOfApp()
        {
            WriteLog(nameof(Logger), START_MARK);
            WriteLog(nameof(Logger), "Finance Tracker started!");
            WriteErrorLog(nameof(Logger), START_MARK);
            WriteErrorLog(nameof(Logger), "Finance Tracker started!");
        }

        private static void CheckIfLogFolderExists()
        {
            if (Directory.Exists(LOG_FOLDER) == false)
            {
                Directory.CreateDirectory(LOG_FOLDER);
                File.Create(LOG_PATH);
                File.Create(ERROR_LOG_PATH);
            }
        }

        public static void WriteLog(object classObject, string message) 
        {
            string className = GetClassName(classObject);
            using (StreamWriter sw = File.AppendText(LOG_PATH))
            {
                sw.WriteLine($"{DateTime.Now} [{className}]: {message}");
            }
        }

        public static void WriteErrorLog(object classObject, string message) 
        {
            string className = GetClassName(classObject);
            using (StreamWriter sw = File.AppendText(ERROR_LOG_PATH))
            {
                sw.WriteLine($"{DateTime.Now} [{className}]: {message}");
            }
        }

        private static string GetClassName(object classObject)
        {
            string className;
            if (classObject.GetType().Equals(typeof(string)))
                className = (string)classObject;
            else
                className = classObject.GetType().Name;
            return className;
        }
    }
}
