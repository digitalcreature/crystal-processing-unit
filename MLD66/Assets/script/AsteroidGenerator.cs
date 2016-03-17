using UnityEngine;
using System.Collections.Generic;

public class AsteroidGenerator : MonoBehaviour {

	public int count = 25;
	public int samplesPerChunk = 25;
	public AsteroidChunk chunkPrefab;

	AsteroidChunk root = null;

	void Start() {
		Generate();
	}

	public void Generate() {
		if (root != null) {
			DestroyImmediate(root.gameObject);
		}
		for (int c = 0; c < count; c ++) {
			AsteroidChunk newChunk = Instantiate(chunkPrefab) as AsteroidChunk;
			newChunk.name = "chunk" + c;
			AsteroidChunk parent = null;
			if (root == null) {
				newChunk.transform.parent = this.transform;
				newChunk.transform.localPosition = Vector3.zero;
				root = newChunk;
			}
			else {
				//sample for points
				List<PointSample> points = null;
				points = SamplePoints(samplesPerChunk, points);
				if (points.Count > 0) {
					PointSample sample = points[UnityEngine.Random.Range(0, points.Count)];
					parent = sample.chunk;
					newChunk.transform.parent = sample.chunk.transform;
					newChunk.transform.position = sample.position;
				}
				else {
					print("no place found for " + c);
					DestroyImmediate(newChunk.gameObject);
				}
			}
			if (newChunk != null) {
				newChunk.Initialize(this, parent);
			}
		}
		CenterMass();
	}

	public List<PointSample> SamplePoints(int samplesPerChunk, List<PointSample> list = null) {
		if (list == null) {
			list = new List<PointSample>();
		}
		else {
			list.Clear();
		}
		foreach (AsteroidChunk chunk in chunks()) {
			for (int s = 0; s < samplesPerChunk; s ++) {
				Vector3 position = UnityEngine.Random.onUnitSphere * chunk.radius;
				position = chunk.transform.TransformPoint(position);
				if (!CheckInternal(position)) {
					list.Add(new PointSample {
							position = position,
							chunk = chunk
					});
				}
			}
		}
		return list;
	}

	//TODO: use iteration instead of recursion for AsteroidGenerator.chunks()
	IEnumerable<AsteroidChunk> chunks() { return chunks(root); }
	IEnumerable<AsteroidChunk> chunks(AsteroidChunk chunk) {
		yield return chunk;
		for (int c = 0; c < chunk.childCount; c ++) {
			foreach (AsteroidChunk child in chunks(chunk.GetChild(c))) {
				yield return child;
			}
		}
	}

	bool CheckInternal(Vector3 point) {
		//point is in world space
		foreach (AsteroidChunk chunk in chunks()) {
			Vector3 localPoint = chunk.transform.InverseTransformPoint(point);
			if ((localPoint.magnitude) < (chunk.radius)) {
				return true;
			}
		}
		return false;
	}

	public void CenterMass() {
		Vector3 position = Vector3.zero;
		int count = 0;
		foreach (AsteroidChunk chunk in chunks()) {
			position += transform.InverseTransformPoint(chunk.transform.position);
			count ++;
		}
		position /= count;
		root.transform.localPosition = - position;
	}

}

public struct PointSample {
	public Vector3 position;
	public AsteroidChunk chunk;
}
