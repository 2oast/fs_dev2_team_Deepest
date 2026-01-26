using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    bool isOpen = false;
    bool isLocked = false;

    [SerializeField] bool isKeyDoor;
    [SerializeField] KeyItem keyNeeded;

    [SerializeField] float openHeight, openSpeed;

    Vector3 closedPos, openPos;

    public void Interact()
    {
        if (isLocked)
            return;

        isOpen = !isOpen;
    }

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * openHeight;
    }

    void Update()
    {
        Vector3 target = isOpen ? openPos : closedPos;
        transform.position = Vector3.MoveTowards(transform.position, target, openSpeed * Time.deltaTime);
    }

    public void LockAndClose()
    {
        isLocked = true;
        isOpen = false;
    }

    public void UnlockAndOpen()
    {
        isLocked = false;
        isOpen = true;
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public bool IsLocked()
    {
        return isLocked;
    }
}
