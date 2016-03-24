using UnityEngine;

public class MiningComponent : BuildingComponent, IMineralMachine {

	public float mineSpeed = 5;

	public float requestedMineralDelta { get { return mineSpeed; } }

	public void ProcessMinerals(float mineralDelta) {

	}
}
