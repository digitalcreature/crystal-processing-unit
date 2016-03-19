using UnityEngine;

public class Building : MonoBehaviour {

	State _state;
	public State state {
		get {
			return _state;
		}
		set {
			_state = value;
		}
	}

	Material material;
	new Renderer renderer;
	new Collider collider;
	static float angle;

	void Awake() {
		renderer = GetComponent<Renderer>();
		if (renderer != null) {
			material = renderer.material;
		}
		collider = GetComponent<Collider>();
	}

	public enum State { Placing, Building, Active }

	void Update() {
		Builder builder = Builder.main;
		switch (state) {
			//building is being placed by player
			case State.Placing:
				SetChildrenActive(false);
				builder.busy = true;
				collider.enabled = false;
				RaycastHit hit;
				Ray ray = CameraRig.main.camera.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, builder.groundLayers)) {
					renderer.enabled = true;
					transform.position = hit.point;
					transform.forward = hit.normal;
					angle += Input.GetAxis("Rotate Building") * Time.deltaTime * builder.rotateSpeed;
					Mathf.Repeat(angle, 360);
					transform.Rotate(0, 0, angle);
				}
				else {
					//hide the building if the player isnt pointing at ground
					renderer.enabled = false;
				}
				bool valid = PositionValid(transform.position);
				renderer.material = valid ? builder.validPlacingMaterial : builder.invalidPlacingMaterial;
				if (Input.GetMouseButtonDown((int) MouseButton.Left) && valid && renderer.enabled) {
					state = State.Active;//Building;
					builder.busy = false;
				}
				if (Input.GetMouseButtonDown((int) MouseButton.Right)) {
					Destroy(gameObject);
					builder.busy = false;
				}
				break;
			//building is constructing or deconstructing
			case State.Building:
				SetChildrenActive(false);
				collider.enabled = true;
				renderer.enabled = true;
				renderer.material = material;
				break;
			//building is finished
			case State.Active:
				SetChildrenActive(true);
				collider.enabled = true;
				renderer.enabled = true;
				renderer.material = material;
				break;
		}
	}

	public virtual bool PositionValid(Vector3 position) {
		return PositionUnobstucted(position) && PositionNearExistingBuilding(position);
	}

	public bool PositionUnobstucted(Vector3 position) {
		Builder builder = Builder.main;
		return !Physics.CheckSphere(position, builder.obstacleRadius, builder.obstacleLayers);
	}

	public bool PositionNearExistingBuilding(Vector3 position) {
		Builder builder = Builder.main;
		return Physics.CheckSphere(position, builder.maxBuildingDistance, builder.buildingLayers);
	}

	void SetChildrenActive(bool active) {
		for (int c = 0; c < transform.childCount; c ++) {
			transform.GetChild(c).gameObject.SetActive(active);
		}
	}
}
