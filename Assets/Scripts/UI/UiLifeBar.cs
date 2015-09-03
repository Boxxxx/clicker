using UnityEngine;
using System.Collections;

namespace Clicker {

	public class UiLifeBar : MonoBehaviour {

		public UISprite bar;
		public UILabel text;

		int maxHp;
		int hp;
		int barMaxLength;

		public void SetHp(int hp, int maxHp) {
			this.hp = hp;
			this.maxHp = maxHp;
			Refresh();
		}

		public void Refresh() {
			text.text = string.Format("{0}/{1}", hp, maxHp);
			bar.width = (int)((float)hp / maxHp * barMaxLength);
		}

		// Use this for initialization
		void Awake() {
			barMaxLength = bar.width;
		}

		// Update is called once per frame
		void Update() {

		}
	}

}