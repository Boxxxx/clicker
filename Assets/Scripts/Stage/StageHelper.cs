using UnityEngine;
using System.Collections;

namespace Clicker {

	public class StageHelper {
		public static Vector3 WorldPointToUI(Vector3 worldPoint) {
			int hw = Screen.width / 2;
			int hh = Screen.height / 2;
			Vector3 midP = Camera.main.WorldToScreenPoint(worldPoint) - new Vector3(hw, hh, 0);
			return new Vector3(midP.x / Screen.width * GameConsts.UIScreenWidth,
				midP.y / Screen.height * GameConsts.UIScreenHeight,
				0);
		}
	}

}