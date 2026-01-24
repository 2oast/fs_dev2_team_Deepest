using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ExplosiveObject : MonoBehaviour, IExplode
{
    SphereCollider explosionCollider;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;
    Rigidbody rb;

    [SerializeField] int explosionDamage = 30;
    [SerializeField] AudioSource audSource;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] GameObject explosionPref;
    [SerializeField] GameObject firePref;
    [SerializeField] float explodeForce = 10f;
    [SerializeField] float explodeRadius = 5f;

    bool exploded = false;
    public bool isThrown = false;

    void Start()
    {
        explosionCollider = GetComponent<SphereCollider>();
        meshRenderer = GetComponent<MeshRenderer>();

        explosionCollider.enabled = false;
        explosionCollider.isTrigger = true;
        meshCollider = GetComponent<MeshCollider>();
        rb = GetComponent<Rigidbody>();
    }

    void BlowUp()
    {
        exploded = true;
        explosionCollider.enabled = true;
        meshCollider.enabled = false;
        rb.isKinematic = true;
        isThrown = false;
        // Visuals
        meshRenderer.enabled = false;
        audSource.PlayOneShot(explosionSound);

        GameObject explodeEffect = Instantiate(explosionPref, transform);
        GameObject fireEffect = Instantiate(firePref, transform);

        Destroy(explodeEffect, 2);
        // Enable explosion hitbox
        explosionCollider.enabled = true;

        StartCoroutine(FadeFire(10, fireEffect));
    }

    private void OnTriggerEnter(Collider other)
    {

        IDamage dmg = other.GetComponent<IDamage>();
        Rigidbody rb = other.GetComponent<Rigidbody>();
        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();

        if (dmg != null)
            dmg.takeDamage(explosionDamage);

        if (agent != null)
            agent.enabled = false;

        if (rb != null)
            rb.AddExplosionForce(explodeForce, transform.position, explodeRadius);

        if (agent != null)
            StartCoroutine(ReenableAgent(agent));

        if(isThrown)
        {
            BlowUp();
        }
    }

    IEnumerator ReenableAgent(NavMeshAgent agent)
    {
        yield return new WaitForSeconds(1f);
        if (agent != null)
            agent.enabled = true;
    }

    IEnumerator FadeFire(float duration, GameObject fireEffect)
    {
        Vector3 targetScale = new Vector3(0, 0, 0);
        Vector3 startScale = fireEffect.transform.localScale;
        float t = 0;

        while (t < 10)
        {
            t += Time.deltaTime;
            fireEffect.transform.localScale = Vector3.MoveTowards(startScale, targetScale, t);
            yield return null;
        }
        Destroy(fireEffect);
        Destroy(gameObject);
    }

    public void Explode()
    {
        BlowUp();
    }
}
