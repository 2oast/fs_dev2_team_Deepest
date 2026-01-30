using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.AI;

public enum SpellType
{
    TeleGrab,
    Teleport,
    Shield,
    Speed,
    Fire,
    Lightning
}

public class MagicController : MonoBehaviour
{
    [SerializeField] Animator animator;

    Dictionary<SpellType, GameObject> spellPrefabsDic;

    [Header("SpellPrefabs")]
    public GameObject teleGrabPref;
    [SerializeField] GameObject teleportPref;
    [SerializeField] GameObject shieldPref;
    [SerializeField] GameObject speedPref;
    [SerializeField] GameObject lightningPref;
    public GameObject throwPref;
    [SerializeField] GameObject firePref;
    public GameObject explodePref;
    [SerializeField] GameObject sparkPrefab;
    GameObject currentPrefabInstance;

    [Header("Transforms")]
    public Transform teleGrabLocation;
    [SerializeField] Transform magicHandTransform;

    [Header("Sound")]
    public AudioSource audSource;
    [SerializeField] AudioClip grabClip;
    public AudioClip throwClip;
    [SerializeField] AudioClip fireBallClip;
    [SerializeField] AudioClip fireBallChargeClip;
    [SerializeField] AudioClip lightningChargeClip;
    [SerializeField] AudioClip lightningCastClip;



    [Header("Floats")]
    [SerializeField] float grabDistance;
    [SerializeField] float grabSpeed;
    [SerializeField] float teleportSpeed;
    public float throwForce;
    [SerializeField] float speedDuration;
    [SerializeField] float grabTimer;
    [SerializeField] float speedTimer;
    [SerializeField] float fireBallSpeed;
    float originalCamFov;
    [SerializeField] float grabFov;
    [SerializeField] float moveInSpeed;
    [SerializeField] float lightningDistance;
    [SerializeField] int lightningDamage;

    [Header("References")]
    [SerializeField] BoxCollider grabCollider;
    public GameObject objectGrabbed;
    public enemyAI enemyGrabbed;
    
    [Header("flags")]
    public bool isTelegrabbing;
    bool isTeleporting;
    public bool isBoosting;
    bool isChargingFireball;
    bool isChargingLightning;

    [SerializeField] LayerMask ignoreLayer;
    public IThrow throwObject;

    private void Awake()
    {
        spellPrefabsDic = new Dictionary<SpellType, GameObject>()
        {
            {SpellType.TeleGrab, teleGrabPref},
            {SpellType.Teleport, teleportPref},
            {SpellType.Shield, shieldPref},
            {SpellType.Speed,  speedPref},
            {SpellType.Fire, firePref },
            {SpellType.Lightning, sparkPrefab }
        };
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalCamFov = Camera.main.fieldOfView;
    }


    // Update is called once per frame
    void Update()
    {

        if (WeaponManager.instance.currentRingEquipped == null)
            return;

        //if i hold down the left mouse button and the an enemy is being grabbed, move the enemies transform towards me and enable the collider
        if (Input.GetButton("Fire2"))
        {
            if (objectGrabbed != null)
            {
                PullEnemyToHand();
            }
        }

        if(Input.GetButton("Fire2") && WeaponManager.instance.currentRingEquipped.spellType == SpellType.Fire)
        {
            isChargingFireball = true;
        }

        if (Input.GetButton("Fire2") && WeaponManager.instance.currentRingEquipped.spellType == SpellType.Lightning)
        {
            isChargingLightning = true;
        }

        if (WeaponManager.instance.currentRingEquipped != null)
        {
            if (WeaponManager.instance.currentRingEquipped.spellType == SpellType.TeleGrab)
                animator.SetBool("IsTelegrabbing", isTelegrabbing);

            else if (WeaponManager.instance.currentRingEquipped.spellType == SpellType.Fire)
                animator.SetBool("IsTelegrabbing", isChargingFireball);

            else if (WeaponManager.instance.currentRingEquipped.spellType == SpellType.Lightning)
                animator.SetBool("IsTelegrabbing", isChargingLightning);
        }

        //if i let go while an enemy is being grabbed
        if (Input.GetButtonUp("Fire2") && isTelegrabbing && objectGrabbed != null)
        {
            throwObject.Throw(this);
        }

        if(Input.GetButtonUp("Fire2") && isChargingFireball)
        {
            
            StartCoroutine(FireBallFly(7));
        }

        if (Input.GetButtonUp("Fire2") && isChargingLightning)
        {
            ShootLightning();
        }


        CastSpell();

        float targetFov = originalCamFov;

        if (isBoosting)
            targetFov = grabFov / 2f;
        else if (isTeleporting)
            targetFov = grabFov;
        else if (isTelegrabbing)
            targetFov = grabFov;

        Camera.main.fieldOfView = Mathf.MoveTowards(
            Camera.main.fieldOfView,
            targetFov,
            Time.deltaTime * moveInSpeed);

        if(isTelegrabbing && objectGrabbed == null)
        {
            isTelegrabbing = false;
            teleGrabPref.SetActive(false);
        }

    }

