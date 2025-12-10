using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] TextMeshProUGUI descriptionTextBox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
   

    void Start()
    {
        instance = this;
        
    }

    // Update is called once per frame
    void Update()
    {
  
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

    public void UseItemFromInventory()
    {
        
    }

}
