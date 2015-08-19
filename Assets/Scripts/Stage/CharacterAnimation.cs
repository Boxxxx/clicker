using UnityEngine;
using System.Collections;

namespace Clicker {

	public class CharacterAnimation : MonoBehaviour {

		public Animation anime;
		string[] animations;

		// Use this for initialization
		void Start() {
			animations = new string[8] { "Idle", "Run", "RunLeft", "RunRight", "Damage", "Die", "ATK1", "ATK2" };
		}

		// Update is called once per frame
		void Update() {

		}

		void OnGUI() {
			return;
			int cnt = 0;
			foreach (var clipName in animations) {
				if (GUI.Button(new Rect(0, cnt * 50, 100, 50), clipName)) {
					anime.CrossFade(clipName);
				}
				cnt++;
			}
		}
	}

}