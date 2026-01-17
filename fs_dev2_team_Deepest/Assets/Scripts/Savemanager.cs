using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] ItemDatabase itemDatabase;

    string SavePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        GameData data = new GameData();

        PlayerController pc = GameManager.instance.playerControllerScript;
        InventoryManager inv = InventoryManager.instance;
        SkillManager sm = SkillManager.instance;
        WeaponManager wm = WeaponManager.instance;
        ArmorManager am = ArmorManager.instance;

        data.hp = pc.HP;
        data.stamina = pc.CurrentStamina;
        data.playerPosition = GameManager.instance.player.transform.position;

        if (sm != null)
        {
            data.meleeLevel = sm.meleeLevel;
            data.rangedLevel = sm.rangedLevel;
            data.sprintLevel = sm.sprintLevel;
            data.toughnessLevel = sm.toughnessLevel;

            data.meleeXP = sm.meleeXP;
            data.rangedXP = sm.rangedXP;
            data.sprintXP = sm.sprintXP;
            data.toughnessXP = sm.toughnessXP;
        }

        if (wm != null && wm.currentWeapon != null)
            data.currentWeaponName = wm.currentWeapon.itemName;

        if (am != null && am.currentArmor != null)
            data.currentArmorName = am.currentArmor.itemName;

        if (wm != null && wm.currentRingEquipped != null)
            data.currentRingName = wm.currentRingEquipped.itemName;

        data.inventoryItemNames.Clear();
        if (inv != null && inv.slots != null)
        {
            foreach (var slot in inv.slots)
            {
                if (slot != null && slot.itemInSlot != null)
                {
                    data.inventoryItemNames.Add(slot.itemInSlot.itemName);
                }
            }
        }

        data.isPoisoned = pc.IsPoisoned;
        data.poisonTimeRemaining = pc.PoisonRemainingTime;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log("Game saved to: " + SavePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No save file found at " + SavePath);
            return;
        }

        string json = File.ReadAllText(SavePath);
        GameData data = JsonUtility.FromJson<GameData>(json);

        PlayerController pc = GameManager.instance.playerControllerScript;
        InventoryManager inv = InventoryManager.instance;
        SkillManager sm = SkillManager.instance;
        WeaponManager wm = WeaponManager.instance;
        ArmorManager am = ArmorManager.instance;

        if (pc != null)
        {
            CharacterController cc = pc.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                pc.transform.position = data.playerPosition;
                cc.enabled = true;
            }
            else
            {
                pc.transform.position = data.playerPosition;
            }
        }

        pc.HP = data.hp;
        pc.SetStamina(data.stamina);
        pc.updatePlayerUI();

        if (data.isPoisoned && data.poisonTimeRemaining > 0f)
        {
            pc.RestorePoisonFromSave(data.poisonTimeRemaining, 5f, 1);
        }
        else
        {
            pc.CurePoison();
        }

        if (sm != null)
        {
            sm.meleeLevel = data.meleeLevel;
            sm.rangedLevel = data.rangedLevel;
            sm.sprintLevel = data.sprintLevel;
            sm.toughnessLevel = data.toughnessLevel;

            sm.meleeXP = data.meleeXP;
            sm.rangedXP = data.rangedXP;
            sm.sprintXP = data.sprintXP;
            sm.toughnessXP = data.toughnessXP;
        }

        inv.ClearInventorySlots();

        foreach (string itemName in data.inventoryItemNames)
        {
            ItemData itemData = itemDatabase.GetItemByName(itemName);
            if (itemData != null)
            {
                inv.AddItemFromData(itemData);
            }
        }

        if (!string.IsNullOrEmpty(data.currentWeaponName))
        {
            ItemData itemData = itemDatabase.GetItemByName(data.currentWeaponName);
            Weapon weapon = itemData as Weapon;
            if (weapon != null)
            {
                if (pc.currentWeaponInstance != null)
                {
                    Object.Destroy(pc.currentWeaponInstance);
                    pc.currentWeaponInstance = null;
                }

                if (WeaponManager.instance != null)
                {
                    WeaponManager.instance.currentWeapon = null;
                }

                pc.EquipWeapon(weapon);
                inv.weaponImage.sprite = weapon.itemIcon;
            }
        }

        if (!string.IsNullOrEmpty(data.currentArmorName))
        {
            ItemData itemData = itemDatabase.GetItemByName(data.currentArmorName);
            Armor armor = itemData as Armor;
            if (armor != null)
            {
                pc.EquipArmor(armor);
                inv.chestPieceImage.sprite = armor.itemIcon;
            }
        }

        if (!string.IsNullOrEmpty(data.currentRingName))
        {
            ItemData itemData = itemDatabase.GetItemByName(data.currentRingName);
            MagicRing ring = itemData as MagicRing;
            if (ring != null)
            {
                pc.EquipRing(ring);
                inv.ringImage.sprite = ring.itemIcon;
            }
        }

        foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            Object.Destroy(enemy.gameObject);
        }

        EnemySpawner.ResetAllSpawners();

        Debug.Log("Game loaded from: " + SavePath);
    }
}
