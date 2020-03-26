using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockUI : MonoBehaviour
{
    private Animator anim = default;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void CountDown(float dayDuration)
    {
        anim.SetFloat("speed", 1 / dayDuration);
    }
}
