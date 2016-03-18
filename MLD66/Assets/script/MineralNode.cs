using UnityEngine;

public class MineralNode : MonoBehaviour {

	public Gradient colors;
	public string colorName = "_EmmisionColor";

	void Awake() {
		Renderer render = GetComponent<Renderer>();
		if (render != null) {
			Material mat = render.material;
			if (mat != null) {
				mat.SetColor(colorName, colors.Evaluate(Random.value));
			}
		}
	}

}
