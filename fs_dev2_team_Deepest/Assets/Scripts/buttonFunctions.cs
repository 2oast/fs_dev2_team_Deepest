using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] string titleSceneName = "TitleScreen";

    [Header("End Menu Load Buttons (optional)")]
    [SerializeField] GameObject loseMenuLoadButton;
    [SerializeField] GameObject winMenuLoadButton;

    void Start()
    {
        RefreshLoadButtons();
    }

    void OnEnable()
    {
        RefreshLoadButtons();
    }

    void RefreshLoadButtons()
    {
        if (SaveManager.instance == null)
        {
            if (loseMenuLoadButton != null) loseMenuLoadButton.SetActive(false);
            if (winMenuLoadButton != null) winMenuLoadButton.SetActive(false);
            return;
        }

        bool hasSave = SaveManager.instance.HasSave();

        if (loseMenuLoadButton != null) loseMenuLoadButton.SetActive(hasSave);
        if (winMenuLoadButton != null) winMenuLoadButton.SetActive(hasSave);
    }

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

        GameManager.instance.playerMovementScript.viewBobScript.enabled = true;
        GameManager.instance.playerMovementScript.enabled = true;
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

        GameManager.instance.playerMovementScript.enabled = true;
        GameManager.instance.playerMovementScript.viewBobScript.enabled = true;
    }

    public void Resume()
    {
        GameManager.instance.StateUnpause();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
                if (slot.isEquipped)
                {
                    InventoryManager.instance.weaponImage.sprite = slot.originalItemSprite;
                    slot.isEquipped = false;
                    item.Use(GameManager.instance.playerControllerScript);
                }
                else
                {
                    InventoryManager.instance.weaponImage.sprite = item.itemIcon;
                    item.Use(GameManager.instance.playerControllerScript);
                    slot.isEquipped = true;
                }
                break;

            case ItemType.ChestPiece:
                if (slot.isEquipped)
                {
                    InventoryManager.instance.chestPieceImage.sprite = slot.originalItemSprite;
                    slot.isEquipped = false;
                    item.Use(GameManager.instance.playerControllerScript);
                }
                else
                {
                    InventoryManager.instance.chestPieceImage.sprite = item.itemIcon;
                    item.Use(GameManager.instance.playerControllerScript);
                    slot.isEquipped = true;
                }
                break;

            case ItemType.Leggings:
                if (slot.isEquipped)
                {
                    InventoryManager.instance.leggingsPieceImage.sprite = slot.originalItemSprite;
                    slot.isEquipped = false;
                    item.Use(GameManager.instance.playerControllerScript);
                }
                else
                {
                    InventoryManager.instance.leggingsPieceImage.sprite = item.itemIcon;
                    item.Use(GameManager.instance.playerControllerScript);
                    slot.isEquipped = true;
                }
                break;

            case ItemType.Gauntlets:
                if (slot.isEquipped)
                {
                    InventoryManager.instance.leftGauntletPieceImage.sprite = slot.originalItemSprite;
                    InventoryManager.instance.rightGauntletPieceImage.sprite = slot.originalItemSprite;
                    slot.isEquipped = false;
                    item.Use(GameManager.instance.playerControllerScript);
                }
                else
                {
                    InventoryManager.instance.leftGauntletPieceImage.sprite = item.itemIcon;
                    InventoryManager.instance.rightGauntletPieceImage.sprite = item.itemIcon;
                    item.Use(GameManager.instance.playerControllerScript);
                    slot.isEquipped = true;
                }
                break;

            case ItemType.Ring:
                if (slot.isEquipped)
                {
                    InventoryManager.instance.ringImage.sprite = slot.originalItemSprite;
                    slot.isEquipped = false;
                    item.Use(GameManager.instance.playerControllerScript);
                }
                else
                {
                    InventoryManager.instance.ringImage.sprite = item.itemIcon;
                    item.Use(GameManager.instance.playerControllerScript);
                    slot.isEquipped = true;
                }
                break;

            case ItemType.Consumable:
                slot.UseItem();
                break;

            case ItemType.Key:
                break;
        }

        InventoryManager.instance.pendingEquipSlot = null;
        InventoryManager.instance.itemDescriptionBox.text = "";
        InventoryManager.instance.selectedSlot = null;
        InventoryManager.instance.itemImage.sprite = slot.originalItemSprite;
        InventoryManager.instance.YesOrNoPanel.SetActive(false);
    }

    public void SaveGame()
    {
        if (SaveManager.instance != null)
        {
            SaveManager.instance.SaveGame();
            RefreshLoadButtons();
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
            if (!SaveManager.instance.HasSave())
            {
                SaveManager.instance.ShowNoSaveFound();
                return;
            }

            SaveManager.instance.LoadGame();

            if (GameManager.instance != null)
                GameManager.instance.ResetAfterLoad();
        }
        else
        {
            Debug.LogWarning("LoadGame button pressed, but SaveManager.instance is null.");
        }
    }

    public void CloseControlsScreen()
    {
        GameManager.instance.controlsScreen.SetActive(false);
    }

    public void OpenControlsScreen()
    {
        GameManager.instance.menuActive = GameManager.instance.controlsScreen;
        GameManager.instance.controlsScreen.SetActive(true);
    }

    public void CloseInventoryScreen()
    {
        if (GameManager.instance.menuActive == GameManager.instance.inventoryScreen)
            GameManager.instance.StateUnpause();
    }
}
