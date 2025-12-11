using System;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;


    public Transform rightHandTransform;
    public GameObject currentWeapon;
    public BoxCollider weaponCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        if (currentWeapon != null)
        {
            weaponCollider = currentWeapon.GetComponent<BoxCollider>();
            weaponCollider.enabled = false;
        }
        currentWeapon = rightHandTransform.GetChild(0).gameObject;
    }

    public bool WeaponEquipped()
    {
        if(rightHandTransform.childCount == 0)
        {
            return false;
        }

        return true;
    }

    public void SwingSword()
    {
        if (WeaponEquipped() && Input.GetButtonDown("Fire1"))
        {
            PlayerAnimatorManager.instance.PlayTargetAnimation(PlayerAnimatorManager.instance.playerAnimator, "SwordSwing");
            GameManager.instance.isInteracting = true;
        }
        else
        {
            return;
        }
    }

    

    private void Update()
    {
        weaponCollider = currentWeapon.GetComponent<BoxCollider>();
    }
}
