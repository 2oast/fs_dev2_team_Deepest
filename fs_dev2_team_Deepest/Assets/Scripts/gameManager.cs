using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    public GameObject player;
    public playerController playerScript;
    public cameraController cameraController;
    public Image playerHPBar;
    public Image playerStaminaBar;
    public GameObject playerDamageScreen;
    public GameObject inventoryScreen;
    public TMP_Text escapePromptText;
    public TMP_Text doorPromptText;
    public TMP_Text icePromptText;

    public AudioSource bgmSource;

    public bool isPaused;

    public bool isInteracting = false;
    public bool keyEquipped;

    float timeScaleOrig;

    int gameGoalCount;

    public Transform playerGrabPosition;

    void Awake()
    {
        instance = this;

        if (inventoryScreen != null)
            inventoryScreen.SetActive(false);

        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        if (player != null)
            playerScript = player.GetComponent<playerController>();

        if (bgmSource != null && !bgmSource.isPlaying)
            bgmSource.Play();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                StateUnpause();
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = inventoryScreen;
                menuActive.SetActive(true);
            }
            else if (menuActive == inventoryScreen)
            {
                StateUnpause();
            }
        }
    }

    public void StatePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (bgmSource != null)
            bgmSource.Pause();
    }

    public void StateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (menuActive != null)
            menuActive.SetActive(false);

        menuActive = null;

        if (bgmSource != null)
            bgmSource.UnPause();
    }

    public void YouWin()
    {
        StatePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
        ShowEscapePrompt(false);
    }

    public void UpdateGameGoal(int amount)
    {
        gameGoalCount += amount;
    }

    public void YouLose()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void ShowEscapePrompt(bool show)
    {
        if (escapePromptText != null)
            escapePromptText.gameObject.SetActive(show);
    }

    public void ShowDoorPrompt(bool show)
    {
        if (doorPromptText != null)
            doorPromptText.gameObject.SetActive(show);
    }

    public void ShowIcePrompt(bool show)
    {
        if (icePromptText != null)
            icePromptText.gameObject.SetActive(show);
    }
}
