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

    [SerializeField] Transform teleGrabLocation;
    [SerializeField] Transform magicHandTransform;

    [SerializeField] float grabDistance;
    [SerializeField] float grabSpeed;
    [SerializeField] float teleportSpeed;
    [SerializeField] float throwForce;

    [SerializeField] BoxCollider grabCollider;
    

    public enemyAI enemyGrabbed;
    [SerializeField] float grabTimer;


    bool isTelegrabbing;
    bool isPullingEnemy;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        //if i hold down the left mouse button and the an enemy is being grabbed, move the enemies transform towards me and enable the collider
        if(Input.GetButton("Fire2"))
        {
            if(enemyGrabbed != null)
            {
                PullEnemyToHand();
            }
        }

        animator.SetBool("IsTelegrabbing", isTelegrabbing);

        //if i let go while an enemy is being grabbed
        if (Input.GetButtonUp("Fire2") && enemyGrabbed != null && isTelegrabbing)
        {
            PlayerAnimatorManager.instance.PlayTargetAnimation(animator, "CastSpell");
            Throw(enemyGrabbed);
        }

        CastSpell();
    }

    public void CastSpell()
    {
        if (!Input.GetButtonDown("Fire2") || GameManager.instance.isInteracting || WeaponManager.instance.currentRingEquipped == null)
            return;

        SpellType spellType = WeaponManager.instance.currentRingEquipped.spellType;
        RaycastHit hit;
        if(Input.GetButtonDown("Fire2") && !GameManager.instance.isInteracting && WeaponManager.instance.currentRingEquipped != null && !isTelegrabbing)
        {
            if (!spellPrefabsDic.TryGetValue(spellType, out GameObject prefab))
                return;
            else
            {
                switch(spellType)
                {
                    case SpellType.TeleGrab:
                        //Instantiate(prefab, teleGrabLocation);
                        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grabDistance))
                            ShootSpellRayCast(hit, spellType);
                        break;
                    case SpellType.Teleport:
                        //Instantiate(prefab, magicHandTransform);
                        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grabDistance))
                            ShootSpellRayCast(hit, spellType);
                        break;
                }
                PlayerAnimatorManager.instance.PlayTargetAnimation(animator, "SpellCast");
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
            if (grab != null && enemy.isStunned)
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

    void Throw(enemyAI enemy)
    {
        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        enemy.transform.SetParent(null);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;

        rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        enemy.isGrabbed = false;
        enemyGrabbed = null;
        isTelegrabbing = false;
    }

    void PullEnemyToHand()
    {
        enemyGrabbed.transform.position = Vector3.MoveTowards(
            enemyGrabbed.transform.position,
            teleGrabLocation.position,
            grabSpeed * Time.deltaTime
        );

        if (Vector3.Distance(enemyGrabbed.transform.position, teleGrabLocation.position) < 0.05f)
        {
            AttachEnemy(enemyGrabbed);
        }
    }

    void AttachEnemy(enemyAI enemy)
    {
        
        Rigidbody rb = enemy.GetComponent<Rigidbody>();

        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        enemy.transform.SetParent(teleGrabLocation);
        enemy.transform.localPosition = Vector3.zero;
        enemy.transform.localRotation = Quaternion.identity;

        enemy.isGrabbed = true;
        isTelegrabbing = true;
    }

}
