using UnityEngine;

public class LadderEscape : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (GameManager.instance != null && !GameManager.instance.isPaused)
            GameManager.instance.YouWin();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance != null)
                GameManager.instance.ShowEscapePrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance != null)
                GameManager.instance.ShowEscapePrompt(false);
        }
    }
}
