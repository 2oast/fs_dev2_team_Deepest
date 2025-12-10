using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image inventorySlotSprite;
    public ItemData itemInSlot;
    public bool isFilled;

    public void Awake()
    {
        inventorySlotSprite = GetComponentInChildren<Image>();
    }
}
