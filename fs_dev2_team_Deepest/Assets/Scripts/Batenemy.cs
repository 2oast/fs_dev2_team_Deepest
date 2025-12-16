using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class batEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] int meleeDamage = 10;
    [SerializeField] float attackRate = 4f;
    [SerializeField] float attackRange = 2f;

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int faceTargetSpeed = 5;
    [SerializeField] int FOV = 90;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip biteSound;

    [SerializeField] float wanderRadius = 10f;
    [SerializeField] float wanderTimer = 5f;

    Color colorOrig;

    float attackTimer;
    float angleToPlayer;
    bool isAttacking;
    bool isWandering;
    Vector3 playerDir;
    Vector3 wanderTarget;

    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.UpdateGameGoal(1);

        StartCoroutine(Wander());
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        if (GameManager.instance == null || GameManager.instance.player == null)
            return;

        bool seeingPlayer = CanSeePlayer();

        if (seeingPlayer)
        {
            StopCoroutine(Wander());
            isWandering = false;

            Vector3 playerPos = GameManager.instance.player.transform.position;
            agent.SetDestination(playerPos);

            float distToPlayer = Vector3.Distance(transform.position, playerPos);

            if (distToPlayer <= attackRange)
            {
                faceTarget();

                if (!isAttacking && attackTimer >= attackRate)
                {
                    StartCoroutine(MeleeAttack());
                }
            }
        }
        else if (!isWandering)
        {
            StartCoroutine(Wander());
            isWandering = true;
        }
    }

    bool CanSeePlayer()
    {
        Vector3 playerPos = GameManager.instance.player.transform.position;

        playerDir = playerPos - transform.position;
        angleToPlayer = Vector3.Angle(transform.forward, playerDir);

        if (angleToPlayer > FOV)
            return false;

        Vector3 origin = transform.position + Vector3.up * 0.5f;

        RaycastHit hit;
        if (Physics.Raycast(origin, playerDir.normalized, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    void faceTarget()
    {
        Vector3 targetPos = GameManager.instance.player.transform.position;
        targetPos.y = transform.position.y;

        Quaternion rot = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    IEnumerator MeleeAttack()
    {
        isAttacking = true;
        attackTimer = 0f;

        if (audioSource != null && biteSound != null)
        {
            audioSource.PlayOneShot(biteSound);
        }

        if (GameManager.instance != null && GameManager.instance.playerScript != null)
        {
            float distToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

            if (distToPlayer <= attackRange)
            {
                GameManager.instance.playerScript.takeDamage(meleeDamage);
            }
        }

        isAttacking = false;
        yield return null;
    }

    IEnumerator Wander()
    {
        while (!CanSeePlayer())
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
            {
                wanderTarget = hit.position;
                agent.SetDestination(wanderTarget);
            }

            yield return new WaitForSeconds(wanderTimer);
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            GameManager.instance.UpdateGameGoal(-1);
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
}
