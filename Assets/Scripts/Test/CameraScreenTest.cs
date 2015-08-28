using UnityEngine;
using System.Collections;

public class CameraScreenTest : MonoBehaviour {

	public void OnGUI() {
		GUI.Label(new Rect(0, 0, 200, 100), Camera.main.WorldToScreenPoint(transform.position).ToString());
	}

}
