using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Look")]
    [SerializeField] int sens = 100;
    [SerializeField] int lockVertMin = -60;
    [SerializeField] int lockVertMax = 60;
    [SerializeField] bool invertY;
    [SerializeField] Transform parentTransform;

    float camRotX;

    [Header("Camera Shake")]
    [SerializeField] float shakeReturnSpeed = 25f;

    Vector3 defaultLocalPos;
    float shakeTimer;
    float shakeIntensity;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        defaultLocalPos = transform.localPosition;
    }

    void Update()
    {
        Look();
        HandleShake();
    }

    void Look()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;

        if (invertY)
            camRotX += mouseY;
        else
            camRotX -= mouseY;

        camRotX = Mathf.Clamp(camRotX, lockVertMin, lockVertMax);

        transform.localRotation = Quaternion.Euler(camRotX, 0f, 0f);
        parentTransform.Rotate(Vector3.up * mouseX);
    }

    public void Shake(float duration, float intensity)
    {
        shakeTimer = duration;
        shakeIntensity = intensity;
    }

    void HandleShake()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;

            Vector3 offset = Random.insideUnitSphere * shakeIntensity;
            transform.localPosition = defaultLocalPos + offset;
        }
        else
        {
            transform.localPosition =
                Vector3.Lerp(transform.localPosition, defaultLocalPos, Time.deltaTime * shakeReturnSpeed);
        }
    }
}
