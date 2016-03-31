using UnityEngine;

public class SolarPanelModule : BuildingModule, IWorker {

	public float energyRate = 0.5f;

	public float GetResourceRate(Resource.Type type) {
		if (type == Resource.Type.Energy) {
			return -energyRate;
		}
		else {
			return 0;
		}
	}

	public void Work(Resource.Usages rates) {
		//dont really do much here tbh
	}

}
