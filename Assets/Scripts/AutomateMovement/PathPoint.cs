using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint : MonoBehaviour
{
    [SerializeField] private PathManager pathManager;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetManager(PathManager pathManager)
    {
        this.pathManager = pathManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        PathFollower player = other.gameObject.GetComponent<PathFollower>();
        if (player == null)
        {
            return;
        }

        player.Stop();
        StartCoroutine(nameof(WaitAndContinue), player);
    }

    IEnumerator WaitAndContinue(PathFollower player)
    {
        yield return new WaitForSeconds(pathManager.WaitTime);
        player.Continue(pathManager);
    }
}
