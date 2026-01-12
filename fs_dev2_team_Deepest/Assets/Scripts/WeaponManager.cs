using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;

    public Weapon currentWeapon;
    public MagicRing currentRingEquipped;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateWeaponAnimator(GameObject weapon, Animator animator)
    {
        animator = weapon.GetComponent<Animator>();
    }
}
