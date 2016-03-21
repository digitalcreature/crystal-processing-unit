using UnityEngine;
using UnityEngine.UI;

public class BuilderButton : MonoBehaviour {

	public Text text;
	public Text hotKeyText;
	public KeyCode hotKey;

	protected Button button;

	protected virtual void Awake() {
		button = GetComponent<Button>();
	}

	protected virtual void Update() {
		if (hotKeyText != null) {
			hotKeyText.text = string.Format("[{0}]", hotKey.ToString());
		}
		if (Input.GetKeyDown(hotKey) && button.interactable) {
			button.onClick.Invoke();
		}
	}

}
