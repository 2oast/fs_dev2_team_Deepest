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

    public AudioSource bgmSource;

    public bool isPaused;

    public bool isInteracting = false;

    float timeScaleOrig;

    int gameGoalCount;

    public Transform playerGrabPosition;

    void Awake()
    {
        instance = this;
        inventoryScreen.SetActive(false);
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
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
        bgmSource.Pause();
    }

    public void StateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
        bgmSource.UnPause();
    }

    public void UpdateGameGoal(int amount)
    {
        gameGoalCount += amount;

        if (gameGoalCount <= 0)
        {
            StatePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void YouLose()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}