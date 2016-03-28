using UnityEngine;

//graphical connector between buildings
public class Connector : MonoBehaviour {

	public string structureRenderersTag = "Building Structure";

	public Building a { get; private set; }
	public Building b { get; private set; }

	RendererGroup structureRenderers;

	public void Initialize(Building a, Building b) {
		this.a = a;
		this.b = b;
		structureRenderers = new RendererGroup(this, structureRenderersTag);
		Transform acp = a.connectionPoint;
		Transform bcp = b.connectionPoint;
		transform.position = (acp.position + bcp.position) / 2;
		Vector3 vector = (acp.position - bcp.position);
		Vector3 scale = transform.localScale;
		scale.z *= vector.magnitude;
		transform.localScale = scale;
		transform.forward = vector;
	}

	void Update() {
		Builder builder = Builder.main;
		if (a.state == Building.State.Constructing || b.state == Building.State.Constructing) {
			structureRenderers.material = builder.inProgressMaterial;
		}
		else {
			structureRenderers.material = a.isConnected || b.isConnected ? Builder.main.connectedMaterial : Builder.main.disconnectedMaterial;
		}
	}

}
