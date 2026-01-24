using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] GameObject explosionPref;
    [SerializeField] float explodeForce;

    private void OnTriggerEnter(Collider other)
    {
        IDamage dmg = other.GetComponent<IDamage>();
        IExplode exp = other.GetComponent<IExplode>();

        GameObject explosion = Instantiate(explosionPref);

        if (exp != null)
            exp.Explode();

        if(dmg != null && !other.CompareTag("Player"))
        {
            dmg.takeDamage(damage);
            Destroy(gameObject);
        }

        Rigidbody rb = other.GetComponent<Rigidbody>();
        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        Vector3 dir = Camera.main.transform.forward;

        switch (WeaponManager.instance.currentRingEquipped.spellType)
        {
            case SpellType.Fire:
                Destroy(explosion, 1);
                if(rb != null && !other.CompareTag("Player"))
                {
                    rb.AddForce(dir * explodeForce, ForceMode.Impulse);
                }
                else if(rb != null && agent != null)
                {
                    agent.enabled = false;
                    rb.isKinematic = false;
                    rb.AddForce(dir * explodeForce, ForceMode.Impulse);
                    StartCoroutine(ReenableAgent(agent, rb));
                }
                break;

        }
    }
    
    IEnumerator ReenableAgent(NavMeshAgent agent, Rigidbody rb)
    {
        yield return new WaitForSeconds(1f);
        if (agent != null)
            agent.enabled = true;
        rb.isKinematic = true;
    }
}
