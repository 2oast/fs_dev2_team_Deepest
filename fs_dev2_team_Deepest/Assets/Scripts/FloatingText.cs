using TreeEditor;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public Vector3 offSet = new Vector3(0, 2, 0);
    public Vector3 RandomizeIntensity = new Vector3(0.5f, 0, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localPosition += offSet;
        transform.localPosition += new Vector3(
            Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
            Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y),
            Random.Range(-RandomizeIntensity.z, RandomizeIntensity.z)
            );
    }

   
}
