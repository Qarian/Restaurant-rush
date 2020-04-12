using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour
{
    public Image face;
    [SerializeField] Transform faceCenter = default;
    private Vector3 facePosition;
    [SerializeField] RectTransform clockHand = default;
    
    private void Awake()
    {
        if (faceCenter is null)
            Debug.LogError("no clock face image in Clock UI", gameObject);
        else
            facePosition = faceCenter.position;

        if (clockHand is null)
            Debug.LogError("no clock hand attached to Clock UI", gameObject);
    }

    public void RotateClock(float degree)
    {
        clockHand.RotateAround(facePosition, Vector3.forward, -degree);
    }
}