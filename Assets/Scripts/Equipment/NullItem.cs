using GeneticAlgorithmForSpecies.Genes;

namespace GeneticAlgorithmForSpecies.Equipment
{
    class NullItem : IItem
    {
        private static NullItem _instance;
        public static NullItem Instance { get => GetInstance(); }

        private NullItem() { }

        private static NullItem GetInstance()
        {
            if (_instance == null)
            {
                _instance = new NullItem();
            }

            return _instance;
        }

        public void ApplyBuffs(ref GeneContainer geneContainer) { }
        public void RemoveBuffs(ref GeneContainer geneContainer) { }
    }
}
