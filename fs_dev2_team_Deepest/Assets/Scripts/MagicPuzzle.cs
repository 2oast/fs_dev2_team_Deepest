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
