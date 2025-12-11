using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite inventoryIcon;
    public Weapon weapon;
    public bool isStackable;
    public bool isHealingItem;
    public bool isWeapon;
    public bool isKeyItem;
    public int damageAmount;
    public int healAmount;
    public string inventoryDescription;
    public GameObject modelPrefab;
}

