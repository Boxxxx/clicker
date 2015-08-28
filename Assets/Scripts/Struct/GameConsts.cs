using UnityEngine;
using System.Collections;

namespace Clicker {

	public class GameConsts : MonoBehaviour {
		public static float ScreenWorldWidth { get { return ScreenWorldHeight * Screen.width / Screen.height; } }
		public static float ScreenWorldHeight { get { return 2.0f; } }

		public float characterMoveSpeed;
		public static float CharacterMoveSpeed { get { return Inst.characterMoveSpeed; } }
		public float monsterYOffset;
		public static float MonsterYOffset { get { return Inst.monsterYOffset; } }

		public static int UIScreenWidth = 1920;
		public static int UIScreenHeight = 1080;

		private static GameConsts instance;
		public static GameConsts Inst {
			get {
				if (instance != null) {
					return instance;
				}
				instance = GameObject.FindObjectOfType<GameConsts>();
				if (instance != null) {
					return instance;
				}
				Debug.LogError("Cannot find Game config consts. Try to add a GameConsts object into the scene.");
				return null;
			}
		}
	}

}