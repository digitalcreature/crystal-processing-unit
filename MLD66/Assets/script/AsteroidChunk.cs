using UnityEngine;
using System.Collections.Generic;

public class AsteroidChunk : MonoBehaviour {

	public AsteroidGenerator generator { get; private set; }
	public AsteroidChunk parent { get; private set; }
	public float size { get; private set; }
	public AnimationCurve sizeCurve;
	public bool randomRotation = true;
	public float radius = 1;

	List<AsteroidChunk> children;

	public int childCount { get { return children.Count; } }

	public void Initialize(AsteroidGenerator generator, AsteroidChunk parent) {
		this.generator = generator;
		this.parent = parent;
		float size = this.size = sizeCurve.Evaluate(Random.value);
		children = new List<AsteroidChunk>();
		if (parent != null) {
			parent.children.Add(this);
		}
		Transform t = transform.parent;
		while (t != generator.transform) {
			size /= t.localScale.x;
			t = t.parent;
		}
		transform.localScale = Vector3.one * size;
		transform.rotation =  randomRotation ? Random.rotation : Quaternion.identity;
	}

	public AsteroidChunk GetChild(int i) {
		return children[i];
	}

}
