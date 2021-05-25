using GeneticAlgorithmForSpecies.Genes;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticAlgorithmForSpecies.Equipment
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Gear", menuName = "Equipment/Gear", order = 1)]
    public class EquipmentContainer : ScriptableObject
    {
        [SerializeField] private IItem head = NullItem.Instance;
        [SerializeField] private IItem body = NullItem.Instance;
        [SerializeField] private IItem pants = NullItem.Instance;
        [SerializeField] private IItem weapon = NullItem.Instance;
        [SerializeField] private IItem armour = NullItem.Instance;

        [SerializeField] private List<IItem> runes = new List<IItem>() { NullItem.Instance };

        public void ApplyBuffs(ref GeneContainer genes)
        {
            head.ApplyBuffs(ref genes);
            body.ApplyBuffs(ref genes);
            pants.ApplyBuffs(ref genes);
            weapon.ApplyBuffs(ref genes);
            armour.ApplyBuffs(ref genes);

            foreach (IItem rune in runes)
            {
                rune.ApplyBuffs(ref genes);
            }
        }

        public void RemoveBuffs(ref GeneContainer genes)
        {
            head.RemoveBuffs(ref genes);
            body.RemoveBuffs(ref genes);
            pants.RemoveBuffs(ref genes);
            weapon.RemoveBuffs(ref genes);
            armour.RemoveBuffs(ref genes);

            foreach (IItem rune in runes)
            {
                rune.RemoveBuffs(ref genes);
            }
        }
    }
}