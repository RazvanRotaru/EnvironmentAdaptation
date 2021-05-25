using UnityEngine;
using GeneticAlgorithmForSpecies.Structures;

namespace GeneticAlgorithmForSpecies.Genes
{
    [System.Serializable]
    public class Gene
    {
        public enum Type
        {
            Temperature,
            Humidity,
            AtmPressure,
        }

        private Interval optimalValues;
        private float influence = 1.0f;

        public float Influnce { get => influence; }
        public Interval OptimalInterval { get => optimalValues; set => optimalValues = value; }

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
            return optimalValues.ToString() + " influence: " + influence.ToString();
        }

        //public float GetDamage(float envValue)
        //{
        //    return optimalValues.Difference(envValue) * influence;
        //}
    }
}