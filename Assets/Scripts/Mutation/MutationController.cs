using GeneticAlgorithmForSpecies.Environment;
using GeneticAlgorithmForSpecies.Equipment;
using GeneticAlgorithmForSpecies.Genes;
using GeneticAlgorithmForSpecies.Structures;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [Header("Logging")]
        [SerializeField] private string debugFile;

        public GeneContainer Genes { get => genes; }

        private void OnValidate()
        {
            //debugFile = EditorUtility.OpenFolderPanel("Select Directory", Application.dataPath, "Logs");
        }

        private void Awake()
        {
            // TODO load genes
            genes = new GeneContainer();
            // TODO load mutator
            // TODO test different mutators, will use sufix param to write different logs
            mutator = new Mutator();

            player = GetComponent<PlayerController>();
            equipment = GetComponent<EquipmentController>();
        }

        private void Start()
        {
            Init((int x) => { return Time.time > 0 && Time.time % x == 0; }, ref mutationRate);
            Debugger.RegisterLogFunction((Dictionary<string, Gene> d) => MutationLog(d), nameof(Mutate));
            Debugger.RegisterLogFunction((List<string> l) => EnvironmentLog(l), nameof(HandleEnvironmnet));
        }

        protected override void CustomUpdate()
        {
            HandleEnvironmnet();
        }

        private void HandleEnvironmnet()
        {
            Dictionary<string, float> currEnvAspects = EnvironmentManager.Instance.GetAspects(transform.position);
            if (currEnvAspects == null)
                return;

            List<string> affectedGenes = new List<string>();
            foreach (KeyValuePair<string, Gene> gene in genes.Data)
            {
                if (currEnvAspects.ContainsKey(gene.Key))
                {
                    if (!gene.Value.IsSuitable(currEnvAspects[gene.Key]))
                    {
                        affectedGenes.Add(gene.Key);
                    }
                }
            }
            
            Debugger.Log(affectedGenes);
            if (affectedGenes.Count != 0)
            {
                Mutate(EnvironmentManager.Instance.GetController(transform.position), affectedGenes);
            }
        }

        private void EnvironmentLog(List<string> affectedGenes)
        {
            string currEnv = EnvironmentManager.Instance.GetEnvironmentType(transform.position);
            string debugText = $"<b>{affectedGenes.Count} genes affected by <color=cyan>{currEnv}</color></b>: ";
            debugText += string.Join(" ", affectedGenes.Select(gene => $"<i><color=orange>{gene}</color></i>").ToArray());
            Debug.Log(debugText);

            string debugLog = string.Join(",", affectedGenes.Select(gene => gene.ToString()).ToArray());
            string debugHelp = "elapsed time(s),affect genes<list>";
            Debugger.WriteToFile(debugLog, header: debugHelp, timestamp: true);
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
            Dictionary<string, Gene> prevGenes = genes.DataColne;

            equipment.ApplyBuffs(ref genes);
            mutator.Adapt(ref genes, envController, affectedGenes);
            equipment.RemoveBuffs(ref genes);

            Debugger.Log(prevGenes);
        }

        private void MutationLog(Dictionary<string, Gene> prev)
        {
            string debugText = "<b>Mutation in progress...</b>\n";
            debugText += string.Join("\n", genes.Data.Select(kv => $"<color=orange></color>{kv.Key}\t-> mutated from " +
                                                    $"<color=red>{prev[kv.Key]}</color> to <color=green>{kv.Value}</color>").ToArray());
            Debug.Log(debugText);

            string debugLog = string.Join(",", genes.Data.Keys.Select(key => key.ToString()).ToArray());
            debugLog += string.Join(",", genes.Data.Values.Select(value => value.ToString()).ToArray());
            debugLog += string.Join(",", prev.Values.Select(value => value.ToString()).ToArray());
            string debugHelp = "elapsed time(s),gene type,current value,previous value\n";
            Debugger.WriteToFile(debugLog, header: debugHelp, timestamp: true);
        }
    }
}