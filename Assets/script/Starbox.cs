using UnityEngine;

public class Starbox : MonoBehaviour {

	public float rotationSpeed = 5;

	void Update() {
		transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
	}

}
