using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Menu's")]
    public GameObject menuActive;
    [SerializeField] GameObject menuPause;
    public GameObject menuWin;
    [SerializeField] GameObject menuLose;
    public GameObject inventoryScreen;
    public GameObject controlsScreen;
    public GameObject radioUI;

    [Header("Text Objects")]
    public GameObject interactTextBox;
    public GameObject YesOrNoObj;
    public GameObject loadingScreen;
    public GameObject flashScreen;
    public TextMeshProUGUI interactText;
    public TextMeshProUGUI pickupText;

    [Header("Player")]
    public PlayerController playerControllerScript;
    public PlayerCam cameraControllerScript;
    public GameObject player;
    public GameObject cam;
    public GameObject radioObj;

    float timeScaleOrig = 1f;

    public bool isInteracting;
    public bool isPaused;

    [Header("Camera Stuff")]
    GameObject currentCam;

    public bool CanPlayerAct()
    {
        return !isPaused && !isInteracting && menuActive == null;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        timeScaleOrig = Time.timeScale;

        if (inventoryScreen != null)
            inventoryScreen.SetActive(false);

        if (player == null)
            player = GameObject.FindWithTag("Player");

        if (player != null && playerControllerScript == null)
            playerControllerScript = player.GetComponent<PlayerController>();

        if (cam == null && Camera.main != null)
            cam = Camera.main.gameObject;

        if (cam != null && cameraControllerScript == null)
            cameraControllerScript = cam.GetComponent<PlayerCam>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == menuLose || menuActive == menuWin)
                return;

            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else
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
                if (menuActive != null) menuActive.SetActive(true);
            }
            else if (menuActive == inventoryScreen)
            {
                StateUnpause();
            }
        }
    }

    public void SwitchCamera(GameObject newCamera)
    {
        if (newCamera == null || currentCam == newCamera)
            return;

        if (currentCam != null)
            currentCam.SetActive(false);

        currentCam = newCamera;
        currentCam.SetActive(true);
    }

    public IEnumerator TransitionScreen(float duration, Transform target)
    {
        Image img = loadingScreen.GetComponent<Image>();
        Color c = img.color;
        CharacterController cc = playerControllerScript.GetComponent<CharacterController>();

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            c.a = Mathf.Lerp(0f, 1f, t);
            img.color = c;
            yield return null;
        }

        cc.enabled = false;
        playerControllerScript.transform.position = target.position;
        cc.enabled = true;

        yield return new WaitForSecondsRealtime(0.5f);

        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            c.a = Mathf.Lerp(1f, 0f, t);
            img.color = c;
            yield return null;
        }
    }

    public void StatePause()
    {
        isPaused = true;
        timeScaleOrig = Time.timeScale;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
    }

    public void YouLose()
    {
        StatePause();
        menuActive = menuLose;
        if (menuActive != null) menuActive.SetActive(true);
    }

    public IEnumerator ScreenFlash()
    {
        flashScreen.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        flashScreen.SetActive(false);
    }

    public void ResetAfterLoad()
    {
        StateUnpause();
        isInteracting = false;

        if (inventoryScreen != null)
            inventoryScreen.SetActive(false);

        if (radioUI != null)
            radioUI.SetActive(false);
    }

    public IEnumerator HitStop(float duration)
    {
        float prev = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = prev;
    }
}

