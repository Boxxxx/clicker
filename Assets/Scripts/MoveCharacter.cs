using UnityEngine;
using System.Collections;

public class MoveCharacter : MonoBehaviour {

	public Animation knight;
	public float idleSpeedThreshold = 0.25f;
	public float moveSpeed = 0.01f;

	void Start() {
		
	}

	void Update() {
		Vector3 moveDirection = Vector3.zero;
		moveDirection.x = Input.GetAxis("Horizontal");
		moveDirection.z = Input.GetAxis("Vertical");
		if (moveDirection.magnitude < idleSpeedThreshold) {
			moveDirection = Vector3.zero;
			knight.CrossFade("Idle");
		} else {
			moveDirection.Normalize();
			knight.transform.localPosition += moveDirection * moveSpeed * Time.deltaTime; 
			knight.CrossFade("Run");
		}

	}
}
