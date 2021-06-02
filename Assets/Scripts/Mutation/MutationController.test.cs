using GeneticAlgorithmForSpecies.Genes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GeneticAlgorithmForSpecies.Mutation
{
    public partial class MutationController
    {
        private Dictionary<string, (int population_size, byte mask, int selected_size, SelectionType type)> _mutatorParams;
        private List<int> _waitTimes;
        private Vector3 _initPos;

        private void GenerateMutationParams()
        {
            _mutatorParams = new Dictionary<string, (int, byte, int, SelectionType)>();
            _waitTimes = new List<int>() { 20, 200 };
            _initPos = transform.position;

            List<byte> masks = new List<byte>() {  0b10101010, 0b11110000, 0b11001100, 0b11000011 };
            List<int> popSizes = new List<int>() { 4, 8, 32 };
            List<int> selSizes = new List<int>() { 2, 4 };
            List<SelectionType> types = System.Enum.GetValues(typeof(SelectionType)).Cast<SelectionType>().ToList();

            foreach (byte mask in masks)
            {
                foreach (int popSize in popSizes)
                {
                    foreach (int selSize in selSizes)
                    {
                        foreach (SelectionType type in types)
                        {
                            string name = $"Type-{type}/p{popSize}m{System.Convert.ToString(mask, 2)}s{selSize}";
                            _mutatorParams[name] = (popSize, mask, selSize, type);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method sets the player up for testing and 
        /// evaluates all the scenarios listed in the above properties
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator TestCoroutine()
        {
            GenerateMutationParams();
            Time.timeScale = 100;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.constraints |= RigidbodyConstraints.FreezePositionY;

            foreach (int waitTime in _waitTimes)
            {
                follower.WaitTime = waitTime;
                foreach (KeyValuePair<string, (int population_size, byte mask, int selected_size, SelectionType type)> entry in _mutatorParams)
                {
                    var (population_size, mask, selected_size, type) = entry.Value;
                    if (population_size == selected_size)
                    {
                        continue;
                    }
                    mutator = new Mutator(populationSize: population_size, mask: mask, selectedSize: selected_size, selectionType: type);
                    _subdir = $"WaitTime-{waitTime}/{entry.Key}";
                    _generation = 0;
                    transform.position = _initPos;
                    genes = new GeneContainer();
                    PathFollower pathFollower = GetComponent<PathFollower>();
                    if (pathFollower != null)
                    {
                        pathFollower.Init();
                    }

                    yield return new WaitForSeconds(15 * 60);
                }
            }
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
