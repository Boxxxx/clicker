using UnityEngine;

namespace Clicker {
    public class DirectorTest : MonoBehaviour {
        public int numOfRegionsPerClick = 10;

        void OnGUI() {
            if (GUI.Button(new Rect(100, 100, 300, 50), "generate next regions")) {
                var regions = Director.Instance.NextRegions(numOfRegionsPerClick);

                bool firstFlag = true;
                string outputStr = "[";
                for (int i = 0; i < numOfRegionsPerClick; i++) {
                    if (firstFlag) {
                        firstFlag = false;
                    }
                    else {
                        outputStr += ", ";
                    }
                    outputStr += regions[i].Second.ToString();
                }
                outputStr += "]";
                Debug.Log(outputStr);
            }
        }
    }
}
