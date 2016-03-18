using UnityEngine;
using System.Collections.Generic;

public class AsteroidChunk : MonoBehaviour {

	public Shape shape = Shape.Sphere;
	public AnimationCurve sizeCurve;
	public bool randomRotation = true;
	public float radius = 1;

	public float size { get; private set; }
	public AsteroidGenerator generator { get; private set; }
	public AsteroidChunk parent { get; private set; }
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

	public Vector3 GetSurfacePoint() {
		Vector3 local = Vector3.zero;
		switch (shape) {
			case Shape.Sphere:
				local = Random.onUnitSphere * radius;
				break;
			case Shape.Cube:
				local = new Vector3(
					Random.Range(-1f, 1f),
					Random.Range(-1f, 1f),
					Random.Range(-1f, 1f)
				);
				switch (Random.Range(0, 3)) {
					case 0:	local.x = Mathf.Clamp(local.x, -1, 1); break;
					case 1:	local.y = Mathf.Clamp(local.y, -1, 1); break;
					case 2:	local.z = Mathf.Clamp(local.z, -1, 1); break;
				}
				local *= radius;
				break;
		}
		return transform.TransformPoint(local);
	}

	public bool PointIsInternal(Vector3 point) {
		point = transform.InverseTransformPoint(point);
		switch (shape) {
			case Shape.Sphere:
				return point.magnitude < radius;
			case Shape.Cube:
				return Mathf.Max(
					Mathf.Abs(point.x),
					Mathf.Abs(point.y),
					Mathf.Abs(point.z)
				) < radius * .9f;	//TODO: fix cube generation
		}
		return true;
	}

	public Vector3 GetNormal(Vector3 point) {
		Vector3 local = Vector3.zero;
		point = transform.InverseTransformPoint(point);
		switch (shape) {
			case Shape.Sphere:
				local = point;
				break;
			case Shape.Cube:
				if (Mathf.Abs(point.x) > Mathf.Abs(point.y) && Mathf.Abs(point.x) > Mathf.Abs(point.z))
					local = Vector3.right * Mathf.Sign(point.x);
				else if (Mathf.Abs(point.y) > Mathf.Abs(point.x) && Mathf.Abs(point.y) > Mathf.Abs(point.z))
					local = Vector3.up * Mathf.Sign(point.y);
				else if (Mathf.Abs(point.z) > Mathf.Abs(point.x) && Mathf.Abs(point.z) > Mathf.Abs(point.y))
					local = Vector3.forward * Mathf.Sign(point.z);
				break;
		}
		return transform.TransformDirection(local);
	}

	public enum Shape { Sphere, Cube }
}
