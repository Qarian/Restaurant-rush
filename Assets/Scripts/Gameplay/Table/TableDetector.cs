using System;
using System.Collections.Generic;
using UnityEngine;

public class TableDetector : MonoBehaviour
{
	private int orderId;
	private Action<FoodScript> onFoodOnTable;
	
	// References to food components
	private List<FoodScript> targetFoodScripts = new List<FoodScript>();

	private void Update()
	{
		// if food is on table, check if it isn't moving
		foreach (FoodScript food in targetFoodScripts)
		{
			if (food.RigidbodyComponent.velocity != Vector3.zero) continue;
			targetFoodScripts.Remove(food);
			onFoodOnTable(food);
			// consume max 1 item per frame
			break;
		}
	}

	// Check if food entered trigger
	private void OnTriggerEnter(Collider collider)
	{
		FoodScript target = collider.GetComponent<FoodScript>();
		if (target && target.OrderId == orderId)
			targetFoodScripts.Add(target);
	}

	// Check if food exit trigger
	private void OnTriggerExit(Collider other)
	{
		FoodScript removedFood = other.GetComponent<FoodScript>();
		if (removedFood && targetFoodScripts.Contains(removedFood))
			targetFoodScripts.Remove(removedFood);
	}

	public void AssignOrder(int id, Action<FoodScript> action)
	{
		onFoodOnTable = action;
		orderId = id;
		gameObject.SetActive(true);
	}
}
