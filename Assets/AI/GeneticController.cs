using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GeneticAlgorithm
{
    public class GeneticController : MonoBehaviour
    {

        // GA params
        private int genomes_count, current_generation = 0;
        private static System.Random random = new System.Random();
        private sbyte[][] genomes;
        NeuroNetwork[] brains;
        double[] fitness;
        public double mutationProb = 0;

        // NeuroNetwork params
        private int neuro_input_nodes, neuro_output_nodes;
        private int[] neurons_per_layer;


        public GeneticController(int genomes_count, int neuro_input_nodes, int[] neurons_per_layer, int neuro_output_nodes)
        {
            // Set GA params
            this.genomes_count = genomes_count;
            this.neuro_input_nodes = neuro_input_nodes;
            this.neurons_per_layer = neurons_per_layer;
            this.neuro_output_nodes = neuro_output_nodes;
        }

        public void createGeneration()
        {
            if (current_generation == 0)
            {
                // Generate random genomes on first generation
                brains = new NeuroNetwork[genomes_count];
                genomes = new sbyte[genomes_count][];
                fitness = new double[genomes_count];

                for (int i = 0; i < brains.Length; i++)
                {
                    genomes[i] = new sbyte[NeuroNetwork.calculateGenomeLength(neuro_input_nodes, neurons_per_layer, neuro_output_nodes)];
                    for (int g = 0; g < genomes[i].Length; g++)
                        genomes[i][g] = (sbyte)random.Next(-128, 127);
                }
            }
            else
            {
                // Generate new solutions trying to improve them
                improveGeneration();
            }
            
            applyGenomesInBrains();
            current_generation++;
        }

        private void applyGenomesInBrains()
        {
            for (int i = 0; i < genomes.Length; i++)
            {
                brains[i] = new NeuroNetwork(neuro_input_nodes, neurons_per_layer, neuro_output_nodes, genomes[i]);
            }
        }

        private void improveGeneration()
        {
            // Selecting better solutions
            Dictionary<sbyte[], double> genomes_dictionary = new Dictionary<sbyte[], double>();
            for (int i = 0; i < genomes_count; i++)
                genomes_dictionary.Add(genomes[i], fitness[i]);
            genomes_dictionary = genomes_dictionary.OrderByDescending(key => key.Value).Take(2).ToDictionary(pair => pair.Key, pair => pair.Value);

            // Create new genomes array
            sbyte[][] newGenomes = new sbyte[genomes_count][];

            // Keep best genomes
            newGenomes[0] = crossover(genomes_dictionary.Keys.First(), genomes_dictionary.Keys.First());
            newGenomes[1] = crossover(genomes_dictionary.Keys.Last(), genomes_dictionary.Keys.Last());

            // Crossed-over / mutated solutions
            for (int i = 2; i < newGenomes.Length; i++)
                newGenomes[i] = crossover(genomes_dictionary.Keys.First(), genomes_dictionary.Keys.Last());

            // Replace old generation with new one
            genomes = newGenomes;
        }

        private sbyte[] crossover(sbyte[] genome1, sbyte[] genome2)
        {
            sbyte[] newGenome = new sbyte[genome1.Length];

            // Set in how many parts the genome should be divided in
            int parts = random.Next(2, 3);
            // Set the splitting points
            int[] splitPoints = new in