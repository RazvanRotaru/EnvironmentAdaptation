using UnityEngine;

namespace GeneticAlgorithmForSpecies.UDCT.Utilities.DevelopersConsole.Commands {
    public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand {
        [SerializeField] private string commandWord = string.Empty;

        public string CommandWord => commandWord;

        public abstract bool Process(string[] args);
    }
}