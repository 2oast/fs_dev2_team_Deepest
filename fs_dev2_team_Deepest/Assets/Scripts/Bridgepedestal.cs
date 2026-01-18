using System.Collections;
using UnityEngine;

public class BridgePedestal : MonoBehaviour, IInteractable
{
    public enum ExtendAxis
    {
        X,
        Z
    }

    [Header("Key Settings")]
    [SerializeField] string requiredKeyName;
    [SerializeField] bool consumeKeyOnUse = true;

    [Header("Bridge Settings")]
    [SerializeField] Transform bridge;
    [SerializeField] ExtendAxis extendAxis = ExtendAxis.X;
    [SerializeField] float extendDistance = 5f;
    [SerializeField] float extendSpeed = 2f;

    [Header("Misc")]
    [SerializeField] bool canOnlyUseOnce = true;

    bool hasActivated = false;
    Vector3 bridgeStartPos;
    Vector3 bridgeEndPos;
    bool initialized = false;

    void Init()
    {
        if (initialized)
            return;

        if (bridge == null)
        {
            Debug.LogWarning("BridgePedestal on " + name + " has no bridge assigned.");
            return;
        }

        bridgeStartPos = bridge.position;

        Vector3 offset = Vector3.zero;
        switch (extendAxis)
        {
            case ExtendAxis.X:
                offset = new Vector3(extendDistance, 0f, 0f);
                break;
            case ExtendAxis.Z:
                offset = new Vector3(0f, 0f, extendDistance);
                break;
        }

        bridgeEndPos = bridgeStartPos + offset;
        initialized = true;
    }

    public void Interact()
    {
        Init();

        if (bridge == null)
            return;

        if (canOnlyUseOnce && hasActivated)
            return;

        if (!PlayerHasKey(requiredKeyName))
        {
            Debug.Log("BridgePedestal: player does not have required key: " + requiredKeyName);
            return;
        }

        if (consumeKeyOnUse)
        {
            ConsumeKey(requiredKeyName);
        }

        hasActivated = true;
        StartCoroutine(ExtendBridge());
    }

    IEnumerator ExtendBridge()
    {
        float t = 0f;
        Vector3 startPos = bridge.position;
        Vector3 targetPos = bridgeEndPos;

        while (t < 1f)
        {
            t += Time.deltaTime * extendSpeed;
            bridge.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        bridge.position = targetPos;
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
            if (slot != null && slot.itemInSlot != null &&
                slot.itemInSlot.itemName == keyName)
            {
                slot.UseItem();
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other) { }
    private void OnTriggerExit(Collider other) { }
}