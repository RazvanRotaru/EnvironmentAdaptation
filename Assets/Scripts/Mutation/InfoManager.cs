using GeneticAlgorithmForSpecies.Environment;
using GeneticAlgorithmForSpecies.Equipment;
using GeneticAlgorithmForSpecies.Genes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GeneticAlgorithmForSpecies.Mutation
{
    class InfoManager : MonoBehaviour
    {
        [SerializeField] private Transform playerBox;
        [SerializeField] private Transform envBox;
        [SerializeField] private Text prefabTextObj;

        [Header("References")]
        [SerializeField] private PlayerController player;
        private EquipmentController _equipment;
        private MutationController _mutation;

        private void Start()
        {
            playerBox = GameObject.Find("PlayerInfo").transform.Find("List");
            envBox = GameObject.Find("EnvironmentInfo").transform.Find("List");

            Assert.IsNotNull(player, "Player is not referenced in InfoManager");

            _equipment = player.GetComponent<EquipmentController>();
            Assert.IsNotNull(_equipment, "Player referenced in InfoManager has no EquipmentController");

            _mutation = player.GetComponent<MutationController>();
            Assert.IsNotNull(_mutation, "Player referenced in InfoManager has no MutationController");
        }

        private void LateUpdate()
        {
            ShowInfo();
        }

        private void ShowInfo()
        {
            // TODO use either an object pool or only one TextBox
            foreach (Transform child in playerBox)
                Destroy(child.gameObject);
            foreach (Transform child in envBox)
                Destroy(child.gameObject);


            prefabTextObj.text = GeneInfo();
            Instantiate(prefabTextObj, playerBox);

            prefabTextObj.text = EnvironmentInfo();
            Instantiate(prefabTextObj, envBox);
        }


        // TODO use string formatter to allign intervals in a column
        private string GeneInfo()
        {
            string text = string.Empty;
            GeneContainer genes = _mutation.Genes;
            
            _equipment.ApplyBuffs(ref genes);
            foreach (KeyValuePair<string, Gene> entry in genes.Data)
            {
                text += entry.Key.ToString() + ": " + entry.Value.ToString() + "\n";

            }
            _equipment.RemoveBuffs(ref genes);

            return text;
        }



        // TODO use string formatter to allign values in a column
        private string EnvironmentInfo()
        {
            string text = string.Empty;
            Dictionary<string, float> currEnvAspects = EnvironmentManager.Instance.GetAspects(transform.position);

            if (currEnvAspects == null)
                return text + "Unknown";

            foreach (KeyValuePair<string, float> entry in currEnvAspects)
            {
                text += entry.Key.ToString() + ": " + entry.Value.ToString() + "\n";
            }

            return text;
        }
    }
}
