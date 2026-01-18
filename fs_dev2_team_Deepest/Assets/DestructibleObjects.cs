using System.Collections;
using UnityEngine;

public class DestructibleObjects : MonoBehaviour, IDestructible
{
    [Header("Destruction Pieces")]
    [SerializeField] GameObject[] pieces;
    [SerializeField] float explodeForce;
    [SerializeField] float radius;
    [SerializeField] float fadeSpeed;

    [Header("Audio")]
    [SerializeField] AudioSource audSource;
    [SerializeField] AudioClip breakingClip;

    [Header("Item Drop")]
    [SerializeField] GameObject itemPickup;

    bool isFading;
    bool hasDroppedItem;

    public void Destruct()
    {
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
        else
        {

        }
    }

    IEnumerator FadeAway()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}

