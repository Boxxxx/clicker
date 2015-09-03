using Clicker.DB;
using UnityEngine;

namespace Clicker {
    public class DirectorTest : MonoBehaviour {
        public int numOfRegionsPerClick = 10;

        void Awake() {
            ConstDB.Instance.LoadDatabase();
        }

        void OnGUI() {
            if (GUI.Button(new Rect(100, 100, 300, 50), "generate next regions")) {
                float lastTime = Time.realtimeSinceStartup;
                var regions = Director.Instance.NextRegions(numOfRegionsPerClick);
                Debug.Log("Time cost: " + (Time.realtimeSinceStartup - lastTime));

                bool firstFlag = true;
                string outputStr = "[";
                for (int i = 0; i < numOfRegionsPerClick; i++) {
                    if (firstFlag) {
                        firstFlag = false;
                    }
                    else {
                        outputStr += ", ";
                    }
                    outputStr += regions[i].ToString();
                }
                outputStr += "]";
                Debug.Log(outputStr);
            }
        }
    }
}
