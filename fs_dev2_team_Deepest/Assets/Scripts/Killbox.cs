using UnityEngine;

public class Killbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IDamage dmg = other.GetComponent<IDamage>();

        if(dmg != null)
        {
            dmg.takeDamage(1000);
        }
    }
}
