using UnityEngine;

public class ResourceStorageModule : BuildingModule, IResourceStorage {

	public float maxMinerals;
	public float maxEnergy;

	public float mineralCapacity { get { return maxMinerals; } }
	public float energyCapacity { get { return maxEnergy; } }


}
