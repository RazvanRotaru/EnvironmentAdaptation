using System.Collections.Generic;
using UnityEngine;

namespace GeneticAlgorithmForSpecies.Structures
{
    /// <summary>
    /// A custom serializable Dictionary implementation
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public class ValueContainer<TKey, TValue>
    {
        private Dictionary<string, TValue> values;
        [SerializeField] private List<STuple<TKey, TValue>> list;

        public Dictionary<string, TValue> Data { get => values; }

        public ValueContainer()
        {
            values = new Dictionary<string, TValue>();
            list = new List<STuple<TKey, TValue>>();
        }

        public ValueContainer(ValueContainer<TKey, TValue> other)
        {
            values = new Dictionary<string, TValue>();
            list = new List<STuple<TKey, TValue>>();

            foreach (STuple<TKey, TValue> elm in other.list)
            {
                SetValue(elm.Item1, elm.Item2);
            }
        }

        public ValueContainer(Dictionary<TKey, TValue> values)
        {
            list = new List<STuple<TKey, TValue>>();
            this.values = new Dictionary<string, TValue>();
             
            foreach (KeyValuePair<TKey, TValue> kvp in values)
            {
                SetValue(kvp.Key, kvp.Value);
            }
        }

        public ValueContainer(List<STuple<TKey, TValue>> tupleList)
        {
            values = new Dictionary<string, TValue>();
            list = new List<STuple<TKey, TValue>>();

            foreach (STuple<TKey, TValue> tuple in tupleList)
            {
                SetValue(tuple.Item1, tuple.Item2);
            }
        }

        public TValue this[TKey key] { get => GetValue(key);  set => SetValue(key, value); }

        public override bool Equals(object obj)
        {
            return obj is ValueContainer<TKey, TValue> container &&
                   EqualityComparer<Dictionary<string, TValue>>.Default.Equals(values, container.values);
        }

        public override int GetHashCode()
        {
            return 1649527923 + EqualityComparer<Dictionary<string, TValue>>.Default.GetHashCode(values);
        }

        public bool ContainsKey(TKey key) => values.Count > 0 && values.ContainsKey(key.ToString());

        private TValue GetValue(TKey key) => values[key.ToString()];


        private void SetValue(TKey key, TValue value)
        {
            System.Type type = typeof(TValue);
            var constructor = type.GetConstructor(System.Type.EmptyTypes);

            TValue newValue = value;
            if (constructor != null && !typeof(TValue).IsSubclassOf(typeof(MonoBehaviour)))
            {
                newValue = (TValue)System.Activator.CreateInstance(typeof(TValue), new object[] { value });
            }

            values[key.ToString()] = newValue;

            if (list.Exists(e => e.Item1.Equals(key)))
            {
                list.RemoveAll(e => e.Item1.Equals(key));
            }

            list.Add(new STuple<TKey, TValue>(key, newValue));
        }
    }
}
