using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CameraSelector))]
public class PlayerFoodHolding : MonoBehaviour
{
    private struct HeldFood
    {
        public Transform Transform;
        public Rigidbody Rb;
        public Transform MeshTransform;
    }
    
    [SerializeField] private Rigidbody playerRigidbody = default;
    [SerializeField] private List<Transform> holdingPoints = default;

    [Space, SerializeField] private KeyCode grabKey = KeyCode.Mouse0;
    [SerializeField] private float foodGrabbingSpeed = 5f;
    
    [Space, SerializeField] private KeyCode throwKey = KeyCode.Mouse1;
    [SerializeField] private float timeToThrowFood = 0.5f;
    public float ThrowKeyHoldingTime { get; private set; } = 0;
    [SerializeField] private Vector2 dropVelocity = default;
    [SerializeField] private Vector2 throwVelocity = default;
    public bool IsThrowing { get; private set; } = false;
    
    private readonly List<HeldFood> heldFoods = new List<HeldFood>();

    private CameraSelector cameraSelector = default;

    private void Start()
    {
        cameraSelector = GetComponent<CameraSelector>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(grabKey))
            GrabFood();

        IsThrowing = Input.GetKey(throwKey);
        if ( IsThrowing)
            ThrowKeyHoldingTime += Time.deltaTime;

        if (Input.GetKeyUp(throwKey))
            DropFood(ThrowKeyHoldingTime >= timeToThrowFood);
    }

    private void GrabFood()
    {
        // max number of holding items
        if (holdingPoints.Count <= heldFoods.Count)
            return;

        // check if hit anything
        if (!cameraSelector.Raycast(out var hit))
            return;

        Transform hitTransform = hit.transform;
        FoodScript foodScript = hitTransform.GetComponent<FoodScript>();
        // Check if selected food
        if (!foodScript)
            return;

        foodScript.TakeFood();
        HeldFood food;
        food.Transform = hitTransform;
        food.Rb = hit.rigidbody;
        food.MeshTransform = foodScript.meshTransform;

        int index = heldFoods.Count;
        heldFoods.Add(food);
        
        // Set parents
        hitTransform.SetParent(null);
        food.MeshTransform.SetParent(holdingPoints[index]);
        // Turn off rigidbody on main object
        hitTransform.gameObject.SetActive(false);
        // Move food to holdingPlace
        food.MeshTransform.DOLocalMove(Vector3.zero, foodGrabbingSpeed);
        food.MeshTransform.DOLocalRotate(Vector3.zero, foodGrabbingSpeed);
    }

    private void DropFood(bool throwFood)
    {
        if (heldFoods.Count == 0)
            return;

        HeldFood foodToDrop = heldFoods[heldFoods.Count - 1];
        heldFoods.RemoveAt(heldFoods.Count - 1);

        foodToDrop.Transform.position = foodToDrop.MeshTransform.position;
        foodToDrop.Transform.rotation = foodToDrop.MeshTransform.rotation;
        foodToDrop.Transform.gameObject.SetActive(true);
        foodToDrop.MeshTransform.SetParent(foodToDrop.Transform);
        foodToDrop.Rb.velocity = playerRigidbody.velocity +
                                 (throwFood ? throwVelocity.y : dropVelocity.y) * foodToDrop.Transform.up +
                                 (throwFood ? throwVelocity.x : dropVelocity.x) * foodToDrop.Transform.forward;
                                 
        ThrowKeyHoldingTime = 0;
    }
}
