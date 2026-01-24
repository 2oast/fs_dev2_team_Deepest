using UnityEngine;

public class LegAnimationEvents : MonoBehaviour
{
    [SerializeField] BoxCollider kickCollider;
   
    public void DisableKick()
    {
        GameManager.instance.playerControllerScript.isKicking = false;
    }

    public void EnableKickCollider()
    {
        kickCollider.enabled = true;
    }

    public void DisableKickCollider()
    {
        kickCollider.enabled = false;
    }
}
