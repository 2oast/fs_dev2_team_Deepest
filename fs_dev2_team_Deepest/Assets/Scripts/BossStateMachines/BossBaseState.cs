using UnityEngine;

public abstract class BossBaseState
{
    public abstract void EnterState(BossStateManager manager);

    public abstract void UpdateState(BossStateManager manager);

    public abstract void OnCollisionEnter(BossStateManager manager);

}
