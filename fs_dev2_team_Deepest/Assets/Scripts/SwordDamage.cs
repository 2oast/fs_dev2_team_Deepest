using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    [SerializeField] GameObject hitEffect;

    private void OnTriggerEnter(Collider other)
    {
        IDestructible destruct = other.GetComponentInParent<IDestructible>();
        if (destruct != null)
            destruct.Destruct();

        IDamage dmg = other.GetComponentInParent<IDamage>();
        int finalDamage = ComputeFinalDamage();

        if (SkillManager.instance != null)
        {
            if (CompareTag("PlayerMelee"))
            {
                SkillManager.instance.AddMeleeXP(finalDamage);
            }
            else if (CompareTag("PlayerRanged"))
            {
                SkillManager.instance.AddRangedXP(finalDamage);
            }
        }

        if (dmg != null && !other.CompareTag("Player"))
            dmg.takeDamage(finalDamage);


    }

    int ComputeFinalDamage()
    {
        if (WeaponManager.instance == null || WeaponManager.instance.currentWeapon == null)
            return 0;

        int finalDamage = WeaponManager.instance.currentWeapon.damage;

        if (CompareTag("PlayerMelee") && SkillManager.instance != null)
        {
            float mult = SkillManager.instance.GetMeleeDamageMultiplier();
            finalDamage = Mathf.CeilToInt(finalDamage * mult);
        }
        else if (CompareTag("PlayerRanged") && SkillManager.instance != null)
        {
            float mult = SkillManager.instance.GetRangedDamageMultiplier();
            finalDamage = Mathf.CeilToInt(finalDamage * mult);
        }

        if (finalDamage < 0)
            finalDamage = 0;

        return finalDamage;
    }
}

