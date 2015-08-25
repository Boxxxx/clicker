using UnityEngine;
using System.Collections;

namespace Clicker {

	public class GameConsts : MonoBehaviour {
		public static float ScreenWidth { get { return ScreenHeight * Screen.width / Screen.height; } }
		public static float ScreenHeight { get { return 2.0f; } }

		public float characterMoveSpeed;
		public static float CharacterMoveSpeed { get { return Inst.characterMoveSpeed; } }
		public float monsterYOffset;
		public static float MonsterYOffset { get { return Inst.monsterYOffset; } }

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