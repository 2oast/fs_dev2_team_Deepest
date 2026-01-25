using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    public enum EnemyType
    {
        RangedSpitter,
        GhostMelee,
        BatMelee
    }

    [Header("General")]
    [SerializeField] EnemyType enemyType;
    public int HP = 50;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    [SerializeField] int faceTargetSpeed = 5;
    [SerializeField] int FOV = 90;

    [Header("Wandering")]
    [SerializeField] float wanderRadius = 8f;
    [SerializeField] float wanderPause = 2f;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hurtSound;

    [Header("Slime")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float shootRate = 2f;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform aimTarget;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] AudioClip spitSound;

    [Header("Melee")]
    [SerializeField] int meleeDamage = 10;
    [SerializeField] float attackRate = 4f;
    [SerializeField] float attackRange = 2f;

    [Header("Ghost")]
    [SerializeField] GameObject ghostHand;
    [SerializeField] Transform handStartPos;
    [SerializeField] Transform handEndPos;
    [SerializeField] float handSwipeTime = 0.25f;
    [SerializeField] AudioClip ghostSlapSound;

    [Header("Bat")]
    [SerializeField] AudioClip batBiteSound;

    Color colorOrig;

    float shootTimer;
    float attackTimer;
    float angleToPlayer;

    bool waitingToWander;
    float wanderTimer;

    bool isAttacking;
    bool playerInRange;

    Vector3 playerDir;

    void Start()
    {
        if (model != null)
            colorOrig = model.material.color;

        if (enemyType == EnemyType.GhostMelee && ghostHand != null)
        {
            ghostHand.SetActive(false);
        }
    }

    void Update()
    {
        if (GameManager.instance == null || GameManager.instance.player == null)
            return;

        shootTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        bool seeingPlayer = CanSeePlayer();

        switch (enemyType)
        {
            case EnemyType.RangedSpitter:
                UpdateRanged(seeingPlayer);
                break;

            case EnemyType.GhostMelee:
                UpdateGhost(seeingPlayer);
                break;

            case EnemyType.BatMelee:
                UpdateBat(seeingPlayer);
                break;
        }

        UpdateAnim();
    }


    bool CanSeePlayer()
    {
        Transform player = GameManager.instance.player.transform;

        Vector3 playerPos = player.position;
        playerDir = playerPos - transform.position;
        angleToPlayer = Vector3.Angle(transform.forward, playerDir);

        if (angleToPlayer > FOV)
            return false;

        Vector3 origin = transform.position + Vector3.up * 0.5f;

        RaycastHit hit;
        if (Physics.Raycast(origin, playerDir.normalized, out hit))
        {
            if (hit.collider.CompareTag("Player"))
                return true;
        }

        return false;
    }

    void Wander()
    {
        if (agent == null)
            return;

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

    void FaceTarget()
    {
        Vector3 targetPos = GameManager.instance.player.transform.position;
        targetPos.y = transform.position.y;

        Quaternion rot = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void UpdateAnim()
    {
        if (anim == null || agent == null)
            return;

        bool moving;

        if (enemyType == EnemyType.GhostMelee || enemyType == EnemyType.BatMelee)
        {
            moving = agent.desiredVelocity.magnitude > 0.05f && !isAttacking;
        }
        else
        {
            moving = agent.velocity.magnitude > 0.1f;
        }

        anim.SetBool("isMoving", moving);
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


    void UpdateRanged(bool seeingPlayer)
    {
        if (seeingPlayer)
        {
            Transform player = GameManager.instance.player.transform;

            if (agent != null)
            {
                agent.SetDestination(player.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }
            }

            if (shootTimer >= shootRate)
            {
                Shoot();
                if (audioSource != null && spitSound != null)
                {
                    audioSource.PlayOneShot(spitSound);
                }
            }
        }
        else
        {
            Wander();
        }
    }

    void Shoot()
    {
        shootTimer = 0f;

        if (bulletPrefab == null || shootPos == null)
            return;

        GameObject proj = Instantiate(bulletPrefab, shootPos.position, Quaternion.identity);

        Vector3 targetPos;
        if (aimTarget != null)
            targetPos = aimTarget.position;
        else
            targetPos = GameManager.instance.player.transform.position;

        Vector3 dir = (targetPos - shootPos.position).normalized;

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = dir * projectileSpeed;
        }
    }


    void UpdateGhost(bool seeingPlayer)
    {
        Transform player = GameManager.instance.player.transform;
        Vector3 playerPos = player.position;

        if (seeingPlayer)
        {
            if (agent != null)
                agent.SetDestination(playerPos);

            float distToPlayer = Vector3.Distance(transform.position, playerPos);

            if (distToPlayer <= attackRange)
            {
                FaceTarget();

                if (!isAttacking && attackTimer >= attackRate)
                {
                    StartCoroutine(GhostMeleeAttack());
                }
            }
        }
        else
        {
            Wander();
        }
    }

    IEnumerator GhostMeleeAttack()
    {
        isAttacking = true;
        attackTimer = 0f;

        if (ghostHand != null)
        {
            ghostHand.SetActive(true);

            Vector3 startPos = handStartPos != null ? handStartPos.position : ghostHand.transform.position;
            Vector3 endPos = handEndPos != null ? handEndPos.position : ghostHand.transform.position + transform.forward * 0.5f;

            float t = 0f;
            while (t < handSwipeTime)
            {
                t += Time.deltaTime;
                float lerp = t / handSwipeTime;
                ghostHand.transform.position = Vector3.Lerp(startPos, endPos, lerp);
                yield return null;
            }
        }

        if (GameManager.instance != null && GameManager.instance.playerControllerScript != null)
        {
            float distToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

            if (distToPlayer <= attackRange + 0.5f)
            {
                GameManager.instance.playerControllerScript.takeDamage(meleeDamage);

                if (audioSource != null && ghostSlapSound != null)
                {
                    audioSource.PlayOneShot(ghostSlapSound);
                }
            }
        }

        if (ghostHand != null)
        {
            if (handStartPos != null)
                ghostHand.transform.position = handStartPos.position;

            ghostHand.SetActive(false);
        }

        isAttacking = false;
    }


    void UpdateBat(bool seeingPlayer)
    {
        Transform player = GameManager.instance.player.transform;
        Vector3 playerPos = player.position;

        if (seeingPlayer)
        {
            if (agent != null)
                agent.SetDestination(playerPos);

            float distToPlayer = Vector3.Distance(transform.position, playerPos);

            if (distToPlayer <= attackRange)
            {
                FaceTarget();

                if (!isAttacking && attackTimer >= attackRate)
                {
                    StartCoroutine(BatAttack());
                }
            }
        }
        else
        {
            Wander();
        }
    }

    IEnumerator BatAttack()
    {
        isAttacking = true;
        attackTimer = 0f;

        if (anim != null)
            anim.SetTrigger("attack");

        if (audioSource != null && batBiteSound != null)
        {
            audioSource.PlayOneShot(batBiteSound);
        }

        if (GameManager.instance != null && GameManager.instance.playerControllerScript != null)
        {
            float distToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

            if (distToPlayer <= attackRange)
            {
                GameManager.instance.playerControllerScript.takeDamage(meleeDamage);
            }
        }

        isAttacking = false;
        yield return null;
    }


    public void takeDamage(int amount)
    {
        HP -= amount;

        if (GameManager.instance != null && GameManager.instance.player != null && agent != null)
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
        }

        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound, 0.5f);
        }

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator FlashRed()
    {
        if (model != null)
        {
            model.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            model.material.color = colorOrig;
        }
        else
        {
            yield return null;
        }
    }
}
