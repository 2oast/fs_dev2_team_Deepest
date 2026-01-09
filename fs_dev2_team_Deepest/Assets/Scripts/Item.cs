using UnityEngine;
using System.Collections;
using NUnit.Framework.Interfaces;

public class Item : MonoBehaviour, IInteractable
{
    public ItemData item;

    [Header("---Stylish Floating---")]

    [SerializeField] float floatSpeed = 2f;
    public int spinSpeed = 50;

    [Header("Flags")]
    public bool isFloating = false;
    public bool isReadyToCollect = false;

    [Header("Position/Rotations")]
    private Vector3 targetPos;
    public Vector3 originalPos;
    public Quaternion originalRot;
    [SerializeField] Transform grabPosition;

    Material itemMat;

    void Start()
    {
        originalPos = transform.position;
        originalRot = transform.rotation;
    }

    void Update()
    {
        if (isFloating || isReadyToCollect)
        {
            transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
            GameManager.instance.isInteracting = true;
            Camera.main.transform.LookAt(this.transform.position);
            GameManager.instance.cameraControllerScript.enabled = false;
        }

        if (isReadyToCollect)
        {
            PickupMessage(this.name);
        }
    }

    public void Interact()
    {
        if (!isReadyToCollect)
        {
            StartCoroutine(FloatToCenter());
        }

    }

    public IEnumerator FloatToCenter()
    {
        isFloating = true;

        targetPos = grabPosition.position;

        Vector3 startPos = transform.position;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * floatSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, time);
            InventoryManager.instance.itemToBeCollected = this;
            yield return null;
        }
        GameManager.instance.YesOrNoObj.SetActive(true);
        isFloating = false;
        isReadyToCollect = true;

    }

    void PickupMessage(string objectName)
    {
        GameManager.instance.pickupText.text = "Pick up " + objectName + "?";
    }
}
