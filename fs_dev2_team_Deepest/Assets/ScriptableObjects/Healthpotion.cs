using UnityEngine;

[CreateAssetMenu(menuName = "Item/Health Potion")]
public class HealthPotionItem : ItemData
{
    [SerializeField] float healPercent = 0.25f;

    public override void Use(PlayerController player)
    {
        if (player == null)
            return;

        int maxHP = player.MaxHP;
        int healAmount = Mathf.RoundToInt(maxHP * healPercent);

        if (healAmount <= 0)
            return;

        player.Heal(healAmount);
        Debug.Log("Health Potion used. Healed " + healAmount + " HP.");
    }
}
