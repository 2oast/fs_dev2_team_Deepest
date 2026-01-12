using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage, IGrab, ITeleport
{
    [SerializeField] int maxHp;
    [SerializeField] int HP;
    [SerializeField] Renderer model;
    public NavMeshAgent agent;
    [SerializeField] Animator anim;

    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;

    [SerializeField] Transform aimTarget;
    [SerializeField] float projectileSpeed = 10f;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip spitSound;
    [SerializeField] AudioClip hurtSound;

    [SerializeField] float wanderRadius = 8f;
    [SerializeField] float wanderPause = 2f;

    public bool isStunned;
    public bool isGrabbed;

    Color colorOrig;

    float shootTimer;
    float angleToPlayer;

    bool waitingToWander;
    float wanderTimer;

    bool playerInRange;

    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
        shootTimer += Time.deltaTime;

        if (GameManager.instance == null || GameManager.instance.player == null)
            return;

        if (HP < maxHp / 2)
        {
            isStunned = true;
        }

        if (agent!= null)
        {
            bool seeingPlayer = canSeePlayer();
            

            if (!isStunned)
            {
                if (!seeingPlayer)
                {
                    Wander();
                    UpdateAnim();
                }
            }
        }
    }

    void UpdateAnim()
    {
        if (anim == null || agent == null)
            return;

        bool moving = agent.velocity.magnitude > 0.1f;
        anim.SetBool("isMoving", moving);
    }

    void Wander()
    {
        if (waitingToWander)
        {
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderPause)
            {
                waitingToWander = false;
                wanderTimer = 0f;
            }
            else
                return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 random = Random.insideUnitSphere * wanderRadius + transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(random, out hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                waitingToWander = true;
            }
        }
    }

    bool canSeePlayer()
    {
        if(agent)
        {

        }
        playerDir = GameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(transform.forward, playerDir);


        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                if (shootTimer >= shootRate)
                {
                    shoot();
                    if (audioSource != null && spitSound != null)
                    {
                        audioSource.PlayOneShot(spitSound);
                    }
                }
                return true;
            }
        }

        return false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void shoot()
    {
        shootTimer = 0;

        GameObject proj = Instantiate(bullet, shootPos.position, Quaternion.identity);

        Vector3 targetPos;

        if (aimTarget != null)
        {
            targetPos = aimTarget.position;
        }
        else
        {
            targetPos = GameManager.instance.player.transform.position;
        }

        Vector3 dir = (targetPos - shootPos.position).normalized;

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = dir * projectileSpeed;
        }
    }

    public void takeDamage(int amount)
    {
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
        magicController.objectGrabbed = gameObject;
    }

    public IEnumerator ReenableAgentAfterThrow(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (agent != null)
            agent.enabled = true;
    }
}