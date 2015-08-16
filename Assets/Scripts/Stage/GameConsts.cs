using UnityEngine;
using System.Collections;

namespace Clicker {

	public class GameConsts {
		public static float ScreenWidth { get { return ScreenHeight * Screen.width / Screen.height; } }
		public static float ScreenHeight { get { return 2.0f; } }
	}

}