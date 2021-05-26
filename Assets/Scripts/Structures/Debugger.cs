using UnityEngine;

namespace GeneticAlgorithmForSpecies.Structures
{
    public static class Debugger
    {
        private static System.Action<object> CustomLog = (object message) => { Debug.Log(message); };

        public static void SetLogFunction(System.Action<object> LogFunction)
        {
            CustomLog = LogFunction;
        }

        public static void Log(object message)
        {
            CustomLog(message);
        }
    }
}
