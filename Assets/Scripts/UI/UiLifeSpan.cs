using UnityEngine;
using System.Collections;

namespace Clicker {

	public class UiLifeSpan : MonoBehaviour {

		public UISprite bar;
		public UILabel text;

		int barMaxLength;
		float percent;

		public void SetLifeSpanPercent(float per) {
			percent = per;
			Refresh();
		}

		public void Refresh() {
			bar.width = (int)(barMaxLength * percent);
			text.text = string.Format("Life Span: {0:0.0}%", percent * 100);
		}

		void Awake() {
			barMaxLength = bar.width;
		}
	}

}