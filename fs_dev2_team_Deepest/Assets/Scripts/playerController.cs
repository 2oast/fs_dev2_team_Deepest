using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("Components")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("Player Stats")]
    [Range(1, 10)] public int HP;
    [Range(1, 5)][SerializeField] float speed;
    [Range(2, 5)][SerializeField] float sprintMod;
    [Range(5, 20)][SerializeField] int JumpSpeed;
    [Range(1, 3)][SerializeField] int maxJumps;
    [Range(15, 50)][SerializeField] int gravity;

    [Header("Stamina")]
    [SerializeField] float maxStamina = 100f;
    [SerializeField] float stamina;
    [SerializeField] float staminaDrainRate = 10f;
    [SerializeField] float staminaRegenRate = 5f;
    [SerializeField] float staminaRegenInterval = 0.5f;

    [Header("Shield")]
    [SerializeField] Transform shieldTransform;
    [SerializeField] Transform armTransform;
    [SerializeField] Vector3 shieldBlockOffset = new Vector3(0.3f, 0.2f, 0f);
    [SerializeField] float shieldMoveSpeed = 10f;
    [SerializeField] float blockStaminaCost = 25f;

    [Header("Shooting")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

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

    [Header("Animations")]
    [SerializeField] Animator currentWeaponAnimator;
    [SerializeField] Animator animator;



    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;

    bool isSprinting;
    float staminaRegenTimer;
    float baseMaxStamina;
    float currentSprintMultiplier = 1f;

    bool isBlocking;
    Vector3 shieldDefaultLocalPos;
    bool weaponEquipped;

    float shootTimer;

    float baseSpeed;

    float nextStepTime;

    public GameObject currentWeaponInstance;

    public Armor currentArmor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        stamina = maxStamina;
        baseMaxStamina = maxStamina;
        baseSpeed = speed;
        updatePlayerUI();
        updateStaminaUI();

        if (shieldTransform != null)
            shieldDefaultLocalPos = shieldTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        shootTimer += Time.deltaTime;

        if (!GameManager.instance.isPaused)
            movement();

        sprint();
        Blocking();
        UpdateShieldPosition();
        Footsteps();

        if (SkillManager.instance != null && isSprinting && moveDir.sqrMagnitude > 0.1f && !GameManager.instance.isPaused)
        {
            SkillManager.instance.AddSprintXP(Time.deltaTime);
        }

        if (isSprinting)
        {
            float drainPerSec = maxStamina * (staminaDrainRate / 100f);
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

        if (SkillManager.instance != null)
        {
            float staminaMult = SkillManager.instance.GetStaminaMaxMultiplier();
            maxStamina = baseMaxStamina * staminaMult;
            stamina = Mathf.Clamp(stamina, 0f, maxStamina);
        }

        updateStaminaUI();

        float blockCost = maxStamina * (blockStaminaCost / 100f);
        if (stamina < blockCost && isBlocking)
            isBlocking = false;

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
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();
        controller.Move(playerVel * Time.deltaTime);

        if(WeaponManager.instance.currentWeapon != null)
        {
            Attack(WeaponManager.instance.currentWeapon);
        }
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

            currentSprintMultiplier = sprintMod;

            if (SkillManager.instance != null)
            {
                currentSprintMultiplier *= SkillManager.instance.GetSprintSpeedMultiplier();
            }

            speed *= currentSprintMultiplier;
        }
        else if ((Input.GetButtonUp("Sprint") || stamina <= 0f) && isSprinting)
        {
            isSprinting = false;
            speed /= currentSprintMultiplier;
            currentSprintMultiplier = 1f;
        }
    }




    public void takeDamage(int amount)
    {
        float blockCost = maxStamina * (blockStaminaCost / 100f);

        // --- Blocking check first ---
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
            float armorPct = Mathf.Clamp(currentArmor.damageReductionPercent, 0f, 100f);

            float armorEffMult = 1f;
            if (SkillManager.instance != null)
            {
                armorEffMult = SkillManager.instance.GetArmorEffectivenessMultiplier();
            }

            armorPct *= armorEffMult;
            armorPct = Mathf.Clamp(armorPct, 0f, 90f);

            float factor = 1f - (armorPct / 100f);
            finalDamage = Mathf.CeilToInt(amount * factor);
        }

        if (SkillManager.instance != null)
        {
            float toughMult = SkillManager.instance.GetToughnessDamageTakenMultiplier();
            finalDamage = Mathf.CeilToInt(finalDamage * toughMult);

            if (finalDamage < 0)
                finalDamage = 0;

            if (finalDamage > 0)
                SkillManager.instance.AddToughnessXP(finalDamage);
        }

        HP -= finalDamage;
        Debug.Log("[PlayerController] Took " + finalDamage + " damage (base " + amount + "). HP now: " + HP);

        updatePlayerUI();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            GameManager.instance.YouLose();
        }
    }


    public void updatePlayerUI()
    {
        float hpMax = HPOrig;

        if (SkillManager.instance != null)
        {
            hpMax *= SkillManager.instance.GetToughnessHealthMultiplier();
        }

        UImanager.instance.playerHPBar.fillAmount = (float)HP / hpMax;
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

    void Blocking()
    {
        if (GameManager.instance.isPaused)
        {
            isBlocking = false;
            return;
        }

        bool blockInput = Input.GetButton("Fire2");

        float blockCost = maxStamina * (blockStaminaCost / 100f);

        bool hasEnoughStamina = stamina >= blockCost;

        if (blockInput && hasEnoughStamina)
            isBlocking = true;
        else
            isBlocking = false; ;
    }

    void UpdateShieldPosition()
    {
        if (shieldTransform == null)
            return;

        Vector3 target = shieldDefaultLocalPos;

        if (isBlocking)
            target = shieldDefaultLocalPos + shieldBlockOffset;

        shieldTransform.localPosition =
            Vector3.Lerp(shieldTransform.localPosition, target, Time.deltaTime * shieldMoveSpeed);
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

    void shoot()
    {
        if (!GameManager.instance.isPaused)
        {
            if (shootTimer > 3)
            {

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

            //armMeshRenderer.enabled = false;
            currentWeaponAnimator = null;
            return;
        }

        // Different weapon or nothing equipped
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
            currentWeaponInstance = null;
        }

        //armMeshRenderer.enabled = true;

        // Always instantiate a new scene object
        currentWeaponInstance = Instantiate(
            weapon.modelPrefab,
            armTransform,
            false
        );
        WeaponManager.instance.currentWeapon = weapon;
        currentWeaponAnimator = currentWeaponInstance.GetComponent<Animator>();
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
        if (weapon == null)
            return;

        if (Input.GetButtonDown("Fire1"))
        {
            if (SkillManager.instance != null)
            {
                if (weapon.itemName == "Sword")
                {
                    SkillManager.instance.AddMeleeXP(1f);
                }
                else if (weapon.itemName == "Gun")
                {
                    SkillManager.instance.AddRangedXP(1f);
                }
            }

            switch (weapon.itemName)
            {
                case "Sword":
                    float swordSwingSpeed = 1f;
                    if (SkillManager.instance != null)
                    {
                        swordSwingSpeed = SkillManager.instance.GetMeleeAttackSpeedMultiplier();
                    }

                    if (currentWeaponAnimator != null)
                        currentWeaponAnimator.speed = swordSwingSpeed;

                    if (PlayerAnimatorManager.instance != null && currentWeaponAnimator != null)
                        PlayerAnimatorManager.instance.PlayTargetAnimation(currentWeaponAnimator, "SwordSwing");

                    stamina -= weapon.staminaDrain;
                    updateStaminaUI();
                    break;
                case "Gun":
                    break;
            }
        }
       
    }
}