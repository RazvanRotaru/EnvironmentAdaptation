using UnityEngine;
using GeneticAlgorithmForSpecies.Structures;
using GeneticAlgorithmForSpecies.Genes;
using System.Collections.Generic;

namespace GeneticAlgorithmForSpecies.Equipment
{
    /// <summary>
    /// This class describes the buff of an item
    /// </summary>
    [System.Serializable]
    public class Buffs
    {
        private ValueContainer<Gene.Type, Interval> _data = null;
        [SerializeField] private List<STuple<Gene.Type, Interval>> values;

        public Buffs()
        {
            values = new List<STuple<Gene.Type, Interval>>()
            {
                new STuple<Gene.Type, Interval>(Gene.Type.Temperature, new Interval()),
                new STuple<Gene.Type, Interval>(Gene.Type.Humidity, new Interval()),
                new STuple<Gene.Type, Interval>(Gene.Type.AtmPressure, new Interval()),
            };
            _data = null;
        }

        public void Init()
        {
            _data = new ValueContainer<Gene.Type, Interval>(tupleList: values);
        }

        private ValueContainer<Gene.Type, Interval> Values { get => GetValues(); }

        private ValueContainer<Gene.Type, Interval> GetValues()
        {
            if (_data == null)
            {
                _data = new ValueContainer<Gene.Type, Interval>(tupleList: values);
            }

            return _data;
        }

        private bool HasBuff(Gene.Type type) => Values.ContainsKey(type);
        private Interval GetBuff(Gene.Type type) => Values[type];

        public void AddBuff(Gene.Type type, Interval value) => Values[type] += value;

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