using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UImanager : MonoBehaviour
{
    public static UImanager instance;

    [Header("Player UI")]
    public Image playerHPBar;
    public Image playerStaminaBar;
    public Image chargeMeter;
    public GameObject playerDamageScreen;

    [Header("Status")]
    public GameObject poisonPanel;
    public Image poisonFillImage;
    public TextMeshProUGUI poisonTimerText;

    [Header("Armor UI")]
    public Image armorIcon;

    [Header("Level Up UI (TMP)")]
    public TMP_Text levelUpText;
    public float fadeInTime = 0.25f;
    public float holdTime = 3.5f;
    public float fadeOutTime = 0.25f;

    [Header("Level Up Audio")]
    public AudioSource levelUpAudioSource;
    public AudioClip levelUpClip;

    public GameObject floatingText;

    struct LevelUpRequest
    {
        public string skillName;
        public int level;

        public LevelUpRequest(string skillName, int level)
        {
            this.skillName = skillName;
            this.level = level;
        }
    }

    Queue<LevelUpRequest> levelUpQueue = new Queue<LevelUpRequest>();
    bool isShowingLevelUp = false;
    Coroutine levelUpRoutine;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;


        if (armorIcon != null)
        {
            armorIcon.enabled = false;
            armorIcon.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (levelUpText != null)
        {
            levelUpText.gameObject.SetActive(true);
            Color c = levelUpText.color;
            c.a = 0f;
            levelUpText.color = c;
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

    public void ShowLevelUpMessage(string skillName, int newLevel)
    {
        if (levelUpText == null)
            return;

        levelUpQueue.Enqueue(new LevelUpRequest(skillName, newLevel));

        if (!isShowingLevelUp)
        {
            levelUpRoutine = StartCoroutine(ProcessLevelUpQueue());
        }
    }

    IEnumerator ProcessLevelUpQueue()
    {
        isShowingLevelUp = true;

        while (levelUpQueue.Count > 0)
        {
            LevelUpRequest req = levelUpQueue.Dequeue();

            levelUpText.text = req.skillName + " leveled up!  Lv " + req.level;

            Color c = levelUpText.color;
            c.a = 0f;
            levelUpText.color = c;
            levelUpText.gameObject.SetActive(true);

            if (levelUpAudioSource != null && levelUpClip != null)
            {
                levelUpAudioSource.PlayOneShot(levelUpClip);
            }

            float t = 0f;
            while (t < fadeInTime)
            {
                t += Time.deltaTime;
                float a = Mathf.Lerp(0f, 1f, fadeInTime > 0f ? t / fadeInTime : 1f);
                c.a = a;
                levelUpText.color = c;
                yield return null;
            }

            if (holdTime > 0f)
                yield return new WaitForSeconds(holdTime);

            t = 0f;
            while (t < fadeOutTime)
            {
                t += Time.deltaTime;
                float a = Mathf.Lerp(1f, 0f, fadeOutTime > 0f ? t / fadeOutTime : 1f);
                c.a = a;
                levelUpText.color = c;
                yield return null;
            }

            c.a = 0f;
            levelUpText.color = c;
        }

        levelUpText.gameObject.SetActive(true);
        isShowingLevelUp = false;
        levelUpRoutine = null;
    }

    public void ShowPoisonUI(float duration)
    {
        if (poisonPanel != null)
            poisonPanel.SetActive(true);

        UpdatePoisonUI(duration, duration);
    }

    public void UpdatePoisonUI(float remaining, float total)
    {
        if (poisonPanel == null)
            return;

        if (poisonTimerText != null)
        {
            int secs = Mathf.CeilToInt(remaining);
            poisonTimerText.text = secs + "s";
        }

        if (poisonFillImage != null && total > 0f)
        {
            poisonFillImage.fillAmount = Mathf.Clamp01(remaining / total);
        }
    }

    public void HidePoisonUI()
    {
        if (poisonPanel != null)
            poisonPanel.SetActive(false);
    }

    public void FillChargeMeter(float chargeTimer)
    {
        chargeMeter.fillAmount = chargeTimer;
    }
}
