using UnityEngine;

public class LegAnimationEvents : MonoBehaviour
{
    [SerializeField] BoxCollider kickCollider;
    [SerializeField] KickBack kickBackScript;

    //private void Start()
    //{
    //    if (kickBackScript == null)
    //    {
    //        kickBackScript = FindAnyObjectByType<KickBack>();
    //    }
    //}

    public void DisableKick()
    {
        GameManager.instance.playerControllerScript.isKicking = false;
    }

    public void EnableKick()
    {
        GameManager.instance.playerControllerScript.isKicking = true;
    }

    //public void EnableKickCollider()
    //{
    //    kickBackScript.kickRaycast();
    //}
}
