using UnityEngine;
using System.Collections.Generic;

//manage resources
public class Economy : SingletonBehaviour<Economy> {

	public Resource.Database resources { get; private set; }

	public Resource mineral { get { return resources[Resource.Type.Mineral]; } }
	public Resource energy { get { return resources[Resource.Type.Energy]; } }

	void Awake() {
		resources = new Resource.Database();
		resources[Resource.Type.Mineral] = new Resource();
		resources[Resource.Type.Energy] = new Resource();
	}

	void Update() {
		resources.Update();
	}

}

[System.Serializable]
public class Resource {

	public float capacity { get; private set; }
	public float count { get; set; }

	public enum Type { Mineral, Energy }

	public class Usages : Dictionary<Type, float> {}

	public class Database : Dictionary<Type, Resource> {

		HashSet<IWorker> workers;

		public Database() : base() {
			workers = new HashSet<IWorker>();
		}

		public void Update() {
			workers.Clear();
			foreach (Resource resource in this.Values) {
				resource.capacity = 0;
			}
			//find all resource workers on the grid
			foreach (Building building in Building.grid) {
				workers.Add(building);
				if (building.isActive) {
					foreach (BuildingModule component in building.modules) {
						if (component is IWorker) {
							IWorker user = (IWorker) component;
							workers.Add(user);
						}
						//while were at it, tally up storage capacities
						if (component is IStorage) {
							IStorage resourceStorage = (IStorage) component;
							foreach (Type type in this.Keys) {
								this[type].capacity += resourceStorage.GetResourceCapacity(type);
							}
						}
					}
				}
			}
		}

	}


}

public interface IWorker {

	float GetResourceRate(Resource.Type type);

	void UseResources(Resource.Usages rates);

}

public interface IStorage {

	float GetResourceCapacity(Resource.Type type);

}
