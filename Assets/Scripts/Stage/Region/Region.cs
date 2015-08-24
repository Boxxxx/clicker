using UnityEngine;
using System.Collections;
using Box;

namespace Clicker {

	public abstract class Region : ReusableObject {
		public RegionType type;
		public float length;

		public TextMesh text;
		public BoxCollider clickArea;

		public abstract void Reset(RegionMeta meta);
		public virtual void RegionUpdate() { }
	}

	public class RegionMeta {
		public RegionType type;
		/// <summary>
		/// Only used when type = Monster
		/// </summary>
		public MonsterDataInst monsterInfo;
		public float length;
	}

	public enum RegionType {
		Battle,
		BlackSmith
	}

}