using UnityEngine;
using System.Collections.Generic;

//manage resources
public class Economy : SingletonBehaviour<Economy> {

	public float mineralCount { get; set; }
	public float mineralCapacity { get; private set; }
	public float mineralUsage { get; private set; }
	float mineralDelta { get { return -mineralUsage * Time.deltaTime; } }
	float mineralUsageFactor { get { return (mineralCount + mineralDelta) < 0 ? (mineralCount / -mineralDelta) : 1; } }

	public float energyCount { get; set; }
	public float energyCapacity { get; private set; }
	public float energyUsage { get; private set; }
	float energyDelta { get { return -energyUsage * Time.deltaTime; } }
	float energyUsageFactor { get { return (energyCount + energyDelta) < 0 ? (energyCount / -energyDelta) : 1; } }

	HashSet<IResourceUser> users;

	void Awake() {
		users = new HashSet<IResourceUser>();
	}

	void Update() {
		mineralCapacity = 0;
		energyCapacity = 0;
		users.Clear();
		//find all resource users on the grid
		foreach (Building building in Building.grid) {
			users.Add(building);
			if (building.isActive) {
				foreach (BuildingModule component in building.modules) {
					if (component is IResourceUser) {
						IResourceUser user = (IResourceUser) component;
						users.Add(user);
					}
					//while were at it, tally up storage capacities
					if (component is IResourceStorage) {
						IResourceStorage resourceStorage = (IResourceStorage) component;
						mineralCapacity += resourceStorage.GetMineralCapacity();
						energyCapacity += resourceStorage.GetEnergyCapacity();
					}
				}
			}
		}
		//collect net usages
		mineralUsage = 0;
		energyUsage = 0;
		foreach (IResourceUser user in users) {
			//we need to do this in a seperate loop from the actual resource usage so that usageFactors are correct
			mineralUsage += user.GetMineralUsage();
			energyUsage += user.GetEnergyUsage();
		}
		//note concerning usage factors: if there is not enough stored resource to satisfy demand, usage
		//must be scaled accordingly. this number is 0 if there is no resource available, 1 if all demand can be met,
		//0.5 if only half of the demand can be met, etc
		foreach (IResourceUser user in users) {
			//what the user said it would use, scaled by usage factor
			float userMineralUsageEstimate = user.GetMineralUsage() * mineralUsageFactor;
			float userEnergyUsageEstimate = user.GetEnergyUsage() * energyUsageFactor;
			//what the user actually used
			float userMineralUsage = userMineralUsageEstimate;
			float userEnergyUsage = userEnergyUsageEstimate;
			//note concerning usage adjustment: because a user's use of one resource may depend on the other,
			//its allotted resource usages must be passed by reference so that global usages can be adjusted
			user.UseResources(ref userMineralUsage, ref userEnergyUsage);
			//adjust global usages according to what the user actually used
			mineralUsage = mineralUsage - userMineralUsageEstimate + userMineralUsage;
			energyUsage = energyUsage - userEnergyUsageEstimate + userEnergyUsage;
		}
		//increment stored resource counts
		mineralCount += mineralDelta;
		energyCount += energyDelta;
		//clamp counts to caps
		mineralCount = Mathf.Clamp(mineralCount, 0, mineralCapacity);
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
