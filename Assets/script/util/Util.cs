using UnityEngine;
using System.Collections.Generic;

public class RendererGroup {

	List<Renderer> renderers;

	public Material material {
		get {
			return firstRenderer.material;
		}
		set {
			foreach (Renderer renderer in renderers) {
				renderer.material = value;
			}
		}
	}

	public bool enabled {
		get {
			return firstRenderer.enabled;
		}
		set {
			foreach (Renderer renderer in renderers) {
				renderer.enabled = value;
			}
		}
	}

	Renderer firstRenderer {
		get {
			return renderers.Count > 0 ? renderers[0] : null;
		}
	}

	public RendererGroup(MonoBehaviour behaviour, string tag) {
		renderers = new List<Renderer>();
		foreach (Renderer childRenderer in behaviour.GetComponentsInChildren<Renderer>()) {
			if (childRenderer.tag == tag) {
				renderers.Add(childRenderer);
			}
		}
	}

}
