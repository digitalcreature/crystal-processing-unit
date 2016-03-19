using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

//singleton manager that handles building construction
public class Builder : SingletonBehaviour<Builder> {

	public LayerMask groundLayers;				//stuff you can build on
	public LayerMask obstacleLayers;				//stuff that blocks building placement
	public float obstacleRadius = .25f;
	public LayerMask buildingLayers;				//stuff that counts as buildings
	public float maxBuildingDistance = .5f;	//maximum distance a new bulding can be from an old one
	public float rotateSpeed = 60;
	public Material validPlacingMaterial;
	public Material invalidPlacingMaterial;
	public Material inProgressMaterial;

	[HideInInspector]
	public bool busy;

	public void StartPlacing(Building buildingPrefab) {
		Building building = Instantiate(buildingPrefab);
		building.name = buildingPrefab.name;
		building.transform.parent = transform;
		building.Initialize(this, buildingPrefab);
	}

	public bool PositionValid(Vector3 position) {
		return !Physics.CheckSphere(position, obstacleRadius, obstacleLayers) &&
				Physics.CheckSphere(position, maxBuildingDistance, buildingLayers);
	}

}
