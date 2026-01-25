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
    public Sprite originalItemSprite;
    public TextMeshProUGUI itemNameTextBox;

    public bool isFilled;
    bool isSelected;

    private void Awake()
    {
    }

    public void Setup(ItemData item)
    {
        itemInSlot = item;
        isFilled = item != null;

        itemNameTextBox = GetComponentInChildren<TextMeshProUGUI>();

        if (itemNameTextBox != null)
            itemNameTextBox.text = item != null ? item.itemName : "";

        if (itemImageComp != null)
        {
            if (item != null)
            {
                itemImageComp.sprite = item.itemIcon;
                itemImageComp.enabled = true;
            }
            else
            {
                itemImageComp.sprite = null;
                itemImageComp.enabled = false;
            }
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (itemInSlot == null)
            return;

        if (InventoryManager.instance.selectedSlot == this)
        {
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

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
        if (itemInSlot == null)
            return;

        Debug.Log("InventorySlot.UseItem() on " + itemInSlot.itemName);

        itemInSlot.Use(GameManager.instance.playerControllerScript);

        if (itemInSlot.itemType == ItemType.Consumable)
        {
            Debug.Log("Item is consumable, clearing slot.");
            Destroy(gameObject);
        }
    }

    void ClearSlot()
    {
        itemInSlot = null;
        itemSprite = null;

        if (itemImageComp != null)
        {
            itemImageComp.sprite = null;
            itemImageComp.enabled = false;
        }

        if (itemNameTextBox != null)
            itemNameTextBox.text = "";

        isFilled = false;
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
        if (itemInSlot == null)
            return;

        if (InventoryManager.instance.selectedSlot == null)
        {
            InventoryManager.instance.itemImage.sprite = itemInSlot.itemIcon;
            InventoryManager.instance.itemDescriptionBox.text = itemInSlot.description;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (InventoryManager.instance.selectedSlot == null)
        {
            InventoryManager.instance.itemImage.sprite = originalItemSprite;
            InventoryManager.instance.itemDescriptionBox.text = null;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (InventoryManager.instance.pendingEquipSlot == null)
        {
            InventoryManager.instance.selectedSlot = null;
            InventoryManager.instance.itemImage.sprite = originalItemSprite;
            InventoryManager.instance.itemDescriptionBox.text = null;
            InventoryManager.instance.YesOrNoPanel.SetActive(false);
        }
    }
}
