﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CustomersManager : MonoBehaviour
{
    [Header("References")]
    public Transform exit = default;

    [Header("Times (seconds)")]
    [SerializeField] float newCustomerTime = 8f;
    public float foodSpawnTime = 10f;
    public float customersFoodChoosingTime = 4f;
    public float customersEatingTime = 5f;

    [Header("Patience (seconds)")]
    public float queuePatienceTime = 25f;
    public float tablePatienceTime = 40f;

    public static CustomersCluster selectedCustomers;

    // Assigned in SceneGameplayWindow
    public List<Table> freeTables = new List<Table>();

    private bool spawningCustomers;
    int customersInside;

	#region singleton
	public static CustomersManager singleton;
    public void Awake()
    {
        singleton = this;
    }
	#endregion

    private void Start()
    {
        selectedCustomers = null;
        if (freeTables.Count == 0)
        {
            Debug.LogError("No tables assigned!");
            return;
        }

        spawningCustomers = true;
        customersInside = 0;
        Debug.Log("Spawn customers");
        StartCoroutine(SpawnCustomers());
        GameManager.singleton.onTimeEnd.AddListener(EndTime);
    }

    private IEnumerator SpawnCustomers()
    {
        while (spawningCustomers)
        {
            if (QueueManager.SpawnNewCustomers())
                customersInside++;
            yield return new WaitForSeconds(newCustomerTime);
        }
    }

    // Player select customers
    public void SelectCustomer(CustomersCluster customersCluster)
    {
        selectedCustomers = customersCluster;
        foreach (var table in freeTables)
        {
            table.Enable();
        }
    }

    // Player choose table for customers
    public CustomersCluster ChooseTable(Table table)
    {
        // make free tables not selectable
        foreach (Table freeTable in freeTables)
        {
            freeTable.Disable();
        }
        CustomersCluster ret = selectedCustomers;
        selectedCustomers.AssignToTable(table);
        freeTables.Remove(table);
        // clear selection
        selectedCustomers = null;
        return ret;
    }

    public IEnumerator WaitForEating(CustomersCluster customersCluster)
    {
        yield return new WaitForSeconds(customersEatingTime);
        if (customersCluster is null)
            Debug.LogError("Gave food to table without customers");
        else
        {
            customersCluster.CustomersAteFood();
            Debug.Log("customers out");
        }
            
    }

    public void FreeTable(Table table)
    {
        freeTables.Add(table);
        // Check if another customer is selected
        if (selectedCustomers)
        {
            table.Enable();
        }
    }

    public void RemoveCluster()
    {
        customersInside--;
        if (!spawningCustomers && customersInside <= 0)
            GameManager.singleton.EndWork();
    }

	public void EndTime()
	{
		QueueManager.CloseQueues();
        spawningCustomers = false;
        if (customersInside == 0)
            GameManager.singleton.EndWork();
	}
}
