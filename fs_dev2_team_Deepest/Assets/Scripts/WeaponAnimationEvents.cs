using UnityEngine;

public class WeaponAnimationEvents : MonoBehaviour
{
    BoxCollider weaponCol;
    TrailRenderer trailRenderer;
    

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

    private void OnColliderEnter(Collider other)
    {
        IDamage dmg = other.GetComponent<IDamage>();
        if(dmg!= null)
        dmg.takeDamage(WeaponManager.instance.currentWeapon.damage);

        IDestructible destruct = other.GetComponent<IDestructible>();
        if(destruct!= null)
        destruct.Destruct();
    }

}
