using UnityEngine;
using System.Collections.Generic;

//a building
public class Building : MonoBehaviour, IWorker {

	public int maxCount = -1;
	public float mineralCost = 5;
	public Transform center;
	public bool permanent = false;
	public string structureRenderersTag = "Building Structure";

	public State state { get; private set; }
	public bool isPlacing { get { return state == State.Placing; } }
	public bool isConstructing { get { return state == State.Constructing; } }
	public bool isActive { get { return state == State.Active; } }
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

	bool _hiding;
	public bool hiding {
		get {
			return _hiding;
		}
		private set {
			if (value) {
				transform.position = builder.placingBuildingHidingPlace.position;
			}
			_hiding = value;
		}
	}

	public bool obstacleBlocked { get; private set; }

	RendererGroup structureRenderers;
	// BoxCollider boxCollider;
	public static float placingAngle;
	public float buildProgress { get; private set; }
	public float buildSpeed { get; private set; }
	//the ui progress indicator for this building
	public BuildingProgressIndicator indicator { get; private set; }

	//building modules attached to this building
	public HashSet<BuildingModule> modules { get; private set; }

	//used to toggle all modules active/inactive
	bool modulesActive {
		set {
			foreach (BuildingModule module in modules) {
				if (module.gameObject != this.gameObject && module.gameObject.activeSelf != value) {
					module.gameObject.SetActive(value);
				}
			}
		}
	}

	//reference to main building module, null if none present
	public static MainBuildingModule mainBuildingModule { get; private set; }
	public bool isMainBuilding {
		get {
			//if this is a prefab, builder will be null and mainBuilding will
			//be some instaniated instance, so check for module
			if (builder == null) {
				return GetComponentInChildren<MainBuildingModule>() != null;
			}
			else {
				//otherwise just compare directly
				return mainBuilding == this;
			}
		}
	}
	public static Building mainBuilding { get { return mainBuildingModule == null ? null : mainBuildingModule.building; } }
	static HashSet<Building> _grid;
	public static HashSet<Building> grid { get { if (_grid == null) _grid = new HashSet<Building>(); return _grid; } }
	public bool isConnected {
		get { return grid.Contains(this); }
	}

	//counts for each building prefab
	static Dictionary<Building, int> _counts; //maps building prefabs to count of existing buildings
	static Dictionary<Building, int> counts {
		get {
			if (_counts == null) {
				_counts = new Dictionary<Building, int>();
			}
			return _counts;
		}
	}

	//count of this building and its prefab in the scene
	public int count {
		get {
			return counts.ContainsKey(prefab) ? counts[prefab] : 0;
		}
		private set {
			counts[prefab] = value;
		}
	}

