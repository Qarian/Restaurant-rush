using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour
{
    public Image face;
    private Vector3 facePosition;
    [SerializeField] RectTransform clockHand = default;
    
    private void Awake()
    {
        if (face is null)
            Debug.LogError("no clock face image in Clock UI", gameObject);
        else
        {
            Vector3 position = face.transform.position;
            facePosition = new Vector3(position.x, position.y + (position.y - transform.position.y)/ 2);
        }

        if (clockHand is null)
            Debug.LogError("no clock hand attached to Clock UI", gameObject);
    }

    public void RotateClock(float degree)
    {
        clockHand.RotateAround(facePosition, Vector3.forward, -degree);
    }
}