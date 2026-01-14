using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    [Header("General Level Settings")]
    public int maxLevel = 10;
    public float baseXPToLevel = 50f;
    public float xpIncreasePerLevel = 25f;

    [Header("Weapon Skills")]
    public int meleeLevel = 1;
    public float meleeXP;

    public int rangedLevel = 1;
    public float rangedXP;

    [Header("Movement Skill (Sprint / Stamina)")]
    public int sprintLevel = 1;
    public float sprintXP;

    [Header("Toughness Skill (HP / Defense)")]
    public int toughnessLevel = 1;
    public float toughnessXP;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    float GetXPToNext(int level)
    {
        if (level >= maxLevel)
            return Mathf.Infinity;

        return baseXPToLevel + (level - 1) * xpIncreasePerLevel;
    }

    void AddXP(ref float xp, ref int level, float amount, string skillName)
    {
        if (level >= maxLevel)
            return;

        xp += amount;

        bool leveledUp = false;

        float xpToNext = GetXPToNext(level);
        while (xp >= xpToNext && level < maxLevel)
        {
            xp -= xpToNext;
            level++;
            leveledUp = true;
            xpToNext = GetXPToNext(level);
        }

        if (leveledUp)
        {
            Debug.Log("[SkillManager] " + skillName + " leveled up to " + level);

            if (UImanager.instance != null)
            {
                UImanager.instance.ShowLevelUpMessage(skillName, level);
            }
        }
    }


    public void AddMeleeXP(float amount)
    {
        AddXP(ref meleeXP, ref meleeLevel, amount, "Melee");
    }

    public void AddRangedXP(float amount)
    {
        AddXP(ref rangedXP, ref rangedLevel, amount, "Ranged");
    }

    public void AddSprintXP(float amount)
    {
        AddXP(ref sprintXP, ref sprintLevel, amount, "Sprint");
    }

    public void AddToughnessXP(float amount)
    {
        AddXP(ref toughnessXP, ref toughnessLevel, amount, "Toughness");
    }


    float Level01(int level)
    {
        if (maxLevel <= 1)
            return 0f;

        return Mathf.Clamp01((float)(level - 1) / (maxLevel - 1));
    }


    public float GetMeleeDamageMultiplier()
    {
        float t = Level01(meleeLevel);
        return Mathf.Lerp(1f, 1.4f, t);
    }

    public float GetRangedDamageMultiplier()
    {
        float t = Level01(rangedLevel);
        return Mathf.Lerp(1f, 1.3f, t);
    }

    public float GetMeleeAttackSpeedMultiplier()
    {
        float t = Level01(meleeLevel);
        return Mathf.Lerp(1f, 1.3f, t);
    }

    public float GetRangedAttackSpeedMultiplier()
    {
        float t = Level01(rangedLevel);
        return Mathf.Lerp(1f, 1.2f, t);
    }

    public float GetSprintSpeedMultiplier()
    {
        float t = Level01(sprintLevel);
        return Mathf.Lerp(1f, 1.4f, t);
    }

    public float GetStaminaMaxMultiplier()
    {
        float t = Level01(sprintLevel);
        return Mathf.Lerp(1f, 2f, t);
    }

    public float GetToughnessDamageTakenMultiplier()
    {
        float t = Level01(toughnessLevel);
        return Mathf.Lerp(1f, 0.75f, t);
    }

    public float GetArmorEffectivenessMultiplier()
    {
        float t = Level01(toughnessLevel);
        return Mathf.Lerp(1f, 1.5f, t);
    }

    public float GetToughnessHealthMultiplier()
    {
        float t = Level01(toughnessLevel);
        return Mathf.Lerp(1f, 1.5f, t);
    }
}
