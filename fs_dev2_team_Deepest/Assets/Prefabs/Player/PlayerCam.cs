using System.Collections;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensY;
    public float sensX;
    float mouseX;
    float mouseY;
    public Transform orientation;

    float xRot;
    float yRot;

    [Header("Camera Shake")]
    [SerializeField] float shakeReturnSpeed = 25f;

    Vector3 defaultLocalPos;
    float shakeTimer;
    float shakeIntensity;

    Coroutine shakeLoop;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultLocalPos = transform.localPosition;
    }

    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;

        yRot += mouseX;
        xRot -= mouseY;

        xRot = Mathf.Clamp(xRot, -90f, 90f);

        HandleShake();
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(xRot, yRot, 0f);

        if (orientation != null)
            orientation.rotation = Quaternion.Euler(0f, yRot, 0f);
    }

    public void Shake(float duration, float intensity)
    {
        if (intensity > shakeIntensity)
            shakeIntensity = intensity;

        if (duration > shakeTimer)
            shakeTimer = duration;
    }

    public void StartShakeLoop(float intensity, float refreshTime = 0.2f)
    {
        if (refreshTime <= 0f)
            refreshTime = 0.2f;

        StopShakeLoop();
        shakeLoop = StartCoroutine(ShakeLoop(intensity, refreshTime));
    }

    public void StopShakeLoop()
    {
        if (shakeLoop != null)
        {
            StopCoroutine(shakeLoop);
            shakeLoop = null;
        }

        shakeTimer = 0f;
        shakeIntensity = 0f;
    }

    IEnumerator ShakeLoop(float intensity, float refreshTime)
    {
        while (true)
        {
            Shake(refreshTime, intensity);

            yield return new WaitForSeconds(refreshTime * 0.5f);
        }
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

            shakeIntensity = 0f;
        }
    }
}
