using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] string titleSceneName = "TitleScreen";

    public void CollectItem()
    {
        if (InventoryManager.instance.itemToBeCollected != null)
        {
            InventoryManager.instance.AddItemToInventory(InventoryManager.instance.itemToBeCollected);

            Destroy(InventoryManager.instance.itemToBeCollected.gameObject);
            InventoryManager.instance.itemToBeCollected = null;
        }

        GameManager.instance.pickupText.text = "";
        GameManager.instance.YesOrNoObj.SetActive(false);

        GameManager.instance.isInteracting = false;
        GameManager.instance.playerControllerScript.enabled = true;

        if (GameManager.instance.cameraControllerScript != null)
            GameManager.instance.cameraControllerScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RefuseItem()
    {
        if (InventoryManager.instance.itemToBeCollected != null)
        {
            InventoryManager.instance.itemToBeCollected.transform.position = InventoryManager.instance.itemToBeCollected.originalPos;
            InventoryManager.instance.itemToBeCollected.transform.rotation = InventoryManager.instance.itemToBeCollected.originalRot;

            InventoryManager.instance.itemToBeCollected.isFloating = false;
            InventoryManager.instance.itemToBeCollected.isReadyToCollect = false;

            InventoryManager.instance.itemToBeCollected = null;
        }

        GameManager.instance.pickupText.text = "";
        GameManager.instance.YesOrNoObj.SetActive(false);

        GameManager.instance.isInteracting = false;
        GameManager.instance.playerControllerScript.enabled = true;

        if (GameManager.instance.cameraControllerScript != null)
            GameManager.instance.cameraControllerScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Resume()
    {
        GameManager.instance.StateUnpause();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.StateUnpause();
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleSceneName);
    }

    public void EquipFromMenu()
    {
        var slot = InventoryManager.instance.pendingEquipSlot;

        if (slot == null || slot.itemInSlot == null)
            return;

        ItemData item = slot.itemInSlot;

        switch (item.itemType)
        {
            case ItemType.Weapon:
                InventoryManager.instance.weaponImage.sprite = item.itemIcon;
                item.Use(GameManager.instance.playerControllerScript);
                break;

            case ItemType.ChestPiece:
                InventoryManager.instance.chestPieceImage.sprite = item.itemIcon;
                item.Use(GameManager.instance.playerControllerScript);
                break;

            case ItemType.Leggings:
                InventoryManager.instance.leggingsPieceImage.sprite = item.itemIcon;
                item.Use(GameManager.instance.playerControllerScript);
                break;

            case ItemType.Gauntlets:
                InventoryManager.instance.leftGauntletPieceImage.sprite = item.itemIcon;
                InventoryManager.instance.rightGauntletPieceImage.sprite = item.itemIcon;
                item.Use(GameManager.instance.playerControllerScript);
                break;

            case ItemType.Ring:
                InventoryManager.instance.ringImage.sprite = item.itemIcon;
                item.Use(GameManager.instance.playerControllerScript);
                break;

            case ItemType.Consumable:
                slot.UseItem();
                break;
        }

        InventoryManager.instance.pendingEquipSlot = null;
        InventoryManager.instance.itemDescriptionBox.text = "";
        InventoryManager.instance.selectedSlot = null;
        InventoryManager.instance.itemImage.sprite = null;
        InventoryManager.instance.YesOrNoPanel.SetActive(false);
    }

    public void SaveGame()
    {
        if (SaveManager.instance != null)
        {
            SaveManager.instance.SaveGame();
        }
        else
        {
            Debug.LogWarning("SaveGame button pressed, but SaveManager.instance is null.");
        }
    }

    public void LoadGame()
    {
        if (SaveManager.instance != null)
        {
            SaveManager.instance.LoadGame();

            if (GameManager.instance != null)
            {
                GameManager.instance.ResetAfterLoad();
            }
        }
        else
        {
            Debug.LogWarning("LoadGame button pressed, but SaveManager.instance is null.");
        }
    }


    public void CloseScreen()
    {
        GameManager.instance.controlsScreen.SetActive(false);
    }

    public void OpenControlsScreen()
    {
        GameManager.instance.menuActive = GameManager.instance.controlsScreen;
        GameManager.instance.controlsScreen.SetActive(true);
    }
}