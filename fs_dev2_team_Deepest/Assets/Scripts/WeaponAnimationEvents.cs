using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;


public class WeaponAnimationEvents : MonoBehaviour
{
    [SerializeField] BoxCollider weaponCol;
    TrailRenderer trailRenderer;

    private void Update()
    {
        if (WeaponManager.instance.currentWeapon == null)
            return;

    }

    public void EnableCollder()
    {
        weaponCol.enabled = true;
    }

    public void DisableCollider()
    {
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
