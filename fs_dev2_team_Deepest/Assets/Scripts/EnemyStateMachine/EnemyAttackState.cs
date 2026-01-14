using UnityEngine;
using System.Collections;

public class EnemyGrabbedState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager manager)
    {
        manager.agent.enabled = false;
    }

    public override void OnCollisionEnter(EnemyStateManager manager)
    {
        
    }

    public override void UpdateState(EnemyStateManager manager)
    {

    }


}
