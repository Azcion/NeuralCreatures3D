using UnityEngine;

public class ObstManager : MonoBehaviour {

	public GameObject Parent;
	public Material Material;

	void Start () {
		for (int i = 0; i < 50; ++i) {
			GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);
			item.transform.parent = Parent.transform;
			item.transform.position = new Vector3(Random.Range(Container.Dimensions[0], Container.Dimensions[1]),
												  Random.Range(Container.Dimensions[2], Container.Dimensions[3]),
												  Random.Range(Container.Dimensions[4], Container.Dimensions[5]));
			item.transform.localScale = new Vector3(.25f, .25f, .25f);
			item.GetComponent<Renderer>().material = Material;
			item.isStatic = true;
			
			Container.ObstList.Add(item);
		}
	}

	void Update () {
		foreach (GameObject item in Container.ObstList) {
			item.transform.Rotate(new Vector3(-15, -30, -45) * Time.deltaTime);
		}
	}

}