using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IDamage dmg = other.GetComponent<IDamage>();
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

        IDestructible destruct = other.GetComponent<IDestructible>();
        if (destruct != null)
            destruct.Destruct();

       
    }

    int ComputeFinalDamage()
    {
        int finalDamage = WeaponManager.instance.currentWeapon.damage;
        if (GameManager.instance.playerControllerScript.chargeAttack)
        {
            finalDamage *= 2;
        }

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
