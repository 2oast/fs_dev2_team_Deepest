
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    bool isOpen = false;
    [SerializeField] bool isKeyDoor;

    [SerializeField]float openHeight, openSpeed;

    Vector3 closedPos, openPos;

    public void Interact()
    {
        if(isOpen)
        {
            isOpen = false;
        }
        else
        {
            isOpen = true;
        }
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

}
