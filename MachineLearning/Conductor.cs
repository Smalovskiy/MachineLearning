using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using Accord.Statistics.Analysis;
using AForge.Neuro;
using MachineLearning.Algorithm;
using MachineLearning.Base;
using MachineLearning.Clustering;
using DataPartition = System.Tuple<System.Double[][], System.Double[][]>;

namespace MachineLearning
{
    /// <summary>
    /// Conductor is a simple, presenter like class, that manages the interaction with the UI.
    /// </summary>
    public class Conductor
    {
        IMainView m_view;

        private double[][] ClassA;
        private double[][] ClassB;

        private Tuple<Double[,], ConfusionMatrix> MostAccuratePerceptron;
        private Tuple<List<Double[,]>, ConfusionMatrix> MostAccurateNN;

        private double m_minX;
        private double m_maxX;
        private int m_iterations;

        public ConfusionMatrix[,] Statistics { get; set; }

        public ConfusionMatrix BestKNN { get; set; }

        public Conductor(IMainView view, int iterations)
        {
            m_view = view;
            m_iterations = iterations;
        }

        public void Start()
        {
            //Read the data from the files
            var file1DataRaw = AlgorithmHelpers.ReadMatrixFromFile(@"class_1.dat");
            var file2DataRaw = AlgorithmHelpers.ReadMatrixFromFile(@"class_2.dat");

            ClassA = AlgorithmHelpers.ScaleDown(AlgorithmHelpers.ChooseFeatures(file1DataRaw));
            ClassB = AlgorithmHelpers.ScaleDown(AlgorithmHelpers.ChooseFeatures(file2DataRaw));

            m_maxX = ClassA.Max(0)[0] > ClassB.Max(0)[0] ? ClassA.Max(0)[0] : ClassB.Max(0)[0];
            m_minX = ClassA.Min(0)[0] < ClassB.Min(0)[0] ? ClassA.Min(0)[0] : ClassB.Min(0)[0];

            //Fill the charts with the data
            m_view.ShowTrainingData(ClassA, ClassB);
            //Clear the list view
            m_view.ClearListView();
            //Fill it with the confusion matrix for each algorithm per iteration
            var statistics = new ConfusionMatrix[4, 5];


            //Merge the two data sets
            //and run kmeans
            var kmeans = new KMeansClustering(AlgorithmHelpers.MergeArrays(file1DataRaw, file2DataRaw),
                m_view.Iterations,
                m_view.ThetaStep);

            var idx = kmeans.Classify();

            m_view.ClustersTextUpdate(idx.Distinct().Length.ToString());

            m_view.ZeroProgressBar();
            m_view.StepProgressBar();

            //Partition m_iterations times and run the algorithms
            for (int i = 0; i < m_iterations; ++i)
            {
                m_view.PerformStep();
                //Partition its class to training and testing set
                var partitions = new DataPartition[] { AlgorithmHelpers.Partition(ClassA), AlgorithmHelpers.Partition(ClassB) };

                //Create the training data
                var trainingPair = AlgorithmHelpers.CreateDataPair(partitions[0].Item1, partitions[1].Item1);
                var trainingSet = trainingPair.Item1;
                var trainingOutput = trainingPair.Item2;

                //Create the testing data
                var testingPair = AlgorithmHelpers.CreateDataPair(partitions[0].Item2, partitions[1].Item2);
                var testingSet = testingPair.Item1;
                var testingOutput = testingPair.Item2;

                //Some functions need the training output to be a vector of doubles
                var doubleTO = trainingOutput
                    .Select(x => new[] { Convert.ToDouble(x) })
                    .ToArray();

                for (int k = 1; k < 3; ++k)
                {
                    var nn = new KNearestNeighboursRuntime(k, trainingSet, trainingOutput, testingSet, testingOutput);

                    if (BestKNN == null)
                    {

                        BestKNN = nn.Execute();
                    }
                    else
                    {
                        var iter = nn.Execute();
                        if (iter.Accuracy > BestKNN.Accuracy)
                            BestKNN = iter;
                    }
                }

                var perceptron = new PerceptronRuntime(trainingSet, doubleTO, testingSet, testingOutput);
                perceptron.Finished += perceptron_Finished;

                var leastSquare = new LeastSquaresRuntime(AlgorithmHelpers.JaggedToMD(trainingSet), AlgorithmHelpers.JaggedToMD(doubleTO), AlgorithmHelpers.JaggedToMD(testingSet), testingOutput);
                var neuralNetwork = new ParallelNeuralNetworkRuntime(trainingSet, doubleTO, testingSet, testingOutput);

                neuralNetwork.Finished += neuralNetwork_Finished;
                //Compute the confusion matrices for the four classifiers                 
                statistics[0, i] = perceptron.Execute();
                statistics[1, i] = leastSquare.Execute();
                //Use the most accurate K of KNN
                statistics[2, i] = BestKNN;
                statistics[3, i] = neuralNetwork.Execute();
            }

            //Update the classifier lines in the charts
            //with the most accurate of the 5 iterations
            m_view.ChartUpdate("", "classifier", MostAccuratePerceptron.Item1);
            m_view.ChartUpdate("", "classifier1", MostAccurateNN.Item1);

            //Process the array with the Confusion Matrices
            //and update the list view
            var processed = AlgorithmHelpers.ProcessStatistics(statistics);
            m_view.UpdateStatisticsListView(processed);
        }

        private void perceptron_Finished(object sender, AlgorithmFinishedEventArgs e)
        {
            var neuron = e.Neurons.First();
            // Calculate the coordinates of the classifier
            double k = (neuron.Weights[1] != 0) ? (-neuron.Weights[0] / neuron.Weights[1]) : 0;
            double b = (neuron.Weights[1] != 0) ? (-((ActivationNeuron)neuron).Threshold / neuron.Weights[1]) : 0;

            // Create the line and feed it to the data series
            double[,] classifier = new double[2, 2]
            {
               { (float)m_minX, (float)m_minX * k + b },
               { (float)m_maxX, (float)m_maxX * k + b }
            };

            var cmatrix = e.Matrix;
            //Update the most accurate classification
            if (MostAccuratePerceptron == null || cmatrix.Accuracy > MostAccuratePerceptron.Item2.Accuracy)
            {
                MostAccuratePerceptron = Tuple.Create(classifier, cmatrix);
            }

            m_view.ChartUpdate("perceChart", "classifier", MostAccuratePerceptron.Item1);
        }

        private void neuralNetwork_Finished(object sender, AlgorithmFinishedEventArgs e)
        {
            var classifiers = new List<Double[,]>();
            foreach (var neuron in e.Neurons)
            {
                double k = (neuron.Weights[1] != 0) ? (-neuron.Weights[0] / neuron.Weights[1]) : 0;
                double b = (neuron.Weights[1] != 0) ? (-((ActivationNeuron)neuron).Threshold / neuron.Weights[1]) : 0;
                double[,] classifier = new double[2, 2] 
                {
                    { (float)m_minX, (float)m_minX * k + b },
                    { (float)m_maxX, (float)m_maxX * k + b }
                };
                classifiers.Add(classifier);
            }

            var bestCM = e.Matrix;

            if (MostAccurateNN == null || bestCM.Accuracy > MostAccurateNN.Item2.Accuracy)
                MostAccurateNN = Tuple.Create(classifiers, bestCM);

            m_view.ChartUpdate("nnChart", "classifier1", MostAccurateNN.Item1);
        }
    }
}
