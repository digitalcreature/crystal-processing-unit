using UnityEngine;
using System.Collections.Generic;

public class MiningModule : BuildingModule, IMineralMachine {

	public float mineSpeed = 5;
	public float range = 1;
	public LayerMask mineralMask;

	public Material activeMaterial;
	public Material inactiveMaterial;

	public HashSet<MineralNode> nodes { get; private set; }

	new Renderer renderer;
	ParticleSystem particles;

	bool _active;
	bool active {
		get {
			return _active;
		}
		set {
			if (value != _active) {
				renderer.material = value ? activeMaterial : inactiveMaterial;
				if (value)
					particles.Play();
				else
					particles.Stop();
			}
			_active = value;
		}
	}

	public float requestedMineralDelta { get { return mineSpeed; } }

	public void ProcessMinerals(float mineralDelta) {

	}

	void Awake() {
		renderer = GetComponent<Renderer>();
		particles = GetComponent<ParticleSystem>();
	}

	void Update() {
		if (nodes == null) {
			nodes = new HashSet<MineralNode>();
			Connect();
		}
		active = building.isConnected;
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

	public override bool PlacingValid() {
		return Physics.CheckSphere(transform.position, range, mineralMask);
	}
}
