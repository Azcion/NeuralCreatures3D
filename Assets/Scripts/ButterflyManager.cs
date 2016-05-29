using UnityEngine;
using Assets.Scripts.NN;

public class ButterflyManager : MonoBehaviour {

	public GameObject Parent;
	public GameObject Butterfly;

	void Start () {
		Creature.SetDimensions(Container.DIMENSIONS);

		for (int i = 0; i < 10; ++i) {
			Vector3 rot = new Vector3(Random.Range(0, 360),
			                          Random.Range(0, 360),
			                          Random.Range(0, 360));
			Creature butterfly = new Creature(Instantiate(Butterfly), rot, Parent.transform);
			Container.CreatureList.Add(butterfly);
		}

		Butterfly.SetActive(false);
	}

	void Update () {
		int deadCount = 0;

		foreach (Creature butterfly in Container.CreatureList) {
			butterfly.Update(Container.FoodList, Container.ObstList);
			if (butterfly.Dead) {
				++deadCount;
			}
		}

		if (deadCount == Container.CreatureList.Count) {
			Container.ResetAllDead();
		}
	}

}