using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class setPowerSliderValue : MonoBehaviour
{
    private PlayerFoodHolding playerFoodHolding;

    public Color lowColor;
    public Color HighColor;
    private Image slider;
    // Start is called before the first frame update
    void Start()
    {
        playerFoodHolding = Camera.main.GetComponent<PlayerFoodHolding>();
        slider = transform.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerFoodHolding.IsThrowing)
        {
            slider.fillAmount = playerFoodHolding.ThrowKeyHoldingTime;
            slider.color = Color.Lerp(lowColor, HighColor, playerFoodHolding.ThrowKeyHoldingTime);
        }
        else
        {
            slider.fillAmount = 0;
        }
      
    }
}
