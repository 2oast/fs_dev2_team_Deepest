using UnityEngine;

public class MagicPuzzle : MonoBehaviour
{
    [SerializeField] float meltSpeed = 0.5f;

    MeshRenderer meshRenderer;
    Material material;
    Color color;

    bool isMelting;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material; 
        color = material.color;
    }

    void OnTriggerEnter(Collider other)
    {
        Damage dmg = other.GetComponent<Damage>();
        if (dmg == null) return;

        if (dmg.elementalType == Damage.ElementalType.Fire)
        {
            isMelting = true;
        }
    }

    void Update()
    {
        if (!isMelting) 
            return;

        MeltIce();
    }

    void MeltIce()
    {
        color.a -= meltSpeed * Time.deltaTime;
        color.a = Mathf.Clamp01(color.a);
        material.color = color;

        if (color.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
