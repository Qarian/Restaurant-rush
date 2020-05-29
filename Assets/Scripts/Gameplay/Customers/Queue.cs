using UnityEngine;
using System.Collections.Generic;

public class Queue : MonoBehaviour
{
	[SerializeField] GameObject customerCluster = default;
    [SerializeField] GameObject barrier = default;
	
	public static Vector3[] queuePositions;

    private List<CustomersCluster> clusters = new List<CustomersCluster>();

    int maxWaitingClusters;
	int currentWaitingClusters = 0;

	private void Start()
	{
		barrier.SetActive(false);
	}

	// Instantiate cluster of 1 - 4 customers 
	public bool GenerateNewCluster()
	{
		// get queue positions
		if (queuePositions == null)// first time
		{
			maxWaitingClusters = transform.childCount;
			queuePositions = new Vector3[maxWaitingClusters];
			for (int i = 0; i < maxWaitingClusters; i++)
			{
				queuePositions[i] = transform.GetChild(i).position;
			}
		}

		if (currentWaitingClusters == maxWaitingClusters)
            return false;

		CustomersCluster cluster = Instantiate(customerCluster, queuePositions[maxWaitingClusters-1], Quaternion.identity).GetComponent<CustomersCluster>();
		cluster.Create(Random.Range(1, 5), currentWaitingClusters);
		clusters.Add(cluster);
		currentWaitingClusters++;
        return true;
	}

	// Move all clusters in queue 
	public void TakeCluster(CustomersCluster takenCustomer)
	{
        currentWaitingClusters--;
		int index = clusters.IndexOf(takenCustomer);
		for (int i = index + 1; i < clusters.Count; i++)
		{
			clusters[i].MoveClusterInQueue();
		}
		clusters.Remove(takenCustomer);
	}

    public void CloseQueue()
    {
	    CustomersCluster[] clustersArray = clusters.ToArray();
        foreach (CustomersCluster cluster in clustersArray)
        {
	        cluster.LeaveRestaurant();
        }
        barrier.SetActive(true);
        queuePositions = null;
    }
}
