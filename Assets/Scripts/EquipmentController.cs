using GeneticAlgorithmForSpecies.Genes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticAlgorithmForSpecies.Equipment
{
    class EquipmentController : MonoBehaviour
    {
        [SerializeField] private EquipmentContainer equipmentContainer;

        private void Start()
        {
            if (equipmentContainer == null)
            {
                equipmentContainer = ScriptableObject.CreateInstance<EquipmentContainer>();
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