	//true if building is under mouse cursor
	public bool underCursor {
		get {
			RaycastHit hit;
			Ray ray = CameraRig.main.camera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
				return hit.transform == transform;
			}
			return false;
		}
	}

	public HashSet<Building> neighbors { get; private set; }
	public HashSet<Connector> connectors { get; private set; }

	HashSet<ConnectionPoint> connectionPoints;

	public enum State { Placing, Constructing, Active }

	public void Initialize(Builder builder, Building prefab) {
		this.builder = builder;
		this.prefab = prefab;
		MainBuildingModule mainBuildingModule = GetComponent<MainBuildingModule>();
		if (mainBuildingModule != null) {
			Building.mainBuildingModule = mainBuildingModule;
			permanent = true;
		}
		center = (center == null) ? (transform) : (center);
		structureRenderers = new RendererGroup(this, structureRenderersTag);
		// boxCollider = GetComponent<BoxCollider>();
		state = State.Placing;
		neighbors = new HashSet<Building>();
		connectors = new HashSet<Connector>();
		modules = new HashSet<BuildingModule>();
		foreach (BuildingModule module in GetComponentsInChildren<BuildingModule>()) {
			modules.Add(module);
			module.Initialize(this);
		}
		indicator = Instantiate(builder.indicatorPrefab) as BuildingProgressIndicator;
		indicator.name = builder.indicatorPrefab.name;
		indicator.building = this;
		indicator.transform.SetParent(builder.transform, false);
		indicator.transform.position = transform.position;
		Rigidbody body = gameObject.AddComponent<Rigidbody>();
		body.constraints = RigidbodyConstraints.FreezeAll;
		connectionPoints = new HashSet<ConnectionPoint>();
		foreach (ConnectionPoint point in GetComponentsInChildren<ConnectionPoint>()) {
			connectionPoints.Add(point);
		}
		if (connectionPoints.Count == 0) {
			ConnectionPoint point = center.gameObject.AddComponent<ConnectionPoint>();
			point.shape = ConnectionPoint.Shape.Point;
			connectionPoints.Add(point);
		}
	}

	void Update() {
		switch (state) {
			//building is being placed by player
			case State.Placing:
				UpdatePlacing();
				break;
			//building is constructing or demolishing
			case State.Constructing:
				UpdateConstructing();
				break;
			//building is finished
			case State.Active:
				UpdateActive();
				break;
		}
	}

	void UpdatePlacing() {
		if (builder.status != Builder.Status.Placing) {
			//if the builder isnt placing anymore, cancel placement
			CancelPlacing();
			return;
		}
		gameObject.layer = builder.placingBuildingLayer;
		modulesActive = false;
		indicator.gameObject.SetActive(false);
		RaycastHit hit;
		Ray ray = CameraRig.main.camera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, builder.groundMask)) {
			hiding = false;
			//structureRenderers.enabled = true;
			transform.position = hit.point;
			transform.forward = hit.normal;
			placingAngle += Input.GetAxis("Rotate Building") * Time.deltaTime * builder.rotateSpeed;
			Mathf.Repeat(placingAngle, 360);
			transform.Rotate(0, 0, placingAngle);
		}
		else {
			//hide the building if the player isnt pointing at ground
			hiding = true;
			//structureRenderers.enabled = false;
		}
		bool valid = PlacingValid();
		structureRenderers.material = valid ? builder.validPlacingMaterial : builder.invalidPlacingMaterial;
		if (Input.GetMouseButtonDown((int) MouseButton.Left) && valid && !hiding) {
			if (mineralCost == 0) {
				Activate();
			}
			else {
				state = State.Constructing;
				buildSpeed = 1;
			}
			Connect();
			builder.status = Builder.Status.Idle;
			count ++;
			if (Input.GetButton("Multi Build")) {
				//if user is holding left shift, start placing another copy of this building
				builder.StartPlacing(prefab);
			}
		}
	}

	//ECONOMY
	public float GetResourceRate(Resource.Type type) {
		switch (type) {
			case Resource.Type.Mineral:
				return isConstructing ? -builder.mineralUsageRate * buildSpeed : 0;
			case Resource.Type.Energy:
				return 0;
		}
		return 0;
	}

	public void Work(Resource.Rates rates) {
		if (isConstructing) {
			buildProgress += -rates.mineral / mineralCost * Time.deltaTime;
			buildProgress = Mathf.Clamp01(buildProgress);
			if (buildSpeed > 0 && buildProgress >= 1) {
				Activate();
			}
			if (buildSpeed < 0 && buildProgress <= 0) {
				Demolish();
			}
		}
	}

	void UpdateConstructing() {
		gameObject.layer = builder.constructingBuildingLayer;
		indicator.gameObject.SetActive(true);
		structureRenderers.material = builder.inProgressMaterial;
		modulesActive = false;
		if (underCursor) {
			switch (builder.status) {
				case Builder.Status.Canceling:
					structureRenderers.material = builder.cancelSelectionMaterial;
					if (Input.GetMouseButtonDown((int) MouseButton.Left)) {
						buildSpeed *= -1;
						if (!Input.GetButton("Multi Build")) {
							builder.status = Builder.Status.Idle;
						}
					}
					break;
			}
		}

	}

	void UpdateActive() {
		gameObject.layer = builder.activeBuildingLayer;
		indicator.gameObject.SetActive(false);
		structureRenderers.material = isConnected ? builder.connectedMaterial : builder.disconnectedMaterial;
		modulesActive = true;
		if (underCursor) {
			switch (builder.status) {
				case Builder.Status.Demolishing:
					if (!permanent) {
						structureRenderers.material = builder.demolishSelectionMaterial;
						if (Input.GetMouseButtonDown((int) MouseButton.Left)) {
							StartDemolish();
							if (!Input.GetButton("Multi Build")) {
								builder.status = Builder.Status.Idle;
							}
						}
					}
					break;
			}
		}
	}

	void Activate() {
		buildSpeed = 0;
		state = State.Active;
		foreach (BuildingModule module in modules) {
			module.Activate();
		}
		UpdateGrid();
	}

	void OnCollisionStay() {
		if (state == State.Placing) {
			obstacleBlocked = true;
		}
	}

	void FixedUpdate() {
		obstacleBlocked = false;
	}

	public bool PlacingValid() {
		foreach (BuildingModule module in modules) {
			if (!module.PlacingValid()) {
				return false;
			}
		}
		return !obstacleBlocked
			//the main building doesn't need to placed near another building, other buildings do
			&& (isMainBuilding || Physics.CheckSphere(center.position, builder.maxBuildingDistance, builder.placingNeighborMask));
	}

	void OnDrawGizmos() {
		if (state == State.Placing && builder != null) {
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(center.position, builder.maxBuildingDistance);
		}
	}

	//return true if the player can start placing the building
	public bool CanBuild() {
		return (maxCount < 0 || count < maxCount) && (isMainBuilding || (mainBuilding != null && mainBuilding.state != State.Placing));
	}

	public void CancelPlacing() {
		if (state == State.Placing) {
			Destroy(gameObject);
			Destroy(indicator.gameObject);
		}
	}

	public void StartDemolish() {
		if (!permanent) {
			buildSpeed = -1;
			state = State.Constructing;
			UpdateGrid();
		}
	}

	//forge new connections with neighbors, if available
	public void Connect() {
		foreach (Collider col in Physics.OverlapSphere(center.position, builder.maxBuildingDistance, builder.placingNeighborMask)) {
			Building building = col.GetComponent<Building>();
			if (building != null && !neighbors.Contains(building)) {
				Connector connector = Instantiate(builder.connectorPrefab) as Connector;
				connector.name = builder.connectorPrefab.name;
				connector.transform.parent = builder.transform;
				connector.Initialize(this, building);
				neighbors.Add(building);
				building.neighbors.Add(this);
				connectors.Add(connector);
				building.connectors.Add(connector);
			}
		}
		UpdateGrid();
	}

	//remove all connections to neighbors
	public void Disconnect() {
		foreach (Building neighbor in neighbors) {
			neighbor.neighbors.Remove(this);
		}
		neighbors.Clear();
		foreach (Connector connector in connectors) {
			if (connector.a == this) connector.b.connectors.Remove(connector);
			if (connector.b == this) connector.a.connectors.Remove(connector);
			Destroy(connector.gameObject);
		}
		connectors.Clear();
		UpdateGrid();
	}

	public Vector3 GetNearestConnectionPoint(Vector3 point) {
		Vector3 nearest = Vector3.one * 1000000;
		foreach (ConnectionPoint connectionPoint in connectionPoints) {
			Vector3 candidate = connectionPoint.GetNearestPoint(point);
			if (Vector3.Distance(candidate, point) < Vector3.Distance(nearest, point))
				nearest = candidate;
		}
		return nearest;
	}

	public void Demolish() {
		Disconnect();
		if (isMainBuilding) mainBuildingModule = null;
		count --;
		Destroy(indicator.gameObject);
		Destroy(gameObject);
	}

	//update the grid of buildings using an iterative brushfire
	//called whenever a building is connected or disconnected
	public static void UpdateGrid() {
		grid.Clear();
		if (mainBuilding != null) {
			Queue<Building> queue = new Queue<Building>();
			queue.Enqueue(mainBuilding);
			while (queue.Count > 0) {
				Building building = queue.Dequeue();
				grid.Add(building);
				if (building.state != State.Constructing) {
					foreach (Building neighbor in building.neighbors) {
						if (!grid.Contains(neighbor)) {
							queue.Enqueue(neighbor);
						}
					}
				}
			}
		}
	}

}
