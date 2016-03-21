using UnityEngine;

//graphical connector between buildings
public class Connector : MonoBehaviour {

	public Building a { get; private set; }
	public Building b { get; private set; }

	public void Initialize(Building a, Building b) {
		this.a = a;
		this.b = b;
		Transform acp = a.connectionPoint;
		Transform bcp = b.connectionPoint;
		transform.position = (acp.position + bcp.position) / 2;
		Vector3 vector = (acp.position - bcp.position);
		Vector3 scale = transform.localScale;
		scale.z *= vector.magnitude;
		transform.localScale = scale;
		transform.forward = vector;
	}

}
