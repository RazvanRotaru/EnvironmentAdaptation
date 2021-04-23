using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    [System.Serializable]
    public class DebugAspect
    {
        public Aspect aspect;
        public float value;

        public DebugAspect(Aspect aspect, float value)
        {
            this.aspect = aspect;
            this.value = value;
        }
    }

    public enum Aspect
    {
        Temperature,
        Humidity,
        AtmPressure,
    }

    private Dictionary<Aspect, float> aspects;
    [SerializeField] List<DebugAspect> debugAsepcts;

    private void Start()
    {
        if (debugAsepcts == null)
        {
            debugAsepcts = new List<DebugAspect>();
            aspects = new Dictionary<Aspect, float>();
            Debug.LogError("Aspects not loaded for " + name);
            foreach (Aspect aspect in (Aspect[])System.Enum.GetValues(typeof(Aspect)))
            {
                aspects.Add(aspect, 0.0f);
                debugAsepcts.Add(new DebugAspect(aspect, 0.0f));
            }
        }

        aspects = new Dictionary<Aspect, float>();
        foreach (DebugAspect aspect in debugAsepcts)
        {
            aspects.Add(aspect.aspect, aspect.value);
        }
    }

    public void SetAsepcts(List<DebugAspect> aspects)
    {
        debugAsepcts = aspects;
    }
    public Dictionary<Aspect, float> GetAspects() => aspects;
}

