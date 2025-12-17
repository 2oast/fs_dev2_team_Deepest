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
        if (other.CompareTag("Player"))
        {
            if (!isMelting && GameManager.instance != null)
                GameManager.instance.ShowIcePrompt(true);
        }

        Damage dmg = other.GetComponent<Damage>();
        if (dmg != null && dmg.elementalType == Damage.ElementalType.Fire)
        {
            isMelting = true;

            if (GameManager.instance != null)
                GameManager.instance.ShowIcePrompt(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.instance != null)
            GameManager.instance.ShowIcePrompt(false);
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
            if (GameManager.instance != null)
                GameManager.instance.ShowIcePrompt(false);

            Destroy(gameObject);
        }
    }

}
