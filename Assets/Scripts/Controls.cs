using UnityEngine;

public class Controls : MonoBehaviour {

	void Update () {
		if (Input.GetKeyUp("escape")) {
			Application.Quit();
		}
	}

}