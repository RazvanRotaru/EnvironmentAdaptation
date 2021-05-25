using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneContainer {
    public Dictionary<string, Gene.Interval> map;
    [SerializeField] List<Gene> debugGenes;

    public GeneContainer() {
        map = new Dictionary<string, Gene.Interval>();
        debugGenes = new List<Gene>();
        
        foreach (Gene.Type type in (Gene.Type[])System.Enum.GetValues(typeof(Gene.Type))) {
            map.Add(type.ToString(), new Gene.Interval(0.0f, 0.0f));
            debugGenes.Add(new Gene(type));
        }
    }

    public GeneContainer(GeneContainer other) {
        map = new Dictionary<string, Gene.Interval>(other.map);
        foreach (KeyValuePair<string, Gene.Interval> kvp in other.map)
            map[kvp.Key] = new Gene.Interval(kvp.Value);

        debugGenes = new List<Gene>(other.debugGenes);
    }

}
