using UnityEngine;

public class RandomColorizer : MonoBehaviour {

	public Gradient colors;
	public string colorName = "_Color";

	void Awake() {
		if (enabled) {
			Renderer render = GetComponent<Renderer>();
			if (render != null) {
				Material mat = render.material;
				if (mat != null) {
					mat.SetColor(colorName, colors.Evaluate(Random.value));
				}
			}
		}
	}

	void Update() {}
}
