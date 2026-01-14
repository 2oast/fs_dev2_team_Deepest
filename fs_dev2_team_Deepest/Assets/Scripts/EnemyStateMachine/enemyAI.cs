using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System;

public class enemyAI : MonoBehaviour, IDamage, IGrab, ITeleport
{
    [SerializeField] int maxHp;
    [SerializeField] int HP;
    [SerializeField] Renderer model;
    [SerializeField] Animator anim;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip spitSound;
    [SerializeField] AudioClip hurtSound;

    public bool isStunned;
    public bool isGrabbed;
    bool isHit;

    Color colorOrig;

    public static event Action OnEnemyHurt;
    public static event Action OnEnemyGrab;

    Vector3 scaleOrig;

    NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scaleOrig = transform.localScale;
        colorOrig = model.material.color;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HP < maxHp / 2)
        {
            isStunned = true;
        }
    }


    public void takeDamage(int amount)
    {
        OnEnemyHurt?.Invoke();
        HP -= amount;
        agent.SetDestination(GameManager.instance.player.transform.position);
        audioSource.PlayOneShot(hurtSound, .5f);
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    public void Teleport()
    {
    }

    public void Grab(MagicController magicController)
    {
        isGrabbed = true;

        if (agent != null)
            agent.enabled = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        magicController.objectGrabbed = gameObject;
    }

    

}