using System;

namespace GeneticAlgorithm
{
    public class Perceptron
    {

        private double[] weights;
        private double bias, output;

        public Perceptron(sbyte[] weights, sbyte bias)
        {
            // Initialize perceptron