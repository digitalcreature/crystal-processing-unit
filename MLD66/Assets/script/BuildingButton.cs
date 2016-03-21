using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : BuilderButton {

	public Building buildingPrefab;

	protected override void Awake() {
		base.Awake();
		button.onClick.AddListener(StartPlacing);
	}

	protected override void Update() {
		base.Update();
		string maxCount = buildingPrefab.maxCount < 0 ? "infinity" : buildingPrefab.maxCount.ToString();
		text.text = string.Format("{0}\n({1}/{2})", buildingPrefab.name, buildingPrefab.count, maxCount);
		button.interactable = !Builder.main.isBusy && buildingPrefab.CanBuild();
	}

	void StartPlacing() {
		Builder.main.StartPlacing(buildingPrefab);
	}

}
