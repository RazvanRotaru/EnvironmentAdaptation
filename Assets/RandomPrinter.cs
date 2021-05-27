using GeneticAlgorithmForSpecies.Structures;
using UnityEngine;

class RandomPrinter : CustomBehaviour<int>
{
    private void Start()
    {
        int deltaTime = 1;
        Init((int x) => { return Time.time % x == 0; }, ref deltaTime);
        Debugger.RegisterLogFunction((object x) => { Debug.Log($"is <color=red>{x}</color>"); }, GetType());
        Debugger.RegisterLogFunction((object x) => { Debug.Log($"this is also <color=red>{x}</color>"); }, nameof(CustomUpdate));
    }

    protected override void CustomUpdate()
    {
        //Debugger.Log("working");
    }
}
