using System.Diagnostics;
using UnityEngine;

public class ChangeLocationState : BossBaseState
{
    public override void EnterState(BossStateManager manager)
    {
        int randomNumber = Random.Range(0, 6);
        manager.randomState = Random.Range(1, 4);
        manager.nextPos = manager.locations[randomNumber];
        manager.isMoving = true;
    }

    public override void OnCollisionEnter(BossStateManager manager)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(BossStateManager manager)
    {
        manager.faceTarget(manager.nextPos.position);

        manager.boss.transform.position = Vector3.MoveTowards(manager.boss.transform.position, manager.nextPos.transform.position, Time.deltaTime * manager.moveSpeed);

        Vector3 delta = manager.boss.transform.position - manager.nextPos.transform.position;

        if (delta.sqrMagnitude < 0.01f)
        {
            if (!manager.stateTypeDic.TryGetValue(manager.randomState, out BossBaseState state))
                return;

            manager.SwitchState(state);
        }
        //enemy must move to a new position
        //Stop
        //change state to one of it's attacks
    }
}
