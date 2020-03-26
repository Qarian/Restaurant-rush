using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour
{
    public Image face;
    private Vector3 facePosition;
    [SerializeField] RectTransform clockHand = default;

    private Animator anim = default;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        
        if (face is null)
            Debug.LogError("no clock face image in Clock UI");
        else
            facePosition = face.transform.position;

        if (clockHand is null)
            Debug.LogError("no clock hand attached to Clock UI");
    }

    public void RotateClock(float degree)
    {
        clockHand.RotateAround(facePosition, Vector3.forward, -degree);
    }
}