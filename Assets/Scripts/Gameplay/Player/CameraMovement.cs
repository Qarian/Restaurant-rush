using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	[SerializeField]
    Transform player = default;
	
	[SerializeField]
	float cameraSpeed = default;
	[SerializeField]
	Vector2 cameraYbounds = default;
	
	
	bool mouseVisible = true;
	float rotationY;
	
	//Hide mouse on start
	void Start() 
	{
		mouseVisible = Cursor.visible;
		if (mouseVisible)
			ChangeMouseMode();
		
		GameManager.singleton.onCleaningEnd.AddListener(OnDayEnd);
		GameManager.singleton.onInputToggle.AddListener(ChangeMouseMode);
	}
	
	void Update()
	{
		// move camera when mouse is invisible
		if (!mouseVisible) {
			float yAngle = cameraSpeed * Input.GetAxis("Mouse Y");
			float xAngle = cameraSpeed * Input.GetAxis("Mouse X");
		
			player.Rotate(0, xAngle, 0);
			
			rotationY = Mathf.Clamp(rotationY + yAngle, cameraYbounds.x, cameraYbounds.y);
			Vector3 currentRotation = transform.localEulerAngles;
			transform.localEulerAngles = new Vector3(-rotationY, currentRotation.y, currentRotation.z);
		}
		
		if (Input.GetKeyDown(KeyCode.L))
			ChangeMouseMode();
	}

	// Toggle mouse visibility
	private void ChangeMouseMode()
	{
		if (mouseVisible)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else 
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		mouseVisible = !mouseVisible;
	}

	private void OnDayEnd()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		mouseVisible = true;
	}
}
