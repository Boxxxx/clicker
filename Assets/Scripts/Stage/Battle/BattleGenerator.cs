using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Clicker.DB;

namespace Clicker {

	public class BattleGenerator {

		List<BattleRecord> records = new List<BattleRecord>();
		CharacterDataInst charData;
		MonsterDataInst monData;

		int nowTurn;
		int turnCnt;
		bool isInDoubleHit;

		public BattleGenerator(CharacterDataInst charData, MonsterDataInst monData) {
			this.charData = new CharacterDataInst(charData);
			this.monData = new MonsterDataInst(monData);

			Begin();
		}

		public void Begin() {
			nowTurn = 0;
			// Decide who's first turn
			double firstTurnProbability =
				(double)ConstDB.Instance.GetCharPriority() / (ConstDB.Instance.GetCharPriority() + monData.raw.priority);
			if (Random.value > firstTurnProbability) {
				nowTurn = 1;
			}

			turnCnt = 0;
			isInDoubleHit = false;
		}

		public BattleRecord GenerateNext() {
			var record = new BattleRecord();
			turnCnt++;
			record.turn = turnCnt;

			if (isInDoubleHit) {
				if (nowTurn == 0) {
					monData.hp -= charData.atk;
					record.damage = charData.atk;
					record.recordType = BattleRecord.RecordType.OurAtk;
                } else {
					charData.hp -= monData.Atk;
					record.damage = monData.Atk;
					record.recordType = BattleRecord.RecordType.EnemyAtk;
				}
				isInDoubleHit = false;
				nowTurn = 1 - nowTurn;

			} else {
				float randValue = Random.value;
				if (nowTurn == 0) {
					if (randValue <= ConstDB.Instance.GetCharDoubleHitPossibility()) {
						isInDoubleHit = true;
					} else {
						nowTurn = 1 - nowTurn;
					}
					monData.hp -= charData.atk;
					record.damage = charData.atk;
					record.recordType = BattleRecord.RecordType.OurAtk;
				} else {
					if (randValue <= monData.raw.doubleHitPossibility) {
						isInDoubleHit = true;
					} else {
						nowTurn = 1 - nowTurn;
					}
					charData.hp -= monData.Atk;
					record.damage = monData.Atk;
					record.recordType = BattleRecord.RecordType.EnemyAtk;
				}
			}

			if (monData.hp <= 0) {
				record.recordType = BattleRecord.RecordType.Win;
			} else if (charData.hp <= 0) {
				record.recordType = BattleRecord.RecordType.Lose;
			}

			record.charHp = charData.hp;
			record.monHp = monData.hp;

			return record;
		}

	}

	public class BattleRecord {
		public enum RecordType { OurAtk, EnemyAtk, Win, Lose }

		public int turn;
		public RecordType recordType;
		public int damage;
		public int charHp;
		public int monHp;
	}

}