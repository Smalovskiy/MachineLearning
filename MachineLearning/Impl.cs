using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression.Linear;
using AForge;
using AForge.Controls;
using AForge.Neuro;
using AForge.Neuro.Learning;
//using NN.Models;
//Using more sensible and convinient names
using DataPair = System.Tuple<System.Double[][], System.Int32[]>;
using DataPartition = System.Tuple<System.Double[][], System.Double[][]>;

namespace MachineLearning
{
    public partial class MainForm : Form
    {
        #region Algorithms
        //The KNN algorithm 
        //It creates a knn library object, classifies the entire data set
        //and returns the confusion matrix for the classification
        private ConfusionMatrix RunKNN(int k, Double[][] trainingSet, int[] trainingOutput, Double[][] testSet, int[] expected)
        {
            //Create a knn classifer with 2 classes
            KNearestNeighbors knn = new KNearestNeighbors(k: k, classes: 2,
                inputs: trainingSet, outputs: trainingOutput);
            //Map the classifier over the test set
            //This wil return an array where index i is the classificatioon of the i-th vector
            //of the testSet
            var predicted = UtilityProvider.MergeArrays(trainingSet, testSet)
                            .Select(x => knn.Compute(x))
                            .ToArray();

            //For test, assume 0 as positive and 1 as negative
            int positive = 0;
            int negative = 1;

            //Create a new confusion matrix with the calculated parameters
            ConfusionMatrix cmatrix = new ConfusionMatrix(predicted, UtilityProvider.MergeArrays(trainingOutput, expected), positive, negative);
            return cmatrix;
        }

        //The Least Squares algorithm
        //It uses a PartialLeastSquaresAnalysis library object using a non-linear iterative partial least squares algorithm
        //and runs on the mean-centered and standardized data
        private ConfusionMatrix RunLS(Double[,] trainingSet, Double[,] trainingOutput, Double[,] testSet, int[] expected)
        {
            //Create an analysis
            PartialLeastSquaresAnalysis pls = new PartialLeastSquaresAnalysis(trainingSet, trainingOutput,
                                                                        AnalysisMethod.Standardize, PartialLeastSquaresAlgorithm.NIPALS);
            pls.Compute();

            //After computing the analysis
            //create a linear model to predict new variables
            MultivariateLinearRegression regression = pls.CreateRegression();

            //This will hold the result of the classifications
            var predictedLifted = new int[testSet.GetLength(0)][];

            for (int i = 0; i < predictedLifted.Length; ++i)
            {

                predictedLifted[i] = regression
                                .Compute(testSet.GetRow(i)) //Retrieve the row vector of the test set
                                .Select(x => Convert.ToInt32(x))// Convert the result to int
                                .ToArray();
            }

            //Unlift the prediction vector
            var predicted = predictedLifted
                            .SelectMany(x => x)
                            .ToArray();

            //For test, assume 1 as positive and 0 as negative
            int positive = 0;
            int negative = 1;

            //Create a new confusion matrix with the calculated parameters
            ConfusionMatrix cmatrix = new ConfusionMatrix(predicted, expected, positive, negative);
            return cmatrix;
        }

        //The Single Layer Perceptron classifier algorithm
        //It uses an Activation Network library object, with one layer and one neuron to the layer
        //To train the network it uses a PerceptronLearning library object
        private ConfusionMatrix RunPerceptron(Double[][] trainingSet, Double[][] trainingOutput, Double[][] testSet, int[] expected)
        {
            //Create an network with one layer and one neuron in that layer
            ActivationNetwork network = new ActivationNetwork(new ThresholdFunction(), 3, 1);

            //Bind the reference of the neuron
            ActivationNeuron neuron = network.Layers[0].Neurons[0] as ActivationNeuron;

            //Create the Perceptron learning algorithm
            //Library perceptron implements a single layer linear classifier
            PerceptronLearning teacher = new PerceptronLearning(network);

            teacher.LearningRate = 0.1;

            //Enrich the dimensions of the vectors, padding 1 to the end
            var richTraining = UtilityProvider.PaddDimension(trainingSet);
            var richTesting = UtilityProvider.PaddDimension(testSet);

            //Training the network until the error is small enough
            //or 500 hundred iterations have been computed
            int epochs = 0;
            while (true)
            {
                double error = teacher.RunEpoch(richTraining, trainingOutput);/// trainingSet.Length;
                ++epochs;
                if (error < 0.025 * trainingSet.Length || epochs == 500) break;
                // Console.Write("Iter: " + epochs + " " + error + "\n");
            }

            var predicted = richTesting
                   .Select(x => neuron.Compute(x))
                   .Select(x => Convert.ToInt32(x))
                   .ToArray();

            // Calculate the coordinates of the classifier
            double k = (neuron.Weights[1] != 0) ? (-neuron.Weights[0] / neuron.Weights[1]) : 0;
            double b = (neuron.Weights[1] != 0) ? (-((ActivationNeuron)neuron).Threshold / neuron.Weights[1]) : 0;

            // Create the line and feed it to the data series
            double[,] classifier = new double[2, 2]{
               { perceChart.RangeX.Min, perceChart.RangeX.Min * k + b},
               { perceChart.RangeX.Max, perceChart.RangeX.Max * k + b}
          };

            //For test, assume 1 as positive and 0 as negative
            int positive = 0;
            int negative = 1;

            //Create a confusion matrix with the calculated parameters
            ConfusionMatrix cmatrix = new ConfusionMatrix(predicted, expected, positive, negative);

            //Update the most accurate classification
            if (MostAccuratePerceptron == null || cmatrix.Accuracy > MostAccuratePerceptron.Item2.Accuracy)
            {
                MostAccuratePerceptron = Tuple.Create(classifier, cmatrix);
            }

            return cmatrix;
        }

