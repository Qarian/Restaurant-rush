using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomersManager : MonoBehaviour
{
    [Header("References")]
    public Transform exit = default;
    [SerializeField] Queue queue = default;

    [Header("Times (seconds)")]
    [SerializeField] float newCustomerTime = 8f;
    public float foodSpawnTime = 10f;
    public float customersFoodChoosingTime = 4f;
    public float customersEatingTime = 5f;

    [Header("Patience (seconds)")]
    public float queuePatienceTime = 25f;
    public float tablePatienceTime = 40f;

    [Header("Points")]
    public int pointsFromTable = 10;

    public static CustomersCluster selectedCustomers;

    // Assigned in SceneGameplayWindow
    public List<Table> freeTables = new List<Table>();

    private bool spawningCustomers = true;
    int acceptedCustomers = 0;

	#region singleton
	public static CustomersManager singleton;
    public void Awake()
    {
        singleton = this;
    }
	#endregion

	private void Start()
    {
        if (freeTables.Count == 0)
        {
            Debug.LogError("No tables assigned!");
            return;
        }

        StartCoroutine(SpawnCustomers());
        GameManager.singleton.onTimeEnd.AddListener(EndTime);
    }

    private IEnumerator SpawnCustomers()
    {
        while (spawningCustomers)
        {
            queue.GenerateNewCluster();
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
        // Increase number of custer clusters inside building
        acceptedCustomers++;
        TakeClusterFromQueue(selectedCustomers);
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
        acceptedCustomers--;
        if (!spawningCustomers && acceptedCustomers == 0)
            GameManager.singleton.EndWork();
    }

	public void EndTime()
	{
		queue.CloseQueue();
        spawningCustomers = false;
        if (acceptedCustomers == 0)
            GameManager.singleton.EndWork();
	}

    public void TakeClusterFromQueue(CustomersCluster customersCluster)
    {
        queue.TakeCluster(customersCluster);
    }
}
