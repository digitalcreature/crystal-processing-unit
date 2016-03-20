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
	public LayerMask buildingLayers;				//stuff that counts as buildings
	public float maxBuildingDistance = .5f;	//maximum distance a new bulding can be from an old one
	public float rotateSpeed = 60;
	public Material validPlacingMaterial;
	public Material invalidPlacingMaterial;
	public Material inProgressMaterial;
	public BuildingProgressIndicator indicatorPrefab;

	[HideInInspector]
	public bool busy;

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
	public void StartPlacing(Building buildingPrefab) {
		if (buildingPrefab.CanBuild()) {
			Building building = Instantiate(buildingPrefab);
			building.name = buildingPrefab.name;
			building.transform.parent = buildingsParent;
			building.Initialize(this, buildingPrefab);
		}
	}

}
