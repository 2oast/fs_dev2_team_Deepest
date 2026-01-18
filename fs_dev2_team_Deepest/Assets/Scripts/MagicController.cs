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
    Speed
}

public class MagicController : MonoBehaviour
{
    [SerializeField] Animator animator;

    Dictionary<SpellType, GameObject> spellPrefabsDic;

    [Header("SpellPrefabs")]
    [SerializeField] GameObject teleGrabPref;
    [SerializeField] GameObject teleportPref;
    [SerializeField] GameObject shieldPref;
    [SerializeField] GameObject speedPref;
    [SerializeField] GameObject throwPref;

    [Header("Transforms")]
    [SerializeField] Transform teleGrabLocation;
    [SerializeField] Transform magicHandTransform;

    [Header("Sound")]
    [SerializeField] AudioSource audSource;
    [SerializeField] AudioClip grabClip;
    [SerializeField] AudioClip throwClip;

    [Header("Floats")]
    [SerializeField] float grabDistance;
    [SerializeField] float grabSpeed;
    [SerializeField] float teleportSpeed;
    [SerializeField] float throwForce;
    [SerializeField] float speedDuration;
    [SerializeField] float grabTimer;
    [SerializeField] float speedTimer;
    float originalCamFov;
    [SerializeField] float grabFov;
    [SerializeField] float moveInSpeed;

    [Header("References")]
    [SerializeField] BoxCollider grabCollider;
    public GameObject objectGrabbed;
    public enemyAI enemyGrabbed;
    
    [Header("flags")]
    public bool isTelegrabbing;
    bool isTeleporting;
    public bool isBoosting;

    private void Awake()
    {
        spellPrefabsDic = new Dictionary<SpellType, GameObject>()
        {
            {SpellType.TeleGrab, teleGrabPref},
            {SpellType.Teleport, teleportPref},
            {SpellType.Shield, shieldPref},
            {SpellType.Speed,  speedPref}
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
        //if i hold down the left mouse button and the an enemy is being grabbed, move the enemies transform towards me and enable the collider
        if (Input.GetButton("Fire2"))
        {
            if (objectGrabbed != null)
            {
                PullEnemyToHand();
            }
        }

        animator.SetBool("IsTelegrabbing", isTelegrabbing);

        //if i let go while an enemy is being grabbed
        if (Input.GetButtonUp("Fire2") && isTelegrabbing && objectGrabbed != null)
        {
            Throw(objectGrabbed);
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

    void Throw(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        objectGrabbed.transform.SetParent(null);

        objectGrabbed = null;
        isTelegrabbing = false;

        rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);

        audSource.PlayOneShot(throwClip);
        GameObject effect = Instantiate(throwPref, teleGrabLocation);
        Destroy(effect, 3);
        teleGrabPref.SetActive(false);//Destroy(currentPrefInstance);

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
        var player = GameManager.instance.playerControllerScript;

        int originalSpeed = player.speed;
        player.speed = 25;

        yield return new WaitForSeconds(duration);

        player.speed = 5;
        speedPref.SetActive(false);

        isBoosting = false;
    }

}
