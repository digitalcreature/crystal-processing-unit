using UnityEngine;
using System.Collections.Generic;

//manage resources
public class Economy : SingletonBehaviour<Economy> {

	HashSet<IResourceUser> users;

	public float mineralCount { get; set; }
	public float mineralCapacity { get; private set; }
	public float mineralUsage { get; private set; }
	public float mineralDelta { get { return -mineralUsage * Time.deltaTime; } }


	public float energyCount { get; set; }
	public float energyCapacity { get; private set; }
	public float energyUsage { get; private set; }
	public float energyDelta { get { return -energyUsage * Time.deltaTime; } }

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
						IResourceStorage mineralStorage = (IResourceStorage) component;
						mineralCapacity += mineralStorage.mineralCapacity;
					}
				}
			}
		}
		foreach (IResourceUser user in users) {
			mineralUsage += user.mineralUsage;
			energyUsage += user.energyUsage;
			user.UseResources(user.mineralUsage * -Time.deltaTime, user.energyUsage * -Time.deltaTime);
		}
		mineralCount += mineralDelta;
		energyCount += energyDelta;
	}


}

public interface IResourceUser {

	float mineralUsage { get; }
	float energyUsage { get; }

	void UseResources(float mineralDelta, float energyDelta);

}

public interface IResourceStorage {

	float mineralCapacity { get; }
	float energyCapacity { get; }

}
