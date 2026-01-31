using System.Collections;
using UnityEngine;

public class OpenSwingingDoor : MonoBehaviour, IInteractable
{
    bool isClosed = true;
    Quaternion originalRot;

    private void Start()
    {
        originalRot = transform.rotation;
    }

    public void Interact()
    {
        if(isClosed)
        {
            isClosed = false;
            StartCoroutine(OpenDoor(5));
        }
        else
        {
            return;
        }
    }

   IEnumerator OpenDoor(float duration)
    {
        float t = 0;
        Quaternion targetRot = Quaternion.Euler(new Vector3(0, 75, 0));
        while(t < 1)
        {
            t += Time.deltaTime / duration;
            transform.rotation = Quaternion.Slerp(originalRot, targetRot, t);
            yield return null;
        }
        transform.rotation = targetRot;
    }
}
