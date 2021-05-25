using System;
using UnityEngine;

namespace GeneticAlgorithmForSpecies.Structures
{
    [Serializable]
    public class STuple<T1, T2> : Tuple<T1, T2>
    {
        [SerializeField]
        private T1 value1;

        [SerializeField]
        private T2 value2;

        public STuple(T1 item1, T2 item2) : base(item1, item2)
        {
            value1 = item1;
            value2 = item2;
        }

        public new T1 Item1 { get => value1; }
        public new T2 Item2 { get => value2; }
    }
}