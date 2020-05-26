using UnityEngine;

[RequireComponent(typeof(Interactive))]
public class DroppedFood : MonoBehaviour
{
    private Interactive interactive;
    private DroppedFoodManager droppedFoodManager;

    private void Start()
    {
        interactive = GetComponent<Interactive>();
        interactive.active = false;
    }

    public void Activate(DroppedFoodManager manager)
    {
        droppedFoodManager = manager;
        interactive.SetAction(CleanDroppedItem);
        interactive.active = true;
    }

    private void CleanDroppedItem()
    {
        droppedFoodManager.RemoveDroppedFood(this);
        Destroy(transform.parent.gameObject);
    }
}
