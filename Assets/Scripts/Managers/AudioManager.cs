using UnityEngine;

public class AudioManager : Service
{
    void Awake()
    {
        RegisterToServiceLocator(this);
    }

    void OnDestroy()
    {
        UnregisterFromServiceLocator<AudioManager>();
    }
}
