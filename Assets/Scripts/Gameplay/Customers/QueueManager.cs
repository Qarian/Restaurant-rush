using System.Collections.Generic;

public static class QueueManager
{
    private static List<Queue> availableQueues = new List<Queue>();

    private static int lastUsedQueue = -1;

    public static bool SpawnNewCustomers()
    {
        if (availableQueues.Count <= 0)
            return false;

        lastUsedQueue = (lastUsedQueue + 1) % availableQueues.Count;
        availableQueues[lastUsedQueue].GenerateNewCluster();
        return true;
    }
    
    
    public static void AddQueue(Queue queue)
    {
        availableQueues.Add(queue);
    }

    public static void RemoveQueue(Queue queue)
    {
        availableQueues.Remove(queue);
    }
}
