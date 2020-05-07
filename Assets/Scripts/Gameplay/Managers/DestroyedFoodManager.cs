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
    }

    public void AddNewDestroyedFood(Transform foodTransform)
    {
        destroyedFoodTransforms.Add(foodTransform);
    }
}
