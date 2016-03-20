using UnityEngine;
using UnityEngine.UI;

public class BuildingProgressIndicator : MonoBehaviour {

	[HideInInspector]
	public Building building;

	Image image;
	void Awake() {
		image = GetComponent<Image>();
	}

	void Update() {
		transform.rotation = CameraRig.main.transform.rotation;
		transform.position = building.transform.position;
		image.fillAmount = building.buildProgress;
	}

}