        //
        private ConfusionMatrix RunNN(Double[][] trainingSet, Double[][] trainingOutput, Double[][] testSet, int[] expected)
        {
            double alpha = 2.0;
            ActivationNetwork network = new ActivationNetwork(new SigmoidFunction(alpha), 2, 3, 1);
            ActivationNeuron neuron = network.Layers[1].Neurons[0] as ActivationNeuron;
            ActivationLayer layer = network.Layers[0] as ActivationLayer;

            // ResilientBackpropagationLearning teacher = new ResilientBackpropagationLearning(network);
            // teacher.LearningRate = 0.01;
            //teacher.Momentum = 0.1;
            //Enrich the dimensions of the vectors, padding 1 to the end

            //Selection method - elite;
            //Crossover rate - 0.75;
            //Mutation rate - 0.25;
            //Rate of injection of random chromosomes during selection - 0.20;
            //Random numbers generator for initializing new chromosome - UniformGenerator( new Range( -1, 1 ) );
            //Random numbers generator used during mutation for genes' multiplication - ExponentialGenerator( 1 );
            //Random numbers generator used during mutation for adding random value to genes - UniformGenerator( new Range( -0.5f, 0.5f ) ).
            EvolutionaryLearning teacher =
                new EvolutionaryLearning(network,
                                            100); // number of chromosomes in genetic population

            var richTraining = trainingSet;
            var richTesting = testSet;

            int epochs = 0;

            while (true)
            {
                double error = teacher.RunEpoch(richTraining, trainingOutput);// / trainingSet.Length;
                //++epochs;
                if (epochs == 200)
                {
                    epochs = 0;
                    network = new ActivationNetwork(new SigmoidFunction(alpha), 3, 3, 1);
                    teacher = new EvolutionaryLearning(network, 100);

                }
                if (error <= 2.5) break;
                //Console.Write("Iter: " + epochs + " " + error + "\n");
            }

            var predicted = richTesting
                   .Select(x => network.Compute(x))
                   .Select(x => Convert.ToInt32(Math.Round(x[0])))
                   .ToArray();

            List<Double[,]> classifiers = new List<Double[,]>();

            // Calculate the coordinates of the classifier
            double k = (neuron.Weights[1] != 0) ? (-neuron.Weights[0] / neuron.Weights[1]) : 0;
            double b = (neuron.Weights[1] != 0) ? (-((ActivationNeuron)neuron).Threshold / neuron.Weights[1]) : 0;

            // Create the line and feed it to the data series
            double[,] classifier = new double[2, 2]{
               { perceChart.RangeX.Min, perceChart.RangeX.Min * k + b},
               { perceChart.RangeX.Max, perceChart.RangeX.Max * k + b}
          };

            classifiers.Add(classifier);
            //For test, assume 0 as positive and 1 as negative
            int positive = 0;
            int negative = 1;

            //Create a confusion matrix with the calculated parameters
            ConfusionMatrix cmatrix = new ConfusionMatrix(predicted, expected, positive, negative);

            if (MostAccurateNN == null || cmatrix.Accuracy > MostAccurateNN.Item2.Accuracy)
                MostAccurateNN = Tuple.Create(classifiers, cmatrix);

            return cmatrix;
        }

        private void RunKMeans(Double[][] data)
        {
            var clusterData = BSASFeatures(data);
            var theta = MinMaxTheta(clusterData);
            var s = (int)iterationsNum.Value;
            var clusters = BSAS(theta, s, clusterData);

            KMeans kmeans = new KMeans(clusters, Distance.Euclidean);
            int[] idx = kmeans.Compute(clusterData);

            ClustersTextUpdate(idx.Distinct().Length.ToString());

        }
        #endregion


