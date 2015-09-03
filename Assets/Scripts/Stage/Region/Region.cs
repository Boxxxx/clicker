using UnityEngine;
using System.Collections;
using Box;

namespace Clicker {

	public abstract class Region : ReusableObject {
		public RegionType type;
		[Tooltip("The scale comparing to ScreenWidth")]
		public float lengthScale = 1.0f;
		[Tooltip("A value from 0 to 1 indicating the key point position in region")]
		public float keyPointPosition = 1.0f;

		public float Length { get { return GameConsts.ScreenWorldWidth * lengthScale; } }
		public float KeyPointOffset { get { return Length * keyPointPosition; } }

		public TextMesh text;
		public BoxCollider clickArea;
		public StageController stageController;

		public abstract void Reset(RegionMeta meta, StageController stageController);
		public virtual void KeyPointAction() {
			stageController.GoNextRegion();
		}
		public virtual void RegionUpdate() { }
	}

	public class RegionMeta {
		public RegionType type;
		public int date;
		/// <summary>
		/// Only used when type = Monster
		/// </summary>
		public MonsterDataInst monsterInfo;

        public override string ToString() {
            return string.Format("({0}, {1}, {2})", date, type, monsterInfo == null ? "none" : monsterInfo.raw.name);
        }
    }

	public enum RegionType {
		Battle,
		BlackSmith,
		ArmorSmith,
		Tarven,
		PotionShop,
		StockMarket,
		DivineRelic
	}

}