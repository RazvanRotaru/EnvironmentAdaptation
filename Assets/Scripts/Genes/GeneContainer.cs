using System.Collections.Generic;
using UnityEngine;
using GeneticAlgorithmForSpecies.Structures;

namespace GeneticAlgorithmForSpecies.Genes
{
    [System.Serializable]
    public class GeneContainer
    {
        [SerializeField] private ValueContainer<Gene.Type, Gene> values;

        public Dictionary<string, Gene> Data { get => values.Data; }
        public Dictionary<string, Gene> DataColne { get => CloneData(); }

        public Gene this[string key] { get => GetGene(key); /*set => SetValue(key, value);*/ }
        
        public GeneContainer()
        {
            values = new ValueContainer<Gene.Type, Gene>();

            foreach (Gene.Type type in (Gene.Type[])System.Enum.GetValues(typeof(Gene.Type)))
            {
                values[type] = new Gene();
            }
        }

        public GeneContainer(GeneContainer other)
        {
            values = new ValueContainer<Gene.Type, Gene>(other.values);
        }

        public Gene GetGene(string name)
        {
            Gene.Type type = (Gene.Type) System.Enum.Parse(typeof(Gene.Type), name);
            return values[type];
        }

        private Dictionary<string, Gene> CloneData()
        {
            Dictionary<string, Gene> ans = new Dictionary<string, Gene>();

            foreach (KeyValuePair<string, Gene> kvp in values.Data)
            {
                ans[kvp.Key] = new Gene(kvp.Value);
            }

            return ans;
        }
    }
}