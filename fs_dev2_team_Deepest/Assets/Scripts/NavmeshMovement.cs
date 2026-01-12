using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class NavmeshMovement : MonoBehaviour
{

    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] NavMeshAgent agent;



    [SerializeField] float wanderRadius = 8f;
    [SerializeField] float wanderPause = 2f;


    Color colorOrig;

    float angleToPlayer;

    bool waitingToWander;
    float wanderTimer;

    bool playerInRange;

    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {


        if (GameManager.instance == null || GameManager.instance.player == null)
            return;

            bool seeingPlayer = canSeePlayer();


                if (!seeingPlayer)
                {
                    Wander();
                }
            
        
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
        if (agent)
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

}