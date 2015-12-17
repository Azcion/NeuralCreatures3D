using UnityEngine;

public class Controls : MonoBehaviour {

	void Update () {
		if (Input.GetKeyUp("escape")) {
			Application.Quit();
		}
		if (Input.GetKeyUp("r")) {
			Container.ResetAllDead();
		}
	}

}