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
    public partial class MutationController : CustomBehaviour<int>
    {
        [SerializeField] private Mutator mutator;
        [SerializeField] private int mutationRate = 5;
        [SerializeField] private float damageRate = 2f;
        [SerializeField] private float envDamage = 1e-01f;

        [SerializeField] private GeneContainer genes;
        private string _subdir = string.Empty;
        private int _generation = 0;

        [Header("References")]
        [SerializeField] private PlayerController player;
        [SerializeField] private EquipmentController equipment;
        [SerializeField] private PathFollower follower;

        [Header("Logging")]
        [SerializeField] private string debugFile;

        public GeneContainer Genes { get => genes; }

        private void Awake()
        {
            // TODO load genes
            genes = new GeneContainer();
            // TODO load mutator
            // TODO test different mutators, will use sufix param to write different logs
            mutator = new Mutator();

            player = GetComponent<PlayerController>();
            equipment = GetComponent<EquipmentController>();
            follower = GetComponent<PathFollower>();
        }

        private void Start()
        {
            StartCoroutine(nameof(TestCoroutine));

            Init((int x) => { return Time.time > 0 && Time.time % x == 0; }, ref mutationRate);
            Debugger.RegisterLogFunction(((Dictionary<string, Gene> p, Dictionary<string, Gene> b) t) => MutationLog(t.p, t.b), nameof(Mutate));
            Debugger.RegisterLogFunction((List<string> l) => EnvironmentLog(l), nameof(HandleEnvironmnet));
            EnvironmentManager.OnEnvironmentChange += EvaluateLog;
        }

        private void Update()
        {

        }

        protected override void CustomUpdate()
        {
            HandleEnvironmnet();
            _generation++;
        }

        /// <summary>
        /// This function marks the genes to be adapted, taking in consideration the current biom
        /// </summary>
        private void HandleEnvironmnet()
        {
            List<string> affectedGenes = GetAffectedGenes();

            Debugger.Log(affectedGenes);
            if (affectedGenes.Count != 0)
            {
                Mutate(EnvironmentManager.Instance.GetController(transform.position), affectedGenes);
            }
        }


        /// <summary>
        /// This function checks if the genes are fitted for the curent environmental conditions
        /// </summary>
        /// <returns>The unfitted genes</returns>
        public List<string> GetAffectedGenes()
        {
            Dictionary<string, float> currEnvAspects = EnvironmentManager.Instance.GetAspects(transform.position);
            if (currEnvAspects == null)
                return new List<string>();

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

            return affectedGenes;
        }

        private void EnvironmentLog(List<string> affectedGenes)
        {
            string currEnv = EnvironmentManager.Instance.GetEnvironmentType(transform.position);
            string debugText = $"<b>{affectedGenes.Count} genes affected by <color=cyan>{currEnv}</color></b>: ";
            debugText += string.Join(" ", affectedGenes.Select(gene => $"<i><color=orange>{gene}</color></i>").ToArray());
            Debug.Log(debugText);

            string debugLog = string.Join(",", affectedGenes.Select(gene => gene.ToString()).ToArray());
            string debugHelp = "affect genes<list>";
            Debugger.WriteToFile(debugLog, header: debugHelp, timestamp: true, subdir: _subdir);
        }


        /// <summary>
        /// This function mutates the genes in order to adapt to the current biom.
        /// </summary>
        /// <param name="envController"></param>
        /// <param name="affectedGenes"></param>
        void Mutate(EnvironmentController envController, List<string> affectedGenes)
        {
            Dictionary<string, Gene> prevGenes = genes.DataColne;
            Dictionary<string, Gene> buffedGenes;

            equipment.ApplyBuffs(ref genes);
            buffedGenes = genes.DataColne;
            mutator.Adapt(ref genes, envController, affectedGenes);
            equipment.RemoveBuffs(ref genes);

            Debugger.Log((prevGenes, buffedGenes));
        }

        private void MutationLog(Dictionary<string, Gene> prev, Dictionary<string, Gene> buffed)
        {
            string debugText = "<b>Mutation in progress...</b>\n";
            debugText += string.Join("\n", genes.Data.Select(kv =>
                                                    $"<color=orange></color>{kv.Key}\t\t" +
                                                    $"(buffed to <color=yellow>{buffed[kv.Key]}</color>)\t\t" +
                                                    $"-> mutated from <color=red>{prev[kv.Key]}</color>\t\t" +
                                                    $" to <color=green>{kv.Value}</color>").ToArray());
            Debug.Log(debugText);

            string debugLog, debugHelp;
            foreach (string type in genes.Data.Keys)
            {
                debugLog = $"{_generation},{genes.Data[type].OptimalInterval.Min},{genes.Data[type].OptimalInterval.Max}," +
                                $"{prev[type].OptimalInterval.Min},{prev[type].OptimalInterval.Max}," +
                                $"{buffed[type].OptimalInterval.Min},{buffed[type].OptimalInterval.Max}";
                debugHelp = "generation,current_value_min,current_value_max,previous_value_min," +
                                "previous_value_max,buffed_value_min,buffed_value_max";
                Debugger.WriteToFile(debugLog, header: debugHelp, subdir: _subdir, sufix: type, fileType: ".csv");
            }
        }

        private void EvaluateLog()
        {
            string debugLog, debugHelp;
            
            debugLog = string.Empty;
            debugHelp = string.Empty;

            foreach (string type in genes.Data.Keys)
            {
                debugLog += $"{genes.Data[type].OptimalInterval.Max - genes.Data[type].OptimalInterval.Min},";
                debugHelp += $"{type},";
            }
            Debugger.WriteToFile(debugLog, header: debugHelp, subdir: _subdir, sufix: "differences", fileType: ".csv");
    }
    }
}