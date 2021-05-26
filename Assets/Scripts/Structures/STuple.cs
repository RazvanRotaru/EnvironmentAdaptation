using System;
using UnityEngine;

namespace GeneticAlgorithmForSpecies.Structures
{
    /// <summary>
    /// STuple extends <c>Tuple</c> in order to allow serialization.
    /// </summary>
    /// <typeparam name="T1">The type of the first item.</typeparam>
    /// <typeparam name="T2">The type of the second item.</typeparam>
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

        /// <summary>
        /// Returns the value of the first item of the tuple.
        /// </summary>
        public new T1 Item1 { get => value1; }

        /// <summary>
        /// Returns the value of the second item of the tuple.
        /// </summary>
        public new T2 Item2 { get => value2; }
    }
}