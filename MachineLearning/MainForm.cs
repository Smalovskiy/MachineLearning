using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Math;
using Accord.Statistics.Analysis;
using AForge;
using AForge.Controls;
using AForge.Neuro;
using MachineLearning.Algorithm;
using MachineLearning.Base;
using MachineLearning.Clustering;

using DataPartition = System.Tuple<System.Double[][], System.Double[][]>;

namespace MachineLearning
{
    public partial class MainForm : Form, IMainView
    {
        #region constructor
        public MainForm()
        {
            InitializeComponent();
            //Create a chart with two data series
            //Blue dots for class 1
            //Red  dots for class 2
            perceChart.AddDataSeries("class1", Color.Red, Chart.SeriesType.Dots, 5);
            perceChart.AddDataSeries("class2", Color.Blue, Chart.SeriesType.Dots, 5);
            perceChart.AddDataSeries("classifier", Color.Gray, Chart.SeriesType.Line, 1, false);
            //Create a chart with two data series
            //Blue dots for class 1
            //Red  dots for class 2
            nnChart.AddDataSeries("class1", Color.Red, Chart.SeriesType.Dots, 5);
            nnChart.AddDataSeries("class2", Color.Blue, Chart.SeriesType.Dots, 5);
            nnChart.AddDataSeries("classifier1", Color.Gray, Chart.SeriesType.Line, 1, false);
            nnChart.AddDataSeries("classifier2", Color.Gray, Chart.SeriesType.Line, 1, false);
            //Create a chart with two data series
            //Blue dots for class 1
            //Red  dots for class 2
            lsChart.AddDataSeries("class1", Color.Red, Chart.SeriesType.Dots, 5);
            lsChart.AddDataSeries("class2", Color.Blue, Chart.SeriesType.Dots, 5);
            lsChart.AddDataSeries("classifier", Color.Gray, Chart.SeriesType.Line, 1, false);

            m_conductor = new Conductor(this, 5);

        }
        #endregion
        //Class data
        private Double[][] class_1;
        private Double[][] class_2;
        private Conductor m_conductor;

        //The most accurate iterations of the neural networks
        //Used by the charts
        private Tuple<Double[,], ConfusionMatrix> MostAccuratePerceptron;
        private Tuple<List<Double[,]>, ConfusionMatrix> MostAccurateNN;
        /// <summary>
        /// The most accurate iteration of the KNN algorithm
        /// </summary>
        private ConfusionMatrix BestKNN;
        //Class that provides serialisation services with protobuf
        // private ProtoSerialisationServiceProvider PSSProvider;

        void PostToUIThread(Action callback)
        {
            if (InvokeRequired)
            {
                Invoke(callback);
            }
            else
            {
                callback();
            }
        }

        public void ChartUpdate(string chartName, string seriesName, double[,] data)
        {
            PostToUIThread(() =>
            {
                if (chartName == perceChart.Name)
                    perceChart.UpdateDataSeries(seriesName, data);
                else
                    lsChart.UpdateDataSeries(seriesName, data);
            });
        }

        public void ChartUpdate(string chartName, string seriesName, List<double[,]> data)
        {
            PostToUIThread(() =>
            {
                nnChart.UpdateDataSeries("classifier1", data[0]);
            });
        }

        public void ClustersTextUpdate(string value)
        {
            PostToUIThread(() =>
             {
                 this.clustersNum.Text = value;
             });

        }

        public void ClearListView()
        {
            PostToUIThread(() =>
             {
                 this.statisticsView.Items.Clear();
             });
        }


        public void UpdateStatisticsListView(Double[,] statistics)
        {
            PostToUIThread(() =>
            {
                string[] algorithms = { "KNN", "Perceptron", "Least Squares", "Neural Network" };
                var lvi = new ListViewItem[5];
                for (int i = 0; i < algorithms.Length; i++)
                {
                    lvi[i] = new ListViewItem(algorithms[i]);
                    for (int j = 0; j < 5; j++)
                    {
                        lvi[i].SubItems.Add(statistics[i, j].ToString());
                    }
                    statisticsView.Items.Add(lvi[i]);
                }
            });

        }

        //Method that updates the chart with the data vectors      
        public void ShowTrainingData(Double[][] classA, Double[][] classB)
        {
            //create data series from the vectors
            var class1 = AlgorithmHelpers.JaggedToMD(classA);
            var class2 = AlgorithmHelpers.JaggedToMD(classB);

            //Compute the minimum and maximum numbers for the X axis
            var maxX = classA.Max(0)[0] > classB.Max(0)[0] ? classA.Max(0)[0] : classB.Max(0)[0];
            var minX = classA.Min(0)[0] < classB.Min(0)[0] ? classA.Min(0)[0] : classB.Min(0)[0];

            //Update the range of the X axis with the max and the min
            perceChart.RangeX = new Range((float)minX, (float)maxX);
            nnChart.RangeX = new Range((float)minX, (float)maxX);
            lsChart.RangeX = new Range((float)minX, (float)maxX);

            //Update the Perceptron chart with the loaded data
            perceChart.UpdateDataSeries("class1", class1);
            perceChart.UpdateDataSeries("class2", class2);
            //Update the BackPropagation chart with the loaded data
            nnChart.UpdateDataSeries("class1", class1);
            nnChart.UpdateDataSeries("class2", class2);
            //Update the LS chart
            lsChart.UpdateDataSeries("class1", class1);
            lsChart.UpdateDataSeries("class2", class2);
        }

