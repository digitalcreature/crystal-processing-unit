using UnityEngine;

public abstract class BuildingModule : MonoBehaviour {

	public Building building { get; private set; }

	public void Initialize(Building building) {
		this.building = building;
	}

}
