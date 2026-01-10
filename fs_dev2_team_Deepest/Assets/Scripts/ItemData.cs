using UnityEngine;
public enum ItemType
{
    Weapon,
    Ring,
    ChestPiece,
    Key,
    Leggings,
    Helmet,
    Gauntlets,
    Consumable
}
[CreateAssetMenu(fileName = "Item")]
public abstract class ItemData : ScriptableObject
{
    public ItemType itemType;

    public string itemName;
    public bool isStackable;
    public bool isRadio;
    public int maxStack;
    public Sprite itemIcon;

    public GameObject modelPrefab;

    [TextArea]
    public string description;

    public abstract void Use(PlayerController player);
}
