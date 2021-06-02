using GeneticAlgorithmForSpecies.Genes;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticAlgorithmForSpecies.Equipment
{
    /// <summary>
    /// This class desribes an equipment
    /// </summary>
    [System.Serializable]
    public class EquipmentContainer
    {
        [SerializeField] private Wearable head = null; 
        [SerializeField] private Wearable body = null;
        [SerializeField] private Wearable pants = null;
        [SerializeField] private Weapon weapon = null;
        [SerializeField] private Wearable armour = null;

        [SerializeField] private List<Rune> runes = new List<Rune>() { };
        
        public void Init()
        {
            if (head != null) head.Init();
            if (body != null) body.Init();
            if (pants != null) pants.Init();
            if (weapon != null) weapon.Init();
            if (armour != null) armour.Init();

            foreach (Rune rune in runes)
            {
                rune.Init();
            }
        }

        public void ApplyBuffs(ref GeneContainer genes)
        {
            if (head != null)   head.ApplyBuffs(ref genes);
            if (body != null)   body.ApplyBuffs(ref genes);
            if (pants != null)  pants.ApplyBuffs(ref genes);
            if (weapon != null) weapon.ApplyBuffs(ref genes);
            if (armour != null) armour.ApplyBuffs(ref genes);

            foreach (Rune rune in runes)
            {
                rune.ApplyBuffs(ref genes);
            }
        }

        public void RemoveBuffs(ref GeneContainer genes)
        {
            if (head != null)   head.RemoveBuffs(ref genes);
            if (body!= null)    body.RemoveBuffs(ref genes);
            if (pants != null)  pants.RemoveBuffs(ref genes);
            if (weapon != null) weapon.RemoveBuffs(ref genes);
            if (armour != null) armour.RemoveBuffs(ref genes);

            foreach (Rune rune in runes)
            {
                rune.RemoveBuffs(ref genes);
            }
        }
    }
}