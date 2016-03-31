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
		switch (type) {
			case Resource.Type.Mineral:
				return mineSpeed;
			case Resource.Type.Energy:
				return -energyRate;
		}
		return 0;
	}

	public void Work(Resource.Rates rates) {
		//here is where we shrink the mineral nodes we are mining
		rates.mineral *= rates.energy / -energyRate;
	}

	void Awake() {
		renderer = GetComponent<Renderer>();
		particles = GetComponent<ParticleSystem>();
	}

	public override void Activate() {
		nodes = new HashSet<MineralNode>();
		Connect();
	}

	public void Connect() {
		nodes.Clear();
		foreach (Collider collider in Physics.OverlapSphere(transform.position, range, mineralMask)) {
			MineralNode node = collider.GetComponent<MineralNode>();
			if (node != null) {
				nodes.Add(node);
			}
		}
	}

	protected override void OnActiveChange(bool active) {
		renderer.material = active ? activeMaterial : inactiveMaterial;
		if (active)
			particles.Play();
		else
			particles.Stop();
	}

	public override bool PlacingValid() {
		return Physics.CheckSphere(transform.position, range, mineralMask);
	}
}
