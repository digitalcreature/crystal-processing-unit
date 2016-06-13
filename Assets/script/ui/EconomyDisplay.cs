using UnityEngine;
using UnityEngine.UI;

public class EconomyDisplay : MonoBehaviour {

	public ResourceDisplay mineral;
	public ResourceDisplay energy;

	void Awake() {
		mineral.type = Resource.Type.Mineral;
		energy.type = Resource.Type.Energy;
	}

	void Update() {
		mineral.Update();
		energy.Update();
	}

	public static string FormatNumber(float number, bool sign = false) {
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

	[System.Serializable]
	public class ResourceDisplay {

		[HideInInspector]
		public Resource.Type type;
		public Text countDisplay;
		public Image countFill;

		public void Update() {
			Economy eco = Economy.main;
			Resource resource = eco.resources[type];
			countDisplay.text = string.Format("{0}/{1}", FormatNumber(resource.count), FormatNumber(resource.capacity));
			countFill.fillAmount = resource.count / resource.capacity;
		}
	}


}
