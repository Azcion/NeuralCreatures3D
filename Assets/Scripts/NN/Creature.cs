using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.NN {

	public class Creature {

		private struct Dimensions {

			public static float xA = 0;
			public static float yA = 0;
			public static float zA = 0;

			public static float xB = 1;
			public static float yB = 1;
			public static float zB = 1;

		}

		public GameObject Body;

		private readonly Vector3 _sensorL;
		private readonly Vector3 _sensorR;
		private readonly Vector3 _sensorU;
		private readonly Vector3 _sensorD;

		public NeuralNetwork Brain;

		public int Age;
		public bool Dead;
		public double Life;
		public double Fitness;
		public double ParentChance;

		private double _itemProx = .5;

		public Creature (GameObject body, Vector3 rotation, Transform parent) {
			Body = body;
			Body.transform.position = new Vector3(Random.Range(Dimensions.xA, Dimensions.xB),
			                                      Random.Range(Dimensions.yA, Dimensions.yB),
			                                      Random.Range(Dimensions.zA, Dimensions.zB));
			Body.transform.Rotate(rotation);
			Body.transform.parent = parent;

			Brain = new NeuralNetwork(8, 6, 4);
			Life = 100;

			_sensorL = Body.transform.GetChild(5).transform.position;
			_sensorR = Body.transform.GetChild(6).transform.position;
			_sensorU = Body.transform.GetChild(7).transform.position;
			_sensorD = Body.transform.GetChild(8).transform.position;
		}

		public static void SetDimensions (float[] dimensions) {
			Dimensions.xA = dimensions[0];
			Dimensions.yA = dimensions[2];
			Dimensions.zA = dimensions[4];
			Dimensions.xB = dimensions[1];
			Dimensions.yB = dimensions[3];
			Dimensions.zB = dimensions[5];
		}

		public void Reset () {
			Body.SetActive(true);
			Brain = new NeuralNetwork(8, 6, 4);
			Dead = false;
			Fitness = 0;
			Life = 100;
		}

		public void Kill () {
			Body.SetActive(false);
			Dead = true;
			Fitness = 0;
			Life = 0;
			ParentChance = 0;
			Age = 0;
		}

		public void Update (List<GameObject> foodList, IList<GameObject> obstList) {
			if (!Dead && Life <= 0) {
				Kill();
				return;
			}
			if (Dead) {
				return;
			}

			GameObject closestFoodItem = GetClosestFood(foodList);
			Vector3 closestFood = closestFoodItem.transform.position;
			Vector3 closestObst = GetClosestObst(obstList).transform.position;

			double closestFoodL = Vector3.Distance(closestFood, _sensorL);
			double closestFoodR = Vector3.Distance(closestFood, _sensorR);
			double closestFoodU = Vector3.Distance(closestFood, _sensorU);
			double closestFoodD = Vector3.Distance(closestFood, _sensorD);

			double closestObstL = Vector3.Distance(closestObst, _sensorL);
			double closestObstR = Vector3.Distance(closestObst, _sensorR);
			double closestObstU = Vector3.Distance(closestObst, _sensorU);
			double closestObstD = Vector3.Distance(closestObst, _sensorD);

			double centerDistFood = Vector3.Distance(closestFood, Body.transform.position);
			double centerDistObst = Vector3.Distance(closestObst, Body.transform.position);

			if (centerDistFood < _itemProx) {
				Life += 30;
				Fitness += 10;
				closestFoodItem.transform.position = new Vector3(Random.Range(Dimensions.xA, Dimensions.xB),
				                                                 Random.Range(Dimensions.yA, Dimensions.yB),
				                                                 Random.Range(Dimensions.zA, Dimensions.zB));
			}

			Life -= .075;

			double[] foodDists = {closestFoodL, closestFoodR, closestFoodU, closestFoodD};
			double[] obstDists = {closestObstL, closestObstR, closestObstU, closestObstD};
			int indexMinFoodDist = 0;
			int indexMinObstDist = 0;
			double minFoodDist = foodDists[0];
			double minObstDist = obstDists[0];

			for (int i = 1; i < 4; ++i) {
				if (foodDists[i] < minFoodDist) {
					minFoodDist = foodDists[i];
					indexMinFoodDist = i;
				}
				if (obstDists[i] < minObstDist) {
					minObstDist = obstDists[i];
					indexMinObstDist = i;
				}
			}

			// {foodL, foodR, foodU, foodD, obstL, obstR, obstU, obstD}
			double[] input = new double[8];

			if (centerDistFood < centerDistObst) {
				for (int i = 0; i < 4; ++i) {
					input[i] = -1;
				}
				input[indexMinFoodDist] = 1;
			} else {
				for (int i = 4; i < 8; ++i) {
					input[i] = -1;
				}
				input[indexMinObstDist] = 1;
			}

			double[] output = Brain.Run(input);

			ProcessOutput(output, centerDistObst);
		}

		private GameObject GetClosestFood (IList<GameObject> foodList) {
			double closest = 30000;
			GameObject closestFood = foodList[0];

			foreach (GameObject obj in foodList) {
				double dist = Vector3.Distance(Body.transform.position, obj.transform.position);
				if (dist < closest) {
					closest = dist;
					closestFood = obj;
				}
			}

			return closestFood;
		}

		private GameObject GetClosestObst (IList<GameObject> obstList) {
			double closest = 30000;
			GameObject closestObst = obstList[0];

			foreach (GameObject obj in obstList) {
				double dist = Vector3.Distance(Body.transform.position, obj.transform.position);
				if (dist < closest) {
					closest = dist;
					closestObst = obj;
				}
			}

			return closestObst;
		}

		private void ProcessOutput (IList<double> output, double centerObstDist) {
			int indexMaxOutput = 0;
			double maxOutput = output[0];

			for (int i = 1; i < output.Count; ++i) {
				if (output[i] > maxOutput) {
					maxOutput = output[i];
					indexMaxOutput = i;
				}
			}

			switch (indexMaxOutput) {
				case 0:
					Body.transform.Rotate(Vector3.left, 1);
					break;
				case 1:
					Body.transform.Rotate(Vector3.right, 1);
					break;
				case 2:
					Body.transform.Rotate(Vector3.up, 1);
					break;
				case 3:
					Body.transform.Rotate(Vector3.down, 1);
					break;
			}

			if (centerObstDist < _itemProx) {
				--Life;
				Fitness -= 10;
				if (Fitness < 0) {
					Fitness = 0;
				}
			}
		}

	}

}