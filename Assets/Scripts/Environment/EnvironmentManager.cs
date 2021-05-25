using System.Collections.Generic;
using UnityEngine;
using GeneticAlgorithmForSpecies.Structures;

namespace GeneticAlgorithmForSpecies.Environment
{
    public class EnvironmentManager : MonoBehaviour
    {
        private static EnvironmentManager _instance;
        public static EnvironmentManager Instance { get => _instance; }

        [SerializeField] private ValueContainer<Vector3, EnvironmentController> controllers;
        [SerializeField] private readonly float chunkSize = 50;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }

        }

        private void Start()
        {
            controllers = new ValueContainer<Vector3, EnvironmentController>();

            EnvironmentController[] envControllers = FindObjectsOfType<EnvironmentController>();
            foreach (EnvironmentController envController in envControllers)
            {
                if (controllers.ContainsKey(envController.transform.position))
                    continue;

                controllers[envController.transform.position] = envController;
            }
        }

        public Vector3 GetChunkOrigin(Vector3 playerPos)
        {
            return new Vector3(Mathf.Floor(playerPos.x / chunkSize), 0, Mathf.Floor(playerPos.z / chunkSize)) * chunkSize;
        }

        public EnvironmentController GetController(Vector3 playerPos)
        {
            Vector3 pos = GetChunkOrigin(playerPos);

            if (controllers.ContainsKey(pos))
                return controllers[pos];
            return null;
        }

        public Dictionary<string, float> GetAspects(Vector3 playerPos)
        {
            EnvironmentController ec = GetController(playerPos);

            return ec == null ? null : ec.GetAspects();
        }

        public string GetEnvironmentType(Vector3 playerPos)
        {
            EnvironmentController ec = GetController(playerPos);

            return ec == null ? null : ec.transform.parent.name;
        }
    }
}