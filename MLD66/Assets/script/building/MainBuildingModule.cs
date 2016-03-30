using UnityEngine;

public class MainBuildingModule : BuildingModule {

	public float startingMineralCount;
	public float startingEnergyCount;

	public override void Activate() {
		Economy eco = Economy.main;
		eco.mineral.count += startingMineralCount;
		eco.energy.count += startingEnergyCount;
	}

}
