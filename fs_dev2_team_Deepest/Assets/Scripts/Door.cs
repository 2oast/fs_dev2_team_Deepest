using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Key Settings")]
    [SerializeField] bool requiresKey = false;
    [SerializeField] string requiredKeyName;
    [SerializeField] bool consumeKeyOnUse = false;

    [Header("Door Settings")]
    [SerializeField] GameObject doorObject;
    [SerializeField] bool disableColliderWhenOpened = true;

    bool isOpen;

    void Awake()
    {
        if (doorObject == null)
            doorObject = gameObject;
    }

    public void Interact()
    {
        if (isOpen)
            return;

        if (requiresKey)
        {
            if (string.IsNullOrEmpty(requiredKeyName))
            {
                Debug.LogWarning("Door on " + name + " requires a key but requiredKeyName is empty.");
                return;
            }

            if (!PlayerHasKey(requiredKeyName))
            {
                Debug.Log("Door: player does not have required key: " + requiredKeyName);
                return;
            }

            if (consumeKeyOnUse)
                ConsumeKey(requiredKeyName);
        }

        OpenDoor();
    }

    void OpenDoor()
    {
        isOpen = true;

        if (doorObject != null)
            doorObject.SetActive(false);

        if (disableColliderWhenOpened)
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;
        }
    }

    bool PlayerHasKey(string keyName)
    {
        if (InventoryManager.instance == null || InventoryManager.instance.slots == null)
            return false;

        foreach (var slot in InventoryManager.instance.slots)
        {
            if (slot != null && slot.itemInSlot != null)
            {
                if (slot.itemInSlot.itemName == keyName)
                    return true;
            }
        }

        return false;
    }

    void ConsumeKey(string keyName)
    {
        if (InventoryManager.instance == null || InventoryManager.instance.slots == null)
            return;

        foreach (var slot in InventoryManager.instance.slots)
        {
            if (slot != null && slot.itemInSlot != null && slot.itemInSlot.itemName == keyName)
            {
                slot.UseItem();
                return;
            }
        }
    }
}
