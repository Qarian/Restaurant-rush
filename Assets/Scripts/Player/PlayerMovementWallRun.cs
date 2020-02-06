﻿using UnityEngine;
using DG.Tweening;

public class PlayerMovementWallRun : MonoBehaviour
{
	[SerializeField] Transform groundChecker = default;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

	[Space]
	[SerializeField] float speed = 10f;
	[SerializeField] float maxSpeed = 10f;
	[SerializeField, Range(0, 1), Tooltip("0 - no vertical movement while wall running,\n1 - no effect")]
	float verticalSpeedModifierWallRunning = 0.85f;

	[SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpOfWallForce = 4f;
    [SerializeField] float wallJumpTime = 0.5f;

	[Header("Camera modification")]
	[SerializeField] Transform cameraTransform = default;
	[SerializeField] float maxCameraRotation = 30f;
	[SerializeField] float rotationSpeed = 0.5f;

	// Rotating camera during wall running
	float cameraRotationTarget = 0;
	float currentCameraRotation = 0;

	// Componets
	Rigidbody rb;
    Transform playerTransform;

	// Vectors relative to player
    Vector3 moveAxis = Vector3.zero;
    Vector3 position;
    Vector3 right;

	// Jumping
    bool onGround = false;
	bool canJump = true;

	// Properties for wall running
    enum Wall { Left = -1, No = 0, Right = 1}
    Wall status = Wall.No;
    Vector3 wallDir;
    Vector3 wallCheckDir;
	Collider wallRunningCollider;
    float leftWallTime = 0f;
	Vector3 lastVelocity;

	// Raycast properties
    const float wallRaycastLength = 0.8f;
	int raycastLayerMask;
	RaycastHit raycastHit;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
        playerTransform = transform;
		// set mask for raycasts to ignore player layer
		raycastLayerMask = 1 << gameObject.layer;
		raycastLayerMask = ~raycastLayerMask;
	}

	void Update()
	{
        position = playerTransform.position;
        right = playerTransform.right;
        // Get movement input
        moveAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		CheckGround();
        WallRunning();
        Jumping();

		// Rotate camera
		Vector3 cameraRotation = cameraTransform.localRotation.eulerAngles;
		currentCameraRotation = Mathf.Lerp(currentCameraRotation, cameraRotationTarget, Time.deltaTime * rotationSpeed);
		cameraRotation.z = currentCameraRotation;
		cameraTransform.localRotation = Quaternion.Euler(cameraRotation);
    }

	private void FixedUpdate()
	{
		if (status == Wall.No)
		{
			Vector3 accelaration = Vector3.zero;
			accelaration += moveAxis.z * playerTransform.forward;
			accelaration += moveAxis.x * playerTransform.right;
			accelaration = accelaration.normalized * speed * Time.fixedDeltaTime;
			rb.velocity = Vector3.ClampMagnitude(rb.velocity + accelaration, maxSpeed);
		}
		else
		{
			Vector3 velocity = rb.velocity;
			//if(velocity.y < 0)
				velocity.y *= verticalSpeedModifierWallRunning * Time.fixedDeltaTime;
			velocity += (wallDir * speed * Time.fixedDeltaTime);

			// Smooth going through acute angles
			float velocityChange = lastVelocity.magnitude / velocity.magnitude;
			if (velocityChange > 1f)
				// Something lower than velocityChange for big velocityChange
				velocity *= Mathf.Sqrt(velocityChange) * (1 + Mathf.Log(velocityChange));

			// TODO: maxSpeed should clamp only velocity on x and z
			rb.velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
		}
		lastVelocity = rb.velocity;
	}

	#region WallRunning
	private void WallRunning()
    {
		// Start Wall running
		if (status == Wall.No)
		{
			if (Input.GetKey(jumpKey) && !onGround && (Time.time - leftWallTime) >= wallJumpTime)
				CheckForWall();
			return;
		}

		// Jump off wall
		if (Input.GetKeyUp(jumpKey))
        {
            leftWallTime = Time.time;
            StopWallRunning();
            return;
        }

		// Check if wall ended
		bool rayFromWall = Physics.Raycast(position, wallCheckDir, out raycastHit, wallRaycastLength, raycastLayerMask);
		if (!rayFromWall)
			StopWallRunning();

		// Check if another wall is in front of player
		else if (RaycastToSide(status, out raycastHit))
		{
			if (wallRunningCollider != raycastHit.collider)
				SetWallRunning(status);
		}
    }

	private void CheckForWall()
	{
		bool rayHit = RaycastToSide(Wall.Left, out raycastHit);
		if (rayHit)
		{
			if (Vector3.Dot(raycastHit.normal, Vector3.up) < 0.001f)
				SetWallRunning(Wall.Left);
		}

		rayHit = RaycastToSide(Wall.Right, out raycastHit);
		if (rayHit)
		{
			if (Vector3.Dot(raycastHit.normal, Vector3.up) < 0.001f)
			{
				float rightDistance = Vector3.Distance(raycastHit.point, position);
				if (rightDistance < wallRaycastLength)
					SetWallRunning(Wall.Right);
			}
		}
	}

	private void SetWallRunning(Wall side)
	{
		wallRunningCollider = raycastHit.collider;
		status = side;
		wallCheckDir = -raycastHit.normal;
		if (side == Wall.Left)
			wallDir = Vector3.Cross(raycastHit.normal, Vector3.up);
		else
			wallDir = -1 * Vector3.Cross(raycastHit.normal, Vector3.up);
		rb.useGravity = false;
		canJump = false;

		// TODO: Set 
		cameraRotationTarget = maxCameraRotation * (int)side;
	}

	private bool RaycastToSide(Wall side, out RaycastHit raycastHit)
	{
		if (side == Wall.No)
			throw new System.Exception("Need to specify side for raycast in wallrunning");
		Vector3 dir = right;
		if (side == Wall.Left)
			dir *= -1;

		if (Physics.Raycast(position, dir, out raycastHit, wallRaycastLength, raycastLayerMask))
			return true;
		if (Physics.Raycast(position, Quaternion.Euler(0, -35, 0) * dir, out raycastHit, wallRaycastLength, raycastLayerMask))
			return true;
		return false;
	}

	private void StopWallRunning()
	{
		cameraRotationTarget = 0;

		rb.useGravity = true;
		status = Wall.No;
		canJump = true;
	}
	#endregion

	private void Jumping()
    {
        if (canJump && Input.GetKeyDown(jumpKey))
        {
            Jump(false);
            if ((Time.time - leftWallTime) < wallJumpTime)
                Jump(true);
        }
    }

    private void Jump(bool fromWall)
    {
        if (!onGround)
            canJump = false;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        if (fromWall)
            rb.AddForce(wallCheckDir * jumpOfWallForce * -1f, ForceMode.VelocityChange);
    }

	private void CheckGround()
	{
		bool hit = Physics.Raycast(groundChecker.position, Vector3.down, out raycastHit, 0.2f, raycastLayerMask);
		if (hit)
		{
			onGround = true;
			canJump = true;
		}
		else
			onGround = false;
	}
}
