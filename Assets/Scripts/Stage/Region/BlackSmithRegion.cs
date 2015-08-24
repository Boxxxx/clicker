using UnityEngine;
using System.Collections;

namespace Clicker {

	public class BlackSmithRegion : Region {

		bool isTriggered;

		public override void Reset(RegionMeta meta) {
			length = GameConsts.ScreenWidth * 2;
			clickArea.gameObject.SetActive(true);
			clickArea.transform.localPosition = new Vector3(GameConsts.ScreenWidth, 0, 0);
			clickArea.size = new Vector3(GameConsts.ScreenWidth, GameConsts.ScreenHeight, 0);
			text.text = "武器升级";
			isTriggered = false;
		}

	}
}