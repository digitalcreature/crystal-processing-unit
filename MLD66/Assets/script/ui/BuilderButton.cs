using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class BuilderButton : MonoBehaviour {

	public Text text;
	public Text hotKeyText;
	public string hotKey;

	protected Button button;

	protected virtual void Awake() {
		button = GetComponent<Button>();
	}

	protected virtual void Update() {
		if (hotKeyText != null) {
			hotKeyText.text = string.Format("[{0}]", hotKey.ToUpper());
		}
		if (Input.GetKeyDown(hotKey.ToLower()) && button.interactable) {
			button.onClick.Invoke();
		}
	}

}
