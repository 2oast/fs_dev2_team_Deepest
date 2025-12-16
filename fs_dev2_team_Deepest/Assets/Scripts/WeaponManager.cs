using System;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;


    public Transform rightHandTransform;
    public Item currentItem;
    public bool ringEquipped;
    
    public BoxCollider weaponCollider;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;

        if (rightHandTransform.childCount > 0)
        {
            currentItem = rightHandTransform.GetChild(0).GetComponent<Item>();

            if (currentItem != null && currentItem.itemData.isWeapon)
            {
                weaponCollider = currentItem.GetComponent<BoxCollider>();
                weaponCollider.enabled = false;
            }
        }
    }

    public bool ItemEquipped()
    {
        if(rightHandTransform.childCount == 0)
        {
            return false;
        }

        return true;
    }

    private void Update()
    {
        if (currentItem == null)
            return;
        else if(currentItem.itemData.isKeyItem)
        {
            GameManager.instance.keyEquipped = true;
        }
    }

    public void SetCurrentItemData(Item item)
    {
        currentItem = item;

        if(currentItem != null)
        {
            if (currentItem.itemData.isWeapon)
            {
                weaponCollider = currentItem.GetComponent<BoxCollider>();
                PlayerAnimatorManager.instance.UpdateAnimator(currentItem.gameObject.GetComponent<Animator>());
            }
            else
                return;
        }
        
    }

    

}
