using UnityEngine;
using System.Collections.Generic;

public class AsteroidGenerator : MonoBehaviour {

	public GameObject chunkPrefab;

	public int count = 25;
	public AnimationCurve sizeCurve;
	public float radiusFactor = 0.5f;
	public int samplesPerChunk = 25;

	Transform root = null;

	struct PointSample {
		public Vector3 position;
		public Transform transform;
	}

	public void Generate() {
		if (root != null) {
			DestroyImmediate(root.gameObject);
		}
		for (int c = 0; c < count; c ++) {
			float newSize = sizeCurve.Evaluate((float) c / count);
			Transform newChunk = Instantiate(chunkPrefab).transform;
			newChunk.name = "chunk" + c;
			if (root == null) {
				newChunk.parent = this.transform;
				newChunk.localPosition = Vector3.zero;
				root = newChunk;
			}
			else {
				//sample for points
				List<PointSample> points = new List<PointSample>();
				foreach (Transform chunk in tree(root)) {
					for (int s = 0; s < samplesPerChunk; s ++) {
						Vector3 position = UnityEngine.Random.onUnitSphere * radiusFactor;
						position = chunk.TransformPoint(position);
						if (!CheckInternal(position)) {
							points.Add(new PointSample {
									position = position,
									transform = chunk
							});
						}
					}
				}
				if (points.Count > 0) {
					PointSample sample = points[UnityEngine.Random.Range(0, points.Count)];
					newChunk.parent = sample.transform;
					newChunk.position = sample.position;
				}
				else {
					print("no place found for " + c);
					DestroyImmediate(newChunk.gameObject);
				}
			}
			if (newChunk != null) {
				newChunk.localScale = Vector3.one * newSize;
				newChunk.rotation = Random.rotation;
			}
		}
	}

	IEnumerable<Transform> tree(Transform node) {
		yield return node;
		for (int c = 0; c < node.childCount; c ++) {
			foreach (Transform child in tree(node.GetChild(c))) {
				yield return child;
			}
		}
	}

	bool CheckInternal(Vector3 point) {
		//point is in world space
		foreach (Transform chunk in tree(root)) {
			Vector3 localPoint = chunk.InverseTransformPoint(point);
			if ((localPoint.magnitude) < (radiusFactor)) {
				return true;
			}
		}
		return false;
	}

}
