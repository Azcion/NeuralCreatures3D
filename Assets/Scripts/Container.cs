﻿using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.NN;

public class Container {

	public static readonly float[] DIMENSIONS = {-4.5f, 4.5f, .5f, 9.5f, -4.5f, 4.5f};

	public static List<GameObject> FoodList = new List<GameObject>();
	public static List<GameObject> ObstList = new List<GameObject>();

	public static List<Creature> CreatureList = new List<Creature>();

	public static GeneticAlgorithm GeneticAlgorithm = new GeneticAlgorithm(50, 1, 10);

	public static void ResetAllDead () {
		foreach (Creature creature in CreatureList) {
			if (creature.Dead) {
				creature.Reset();
			}
		}
	}

	public static void ResetAll () {
		foreach (Creature creature in CreatureList) {
			creature.Reset();
		}
	}

}