using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
	private bool switchedDirection = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x <= -9) {
			switchedDirection = true;
		}
		if (transform.position.x >= 8) {
			switchedDirection = false;
		}
		if (switchedDirection == true) {
			transform.Translate (Vector3.right * Time.deltaTime);
		}
		if (switchedDirection == false) {
			transform.Translate (Vector3.left * Time.deltaTime);
		}
	}
}
