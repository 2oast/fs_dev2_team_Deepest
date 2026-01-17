using System.Collections;
using UnityEngine;

public class DestructibleObjects : MonoBehaviour, IDestructible
{ 
    [SerializeField] GameObject[] pieces;
    [SerializeField] float explodeForce;
    [SerializeField] float radius;
    [SerializeField] float fadeSpeed;

    [SerializeField] AudioSource audSource;
    [SerializeField] AudioClip breakingClip;
    

    Material mat;
    Color color;

    bool isFading;

    

    public void Destruct()
    {
        Collider rootCol = GetComponent<Collider>();
        if (rootCol) rootCol.enabled = false;
        audSource.PlayOneShot(breakingClip);
        foreach (GameObject piece in pieces)
        {
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            Collider col = piece.GetComponent<Collider>();
            Renderer rend = piece.GetComponent<Renderer>();


            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddExplosionForce(explodeForce, rb.transform.position, radius);
            col.enabled = true;
            rb.WakeUp();
            StartCoroutine(FadeAway());
        }

    }

   

    IEnumerator FadeAway()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
        
    }

    
}
