using UnityEngine;
using System.Collections;
using Utils;

public class TestUnitys : MonoBehaviour {
    int _frameCnt = 0;

	void Start () {
        int cnt = 0;
        Unitys.InvokeRepeating(this, () => {
            Debugs.Log("Repeat loop! {0}", cnt);
            return ++cnt < 5;
        }, 5f, 1f);
        this.AfterPhysics(() => Debugs.Log("Before Physics"));
        this.NextTick(() => Debugs.Log("Next tick"));
        this.SendMessage("TestMessage", SendMessageOptions.RequireReceiver, "testMessage", 123);
	}

    void Update() {
        ++_frameCnt;
        if (_frameCnt <= 2) {
            Debugs.Log("Update frame{0}", _frameCnt);
        }
	}

    void FixedUpdate() {
        if (_frameCnt <= 2) {
            Debugs.Log("FixedUpdate frame{0}", _frameCnt);
        }
    }

    void TestMessage(string arg1, int arg2) {
        Debugs.Log(arg1 + ": " + arg2);
    }
}
