using UnityEngine;

public class MineralNode : MonoBehaviour {

	public GameObject minerConnection;

	MiningModule _miner;
	public MiningModule miner {
		get {
			return _miner;
		}
		set {
			if (_miner != value) {
				_miner = value;
				if (value) {
					Vector3 scale = minerConnection.transform.localScale;
					minerConnection.transform.SetParent(null, true);
					minerConnection.SetActive(true);
					Vector3 a = minerConnection.transform.position;
					Vector3 b = value.transform.position;
					minerConnection.transform.position = (a + b) / 2;
					scale.z *= (a - b).magnitude;
					minerConnection.transform.localScale = scale;
					minerConnection.transform.forward = b - a;
				}
				else {
					minerConnection.SetActive(false);
				}
			}
		}
	}

	public bool isMined { get { return miner != null; } }

}
