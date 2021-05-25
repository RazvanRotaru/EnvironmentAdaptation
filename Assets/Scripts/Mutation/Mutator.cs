using GeneticAlgorithmForSpecies.Genes;
using GeneticAlgorithmForSpecies.Structures;
using GeneticAlgorithmForSpecies.Environment;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace GeneticAlgorithmForSpecies.Mutation
{
    [System.Serializable]
    public class Mutator
    {
        [SerializeField] private readonly int populationSize;
        [SerializeField] private readonly int selectedSize;
        [SerializeField] private readonly byte mask;
        [SerializeField] private List<string> affectedGenes;

        public Mutator(int populationSize = 8, string mask = "", int selectedSize = 2)
        {
            this.populationSize = populationSize;
            this.selectedSize = selectedSize;

            if (mask.Equals(""))
                this.mask = (1 << sizeof(byte) * 4 /* *8/2 */) - 1; // 15 = b00001111
            else
                this.mask = System.Convert.ToByte(mask);
        }

        private float Fitness(GeneContainer geneContainer, Dictionary<string, float> envAspects)
        {
            float ans = 1.0f;

            foreach (var affectedGene in affectedGenes)
            {
                if (envAspects.ContainsKey(affectedGene))
                {
                    ans += geneContainer.GetGene(affectedGene).OptimalInterval.Difference(envAspects[affectedGene]);
                }
            }

            return 1.0f / ans;
        }

        private GeneContainer Mutate(GeneContainer geneContainer)
        {
            GeneContainer ans = new GeneContainer(geneContainer);

            foreach (var affectedGene in affectedGenes)
            {
                //var val = ans.map[gene];

                //int minBit = Mathf.NextPowerOfTwo((int)Random.Range(0, (1 << sizeof(byte) * 8) - 1)) / 2;
                //int maxBit = Mathf.NextPowerOfTwo((int)Random.Range(0, (1 << sizeof(byte) * 8) - 1)) / 2;
                //val.min = (int)val.min ^ minBit;
                //val.max = (int)val.max ^ maxBit;

                var v1 = Mathf.Clamp(ans.GetGene(affectedGene).OptimalInterval.Min + Random.Range(-5, 5), 0, byte.MaxValue);
                var v2 = Mathf.Clamp(ans.GetGene(affectedGene).OptimalInterval.Max + Random.Range(-5, 5), 0, byte.MaxValue);
                ans.GetGene(affectedGene).OptimalInterval.Min = Mathf.Min(v1, v2);
                ans.GetGene(affectedGene).OptimalInterval.Max = Mathf.Max(v1, v2);
                //ans.map[gene] = val;
            }

            return ans;
        }

        private float Combine(float val1, float val2)
        {
            return val1 - (int)val1 + ((int)val1 & mask) +
                    ((int)val2 & (byte.MaxValue ^ mask));
        }

        private (GeneContainer first, GeneContainer second) CrossOver(GeneContainer father, GeneContainer mother)
        {
            GeneContainer child1 = new GeneContainer(father);
            GeneContainer child2 = new GeneContainer(mother);


            foreach (var affectedGene in affectedGenes)
            {
                Interval fatherI = new Interval(father.GetGene(affectedGene).OptimalInterval);
                Interval motherI = new Interval(mother.GetGene(affectedGene).OptimalInterval);

                child1.GetGene(affectedGene).OptimalInterval.Min = Combine(fatherI.Min, motherI.Min);
                child1.GetGene(affectedGene).OptimalInterval.Max = Combine(motherI.Max, fatherI.Max);

                child2.GetGene(affectedGene).OptimalInterval.Min = Combine(motherI.Min, fatherI.Min);
                child2.GetGene(affectedGene).OptimalInterval.Max = Combine(fatherI.Max, motherI.Max);
            }

            return (first: child1, second: child2);
        }

        private void CrossOver(ref List<GeneContainer> population)
        {
            var indices = Enumerable.Range(0, population.Count).ToList();
            int iterations = population.Count / 2;
            for (int i = 0; i < iterations; ++i)
            {
                int fatherIndex = Random.Range(0, indices.Count - 1);
                GeneContainer father = population[indices[fatherIndex]];
                indices.RemoveAt(fatherIndex);

                int motherIndex = Random.Range(0, indices.Count - 1);
                GeneContainer mother = population[indices[motherIndex]];
                indices.RemoveAt(motherIndex);

                var (first, second) = CrossOver(father, mother);
                population.Add(first);
                population.Add(second);
            }
        }

        private List<(GeneContainer gene, float f)> RankFitness(List<GeneContainer> geneContainers, EnvironmentController environmentController, float SP)
        {
            Assert.IsTrue(1 < SP && SP <= 2f, "SP is out of range. Please use SP in range of (1, 2]");
            // TODO test
            Select(ref geneContainers, environmentController);
            List<(GeneContainer gene, float f)> ans = new List<(GeneContainer, float)>();

            float totalF = 0f;
            for (int i = 0; i < geneContainers.Count; ++i)
            {
                float val = 2 - SP + 2 * (SP - 1) * (i - 1) / (geneContainers.Count - 1);
                totalF += val;
                ans.Add((gene: geneContainers[i], f: val));
            }
            for (int i = 0; i < ans.Count; ++i)
            {
                ans[i] = (ans[i].gene, f: ans[i].f / totalF);
            }
            return ans;
        }

        private void TourSelect(ref List<GeneContainer> geneContainers, EnvironmentController environmentController, int Tour)
        {
            Assert.IsTrue(2 <= Tour && Tour <= geneContainers.Count, "Tour is out of range. Please use Tour in range of (2, N - 1]");
            // TODO test
            List<GeneContainer> ans = new List<GeneContainer>();
            for (int i = 0; i < selectedSize; ++i)
            {
                List<int> currTourIndices = new List<int>();
                while (currTourIndices.Count < Tour)
                {
                    int index = Random.Range(0, geneContainers.Count - 1);
                    while (currTourIndices.Contains(index))
                    {
                        index = Random.Range(0, geneContainers.Count - 1);
                    }
                }

                List<GeneContainer> currTour = new List<GeneContainer>();
                foreach (var index in currTourIndices)
                {
                    currTour.Add(geneContainers[index]);
                }

                Select(ref currTour, environmentController);
                ans.Add(currTour[0]);
            }
            geneContainers = ans;
        }

        private void RouletteSelect(ref List<GeneContainer> geneContainers, EnvironmentController environmentController, float SP)
        {
            // TODO test
            List<float> values = new List<float>();
            for (int i = 0; i < selectedSize; ++i)
            {
                float val = (float)Random.Range(0, 100) / 100;
                values.Add(val);
            }

            var rankedGenes = RankFitness(geneContainers, environmentController, SP);

            List<GeneContainer> ans = new List<GeneContainer>();
            foreach (float val in values)
            {
                foreach (var (gene, f) in rankedGenes)
                {
                    if (val < f)
                    {
                        ans.Add(gene);
                        break;
                    }
                }
            }

            geneContainers = ans;
        }

        private void Select(ref List<GeneContainer> geneContainers, EnvironmentController environmentController)
        {
            List<(GeneContainer gene, float fitness)> sortedGenes = new List<(GeneContainer, float)>();
            var envAspects = environmentController.GetAspects();
            foreach (var gene in geneContainers)
            {
                sortedGenes.Add((gene, Fitness(gene, envAspects)));
            }
            sortedGenes.Sort((elm1, elm2) => elm2.fitness.CompareTo(elm1.fitness));

            geneContainers.Clear();
            for (int i = 0; i < selectedSize; ++i)
                geneContainers.Add(sortedGenes[i].gene);
        }

        public void Adapt(ref GeneContainer geneContainer, EnvironmentController environmentController, List<string> affectedGenes)
        {
            List<GeneContainer> population = new List<GeneContainer> { geneContainer };
            //PrintGenes(population, "Inital gene");
            this.affectedGenes = affectedGenes;

            // Mutate N/2-1 individuals
            for (int i = 1; i < populationSize / 2; ++i)
            {
                population.Add(Mutate(geneContainer));
            }
            //PrintGenes(population, "Mutated genes");
            Assert.IsTrue(population.Contains(geneContainer), "Initial gene has been removed");

            // CrossOver N/2 individual parents to form another N/2 individual children
            CrossOver(ref population);
            //PrintGenes(population, "CrossedOver genes");
            Assert.AreEqual(populationSize, population.Count, "CrossOver failed, not all individuals were used");

            // Select K individuals from the current population
            Select(ref population, environmentController);
            //PrintGenes(population, "Selected genes");
            Assert.AreEqual(selectedSize, population.Count, "Selection failed, not all individuals were used");

            // CrossOver K individual parent to generate another K children
            CrossOver(ref population);
            //PrintGenes(population, "CrossedOver Selected genes");

            // Select a random individual from the latest population of 2*K individuals
            int index = Random.Range(0, population.Count - 1);
            //Debug.Log("Selected index: " + index);

            geneContainer = population[index];
        }

        private void PrintGenes(List<GeneContainer> geneContainers, string text)
        {
            text += " " + geneContainers.Count + " elements: ";
            foreach (var genes in geneContainers)
            {
                text += "{";
                foreach (KeyValuePair<string, Gene> entry in genes.Data)
                {
                    text += entry.Key.ToString() + ": " + entry.Value.ToString();
                }
                text += "} ---- ";
            }

            Debug.Log(text);
        }
    }
}