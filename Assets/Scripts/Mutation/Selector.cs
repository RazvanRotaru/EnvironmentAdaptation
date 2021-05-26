using GeneticAlgorithmForSpecies.Environment;
using GeneticAlgorithmForSpecies.Genes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GeneticAlgorithmForSpecies.Mutation
{
    class Selector
    {
        private readonly System.Func<GeneContainer, Dictionary<string, float>, float> Fitness;
        private readonly int selectedSize;

        public Selector(System.Func<GeneContainer, Dictionary<string, float>, float> Fitness, int selectedSize)
        {
            this.Fitness = Fitness;
            this.selectedSize = selectedSize;
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

                    currTourIndices.Add(index);
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


        public ActionRef<List<GeneContainer>, EnvironmentController, float> GetSelectionFunction(SelectionType selectionType = SelectionType.Default)
        {
            switch (selectionType)
            {
                case SelectionType.Default:
                    Debug.Log("using default Selection");
                    return (ref List<GeneContainer> genes, EnvironmentController env, float _) => Select(ref genes, env);
                case SelectionType.Roulette:
                    Debug.Log("using roulette Selection");
                    return (ref List<GeneContainer> genes, EnvironmentController env, float SP) => RouletteSelect(ref genes, env, SP);
                case SelectionType.Tour:
                    Debug.Log("using tour Selection");
                    return (ref List<GeneContainer> genes, EnvironmentController env, float tourSize) => TourSelect(ref genes, env, (int)tourSize);
                default:
                    throw new System.Exception("Cannot select a function if no SelectionType is provided.");
            }
        }
    }
}
