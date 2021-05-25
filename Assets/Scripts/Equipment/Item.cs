using UnityEngine;
using GeneticAlgorithmForSpecies.Genes;

namespace GeneticAlgorithmForSpecies.Equipment
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Item", menuName = "Equipment/Item", order = 1)]
    public class Item : ScriptableObject, IItem
    {
        public enum ItemType
        {
            Wearable,
            Defensive,
            Offensive,
            Rune,
        }

        [SerializeField] private ItemType type;
        [SerializeField] private new string name;
        [SerializeField] private float price;

        [SerializeField] private Buffs buffs;

        public void ApplyBuffs(ref GeneContainer geneContainer) => buffs.ApplyBuffs(ref geneContainer);
        public void RemoveBuffs(ref GeneContainer geneContainer) => buffs.RemoveBuffs(ref geneContainer);
    }
}