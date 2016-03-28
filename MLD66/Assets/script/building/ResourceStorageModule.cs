using UnityEngine;

public class ResourceStorageModule : BuildingModule, IResourceStorage {

	public float mineralCapacity;
	public float energyCapacity;

	public float GetMineralCapacity() { return mineralCapacity; }
	public float GetEnergyCapacity() { return energyCapacity; }


}
