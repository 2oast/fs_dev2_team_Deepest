using System.Collections;
using UnityEngine;

public class DestructibleObjects : MonoBehaviour, IDestructible
{
    [Header("Save State")]
    [Tooltip("Unique ID for THIS crate. Must be unique across the whole scene.")]
    public string id;

    [Header("Destruction Pieces")]
    [SerializeField] GameObject[] pieces;
    [SerializeField] float explodeForce;
    [SerializeField] float radius;
    [SerializeField] float fadeSpeed;

    [Header("Audio")]
    [SerializeField] AudioSource audSource;
    [SerializeField] AudioClip breakingClip;

    [Header("Item Drop")]
    [Tooltip("This pickup should usually start DISABLED in the scene, as a child of the crate.")]
    [SerializeField] GameObject itemPickup;

    bool isFading;
    bool hasDroppedItem;

    bool isBroken;

    public bool IsBroken { get { return isBroken; } }
    public bool HasDroppedItem { get { return hasDroppedItem; } }
    public GameObject ItemPickup { get { return itemPickup; } }

    public void Destruct()
    {
        if (isBroken)
            return;

        isBroken = true;

        Collider rootCol = GetComponent<Collider>();
        if (rootCol) rootCol.enabled = false;

        if (audSource != null && breakingClip != null)
        {
            audSource.PlayOneShot(breakingClip);
        }

        foreach (GameObject piece in pieces)
        {
            if (piece == null)
                continue;

            Rigidbody rb = piece.GetComponent<Rigidbody>();
            Collider col = piece.GetComponent<Collider>();

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.AddExplosionForce(explodeForce, rb.transform.position, radius);
                rb.WakeUp();
            }

            if (col != null)
            {
                col.enabled = true;
            }
        }

        ActivatePickup();

        StartCoroutine(FadeAway());
    }

    void ActivatePickup()
    {
        if (hasDroppedItem)
            return;

        if (itemPickup != null)
        {
            itemPickup.transform.SetParent(null);

            itemPickup.SetActive(true);
            hasDroppedItem = true;
        }
    }

    IEnumerator FadeAway()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    public void GetSaveState(out bool broken, out bool pickupDropped)
    {
        broken = isBroken;
        pickupDropped = hasDroppedItem;
    }

    public void ApplySaveState(bool broken, bool pickupCollected)
    {
        isBroken = broken;

        if (!broken)
        {
            if (itemPickup != null)
            {
                if (itemPickup.transform.parent != transform)
                    itemPickup.transform.SetParent(transform);

                if (!pickupCollected)
                    itemPickup.SetActive(false);
                else
                    itemPickup.SetActive(false);
            }

            hasDroppedItem = false;
            return;
        }

        if (pickupCollected)
        {
            if (itemPickup != null)
            {
                Destroy(itemPickup);
            }
        }
        else
        {
            ActivatePickup();
        }

        Destroy(gameObject);
    }

    public void ResetStateForNewGame()
    {
        isBroken = false;
        hasDroppedItem = false;

        if (itemPickup != null)
        {
            itemPickup.transform.SetParent(transform);
            itemPickup.SetActive(false);
        }
    }
}


