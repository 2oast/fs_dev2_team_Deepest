using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Amount")]
    public float positionSway = 0.03f;
    public float rotationSway = 4f;

    [Header("Smoothing")]
    public float smoothTime = 8f;

    [Header("Limits")]
    public float maxPositionSway = 0.05f;
    public float maxRotationSway = 6f;

    Vector3 initialLocalPos;
    Quaternion initialLocalRot;

    Vector3 currentPos;
    Quaternion currentRot;

    void Start()
    {
        initialLocalPos = transform.localPosition;
        initialLocalRot = transform.localRotation;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        
        Vector3 targetPos = initialLocalPos +
            new Vector3(
                Mathf.Clamp(-mouseX * positionSway, -maxPositionSway, maxPositionSway),
                Mathf.Clamp(-mouseY * positionSway, -maxPositionSway, maxPositionSway),
                0f
            );

        Quaternion targetRot = initialLocalRot *
            Quaternion.Euler(
                Mathf.Clamp(-mouseY * rotationSway, -maxRotationSway, maxRotationSway),
                Mathf.Clamp(mouseX * rotationSway, -maxRotationSway, maxRotationSway),
                0f
            );

        currentPos = Vector3.Lerp(currentPos, targetPos, smoothTime * Time.deltaTime);
        currentRot = Quaternion.Slerp(currentRot, targetRot, smoothTime * Time.deltaTime);

        transform.localPosition = currentPos;
        transform.localRotation = currentRot;
    }
}
