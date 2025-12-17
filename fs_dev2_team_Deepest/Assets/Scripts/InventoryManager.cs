using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [SerializeField] InventorySlot[] inventorySlots;
    public InventorySlot ringSlot;
    public InventorySlot activeSlot;
    [SerializeField] TextMeshProUGUI descriptionTextBox;

    void Start()
    {
        instance = this;
    }
    
    public void ResetInventorySlot(InventorySlot slot)
    {
        slot.itemInSlot = null;
        slot.inventorySlotSprite.sprite = null;
        slot.isFilled = false;
    }

    bool InventoryFull()
    {
        foreach(InventorySlot slot in inventorySlots)
        {
            if (slot.itemInSlot == null)
            {
                return false;
            }
        }
        return true;
    }

    public void AddItemToInventory(ItemData item)
    {
        if(!InventoryFull())
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].itemInSlot == null)
                {
                    inventorySlots[i].itemInSlot = item;
                    inventorySlots[i].inventorySlotSprite.sprite = item.inventoryIcon;
                    inventorySlots[i].isFilled = true;
                    return;
                }
            }
        }
    }
}
