using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("Components")]
    public CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("Player Stats")]
    [Range(1, 10)] public int HP;
    [Range(5, 25)]public int speed;
    [Range(2, 5)][SerializeField] int sprintMod;
    [Range(5, 20)][SerializeField] int JumpSpeed;
    [Range(1, 3)][SerializeField] int maxJumps;
    [Range(15, 50)][SerializeField] int gravity;
    float currentWeight;
    [Range(0,1)]public float chargeTimer;
    float staminaRegenTimer;
    int jumpCount;
    int HPOrig;

    [Header("Encumbrance / Weight")]
    [SerializeField] float baseWeightLimit = 20f;
    [SerializeField] float weightPerStaminaLevel = 2f;
    [SerializeField] float encumberedSpeedMultiplier = 0.6f;
    [SerializeField] float encumberedStaminaCostMultiplier = 1.5f;


    [Header("Stamina")]
    [SerializeField] float maxStamina = 100f;
    [SerializeField] float stamina;
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
    [SerializeField] AudioClip[] footstepClips;
    [SerializeField] float walkStepInterval = 0.5f;
    [SerializeField] float sprintStepInterval = 0.28f;
    [SerializeField] AudioClip armorEquipClip;

    [Header("References")]
    [SerializeField] MeshRenderer armMeshRenderer;
    public GameObject currentWeaponInstance;
    public Armor currentArmor;

    [Header("Animations")]
    [SerializeField] Animator animator;

    [Header("Status Effects")]
    [SerializeField] bool isPoisoned;
    Coroutine poisonCoroutine;
    [SerializeField] float poisonRemainingTime;
    [SerializeField] float poisonTotalDuration;
    float poisonEndTime;
    
    public float PoisonRemainingTime => poisonRemainingTime;

    [Header("Vectors")]
    Vector3 moveDir;
    Vector3 playerVel;

    [Header("Flags")]
    public bool IsPoisoned => isPoisoned;
    bool isSprinting;
    bool isBlocking;
    bool isCharging;
    bool isEncumbered;
    public bool chargeAttack;

    float nextStepTime;

    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        stamina = maxStamina;
        updatePlayerUI();
        updateStaminaUI();

       
    }

    void Update()
    {
        //Timers
        speed = Mathf.Clamp(speed, 5, 25);
        chargeTimer = Mathf.Clamp01(chargeTimer);

        UpdateEncumbrance();

        //functionality
        if (!GameManager.instance.isPaused)
            movement();
        UImanager.instance.FillChargeMeter(chargeTimer);
        sprint();
        Footsteps();

        //sprinting
        if (isSprinting)
        {
            float drainPerSec = maxStamina * (staminaDrainRate / 100f);

            if (isEncumbered)
                drainPerSec *= encumberedStaminaCostMultiplier;

            stamina -= drainPerSec * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0f, maxStamina);

            staminaRegenTimer = 0f;
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

        float blockCost = GetBlockCost();
        if (stamina < blockCost && isBlocking)
            isBlocking = false;

        if (isPoisoned)
        {
            poisonRemainingTime = Mathf.Max(0f, poisonEndTime - Time.time);

            if (UImanager.instance != null)
            {
                UImanager.instance.UpdatePoisonUI(poisonRemainingTime, poisonTotalDuration);
            }
        }

        OnInteract();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCount = 0;
        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        float moveSpeed = speed;
        if (isEncumbered)
            moveSpeed *= encumberedSpeedMultiplier;

        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        jump();
        controller.Move(playerVel * Time.deltaTime);

        if (WeaponManager.instance.currentWeapon != null)
        {
            Attack(WeaponManager.instance.currentWeapon);
        }
    }

    public float CurrentStamina
    {
        get { return stamina; }
    }

    public void SetStamina(float value)
    {
        stamina = Mathf.Clamp(value, 0f, maxStamina);
        updateStaminaUI();
    }

    void Footsteps()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;

        if (audioSource == null || footstepClips == null || footstepClips.Length == 0)
            return;

        if (!controller.isGrounded)
            return;

        bool moving =
            Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f ||
            Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;

        if (!moving)
            return;

        float interval = isSprinting ? sprintStepInterval : walkStepInterval;

        if (Time.time >= nextStepTime)
        {
            AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
            audioSource.PlayOneShot(clip);
            nextStepTime = Time.time + interval;
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            playerVel.y = JumpSpeed;
            jumpCount++;
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint") && stamina > 0f && !isSprinting)
        {
            isSprinting = true;
            speed *= sprintMod;
        }
        else if ((Input.GetButtonUp("Sprint") || stamina <= 0f) && isSprinting)
        {
            isSprinting = false;
            speed /= sprintMod;
        }
    }

    public void takeDamage(int amount)
    {
        float blockCost = GetBlockCost();

        if (isBlocking && stamina >= blockCost)
        {
            stamina -= blockCost;
            if (stamina < 0f)
                stamina = 0f;

            staminaRegenTimer = 0f;
            updateStaminaUI();

            if (stamina < blockCost)
                isBlocking = false;

            Debug.Log("[PlayerController] Blocked attack. No HP lost.");
            return;
        }

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

        HP -= finalDamage;
        Debug.Log("[PlayerController] Took " + finalDamage + " damage. HP now: " + HP);

        updatePlayerUI();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            GameManager.instance.YouLose();
        }
    }

    public void updatePlayerUI()
    {
        UImanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    void updateStaminaUI()
    {
        UImanager.instance.playerStaminaBar.fillAmount = stamina / maxStamina;
    }

    IEnumerator flashRed()
    {
        UImanager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        UImanager.instance.playerDamageScreen.SetActive(false);
    }

    void OnInteract()
    {
        if (!GameManager.instance.isPaused)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance))
                {
                    Debug.Log(hit.collider.name);

                    IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        interactable.Interact();
                    }
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

    void Attack(Weapon weapon)
    {
        isCharging = Input.GetButton("Fire1");
        animator.SetBool("IsChargingSwing", isCharging);

        if (isCharging)
            chargeTimer += Time.deltaTime / 3;

        if (Input.GetButtonUp("Fire1") && !GameManager.instance.isInteracting)
        {
            switch (weapon.itemType)
            {
                case ItemType.Weapon:
                    if (chargeTimer > .7f)
                    {
                        PlayerAnimatorManager.instance.PlayTargetAnimation(animator, "BigSwing", .5f);
                    }
                    else if(chargeTimer < .7f)
                    {
                        PlayerAnimatorManager.instance.PlayTargetAnimation(animator, "regSwing",.5f);
                    }
                    break;
            }
            chargeTimer = 0;

        }
    }

    public void EquipRing(MagicRing ring)
    {
        WeaponManager.instance.currentRingEquipped = ring;
    }

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

            takeDamage(damagePerTick);
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

        float weightLimit = baseWeightLimit + staminaLevel * weightPerStaminaLevel;

        isEncumbered = currentWeight > weightLimit;
    }

    float GetBlockCost()
    {
        float cost = maxStamina * (blockStaminaCost / 100f);

        if (isEncumbered)
            cost *= encumberedStaminaCostMultiplier;

        return cost;
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

}



