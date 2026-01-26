using UnityEngine;

public class Rock : MonoBehaviour, IGrab, IThrow
{
    [SerializeField] SphereCollider hitCollider;
    [SerializeField] int rockDamage;
    Rigidbody rb;
    bool isFalling = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Grab(MagicController magicController)
    {
        hitCollider.enabled = true;
        hitCollider.isTrigger = true;
        magicController.objectGrabbed = gameObject;
        magicController.throwObject = GetComponent<IThrow>();
        isFalling = false;
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
    }

    private void OnTriggerEnter(Collider other)
    {

        if(!isFalling)
        {
            IDamage dmg = other.GetComponent<IDamage>();

            if (dmg != null && !other.CompareTag("Player"))
                dmg.takeDamage(rockDamage);
        }
        

    }



}
