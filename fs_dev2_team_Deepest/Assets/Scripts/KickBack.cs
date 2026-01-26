using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class KickBack : MonoBehaviour
{
    BoxCollider kickCollider;
    [SerializeField] float kickForce;
    AudioSource audSource;
    [SerializeField] AudioClip kickSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        kickCollider = GetComponent<BoxCollider>();
        audSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        Vector3 dir = (other.transform.position - GameManager.instance.player.transform.position).normalized;
        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();

        if(rb != null)
        {
            if(agent != null)
            {
                StartCoroutine(KickKinematic(rb, agent, dir));
            }
            else
            {
                rb.AddForce(-dir * kickForce, ForceMode.Impulse);

            }
        }

        IDestructible destruct = other.GetComponent<IDestructible>();
        if (destruct != null)
            destruct.Destruct();

        audSource.PlayOneShot(kickSound);
    }

    IEnumerator KickKinematic(Rigidbody rb, NavMeshAgent agent, Vector3 dir)
    {
        rb.isKinematic = false;
        agent.enabled = false;

        rb.AddForce(-dir * kickForce, ForceMode.Impulse);

        yield return new WaitForSeconds(2);

        rb.isKinematic = true;
        agent.enabled = true;

    }
}
