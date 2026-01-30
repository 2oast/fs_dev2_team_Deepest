using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] ItemDatabase itemDatabase;

    [Header("No Save UI")]
    [SerializeField] TextMeshProUGUI noSaveText;
    [SerializeField] float noSaveMessageDuration = 2f;

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

    // ---------------- SAVE EXISTS ----------------
    public bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public static void DeleteSaveFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "savegame.json");

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted at: " + path);
        }
        else
        {
            Debug.Log("No save file to delete at: " + path);
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted at: " + SavePath);
        }
        else
        {
            Debug.Log("No save file to delete at: " + SavePath);
        }
    }

    // ---------------- SAVE ----------------
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
                    data.inventoryItemNames.Add(slot.itemInSlot.itemName);
            }
        }

        data.isPoisoned = pc.IsPoisoned;
        data.poisonTimeRemaining = pc.PoisonRemainingTime;

        data.bridgeIDs.Clear();
        data.bridgeExtended.Clear();

        foreach (var bridge in FindObjectsByType<BridgeScript>(FindObjectsSortMode.None))
        {
            if (bridge == null || string.IsNullOrEmpty(bridge.id))
                continue;

            data.bridgeIDs.Add(bridge.id);
            data.bridgeExtended.Add(bridge.IsExtended);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log("Game saved to: " + SavePath);
    }

    // ---------------- LOAD ----------------
    public void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No save file found at " + SavePath);
            ShowNoSaveFound();
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
                inv.AddItemFromData(itemData);
        }

        Debug.Log("Game loaded from: " + SavePath);
    }

    void ShowNoSaveFound()
    {
        if (noSaveText == null)
            return;

        noSaveText.text = "No save found";
        noSaveText.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(HideNoSaveFound());
    }

    IEnumerator HideNoSaveFound()
    {
        yield return new WaitForSecondsRealtime(noSaveMessageDuration);

        if (noSaveText != null)
            noSaveText.gameObject.SetActive(false);
    }
}

