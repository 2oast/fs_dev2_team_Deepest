using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.AI;

public enum SpellType
{
    TeleGrab,
    Teleport,
    Shield
}

public class MagicController : MonoBehaviour
{
    [SerializeField] Animator animator;

    Dictionary<SpellType, GameObject> spellPrefabsDic;

    [SerializeField] GameObject teleGrabPref;
    [SerializeField] GameObject teleportPref;
    [SerializeField] GameObject shieldPref;

    [SerializeField] GameObject throwPref;

    [SerializeField] Transform teleGrabLocation;
    [SerializeField] Transform magicHandTransform;

    [SerializeField] AudioSource audSource;
    [SerializeField] AudioClip grabClip;
    [SerializeField] AudioClip throwClip;

    [SerializeField] float grabDistance;
    [SerializeField] float grabSpeed;
    [SerializeField] float teleportSpeed;
    [SerializeField] float throwForce;

    float originalCamFov;
    [SerializeField] float grabFov;
    [SerializeField] float moveInSpeed;

    [SerializeField] BoxCollider grabCollider;

    public GameObject objectGrabbed;


    public enemyAI enemyGrabbed;
    [SerializeField] float grabTimer;


    public bool isTelegrabbing;
    bool isPullingEnemy;
    bool isDonePunching;

    private GameObject currentPrefInstance;

    private void Awake()
    {
        spellPrefabsDic = new Dictionary<SpellType, GameObject>()
        {
            {SpellType.TeleGrab, teleGrabPref},
            {SpellType.Teleport, teleportPref},
            {SpellType.Shield, shieldPref}
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

        float targetFov = isTelegrabbing ? grabFov : originalCamFov;
        Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, targetFov, Time.deltaTime * moveInSpeed);
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
                            if(objectGrabbed != null)
                            {
                                GameObject effect = Instantiate(prefab, teleGrabLocation);
                                currentPrefInstance = effect;
                                audSource.PlayOneShot(grabClip);
                            }
                        }
                        break;
                    case SpellType.Teleport:
                        //Instantiate(prefab, magicHandTransform);
                        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grabDistance))
                            ShootSpellRayCast(hit, spellType);
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
                grab.Grab(this);
            }
        }

        if (spell == SpellType.Teleport)
        {
            if (teleport != null)
            {
                Teleport(enemy);
                teleport.Teleport();
            }
        }
    }

    void Teleport(enemyAI enemy)
    {
        isTelegrabbing = false;
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
        Destroy(currentPrefInstance);

    }

    void PullEnemyToHand()
    {
        
        if (objectGrabbed == null) return; // early out

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
    }

    

}
