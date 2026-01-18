using UnityEngine;

public class GetStunned : MonoBehaviour
{
    BossStateManager bossStateManager;

    private void Start()
    {
        bossStateManager = GetComponentInParent<BossStateManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Rock rock = other.GetComponent<Rock>();
        if(rock != null)
        {
            bossStateManager.SwitchState(bossStateManager.stunnedState);
        }
    }
}
