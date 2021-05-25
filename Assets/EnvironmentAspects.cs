using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Environment Aspects", menuName = "Environment/Aspects", order = 1)]
public class EnvironmentAspects : ScriptableObject {
    public List<EnvironmentController.DebugAspect> list = new List<EnvironmentController.DebugAspect>
    {
        new EnvironmentController.DebugAspect(EnvironmentController.Aspect.Temperature, 0),
        new EnvironmentController.DebugAspect(EnvironmentController.Aspect.Humidity, 0),
        new EnvironmentController.DebugAspect(EnvironmentController.Aspect.AtmPressure, 0),
    };
}
