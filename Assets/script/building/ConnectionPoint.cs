using UnityEngine;

public class ConnectionPoint : MonoBehaviour {

	public Shape shape;

	public Vector3 GetNearestPoint(Vector3 point) {
		Vector3 a, b, c, d;
		switch (shape) {
			case Shape.Point:
				return transform.position;
			case Shape.Line:
				a = transform.TransformPoint(new Vector3(.5f, 0));
				b = transform.TransformPoint(new Vector3(-.5f, 0));
				return ClosestOnLine(a, b, point);
			case Shape.Square:
				a = transform.TransformPoint(new Vector3(.5f, .5f));
				b = transform.TransformPoint(new Vector3(.5f, -.5f));
				c = transform.TransformPoint(new Vector3(-.5f, -.5f));
				d = transform.TransformPoint(new Vector3(-.5f, .5f));
				Vector3 nearest = ClosestOnLine(a, b, point);
				Vector3 candidate = ClosestOnLine(b, c, point);
				if (Vector3.Distance(candidate, point) < Vector3.Distance(nearest, point))
					nearest = candidate;
				candidate = ClosestOnLine(c, d, point);
				if (Vector3.Distance(candidate, point) < Vector3.Distance(nearest, point))
					nearest = candidate;
				candidate = ClosestOnLine(d, a, point);
				if (Vector3.Distance(candidate, point) < Vector3.Distance(nearest, point))
					nearest = candidate;
				return nearest;
		}
		return transform.position;
	}

	void OnDrawGizmosSelected() {
		Vector3 a, b, c, d;
		Gizmos.color = Color.white;
		switch (shape) {
			case Shape.Point:
				break;
			case Shape.Line:
				a = transform.TransformPoint(new Vector3(.5f, 0));
				b = transform.TransformPoint(new Vector3(-.5f, 0));
				Gizmos.DrawLine(a, b);
				break;
			case Shape.Square:
				a = transform.TransformPoint(new Vector3(.5f, .5f));
				b = transform.TransformPoint(new Vector3(.5f, -.5f));
				c = transform.TransformPoint(new Vector3(-.5f, -.5f));
				d = transform.TransformPoint(new Vector3(-.5f, .5f));
				Gizmos.DrawLine(a, b);
				Gizmos.DrawLine(b, c);
				Gizmos.DrawLine(c, d);
				Gizmos.DrawLine(d, a);
				break;
		}

	}

	Vector3 ClosestOnLine(Vector3 a, Vector3 b, Vector3 point) {
		Vector3 ap = point - a;
		Vector3 ab = b - a;
		float t = Mathf.Clamp01(Vector3.Dot(ap, ab) / ab.sqrMagnitude);
		return a + ab * t;
	}

	public enum Shape { Point, Line, Square }
}
