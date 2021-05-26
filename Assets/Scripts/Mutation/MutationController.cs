using GeneticAlgorithmForSpecies.Environment;
using GeneticAlgorithmForSpecies.Equipment;
using GeneticAlgorithmForSpecies.Genes;
using GeneticAlgorithmForSpecies.Structures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticAlgorithmForSpecies.Mutation
{
    public class MutationController : CustomBehaviour<int>
    {
        [SerializeField] private Mutator mutator;
        [SerializeField] private int mutationRate = 5;
        [SerializeField] private float damageRate = 2f;
        [SerializeField] private float envDamage = 1e-01f;

        [SerializeField] private GeneContainer genes;

        [Header("References")]
        [SerializeField] private PlayerController player;
        [SerializeField] private EquipmentController equipment;

        public GeneContainer Genes { get => genes; }

        private void Start()
        {
            // TODO load genes
            genes = new GeneContainer();
            // TODO load mutator
            mutator = new Mutator();

            player = GetComponent<PlayerController>();
            equipment = GetComponent<EquipmentController>();

            Init((int x) => { return Time.time > 0 && Time.time % x == 0; }, ref mutationRate);
        }

        protected override void CustomUpdate()
        {
            HandleEnvironmnet();
        }

        private void HandleEnvironmnet()
        {
            Dictionary<string, float> currEnvAspects;
            currEnvAspects = EnvironmentManager.Instance.GetAspects(transform.position);
            if (currEnvAspects == null)
                return;

            List<string> affectedGenes = new List<string>();
            string debugText = "<b>Genes affected by <color=cyan>"
                                + EnvironmentManager.Instance.GetEnvironmentType(transform.position) + "</color></b>: ";
            foreach (KeyValuePair<string, Gene> gene in genes.Data)
            {
                if (currEnvAspects.ContainsKey(gene.Key))
                {
                    var entry = currEnvAspects[gene.Key];
                    if (!gene.Value.OptimalInterval.Compare(entry))
                    {
                        debugText += "<i><color=orange>" + gene.Key + "</color></i> ";
                        affectedGenes.Add(gene.Key);
                    }
                }
            }

            Debug.Log($"Affecteg genes {affectedGenes.Count}");
            if (affectedGenes.Count != 0)
            {
                Debug.Log(debugText);
                Mutate(EnvironmentManager.Instance.GetController(transform.position), affectedGenes);
            }
        }


        // TODO move somewhere else
        IEnumerator TakeDamage(List<string> affectedGenes)
        {


            /* foreach (var gene in affectedGenes)
             {
                 //take damage from every gene

             }*/

            // TODO use events
            player.Health -= affectedGenes.Count * envDamage;
            if (player.Health <= 0)
            {
                player.Health = 0;
                // TODO trigger player on death
                //player.OnDeath();
                yield break;
            }

            yield return new WaitForSeconds(damageRate);

            yield break;
        }


        void Mutate(EnvironmentController envController, List<string> affectedGenes)
        {
            string debugText = "<b>Mutation in progress...</b>\n";

            Dictionary<string, Gene> prevGenes = genes.DataColne;


            equipment.ApplyBuffs(ref genes);
            // mutate to be adapt to current region
            mutator.Adapt(ref genes, envController, affectedGenes);
            equipment.RemoveBuffs(ref genes);

            foreach (KeyValuePair<string, Gene> entry in genes.Data)
            {
                debugText += "<color=orange>" + entry.Key.ToString() + "</color>\t -> mutated to <color=green>" + entry.Value.ToString()
                                + "</color> from <color=red>[" + prevGenes[entry.Key].ToString() + "</color>\n";
                //Debug.Log(text);
            }
            Debug.Log(debugText);
        }
    }
}