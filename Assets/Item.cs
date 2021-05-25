using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item", order = 1)]
public class Item : ScriptableObject {
    public enum ItemType {
        Wearable,
        Defensive,
        Offensive,
        Rune,
    }

    public ItemType type;
    new public string name;
    public float value;

    public Buffs buffs;
}
