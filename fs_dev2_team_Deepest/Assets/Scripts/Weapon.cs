using UnityEngine;

public class Weapon : Item
{
    public ItemData weaponData;

    public void StartDamageWindow()
    {
        GameManager.instance.isInteracting = true;
        WeaponManager.instance.weaponCollider.enabled = true;
    }

    public void EndDamageWindow()
    {
        GameManager.instance.isInteracting = false;
        WeaponManager.instance.weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamage dmg = other.GetComponent<IDamage>();
        if(dmg != null)
        {
            dmg.takeDamage(weaponData.damageAmount);
        }
    }
}
