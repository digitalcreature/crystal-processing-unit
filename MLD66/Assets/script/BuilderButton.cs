using UnityEngine;
using UnityEngine.UI;

public class BuilderButton : MonoBehaviour {

	protected Button button;
	protected Text text;

	protected virtual void Awake() {
		button = GetComponent<Button>();
		text = GetComponentInChildren<Text>();
	}

	protected virtual void Update() {
		button.interactable = !Builder.main.isBusy;
	}

}
