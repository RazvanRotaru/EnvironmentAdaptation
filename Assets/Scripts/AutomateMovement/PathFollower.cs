using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private Vector3? target = null;
    [SerializeField] private PathManager defaultPath;
    [SerializeField] private float speed = 5f;
    [SerializeField] private int currPoint = 0;

    public float WaitTime { get => defaultPath.WaitTime; set => defaultPath.WaitTime = value; }

    private void Start()
    {
        Init();
    }

    void Update()
    {
        if (target == null)
        {
            return;
        }

        Move();
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, (Vector3)target, speed * Time.deltaTime);
    }

    public void Stop()
    {
        target = null;
    }

    public void Init()
    {
        currPoint = 0;
        if (defaultPath != null)
        {
            target = defaultPath.First;
        }
    }

    public void Continue(PathManager path)
    {
        Vector3 target = path.GetNextPathPoint(currPoint++);
        this.target = target;
    }
}
