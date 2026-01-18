using UnityEditor.Analytics;
using UnityEngine;

public class StunnedState : BossBaseState
{
    public override void EnterState(BossStateManager manager)
    {
        manager.waitTimer = 0;
        manager.isStunned = true;
        manager.mats.Add(manager.stunMat);
        manager.model.materials = manager.mats.ToArray();
        manager.isStunned = true;
        manager.damageCollider.enabled = true;
        manager.rockCollider.enabled = false;
    }

    public override void OnCollisionEnter(BossStateManager manager)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(BossStateManager manager)
    {
        manager.waitTimer += Time.deltaTime;

        if(manager.waitTimer >= 10)
        {
            manager.damageCollider.enabled = false;
            manager.isStunned = false;
            manager.mats.Remove(manager.stunMat);
            manager.model.materials = manager.mats.ToArray();
            manager.SwitchState(manager.changeLocationState);
        }
    }
}
