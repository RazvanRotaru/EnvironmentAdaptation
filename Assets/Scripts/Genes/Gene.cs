using UnityEngine;
using GeneticAlgorithmForSpecies.Structures;
using System.Collections.Generic;

namespace GeneticAlgorithmForSpecies.Genes
{
    /// <summary>
    /// This class describes a gene
    /// </summary>
    [System.Serializable]
    public class Gene
    {
        public enum Type
        {
            Temperature,
            Humidity,
            AtmPressure,
        }

        private static Dictionary<string, Interval> _ranges = new Dictionary<string, Interval>()
        {
            { Type.Temperature.ToString(), new Interval(0, 60) },
            { Type.Humidity.ToString(), new Interval(0, 100) },
            { Type.AtmPressure.ToString(), new Interval(0, 5) },
        };

        private static Dictionary<string, Interval> _defaults = new Dictionary<string, Interval>()
        {
            { Type.Temperature.ToString(), new Interval(15, 25) },
            { Type.Humidity.ToString(), new Interval(20, 40) },
            { Type.AtmPressure.ToString(), new Interval(0.5f, 0.65f) },
        };

        private Interval optimalValues;
        private float influence = 1.0f;

        public float Influnce { get => influence; }
        public Interval OptimalInterval { get => optimalValues; set => SetInterval(value); }

        public Gene()
        {
            optimalValues = new Interval(0.0f, 0.0f);
        }

        public Gene(Gene other)
        {
            optimalValues = new Interval(other.OptimalInterval);
            influence = other.Influnce;
        }

        public Gene(Interval values)
        {
            optimalValues = values;
        }

        public override string ToString()
        {
            return optimalValues.ToString();
        }

        public string FullString()
        {
            return optimalValues.ToString() +" of influence: " + influence.ToString();
        }

        static public Interval GetRange(string type)
        {
            return _ranges[type];
        }

        static public Gene GetDefault(string type)
        {
            return new Gene(_defaults[type]);
        }

        private void SetInterval(Interval newInterval)
        {
            optimalValues = new Interval(newInterval);
            influence = 1.0f / (optimalValues.Max - optimalValues.Min + 1);
        }

        public bool IsSuitable(float value)
        {
            return optimalValues.Compare(value);
        }
    }
}