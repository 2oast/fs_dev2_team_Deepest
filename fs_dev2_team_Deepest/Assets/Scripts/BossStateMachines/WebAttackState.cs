using Microsoft.Win32.SafeHandles;
using UnityEngine;

public class WebAttackState : BossBaseState
{
    public override void EnterState(BossStateManager manager)
    {
        manager.isMoving = false;
        manager.waitTimer = 0;
    }

    public override void OnCollisionEnter(BossStateManager manager)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(BossStateManager manager)
    {
        manager.waitTimer += Time.deltaTime;

        manager.faceTarget(GameManager.instance.player.transform.position);

        if (manager.waitTimer >= 3 && manager.shootTimer > manager.shootRate)
        {
            manager.shoot();
        }

        if(manager.waitTimer >= 10)
        {
            manager.SwitchState(manager.changeLocationState);
        }
    }
}
