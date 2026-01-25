using System.Collections;
using UnityEngine;

public class DestructibleObjects : MonoBehaviour, IDestructible
{
    [Header("Save ID (unique per crate)")]
    public string id;

    [Header("Destruction Pieces")]
    [SerializeField] GameObject[] pieces;
    [SerializeField] float explodeForce = 150f;
    [SerializeField] float radius = 2f;

    [Header("Audio")]
    [SerializeField] AudioSource audSource;
    [SerializeField] AudioClip breakingClip;

    [Header("Item Drop")]
    [SerializeField] GameObject itemPickup;

    [Header("Crate Visual Root (the intact crate mesh)")]
    [SerializeField] GameObject intactCrateRoot;

    [Header("Cleanup")]
    [SerializeField] float destroyPiecesAfter = 3f;

    [SerializeField] bool isBroken;
    public bool IsBroken { get { return isBroken; } }

    bool hasDroppedItem;

    void Reset()
    {
        intactCrateRoot = gameObject;
    }

    void Awake()
    {
        if (intactCrateRoot == null)
            intactCrateRoot = gameObject;

        if (!isBroken)
        {
            SetPiecesActive(false);
        }
    }

    public void ApplyState(bool broken)
    {
        isBroken = broken;

        if (isBroken)
        {
            SetIntactCrateActive(false);
            SetPiecesActive(false);

            hasDroppedItem = true;

            if (itemPickup != null)
            {
                itemPickup.transform.SetParent(null);
                itemPickup.SetActive(true);
            }
        }
        else
        {
            SetIntactCrateActive(true);
            SetPiecesActive(false);
            hasDroppedItem = false;

            if (itemPickup != null)
            {
                itemPickup.SetActive(false);
            }
        }
    }

    public void Destruct()
    {
        if (isBroken)
            return;

        isBroken = true;

        SetIntactCrateActive(false);

        if (audSource != null && breakingClip != null)
            audSource.PlayOneShot(breakingClip);

        BreakPieces();

        ActivatePickup();

        if (destroyPiecesAfter > 0f)
            StartCoroutine(CleanupPieces());
    }

    void SetIntactCrateActive(bool active)
    {
        if (intactCrateRoot == null)
            return;

        intactCrateRoot.SetActive(active);
    }

    void SetPiecesActive(bool active)
    {
        if (pieces == null)
            return;

        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i] != null)
                pieces[i].SetActive(active);
        }
    }

    void BreakPieces()
    {
        if (pieces == null)
            return;

        for (int i = 0; i < pieces.Length; i++)
        {
            GameObject piece = pieces[i];
            if (piece == null)
                continue;

            piece.SetActive(true);

            Rigidbody rb = piece.GetComponent<Rigidbody>();
            Collider col = piece.GetComponent<Collider>();

            if (col != null)
                col.enabled = true;

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.AddExplosionForce(explodeForce, piece.transform.position, radius);
                rb.WakeUp();
            }
        }
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

    IEnumerator CleanupPieces()
    {
        yield return new WaitForSeconds(destroyPiecesAfter);

        if (pieces != null)
        {
            for (int i = 0; i < pieces.Length; i++)
            {
                if (pieces[i] != null)
                    Destroy(pieces[i]);
            }
        }
    }
}

