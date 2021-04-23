using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneContainer
{
    public Dictionary<Gene.Type, Gene.Interval> map;
    [SerializeField] List<Gene> debugGenes;
    //public Gene temperature;
    //public Gene atmPressure;
    //public Gene humidity;

    public GeneContainer()
    {
        map = new Dictionary<Gene.Type, Gene.Interval>();
        debugGenes = new List<Gene>();
        //temperature = new Gene(Gene.Type.Temperature);
        //atmPressure = new Gene(Gene.Type.AtmPressure);
        //humidity = new Gene(Gene.Type.Humidity);
        foreach (Gene.Type type in (Gene.Type[])System.Enum.GetValues(typeof(Gene.Type)))
        {
            map.Add(type, new Gene.Interval(0.0f, 0.0f));
            debugGenes.Add(new Gene(type));
        }
    }
}
