using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Buffs {
    [SerializeField]
    Dictionary<string, float> buffs = new Dictionary<string, float>();

    public bool HasBuff(string type) => buffs.ContainsKey(type);
    public void AddBuff(string type, float value) => buffs[type] += value;
    public float GetBuff(string type) => buffs[type];
}
