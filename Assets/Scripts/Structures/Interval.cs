using UnityEngine;

namespace GeneticAlgorithmForSpecies.Structures
{
    [System.Serializable]
    public class Interval
    {
        private float min = 0.0f;
        private float max = 0.0f;

        public float Min { get => min; set => min = value; }
        public float Max { get => max; set => max = value; }

        public Interval(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public Interval(Interval other)
        {
            min = other.min;
            max = other.max;
        }

        public static Interval operator +(Interval a, float b)
            => new Interval(a.min + b, a.max + b);

        public static Interval operator +(Interval a, Interval b)
            => new Interval(a.min + b.min, a.max + b.max);

        public static Interval operator *(Interval a, float b)
           => new Interval(a.min * b, a.max * b);

        public bool Contains(float value)
        {
            return value >= min && value <= max;
        }

        public bool Compare(float value)
        {
            return value == min && value == max;
        }

        public float Difference(float value)
        {
            return Mathf.Sqrt(Mathf.Pow(min - value, 2) +
                                Mathf.Pow(max - value, 2));
        }

        public override string ToString()
        {
            return "[" + min + ", " + max + "]";
        }
    }
}
