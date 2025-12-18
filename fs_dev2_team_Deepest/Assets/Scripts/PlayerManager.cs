using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public int interactDistance;

    private void Start()
    {
        instance = this;
    }

    
}
