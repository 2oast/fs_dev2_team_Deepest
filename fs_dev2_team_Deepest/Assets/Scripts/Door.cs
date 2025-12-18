using TreeEditor;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    bool isOpen = false;
    [SerializeField] bool isKeyDoor;

    [SerializeField]float openHeight, openSpeed;

    Vector3 closedPos, openPos;

    public void Interact()
    {
        if(!isKeyDoor)
        {
            if (!isOpen)
                isOpen = true;
            else
                isOpen = false;
        }
        else
        {
            if(GameManager.instance.keyEquipped)
            {
                if (!isOpen)
                    isOpen = true;
            }
        }

        if (isOpen && GameManager.instance != null)
            GameManager.instance.ShowDoorPrompt(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * openHeight;
    }

    // Update is called once per frame
    void Update()
    {
        if(isOpen)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPos, openSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, closedPos, openSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.instance != null && !isOpen)
            GameManager.instance.ShowDoorPrompt(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.instance != null)
            GameManager.instance.ShowDoorPrompt(false);
    }

}
