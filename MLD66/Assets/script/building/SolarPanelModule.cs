using UnityEngine;

public class SolarPanelModule : BuildingModule, IResourceUser {

	public float energyRate = 0.5f;

	public float GetMineralUsage() { return 0; }
	public float GetEnergyUsage() { return -energyRate; }

	public void UseResources(ref float mineralUsage, ref float energyUsage) {
		//dont really do much here tbh
	}

}
