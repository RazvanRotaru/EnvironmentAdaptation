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

        private Text _playerInfo;
        private Text _envInfo;

        [Header("References")]
        [SerializeField] private PlayerController player;
        private EquipmentController _equipment;
        private MutationController _mutation;

        private void Start()
        {
            playerBox = GameObject.Find("PlayerInfo").transform.Find("List");
            _playerInfo = Instantiate(prefabTextObj, playerBox) as Text;
            envBox = GameObject.Find("EnvironmentInfo").transform.Find("List");
            _envInfo = Instantiate(prefabTextObj, envBox) as Text;

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
            _playerInfo.text = GeneInfo();
            _envInfo.text = EnvironmentInfo();
        }


        // TODO use string formatter to allign intervals in a column
        private string GeneInfo()
        {
            string text = string.Empty;
            GeneContainer genes = _mutation.Genes;
            
            _equipment.ApplyBuffs(ref genes);
            foreach (KeyValuePair<string, Gene> entry in genes.Data)
            {
                text += string.Format("{0} {1}\n", $"{entry.Key}:", entry.Value.ToString());
            }   
            _equipment.RemoveBuffs(ref genes);

            return text;
        }



        // TODO use string formatter to allign values in a column
        private string EnvironmentInfo()
        {
            string text = string.Empty;
            Dictionary<string, float> currEnvAspects = EnvironmentManager.Instance.GetAspects(player.transform.position);

            if (currEnvAspects == null)
                return text + "Unknown";

            foreach (KeyValuePair<string, float> entry in currEnvAspects)
            {
                text += string.Format("{0} {1}\n", $"{entry.Key}:", entry.Value.ToString());
            }

            return text;
        }
    }
}
