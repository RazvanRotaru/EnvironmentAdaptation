using System.Collections.Generic;
using UnityEngine;
using GeneticAlgorithmForSpecies.Structures;

namespace GeneticAlgorithmForSpecies.Environment
{
    /// <summary>
    /// This class describes a biom
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Environment Aspects", menuName = "Environment/Aspects", order = 1)]
    public class EnvironmentAspects : ScriptableObject
    {
        [SerializeField]
        private List<STuple<EnvironmentController.Aspect, float>> aspects;

        void Reset()
        {
            if (aspects == null)
            {
                aspects = new List<STuple<EnvironmentController.Aspect, float>>()
                {
                    new STuple<EnvironmentController.Aspect, float>(EnvironmentController.Aspect.Temperature, 28),
                    new STuple<EnvironmentController.Aspect, float>(EnvironmentController.Aspect.Humidity, 50),
                    new STuple<EnvironmentController.Aspect, float>(EnvironmentController.Aspect.AtmPressure, 1),
                };
            }
        }

        public ValueContainer<EnvironmentController.Aspect, float> List
        {
            get
            {
                return new ValueContainer<EnvironmentController.Aspect, float>(aspects);
            }
        }
    }
}