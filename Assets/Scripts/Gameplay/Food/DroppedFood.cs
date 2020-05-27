using UnityEngine;

[RequireComponent(typeof(Interactive))]
public class DroppedFood : MonoBehaviour
{
    private Interactive interactive;

    private void Start()
    {
        interactive = GetComponent<Interactive>();
        interactive.active = false;
    }

    public void Activate()
    {
        interactive.SetAction(CleanDroppedItem);
        interactive.active = true;
    }

    private void CleanDroppedItem()
    {
        DroppedFoodManager.Singleton.RemoveDroppedFood(this);
        Destroy(transform.parent.gameObject);
    }
}
