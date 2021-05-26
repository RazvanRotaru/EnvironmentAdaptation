using GeneticAlgorithmForSpecies.Structures;
using UnityEngine;

class RandomPrinter : CustomBehaviour<int>
{
    private void Start()
    {
        int deltaTime = 1;
        Init((int x) => { return Time.time % x == 0; }, ref deltaTime);
        Debugger.SetLogFunction((object x) => { Debug.Log($"<color=red>{x}</color>"); });
    }

    protected override void CustomUpdate()
    {
        Debugger.Log("working");
    }
}
