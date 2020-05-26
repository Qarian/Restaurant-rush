using System.Collections.Generic;
using UnityEngine;

public class DayTimeManager : MonoBehaviour
{
    [HideInInspector] public Material skyboxDay;
    [HideInInspector] public Material skyboxNight;
    
    public Light directionalLightDay = default;
    public Light directionalLightNight = default;

    [HideInInspector] public float skyboxIntensityDay;
    [HideInInspector] public float skyboxIntensityNight;

    public List<GameObject> enabledAtDay = new List<GameObject>();
    public List<GameObject> enabledAtNight = new List<GameObject>();

    private void Start()
    {
        StartDay();
        GameManager.singleton.onTimeEnd.AddListener(StartNight);
    }

    public void StartDay()
    {
        RenderSettings.skybox = skyboxDay;
        RenderSettings.ambientIntensity = skyboxIntensityDay;
        
        directionalLightDay.gameObject.SetActive(true);
        directionalLightNight.gameObject.SetActive(false);
        
        foreach (var obj in enabledAtDay)
        {
            obj.SetActive(true);
        }
        foreach (var obj in enabledAtNight)
        {
            obj.SetActive(false);
        }
    }

    public void StartNight()
    {
        RenderSettings.skybox = skyboxNight;
        RenderSettings.ambientIntensity = skyboxIntensityNight;
        
        directionalLightDay.gameObject.SetActive(false);
        directionalLightNight.gameObject.SetActive(true);
        
        foreach (var obj in enabledAtDay)
        {
            obj.SetActive(false);
        }
        foreach (var obj in enabledAtNight)
        {
            obj.SetActive(true);
        }
    }
}
