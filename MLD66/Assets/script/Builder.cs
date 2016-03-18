using UnityEngine;

public class Builder : SingletonBehaviour<Builder> {

	public LayerMask validLayers;
	public LayerMask invalidLayers;
	public float minimumDistance = .25f;
	[Header("Materials")]
	public Material validPlacing;
	public Material invalidPlacing;

	public void Build(Building buildingPrefab) {
		Building building = Instantiate(buildingPrefab);
		building.name = buildingPrefab.name;
		building.transform.parent = transform;
		building.state = Building.State.Placing;
	}

}
