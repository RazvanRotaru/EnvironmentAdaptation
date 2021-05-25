using GeneticAlgorithmForSpecies.Genes;

namespace GeneticAlgorithmForSpecies.Equipment
{
    public interface IItem
    {
        void ApplyBuffs(ref GeneContainer geneContainer);
        void RemoveBuffs(ref GeneContainer geneContainer);
    }
}
