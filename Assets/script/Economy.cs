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
	public float rate { get { return gain + loss; } }
	public float delta { get { return rate * Time.deltaTime; } }

	public enum Type { Mineral, Energy }

	public class Rates : Dictionary<Type, float> {

		public float mineral { get { return this[Type.Mineral]; } set { this[Type.Mineral] = value; } }
		public float energy { get { return this[Type.Energy]; } set { this[Type.Energy] = value; } }

	}

	public class Database : Dictionary<Type, Resource> {

		HashSet<IWorker> workers;
		Rates rates;

		public Database() : base() {
			workers = new HashSet<IWorker>();
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
				//reset global usage rates this frame
				resource.gain = 0;
				resource.loss = 0;
			}
			foreach (IWorker worker in workers) {
				//find out how much of each resource the worker needs
				rates.Clear();
				bool canWork = true;	//this flag is false if there isnt enough of the required resources for this worker to work this frame
				foreach (Type type in Keys) {
					Resource resource = this[type];
					float rate = worker.GetResourceRate(type);
					rates[type] = rate;
					if (resource.count + (rate * Time.deltaTime) < 0) {
						canWork = false;
						break;
					}
				}
				if (canWork) {
					//if we have the resources we need, do the work
					foreach (Type type in Keys) {
						//first update the global usage rates of the resources being used
						float rate = rates[type];
						if (rate > 0)
							this[type].gain += rate;
						else
							this[type].loss += rate;
					}
					//do the work
					worker.Work(rates);
					foreach (Type type in Keys) {
						//update the resources based on rates
						Resource resource = this[type];
						resource.count += rates[type] * Time.deltaTime;
						resource.count = Mathf.Clamp(resource.count, 0, resource.capacity);
					}
				}
			}
		}

	}


}

public interface IWorker {

	//get the usage rate for a resource
	//called once per resource each frame
	float GetResourceRate(Resource.Type type);

	//do the work using a certain allocation of resources
	//called each frame if there are enough resources available
	void Work(Resource.Rates rates);

}

public interface IStorage {

	//return the capacity of this storage unit
	//called each frame (storage units can be dynamic)
	float GetResourceCapacity(Resource.Type type);

}
