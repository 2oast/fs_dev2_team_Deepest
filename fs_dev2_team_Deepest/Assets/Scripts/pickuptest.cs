using UnityEngine;

public class pickuptest : MonoBehaviour, IGrab
{
    public bool isGrabbed;

    public void Grab(MagicController magicController)
    {
        isGrabbed = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        magicController.objectGrabbed = gameObject;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
