using UnityEngine;

public class MoveCam : MonoBehaviour
{
    [SerializeField] Transform cameraPos;
   

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPos.position;
    }
}
