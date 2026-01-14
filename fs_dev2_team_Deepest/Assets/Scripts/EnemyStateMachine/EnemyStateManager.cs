using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

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
    public float faceTargetSpeed;

    [Header("Projectile settings")]
    public float shootTimer;
    public int shootRate;
    public GameObject bullet;
    public Transform shootPos;
    public Transform aimTarget;
    public float projectileSpeed = 10f;

    public Vector3 playerDir;

    public enemyAI enemy;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        aimTarget = GameManager.instance.player.transform;
        enemy = GetComponent<enemyAI>();

        currentState = wanderState;

        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
       // if(!enemy.isStunned)
       // {
            shootTimer += Time.deltaTime;

            currentState.UpdateState(this);
      //  }
        
    }

    public void SwitchState(EnemyBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    public void shoot()
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

    public void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}
