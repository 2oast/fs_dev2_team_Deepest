using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyPursueState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager manager)
    {
        
    }

    public override void OnCollisionEnter(EnemyStateManager manager)
    {
       
    }

    public override void UpdateState(EnemyStateManager manager)
    {
        if(manager.enabled)
        {
            manager.agent.SetDestination(GameManager.instance.player.transform.position);

            if (manager.agent.remainingDistance <= manager.agent.stoppingDistance)
            {
                manager.faceTarget();
            }

            if (manager.shootTimer >= manager.shootRate)
            {
                manager.shoot();
            }
            else
                return;
        }
        else
        {
            manager.SwitchState(manager.grabbedState);
        }
       
    }

    
}
