using UnityEngine;

public enum KeyNames
{
    ShackDoor,
    RadioTowerKitchen
}

[CreateAssetMenu(menuName = "Item/KeyItems")]
public class KeyItem : ItemData
{
    public KeyNames keyName;

    public override void Use(PlayerController player)
    {

    }
}
