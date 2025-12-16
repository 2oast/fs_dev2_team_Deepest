using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite inventoryIcon;
    public Weapon weapon;
    public bool isConsumable;
    public bool isHealingItem;
    public bool isEquippable;
    public bool isRing;
    public bool isWeapon;
    public int staminaDrainAmount;
    public bool isKeyItem;
    public int damageAmount;
    public int healAmount;
    public string inventoryDescription;
    public GameObject modelPrefab;
}

