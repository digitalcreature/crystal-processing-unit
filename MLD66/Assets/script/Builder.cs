using UnityEngine;
using UnityEngine.UI;

//singleton manager that handles building construction
public class Builder : SingletonBehaviour<Builder> {

	public LayerMask validLayers;
	public LayerMask invalidLayers;
	public float minimumDistance = .25f;
	public float rotateSpeed = 60;
	public Material validPlacingMaterial;
	public Material invalidPlacingMaterial;
	public Material inProgressMaterial;

	bool _busy;
	public bool busy {
		get {
			return _busy;
		}
		set {
			if (_busy != value) {
				Button[] buttons = FindObjectsOfType<Button>();
				foreach (Button button in buttons) {
					if (button.tag == "Build Button") {
						button.interactable = !value;
					}
				}
				_busy = value;
			}
		}
	}

	public void Build(Building buildingPrefab) {
		Building building = Instantiate(buildingPrefab);
		building.name = buildingPrefab.name;
		building.transform.parent = transform;
		building.state = Building.State.Placing;
	}

	public bool PositionValid(Vector3 position) {
		return !Physics.CheckSphere(position, minimumDistance, invalidLayers);
	}

}
