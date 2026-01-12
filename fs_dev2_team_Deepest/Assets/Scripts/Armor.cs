using UnityEngine;

[CreateAssetMenu(menuName = "Item/Armor")]
public class Armor : ItemData
{
    [Header("Armor Stats")]
    [Tooltip("Percent damage reduction (e.g. 20 = 20% less damage).")]
    public float damageReductionPercent = 25f;

    public override void Use(PlayerController player)
    {
        player.EquipArmor(this);
    }
}
