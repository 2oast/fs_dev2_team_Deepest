using UnityEngine;

public class PlayerAnimatorManager : AnimatorManager
{
    public Animator playerAnimator;
    public static PlayerAnimatorManager instance;

    private void Start()
    {
        instance = this;
    }

    public void UpdateAnimator(Animator animator)
    {
        playerAnimator = animator;
    }

}
