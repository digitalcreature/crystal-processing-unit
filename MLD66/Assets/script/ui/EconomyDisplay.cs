using UnityEngine;
using UnityEngine.UI;

public class EconomyDisplay : MonoBehaviour {

	public Text mineralCountDisplay;
	public Text mineralUsageDisplay;
	public Text energyCountDisplay;
	public Text energyUsageDisplay;

	void Awake() {
		if (mineralCountDisplay == null) {
			mineralCountDisplay = GetComponent<Text>();
		}
	}

	void Update() {
		Economy eco = Economy.main;
		mineralCountDisplay.text = string.Format("{0:0.00}/{1:0.00} M", eco.mineralCount, eco.mineralCapacity);
		mineralUsageDisplay.text = string.Format("{0:+#.00;-#.00;0.00}", -eco.mineralUsage);
		energyCountDisplay.text = string.Format("{0:0.00}/{1:0.00} E", eco.energyCount, eco.energyCapacity);
		energyUsageDisplay.text = string.Format("{0:+#.00;-#.00;0.00}", -eco.energyUsage);
	}

}
