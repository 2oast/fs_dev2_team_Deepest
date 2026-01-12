using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour, ISubmitHandler, IPointerClickHandler, ISelectHandler, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
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
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // Clicking the same slot again → deselect
        if (InventoryManager.instance.selectedSlot == this)
        {
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        // Otherwise select this slot
        if (InventoryManager.instance.selectedSlot != this)
        {
            InventoryManager.instance.selectedSlot = this;
            InventoryManager.instance.YesOrNoPanel.SetActive(true);
            InventoryManager.instance.itemDescriptionBox.text = "Equip " + itemInSlot.itemName + "?";
            InventoryManager.instance.pendingEquipSlot = this;
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

    public void OnDeselect(BaseEventData eventData)
    {
        
        if(InventoryManager.instance.pendingEquipSlot == null)
        {
            InventoryManager.instance.selectedSlot = null;
            InventoryManager.instance.itemImage.sprite = null;
            InventoryManager.instance.itemDescriptionBox.text = null;
            InventoryManager.instance.YesOrNoPanel.SetActive(false);
        }
    }
}