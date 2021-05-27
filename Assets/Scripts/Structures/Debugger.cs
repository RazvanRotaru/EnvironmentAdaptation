using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GeneticAlgorithmForSpecies.Structures
{
    /// <summary>
    /// This class provides custom logs either globally, for a specific class or for a specific method.
    /// </summary>
    /// <remarks>
    /// The priority order is: method, class, global
    /// </remarks>
    public static class Debugger
    {
        private static System.Action<object> CustomLog = (object message) => { Debug.Log(message); };
        private static readonly Dictionary<string, System.Action<object>> logFunctions = new Dictionary<string, System.Action<object>>();
        private static readonly Dictionary<string, string> path2type = new Dictionary<string, string>();
        private static string _folder = Application.dataPath + "/Logs"; // "..../Assets/Logs"

        static Debugger() {
            if (!Directory.Exists(_folder))
            {
                Directory.CreateDirectory(_folder);
            }
        }

        public static void SetLoggingFolder()
        {
            _folder = EditorUtility.OpenFolderPanel("Select Directory", Application.dataPath, "Logs");
        }

        /// <summary>
        /// This functions sets a custom global logging function.
        /// </summary>
        /// <param name="LogFunction">The custom logging function</param>
        public static void SetLogFunction(System.Action<object> LogFunction)
        {
            CustomLog = LogFunction;
        }

        /// <summary>
        /// This function logs a message in a custom way.
        /// </summary>
        /// <remarks>
        /// If no global or specific custom logging function was set, <c>Debug.Log()</c> will be used by default.
        /// </remarks>
        /// <param name="message">The message to be logged</param>
        /// <param name="method">The name of the caller methods</param>
        /// <param name="path">The path of the caller methods file, which determines its type</param>
        public static void Log(object message, 
                [System.Runtime.CompilerServices.CallerMemberName] string method = "", 
                [System.Runtime.CompilerServices.CallerFilePath] string path = "")
        {
            if (logFunctions.TryGetValue(method, out System.Action<object> SpecificLog))
            {
                SpecificLog.Invoke(message);
            } 
            else
            {
                string fileName = GetTypeFromPath(path);

                if (logFunctions.TryGetValue(fileName, out SpecificLog))
                {
                    SpecificLog.Invoke(message);
                }
                else
                {
                    CustomLog.Invoke(message);
                }
            }
        }

        /// <summary>
        /// This function registers a specific logging function for a specified key.
        /// </summary>
        /// <param name="LogFunction">The custom logging function</param>
        /// <param name="name">Could be either a key or the name of the class that will be custom logged</param>
        public static void RegisterLogFunction<T>(System.Action<T> LogFunction, string name)
        {
            logFunctions[name] = (x) => LogFunction.Invoke((T)x);
        }

        /// <summary>
        /// This function registers a specific logging function for a specified type.
        /// </summary>
        /// <param name="LogFunction">The custom logging function</param>
        /// <param name="type">The type that will be custom logged</param>
        public static void RegisterLogFunction<T>(System.Action<T> LogFunction, System.Type type)
        {
            logFunctions[type.ToString()] = (x) => LogFunction.Invoke((T)x);
        }

        private static string GetTypeFromPath(string path = "")
        {
            if (path2type.TryGetValue(path, out string fileName)) {
                return fileName;
            }

            fileName = Path.GetFileName(path);
            
            if (fileName is null || fileName == string.Empty)
            {
                throw new System.Exception("Could not extract the file of the callling method");
            }

            int extensionIndex = fileName.LastIndexOf('.');
            if (extensionIndex < 0)
            {
                throw new System.Exception("The file of the calling method is not correctly named.");
            }

            while (extensionIndex > 0)
            {
                fileName = fileName.Substring(0, extensionIndex);
                extensionIndex = fileName.LastIndexOf('.');
            }

            path2type[path] = fileName;
            return fileName;
        }

        /// <summary>
        /// This function writes the log message to a file of format [LOG_DIR]/[CALLING_CLASS]/[CALLING_METHOD]_[SUFFIX].
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="sufix">The sufix of the file</param>
        /// <param name="timestamp">Enables timestamping, in seconds since the start-up</param>
        /// <param name="header">Description of containing logs, is only written once, at the beggining of the file</param>
        /// <param name="method">Calling method</param>
        /// <param name="callerPath">The path of the caller methods file, which determines its type</param>
        public static void WriteToFile(object message, string sufix = "log.txt", bool timestamp = false, string header = "",
                                [System.Runtime.CompilerServices.CallerMemberName] string method = "",
                                [System.Runtime.CompilerServices.CallerFilePath] string callerPath = "")
        {
            if (timestamp)
            {
                message = $"{Time.time},{message}";
            }
            
            string dir = $"{_folder}/{GetTypeFromPath(callerPath)}";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string path = $"{dir}/{method}_{sufix}";

            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(header);
                    sw.WriteLine(message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(message);
                }
            }
        }
    }
}
