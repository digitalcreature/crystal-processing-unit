using UnityEngine;

public class MainBuilding : Building {

	public override bool PositionValid(Vector3 position) {
		return PositionUnobstucted(position);
	}

}
