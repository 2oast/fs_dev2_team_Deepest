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
            int baseDamage = GameManager.instance.playerControllerScript.chargeAttack ? WeaponManager.instance.currentWeapon.damage * 2 : WeaponManager.instance.currentWeapon.damage;

            int finalDamage = ComputeFinalDamage();
            dmg.takeDamage(finalDamage);
        }

        if (type == damageType.homing || type == damageType.moving)
        {
            Destroy(gameObject);
        }

        IDestructible destruct = other.GetComponent<IDestructible>();
        if (destruct != null)
            destruct.Destruct();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponentInParent<IDamage>();

        if (dmg != null && type == damageType.DOT && !isDamaging)
        {
            StartCoroutine(DamageOther(dmg));
        }
    }

    IEnumerator DamageOther(IDamage d)
    {
        isDamaging = true;
        int finalDamage = ComputeFinalDamage();
        d.takeDamage(finalDamage);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }

    int ComputeFinalDamage()
    {
        int baseDamage = GameManager.instance.playerControllerScript.chargeAttack ? WeaponManager.instance.currentWeapon.damage * 2 : WeaponManager.instance.currentWeapon.damage;

        int finalDamage = baseDamage;

        if (CompareTag("PlayerMelee") && SkillManager.instance != null)
        {
            float mult = SkillManager.instance.GetMeleeDamageMultiplier();
            finalDamage = Mathf.CeilToInt(baseDamage * mult);
        }
        else if (CompareTag("PlayerRanged") && SkillManager.instance != null)
        {
            float mult = SkillManager.instance.GetRangedDamageMultiplier();
            finalDamage = Mathf.CeilToInt(baseDamage * mult);
        }

        if (finalDamage < 0)
            finalDamage = 0;

        return finalDamage;
    }

    
}
