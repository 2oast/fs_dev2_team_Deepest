using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;


public class WeaponAnimationEvents : MonoBehaviour
{
    BoxCollider weaponCol;
    TrailRenderer trailRenderer;

    private void Update()
    {
        if (WeaponManager.instance.currentWeapon == null)
            return;

        weaponCol = WeaponManager.instance.currentWeapon.modelPrefab.GetComponent<BoxCollider>();
    }

    public void EnableCollder()
    {
        weaponCol = GetComponentInChildren<BoxCollider>();
        weaponCol.enabled = true;
    }

    public void DisableCollider()
    {
        weaponCol = GetComponentInChildren<BoxCollider>();
        weaponCol.enabled = false;
    }

    public void ActivateTrail()
    {
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        trailRenderer.enabled = true;
    }

    public void DeactivateTrail()
    {
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        trailRenderer.enabled = false;
    }

    

}
