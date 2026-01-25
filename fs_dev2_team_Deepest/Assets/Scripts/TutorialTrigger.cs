using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [TextArea(2, 6)]
    [SerializeField] string message;

    [Header("Options")]
    [SerializeField] bool playOnce = true;
    [SerializeField] bool hideOnExit = false;

    bool hasPlayed = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (playOnce && hasPlayed)
            return;

        if (TutorialHUD.instance != null)
        {
            TutorialHUD.instance.Show(message);
            hasPlayed = true;
        }
        else
        {
            Debug.LogWarning("TutorialTrigger: No TutorialHUD.instance found in scene.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!hideOnExit) return;

        if (!other.CompareTag("Player"))
            return;

        if (TutorialHUD.instance != null)
            TutorialHUD.instance.Hide();
    }
}