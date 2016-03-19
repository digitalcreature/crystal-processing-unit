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
		text.text = string.Format("{0}\n({1})", buildingPrefab.name, buildingPrefab.count);
		button.interactable = !Builder.main.busy;
	}

	void StartPlacing() {
		Builder.main.StartPlacing(buildingPrefab);
	}

}
