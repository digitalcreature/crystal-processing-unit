using UnityEngine;
using UnityEngine.UI;

public class BuilderCursor : MonoBehaviour {

	public Sprite buildCursor;
	public Sprite demolishCursor;
	public Sprite cancelCursor;

	Image image;

	void Awake() {
		image = GetComponent<Image>();
	}

	void Update() {
		transform.position = Input.mousePosition;
		image.enabled = true;
		switch (Builder.main.status) {
			case Builder.Status.Placing:
				image.sprite = buildCursor;
				break;
			case Builder.Status.Demolishing:
				image.sprite = demolishCursor;
				break;
			case Builder.Status.Canceling:
				image.sprite = cancelCursor;
				break;
			default:
				image.enabled = false;
				break;
		}
	}

}
