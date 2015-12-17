using UnityEngine;
using Random = UnityEngine.Random;

public class ButterflyAnimator : MonoBehaviour {

	public GameObject Butterfly;
	public GameObject Body;
	public GameObject RT;
	public GameObject RB;
	public GameObject LT;
	public GameObject LB;

	private int _dir;
	private int _i;

	void Start () {
		_dir = 1;
		_i = 0;
	}

	void Update () {
		++_i;
		if (_i >= 25) {
			_dir = -_dir;
			_i = 0;

			return;
		}

		Butterfly.transform.Translate(0, 0, -.01f);

		float angle = 425 * Time.deltaTime * _dir;
		RT.transform.RotateAround(Body.transform.position, Butterfly.transform.forward, angle);
		RB.transform.RotateAround(Body.transform.position, Butterfly.transform.forward, angle);
		LT.transform.RotateAround(Body.transform.position, Butterfly.transform.forward, -angle);
		LB.transform.RotateAround(Body.transform.position, Butterfly.transform.forward, -angle);

		ClampPosition();
	}

	private void ClampPosition () {
		int x = 0;
		int y = 0;
		int z = 0;

		if (Butterfly.transform.position.x > 4.5) {
			x = 45;
		} else if (Butterfly.transform.position.x < -4.5) {
			x = -45;
		}
		if (Butterfly.transform.position.y > 9.5) {
			y = 95;
		} else if (Butterfly.transform.position.y < .5) {
			y = 5;
		}
		if (Butterfly.transform.position.z > 4.5) {
			z = 45;
		} else if (Butterfly.transform.position.z < -4.5) {
			z = -45;
		}

		Vector3 clamped = new Vector3(x == 0 ? Butterfly.transform.position.x : x / 10f,
		                              y == 0 ? Butterfly.transform.position.y : y / 10f,
		                              z == 0 ? Butterfly.transform.position.z : z / 10f);
		Butterfly.transform.position = clamped;
	}

}