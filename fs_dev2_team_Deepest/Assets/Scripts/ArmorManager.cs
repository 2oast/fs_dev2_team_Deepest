using UnityEngine;

public class ArmorManager : MonoBehaviour
{
    public static ArmorManager instance;

    [Header("Current Equipped Armor")]
    public Armor currentArmor;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public int ModifyDamage(int baseDamage)
    {
        if (currentArmor == null)
        {
            Debug.Log("[ArmorManager] No armor equipped. Full damage.");
            return baseDamage;
        }

        float pct = Mathf.Clamp(currentArmor.damageReductionPercent, 0f, 90f);
        float factor = 1f - (pct / 100f);
        int final = Mathf.CeilToInt(baseDamage * factor);
        if (final < 1)
            final = 1;

        Debug.Log($"[ArmorManager] {currentArmor.itemName} reduces {baseDamage} -> {final} ({pct}% DR)");
        return final;
    }
}
