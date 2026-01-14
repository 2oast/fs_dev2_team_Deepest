using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class EnemyWanderState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager manager)
    {
        
    }

    public override void OnCollisionEnter(EnemyStateManager manager)
    {
        
    }

    public override void UpdateState(EnemyStateManager manager)
    {
        if (manager.agent.enabled)
        {
            manager.wanderTimer += Time.deltaTime;
            if (manager.wanderTimer >= manager.wanderPause)
            {
                manager.wanderTimer = 0f;
            }
            else
                return;

            if (!manager.agent.pathPending && manager.agent.remainingDistance <= manager.agent.stoppingDistance)
            {
                Vector3 random = Random.insideUnitSphere * manager.wanderRadius + manager.transform.position;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(random, out hit, manager.wanderRadius, NavMesh.AllAreas))
                {
                    manager.agent.SetDestination(hit.position);
                }
            }

            //checking to see if player is visible
            manager.playerDir = GameManager.instance.player.transform.position - manager.transform.position;
            float angleToPlayer = Vector3.Angle(manager.transform.forward, manager.playerDir);


            RaycastHit rayHit;
            if (Physics.Raycast(manager.transform.position, manager.playerDir, out rayHit))
            {
                if (angleToPlayer <= manager.FOV && rayHit.collider.CompareTag("Player"))
                {
                    manager.SwitchState(manager.pursueState);
                }
            }
        }
        else
            manager.SwitchState(manager.grabbedState);
    }
}
