using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Environment Aspects", menuName = "Environment/Aspects", order = 1)]
public class EnvironmentAspects : ScriptableObject
{
    public List<EnvironmentController.DebugAspect> list;
}
