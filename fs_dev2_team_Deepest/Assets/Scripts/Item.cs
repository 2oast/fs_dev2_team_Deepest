using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour, IInteractable
{
    public ItemData itemData;

    [Header("---Stylish Floating---")]
    public float floatSpeed = 2f;
    public int spinSpeed = 50;

    private bool isFloating = false;
    private bool isReadyToCollect = false;
    private bool isInspecting = false;
    private Vector3 targetPos;

    void Update()
    {
<<<<<<< HEAD
        if (isFloating || isReadyToCollect)
        {
            transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
            Camera.main.transform.LookAt(this.transform.position);
            GameManager.instance.cameraController.enabled = false;
        }
=======
        if (isInspecting)
            transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
>>>>>>> 2749fabcb7acc920fcdb7825b3ae917952e0d730
    }

    public void Interact()
    {
        if (!isInspecting && !isReadyToCollect)
        {
            StartCoroutine(FloatToCenter());
        }
        else if (isReadyToCollect)
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        isInspecting = false;

        InventoryManager.instance.AddItemToInventory(itemData);
<<<<<<< HEAD
        GameManager.instance.cameraController.enabled = true;
=======

>>>>>>> 2749fabcb7acc920fcdb7825b3ae917952e0d730
        Destroy(gameObject);
    }

    IEnumerator FloatToCenter()
    {
        isFloating = true;
        isInspecting = true;
        targetPos = GameManager.instance.playerGrabPosition.position;

        Vector3 startPos = transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * floatSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        isFloating = false;
        isReadyToCollect = true;

    }
}
