using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order
{
    private Color color;
    private Table table;
    private int id;

    // list of customers id
    private List<int> foodLeftToEat;

    public Order(Color color, int id, Table table)
    {
        this.table = table;
        this.color = color;
        this.id = id;
        foodLeftToEat = new List<int>(table.SittingCustomers.numberOfCustomers);
        for (int i = 0; i < table.SittingCustomers.numberOfCustomers; i++)
        {
            foodLeftToEat.Add(i);
        }

        table.OrderSphereInteractive.SetAction(PlaceOrder);
        ColorScript.SetColor(table.OrderSphereInteractive.gameObject, color);
    }
    
    public IEnumerator PreparingOrder()
    {
        yield return new WaitForSeconds(CustomersManager.singleton.customersFoodChoosingTime);
        
        table.OrderSphereInteractive.gameObject.SetActive(true);
    }

    // Place order, function to add to interactive object
    private void PlaceOrder()
    {
        table.OrderSphereInteractive.gameObject.SetActive(false);
        table.TableDetector.AssignOrder(id, FoodOnTable);

        GenerateFood();
    }

    private void FoodOnTable(FoodScript foodScript)
    {
        foodLeftToEat.Remove(foodScript.CustomerId);
        table.OrderGui.RemoveImage(foodScript.CustomerId);
        
        if (foodLeftToEat.Count <= 0)
        {
            OrdersManager.CompleteOrder(color);
            GameManager.singleton.RunCoroutine(CustomersManager.singleton.WaitForEating(table.SittingCustomers));
            table.TableDetector.gameObject.SetActive(false);
            table.OrderGui.HideIcons();
        }
        
        // TODO: Pooling for food
        Object.Destroy(foodScript.gameObject);
    }

    public void CancelOrder()
    {
        OrdersManager.CompleteOrder(color);
        table.TableDetector.gameObject.SetActive(false);
    }
    
    private void GenerateFood()
    {
        for (int i = 0; i < table.SittingCustomers.numberOfCustomers; i++)
        {
            FoodSO randomFood = OrdersManager.GetRandomFood();
            DrawFood(randomFood, i);
            SpawnFood(randomFood, i);
        }
    }


    private void DrawFood(FoodSO foodSO, int customerId)
    {
        table.OrderGui.ShowIcons();
        table.OrderGui.AddIcon(color, foodSO.icon, customerId);
    }

    private void SpawnFood(FoodSO foodSO, int customerId)
    {
        Food food = new Food();
        food.color = color;
        food.orderId = id;
        food.customerId = customerId;
        food.prefab = foodSO.prefab;
        FoodSpawner.Singleton.OrderFood(food);
    }
}
