using UnityEngine;
using System.Collections.Generic;

//generates an asteroid from chunks
public class AsteroidGenerator : MonoBehaviour {

	public int chunkCount = 25;					//how many chunks to try to place
	public int samplesPerChunk = 25;				//how many points to sample for each chunk when placing new chunks
	public int mineralNodesPerChunk = 5;		//how many mineral nodes to try to place for each chunk
	public MineralNode mineralNodePrefab;		//mineral node prefab
	public AsteroidChunk chunkPrefab;			//asteroid chunk prefab

	AsteroidChunk root = null;						//root of the asteroid chunk tree

	void Start() {
		Generate();
	}

	//generate asteroid
	public void Generate() {
		if (root != null) {
			//if the tree already exists, destroy it and start anew
			DestroyImmediate(root.gameObject);
		}
		List<PointSample> samples = null;
		//start placing chunks
		for (int c = 0; c < chunkCount; c ++) {
			//create a chunk
			AsteroidChunk newChunk = Instantiate(chunkPrefab) as AsteroidChunk;
			newChunk.name = chunkPrefab.name + c;
			AsteroidChunk parent = null;	//the chunk that is to be the parent of the new chunk
			if (root == null) {
				//if there isnt a root yet, this chunk should be the new root
				newChunk.transform.parent = this.transform;
				newChunk.transform.localPosition = Vector3.zero;
				root = newChunk;
			}
			else {
				//if there already is a root, try to attach the chunk to one of the existing chunks
				samples = SampleSurfacePoints(samplesPerChunk, samples);
				if (samples.Count > 0) {
					//if at least one sample was found, pick one at random and place the new chunk there
					PointSample sample = samples[Random.Range(0, samples.Count)];
					parent = sample.chunk;
					newChunk.transform.parent = sample.chunk.transform;
					newChunk.transform.position = sample.position;
				}
				else {
					//if there werent any points found, destroy the chunk because theres no where for it to go
					Debug.Log("no place found for " + c);
					DestroyImmediate(newChunk.gameObject);
				}
			}
			if (newChunk != null) {
				//initialize the chunk
				newChunk.Initialize(this, parent);
			}
		}
		if (root != null) {
			//rotate the whole tree
			root.transform.rotation = Random.rotation;
		}
		CenterMass();	//center the tree in space
		//sample more points for mineral node placement
		samples = SampleSurfacePoints(mineralNodesPerChunk, samples);
		foreach (PointSample sample in samples) {
			//create a mineral node and place it
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
		//loop through the chunks
		foreach (AsteroidChunk chunk in chunks()) {
			//loop through the samples
			for (int s = 0; s < samplesPerChunk; s ++) {
				Vector3 position = chunk.GetSurfacePoint();	//get a point
				if (!CheckInternal(position)) {
					//if the point isnt inside any chunks, add it to the list
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

	//return true if world space point is inside at least one chunk
	bool CheckInternal(Vector3 point) {
		//point is in world space
		foreach (AsteroidChunk chunk in chunks()) {
			if (chunk.PointIsInternal(point)) {
				return true;
			}
		}
		return false;
	}

	//center the chunk tree in space
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

//represents a single point in world space and the chunk whose surface it is on
public struct PointSample {
	public Vector3 position;
	public AsteroidChunk chunk;
}
