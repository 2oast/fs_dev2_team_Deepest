using UnityEngine;

public class Weapon : MonoBehaviour
{
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
}
