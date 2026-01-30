using UnityEngine;
using UnityEngine.AI;

public class EnemyStateManager : MonoBehaviour
{
    EnemyBaseState currentState;

    public EnemyWanderState wanderState = new EnemyWanderState();
    public EnemyGrabbedState grabbedState = new EnemyGrabbedState();
    public EnemyStunnedState stunnedState = new EnemyStunnedState();
    public EnemyPursueState pursueState = new EnemyPursueState();

    [Header("Wander")]
    public float wanderTimer;
    public float wanderPause;
    public float wanderRadius;

    [Header("Navmesh")]
    public NavMeshAgent agent { get; private set; }
    public int FOV;
    public float faceTargetSpeed = 8f;

    [Header("Projectile settings")]
    public float shootTimer;
    public int shootRate;
    public GameObject bullet;
    public Transform shootPos;
    public Transform aimTarget;
    public float projectileSpeed = 10f;

    public Vector3 playerDir;

    public enemyAI enemy;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<enemyAI>();

        if (GameManager.instance != null && GameManager.instance.player != null)
            aimTarget = GameManager.instance.player.transform;

        if (agent != null)
            agent.updateRotation = false;

        currentState = wanderState;
        currentState.EnterState(this);
    }

    void Update()
    {
        if (agent == null || !agent.enabled)
            return;

        if (enemy != null && enemy.isStunned)
            return;

        shootTimer += Time.deltaTime;
        currentState.UpdateState(this);
    }

    public void SwitchState(EnemyBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    public void shoot()
    {
        shootTimer = 0f;

        if (bullet == null || shootPos == null)
            return;

        if (aimTarget == null && GameManager.instance != null && GameManager.instance.player != null)
            aimTarget = GameManager.instance.player.transform;

        if (aimTarget != null)
            faceTarget();

        GameObject proj = Instantiate(bullet, shootPos.position, Quaternion.identity);

        Vector3 targetPos = aimTarget != null ? aimTarget.position : shootPos.position + transform.forward;
        Vector3 dir = (targetPos - shootPos.position).normalized;

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = dir * projectileSpeed;
    }

    public void faceTarget()
    {
        if (aimTarget == null)
            return;

        Vector3 toPlayer = aimTarget.position - transform.position;
        toPlayer.y = 0f;

        if (toPlayer.sqrMagnitude < 0.0001f)
            return;

        playerDir = toPlayer.normalized;

        Quaternion rot = Quaternion.LookRotation(toPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}

