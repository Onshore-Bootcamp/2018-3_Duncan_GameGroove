using System;
using System.Configuration;
using System.IO;

namespace GameGrooveDAL
{
    public class Logger
    {
        //instantiate logpath variable
        private readonly string _LogPath;

        /// <summary>
        /// My logger needs to be supplied the file path for ErrorLog.txt
        /// </summary>
        /// <param name="logPath">File path for ErrorLog.txt found in WebConfig</param>
        public Logger(string logPath)
        {
            _LogPath = logPath;
        }

        /// <summary>
        /// Method to write errors to a text file for use in debugging.
        /// </summary>
        /// <param name="className">Name of the class in which the error was thrown</param>
        /// <param name="methodName">Name of the method in which the error was thrown</param>
        /// <param name="ex">Exception variable</param>
        /// <param name="level"></param>
        public void ErrorLog(string className, string methodName, Exception ex, string level = "Error")
        {
            //pull stack trace
            string stackTrace = ex.StackTrace;

            //write to ErrorLog.txt
            using (StreamWriter errorWriter = new StreamWriter(_LogPath, true))
            {
                errorWriter.WriteLine(new string('~', 40));
                errorWriter.WriteLine($"Class: {className} Method: {methodName} Date: {DateTime.Now.ToString()} {level}\n{ex.Message}\n{stackTrace}");
            }
        }
    }
}
