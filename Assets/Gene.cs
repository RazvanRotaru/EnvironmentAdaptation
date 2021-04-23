using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gene
{
    [System.Serializable]
    public class Interval
    {
        public float min = 0.0f;
        public float max = 0.0f;

        public Interval(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public bool Contains(float value)
        {
            return value >= min && value <= max;
        }
    }

    public enum Type
    {
        Temperature,
        Humidity,
        AtmPressure,
    }

    public Type type;
    public Interval optimalValues;

    public Gene(Type type)
    {
        this.type = type;
        optimalValues = new Interval(0.0f, 0.0f);
    }

    public Gene(Type type, Interval values)
    {
        this.type = type;
        this.optimalValues.min= values.min;
        this.optimalValues.max = values.max;
    }
}
