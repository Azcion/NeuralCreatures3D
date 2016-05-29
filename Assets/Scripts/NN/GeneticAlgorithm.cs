using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.NN {

	public class GeneticAlgorithm {

		public int Generation;
		public int TotalMutations;

		public float HighestFitness;
		public float AverageFitness;

		public double CrossOverChance;
		public double ElitismChance;
		public double MutationChance;

		public List<Creature> NewGeneration;

		private List<Creature> _oldGeneration; 

		private int _elitesCount;

		public GeneticAlgorithm (double elitismChance, double mutationChance, int population) {
			CrossOverChance = 100 - elitismChance;
			ElitismChance = elitismChance;
			MutationChance = mutationChance;

			_elitesCount = (int) (population * ElitismChance / 100);
		}

		public void Evolve (List<Creature> creatures) {
			NewGeneration = new List<Creature>();
			_oldGeneration = creatures;

			CalculateFitness();
			Elitism();
			CrossOver();

			for (int i = 0; i < creatures.Count; ++i) {
				Creature c = NewGeneration[i];
				c.Reset();
				creatures[i] = c;
			}
		}

		private void CalculateFitness () {
			HighestFitness = 0;
			AverageFitness = 0;

			foreach (Creature c in _oldGeneration) {
				++c.Age;
				AverageFitness += c.Fitness;
				if (c.Fitness > HighestFitness) {
					HighestFitness = c.Fitness;
				}
			}

			AverageFitness /= _oldGeneration.Count;

			foreach (Creature c in _oldGeneration) {
				if (HighestFitness > 0) {
					if (!c.Dead) {
						c.ParentChance = c.Fitness / HighestFitness * 100;
					} else {
						c.ParentChance = 0;
					}
				} else {
					// todo - everyone was awful
					c.Reset();
				}
			}
		}

		private void Elitism () {
			_oldGeneration = _oldGeneration.OrderByDescending(c => c.Fitness).ToList();

			for (int i = 0; i < _elitesCount; ++i) {
				NewGeneration.Add(_oldGeneration[i]);
			}
		}

		private Creature Selection () {
			NewGeneration = NewGeneration.OrderBy(c => Guid.NewGuid()).ToList();

			float parentThreshold = Random.Range(AverageFitness, HighestFitness);

			return NewGeneration.FirstOrDefault(c => c.ParentChance >= parentThreshold);
		}

		private void CrossOver () {
			int crossOverCount = (int) (_oldGeneration.Count * CrossOverChance / 100);

			for (int i = 0; i <= crossOverCount; ++i) {
				Creature parentA = Selection();
				Creature parentB = Selection();

				if (parentA == null || parentB == null) {
					continue;
				}

				double[] parentAWeights = parentA.Brain.GetWeights();
				double[] parentBWeights = parentB.Brain.GetWeights();
				double[] parentABiases = parentA.Brain.GetBiases();
				double[] parentBBiases = parentB.Brain.GetBiases();

				double[] childWeights = new double[parentAWeights.Length];
				double[] childBiases = new double[parentABiases.Length];

				int crossOverPointW = Random.Range(0, parentAWeights.Length);
				int crossOverPointB = Random.Range(0, parentABiases.Length);

				for (int j = 0; j < crossOverPointW; ++j) {
					childWeights[j] = parentAWeights[j];
				}
				for (int j = crossOverPointW; j < parentAWeights.Length; ++j) {
					childWeights[j] = parentBWeights[j];
				}
				for (int j = 0; j < crossOverPointB; ++j) {
					childBiases[j] = parentABiases[j];
				}
				for (int j = crossOverPointB; j < parentABiases.Length; ++j) {
					childBiases[j] = parentBBiases[j];
				}

				Creature child = new Creature(parentA.Body, parentA.Rotation, parentA.Parent);

				child.Brain.SetWeights(childWeights);
				child.Brain.SetBias(childBiases);
				NewGeneration.Add(child);
			}
		}

	}

}