using UnityEngine;

public class Builder : MonoBehaviour {

	[Header("Materials")]
	public Material validPlacing;
	public Material invalidPlacing;

	static Builder _main;
	public static Builder main {
		get {
			if (_main == null) {
				_main = FindObjectOfType<Builder>();
				if (_main == null) {
					Debug.LogWarning("No Builder in scene!");
				}
			}
			return _main;
		}
	}

	public void Build(Building buildingPrefab) {
		Building building = Instantiate(buildingPrefab);
		building.name = buildingPrefab.name;
		building.transform.parent = transform;
		building.state = Building.State.Placing;
	}

}
