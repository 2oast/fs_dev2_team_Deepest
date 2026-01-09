using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Menu's")]
    public GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    public GameObject inventoryScreen;
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
    public CameraController cameraControllerScript;
    public GameObject player;
    public GameObject radioObj;

    float timeScaleOrig;

    public bool isInteracting;
    public bool isPaused;


    [Header("Camera Stuff")]
    GameObject currentCam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        if (inventoryScreen != null)
            inventoryScreen.SetActive(false);

        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerControllerScript = player.GetComponent<PlayerController>();
            cameraControllerScript = player.GetComponentInChildren<CameraController>();
        }


    }

    private void Update()
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

    public void SwitchCamera(GameObject newCamera)
    {
        if (currentCam == newCamera)
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
            t += Time.deltaTime / duration;
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
            t += Time.deltaTime / duration;
            c.a = Mathf.Lerp(1f, 0f, t);
            img.color = c;
            yield return null;
        }

    }

    public void StatePause()
    {
        isPaused = true;
        Time.timeScale = 0;
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
        menuActive.SetActive(true);
    }


    public IEnumerator ScreenFlash()
    {
        flashScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(.1f);
        flashScreen.gameObject.SetActive(false);
    }
}
