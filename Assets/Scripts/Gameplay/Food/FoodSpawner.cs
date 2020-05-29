using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [HideInInspector]
    public List<Transform> spawnPoints;
    private List<Transform> freeSpawnPoints;

    private Queue<Food> foodQueue = new Queue<Food>();
    public readonly List<Food> foodToRemove = new List<Food>();
    
    public static FoodSpawner Singleton;

    private void Start()
    {
        if (spawnPoints.Count == 0)
            Debug.LogError("No food spawning points!", gameObject);
        
        freeSpawnPoints = spawnPoints;
        
        Singleton = this;
    }

    private void Update()
    {
        while (foodQueue.Count > 0)
        {
            if (freeSpawnPoints.Count == 0)
                return;
            
            Food foodToPrepare = foodQueue.Dequeue();
            if (foodToRemove.Contains(foodToPrepare))
                foodToRemove.Remove(foodToPrepare);
            else
                StartCoroutine(SpawnFood(foodToPrepare, freeSpawnPoints[0]));
        }
    }

    public void OrderFood(Food food)
    {
        foodQueue.Enqueue(food);
    }

    private IEnumerator SpawnFood(Food food, Transform spawnPoint)
    {
        freeSpawnPoints.Remove(spawnPoint);
        yield return new WaitForSeconds(CustomersManager.singleton.foodSpawnTime);

        if (foodToRemove.Contains(food))
        {
            foodToRemove.Remove(food);
            freeSpawnPoints.Add(spawnPoint);
        }
        else
        {
            GameObject go = Instantiate(food.prefab, spawnPoint.position, Quaternion.identity);
            go.transform.SetParent(spawnPoint);
            go.GetComponent<FoodScript>().Init(food, spawnPoint);
        }
    }

    public void FreeSpawnPoint(Transform spawnPoint)
    {
        if (freeSpawnPoints.Contains(spawnPoint))
            Debug.LogWarning("Spawn point is already free!", spawnPoint);
        else
            freeSpawnPoints.Add(spawnPoint);
    }
    
    public void SetSpawnPoints(List<Transform> newSpawnPoints) => spawnPoints = newSpawnPoints;
}
