using System.Collections.Generic;
using UnityEngine;
using GeneticAlgorithmForSpecies.Structures;

namespace GeneticAlgorithmForSpecies.Environment
{
    /// <summary>
    /// This class handles a biom
    /// </summary>
    public class EnvironmentController : MonoBehaviour
    {
        public enum Aspect
        {
            Temperature,
            Humidity,
            AtmPressure,
        }

        [SerializeField] private ValueContainer<Aspect, float> aspects;

        private void Start()
        {
            if (aspects == null)
            {
                aspects = new ValueContainer<Aspect, float>();
                Debug.LogError("Aspects not loaded for " + name);
                foreach (Aspect aspect in (Aspect[])System.Enum.GetValues(typeof(Aspect)))
                {
                    aspects[aspect] = 0.0f;
                }
            }
        }

        public void SetAspects(ValueContainer<Aspect, float> aspects)
        {
            this.aspects = aspects;
        }

        public Dictionary<string, float> GetAspects() => aspects.Data;
    }
}