using UnityEngine;

[CreateAssetMenu(fileName = "Magic Ring")]
public class MagicRing : ItemData
{
    public SpellType spellType;
    [SerializeField] int castingCost;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    public AudioClip shootSound;
    public int shootSpeed;

    public GameObject shootEffect;

    public override void Use(PlayerController player)
    {
        player.EquipRing(this);
    }
}
