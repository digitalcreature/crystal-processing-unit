using UnityEngine;

public class Building : MonoBehaviour {

	State _state;
	public State state {
		get {
			return _state;
		}
		set {
			_state = value;
		}
	}

	new Renderer renderer;
	Material material;

	void Awake() {
		renderer = GetComponent<Renderer>();
		if (renderer != null) {
			material = renderer.material;
		}
	}

	public enum State { Placing, Building, Active }

	void Update() {
		switch (state) {
			//building is being placed by player
			case State.Placing:
				renderer.material = Builder.main.validPlacing;
				RaycastHit hit;
				if (Physics.Raycast()) //TODO: finish this
				break;
			//building is constructing or deconstructing
			case State.Building:
				break;
			//building is finished
			case State.Active:
				break;
		}
	}

}
