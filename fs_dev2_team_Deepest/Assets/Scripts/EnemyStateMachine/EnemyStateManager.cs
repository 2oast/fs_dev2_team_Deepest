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
    public int FOV = 90;
    public float faceTargetSpeed = 8f;

    [Header("Aggro")]
    public float aggroRange = 12f;

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

    public void EngageTarget(Transform target)
    {
        if (target == null)
            return;

        aimTarget = target;

        if (currentState != pursueState)
            SwitchState(pursueState);
    }

    public bool IsTargetInRange()
    {
        if (aimTarget == null)
            return false;

        float dist = Vector3.Distance(transform.position, aimTarget.position);
        return dist <= aggroRange;
    }

    public bool CanSeeTarget()
    {
        if (aimTarget == null)
            return false;

        Vector3 toTarget = aimTarget.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f)
            return true;

        float angle = Vector3.Angle(transform.forward, toTarget.normalized);
        return angle <= (FOV * 0.5f);
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

        Vector3 toTarget = aimTarget.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f)
            return;

        playerDir = toTarget.normalized;

        Quaternion rot = Quaternion.LookRotation(toTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}
