using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [SerializeField] Transform contentParent;
    [SerializeField] InventorySlot slotPrefab;
    public GameObject YesOrNoPanel;
    public InventorySlot selectedSlot;
    public InventorySlot pendingEquipSlot;

    public Image itemImage;
    public Image headPieceImage;
    public Image chestPieceImage;
    public Image leggingsPieceImage;
    public Image leftGauntletPieceImage;
    public Image rightGauntletPieceImage;
    public Image ringImage;
    public Image weaponImage;

    public TextMeshProUGUI itemDescriptionBox;

    public Item itemToBeCollected;
    public List<InventorySlot> slots;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        slots = new List<InventorySlot>();
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
        InventorySlot newSlot = Instantiate(slotPrefab, contentParent);

        newSlot.Setup(item.item);
    }
}

