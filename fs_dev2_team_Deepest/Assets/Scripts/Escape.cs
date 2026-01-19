using UnityEngine;

public class Escape : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.instance.menuActive = GameManager.instance.menuWin;
            GameManager.instance.StatePause();
            GameManager.instance.menuWin.SetActive(true);
        }
    }
}
