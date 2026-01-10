using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour, ISubmitHandler, IPointerClickHandler, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData itemInSlot;
    public GameObject slotPref;
    public Image itemImageComp;
    public Sprite itemSprite;
    public TextMeshProUGUI itemNameTextBox;

    public bool isFilled;
    bool isSelected;

    private void Awake()
    {
    }

    public void Setup(ItemData item)
    {
        itemInSlot = item;
        itemNameTextBox = GetComponentInChildren<TextMeshProUGUI>();

        if (itemNameTextBox != null)
            itemNameTextBox.text = item.itemName;
    }
    

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if(InventoryManager.instance.selectedSlot != this)
            {
                InventoryManager.instance.selectedSlot = this;
                switch (itemInSlot.itemType)
                {
                    case ItemType.Weapon:
                        InventoryManager.instance.weaponImage.sprite = itemInSlot.itemIcon;
                        break;
                    case ItemType.ChestPiece:
                        InventoryManager.instance.chestPieceImage.sprite = itemInSlot.itemIcon;
                        break;
                    case ItemType.Leggings:
                        InventoryManager.instance.leggingsPieceImage.sprite = itemInSlot.itemIcon;
                        break;
                    case ItemType.Gauntlets:
                        InventoryManager.instance.leftGauntletPieceImage.sprite = itemInSlot.itemIcon;
                        InventoryManager.instance.rightGauntletPieceImage.sprite = itemInSlot.itemIcon;
                        break;
                    case ItemType.Ring:
                        InventoryManager.instance.ringImage.sprite = itemInSlot.itemIcon;
                        break;
                }
            }
            else
            {
                InventoryManager.instance.selectedSlot = null;
                switch (itemInSlot.itemType)
                {
                    case ItemType.Weapon:
                        InventoryManager.instance.weaponImage.sprite = null;
                        break;
                    case ItemType.ChestPiece:
                        InventoryManager.instance.chestPieceImage.sprite = null;
                        break;
                    case ItemType.Leggings:
                        InventoryManager.instance.leggingsPieceImage.sprite = null;
                        break;
                    case ItemType.Gauntlets:
                        InventoryManager.instance.leftGauntletPieceImage.sprite = null;
                        InventoryManager.instance.rightGauntletPieceImage.sprite = null;
                        break;
                    case ItemType.Ring:
                        InventoryManager.instance.ringImage.sprite = null;
                        break;
                }
            }

            
        }
    }

    public void UseItem()
    {
        itemInSlot.Use(GameManager.instance.playerControllerScript);
    }

    public void OnSelect(BaseEventData eventData)
    {

    }

    public void OnSubmit(BaseEventData eventData)
    {
        UseItem();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(InventoryManager.instance.selectedSlot == null)
        {

            InventoryManager.instance.itemImage.sprite = itemInSlot.itemIcon;
            InventoryManager.instance.itemDescriptionBox.text = itemInSlot.description;
        }
       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(InventoryManager.instance.selectedSlot == null)
        {
            InventoryManager.instance.itemImage.sprite = null;
            InventoryManager.instance.itemDescriptionBox.text = null;
        }
    }
}