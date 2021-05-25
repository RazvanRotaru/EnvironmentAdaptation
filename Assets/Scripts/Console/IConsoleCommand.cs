namespace GeneticAlgorithmForSpecies.UDCT.Utilities.DevelopersConsole.Commands {
    public interface IConsoleCommand {
        string CommandWord { get; }

        bool Process(string[] args);
    }
}