using UnityEngine;
using UnityEngine.UI;

public class EconomyDisplay : MonoBehaviour {

	public Text mineralCountDisplay;

	void Awake() {
		if (mineralCountDisplay == null) {
			mineralCountDisplay = GetComponent<Text>();
		}
	}

	void Update() {
		Economy eco = Economy.main;
		mineralCountDisplay.text = string.Format("{0:0.00}/{1:0.00} Minerals", eco.mineralCount, eco.mineralCapacity);
	}

}
