using UnityEngine;

public class Rock : MonoBehaviour, IGrab
{
    [SerializeField] SphereCollider hitCollider;
    public void Grab(MagicController magicController)
    {
        hitCollider.enabled = true;
        hitCollider.isTrigger = true;
        magicController.objectGrabbed = gameObject;
    }

  
   
}
