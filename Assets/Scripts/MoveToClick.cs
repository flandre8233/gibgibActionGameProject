using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToClick : MonoBehaviour {
	private Ray _ray;
	private RaycastHit Hit;
	public Transform OBJ;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out Hit)) {
				OBJ.position = Hit.point;
			}
		}
	}
}
