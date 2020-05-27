using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedFoodManager : MonoBehaviour
{
    [SerializeField] private CleaningUI ui = default;
    [SerializeField] private GameObject droppedFoodColliderPrefab = default;
    private List<DroppedFood> droppedFoods = new List<DroppedFood>();
    private bool cleaningTime = false;

    public static DroppedFoodManager Singleton { get; private set; }
    
    private void Start()
    {
        Singleton = this;
        GameManager.singleton.onWorkEnd.AddListener(StartCleaning);
    }

    public void AddNewDestroyedFood(Transform foodTransform)
    {
        var created = Instantiate(droppedFoodColliderPrefab, foodTransform.position, Quaternion.identity, foodTransform);
        var component = created.GetComponent<DroppedFood>();
        droppedFoods.Add(component);
        if (cleaningTime)
        {
            component.Activate();
            ui.UpdateFilthCount(droppedFoods.Count);
        }
    }

    public void RemoveDroppedFood(DroppedFood droppedFood)
    {
        droppedFoods.Remove(droppedFood);
        ui.UpdateFilthCount(droppedFoods.Count);
        if (droppedFoods.Count == 0)
            EndCleaning();
    }

    private void StartCleaning()
    {
        cleaningTime = true;
        ui.ShowUI(droppedFoods.Count);
        //ui.gameObject.SetActive(true);
        foreach (var dropped in droppedFoods)
        {
            dropped.Activate();
        }
        if (droppedFoods.Count == 0)
            EndCleaning();
    }

    private void EndCleaning()
    {
        GameManager.singleton.EndCleaning();
    }
}
