using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    [SerializeField] List<Item> items;
    [SerializeField] int size;

    // Start is called before the first frame update
    void Start()
    {
        // load items from PlayerPrefs
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
