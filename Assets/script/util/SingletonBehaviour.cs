using UnityEngine;

public class SingletonBehaviour<S> : MonoBehaviour where S : SingletonBehaviour<S> {

	static S _main;
	public static S main {
		get {
			if (_main == null) {
				_main = FindObjectOfType<S>();
				if (_main == null) {
					Debug.LogWarning("No " + typeof(S).Name + " in scene!");
				}
			}
			return _main;
		}
	}

}
