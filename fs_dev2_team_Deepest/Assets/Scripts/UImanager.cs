using UnityEngine;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    public static UImanager instance;

    [Header("Player UI")]
    public Image playerHPBar;
    public Image playerStaminaBar;
    public GameObject playerDamageScreen;

    [Header("Armor UI")]
    public Image armorIcon;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (armorIcon != null)
        {
            armorIcon.enabled = false;
            armorIcon.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowArmorIcon()
    {
        if (armorIcon != null)
        {
            armorIcon.gameObject.SetActive(true);
            armorIcon.enabled = true;
        }
    }

    public void HideArmorIcon()
    {
        if (armorIcon != null)
        {
            armorIcon.enabled = false;
            armorIcon.gameObject.SetActive(false);
        }
    }
}