        //Button Handler for "Run"
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            //Spawn the host
            Task.Factory.StartNew(() =>
            {
                //SearchSolutionShim();
                m_conductor.Start();
            });
        }

        public void ZeroProgressBar()
        {
            PostToUIThread(() => progressBar1.Value = 0);
        }

        public void StepProgressBar()
        {
            PostToUIThread(() => progressBar1.Step *= 2);
        }

        public void PerformStep()
        {
            PostToUIThread(() => progressBar1.PerformStep());
        }

        //This is the function that runs the demo application
        //It handles the file loading the execution of the algorithms
        //and the manipulation of the controls
        private void SearchSolutionShim()
        {
            //Read the data from the files
            var file1DataRaw = AlgorithmHelpers.ReadMatrixFromFile(@"classA.dat");
            var file2DataRaw = AlgorithmHelpers.ReadMatrixFromFile(@"classB.dat");

            //Chose 2 features
            class_1 = AlgorithmHelpers.ScaleDown(AlgorithmHelpers.ChooseFeatures(file1DataRaw));
            class_2 = AlgorithmHelpers.ScaleDown(AlgorithmHelpers.ChooseFeatures(file2DataRaw));

            //Fill the charts with the data
            ShowTrainingData(class_1, class_2);
            //Clear the list view
            ClearListView();
            //Fill it with the confusion matrix for each algorithm per iteration
            var statistics = new ConfusionMatrix[4, 5];


            //Merge the two data sets
            //and run kmeans
            var kmeans = new KMeansClustering(AlgorithmHelpers.MergeArrays(file1DataRaw, file2DataRaw),
                (int)iterationsNum.Value,
                (double)thetaStepNum.Value);

            var idx = kmeans.Classify();

            ClustersTextUpdate(idx.Distinct().Length.ToString());

            ZeroProgressBar();
            StepProgressBar();

            //Partition 5 times and run the algorithms
            for (int i = 0; i < 5; ++i)
            {
                PerformStep();
                //Partition its class to training and testing set
                var partitions = new DataPartition[] { AlgorithmHelpers.Partition(class_1), AlgorithmHelpers.Partition(class_2) };

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


            //Process the array with the Confusion Matrices
            //and update the list view
            var processed = AlgorithmHelpers.ProcessStatistics(statistics);
            UpdateStatisticsListView(processed);
        }

        void neuralNetwork_Finished(object sender, AlgorithmFinishedEventArgs e)
        {
            var classifiers = new List<Double[,]>();
            foreach (var neuron in e.Neurons)
            {
                double k = (neuron.Weights[1] != 0) ? (-neuron.Weights[0] / neuron.Weights[1]) : 0;
                double b = (neuron.Weights[1] != 0) ? (-((ActivationNeuron)neuron).Threshold / neuron.Weights[1]) : 0;
                double[,] classifier = new double[2, 2] 
                {
                    { nnChart.RangeX.Min, nnChart.RangeX.Min * k + b },
                    { nnChart.RangeX.Max, nnChart.RangeX.Max * k + b }
                };
                classifiers.Add(classifier);
            }

            var bestCM = e.Matrix;

            if (MostAccurateNN == null || bestCM.Accuracy > MostAccurateNN.Item2.Accuracy)
                MostAccurateNN = Tuple.Create(classifiers, bestCM);

            ChartUpdate(nnChart.Name, "classifier1", MostAccurateNN.Item1);
        }


        void perceptron_Finished(object sender, AlgorithmFinishedEventArgs e)
        {
            var neuron = e.Neurons.First();
            // Calculate the coordinates of the classifier
            double k = (neuron.Weights[1] != 0) ? (-neuron.Weights[0] / neuron.Weights[1]) : 0;
            double b = (neuron.Weights[1] != 0) ? (-((ActivationNeuron)neuron).Threshold / neuron.Weights[1]) : 0;

            // Create the line and feed it to the data series
            double[,] classifier = new double[2, 2]
            {
               { perceChart.RangeX.Min, perceChart.RangeX.Min * k + b},
               { perceChart.RangeX.Max, perceChart.RangeX.Max * k + b}
            };

            var cmatrix = e.Matrix;
            //Update the most accurate classification
            if (MostAccuratePerceptron == null || cmatrix.Accuracy > MostAccuratePerceptron.Item2.Accuracy)
            {
                MostAccuratePerceptron = Tuple.Create(classifier, cmatrix);
            }

            ChartUpdate(perceChart.Name, "classifier", MostAccuratePerceptron.Item1);
        }


        public double ThetaStep
        {
            get
            {
                return (double)thetaStepNum.Value;
            }
            set
            {
                thetaStepNum.Value = (decimal)value;
            }
        }

        public int Iterations
        {
            get
            {
                return (int)iterationsNum.Value;
            }
            set
            {
                iterationsNum.Value = (decimal)value;
            }
        }
    }
}
