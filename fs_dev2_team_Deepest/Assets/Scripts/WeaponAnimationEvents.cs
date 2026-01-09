using UnityEngine;

public class WeaponAnimationEvents : MonoBehaviour
{
    BoxCollider weaponCol;

    public void EnableCollder()
    {
        weaponCol = GetComponent<BoxCollider>();
        weaponCol.enabled = true;
    }

    public void DisableCollider()
    {
        weaponCol = GetComponent<BoxCollider>();
        weaponCol.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();
        if(dmg!= null)
        dmg.takeDamage(WeaponManager.instance.currentWeapon.damage);
    }
}
