using System.Collections.Generic;
using UnityEngine;

public class BossStateManager : MonoBehaviour
{

    BossBaseState currentState;
    public BossBaseState nextState;

    public ChangeLocationState changeLocationState = new ChangeLocationState();
    public ChargeAttackState chargeAttackState = new ChargeAttackState();
    public SpecialAttackState specialAttackState = new SpecialAttackState();
    public WebAttackState webAttackState = new WebAttackState();
    public StunnedState stunnedState = new StunnedState();

    public Dictionary<int, BossBaseState> stateTypeDic;
    public List<Material> mats;

    public Material stunMat;
    public Material redMat;

    public Renderer model;


    public float turnSpeed;
    public float moveSpeed;
    public int randomState;

    public Transform[] locations;
    public Transform[] rockSpawnLocations;
    public Transform nextPos;
    public Transform centerPos;
    public int index;
    public float waitTimer = 0;
    

    [Header("Projectile settings")]
    public float shootTimer;
    public int shootRate;
    public GameObject bullet;
    public GameObject rockPref;
    public Transform shootPos;
    public Transform aimTarget;
    public float projectileSpeed = 20f;

    public BoxCollider hitCollider;
    public BoxCollider damageCollider;
    public BoxCollider rockCollider;

    public BossStats boss;

    public Animator animator;

    public bool isMoving;
    public bool isStunned;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mats = new List<Material>(model.materials);


        stateTypeDic = new Dictionary<int, BossBaseState>()
        {
            {1, chargeAttackState },
            {2, webAttackState },
            {3, specialAttackState }
        };

        currentState = changeLocationState;

        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
        shootTimer += Time.deltaTime;


        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsStunned", isStunned);
    }

    public void SwitchState(BossBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    public void faceTarget(Vector3 position)
    {
        Vector3 dir = position - transform.position;
        dir.y = 0f; // keep boss upright

        if (dir.sqrMagnitude < 0.0001f)
            return;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            rot,
            turnSpeed * Time.deltaTime
        );
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
        
        if(rb)
        {
            rb.linearVelocity = dir * projectileSpeed;
        }
        
    }

    public void SpawnRocks()
    {
        shootTimer = 0;
        foreach (Transform location in rockSpawnLocations)
        {
            Instantiate(rockPref, location);
        }
    }
}
