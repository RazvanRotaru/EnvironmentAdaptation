using GeneticAlgorithmForSpecies.Genes;
using GeneticAlgorithmForSpecies.Structures;
using GeneticAlgorithmForSpecies.Environment;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace GeneticAlgorithmForSpecies.Mutation
{
    public delegate void ActionRef<T1, T2, T3>(ref T1 genes, T2 env, T3 aux);

    public enum SelectionType
    {
        Default,
        Roulette,
        Tour,
    }

    /// <summary>
    /// This class is handling the mutation
    /// </summary>
    [System.Serializable]
    public class Mutator
    {
        [SerializeField] private readonly int populationSize;
        [SerializeField] private readonly int selectedSize;
        [SerializeField] private readonly byte mask;
        [SerializeField] private string affectedGene;

        private string Params { get => $"\nPopulation Size {populationSize}, Selected Size {selectedSize}, Mask {System.Convert.ToString(mask, 2)}"; }
        private readonly Selector _selector;
        private readonly ActionRef<List<Gene>, EnvironmentController, float> Select;

        public Mutator(int populationSize = 8, byte mask = 0b01010101, int selectedSize = 4, SelectionType selectionType = SelectionType.Roulette)
        {
            Assert.IsTrue(selectedSize >= 2 && selectedSize % 2 == 0, "Selection size should be even and greater than 2");
            Assert.IsTrue(populationSize >= 2 && populationSize % 2 == 0, "Population size should be even and greater than 2");

            this.populationSize = populationSize;
            this.selectedSize = selectedSize;
            this.mask = mask;

            _selector = new Selector(Fitness, selectedSize);
            Select = _selector.GetSelectionFunction(selectionType);
        }

        /// <summary>
        /// This function is used to select the best fitting individuals out of the population
        /// </summary>
        /// <param name="gene">The gene to be avaluated</param>
        /// <param name="envAspects">Current environment aspects</param>
        /// <returns></returns>
        private double Fitness(Gene gene, Dictionary<string, float> envAspects)
        {
            double ans = 1.0d;
            double kDiff = 2d;
            double kRange = 1d;

            double kIsAffected = 1d;

            if (envAspects.ContainsKey(affectedGene))
            {
                float envAspect = envAspects[affectedGene];
                kIsAffected = gene.OptimalInterval.Contains(envAspect) ? 100d : 1d;
                
                Interval interval = gene.OptimalInterval;
                ans = kDiff * interval.Difference(envAspect)
                        + kRange * (interval.Max - interval.Min);
            }

            return kIsAffected / ans;
        }

        /// <summary>
        /// This function returns a new value for a given seed
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private float GenerateNewValue(float seed, string type)
        {
            Interval range = Gene.GetRange(type);
            // deviation of 10 % of the range of the specific gene
            float eps = ((range.Max - range.Min) / 10);


            //return (int)seed ^ Mathf.NextPowerOfTwo((int)Random.Range(0, (1 << sizeof(byte) * 8) - 1)) / 2; // another variant
            return Mathf.Clamp(seed + Random.Range(-eps, eps), range.Min, range.Max);
        }


        /// <summary>
        /// This gene alters a given gene
        /// </summary>
        /// <param name="gene"></param>
        /// <returns></returns>
        private Gene Mutate(Gene gene)
        {
            Gene ans = new Gene(gene);

            var v1 = GenerateNewValue(ans.OptimalInterval.Min, affectedGene);
            var v2 = GenerateNewValue(ans.OptimalInterval.Max, affectedGene);
 
            ans.OptimalInterval = new Interval(v1, v2);
           
            return ans;
        }

        /// <summary>
        /// This function combines two values in relation to this mutator's mask
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="type">The type of the gene, which determines the maximum range</param>
        /// <returns></returns>
        private float Combine(float val1, float val2, string type)
        {
            Interval range = Gene.GetRange(type);
            float newVal = Mathf.Abs(val1) - Mathf.Floor(Mathf.Abs(val1)) +
                            (Mathf.FloorToInt(Mathf.Abs(val1)) & mask) +
                                (Mathf.FloorToInt(Mathf.Abs(val2)) & (byte.MaxValue ^ mask));
            return Mathf.Clamp(newVal, range.Min, range.Max);
        }

        /// <summary>
        /// This gene creates two genes by combining the given two 
        /// </summary>
        /// <param name="father"></param>
        /// <param name="mother"></param>
        /// <returns></returns>
        private (Gene first, Gene second) CrossOver(Gene father, Gene mother)
        {
            Gene child1 = new Gene(father);
            Gene child2 = new Gene(mother);


            float v1, v2;

            Interval fatherI = father.OptimalInterval;
            Interval motherI = mother.OptimalInterval;

            v1 = Combine(fatherI.Min, motherI.Min, affectedGene);
            v2= Combine(motherI.Max, fatherI.Max, affectedGene);
            child1.OptimalInterval = new Interval(v1, v2);

            v1 = Combine(motherI.Min, fatherI.Min, affectedGene);
            v2 = Combine(fatherI.Max, motherI.Max, affectedGene);
            child2.OptimalInterval = new Interval(v1, v2);

            return (first: child1, second: child2);
        }

        /// <summary>
        /// This function alters the genes in raport to this mutator's mask
        /// </summary>
        /// <param name="population">The list of genes to be altered</param>
        private void CrossOver(ref List<Gene> population)
        {
            var indices = Enumerable.Range(0, population.Count).ToList();
            int iterations = population.Count / 2;
            for (int i = 0; i < iterations; ++i)
            {
                int fatherIndex = Random.Range(0, indices.Count - 1);
                Gene father = population[indices[fatherIndex]];
                indices.RemoveAt(fatherIndex);

                int motherIndex = Random.Range(0, indices.Count - 1);
                Gene mother = population[indices[motherIndex]];
                indices.RemoveAt(motherIndex);

                var (first, second) = CrossOver(father, mother);
                population.Add(first);
                population.Add(second);
            }
        }

        /// <summary>
        /// This function adapts the player's genes in regard to this mutator's parameters
        /// </summary>
        /// <param name="geneContainer">The gene container to be altered</param>
        /// <param name="environmentController">The environment aspects of the current location</param>
        /// <param name="affectedGenes">The genes affected by the current biom</param>
        /// <param name="aux">An auxiliar parameter used by Tournament and Roullete Selections</param>
        public void Adapt(ref GeneContainer geneContainer, EnvironmentController environmentController, List<string> affectedGenes, float aux = 2)
        {
            Dictionary<string, Gene> adaptedGenes = new Dictionary<string, Gene>();
            foreach (string affectedGene in affectedGenes)
            {
                Gene currGene = geneContainer.Data[affectedGene];
                List<Gene> population = new List<Gene> { currGene };
                this.affectedGene = affectedGene;

                // Mutate N/2-1 individuals
                for (int i = 1; i < populationSize / 2; ++i)
                {
                    population.Add(Mutate(currGene));
                }
                Assert.IsTrue(population.Contains(currGene), "Initial gene has been removed" + Params);

                // CrossOver N/2 individual parents to form another N/2 individual children
                CrossOver(ref population);

                Assert.AreEqual(populationSize, population.Count, "CrossOver failed, not all individuals were used" + Params);
                Assert.IsTrue(selectedSize <= population.Count, "Cannot select from a smaller population" + Params);
                // Select K individuals from the current population
                Select(ref population, environmentController, aux);
                Assert.AreEqual(selectedSize, population.Count, "Selection failed, not all individuals were used" + Params);


                // CrossOver K individual parent to generate another K children
                CrossOver(ref population);

                // Select a random individual from the latest population of 2*K individuals
                int index = Random.Range(0, population.Count - 1);

                adaptedGenes[affectedGene] = population[index];
            }
            geneContainer = new GeneContainer(geneContainer, adaptedGenes);
        }
    }
}