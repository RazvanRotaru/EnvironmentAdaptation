using GeneticAlgorithmForSpecies.Genes;

using UnityEngine;

namespace GeneticAlgorithmForSpecies.Equipment
{
    /// <summary>
    /// This class handles the equipment of a character
    /// </summary>
    class EquipmentController : MonoBehaviour
    {
        [SerializeField] private EquipmentContainer equipmentContainer;

        private void Start()
        {
            if (equipmentContainer == null)
            {
                equipmentContainer = new EquipmentContainer();
            }
            else
            {
                equipmentContainer.Init();
            }
        }

        public void ApplyBuffs(ref GeneContainer geneContainer)
        {
            equipmentContainer.ApplyBuffs(ref geneContainer);
        }

        public void RemoveBuffs(ref GeneContainer geneContainer)
        {
            equipmentContainer.RemoveBuffs(ref geneContainer);
        }
    }
}
