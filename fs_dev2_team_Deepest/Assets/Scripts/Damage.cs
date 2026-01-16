using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum damageType { moving, stationary, DOT, homing }
    public enum ElementalType { Fire, Ice, Earth, Wind }
    public ElementalType elementalType;
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    bool isDamaging;

    [SerializeField] Transform target;
    public Transform Target { get { return target; } set { target = value; } }

    [Header("Poison")]
    [SerializeField] bool applyDotOnHit = false;
    [SerializeField] float dotDuration = 50f;
    [SerializeField] float dotTickInterval = 5f;
    [SerializeField] int dotDamagePerTick = 1;

    void Start()
    {
        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject, destroyTime);
        }
    }

    void Update()
    {
        if (type == damageType.homing)
        {
            rb.linearVelocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponentInParent<IDamage>();

        if (dmg != null && type != damageType.DOT)
        {
            dmg.takeDamage(damageAmount);

            if (applyDotOnHit && other.CompareTag("Player"))
            {
                PlayerController pc = GameManager.instance != null ? GameManager.instance.playerControllerScript : null;
                if (pc != null)
                {
                    pc.ApplyPoison(dotDuration, dotTickInterval, dotDamagePerTick);
                }
            }
        }

        if (type == damageType.homing || type == damageType.moving)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponentInParent<IDamage>();

        if (dmg != null && type == damageType.DOT && !isDamaging && !applyDotOnHit)
        {
            StartCoroutine(DamageOther(dmg));
        }
    }

    IEnumerator DamageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }

    int ComputeFinalDamage()
    {
        int finalDamage = damageAmount;

        if (CompareTag("PlayerMelee") && SkillManager.instance != null)
        {
            float mult = SkillManager.instance.GetMeleeDamageMultiplier();
            finalDamage = Mathf.CeilToInt(damageAmount * mult);
        }
        else if (CompareTag("PlayerRanged") && SkillManager.instance != null)
        {
            float mult = SkillManager.instance.GetRangedDamageMultiplier();
            finalDamage = Mathf.CeilToInt(damageAmount * mult);
        }

        if (finalDamage < 0)
            finalDamage = 0;

        return finalDamage;
    }
}
