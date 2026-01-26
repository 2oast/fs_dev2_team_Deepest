using System.Collections;
using UnityEngine;

public class BridgePedestal : MonoBehaviour, IInteractable
{
    public enum ExtendAxis { X, Z }

    [Header("Key Settings")]
    [SerializeField] string requiredKeyName;
    [SerializeField] bool consumeKeyOnUse = true;

    [Header("Bridge Settings")]
    [SerializeField] Transform bridge;
    [SerializeField] ExtendAxis extendAxis = ExtendAxis.X;
    [SerializeField] float extendDistance = 5f;
    [SerializeField] float extendSpeed = 2f;

    [Header("Save State (BridgeScript)")]
    [SerializeField] BridgeScript bridgeSave;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip extendLoopClip;
    [SerializeField] AudioClip extendEndClip;
    [SerializeField] float loopVolume = 1f;
    [SerializeField] float endVolume = 1f;

    [Header("Camera Shake")]
    [SerializeField] float shakeDuration = 0.25f;
    [SerializeField] float shakeIntensity = 0.05f;
    [SerializeField] float extendShakeIntensity = 0.08f;
    [SerializeField] float shakeRefreshTime = 0.2f;

    [Header("Misc")]
    [SerializeField] bool canOnlyUseOnce = true;

    bool hasActivated = false;
    Vector3 bridgeStartPos;
    Vector3 bridgeEndPos;
    bool initialized = false;

    bool isExtending;
    Coroutine extendRoutine;

    private void Start()
    {
        
    }

    void Init()
    {
        if (initialized)
            return;

        if (bridge == null)
        {
            Debug.LogWarning("BridgePedestal on " + name + " has no bridge assigned.");
            return;
        }

        bridgeStartPos = bridge.position;

        Vector3 offset = Vector3.zero;
        if (extendAxis == ExtendAxis.X)
            offset = new Vector3(extendDistance, 0f, 0f);
        else
            offset = new Vector3(0f, 0f, extendDistance);

        bridgeEndPos = bridgeStartPos + offset;
        initialized = true;

        if (bridgeSave != null)
        {
            bridgeSave.SetExtendedWorldPosition(bridgeEndPos);
        }

        if (bridgeSave != null && bridgeSave.IsExtended)
        {
            hasActivated = true;
            bridge.position = bridgeEndPos;
        }
    }

    public void Interact()
    {
        Init();

        if (bridge == null)
            return;

        if (canOnlyUseOnce && hasActivated)
            return;

        if (isExtending)
            return;

        if (bridgeSave != null && bridgeSave.IsExtended)
        {
            hasActivated = true;
            return;
        }

        if (!PlayerHasKey(requiredKeyName))
        {
            Debug.Log("BridgePedestal: player does not have required key: " + requiredKeyName);
            return;
        }

        if (consumeKeyOnUse)
            ConsumeKey(requiredKeyName);

        hasActivated = true;
        isExtending = true;

        PlayerCam cam = null;
        if (Camera.main != null)
            cam = Camera.main.GetComponent<PlayerCam>();
        if (cam != null)
            cam.Shake(shakeDuration, shakeIntensity);

        if (extendRoutine != null)
            StopCoroutine(extendRoutine);

        extendRoutine = StartCoroutine(ExtendBridge());
    }

    IEnumerator ExtendBridge()
    {
        if (audioSource != null && extendLoopClip != null)
        {
            audioSource.loop = true;
            audioSource.clip = extendLoopClip;
            audioSource.volume = loopVolume;
            audioSource.Play();
        }

        PlayerCam cam = null;
        if (Camera.main != null)
            cam = Camera.main.GetComponent<PlayerCam>();

        if (cam != null)
            cam.StartShakeLoop(extendShakeIntensity, shakeRefreshTime);

        float t = 0f;
        Vector3 startPos = bridge.position;
        Vector3 targetPos = bridgeEndPos;

        while (t < 1f)
        {
            t += Time.deltaTime * extendSpeed;
            bridge.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        bridge.position = targetPos;

        if (audioSource != null)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.loop = false;
            audioSource.clip = null;

            if (extendEndClip != null)
                audioSource.PlayOneShot(extendEndClip, endVolume);
        }

        if (cam != null)
            cam.StopShakeLoop();

        if (bridgeSave != null)
            bridgeSave.SetExtended(true);

        isExtending = false;
        extendRoutine = null;
    }

    bool PlayerHasKey(string keyName)
    {
        if (InventoryManager.instance == null || InventoryManager.instance.slots == null)
            return false;

        foreach (var slot in InventoryManager.instance.slots)
        {
            if (slot != null && slot.itemInSlot != null)
            {
                if (slot.itemInSlot.itemName == keyName)
                    return true;
            }
        }

        return false;
    }

    void ConsumeKey(string keyName)
    {
        if (InventoryManager.instance == null || InventoryManager.instance.slots == null)
            return;

        foreach (var slot in InventoryManager.instance.slots)
        {
            if (slot != null && slot.itemInSlot != null && slot.itemInSlot.itemName == keyName)
            {
                slot.UseItem();
                return;
            }
        }
    }

    void OnDisable()
    {
        if (extendRoutine != null)
        {
            StopCoroutine(extendRoutine);
            extendRoutine = null;
        }

        isExtending = false;

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        if (Camera.main != null)
        {
            PlayerCam cam = Camera.main.GetComponent<PlayerCam>();
            if (cam != null)
                cam.StopShakeLoop();
        }
    }
}