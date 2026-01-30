using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("Components")]
    public CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    public PlayerMovement playerMovement;

    [Header("Attacking")]
    float comboWindowTimer;
    string nextSwing;

    [Header("Player Stats")]
    public int HP;
    float currentWeight;
    [Range(0, 1)] public float chargeTimer;
    float staminaRegenTimer;
    int HPOrig;

    [Header("Stamina")]
    [SerializeField] float maxStamina = 100f;
    public float stamina;
    [SerializeField] float staminaDrainRate = 10f;
    [SerializeField] float staminaRegenRate = 5f;
    [SerializeField] float staminaRegenInterval = 0.5f;

    [Header("Shield")]
    [SerializeField] Transform shieldTransform;
    [SerializeField] Transform armTransform;
    [SerializeField] float blockStaminaCost = 25f;

    [Header("Interaction")]
    [SerializeField] int interactDistance;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip armorEquipClip;
    [SerializeField] AudioClip swordSwing;

    [Header("References")]
    [SerializeField] MeshRenderer armMeshRenderer;
    public GameObject currentWeaponInstance;
    public Armor currentArmor;

    [Header("Animations")]
    [SerializeField] Animator animator;
    [SerializeField] Animator legAnimator;

    [Header("Status Effects")]
    [SerializeField] bool isPoisoned;
    Coroutine poisonCoroutine;
    [SerializeField] float poisonRemainingTime;
    [SerializeField] float poisonTotalDuration;
    float poisonEndTime;

    public float PoisonRemainingTime => poisonRemainingTime;

    [Header("Flags")]
    public bool IsPoisoned => isPoisoned;
    bool isBlocking;
    bool isCharging;
    public bool chargeAttack;
    public bool isKicking;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        HPOrig = HP;
        stamina = maxStamina;
        updatePlayerUI();
        updateStaminaUI();
    }

    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.CanPlayerAct())
            return;

        animator.SetBool("SwordEquipped", WeaponManager.instance.currentWeapon != null);

        chargeTimer = Mathf.Clamp01(chargeTimer);

        UpdateEncumbrance();

        Kick();

        if (playerMovement.isSprinting)
        {
            float drainPerSec = maxStamina * (staminaDrainRate / 100f);

            if (playerMovement.isEncumbered)
                drainPerSec *= playerMovement.encumberedStaminaCostMultiplier;

            stamina -= drainPerSec * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0f, maxStamina);

            staminaRegenTimer = 0f;

            if (SkillManager.instance != null)
            {
                SkillManager.instance.AddSprintXP(Time.deltaTime * 2f);
            }
        }
        else
        {
            if (stamina < maxStamina)
            {
                staminaRegenTimer += Time.deltaTime;

                if (staminaRegenTimer >= staminaRegenInterval)
                {
                    float regenAmount = maxStamina * (staminaRegenRate / 100f);
                    stamina += regenAmount;
                    stamina = Mathf.Clamp(stamina, 0f, maxStamina);
                    staminaRegenTimer = 0f;
                }
            }
            else
            {
                staminaRegenTimer = 0f;
            }
        }

        updateStaminaUI();

        if (isPoisoned)
        {
            poisonRemainingTime = Mathf.Max(0f, poisonEndTime - Time.time);

            if (UImanager.instance != null)
            {
                UImanager.instance.UpdatePoisonUI(poisonRemainingTime, poisonTotalDuration);
            }
        }

        OnInteract();

        if (WeaponManager.instance != null && WeaponManager.instance.currentWeapon != null)
        {
            Attack(WeaponManager.instance.currentWeapon);
        }
    }

    #region UI
    public float CurrentStamina
    {
        get { return stamina; }
    }

    public void SetStamina(float value)
    {
        stamina = Mathf.Clamp(value, 0f, maxStamina);
        updateStaminaUI();
    }

    public void takeDamage(int amount)
    {
        ApplyDamage(amount, true);
    }

    public void TakeDamage_NoShake(int amount)
    {
        ApplyDamage(amount, false);
    }

    void ApplyDamage(int amount, bool doCameraShake)
    {
        int finalDamage = amount;

        if (currentArmor != null)
        {
            float pct = Mathf.Clamp(currentArmor.damageReductionPercent, 0f, 100f);
            float factor = 1f - (pct / 100f);

            finalDamage = Mathf.CeilToInt(amount * factor);
            if (finalDamage < 0)
                finalDamage = 0;

            Debug.Log("[PlayerController] Armor " + currentArmor.itemName +
                      " (" + pct + "% DR) reduced " + amount + " -> " + finalDamage);
        }
        else
        {
            Debug.Log("[PlayerController] No armor. Full damage: " + amount);
        }

        if (finalDamage <= 0)
            doCameraShake = false;

        HP -= finalDamage;
        Debug.Log("[PlayerController] Took " + finalDamage + " damage. HP now: " + HP);

        if (SkillManager.instance != null && finalDamage > 0)
        {
            SkillManager.instance.AddToughnessXP(finalDamage);
        }

        updatePlayerUI();
        StartCoroutine(flashRed());

        if (doCameraShake && GameManager.instance != null && GameManager.instance.cameraControllerScript != null)
        {
            GameManager.instance.cameraControllerScript.Shake(0.15f, 0.15f);
        }

        if (HP <= 0)
        {
            if (GameManager.instance != null)
                GameManager.instance.YouLose();
        }
    }

    public void updatePlayerUI()
    {
        if (UImanager.instance != null && UImanager.instance.playerHPBar != null)
            UImanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    void updateStaminaUI()
    {
        if (UImanager.instance != null && UImanager.instance.playerStaminaBar != null)
            UImanager.instance.playerStaminaBar.fillAmount = stamina / maxStamina;
    }

    IEnumerator flashRed()
    {
        if (UImanager.instance != null && UImanager.instance.playerDamageScreen != null)
        {
            UImanager.instance.playerDamageScreen.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            UImanager.instance.playerDamageScreen.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    public int MaxHP
    {
        get { return HPOrig; }
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
            return;

        HP += amount;
        if (HP > HPOrig)
            HP = HPOrig;

        updatePlayerUI();
    }
    #endregion

    #region WeaponAndInteraction
    void OnInteract()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (currentWeaponInstance != null &&
            WeaponManager.instance.currentWeapon == weapon)
        {
            Destroy(currentWeaponInstance);
            currentWeaponInstance = null;
            WeaponManager.instance.currentWeapon = null;
            return;
        }

        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
            currentWeaponInstance = null;
        }

        currentWeaponInstance = Instantiate(
            weapon.modelPrefab,
            armTransform,
            false
        );

        WeaponManager.instance.currentWeapon = weapon;
    }

    public void EquipArmor(Armor armor)
    {
        currentArmor = armor;

        if (ArmorManager.instance != null)
        {
            ArmorManager.instance.currentArmor = armor;
        }

        if (armor != null && armorEquipClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(armorEquipClip);
        }

        if (UImanager.instance != null)
        {
            if (armor != null)
                UImanager.instance.ShowArmorIcon();
            else
                UImanager.instance.HideArmorIcon();
        }
    }

    void HandleCombo()
    {
        switch (nextSwing)
        {
            case null:
                nextSwing = "Swing1";
                break;
            case "Swing1":
                nextSwing = "Swing2";
                break;
            case "Swing2":
                nextSwing = "Swing1";
                break;
        }
    }

    void Attack(Weapon weapon)
    {
        if (GameManager.instance != null && !GameManager.instance.CanPlayerAct())
            return;

        isCharging = Input.GetButton("Fire1");
        animator.SetBool("IsChargingSwing", isCharging);

        if (isCharging)
            chargeTimer += Time.deltaTime / 3f;

        if (UImanager.instance != null)
            UImanager.instance.FillChargeMeter(chargeTimer);

        if (Input.GetButtonDown("Fire1"))
        {
            if (GameManager.instance != null && GameManager.instance.isInteracting)
                return;

            HandleCombo();
            if (PlayerAnimatorManager.instance != null)
                PlayerAnimatorManager.instance.PlayTargetAnimation(animator, nextSwing, 0f);

            if (audioSource != null && swordSwing != null)
            {
                audioSource.pitch = Random.Range(.7f, 1.2f);
                audioSource.PlayOneShot(swordSwing);
            }
        }
    }

    void Kick()
    {
        if (GameManager.instance != null && !GameManager.instance.CanPlayerAct())
            return;

        if (Input.GetKeyDown(KeyCode.F) && !isKicking)
        {
            if (legAnimator != null && PlayerAnimatorManager.instance != null)
            {
                PlayerAnimatorManager.instance.PlayTargetAnimation(legAnimator, "Kick", .5f);
            }
        }
    }

    public void EquipRing(MagicRing ring)
    {
        if (WeaponManager.instance != null)
            WeaponManager.instance.currentRingEquipped = ring;
    }
    #endregion

    #region StatusEffects
    public void ApplyPoison(float duration, float interval, int damagePerTick)
    {
        if (poisonCoroutine != null)
        {
            StopCoroutine(poisonCoroutine);
        }

        isPoisoned = true;

        poisonTotalDuration = duration;
        poisonEndTime = Time.time + duration;
        poisonRemainingTime = duration;

        if (UImanager.instance != null)
        {
            UImanager.instance.ShowPoisonUI(poisonTotalDuration);
            UImanager.instance.UpdatePoisonUI(poisonRemainingTime, poisonTotalDuration);
        }

        poisonCoroutine = StartCoroutine(PoisonRoutine(interval, damagePerTick));
    }

    IEnumerator PoisonRoutine(float interval, int damagePerTick)
    {
        while (Time.time < poisonEndTime)
        {
            if (HP <= 0 || GameManager.instance == null)
                break;

            TakeDamage_NoShake(damagePerTick);

            yield return new WaitForSeconds(interval);
        }

        isPoisoned = false;
        poisonCoroutine = null;
        poisonRemainingTime = 0f;
        poisonTotalDuration = 0f;

        if (UImanager.instance != null)
        {
            UImanager.instance.HidePoisonUI();
        }
    }

    public void CurePoison()
    {
        if (poisonCoroutine != null)
        {
            StopCoroutine(poisonCoroutine);
            poisonCoroutine = null;
        }

        isPoisoned = false;
        poisonRemainingTime = 0f;
        poisonTotalDuration = 0f;
        poisonEndTime = 0f;

        if (UImanager.instance != null)
        {
            UImanager.instance.HidePoisonUI();
        }

        Debug.Log("Poison cured.");
    }

    public void RestorePoisonFromSave(float remainingTime, float interval, int damagePerTick)
    {
        CurePoison();

        if (remainingTime <= 0f)
            return;

        isPoisoned = true;

        poisonTotalDuration = remainingTime;
        poisonEndTime = Time.time + remainingTime;
        poisonRemainingTime = remainingTime;

        if (UImanager.instance != null)
        {
            UImanager.instance.ShowPoisonUI(poisonTotalDuration);
            UImanager.instance.UpdatePoisonUI(poisonRemainingTime, poisonTotalDuration);
        }

        poisonCoroutine = StartCoroutine(PoisonRoutine(interval, damagePerTick));
    }

    void UpdateEncumbrance()
    {
        currentWeight = 0f;

        if (WeaponManager.instance != null && WeaponManager.instance.currentWeapon != null)
        {
            currentWeight += WeaponManager.instance.currentWeapon.weight;
        }

        if (ArmorManager.instance != null && ArmorManager.instance.currentArmor != null)
        {
            currentWeight += ArmorManager.instance.currentArmor.weight;
        }

        int staminaLevel = 0;
        if (SkillManager.instance != null)
        {
            staminaLevel = SkillManager.instance.sprintLevel;
        }

        float weightLimit = playerMovement.baseWeightLimit + staminaLevel * playerMovement.weightPerStaminaLevel;

        playerMovement.isEncumbered = currentWeight > weightLimit;
    }
    #endregion
}
