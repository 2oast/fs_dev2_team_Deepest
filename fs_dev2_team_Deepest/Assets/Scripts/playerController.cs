using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Range(1, 10)][SerializeField] int HP;
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

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;

    bool isSprinting;
    float staminaRegenTimer;

    bool isBlocking;
    Vector3 shieldDefaultLocalPos;

    float shootTimer;

    float baseSpeed;

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

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            shoot();
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

    void shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
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

}