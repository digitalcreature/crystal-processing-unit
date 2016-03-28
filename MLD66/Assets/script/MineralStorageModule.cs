using UnityEngine;

public class MineralStorageModule : BuildingModule, IMineralStorage {

	public float capacity;

	public float mineralCapacity { get { return capacity; } }

}