    public void CastSpell()
    {
        if (!Input.GetButtonDown("Fire2") || GameManager.instance.isInteracting || WeaponManager.instance.currentRingEquipped == null)
            return;

        SpellType spellType = WeaponManager.instance.currentRingEquipped.spellType;
        RaycastHit hit;
        if (Input.GetButtonDown("Fire2") && !GameManager.instance.isInteracting && WeaponManager.instance.currentRingEquipped != null && !isTelegrabbing)
        {
            if (!spellPrefabsDic.TryGetValue(spellType, out GameObject prefab))
                return;
            else
            {
                switch (spellType)
                {
                    case SpellType.TeleGrab:
                        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grabDistance))
                        {
                            ShootSpellRayCast(hit, spellType);
                            //if(objectGrabbed != null)
                            //{
                            //    GameObject telegrabeffect = Instantiate(prefab, teleGrabLocation);
                            //    currentPrefInstance = telegrabeffect;
                                audSource.PlayOneShot(grabClip);
                           // }
                        }
                        break;
                    case SpellType.Teleport:
                        GameObject effect = Instantiate(prefab, teleGrabLocation);
                        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grabDistance))
                            ShootSpellRayCast(hit, spellType);
                        if(effect != null)
                        {
                            Destroy(effect, 1);
                        }
                        break;
                    case SpellType.Speed:
                        if(!isBoosting)
                        StartCoroutine(SpeedBoost(2));
                        Debug.Log("BOOSTING");
                        break;
                    case SpellType.Fire:
                        currentPrefabInstance = Instantiate(prefab, magicHandTransform);
                        audSource.PlayOneShot(fireBallChargeClip);
                         break;
                    case SpellType.Lightning:
                        currentPrefabInstance = Instantiate(prefab, magicHandTransform);
                        audSource.PlayOneShot(lightningChargeClip);
                        break;
                }
                PlayerAnimatorManager.instance.PlayTargetAnimation(animator, "MagicCast", .1f);
            }
        }
    }

    void ShootSpellRayCast(RaycastHit hit, SpellType spell)
    {
        IGrab grab = hit.collider.GetComponent<IGrab>();
        ITeleport teleport = hit.collider.GetComponent<ITeleport>();
        enemyAI enemy = hit.collider.GetComponent<enemyAI>();

        if (spell == SpellType.TeleGrab)
        {
            if (grab != null)
            {
                if(enemy != null && enemy.isStunned)
                {
                    grab.Grab(this);
                }
                else if(enemy == null)
                {
                    grab.Grab(this);
                }

            }
        }

        if (spell == SpellType.Teleport)
        {
            if (teleport != null)
            {
                StartCoroutine(TeleportToEnemy(.5f, hit));
            }
        }

    }

    void ShootLightning()
    {
        GameObject lightningBolt = Instantiate(lightningPref, magicHandTransform);
        Destroy(lightningBolt, .5f);
        Destroy(currentPrefabInstance);
        RaycastHit hit;
        audSource.PlayOneShot(lightningCastClip);

        if (Physics.Raycast(transform.position, Camera.main.transform.forward, out hit, lightningDistance, ~ignoreLayer))
        {
            ILightning lightning = hit.collider.GetComponent<ILightning>();
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (lightning != null)
            {
                lightning.ChainLightning();
            }

            if (dmg != null && !hit.collider.CompareTag("Player"))
            {
                dmg.takeDamage(lightningDamage);
            }

            GameObject sparkEffect = Instantiate(sparkPrefab, hit.point, Quaternion.identity);

            Destroy(sparkEffect, 2);
        }

        isChargingLightning = false;
    }

    void PullEnemyToHand()
    {
        
        if (objectGrabbed == null) return;

        Rigidbody rb = objectGrabbed.GetComponent<Rigidbody>();
        Vector3 targetPos = teleGrabLocation.position;
        rb.MovePosition(Vector3.MoveTowards(rb.position, targetPos, grabSpeed * Time.fixedDeltaTime));

        if (Vector3.Distance(rb.position, targetPos) < 1f)
        {
            AttachEnemy(objectGrabbed);

        }
    }

    void AttachEnemy(GameObject obj)
    {

        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        obj.transform.SetParent(teleGrabLocation);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        rb.isKinematic = false;
        isTelegrabbing = true;
        teleGrabPref.SetActive(true);
    }

    IEnumerator TeleportToEnemy(float duration, RaycastHit hit)
    {
        isTeleporting = true;

        var controller = GameManager.instance.playerControllerScript.controller;

        Vector3 start = transform.position;
        Vector3 target = hit.collider.transform.position
                         - transform.forward;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            // Ease out (snappy start, smooth stop)
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            Vector3 nextPos = Vector3.Lerp(start, target, easedT);
            controller.Move(nextPos - transform.position);

            yield return null;
        }

        isTeleporting = false;
    }

    IEnumerator SpeedBoost(float duration)
    {

        isBoosting = true;
        speedPref.SetActive(true);
        var player = GameManager.instance.playerControllerScript.playerMovement;

        int originalSpeed = player.speed;
        player.speed = 25;

        yield return new WaitForSeconds(duration);

        player.speed = 5;
        speedPref.SetActive(false);

        isBoosting = false;
    }

    IEnumerator FireBallFly(float duration)
    {
        audSource.pitch = Random.Range(.8f, 1.2f);
        audSource.PlayOneShot(fireBallClip);
        GameObject fireball = Instantiate(currentPrefabInstance, magicHandTransform);
        fireball.transform.SetParent(null);
        Destroy(currentPrefabInstance);
        GameObject explosionEffect = Instantiate(explodePref, magicHandTransform);
        Destroy(explosionEffect, 1f);

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 dir;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~ignoreLayer))
            dir = (hit.point - fireball.transform.position).normalized;
        else
            dir = Camera.main.transform.forward;

        isChargingFireball = false;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            fireball.transform.position += dir * fireBallSpeed * timer;
            yield return null;
        }

        Destroy(fireball);
    }

}
