using UnityEngine;
using System.Collections.Generic;

//manage resources
public class Economy : SingletonBehaviour<Economy> {


	public float initialMineralCount;

	HashSet<IMineralMachine> mineralMachines;

	public float mineralCount { get; private set; }
	public float mineralCapacity { get; private set; }

	void Awake() {
		mineralMachines = new HashSet<IMineralMachine>();
		mineralCount = initialMineralCount;
	}

	void Update() {
		float mineralDelta = 0;
		mineralCapacity = 0;
		mineralMachines.Clear();
		foreach (Building building in Building.grid) {
			mineralMachines.Add(building);
			if (building.isActive) {
				foreach (BuildingModule component in building.modules) {
					if (component is IMineralMachine) {
						IMineralMachine mineralMachine = (IMineralMachine) component;
						mineralMachines.Add(mineralMachine);
					}
					if (component is IMineralStorage) {
						IMineralStorage mineralStorage = (IMineralStorage) component;
						mineralCapacity += mineralStorage.mineralCapacity;
					}
				}
			}
		}
		foreach (IMineralMachine mineralMachine in mineralMachines) {
			float requestedMineralDelta = mineralMachine.requestedMineralDelta;
			mineralDelta += requestedMineralDelta;
			mineralMachine.ProcessMinerals(requestedMineralDelta);
		}
		mineralCount += mineralDelta * Time.deltaTime;
	}


}

public interface IMineralMachine {

	float requestedMineralDelta { get; }

	void ProcessMinerals(float mineralDelta);

}

public interface IMineralStorage {

	float mineralCapacity { get; }

}
