using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [System.Serializable]
    public class DebugEnvController
    {
        public Vector3 position;
        public EnvironmentController controller;

        public DebugEnvController(Vector3 position, EnvironmentController controller)
        {
            this.position = position;
            this.controller = controller;
        }
    }

    public static EnvironmentManager instance;

    Dictionary<Vector3, EnvironmentController> controllers;
    [SerializeField] List<DebugEnvController> debugControllers;
    [SerializeField] float chunkSize = 50;

    private void Start()
    {
        if (instance != null)
            return;

        instance = this;

        controllers = new Dictionary<Vector3, EnvironmentController>();
        debugControllers = new List<DebugEnvController>();
        EnvironmentController[] envControllers = FindObjectsOfType<EnvironmentController>();
        foreach (EnvironmentController envController in envControllers)
        {
            if (controllers.ContainsKey(envController.transform.position))
                continue;
            controllers.Add(envController.transform.position, envController);
            debugControllers.Add(new DebugEnvController(envController.transform.position, envController));
        }
    }

    public Vector3 GetChunkOrigin(Vector3 playerPos)
    {
        return new Vector3(Mathf.Floor(playerPos.x / chunkSize), 0, Mathf.Floor(playerPos.z / chunkSize)) * chunkSize;
    }

    public EnvironmentController GetController(Vector3 playerPos)
    {
        EnvironmentController c = controllers[GetChunkOrigin(playerPos)];
        return controllers[GetChunkOrigin(playerPos)];
    }

    public Dictionary<EnvironmentController.Aspect, float> GetAspects(Vector3 playerPos)
    {
        Dictionary<EnvironmentController.Aspect, float> kvp = GetController(playerPos).GetAspects();
        return GetController(playerPos).GetAspects();
    }

    public string GetEnvironmentType(Vector3 playerPos)
    {
        return GetController(playerPos).transform.parent.name;
    }
}