        #region Parallel
        private ConfusionMatrix ParallelRunNN(Double[][] trainingSet, Double[][] trainingOutput, Double[][] testSet, int[] expected)
        {

            var richTesting = testSet;

            ConcurrentDictionary<double, ActivationNetwork> networks = new ConcurrentDictionary<double, ActivationNetwork>();

            Parallel.For(0, 2000, (int i) =>
            {
                //Create an activation network
                ThreadLocal<ActivationNetwork> network = new ThreadLocal<ActivationNetwork>(() =>
                {
                    return new ActivationNetwork(new SigmoidFunction(2), 2, 2, 1);
                });

                ThreadLocal<ResilientBackpropagationLearning> teachear = new ThreadLocal<ResilientBackpropagationLearning>(() =>
                {
                    return new ResilientBackpropagationLearning(network.Value);
                });

                ThreadLocal<int> iter = new ThreadLocal<int>(() => { return 0; });
                ThreadLocal<double> error = new ThreadLocal<double>(() => { return 0; });

                while (networks.IsEmpty)
                {
                    error.Value = teachear.Value.RunEpoch(trainingSet, trainingOutput);
                    iter.Value++;
                    if (iter.Value == 1000) break;
                }
                if (!networks.ContainsKey(error.Value)) networks.TryAdd(error.Value, network.Value);
            }
            );

            int positive = 0;
            int negative = 1;

            Dictionary<ConfusionMatrix, ActivationNetwork> cms = new Dictionary<ConfusionMatrix, ActivationNetwork>();
            foreach (var keyv in networks)
            {
                var p = richTesting
                   .Select(x => keyv.Value.Compute(x))
                   .Select(x => Convert.ToInt32(x[0]))
                   .ToArray();

                ConfusionMatrix cm = new ConfusionMatrix(p, expected, positive, negative);
                cms.Add(cm, keyv.Value);
            }

            var kv = (from x in cms
                      orderby x.Key.Accuracy descending
                      select x).First();

            //   ActivationNeuron neuron = kv.Value.Layers[1].Neurons[0] as ActivationNeuron;


            var classifiers = new List<Double[,]>();
            for (int i = 0; i < kv.Value.Layers[0].Neurons.Length; ++i)
            {
                var neuron = kv.Value.Layers[0].Neurons[i] as ActivationNeuron;

                double k = (neuron.Weights[1] != 0) ? (-neuron.Weights[0] / neuron.Weights[1]) : 0;
                double b = (neuron.Weights[1] != 0) ? (-((ActivationNeuron)neuron).Threshold / neuron.Weights[1]) : 0;
                double[,] classifier = new double[2, 2] {
                            { nnChart.RangeX.Min, nnChart.RangeX.Min * k + b },
                            { nnChart.RangeX.Max, nnChart.RangeX.Max * k + b }
                                                            };
                classifiers.Add(classifier);
            }

            var bestCM = kv.Key;

            if (MostAccurateNN == null || bestCM.Accuracy > MostAccurateNN.Item2.Accuracy)
                MostAccurateNN = Tuple.Create(classifiers, bestCM);

            return bestCM;

        }

        private Tuple<double, double> MinMaxTheta(double[][] data)
        {

            Double[][] clusterData = BSASFeatures(data);
            double MinTheta = 500000;
            double MaxTheta = -500000;
            for (int i = 0; i < data.Length - 1; ++i)
            {
                double dist = Distance.Euclidean(clusterData[i], clusterData[i + 1]);
                if (dist < MinTheta)
                {
                    MinTheta = dist;
                }
                if (dist > MaxTheta)
                {
                    MaxTheta = dist;
                }
            }
            return Tuple.Create(MinTheta, MaxTheta);
        }

        private int BSAS(Tuple<double, double> thetavalues, int s, double[][] data)
        {
            List<int> nClasses = new List<int>();

            for (double i = thetavalues.Item1; i < thetavalues.Item2; i += (double)thetaStepNum.Value)
            {
                for (int z = 0; z < s; z++)
                {
                    double[][] rndata = UtilityProvider.RandomizeArray(data);
                    int Classes = 0;
                    for (int j = 0; j < rndata.Length - 1; ++j)
                    {
                        double dist = Distance.Euclidean(rndata[j], rndata[j + 1]);
                        if (dist > i)
                        {
                            Classes += 1;
                        }
                    }
                    nClasses.Add(Classes);
                }
            }

            var most = nClasses
                .GroupBy(x => x)
                .OrderByDescending(grp => grp.Count())
                .Select(grp => grp.Key)
                .First();


            return most;
        }

        #endregion
    }
}
