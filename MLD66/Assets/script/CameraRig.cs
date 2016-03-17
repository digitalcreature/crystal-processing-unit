using UnityEngine;

public class CameraRig : MonoBehaviour {

	public float sensitivity = 5f;
	public MouseButton orbitButton = MouseButton.Right;
	public float zoomMin = 3;
	public float zoomMax = 15;
	public float zoomSensitivity = 1;
	public float zoomSmoothing = 10;

	new Camera camera;
	float zoom;

	void Awake() {
		camera = GetComponentInChildren<Camera>();
		zoom = - camera.transform.localPosition.z;
	}

	void Update() {
		if (Input.GetMouseButton((int) orbitButton)) {
			float x = Input.GetAxis("Mouse X") * sensitivity;
			float y = Input.GetAxis("Mouse Y") * sensitivity;
			transform.Rotate(-y, x, 0);
		}
		zoom -= Input.mouseScrollDelta.y * zoomSensitivity;
		zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
		Vector3 camPos = camera.transform.localPosition;
		camPos.z = Mathf.Lerp(camPos.z, - zoom, Time.deltaTime * zoomSmoothing);
		camera.transform.localPosition = camPos;
	}

}

public enum MouseButton { Left, Right, Middle }
