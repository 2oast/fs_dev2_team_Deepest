using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public void PlayTargetAnimation(Animator animator, string animationName, float crossFade)
    {
        animator.Play(animationName);
        animator.CrossFade(animationName, crossFade);
    }
}
