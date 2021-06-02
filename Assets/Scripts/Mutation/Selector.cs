using GeneticAlgorithmForSpecies.Environment;
using GeneticAlgorithmForSpecies.Genes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GeneticAlgorithmForSpecies.Mutation
{
    /// <summary>
    /// This class is handling the selection
    /// </summary>
    class Selector
    {
        private readonly System.Func<Gene, Dictionary<string, float>, double> Fitness;
        private readonly int selectedSize;

        public Selector(System.Func<Gene, Dictionary<string, float>, double> Fitness, int selectedSize)
        {
            this.Fitness = Fitness;
            this.selectedSize = selectedSize;
        }

        private void Select(ref List<Gene> genes, EnvironmentController environmentController, int size = -1)
        {
            List<(Gene gene, double fitness)> sortedGenes = new List<(Gene, double)>();
            var envAspects = environmentController.GetAspects();
            foreach (var gene in genes)
            {
                sortedGenes.Add((new Gene(gene), Fitness(gene, envAspects)));
            }
            sortedGenes.Sort((elm1, elm2) => elm2.fitness.CompareTo(elm1.fitness));

            genes.Clear();

            if (size < 0)
            {
                size = selectedSize;
            }
            for (int i = 0; i < selectedSize; ++i)
            {
                genes.Add(sortedGenes[i].gene);
            }
        }

        private void TourSelect(ref List<Gene> genes, EnvironmentController environmentController, int Tour)
        {
            Assert.IsTrue(2 <= Tour && Tour <= genes.Count, "Tour is out of range. Please use Tour in range of (2, N - 1]");
            List<Gene> ans = new List<Gene>();
            for (int i = 0; i < selectedSize; ++i)
            {
                List<int> currTourIndices = new List<int>();
                while (currTourIndices.Count < Tour)
                {
                    int index = Random.Range(0, genes.Count - 1);
                    while (currTourIndices.Contains(index))
                    {
                        index = Random.Range(0, genes.Count - 1);
                    }

                    currTourIndices.Add(index);
                }

                List<Gene> currTour = new List<Gene>();
                foreach (var index in currTourIndices)
                {
                    currTour.Add(genes[index]);
                }

                Assert.IsTrue(currTour.Count >= Tour, "Tour Selection: Cannot select from a smaller population");
                Select(ref currTour, environmentController, size: Tour);
                ans.Add(currTour[0]);
            }
            genes = ans;
        }

        private List<(Gene gene, float f)> RankFitness(List<Gene> genes, EnvironmentController environmentController, float SP)
        {
            Assert.IsTrue(1 < SP && SP <= 2f, "SP is out of range. Please use SP in range of (1, 2]");
            Select(ref genes, environmentController);
            List<(Gene gene, float f)> ans = new List<(Gene, float)>();

            float totalF = 0f;
            for (int i = 0; i < genes.Count; ++i)
            {
                float val = 2 - SP + 2 * (SP - 1) * (i - 1) / (genes.Count - 1);
                totalF += val;
                ans.Add((gene: genes[i], f: val));
            }
            for (int i = 0; i < ans.Count; ++i)
            {
                ans[i] = (ans[i].gene, f: ans[i].f / totalF);
            }
            return ans;
        }

        private void RouletteSelect(ref List<Gene> genes, EnvironmentController environmentController, float SP)
        {
            List<float> values = new List<float>();
            for (int i = 0; i < selectedSize; ++i)
            {
                float val = (float)Random.Range(0, 100) / 100;
                values.Add(val);
            }

            var rankedGenes = RankFitness(genes, environmentController, SP);

            List<Gene> ans = new List<Gene>();
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
            genes = ans;
        }


        public ActionRef<List<Gene>, EnvironmentController, float> GetSelectionFunction(SelectionType selectionType = SelectionType.Default)
        {
            switch (selectionType)
            {
                case SelectionType.Default:
                    Debug.Log("using default Selection");
                    return (ref List<Gene> genes, EnvironmentController env, float _) => Select(ref genes, env);
                case SelectionType.Roulette:
                    Debug.Log("using roulette Selection");
                    return (ref List<Gene> genes, EnvironmentController env, float SP) => RouletteSelect(ref genes, env, SP);
                case SelectionType.Tour:
                    Debug.Log("using tour Selection");
                    return (ref List<Gene> genes, EnvironmentController env, float tourSize) => TourSelect(ref genes, env, (int)tourSize);
                default:
                    throw new System.Exception("Cannot select a function if no SelectionType is provided.");
            }
        }
    }
}
