using UnityEngine;

public class CustomersCluster : MonoBehaviour
{
	[SerializeField] Customer customerPrefab = default;
	[SerializeField] Transform customersPositions = default;

	private Transform[] customersTransforms;
	private Customer[] customers;
	[HideInInspector] public int numberOfCustomers;
	private Queue queue;
	private int orderInQueue;

	private Table assignedTable;
	private int customersAtTable = 0;

	// Some number for start
	private float remainingPatienceTime;
	private bool reducePatience = false;
	private float maxPatienceTime;

	// Instantiate new cluster of customers
	public void Create(Queue queue, int numberOfCustomers, int order, Vector3 targetPos)
	{
		this.queue = queue;
		// clear values (in case of pooling)
		customersAtTable = 0;
		remainingPatienceTime = CustomersManager.singleton.queuePatienceTime;
		maxPatienceTime = remainingPatienceTime/2f + CustomersManager.singleton.tablePatienceTime;
		reducePatience = true;
		
		orderInQueue = order;

		// Get positions for customers
		this.numberOfCustomers = numberOfCustomers;
		Debug.Log("Created " + numberOfCustomers + " new customers");
		customersTransforms = new Transform[numberOfCustomers];
		Transform tmp = customersPositions.GetChild(numberOfCustomers - 1);
		for (int i = 0; i < numberOfCustomers; i++)
		{
			customersTransforms[i] = tmp.GetChild(i);
		}

		// Create customers
		customers = new Customer[numberOfCustomers];
		for (int i = 0; i < numberOfCustomers; i++)
		{
			customers[i] = InstantiateCustomer(customersTransforms[i].position);
			customers[i].Cluster = this;
		}

		// Move Customers to their place in queue
		transform.position = targetPos;
		for (int i = 0; i < numberOfCustomers; i++)
		{
			customers[i].SetDestination(customersTransforms[i]);
		}
	}

	private void Update()
	{
		if (!reducePatience)
			return;
		
		remainingPatienceTime -= Time.deltaTime;
		if (remainingPatienceTime <= 0f)
		{
			// Stop counting
			reducePatience = false;
			LeaveRestaurant();
		}
	}

	// Move cluster to next position in queue
	public void MoveClusterInQueue(Vector3 targetPos)
	{
		orderInQueue--;
		if (orderInQueue >= 0)
		{
			transform.position = targetPos;
			for (int i = 0; i < numberOfCustomers; i++)
			{
				customers[i].SetDestination(customersTransforms[i]);
			}
		}
	}

	// disable interactive component on all customers
	public void SelectCustomer()
	{
		reducePatience = false;

		if (CustomersManager.selectedCustomers == null)
		{
            CustomersManager.singleton.SelectCustomer(this);
			for (int i = 0; i < numberOfCustomers; i++)
			{
				customers[i].Disable();
			}
		}
	}


	// Assign every customer their place at table
	public void AssignToTable(Table table)
	{
		orderInQueue = -1;
		assignedTable = table;

		for (int i = 0; i < numberOfCustomers; i++)
		{
			customers[i].GoToTable(table.chairPositions.GetChild(i));
		}
		
		queue.TakeCluster(this);
		queue = null;
	}

	public void CustomerArrivedAtTable()
	{
		customersAtTable++;
		// If everyone came to table, wait for order
		if (customersAtTable == numberOfCustomers)
		{
			reducePatience = true;
			remainingPatienceTime /= 2f;
			remainingPatienceTime += CustomersManager.singleton.tablePatienceTime;
			
			StartCoroutine(assignedTable.currentOrder.PreparingOrder());
		}
	}

	public void CustomersAteFood()
	{
		LeaveRestaurant();
		GameManager.singleton.PointsManager.AddPoints(numberOfCustomers, remainingPatienceTime/maxPatienceTime);
	}


    public void LeaveRestaurant()
    {
	    // In Queue
	    queue?.TakeCluster(this);
	    queue = null;
		
		// At table
		if (assignedTable)
		{
			assignedTable.ResetTable();
			assignedTable = null;
		}

		for (int i = 0; i < numberOfCustomers; i++)
        {
			customers[i].Disable();
			customers[i].SetDestination(CustomersManager.singleton.exit, DeleteCustomers);
        }
    }

    private void DeleteCustomers()
    {
        for (int i = 0; i < numberOfCustomers; i++)
        {
            Destroy(customers[i].gameObject);
        }
		CustomersManager.singleton.RemoveCluster();
		Destroy(gameObject);
    }

    private Customer InstantiateCustomer(Vector3 position)
    {
        //TODO: Pooling for customers
        return Instantiate(customerPrefab, position, Quaternion.identity, null);
    }
}
