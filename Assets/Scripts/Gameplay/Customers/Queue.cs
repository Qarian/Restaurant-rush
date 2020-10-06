using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Queue : MonoBehaviour
{
	[SerializeField] CustomersCluster clusterPrefab = default;
    [SerializeField] GameObject barrier = default;

    private Vector3[] queuePositions;

    private List<CustomersCluster> clusters = new List<CustomersCluster>();

    int maxWaitingClusters;
	int currentWaitingClusters = 0;

	private void OnEnable()
	{
		if (barrier) barrier.SetActive(false);
		QueueManager.AddQueue(this);
		if (!clusterPrefab)
			Debug.LogError("Queue don't have cluster prefab assigned", gameObject);
	}

	private void OnDisable()
	{
		QueueManager.RemoveQueue(this);
	}

	// Instantiate cluster of 1 - 4 customers 
	public void GenerateNewCluster()
	{
		// get queue positions
		if (queuePositions is null || queuePositions.Length == 0)// first time
		{
			maxWaitingClusters = transform.childCount;
			queuePositions = new Vector3[maxWaitingClusters];
			for (int i = 0; i < maxWaitingClusters; i++)
			{
				queuePositions[i] = transform.GetChild(i).position;
			}
		}
		
		CustomersCluster cluster = Instantiate(clusterPrefab, queuePositions[maxWaitingClusters-1], Quaternion.identity);
		cluster.Create(this, Random.Range(1, 5), currentWaitingClusters, queuePositions[currentWaitingClusters]);
		clusters.Add(cluster);
		currentWaitingClusters++;
		if (currentWaitingClusters == maxWaitingClusters)
			QueueManager.RemoveQueue(this);
	}

	// Move all clusters in queue 
	public void TakeCluster(CustomersCluster takenCustomer)
	{
		if (currentWaitingClusters == maxWaitingClusters)
			QueueManager.AddQueue(this);
        currentWaitingClusters--;
        clusters.Remove(takenCustomer);
        for (int i = 0; i < clusters.Count; i++)
		{
			clusters[i].MoveClusterInQueue(queuePositions[i]);
		}
	}

    public void CloseQueue()
    {
	    CustomersCluster[] clustersArray = clusters.ToArray();
        foreach (CustomersCluster cluster in clustersArray)
        {
	        cluster.LeaveRestaurant();
        }

        if (barrier) barrier.SetActive(true);
        queuePositions = null;
    }
}
