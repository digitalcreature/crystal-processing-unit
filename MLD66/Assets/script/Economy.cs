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
	public float delta { get { return rate * Time.deltaTime; } }
	public float lossFactor {
		get {
			if (gain + loss < 0 && count < 0)
				return gain / -loss;
			else
				return 1;
		}
	}

	public enum Type { Mineral, Energy }

	public class Rates : Dictionary<Type, float> {

		public float mineral { get { return this[Type.Mineral]; } set { this[Type.Mineral] = value; } }
		public float energy { get { return this[Type.Energy]; } set { this[Type.Energy] = value; } }

	}

	public class Database : Dictionary<Type, Resource> {

		HashSet<IWorker> workers;
		Rates expectedRates;
		Rates rates;

		public Database() : base() {
			workers = new HashSet<IWorker>();
			expectedRates = new Rates();
			rates = new Rates();
		}

		public void Update() {
			workers.Clear();
			foreach (Resource resource in Values) {
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
							foreach (Type type in Keys) {
								this[type].capacity += storage.GetResourceCapacity(type);
							}
						}
					}
				}
			}
			foreach (Resource resource in Values) {
				resource.gain = 0;
				resource.loss = 0;
			}
			foreach (IWorker worker in workers) {
				foreach (Type type in Keys) {
					Resource resource = this[type];
					float workerRate = worker.GetResourceRate(type);
					if (workerRate > 0) resource.gain += workerRate;
					if (workerRate < 0) resource.loss += workerRate;
				}
			}
			rates.Clear();
			expectedRates.Clear();
			foreach (IWorker worker in workers) {
				foreach (Type type in Keys) {
					expectedRates[type] = worker.GetResourceRate(type);
					rates[type] = expectedRates[type];
				}
				worker.Work(rates);
				foreach (Type type in rates.Keys) {
					if (ContainsKey(type)) {
						Resource resource = this[type];
						float expectedRate = expectedRates[type];
						float rate = rates[type];
						if (expectedRate > 0) resource.gain -= expectedRate;
						if (expectedRate < 0) resource.loss -= expectedRate;
						if (rate > 0) resource.gain += rate;
						if (rate < 0) resource.loss += rate;
					}
				}
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

	void Work(Resource.Rates rates);

}

public interface IStorage {

	float GetResourceCapacity(Resource.Type type);

}
