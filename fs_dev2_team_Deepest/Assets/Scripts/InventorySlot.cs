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
                itemInSlot = null;
                inventorySlotSprite.sprite = null;
            }
            else if(itemInSlot.isWeapon)
            {
                if(WeaponManager.instance.WeaponEquipped())
                {
                    Destroy(WeaponManager.instance.currentWeapon);
                }
                Instantiate(itemInSlot.modelPrefab, WeaponManager.instance.rightHandTransform, false);
                WeaponManager.instance.currentWeapon = itemInSlot.GetComponent<Weapon>();
            }
        }
        else
        {
            return;
        }


    }
}
