using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class ghostEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] int meleeDamage = 10;
    [SerializeField] float attackRate = 4f;
    [SerializeField] float attackRange = 2f;

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int faceTargetSpeed = 5;
    [SerializeField] int FOV = 90;

    [SerializeField] GameObject ghostHand;
    [SerializeField] Transform handStartPos;
    [SerializeField] Transform handEndPos;
    [SerializeField] float handSwipeTime = 0.25f;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip slapSound;

    Color colorOrig;

    float attackTimer;
    float angleToPlayer;
    bool isAttacking;

    Vector3 playerDir;

    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.UpdateGameGoal(1);

        if (ghostHand != null)
            ghostHand.SetActive(false);
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        if (GameManager.instance == null || GameManager.instance.player == null)
            return;

        bool seeingPlayer = CanSeePlayer();

        if (seeingPlayer)
        {
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

        if (GameManager.instance != null && GameManager.instance.playerScript != null)
        {
            float distToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

            if (distToPlayer <= attackRange + 0.5f)
            {
                GameManager.instance.playerScript.takeDamage(meleeDamage);
                if (audioSource != null && slapSound != null)
                {
                    audioSource.PlayOneShot(slapSound);
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
