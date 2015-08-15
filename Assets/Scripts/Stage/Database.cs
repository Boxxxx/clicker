using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Clicker.DB {

	public class Database {

		private static string DATABASE_PATH = "Data/database";

		private static Database instance;
		public static Database Instance {
			get {
				if (instance == null) {
					instance = new Database();
				}
				return instance;
			}
		}

		private DBRoot root = new DBRoot();

		private Database() {

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
	}

	public class DBRoot {
		public Dictionary<string, DBMonster> monster;
	}


	public class DBMonster {
		public string id;
		public int hp;
		public int atk;
		public string name;
	}

}
