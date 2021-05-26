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

    [System.Serializable]
    public class Mutator
    {
        [SerializeField] private readonly int populationSize;
        [SerializeField] private readonly int selectedSize;
        [SerializeField] private readonly byte mask;
        [SerializeField] private List<string> affectedGenes;

        private readonly Selector _selector;
        private readonly ActionRef<List<GeneContainer>, EnvironmentController, float> Select;

        public Mutator(int populationSize = 8, string mask = "", int selectedSize = 2, SelectionType selectionType = SelectionType.Roulette)
        {
            this.populationSize = populationSize;
            this.selectedSize = selectedSize;

            _selector = new Selector(Fitness, selectedSize);
            Select = _selector.GetSelectionFunction(selectionType);

            if (mask.Equals(""))
                //this.mask = (1 << sizeof(byte) * 4 /* *8/2 */) - 1; // 15 = b00001111
                this.mask = 0b00001111;
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

        private float GenerateNewValue(float seed)
        {
            //return (int)seed ^ Mathf.NextPowerOfTwo((int)Random.Range(0, (1 << sizeof(byte) * 8) - 1)) / 2;
            return Mathf.Clamp(seed + Random.Range(-5, 5), 0, byte.MaxValue);
        }


        private GeneContainer Mutate(GeneContainer geneContainer)
        {
            GeneContainer ans = new GeneContainer(geneContainer);

            foreach (var affectedGene in affectedGenes)
            {
                var v1 = GenerateNewValue(ans.GetGene(affectedGene).OptimalInterval.Min);
                var v2 = GenerateNewValue(ans.GetGene(affectedGene).OptimalInterval.Max);
                ans.GetGene(affectedGene).OptimalInterval = new Interval(v1, v2);
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
                float v1, v2;

                Interval fatherI = father.GetGene(affectedGene).OptimalInterval;
                Interval motherI = mother.GetGene(affectedGene).OptimalInterval;

                v1 = Combine(fatherI.Min, motherI.Min);
                v2= Combine(motherI.Max, fatherI.Max);
                child1.GetGene(affectedGene).OptimalInterval = new Interval(v1, v2);

                v1 = Combine(motherI.Min, fatherI.Min);
                v2 = Combine(fatherI.Max, motherI.Max);
                child2.GetGene(affectedGene).OptimalInterval = new Interval(v1, v2);
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

        public void Adapt(ref GeneContainer geneContainer, EnvironmentController environmentController, List<string> affectedGenes, float aux = 2)
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
            Select(ref population, environmentController, aux);
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