using UnityEngine;

public class PickUpObject : MonoBehaviour, IGrab, IThrow
{
    Rigidbody rb;
    [SerializeField] ExplosiveObject explosiveObject;
    [SerializeField] BoxCollider throwCollider;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        throwCollider = GetComponent<BoxCollider>();
    }

    public void Grab(MagicController magicController)
    {
        magicController.objectGrabbed = gameObject;
        magicController.throwObject = this.GetComponent<IThrow>();
    }

    public void Throw(MagicController magicController)
    {
        magicController.objectGrabbed.transform.SetParent(null);

        magicController.objectGrabbed = null;
        magicController.isTelegrabbing = false;

        rb.AddForce(Camera.main.transform.forward * magicController.throwForce, ForceMode.Impulse);

        magicController.audSource.PlayOneShot(magicController.throwClip);
        GameObject effect = Instantiate(magicController.throwPref, magicController.teleGrabLocation);
        Destroy(effect, 3);
        magicController.teleGrabPref.SetActive(false);
        throwCollider.enabled = true;
        if(explosiveObject != null)
            explosiveObject.isThrown = true;
    }
}
