using UnityEditor.Analytics;
using UnityEngine;
using System.Collections.Generic;


public class SpecialAttackState : BossBaseState
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
        manager.faceTarget(GameManager.instance.player.transform.position);
        manager.waitTimer += Time.deltaTime;

        if(manager.waitTimer >= 2 && manager.shootTimer > manager.shootRate)
        {
            manager.animator.SetTrigger("SpecialAttack");

            manager.SpawnRocks();
        }

        if(manager.waitTimer >= 10)
        {
            manager.SwitchState(manager.changeLocationState);
        }
    }

}
