using UnityEngine;
using GeneticAlgorithmForSpecies.Structures;
using GeneticAlgorithmForSpecies.Genes;
using System.Collections.Generic;

namespace GeneticAlgorithmForSpecies.Equipment
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Buff", menuName = "Equipment/Buffs", order = 1)]
    public class Buffs : ScriptableObject
    {
        [SerializeField] private ValueContainer<Gene.Type, Interval> buffs;

        public Buffs()
        {
            buffs = new ValueContainer<Gene.Type, Interval>();
        }

        private bool HasBuff(Gene.Type type) => buffs.ContainsKey(type);
        private Interval GetBuff(Gene.Type type) => buffs[type];

        public void AddBuff(Gene.Type type, Interval value) => buffs[type] += value;

        private void ApplyBuffs(ref GeneContainer geneContainer, int sign)
        {
            foreach (KeyValuePair<string, Gene> kvp in geneContainer.Data)
            {
                Gene.Type type = (Gene.Type)System.Enum.Parse(typeof(Gene.Type), kvp.Key);
                if (HasBuff(type))
                {
                    kvp.Value.OptimalInterval += GetBuff(type) * sign;
                }
            }
        }

        public void ApplyBuffs(ref GeneContainer geneContainer)
        {
            ApplyBuffs(ref geneContainer, +1);
        }

        public void RemoveBuffs(ref GeneContainer geneContainer)
        {
            ApplyBuffs(ref geneContainer, -1);
        }
    }
}