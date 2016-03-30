using UnityEngine;

public class ResourceStorageModule : BuildingModule, IStorage {

	public float mineralCapacity;
	public float energyCapacity;

	public float GetResourceCapacity(Resource.Type type) {
		switch (type) {
			case Resource.Type.Mineral:
				return mineralCapacity;
			case Resource.Type.Energy:
				return energyCapacity;
		}
		return 0;
	}


}
