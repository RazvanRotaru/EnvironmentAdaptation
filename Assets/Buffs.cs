using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Buffs
{
    [SerializeField]
    Dictionary<Gene.Type, float> buffs = new Dictionary<Gene.Type, float>();

    public bool HasBuff(Gene.Type type) => buffs.ContainsKey(type);
    public void AddBuff(Gene.Type type, float value) => buffs[type] += value;
    public float GetBuff(Gene.Type type) => buffs[type];
}
