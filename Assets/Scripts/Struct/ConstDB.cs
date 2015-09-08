using UnityEngine;
using Utils;
using System.Collections;
using System.Collections.Generic;

namespace Clicker.DB {

	public class ConstDB {

		private static string DATABASE_PATH = "Data/database";

		private static ConstDB instance;
		public static ConstDB Instance {
			get {
				if (instance == null) {
					instance = new ConstDB();
				}
				return instance;
			}
		}

		private DBRoot root = new DBRoot();

		private ConstDB() {

		}
		
		/// <summary>
		/// Load Database from local file.
		/// </summary>
		public void LoadDatabase(string databasePath = "") {
			if (string.IsNullOrEmpty(databasePath)) {
				databasePath = DATABASE_PATH;
			}
			var dbRaw = Resources.Load<TextAsset>(databasePath);
			MiniJSON.Json.Deserialize(dbRaw.text, root);
		}

		public DBMonster GetMonsterById(string id) {
			if (root.monster.ContainsKey(id)) {
				return root.monster[id];
			}
			return null;
		}

        public string[] GetAllMonsterIds() {
            return root.monster.Keys.ToArray();
        }

		public DBPropertyLevel GetAtkProperty(int level) {
			return root.character.atkLevels[level];
		}

		public DBPropertyLevel GetDefProperty(int level) {
			return root.character.defLevels[level];
		}

		public int GetCharPriority() {
			return root.character.priority;
		}

		public float GetCharLifeTime() {
			return root.character.lifeTime;
		}

		public double GetCharDoubleHitPossibility() {
			return root.character.doubleHitPossibility;
		}

		public int GetAtkMaxLevel() {
			return root.character.atkLevels.Count - 1;
		}

		public int GetDefMaxLevel() {
			return root.character.defLevels.Count - 1;
		}
		
		public int GetAtkToNextLevelGold(int level) {
			return root.cost.atkLevelUp[level];
		}

		public int GetDefToNextLevelGold(int level) {
			return root.cost.defLevelUp[level];
		}

		public int GetLifeSpanRestoreGold() {
			return root.cost.lifeSpanRestore;
		}

		public int GetPurchasePotionGold() {
			return root.cost.potion;
		}

	}

	public class DBRoot {
		public Dictionary<string, DBMonster> monster = new Dictionary<string, DBMonster>();
		public DBCharacter character = new DBCharacter();
		public DBCost cost = new DBCost();
	}

	public class DBMonster {
		public string id = "-1";
		public int hp = 0;
		public int atk = 0;
		public string name = "None";
		public int priority = 0;
		public double doubleHitPossibility = 0;
		public int goldDrop = 0;
	}

	public class DBCharacter {
		public List<DBPropertyLevel> atkLevels = new List<DBPropertyLevel>();
		public List<DBPropertyLevel> defLevels = new List<DBPropertyLevel>();
		public int priority = 0;
		public double doubleHitPossibility = 0;
		public float lifeTime = 0.0f;
	}

	public class DBCost {
		public List<int> atkLevelUp = new List<int>();
		public List<int> defLevelUp = new List<int>();
		public int lifeSpanRestore = 0;
		public int potion = 0;
	}

	public class DBPropertyLevel {
		public int level = 0;
		public int value = 0;
	}

}
