using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Timer : MonoBehaviour
{
    [Space, Tooltip("In game time in minutes")]
    [SerializeField] float dayDuration = 6;
    
    [Header("UI")]
    [SerializeField] private ClockUI clockUI = default;
    [SerializeField] float timeToWarning = 5.5f;
    [SerializeField] private float warningPulseTime = 0.5f;
    [SerializeField] private float rotation = 270f;
    private WaitForSeconds rotateTime;
    private const float RotationStep = 6f;
    private Tween warningTween;

    private bool isDay = false;

    public void StartDay()
    {
        isDay = true;
        StartCoroutine(GameTimer());
        
        // UI
        rotateTime = new WaitForSeconds(dayDuration/rotation * RotationStep * 60);
        StartCoroutine(ClockHandRotation());
        StartCoroutine(ClockUIWarning());
    }
        
    private IEnumerator GameTimer()
    {
        yield return new WaitForSeconds(dayDuration * 60);
        EndTime();
    }
    
    private IEnumerator ClockUIWarning()
    {
        yield return new WaitForSeconds(timeToWarning * 60);
        warningTween = clockUI.face.DOColor(Color.red, warningPulseTime)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Flash)
            .Play();
    }

    private IEnumerator ClockHandRotation()
    {
        while (isDay)
        {
            yield return rotateTime;
            clockUI.RotateClock(RotationStep);
        }
    }
    
    private void EndTime()
    {
        isDay = false;
        Debug.Log("Time Ended!");
        warningTween.Kill();
        clockUI.face.DOColor(Color.white, warningPulseTime / 2).SetEase(Ease.Flash);
        // End day when nobody is inside
        CustomersManager.singleton.EndTime();
    }
}
