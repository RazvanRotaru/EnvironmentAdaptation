using UnityEngine;

namespace GeneticAlgorithmForSpecies.Environment
{
    /// <summary>
    /// This class handles a certain environment type
    /// </summary>
    /// <remarks>
    /// This class will add a <c>EnvironmentController</c> component to it's children
    /// </remarks>
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