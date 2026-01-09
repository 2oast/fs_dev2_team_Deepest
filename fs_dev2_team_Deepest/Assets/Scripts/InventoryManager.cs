using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public Item itemToBeCollected;
    public List<InventorySlot> slots;

    public bool radioInInventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        Init();
    }

    public void Init()
    {
        foreach (InventorySlot slot in slots)
        {
            slot.itemImageComp = slot.GetComponent<Image>();
        }
    }

    bool InventoryFull()
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.itemInSlot == null)
            {
                return false;
            }
        }
        return true;
    }


    public void AddItemToInventory(Item item)
    {
        if (!InventoryFull())
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].itemInSlot == null)
                {
                    slots[i].itemInSlot = item.item;
                    slots[i].itemImageComp.sprite = item.item.itemIcon;
                    slots[i].isFilled = true;
                    return;
                }
            }
        }
    }
}
