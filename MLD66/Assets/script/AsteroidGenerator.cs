using UnityEngine;
using System.Collections.Generic;

public class AsteroidGenerator : MonoBehaviour {

	public int chunkCount = 25;
	public int samplesPerChunk = 25;
	public int mineralNodesPerChunk = 5;
	public MineralNode mineralNodePrefab;
	public AsteroidChunk chunkPrefab;

	AsteroidChunk root = null;

	void Start() {
		Generate();
	}

	public void Generate() {
		if (root != null) {
			DestroyImmediate(root.gameObject);
		}
		List<PointSample> samples = null;
		for (int c = 0; c < chunkCount; c ++) {
			AsteroidChunk newChunk = Instantiate(chunkPrefab) as AsteroidChunk;
			newChunk.name = chunkPrefab.name + c;
			AsteroidChunk parent = null;
			if (root == null) {
				newChunk.transform.parent = this.transform;
				newChunk.transform.localPosition = Vector3.zero;
				root = newChunk;
			}
			else {
				samples = SampleSurfacePoints(samplesPerChunk, samples);
				if (samples.Count > 0) {
					PointSample sample = samples[UnityEngine.Random.Range(0, samples.Count)];
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
		if (root != null) {
			root.transform.rotation = Random.rotation;
		}
		CenterMass();
		samples = SampleSurfacePoints(mineralNodesPerChunk, samples);
		foreach (PointSample sample in samples) {
			MineralNode node = Instantiate(mineralNodePrefab) as MineralNode;
			node.name = mineralNodePrefab.name;
			node.transform.SetParent(sample.chunk.transform, true);
			node.transform.position = sample.position;
			node.transform.forward = sample.chunk.GetNormal(node.transform.position);
		}
	}

	//sample for points in world space on the surface of chunks
	//throws out samples that are inside one or more chunks
	public List<PointSample> SampleSurfacePoints(int samplesPerChunk, List<PointSample> list = null) {
		if (list == null) {
			list = new List<PointSample>();
		}
		else {
			list.Clear();
		}
		foreach (AsteroidChunk chunk in chunks()) {
			for (int s = 0; s < samplesPerChunk; s ++) {
				Vector3 position = chunk.GetSurfacePoint();
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
			if (chunk.PointIsInternal(point)) {
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
