using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Movement")]
    [SerializeField] float openHeight = 4f;
    [SerializeField] float moveSpeed = 2f;

    [Header("Key Settings")]
    [SerializeField] bool requiresKey = false;
    [SerializeField] string requiredKeyName;
    [SerializeField] bool consumeKeyOnUse = false;

    [Header("State")]
    [SerializeField] bool startsLocked = false;
    [SerializeField] bool startsClosed = true;

    Vector3 closedPos;
    Vector3 openPos;

    bool isLocked;
    bool isOpen;
    bool isMoving;

    Coroutine moveRoutine;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * openHeight;

        isLocked = startsLocked;
        isOpen = !startsClosed;

        if (isOpen)
            transform.position = openPos;
        else
            transform.position = closedPos;
    }

    public void Interact()
    {
        if (isMoving || isOpen)
            return;

        if (isLocked)
            return;

        if (requiresKey)
        {
            if (string.IsNullOrEmpty(requiredKeyName))
                return;

            if (!PlayerHasKey(requiredKeyName))
                return;

            if (consumeKeyOnUse)
                ConsumeKey(requiredKeyName);
        }

        Open();
    }

    public void Lock()
    {
        isLocked = true;
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public void LockAndClose()
    {
        isLocked = true;
        Close();
    }

    public void UnlockAndOpen()
    {
        isLocked = false;
        Open();
    }

    public void Open()
    {
        if (isOpen || isMoving)
            return;

        StartMove(openPos, true);
    }

    public void Close()
    {
        if (!isOpen || isMoving)
            return;

        StartMove(closedPos, false);
    }

    void StartMove(Vector3 target, bool opening)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveDoor(target, opening));
    }

    IEnumerator MoveDoor(Vector3 target, bool opening)
    {
        isMoving = true;

        Vector3 start = transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
        isOpen = opening;
        isMoving = false;
        moveRoutine = null;
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
