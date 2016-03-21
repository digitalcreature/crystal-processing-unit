using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

//singleton manager that handles building construction
public class Builder : SingletonBehaviour<Builder> {

	[Layer] public int buildingLayer;
	[Layer] public int incompleteBuildingLayer;
	public LayerMask groundLayers;				//stuff you can build on
	public LayerMask obstacleLayers;				//stuff that blocks building placement
	public float obstacleRadius = .25f;
	public LayerMask buildingLayers;				//stuff that counts as completed buildings
	public float maxBuildingDistance = .5f;	//maximum distance a new bulding can be from an old one
	public float rotateSpeed = 60;
	public Material validPlacingMaterial;
	public Material invalidPlacingMaterial;
	public Material inProgressMaterial;
	public Material demolishSelectionMaterial;
	public Material cancelSelectionMaterial;
	public BuildingProgressIndicator indicatorPrefab;
	public Connector connectorPrefab;

	[HideInInspector]
	public Status status;
	public bool isBusy { get { return status != Status.Idle; } }

	Transform _buildingsParent;
	public Transform buildingsParent {
		get {
			if (_buildingsParent == null) {
				_buildingsParent = new GameObject("buildings").transform;
				_buildingsParent.parent = transform;
			}
			return _buildingsParent;
		}
	}

	Building placingBuilding;

	public void StartPlacing(Building buildingPrefab) {
		if (buildingPrefab.CanBuild()) {
			if (placingBuilding != null) {
				placingBuilding.CancelPlacing();
			}
			placingBuilding = Instantiate(buildingPrefab);
			placingBuilding.name = buildingPrefab.name;
			placingBuilding.transform.parent = buildingsParent;
			placingBuilding.Initialize(this, buildingPrefab);
			status = Status.Placing;
		}
	}

	void Update() {
		if (status != Status.Placing) {
			placingBuilding = null;
		}
		if (Input.GetMouseButtonDown((int) MouseButton.Right)) {
			status = Status.Idle;
		}
	}

	public void StartDemolishing() {
		status = Status.Demolishing;
	}

	public void StartCanceling() {
		status = Status.Canceling;
	}

	public enum Status { Idle, Placing, Demolishing, Canceling }

}
