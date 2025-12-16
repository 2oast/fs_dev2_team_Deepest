using UnityEngine;

public class MagicRing : Item
{
    [SerializeField] int castingCost;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    public int shootSpeed;

    public GameObject shootEffect;
}
