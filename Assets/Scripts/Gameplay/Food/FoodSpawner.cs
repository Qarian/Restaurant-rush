using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    private List<Transform> spawnPoints = new List<Transform>();

    public static FoodSpawner Singleton;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
        }
        
        if (spawnPoints.Count == 0)
            Debug.LogError("No food spawning points!", gameObject);

        Singleton = this;
    }

    public void OrderFood(Food food)
    {
        //TODO: implement Queue
        StartCoroutine(SpawnFood(food));
    }

    private IEnumerator SpawnFood(Food food)
    {
        yield return new WaitForSeconds(CustomersManager.singleton.foodSpawnTime);
        // waiting for spawn point to be free
        WaitForSeconds waiting = new WaitForSeconds(1);
        while (true)
        {
            // TODO: check for empty places somewhere else
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                if (spawnPoints[i].childCount == 0)
                {
                    GameObject go = Instantiate(food.prefab, spawnPoints[i].position + new Vector3(0, i * 0.5f, 0), Quaternion.identity);
                    go.transform.SetParent(spawnPoints[i]);
                    go.GetComponent<FoodScript>().Init(food.color, food.orderId, food.customerId);
                    yield break;
                }
            }
            yield return waiting;
        }
    }
}
