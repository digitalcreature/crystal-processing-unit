using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour {

	public Building buildingPrefab;
	Button button;
	Text text;

	void Awake() {
		button = GetComponent<Button>();
		text = GetComponentInChildren<Text>();
		button.onClick.AddListener(StartPlacing);
	}

	void Update() {
		string maxCount = buildingPrefab.maxCount < 0 ? "infinity" : buildingPrefab.maxCount.ToString();
		text.text = string.Format("{0}\n({1}/{2})", buildingPrefab.name, buildingPrefab.count, maxCount);
		button.interactable = !Builder.main.busy && buildingPrefab.CanBuild();
	}

	void StartPlacing() {
		Builder.main.StartPlacing(buildingPrefab);
	}

}
