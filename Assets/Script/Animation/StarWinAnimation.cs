using System.Collections;
using UnityEngine;

public class StarWinAnimation : MonoBehaviour
{
    private LTDescr rotateTween;

    [SerializeField] private float fastRotateSpeed = 1f;
    [SerializeField] private float slowRotateSpeed = .5f;
    [SerializeField] private float autoStopDelay = .5f;

    private void OnEnable()
    {
        PlayStarAnimation();
    }

    public void PlayStarAnimation()
    {
        gameObject.SetActive(true);

        // Start fast infinite rotation
        rotateTween = LeanTween.rotateAround(gameObject, Vector3.forward, 360f, fastRotateSpeed)
            .setLoopClamp();

        StartCoroutine(StopAfterDelay());
    }

    private IEnumerator StopAfterDelay()
    {
        yield return new WaitForSeconds(autoStopDelay);
        StopStarAnimation();
    }

    public void StopStarAnimation()
    {
        if (rotateTween != null)
        {
            LeanTween.cancel(gameObject);
        }

        // Smooth deceleration (simulate stop)
        LeanTween.rotateAround(gameObject, Vector3.forward, 360f, slowRotateSpeed)
            .setEaseOutQuad()
            .setLoopOnce();
    }
}
