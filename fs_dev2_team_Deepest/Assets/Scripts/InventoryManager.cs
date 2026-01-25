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

    public ItemData equippedRing;

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

    public void AddItemToInventory(Item item)
    {
        InventorySlot newSlot = Instantiate(slotPrefab, contentParent);
        newSlot.Setup(item.item);
        slots.Add(newSlot);
    }

    public void AddItemFromData(ItemData itemData)
    {
        InventorySlot newSlot = Instantiate(slotPrefab, contentParent);
        newSlot.Setup(itemData);
        slots.Add(newSlot);
    }

    public void ClearInventorySlots()
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] != null)
                Destroy(slots[i].gameObject);
        }

        slots.Clear();
    }

}

