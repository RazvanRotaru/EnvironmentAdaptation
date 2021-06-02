using System.Collections.Generic;
using UnityEngine;
using GeneticAlgorithmForSpecies.Structures;

namespace GeneticAlgorithmForSpecies.Genes
{
    /// <summary>
    /// This class describes a individual
    /// </summary>
    [System.Serializable]
    public class GeneContainer
    {
        [SerializeField] private ValueContainer<Gene.Type, Gene> values;

        public Dictionary<string, Gene> Data { get => values.Data; }
        public Dictionary<string, Gene> DataColne { get => CloneData(); }

        public Gene this[string key] { get => GetGene(key); }
        
        public GeneContainer()
        {
            values = new ValueContainer<Gene.Type, Gene>();

            foreach (Gene.Type type in (Gene.Type[])System.Enum.GetValues(typeof(Gene.Type)))
            {
                values[type] = Gene.GetDefault(type.ToString());
            }
        }

        public GeneContainer(GeneContainer other)
        {
            values = new ValueContainer<Gene.Type, Gene>(other.values);
        }

        public GeneContainer(GeneContainer otherGene, Dictionary<string, Gene> other)
        {
            Dictionary<Gene.Type, Gene> newValues = new Dictionary<Gene.Type, Gene>();
            values = new ValueContainer<Gene.Type, Gene>(otherGene.values);

            foreach (Gene.Type type in (Gene.Type[])System.Enum.GetValues(typeof(Gene.Type)))
            {
                if (other.TryGetValue(type.ToString(), out Gene gene))
                {
                    newValues[type] = gene;
                }
                else if (values.Data.TryGetValue(type.ToString(), out gene))
                {
                    newValues[type] = gene;
                }
                else
                {
                    newValues[type] = Gene.GetDefault(type.ToString());
                }
            }

            values = new ValueContainer<Gene.Type, Gene>(newValues);
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