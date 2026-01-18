using UnityEngine;

public class ChargeAttackState : BossBaseState
{
    public override void EnterState(BossStateManager manager)
    {
        manager.isMoving = false;
        manager.hitCollider.enabled = true;
        manager.waitTimer = 0f;
    }

    public override void OnCollisionEnter(BossStateManager manager)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(BossStateManager manager)
    {
        manager.waitTimer += Time.deltaTime;
        manager.faceTarget(manager.centerPos.position);

        if (manager.waitTimer >= 3)
        {
            manager.isMoving = true;
            manager.transform.position = Vector3.MoveTowards(manager.transform.position, manager.centerPos.position, manager.moveSpeed * Time.deltaTime);
        }

        Vector3 delta = manager.boss.transform.position - manager.centerPos.position;

        if (delta.sqrMagnitude < 0.01f)
        {
            manager.SwitchState(manager.changeLocationState);
        }
    }
}
