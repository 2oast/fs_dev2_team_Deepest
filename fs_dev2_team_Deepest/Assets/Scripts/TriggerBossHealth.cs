using UnityEngine;
using UnityEngine.UI;


public class TriggerBossHealth : MonoBehaviour
{
    [SerializeField] GameObject bossHealthBox;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            bossHealthBox.SetActive(true);
        }
    }
}
