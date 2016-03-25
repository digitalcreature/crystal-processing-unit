using UnityEngine;
using System.Collections.Generic;

public class MiningModule : BuildingModule, IMineralMachine {

	public float mineSpeed = 5;
	public float range = 1;
	public LayerMask mineralMask;

	public HashSet<MineralNode> nodes { get; private set; }

	public float requestedMineralDelta { get { return mineSpeed; } }

	public void ProcessMinerals(float mineralDelta) {

	}

	void Update() {
		if (nodes == null) {
			nodes = new HashSet<MineralNode>();
			foreach (Collider collider in Physics.OverlapSphere(transform.position, range, mineralMask)) {
				MineralNode node = collider.GetComponent<MineralNode>();
				if (node != null) {
					nodes.Add(node);
				}
			}
			Debug.LogFormat("Connected to {0} mineral nodes", nodes.Count);
		}
	}

	public override bool PlacingValid() {
		return Physics.CheckSphere(transform.position, range, mineralMask);
	}
}
