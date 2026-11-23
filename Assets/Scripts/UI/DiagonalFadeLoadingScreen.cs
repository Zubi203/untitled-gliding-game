using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SideSwipeLoadingScreen : LoadingScreen
{
    [SerializeField] private Slider transitionProgress;
    private float sceneTransitionEnterCountdown;
    private float sceneTransitionExitCountdown;



    public override void EndTransition()
    {
        StartCoroutine(SweepRightExit());
    }

    public override void StartTransition()
    {
        StartCoroutine(SweepRightEnter());
    }

    public override void UpdateLoadingView(float loadingPercent)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator SweepRightEnter()
    {
        transitionProgress.direction = Slider.Direction.LeftToRight;
        transitionProgress.value = 0f;
        sceneTransitionEnterCountdown = 0f;
        while (sceneTransitionEnterCountdown < TransitionDuration)
        {
            sceneTransitionEnterCountdown += Time.deltaTime;
            float progress = sceneTransitionEnterCountdown / TransitionDuration;
            transitionProgress.value = progress;
            yield return null;
        }
    }

    private IEnumerator SweepRightExit()
    {
        transitionProgress.direction = Slider.Direction.RightToLeft;
        transitionProgress.value = 1f;
        sceneTransitionExitCountdown = TransitionDuration;
        while (sceneTransitionExitCountdown > 0f)
        {
            sceneTransitionExitCountdown -= Time.deltaTime;
            float progress = sceneTransitionExitCountdown / TransitionDuration;
            transitionProgress.value = progress;
            yield return null;
        } 
    }
}
