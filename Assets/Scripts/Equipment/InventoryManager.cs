using System.Collections.Generic;
using UnityEngine;

namespace GeneticAlgorithmForSpecies.Equipment
{
    /// <summary>
    /// Thiss class handles the inventory
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        private static InventoryManager _instance;
        public static InventoryManager Instance { get => _instance; }


        [SerializeField] List<Item> items;
        [SerializeField] int size;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        void Start()
        {
            // load items from PlayerPrefs
        }

        private static InventoryManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new InventoryManager();
            }
            return _instance;
        }

        public bool Add(Item item)
        {
            if (items.Count > size)
            {
                return false;
            }

            items.Add(item);
            return true;
        }

        public void Remove(Item item)
        {
            items.Remove(item);
        }
    }
}