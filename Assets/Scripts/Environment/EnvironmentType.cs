using UnityEngine;

namespace GeneticAlgorithmForSpecies.Environment
{
    public class EnvironmentType : MonoBehaviour
    {
        [SerializeField] private EnvironmentAspects aspects;

        private void Awake()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.AddComponent<EnvironmentController>().SetAspects(aspects.List);
            }
        } 
    }
}