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
		Vector3 center = (a.center.position + b.center.position) / 2;
		Vector3 ap = a.GetNearestConnectionPoint(center);
		Vector3 bp = b.GetNearestConnectionPoint(center);
		transform.position = (ap + bp) / 2;
		Vector3 vector = (ap - bp);
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
