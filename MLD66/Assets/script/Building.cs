using UnityEngine;
using System.Collections.Generic;

//a building
public class Building : MonoBehaviour {

	public bool isMainBuilding = false;
	public int maxCount = -1;
	public float buildTime = 2;
	public Transform center;
	public Transform connectionPoint;		//point that connectors connect to
	public bool permanent = false;
	public string structureRenderersTag = "Building Structure";

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

	public static Building mainBuilding { get; private set; }
	RendererGroup structureRenderers;
	new Collider collider;
	static float angle;
	public float buildProgress { get; private set; }
	float buildSpeed = 0;
	BuildingProgressIndicator _indicator;
	BuildingProgressIndicator indicator {
		get {
			if (_indicator == null) {
				_indicator = Instantiate(builder.indicatorPrefab) as BuildingProgressIndicator;
				_indicator.name = builder.indicatorPrefab.name;
				_indicator.building = this;
				_indicator.transform.SetParent(builder.transform, false);
				_indicator.transform.position = transform.position;
			}
			return _indicator;
		}
	}

	public HashSet<BuildingModule> modules { get; private set; }

	bool modulesActive {
		set {
			foreach (BuildingModule module in modules) {
				if (module.gameObject != this.gameObject && module.gameObject.activeSelf != value) {
					module.gameObject.SetActive(value);
				}
			}
		}
	}

	static HashSet<Building> _grid;
	public static HashSet<Building> grid { get { if (_grid == null) _grid = new HashSet<Building>(); return _grid; } }

	public bool isConnected {
		get { return grid.Contains(this); }
	}

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

	public enum State { Placing, Constructing, Active }

	public void Initialize(Builder builder, Building prefab) {
		this.builder = builder;
		this.prefab = prefab;
		if (isMainBuilding) mainBuilding = this;
		center = (center == null) ? (transform) : (center);
		connectionPoint = (connectionPoint == null) ? (center) : (connectionPoint);
		structureRenderers = new RendererGroup(this, structureRenderersTag);
		collider = GetComponent<Collider>();
		state = State.Placing;
		neighbors = new HashSet<Building>();
		connectors = new HashSet<Connector>();
		modules = new HashSet<BuildingModule>();
		foreach (BuildingModule module in GetComponentsInChildren<BuildingModule>()) {
			modules.Add(module);
			module.Initialize(this);
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
		modulesActive = false;
		gameObject.layer = builder.incompleteBuildingLayer;
		indicator.gameObject.SetActive(false);
		collider.enabled = false;
		RaycastHit hit;
		Ray ray = CameraRig.main.camera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, builder.groundLayers)) {
			structureRenderers.enabled = true;
			transform.position = hit.point;
			transform.forward = hit.normal;
			angle += Input.GetAxis("Rotate Building") * Time.deltaTime * builder.rotateSpeed;
			Mathf.Repeat(angle, 360);
			transform.Rotate(0, 0, angle);
		}
		else {
			//hide the building if the player isnt pointing at ground
			structureRenderers.enabled = false;
		}
		bool valid = PlacingValid();
		structureRenderers.material = valid ? builder.validPlacingMaterial : builder.invalidPlacingMaterial;
		if (Input.GetMouseButtonDown((int) MouseButton.Left) && valid && structureRenderers.enabled) {
			state = State.Constructing;
			builder.status = Builder.Status.Idle;
			buildSpeed = 1;
			Connect();
			count ++;
			if (Input.GetButton("Multi Build")) {
				//if user is holding left shift, start placing another copy of this building
				builder.StartPlacing(prefab);
			}
		}
	}

	void UpdateConstructing() {
		gameObject.layer = builder.incompleteBuildingLayer;
		indicator.gameObject.SetActive(true);
		collider.enabled = true;
		structureRenderers.enabled = true;
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
		buildProgress += buildSpeed * Time.deltaTime / buildTime;
		buildProgress = Mathf.Clamp01(buildProgress);
		if (buildSpeed > 0 && buildProgress == 1) {
			buildSpeed = 0;
			state = State.Active;
			UpdateGrid();
		}
		if (buildSpeed < 0 && buildProgress == 0) {
			Demolish();
		}
	}

	void UpdateActive() {
		gameObject.layer = builder.buildingLayer;
		indicator.gameObject.SetActive(false);
		collider.enabled = true;
		structureRenderers.enabled = true;
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

	public bool PlacingValid() {
		foreach (BuildingModule module in modules) {
			if (!module.PlacingValid()) {
				return false;
			}
		}
		return !Physics.CheckSphere(transform.position, builder.obstacleRadius, builder.obstacleLayers) 
			//the main building doesn't need to placed near another building, other buildings do
			&& (isMainBuilding || Physics.CheckSphere(center.position, builder.maxBuildingDistance, builder.buildingLayers));
	}

	public virtual bool CanBuild() {
		return prefab.maxCount < 0 || prefab.count < prefab.maxCount;
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
		foreach (Collider col in Physics.OverlapSphere(center.position, builder.maxBuildingDistance, builder.buildingLayers)) {
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

	public void Demolish() {
		Disconnect();
		if (isMainBuilding) mainBuilding = null;
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
