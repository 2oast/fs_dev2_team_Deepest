using UnityEngine;

[CreateAssetMenu(menuName = "Item/Antidote")]
public class AntidoteItem : ItemData
{
    public override void Use(PlayerController player)
    {
        if (player != null)
        {
            player.CurePoison();
        }
    }
}
