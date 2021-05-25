using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private Vector3? target = null;
    [SerializeField] private Vector3 dir = Vector3.zero;
    [SerializeField] private PathManager defaultPath;
    [SerializeField] private float speed = 5f;
    [SerializeField] private int currPoint = 0;

    private void Start()
    {
        if (defaultPath != null)
        {
            target = defaultPath.First;
            dir = (Vector3)(target - transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            return;
        }

        string debugText = "Moving from " + transform.position.ToString();
        Move();
        debugText += " to " + transform.position.ToString() + " having as target " + target.ToString();
        //Debug.Log(debugText);
    }

    private void ThrowYourself()
    {
        transform.position = Vector3.Lerp(transform.position, (Vector3)target, speed * Time.deltaTime);
    }

    private void Move()
    {
        transform.position += dir * speed / 10 * Time.deltaTime;
    }

    public void Stop()
    {
        target = null;
    }

    public void Continue(PathManager path)
    {
        Vector3 target = path.GetNextPathPoint(currPoint++);
        this.target = target;
        dir = target - transform.position;
    }
}
