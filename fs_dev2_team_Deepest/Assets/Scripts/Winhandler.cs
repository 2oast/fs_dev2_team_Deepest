using UnityEngine;

public class WinHandler : MonoBehaviour, IInteractable
{
    [Header("Win Settings")]
    [SerializeField] GameObject winMenu;
    [SerializeField] string playerTag = "Player";

    bool hasTriggered = false;

    public void Interact()
    {
        if (hasTriggered)
            return;

        hasTriggered = true;

        if (winMenu != null)
        {
            Time.timeScale = 0f;
            winMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Escape: winMenu is not assigned in the Inspector.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (other.CompareTag(playerTag))
        {
            Interact();
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }
}
