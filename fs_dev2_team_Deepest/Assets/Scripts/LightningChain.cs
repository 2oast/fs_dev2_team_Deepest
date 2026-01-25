using UnityEngine;

public class LightningChain : MonoBehaviour, ILightning
{
    [SerializeField] float chainRadius;
    [SerializeField] int chainLightningDamage;
    [SerializeField] GameObject chainLightningPref;

    public void ChainLightning()
    {
        Chain();
    }

    void Chain()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, chainRadius);

        foreach (var hit in hits)
        {
            Vector3 targetPos = hit.ClosestPoint(transform.position);
            Vector3 direction = (targetPos - transform.position).normalized;

            IDamage dmg = hit.GetComponent<IDamage>();

            if(dmg != null && !hit.CompareTag("Player"))
            {
                dmg.takeDamage(chainLightningDamage);
            }

            Quaternion lightningRot = Quaternion.LookRotation(direction) * Quaternion.Euler(90,0,0);

            GameObject lightningEffect = Instantiate(chainLightningPref, hit.transform.position, lightningRot);
            lightningEffect.transform.SetParent(hit.transform);
            Destroy(lightningEffect, 2);
        }
    }
}
