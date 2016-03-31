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

	public float gain { get; private set; }
	public float loss { get; private set; }
	public float rate { get { return gain + (loss * lossFactor); } }
	public float delta { get { return rate * -Time.deltaTime; } }
	public float lossFactor {
		get {
			if (gain + loss < 0 && count < 0)
				return gain / -loss;
			else
				return 1;
		}
	}

	public enum Type { Mineral, Energy }

	public class Usages : Dictionary<Type, float> {}

	public class Database : Dictionary<Type, Resource> {

		HashSet<IWorker> workers;
		Usages usages;

		public Database() : base() {
			workers = new HashSet<IWorker>();
			usages = new Usages();
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
							IWorker worker = (IWorker) component;
							workers.Add(worker);
						}
						//while were at it, tally up storage capacities
						if (component is IStorage) {
							IStorage storage = (IStorage) component;
							foreach (Type type in this.Keys) {
								this[type].capacity += storage.GetResourceCapacity(type);
							}
						}
					}
				}
			}
			foreach (Resource resource in this.Values) {
				resource.gain = 0;
				resource.loss = 0;
			}
			usages.Clear();
			foreach (IWorker worker in workers) {
				foreach (Type type in Keys) {
					Resource resource = this[type];
					float workerRate = worker.GetResourceRate(type);
					if (workerRate > 0) resource.gain += workerRate;
					if (workerRate < 0) resource.loss += workerRate;
				}
			}
			foreach (IWorker worker in workers) {
				foreach (Type type in Keys) {
					usages[type] = worker.GetResourceRate(type);
				}
				worker.Work(usages);
			}
			foreach (Resource resource in Values) {
				resource.count += resource.delta;
				resource.count = Mathf.Clamp(resource.count, 0, resource.capacity);
			}
		}

	}


}

public interface IWorker {

	float GetResourceRate(Resource.Type type);

	void Work(Resource.Usages rates);

}

public interface IStorage {

	float GetResourceCapacity(Resource.Type type);

}
