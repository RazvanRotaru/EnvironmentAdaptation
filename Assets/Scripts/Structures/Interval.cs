using UnityEngine;

namespace GeneticAlgorithmForSpecies.Structures
{
    [System.Serializable]
    public class Interval
    {
        [SerializeField] private float min = 0.0f;
        [SerializeField] private float max = 0.0f;

        public float Min { get => min; set => min = value; }
        public float Max { get => max; set => max = value; }

        public Interval() { }

        public Interval(float a, float b, bool fix = false)
        {
            if (!fix)
            {
                min = Mathf.Min(a, b);
                max = Mathf.Max(a, b);
            } 
            else
            {
                min = a;
                max = b;
            }
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
           => new Interval(a.min * b, a.max * b, fix:true);

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
            return Mathf.Min(Mathf.Abs(min - value),
                                Mathf.Abs(max - value));
        }

        public override string ToString()
        {
            return $"[{min:0.00}, {max:0.00}]";
        }
    }
}
