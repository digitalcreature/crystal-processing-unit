using UnityEngine;
using System.Collections.Generic;

public class MiningModule : BuildingModule, IWorker {

	public float mineSpeed = 5;
	public float energyRate = 1;
	public float range = 1;
	public LayerMask mineralMask;

	public Material activeMaterial;
	public Material inactiveMaterial;

	public HashSet<MineralNode> nodes { get; private set; }

	new Renderer renderer;
	ParticleSystem particles;

	public float GetResourceRate(Resource.Type type) {
		if (nodes.Count > 0) {
			switch (type) {
				case Resource.Type.Mineral:
					return mineSpeed;
				case Resource.Type.Energy:
					return -energyRate;
			}
		}
		return 0;
	}

	public void Work(Resource.Rates rates) {
		//here is where we shrink the mineral nodes we are mining
		//if ()
		rates.mineral *= rates.energy / -energyRate;
	}

	void Awake() {
		renderer = GetComponent<Renderer>();
		particles = GetComponent<ParticleSystem>();
	}

	public override void Activate() {
		nodes = new HashSet<MineralNode>();
		nodes.Clear();
		foreach (Collider collider in Physics.OverlapSphere(transform.position, range, mineralMask)) {
			MineralNode node = collider.GetComponent<MineralNode>();
			if (node != null && !node.isMined) {
				nodes.Add(node);
				node.miner = this;
			}
		}
	}

	public override void Deactivate() {
		foreach (MineralNode node in nodes) {
			node.miner = null;
		}
	}

	protected override void OnActiveChange(bool active) {
		renderer.material = active ? activeMaterial : inactiveMaterial;
		if (active)
			StartMining();
		else
			StopMining();
	}

	void StartMining() {
		particles.Play();
		foreach (MineralNode node in nodes) {
			node.miner = this;
		}
	}

	void StopMining() {
		particles.Stop();
		foreach (MineralNode node in nodes) {
			node.miner = null;
		}
	}

	public override bool PlacingValid() {
		foreach (Collider collider in Physics.OverlapSphere(transform.position, range, mineralMask)) {
			MineralNode node = collider.GetComponent<MineralNode>();
			if (node != null && node.isMined) {
				return false;
			}
		}
		return true;
	}

}
