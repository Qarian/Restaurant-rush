using UnityEngine;

public class CameraSelector : MonoBehaviour
{
	[SerializeField] private RectTransform crosshair = default;

	[SerializeField, Space] private float maxDistance = 20f;

	[SerializeField, Space]
	private Vector2 crosshairSize = new Vector2(10f, 50f);
	[SerializeField]
	private float crosshairChangeSpeed = 5f;
	private float size;

	new Camera camera;

	Interactive currentInteractive;

	private void Start()
	{
		camera = GetComponent<Camera>();
		size = crosshair.sizeDelta.x;
		crosshair.gameObject.SetActive(true);
	}

	private void Update()
	{
		// Has hit anything
		if (Raycast(out var hit))
		{
			currentInteractive = hit.transform.GetComponent<Interactive>();
			// Hit something with Interactive component
			if (currentInteractive && currentInteractive.active)
			{
				if (Input.GetKeyDown(KeyCode.E) && !Pause.Paused)
					currentInteractive.Interaction();
				size += 2 * crosshairChangeSpeed * Time.deltaTime;
			}
		}
		size -= crosshairChangeSpeed * Time.deltaTime;

		// Change size of crosshair
		size = Mathf.Clamp(size, crosshairSize.x, crosshairSize.y);
		crosshair.sizeDelta = new Vector2(size, size);
	}

	public bool Raycast(out RaycastHit hit)
	{
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		return Physics.Raycast(ray, out hit, maxDistance);
	}
}
