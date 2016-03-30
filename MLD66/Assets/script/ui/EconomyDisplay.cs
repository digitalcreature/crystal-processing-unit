using UnityEngine;
using UnityEngine.UI;

public class EconomyDisplay : MonoBehaviour {

	public Text mineralCountDisplay;
	public Text mineralUsageDisplay;
	public Text energyCountDisplay;
	public Text energyUsageDisplay;
	public Image mineralBar;
	public Image energyBar;

	void Update() {
		// Economy eco = Economy.main;
		// mineralCountDisplay.text = string.Format("{0}/{1} Mineral", FormatNumber(eco.mineralCount), FormatNumber(eco.mineralCapacity));
		// mineralUsageDisplay.text = string.Format("{0}", FormatNumber(-eco.mineralRate, true));
		// energyCountDisplay.text = string.Format("{0}/{1} Energy", FormatNumber(eco.energyCount), FormatNumber(eco.energyCapacity));
		// energyUsageDisplay.text = string.Format("{0}", FormatNumber(-eco.energyRate, true));
		// mineralBar.fillAmount = eco.mineralCount / eco.mineralCapacity;
		// energyBar.fillAmount = eco.energyCount / eco.energyCapacity;
	}

	public string FormatNumber(float number, bool sign = false) {
		string postfix = "";
		if (number >= 1000000000) {	//billions
			number /= 1000000000;
			postfix = "B";
		}
		else if (number >= 1000000) {	//millions
			number /= 1000000;
			postfix = "M";
		}
		else if (number >= 1000) {		//thousands
			number /= 1000;
			postfix = "K";
		}
		return string.Format(sign ? "{0:+#.00;-#.00;0.00}{1}" : "{0:0.00}{1}", number, postfix);
	}

}
