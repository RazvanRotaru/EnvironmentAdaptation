using UnityEngine;

namespace GeneticAlgorithmForSpecies.Environment
{
    public class EnvironmentType : MonoBehaviour
    {
        [SerializeField] private EnvironmentAspects aspects;

        private void Awake()
        {
            if (transform.childCount == 1)
            {
                Debug.Log(name);
            }

            foreach (Transform child in transform)
            {
                child.gameObject.AddComponent<EnvironmentController>().SetAspects(aspects.List);
            }
        } 
    }
}