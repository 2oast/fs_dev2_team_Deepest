using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public void PlayTargetAnimation(Animator animator, string animationName)
    {
        animator.Play(animationName);
        animator.CrossFade(animationName, .05f);
    }
}
