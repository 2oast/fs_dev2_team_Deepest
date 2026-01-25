using UnityEngine;

public class BridgeScript : MonoBehaviour
{
    [Header("Unique ID for this bridge")]
    public string id;

    [Header("Bridge Transform")]
    [SerializeField] Transform bridgeTransform;

    [Header("World Positions")]
    [SerializeField] Vector3 retractedWorldPos;
    [SerializeField] Vector3 extendedWorldPos;

    [SerializeField] bool isExtended;
    public bool IsExtended => isExtended;

    bool initialized;

    void Awake()
    {
        if (bridgeTransform == null)
            bridgeTransform = transform;

        if (!initialized)
        {
            retractedWorldPos = bridgeTransform.position;
            initialized = true;
        }

        if (!TitleScreenManager.loadFromSave)
        {
            isExtended = false;

            if (bridgeTransform != null)
            {
                bridgeTransform.position = retractedWorldPos;
            }
        }
    }

    public void SetExtended(bool extended)
    {
        isExtended = extended;

        if (bridgeTransform != null)
        {
            bridgeTransform.position = extended ? extendedWorldPos : retractedWorldPos;
        }
    }

    public void ApplyState(bool extended)
    {
        isExtended = extended;

        if (bridgeTransform != null)
        {
            bridgeTransform.position = extended ? extendedWorldPos : retractedWorldPos;
        }
    }

    public void SetExtendedWorldPosition(Vector3 endPos)
    {
        extendedWorldPos = endPos;
    }
}

