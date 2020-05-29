using UnityEngine;

[RequireComponent(typeof(Interactive))]
public class DroppedFood : MonoBehaviour
{
    private Interactive interactive;

    private void Start()
    {
        if (interactive == null)
        {
            interactive = GetComponent<Interactive>();
            interactive.active = false;
        }
    }

    public void Activate()
    {
        if (interactive == null)
            interactive = GetComponent<Interactive>();
        interactive.SetAction(CleanDroppedItem);
        interactive.active = true;
    }

    private void CleanDroppedItem()
    {
        DroppedFoodManager.Singleton.RemoveDroppedFood(this);
        Destroy(transform.parent.gameObject);
    }
}
