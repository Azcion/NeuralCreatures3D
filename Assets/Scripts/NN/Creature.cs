using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.NN {

	public class Creature {

		private struct Dimensions {

			public static float xA;
			public static float yA;
			public static float zA;

			public static float xB = 1;
			public static float yB = 1;
			public static float zB = 1;

		}

		public GameObject Body;
		public Vector3 Rotation;
		public Transform Parent;

		public GameObject SensorL;
		public GameObject SensorR;
		public GameObject SensorU;
		public GameObject SensorD;

		public NeuralNetwork Brain;

		public int Age;
		public bool Dead;
		public double Life;
		public int Fitness;
		public double ParentChance;

		private const double ItemProx = .5;

		public Creature (GameObject body, Vector3 rotation, Transform parent) {
			Body = body;
			Body.transform.position = new Vector3(Random.Range(Dimensions.xA, Dimensions.xB),
			                                      Random.Range(Dimensions.yA, Dimensions.yB),
			                                      Random.Range(Dimensions.zA, Dimensions.zB));
			Body.transform.Rotate(rotation);
			Body.transform.parent = parent;

			Rotation = rotation;
			Parent = parent;

			Brain = new NeuralNetwork(8, 15, 4);
			Life = 100;

			SensorL = Body.transform.GetChild(5).gameObject;
			SensorR = Body.transform.GetChild(6).gameObject;
			SensorU = Body.transform.GetChild(7).gameObject;
			SensorD = Body.transform.GetChild(8).gameObject;
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
			Body.transform.position = new Vector3(Random.Range(Dimensions.xA, Dimensions.xB),
												  Random.Range(Dimensions.yA, Dimensions.yB),
												  Random.Range(Dimensions.zA, Dimensions.zB));
			Brain = new NeuralNetwork(8, 15, 4);
			Dead = false;
			Fitness = 0;
			Life = 100;
		}

		public void Kill () {
			Body.SetActive(false);
			Dead = true;
			Fitness = 0;
			Life = 0;
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

			double closestFoodL = Vector3.Distance(closestFood, SensorL.transform.position);
			double closestFoodR = Vector3.Distance(closestFood, SensorR.transform.position);
			double closestFoodU = Vector3.Distance(closestFood, SensorU.transform.position);
			double closestFoodD = Vector3.Distance(closestFood, SensorD.transform.position);

			double closestObstL = Vector3.Distance(closestObst, SensorL.transform.position);
			double closestObstR = Vector3.Distance(closestObst, SensorR.transform.position);
			double closestObstU = Vector3.Distance(closestObst, SensorU.transform.position);
			double closestObstD = Vector3.Distance(closestObst, SensorD.transform.position);

			double centerDistFood = Vector3.Distance(closestFood, Body.transform.position);
			double centerDistObst = Vector3.Distance(closestObst, Body.transform.position);

			if (centerDistFood < ItemProx) {
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

			double[] input = {-1, -1, -1, -1, -1, -1, -1, -1};
			input[indexMinFoodDist] = 1;
			input[indexMinObstDist + 4] = 1;

			// todo lol
			double[] output = SmartBrain(input);
			//double[] output = Brain.Run(input);

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

		private double[] SmartBrain (double[] inputs) {
			double[] outputs = {1, 1, 1, 1};

			for (int i = 0; i < 4; ++i) {
				outputs[i] += inputs[i] - inputs[i + 4];
			}

			return outputs;
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

			const float angle = 2f;

			switch (indexMaxOutput) {
				case 3:
					Body.transform.Rotate(Vector3.left, angle);  // down
					break;
				case 2:
					Body.transform.Rotate(Vector3.right, angle);  // up
					break;
				case 1:
					Body.transform.Rotate(Vector3.up, angle);  // right
					break;
				case 0:
					Body.transform.Rotate(Vector3.down, angle);  // left
					break;
			}

			if (centerObstDist < ItemProx) {
				--Life;
				Fitness -= 10;
				if (Fitness < 0) {
					Fitness = 0;
				}
			}
		}

	}

}