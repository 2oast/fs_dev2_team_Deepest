using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void CollectItem()
    {
        InventoryManager.instance.AddItemToInventory(InventoryManager.instance.itemToBeCollected);
        
        Destroy(InventoryManager.instance.itemToBeCollected.gameObject);
        InventoryManager.instance.itemToBeCollected = null;
        GameManager.instance.pickupText.text = "";
        GameManager.instance.YesOrNoObj.SetActive(false);
        GameManager.instance.playerControllerScript.enabled = true;
        GameManager.instance.isInteracting = false;
        GameManager.instance.cameraControllerScript.enabled = true;
    }

    public void RefuseItem()
    {
        InventoryManager.instance.itemToBeCollected.transform.position = InventoryManager.instance.itemToBeCollected.originalPos;
        InventoryManager.instance.itemToBeCollected.transform.rotation = InventoryManager.instance.itemToBeCollected.originalRot;
        GameManager.instance.YesOrNoObj.SetActive(false);
        StopCoroutine(InventoryManager.instance.itemToBeCollected.FloatToCenter());
        InventoryManager.instance.itemToBeCollected.isFloating = false;
        InventoryManager.instance.itemToBeCollected.isReadyToCollect = false;
        InventoryManager.instance.itemToBeCollected = null;
        GameManager.instance.pickupText.text = "";
        GameManager.instance.playerControllerScript.enabled = true;
        GameManager.instance.isInteracting = false;
        GameManager.instance.cameraControllerScript.enabled = true;
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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void EquipFromMenu()
    {
        switch (InventoryManager.instance.pendingEquipSlot.itemInSlot.itemType)
        {
            case ItemType.Weapon:
                InventoryManager.instance.weaponImage.sprite = InventoryManager.instance.pendingEquipSlot.itemInSlot.itemIcon;
                InventoryManager.instance.pendingEquipSlot.itemInSlot.Use(GameManager.instance.playerControllerScript);
                break;
            case ItemType.ChestPiece:
                InventoryManager.instance.chestPieceImage.sprite = InventoryManager.instance.pendingEquipSlot.itemInSlot.itemIcon;
                break;
            case ItemType.Leggings:
                InventoryManager.instance.leggingsPieceImage.sprite = InventoryManager.instance.pendingEquipSlot.itemInSlot.itemIcon;
                break;
            case ItemType.Gauntlets:
                InventoryManager.instance.leftGauntletPieceImage.sprite = InventoryManager.instance.pendingEquipSlot.itemInSlot.itemIcon;
                InventoryManager.instance.rightGauntletPieceImage.sprite = InventoryManager.instance.pendingEquipSlot.itemInSlot.itemIcon;
                break;
            case ItemType.Ring:
                InventoryManager.instance.ringImage.sprite = InventoryManager.instance.pendingEquipSlot.itemInSlot.itemIcon;
                InventoryManager.instance.pendingEquipSlot.itemInSlot.Use(GameManager.instance.playerControllerScript);
                break;
        }

        InventoryManager.instance.pendingEquipSlot = null;
        InventoryManager.instance.itemDescriptionBox.text = "";
        InventoryManager.instance.selectedSlot = null;
        InventoryManager.instance.itemImage.sprite = null;
        InventoryManager.instance.YesOrNoPanel.SetActive(false);
    }

}
