using UnityEngine;
using System.Collections.Generic;

//manage resources
public class Economy : SingletonBehaviour<Economy> {

	public float mineralCount { get; set; }
	public float mineralCapacity { get; private set; }
	public float mineralUsage { get; private set; }
	public float mineralDelta { get { return -mineralUsage * Time.deltaTime; } }

	public float energyCount { get; set; }
	public float energyCapacity { get; private set; }
	public float energyUsage { get; private set; }
	public float energyDelta { get { return -energyUsage * Time.deltaTime; } }

	HashSet<IResourceUser> users;

	void Awake() {
		users = new HashSet<IResourceUser>();
	}

	void Update() {
		mineralUsage = 0;
		energyUsage = 0;
		mineralCapacity = 0;
		energyCapacity = 0;
		users.Clear();
		foreach (Building building in Building.grid) {
			users.Add(building);
			if (building.isActive) {
				foreach (BuildingModule component in building.modules) {
					if (component is IResourceUser) {
						IResourceUser user = (IResourceUser) component;
						users.Add(user);
					}
					if (component is IResourceStorage) {
						IResourceStorage resourceStorage = (IResourceStorage) component;
						mineralCapacity += resourceStorage.GetMineralCapacity();
						energyCapacity += resourceStorage.GetEnergyCapacity();
					}
				}
			}
		}
		//collect net usages
		foreach (IResourceUser user in users) {
			mineralUsage += user.GetMineralUsage();
			energyUsage += user.GetEnergyUsage();
		}
		float mineralUsageFactor = (mineralCount + mineralDelta) < 0 ? (mineralUsageFactor = mineralCount / - mineralDelta) : 1;
		float energyUsageFactor = (energyCount + energyDelta) < 0 ? (energyUsageFactor = energyCount / - energyDelta) : 1;
		//use resources
		mineralUsage = 0;
		energyUsage = 0;
		foreach (IResourceUser user in users) {
			float userMineralUsage = user.GetMineralUsage() * mineralUsageFactor;
			float userEnergyUsage = user.GetEnergyUsage() * energyUsageFactor;
			user.UseResources(ref userMineralUsage, ref userEnergyUsage);
			mineralUsage += userMineralUsage;
			energyUsage += userEnergyUsage;
		}
		mineralCount += mineralDelta;
		mineralCount = Mathf.Clamp(mineralCount, 0, mineralCapacity);
		energyCount += energyDelta;
		energyCount = Mathf.Clamp(energyCount, 0, energyCapacity);
	}


}

public interface IResourceUser {

	float GetMineralUsage();
	float GetEnergyUsage();

	void UseResources(ref float mineralUsage, ref float energyUsage);

}

public interface IResourceStorage {

	float GetMineralCapacity();
	float GetEnergyCapacity();

}
