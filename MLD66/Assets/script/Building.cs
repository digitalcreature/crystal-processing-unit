using UnityEngine;
using System.Collections.Generic;

public class Building : MonoBehaviour {

	public int maxCount = -1;
	public float buildTime = 2;

	public State state { get; private set; }
	public Builder builder { get; private set; }
	Building _prefab;
	public Building prefab {
		get {
			return _prefab == null ? this : _prefab;
		}
		private set {
			_prefab = value;
		}
	}

	Material material;
	new Renderer renderer;
	new Collider collider;
	static float angle;
	float buildProgress = 0;
	float buildSpeed = 0;

	static Dictionary<Building, int> _counts; //maps building prefabs to count of existing buildings
	static Dictionary<Building, int> counts {
		get {
			if (_counts == null) {
				_counts = new Dictionary<Building, int>();
			}
			return _counts;
		}
	}

	public int count {
		get {
			return counts.ContainsKey(prefab) ? counts[prefab] : 0;
		}
		private set {
			counts[prefab] = value;
		}
	}

	void Awake() {
		renderer = GetComponent<Renderer>();
		if (renderer != null) {
			material = renderer.material;
		}
		collider = GetComponent<Collider>();
	}

	public enum State { Placing, Building, Active }

	public void Initialize(Builder builder, Building prefab) {
		this.builder = builder;
		this.prefab = prefab;
		state = State.Placing;
	}

	void Update() {
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
					state = State.Building;
					builder.busy = false;
					buildSpeed = 1;
					count ++;
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
				renderer.material = builder.inProgressMaterial;
				buildProgress += buildSpeed * Time.deltaTime / buildTime;
				buildProgress = Mathf.Clamp01(buildProgress);
				if (buildProgress == 1) {
					buildSpeed = 0;
					state = State.Active;
				}
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
		return !Physics.CheckSphere(position, builder.obstacleRadius, builder.obstacleLayers);
	}

	public bool PositionNearExistingBuilding(Vector3 position) {
		return Physics.CheckSphere(position, builder.maxBuildingDistance, builder.buildingLayers);
	}

	public virtual bool CanBuild() {
		return prefab.maxCount < 0 || prefab.count < prefab.maxCount;
	}

	void SetChildrenActive(bool active) {
		for (int c = 0; c < transform.childCount; c ++) {
			transform.GetChild(c).gameObject.SetActive(active);
		}
	}
}
