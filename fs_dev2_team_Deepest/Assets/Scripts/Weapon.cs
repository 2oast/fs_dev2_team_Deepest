using UnityEngine;

public enum WeaponType
{
    Sword,
    Gun
}

[CreateAssetMenu(menuName = "Item/Weapon")]
public class Weapon : ItemData
{
    [Header("General")]
    public WeaponType weaponType;
    public int damage;

    [Header("Gun Settings")]
    public Material bulletHoles;
    public ParticleSystem smoke;

    [Header("Sword Settings")]
    public float staminaDrain;

    public override void Use(PlayerController player)
    {
        player.EquipWeapon(this);
    }
}


