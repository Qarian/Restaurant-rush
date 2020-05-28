using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order
{
    private Color color;
    private Table table;
    private int id;

    // list of customers id
    private List<Food> foodLeftToEat;

    public Order(Color color, int id, Table table)
    {
        this.table = table;
        this.color = color;
        this.id = id;
        foodLeftToEat = new List<Food>(table.SittingCustomers.numberOfCustomers);

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
        foodLeftToEat.Remove(foodScript.Origin);
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
        table.OrderGui.HideIcons();
        for (int i = 0; i < foodLeftToEat.Count; i++)
        {
            FoodSpawner.Singleton.foodToRemove.Add(foodLeftToEat[i]);
        }
    }
    
    
    private void GenerateFood()
    {
        for (int i = 0; i < table.SittingCustomers.numberOfCustomers; i++)
        {
            FoodSO randomFoodSO = OrdersManager.GetRandomFood();
            Food food = MakeFoodObject(randomFoodSO, i);
            
            DrawFood(randomFoodSO, i);
            SpawnFood(food);
            foodLeftToEat.Add(food);
        }
    }
    
    private Food MakeFoodObject(FoodSO foodSO, int customerId)
    {
        Food food = new Food();
        food.color = color;
        food.orderId = id;
        food.customerId = customerId;
        food.prefab = foodSO.prefab;
        return food;
    }
    
    private void DrawFood(FoodSO foodSO, int customerId)
    {
        table.OrderGui.ShowIcons();
        table.OrderGui.AddIcon(color, foodSO.icon, customerId);
    }

    private void SpawnFood(Food food)
    {
        FoodSpawner.Singleton.OrderFood(food);
    }
}
