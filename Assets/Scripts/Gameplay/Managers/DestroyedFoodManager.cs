using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedFoodManager : MonoBehaviour
{
    private List<Transform> destroyedFoodTransforms = new List<Transform>();

    public static DestroyedFoodManager Singleton { get; private set; }
    private void Awake()
    {
        Singleton = this;
        GameManager.singleton.onWorkEnd.AddListener(StartCleaning);
    }

    public void AddNewDestroyedFood(Transform foodTransform)
    {
        destroyedFoodTransforms.Add(foodTransform);
    }

    private void StartCleaning()
    {
        if (destroyedFoodTransforms.Count == 0)
            EndCleaning();
    }

    private void EndCleaning()
    {
        GameManager.singleton.EndCleaning();
    }
}
