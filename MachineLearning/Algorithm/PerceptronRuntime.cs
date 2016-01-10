using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Analysis;
using AForge.Neuro;
using AForge.Neuro.Learning;
using MachineLearning.Base;

namespace MachineLearning.Algorithm
{
    public class PerceptronRuntime : AlgorithmRuntime
    {
        private double[][] trainingSet;
        private double[][] trainingOutput;
        private double[][] testSet;
        private int[] expected;

        public PerceptronRuntime(double[][] trainingSet, double[][] trainingOutput, double[][] testSet, int[] expected)
        {
            this.trainingSet = trainingSet;
            this.trainingOutput = trainingOutput;
            this.testSet = testSet;
            this.expected = expected;
        }

        public override ConfusionMatrix Execute()
        {
            //Create an network with one layer and one neuron in that layer
            var network = new ActivationNetwork(new ThresholdFunction(), 3, 1);

            //Bind the reference of the neuron
            var neuron = network.Layers[0].Neurons[0] as ActivationNeuron;

            //Create the Perceptron learning algorithm
            //Library perceptron implements a single layer linear classifier
            var teacher = new PerceptronLearning(network);

            teacher.LearningRate = 0.1;

            //Enrich the dimensions of the vectors, padding 1 to the end
            var richTraining = AlgorithmHelpers.PaddDimension(trainingSet);
            var richTesting = AlgorithmHelpers.PaddDimension(testSet);

            //Training the network until the error is small enough
            //or 500 hundred iterations have been computed
            int epochs = 0;
            while (true)
            {
                double error = teacher.RunEpoch(richTraining, trainingOutput);/// trainingSet.Length;
                ++epochs;
                if (error < 0.025 * trainingSet.Length || epochs == 500) break;
            }

            var predicted = richTesting
                   .Select(x => neuron.Compute(x))
                   .Select(x => Convert.ToInt32(x))
                   .ToArray();


            //Create a confusion matrix with the calculated parameters
            ConfusionMatrix cmatrix = new ConfusionMatrix(predicted, expected, POSITIVE, NEGATIVE);

            OnAlgorithmEnded(Enumerable.Repeat(neuron, 1), cmatrix);
            return cmatrix;
        }
    }
}
