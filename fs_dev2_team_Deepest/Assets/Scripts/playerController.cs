using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Range(1, 10)]public int HP;
    [Range(1, 5)][SerializeField] int speed;
    [Range(2, 5)][SerializeField] int sprintMod;
    [Range(5, 20)][SerializeField] int JumpSpeed;
    [Range(1, 3)][SerializeField] int maxJumps;
    [Range(15, 50)][SerializeField] int gravity;


    [SerializeField] float maxStamina = 100f;
    [SerializeField] float stamina;
    [SerializeField] float staminaDrainRate = 10f;
    [SerializeField] float staminaRegenRate = 5f;
    [SerializeField] float staminaRegenInterval = 0.5f;

    [SerializeField] Transform shieldTransform;
    [SerializeField] Vector3 shieldBlockOffset = new Vector3(0.3f, 0.2f, 0f);
    [SerializeField] float shieldMoveSpeed = 10f;
    [SerializeField] float blockStaminaCost = 25f;

    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    [SerializeField] int interactDistance;

    [SerializeField] AudioSource footstepSource;
    [SerializeField] AudioClip[] footstepClips;
    [SerializeField] float walkStepInterval = 0.5f;
    [SerializeField] float sprintStepInterval = 0.28f;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;

    bool isSprinting;
    float staminaRegenTimer;

    bool isBlocking;
    Vector3 shieldDefaultLocalPos;
    bool weaponEquipped;

    float shootTimer;

    float baseSpeed;

    float nextStepTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        stamina = maxStamina;
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
        SwingSword();
        Footsteps();

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

        if (Input.GetKeyDown(KeyCode.F) && shootTimer >= shootRate)
        {
            shoot();
        }
    }

    void Footsteps()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;

        if (footstepSource == null || footstepClips == null || footstepClips.Length == 0)
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
            footstepSource.PlayOneShot(clip);
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

        float blockCost = maxStamina * (blockStaminaCost / 100f);

        if (isBlocking && stamina >= blockCost)
        {
            stamina -= blockCost;
            if (stamina < 0f)
                stamina = 0f;

            staminaRegenTimer = 0f;
            updateStaminaUI();

            if (stamina < blockCost)
                isBlocking = false;

            return;
        }

        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            GameManager.instance.YouLose();
        }
    }

    public void updatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    void updateStaminaUI()
    {
        GameManager.instance.playerStaminaBar.fillAmount = stamina / maxStamina;
    }

    IEnumerator flashRed()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
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
    public void SwingSword()
    {
        if (WeaponManager.instance.ItemEquipped() && WeaponManager.instance.currentItem.itemData.isWeapon && Input.GetButtonDown("Fire1"))
        {
            PlayerAnimatorManager.instance.PlayTargetAnimation(PlayerAnimatorManager.instance.playerAnimator, "SwordSwing");
            GameManager.instance.isInteracting = true;
            stamina -= WeaponManager.instance.currentItem.itemData.staminaDrainAmount;
        }
        else
        {
            return;
        }
    }

    void OnInteract()
    {
        if(Input.GetKeyDown(KeyCode.E))
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

    void shoot()
    {
        if (WeaponManager.instance.ringEquipped)
        {
            shootTimer = 0;
            MagicRing magic = InventoryManager.instance.ringSlot.itemInSlot.modelPrefab.GetComponent<MagicRing>();
            GameObject shootEffect = Instantiate(magic.shootEffect, WeaponManager.instance.rightHandTransform, false);
            shootEffect.transform.parent = null;
            Rigidbody shootEffectRB = shootEffect.GetComponent<Rigidbody>();
            shootEffectRB.linearVelocity = Camera.main.transform.forward * magic.shootSpeed * Time.deltaTime;
        }
        else
            return;
    }
}