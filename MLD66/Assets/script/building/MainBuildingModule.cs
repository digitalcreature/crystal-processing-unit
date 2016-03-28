using UnityEngine;

public class MainBuildingModule : BuildingModule {

	public float startingMinerals;
	public float startingEnergy;

	public override void Activate() {
		Economy eco = Economy.main;
		eco.mineralCount += startingMinerals;
		eco.energyCount += startingEnergy;
	}

}
