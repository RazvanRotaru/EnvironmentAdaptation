using UnityEngine;

[System.Serializable]
public class Gene {
    [System.Serializable]
    public class Interval {
        public float min = 0.0f;
        public float max = 0.0f;

        public Interval(float min, float max) {
            this.min = min;
            this.max = max;
        }

        public Interval(Interval other) {
            min = other.min;
            max = other.max;
        }

        public static Interval operator +(Interval a, float b)
            => new Interval(a.min + b, a.max + b);

        public bool Contains(float value) {
            return value >= min && value <= max;
        }

        public bool Compare(float value)
        {
            return value == min && value == max;
        }

        public float Difference(float value) {
            return Mathf.Sqrt(Mathf.Pow(min - value, 2) +
                                Mathf.Pow(max - value, 2));
        }
    }

    public enum Type {
        Temperature,
        Humidity,
        AtmPressure,
    }

    public Type type;
    public Interval optimalValues;
    private float influence = 1.0f;

    public float Influnce { get => influence; }


    public Gene(Type type) {
        this.type = type;
        optimalValues = new Interval(0.0f, 0.0f);
    }

    public Gene(Gene other) {
        type = other.type;
        optimalValues = new Interval(other.optimalValues);
    }

    public Gene(Type type, Interval values) {
        this.type = type;
        this.optimalValues.min = values.min;
        this.optimalValues.max = values.max;
    }

    public float GetDamage(float envValue)
    {
        return optimalValues.Difference(envValue) * influence;
    }
}
