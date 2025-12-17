using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image originalImage;
    public Image inventorySlotSprite;
    public ItemData itemInSlot;
    public Button useItemButton;
    public bool isFilled;

    public void Awake()
    {
        originalImage = GetComponent<Image>();
        inventorySlotSprite = GetComponent<Image>();
        useItemButton = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        if(itemInSlot != null)
        {
            if (itemInSlot.isHealingItem)
            {
                GameManager.instance.playerScript.HP += 10;
                InventoryManager.instance.ResetInventorySlot(this);
            }
            else if(itemInSlot.isEquippable && !itemInSlot.isRing)
            {
                if(WeaponManager.instance.ItemEquipped())
                {
                    Destroy(WeaponManager.instance.currentItem.gameObject);
                }
                GameObject newItem = Instantiate( itemInSlot.modelPrefab, WeaponManager.instance.rightHandTransform, false);
                WeaponManager.instance.SetCurrentItemData(newItem.GetComponent<Item>());
                InventoryManager.instance.activeSlot = this;
            }
            else if (itemInSlot.isEquippable && itemInSlot.isRing)
            {
                InventoryManager.instance.ringSlot.inventorySlotSprite.sprite = itemInSlot.inventoryIcon;
                WeaponManager.instance.ringEquipped = true;
                InventoryManager.instance.ringSlot.itemInSlot = this.itemInSlot;
            }

        }
        else
        {
            return;
        }


    }
}
