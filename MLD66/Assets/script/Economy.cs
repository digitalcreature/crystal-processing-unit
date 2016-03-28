using UnityEngine;
using System.Collections.Generic;

public class Economy : SingletonBehaviour<Economy> {

	public float minerals;

	HashSet<IMineralMachine> _mineralMachines;
	HashSet<IMineralMachine> mineralMachines { get { if (_mineralMachines == null) _mineralMachines = new HashSet<IMineralMachine>(); return _mineralMachines; } }

	void Update() {
		float mineralDelta = 0;
		mineralMachines.Clear();
		foreach (Building building in Building.grid) {
			mineralMachines.Add(building);
			foreach (BuildingModule component in building.modules) {
				if (component is IMineralMachine) {
					IMineralMachine mineralMachine = (IMineralMachine) component;
					mineralMachines.Add(mineralMachine);
				}
			}
		}
		foreach (IMineralMachine mineralMachine in mineralMachines) {
			float requestedMineralDelta = mineralMachine.requestedMineralDelta;
			mineralDelta += requestedMineralDelta;
			mineralMachine.ProcessMinerals(requestedMineralDelta);
		}
		minerals += mineralDelta * Time.deltaTime;
	}


}

public interface IMineralMachine {

	float requestedMineralDelta { get; }

	void ProcessMinerals(float mineralDelta);

}
