using UnityEngine;
using UnityEngine.UI;

public class BuildingProgressIndicator : MonoBehaviour {

	public Image wheelImage;

	[HideInInspector]
	public Building building;

	void Update() {
		transform.rotation = CameraRig.main.transform.rotation;
		transform.position = building.center.position;
		wheelImage.fillAmount = building.buildProgress;
	}

}
