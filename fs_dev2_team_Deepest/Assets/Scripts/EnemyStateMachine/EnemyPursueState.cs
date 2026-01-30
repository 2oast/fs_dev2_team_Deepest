using UnityEngine;

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
        if (!manager.enabled)
        {
            manager.SwitchState(manager.grabbedState);
            return;
        }

        if (manager.agent == null || !manager.agent.enabled)
            return;

        Transform target = manager.aimTarget;

        if (target == null && GameManager.instance != null && GameManager.instance.player != null)
            target = GameManager.instance.player.transform;

        if (target == null)
            return;

        manager.agent.SetDestination(target.position);

        if (manager.IsTargetInRange() || manager.CanSeeTarget())
        {
            manager.faceTarget();

            if (manager.shootTimer >= manager.shootRate)
            {
                manager.shoot();
            }
        }
    }
}

