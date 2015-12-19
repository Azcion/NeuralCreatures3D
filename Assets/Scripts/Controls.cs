using UnityEngine;

public class Controls : MonoBehaviour {

	public GameObject Camera;

	void Update () {
		if (Input.GetKeyUp("escape")) {
			Application.Quit();
		}
		if (Input.GetKeyUp("r")) {
			Container.ResetAllDead();
		}
		if (Input.GetKey("left")) {
			Camera.transform.RotateAround(new Vector3(0, 5, 0), Vector3.up, 1);
		}
		if (Input.GetKey("right")) {
			Camera.transform.RotateAround(new Vector3(0, 5, 0), Vector3.up, -1);
		}

		Camera.transform.RotateAround(new Vector3(0, 5, 0), Vector3.up, .01f);
	}

}