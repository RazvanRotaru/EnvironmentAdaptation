using UnityEngine;
using GeneticAlgorithmForSpecies.Genes;

namespace GeneticAlgorithmForSpecies.Equipment
{
    /// <summary>
    /// This class describes an item
    /// </summary>
    [System.Serializable]
    public class Item : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private float price;

        [SerializeField] private Buffs buffs;

        public void Init()
        {
            if (buffs != null)
            {
                buffs.Init();
            }    
        }

        void Reset()
        {
            if (buffs == null)
            {
                buffs = new Buffs();
            }
        }

        public void ApplyBuffs(ref GeneContainer geneContainer) => buffs.ApplyBuffs(ref geneContainer);
        public void RemoveBuffs(ref GeneContainer geneContainer) => buffs.RemoveBuffs(ref geneContainer);
    }
}