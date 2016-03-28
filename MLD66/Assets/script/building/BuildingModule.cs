using UnityEngine;

public abstract class BuildingModule : MonoBehaviour {

	public Building building { get; private set; }

	bool _active;
	public bool active {
		get {
			return _active;
		}
		private set {
			if (_active != value)
				OnActiveChange(value);
			_active = value;
		}
	}

	public void Initialize(Building building) {
		this.building = building;
	}

	//called when building becomes active
	public virtual void Activate() {}

	protected virtual void Update() {
		active = building.isConnected;
	}

	protected virtual void OnActiveChange(bool value) {}

	public virtual bool PlacingValid() {
		return true;
	}

}
