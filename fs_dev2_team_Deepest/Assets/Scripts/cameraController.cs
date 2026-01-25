using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;
    [SerializeField] Transform parentTransform;

    float camRotX;

    void Start()
    {
        SetCursorLocked(true);
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isInteracting)
        {
            SetCursorLocked(false);
            return;
        }

        SetCursorLocked(true);

        float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;

        if (invertY)
            camRotX += mouseY;
        else
            camRotX -= mouseY;

        camRotX = Mathf.Clamp(camRotX, lockVertMin, lockVertMax);

        parentTransform.Rotate(Vector3.up * mouseX);
    }

    void SetCursorLocked(bool locked)
    {
        Cursor.visible = !locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
