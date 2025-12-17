using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour, IInteractable
{
    public ItemData itemData;

    [Header("---Stylish Floating---")]
    public float floatSpeed = 2f;
    public int spinSpeed = 50;

    [SerializeField] Renderer itemRenderer;

    private bool isFloating = false;
    private bool isReadyToCollect = false;
    private Vector3 targetPos;

    Material itemMat;

    void Start()
    {
        if (itemRenderer == null)
            itemRenderer = GetComponentInChildren<Renderer>();

        if (itemRenderer != null)
            itemMat = itemRenderer.material;
    }

    void Update()
    {
        if (isFloating || isReadyToCollect)
        {
            transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
            Camera.main.transform.LookAt(this.transform.position);
            GameManager.instance.cameraController.enabled = false;
        }
    }

    public void Interact()
    {
        if (!GameManager.instance.isInteracting && !isReadyToCollect)
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
        GameManager.instance.isInteracting = false;
        InventoryManager.instance.AddItemToInventory(itemData);
        GameManager.instance.ShowInventoryTutorial();
        GameManager.instance.cameraController.enabled = true;
        Destroy(gameObject);
    }

    IEnumerator FloatToCenter()
    {
        isFloating = true;

        GameManager.instance.isInteracting = true;
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

        GameManager.instance.isInteracting = false;
    }
}
