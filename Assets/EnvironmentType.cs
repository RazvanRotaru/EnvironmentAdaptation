using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentType : MonoBehaviour
{
    [SerializeField] public EnvironmentAspects aspects;

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.AddComponent<EnvironmentController>().SetAsepcts(aspects.list);
        }
    }
}
