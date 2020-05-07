using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FoodScript : Interactive
{
	[SerializeField] private Renderer meshRenderer = default;
	public Transform meshTransform;
	public Rigidbody RigidbodyComponent { get; private set; }
	
	private Food origin;
	private Transform foodSpawnPoint;

	public int OrderId => origin.orderId;
	public int CustomerId => origin.customerId;

	private void Awake()
	{
		RigidbodyComponent = GetComponent<Rigidbody>();
		// No interaction with food - only for modifying cursor
		SetAction(() => { });
	}
	
	public void Init(Food template, Transform spawnPoint)
	{
		ColorScript.SetColor(meshRenderer, template.color);
		origin = template;
		foodSpawnPoint = spawnPoint;
	}

	public void TakeFood()
	{
		if (foodSpawnPoint)
		{
			FoodSpawner.Singleton.FreeSpawnPoint(foodSpawnPoint);
			foodSpawnPoint = null;
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		GameObject otherGO = other.gameObject;
		Debug.Log("food collision", otherGO);
		Table table = otherGO.GetComponent<Table>();
		if (!table)
		{
			if (otherGO.CompareTag("Environment"))
				DestroyFood();
		}
	}

	private void DestroyFood()
	{
		// TODO: make destroyed food look different
		meshTransform.SetParent(null);
		FoodSpawner.Singleton.OrderFood(origin);
		DestroyedFoodManager.Singleton.AddNewDestroyedFood(meshTransform);
		// If by accident food fall down before player takes it from its place
		TakeFood();
		// TODO: make more dynamic food destruction
		Destroy(gameObject);
	}
}
