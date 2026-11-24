using UnityEngine;

public abstract class LoadingScreen : MonoBehaviour
{
    public float TransitionDuration = 0.5f;
    public abstract void StartTransition();

    public abstract void EndTransition();

    public abstract void UpdateLoadingView(float loadingPercent);
}
