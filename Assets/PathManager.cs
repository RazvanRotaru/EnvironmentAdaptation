using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField] private List<PathPoint> pathPoints;
    [SerializeField] private float waitTime = 5f;

    private void Awake()
    {
        pathPoints = new List<PathPoint>(GetComponentsInChildren<PathPoint>());

        foreach (var point in pathPoints)
        {
            point.SetManager(this);
        }
    }

    public Vector3 First { get => pathPoints[0].transform.position; }
    public float WaitTime { get => waitTime; set => waitTime = value; }

    public Vector3 GetNextPathPoint(int currPoint)
    {
        int nextPoint = (currPoint + 1) % pathPoints.Count;
        return pathPoints[nextPoint].transform.position;
    }
}
