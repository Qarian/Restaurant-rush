using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class PlayerMovementWallRun : MonoBehaviour
{
	[SerializeField] private Transform groundChecker = default;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Space]
	public float accelaration = 10f;
	public float maxSpeed = 10f;
	[SerializeField, Range(0, 1), Tooltip("0 - no vertical movement while wall running,\n1 - no change")]
	private float verticalSpeedModifierWallRunning = 0.85f;

	[SerializeField] public float jumpForce = 10f;
    [SerializeField] private float jumpOfWallForce = 4f;
    [SerializeField] private float wallJumpTime = 0.5f;

	[Header("Camera modification")]
	[SerializeField] private Transform cameraTransform = default;
	[SerializeField] private new Camera camera = default;

	[SerializeField] private float maxCameraRotation = 30f;
	[SerializeField] private float rotationSpeed = 0.5f;

    public static PlayerMovementWallRun singleton;
    // Rotating camera during wall running
	private float cameraRotationTarget = 0;
	private float currentCameraRotation = 0;

	// Components
	private Rigidbody rb;
	private Transform playerTransform;

	// Vectors relative to player
	private Vector3 moveAxis = Vector3.zero;
	private Vector3 position;
	private Vector3 right;

	// Jumping
	private bool onGround = false;
	private Vector3 groundNormalVector;
	private bool canJump = true;

	// Properties for wall running
	private enum Wall { LeftWall = -1, NotAtWall = 0, RightWall = 1}
	private Wall status = Wall.NotAtWall;
	private Vector3 wallDir;
	private Vector3 wallCheckDir;
	private Collider wallRunningCollider;
	private float leftWallTime = 0f;
	private Vector3 lastVelocity;

	// Raycast properties
	private const float wallRaycastLength = 0.8f;
	private int raycastLayerMask;
	private RaycastHit raycastHit;
	
	private bool registerInput = true;


	private void Start()
	{
		camera = cameraTransform.GetComponent<Camera>();
		rb = GetComponent<Rigidbody>();
        playerTransform = transform;
		// set mask for raycasts to ignore player layer
		raycastLayerMask = 1 << gameObject.layer;
		raycastLayerMask = ~raycastLayerMask;
		
		GameManager.singleton.onInputToggle.AddListener(ChangeInput);

        singleton = this;
	}

	private void Update()
	{
		if (Pause.Paused) return;
		
		CheckInput();
	}
	
	private void FixedUpdate()
	{
		lastVelocity = rb.velocity = status == Wall.NotAtWall ? SetGroundVelocity() : SetWallVelocity(rb.velocity);
	}

	
	private void LateUpdate()
	{
		UpdateCamera();
	}

	private void UpdateCamera()
	{
		// Rotate camera
        		float offset = 0f;
        		Vector3 cameraRotation = cameraTransform.localRotation.eulerAngles;
        		currentCameraRotation = Mathf.Lerp(currentCameraRotation, cameraRotationTarget, Time.deltaTime * rotationSpeed);
        		if(status != Wall.NotAtWall && moveAxis != Vector3.zero)
        			 offset = Random.Range(-0.25f, 0.25f);
        		cameraRotation.z = currentCameraRotation+offset;
        		cameraTransform.localRotation = Quaternion.Euler(cameraRotation);
	}

	private void CheckInput()
	{
		position = playerTransform.position;
		right = playerTransform.right;
        
		if (registerInput)
		{
			moveAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			Jumping();
		}
		else
			moveAxis = Vector3.zero;
        
		CheckGround();
		WallRunning();
	}
	
	Vector3  SetWallVelocity(Vector3 speed)
	{
		speed.y *= verticalSpeedModifierWallRunning * Time.fixedDeltaTime;
		speed += (wallDir * accelaration * Time.fixedDeltaTime);

		// Smooth going through acute angles
		float velocityChange = lastVelocity.magnitude / speed.magnitude;
		if (velocityChange > 1f)
			// Something lower than velocityChange for big velocityChange
			speed *= Mathf.Sqrt(velocityChange) * (1 + Mathf.Log(velocityChange));

		// TODO: maxSpeed should clamp only velocity on x and z
		speed = Vector3.ClampMagnitude(speed, maxSpeed);
		return speed;
	}

	Vector3  SetGroundVelocity()
	{
		Vector3 speedGain = Vector3.zero;
		speedGain += moveAxis.z * playerTransform.forward;
		speedGain += moveAxis.x * playerTransform.right;
		speedGain = Vector3.ProjectOnPlane(speedGain.normalized, groundNormalVector);
		speedGain = speedGain.normalized * (accelaration * Time.fixedDeltaTime);
		speedGain = new Vector3(speedGain.x, speedGain.y * 2f, speedGain.z);
		return Vector3.ClampMagnitude(rb.velocity + speedGain, maxSpeed);
	}

	
	#region WallRunning
	private void WallRunning()
    {
		// Start Wall running
		if (status == Wall.NotAtWall)
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
		bool rayHit = RaycastToSide(Wall.LeftWall, out raycastHit);
		if (rayHit)
		{
			if (Vector3.Dot(raycastHit.normal, Vector3.up) < 0.001f)
				SetWallRunning(Wall.LeftWall);
		}

		rayHit = RaycastToSide(Wall.RightWall, out raycastHit);
		if (rayHit)
		{
			if (Vector3.Dot(raycastHit.normal, Vector3.up) < 0.001f)
			{
				float rightDistance = Vector3.Distance(raycastHit.point, position);
				if (rightDistance < wallRaycastLength)
					SetWallRunning(Wall.RightWall);
			}
		}
	}

	private void SetWallRunning(Wall side)
	{
		//ZoomCamera(20f);
		wallRunningCollider = raycastHit.collider;
		status = side;
		wallCheckDir = -raycastHit.normal;
		if (side == Wall.LeftWall)
			wallDir = Vector3.Cross(raycastHit.normal, Vector3.up);
		else
			wallDir = -1 * Vector3.Cross(raycastHit.normal, Vector3.up);
		rb.useGravity = false;
		canJump = false;

		// TODO: Set 
		cameraRotationTarget = maxCameraRotation * (int)side;
	}
	/*
	private void ZoomCamera(float zoom)
	{
		var FOV = camera.fieldOfView;
    	DOTween.To(x => camera.fieldOfView = x, FOV,FOV+zoom,1f );
	}*/

	private bool RaycastToSide(Wall side, out RaycastHit raycastHit)
	{
		if (side == Wall.NotAtWall)
			throw new System.Exception("Need to specify side for raycast in wallrunning");
		Vector3 dir = right;
		if (side == Wall.LeftWall)
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
		status = Wall.NotAtWall;
		//ZoomCamera(-20f);
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
		groundNormalVector = raycastHit.normal;
		if (hit)
		{
			onGround = true;
			canJump = true;
		}
		else
			onGround = false;
	}
	
	private void ChangeInput()
	{
		registerInput = !registerInput;
	}
}
