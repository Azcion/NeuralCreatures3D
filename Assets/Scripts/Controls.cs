using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour {

	void Update () {
		if (Input.GetKey("escape")) {
			Application.Quit();
		}
	}

}