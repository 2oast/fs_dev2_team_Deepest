using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] ItemData weapon;

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
            dmg.takeDamage(weapon.damageAmount);
        }
    }
}
