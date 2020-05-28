using System.Collections.Generic;
using UnityEngine;

public class OrdersManager : MonoBehaviour
{
    [SerializeField] private OrdersColorSchemeSO colorScheme = default;
    [SerializeField] private FoodListSO foodList = default;
    public static FoodListSO FoodList { get; private set; }

    private static List<Color> colors;

    private static int lastOrderId = 0;

    private void Start()
    {
        colors = new List<Color>(colorScheme.colors.ToArray());
        FoodList = foodList;
    }

    public static Order NewOrder(Table table)
    {
        lastOrderId++;

        Color color;
        if (colors.Count > 0)
        {
            color = colors[0];
            colors.RemoveAt(0);
        }
        else
        {
            Debug.LogError("No colors left for order nr " + lastOrderId + "!");
            color = Color.black;
        }

        return new Order(color, lastOrderId, table);
    }

    public static void CompleteOrder(Color color)
    {
        colors.Add(color);
    }

    public static FoodSO GetRandomFood()
    {
        int randomId = Random.Range(0, FoodList.list.Count);
        return FoodList[randomId];
    }
}
