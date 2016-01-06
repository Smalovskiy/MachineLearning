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
using NN.Models;
// NN.Models;
//Using more sensible and convinient names
using DataPartition = System.Tuple<System.Double[][], System.Double[][]>;

namespace MachineLearning
{
    public partial class MainForm : Form
    {
        #region constructor
        public MainForm()
        {
            InitializeComponent();
            //Create a serialisation service
            //PSSProvider = new ProtoSerialisationServiceProvider(); 

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

        }
        #endregion
        //Class data
        private Double[][] class_1;
        private Double[][] class_2;

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

        //Number of classes
        private int classCount;

        //The host for the algorithms
        private Thread workerThread = null;

        #region Delegates
        //Delegates for thread-safe manipulation of the Form's controls
        delegate void UpdatePLChartCallBack(string chartName, string seriesName, Double[,] data);
        delegate void UpdateNChartCallBack(string chartName, string seriesName, List<Double[,]> data);
        delegate void EraseListView();
        delegate void UpdateTextBox(string value);
        delegate void UpdateListView(Double[,] statistics);
        #endregion
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

        private void ChartUpdate(string chartName, string seriesName, Double[,] data)
        {

            PostToUIThread(() =>
            {
                if (chartName == perceChart.Name)
                    this.perceChart.UpdateDataSeries(seriesName, data);
                else
                    this.lsChart.UpdateDataSeries(seriesName, data);
            });
        }

        //Thread safe call on the chart
        //If the calling thread is different from the thread that created
        //the Chart control, this method creates a callback and calls itself
        //asynchronously using the Invole method
        //
        //If the calling thread is the same as the the one that created the control
        //it updates the data directly
        private void ChartUpdate(string chartName, string seriesName, List<Double[,]> data)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true.
            if (nnChart.InvokeRequired)
            {
                UpdateNChartCallBack d = new UpdateNChartCallBack(ChartUpdate);
                this.Invoke(d, new object[] { chartName, seriesName, data });
            }
            else
            {
                this.nnChart.UpdateDataSeries("classifier1", data[0]);
            }
        }

        //Thread safe call on the TextBox
        //If the calling thread is different from the thread that created
        //the TextBox control, this method creates a callback and calls itself
        //asynchronously using the Invole method
        //
        //If the calling thread is the same as the the one that created the control
        //it updates the data directly
        private void ClustersTextUpdate(string value)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true.
            if (this.clustersNum.InvokeRequired)
            {
                UpdateTextBox d = new UpdateTextBox(ClustersTextUpdate);
                this.Invoke(d, new object[] { value });
            }
            else
            {
                this.clustersNum.Text = value;
            }

        }

        //Thread safe call on the ListView
        //If the calling thread is different from the thread that created
        //the ListView control, this method creates a callback and calls itself
        //asynchronously using the Invole method
        //
        //If the calling thread is the same as the the one that created the control
        //it updates the data directly
        private void ClearListView()
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true.
            if (statisticsView.InvokeRequired)
            {
                EraseListView d = new EraseListView(ClearListView);
                this.Invoke(d, new object[] { });
            }
            else
            {
                this.statisticsView.Items.Clear();
            }
        }

        //Thread safe call on the ListView
        //If the calling thread is different from the thread that created
        //the ListView control, this method creates a callback and calls itself
        //asynchronously using the Invole method
        //
        //If the calling thread is the same as the the one that created the control
        //it updates the data directly
        private void UpdateStatisticsListView(Double[,] statistics)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true.
            if (this.statisticsView.InvokeRequired)
            {
                UpdateListView d = new UpdateListView(UpdateStatisticsListView);
                this.Invoke(d, new object[] { statistics });
            }
            else
            {
                String[] algorithms = { "KNN", "Perceptron", "Least Squares", "Neural Network" };
                ListViewItem[] lvi = new ListViewItem[5];
                for (int i = 0; i < algorithms.Length; i++)
                {
                    lvi[i] = new ListViewItem(algorithms[i]);
                    for (int j = 0; j < 5; j++)
                    {
                        lvi[i].SubItems.Add(statistics[i, j].ToString());
                    }
                    statisticsView.Items.Add(lvi[i]);
                }
            }

        }

        //Method that updates the chart with the data vectors      
        private void ShowTrainingData()
        {
            //create data series from the vectors
            Double[,] class1 = UtilityProvider.JaggedToMD(class_1);
            Double[,] class2 = UtilityProvider.JaggedToMD(class_2);
            classCount = 2;

            //Compute the minimum and maximum numbers for the X axis
            var maxX = class_1.Max(0)[0] > class_2.Max(0)[0] ? class_1.Max(0)[0] : class_2.Max(0)[0];
            var minX = class_1.Min(0)[0] < class_2.Min(0)[0] ? class_1.Min(0)[0] : class_2.Min(0)[0];

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
                SearchSolution();
            });
        }

        //This is the function that runs the demo application
        //It handles the file loading the execution of the algorithms
        //and the manipulation of the controls
        private void SearchSolution()
        {
            //Read the data from the files
            Double[][] file1DataRaw = UtilityProvider.ReadMatrixFromFile(@"class_1.dat");
            Double[][] file2DataRaw = UtilityProvider.ReadMatrixFromFile(@"class_2.dat");

            ////Seriliase the matrix with protobuf
            //PSSProvider.Serialise("ProtoMatrix", file1DataRaw);

            //Double[][] protobufMatrix = PSSProvider.Deserialise("ProtoMatrix");
            //Console.WriteLine(PSSProvider.ProviderID.ToString());
            //Chose 2 features
            class_1 = UtilityProvider.ScaleDown(UtilityProvider.ChooseFeatures(file1DataRaw));
            class_2 = UtilityProvider.ScaleDown(UtilityProvider.ChooseFeatures(file2DataRaw));

            //Fill the charts with the data
            ShowTrainingData();
            //Clear the list view
            ClearListView();
            //Fill it with the confusion matrix for each algorithm per iteration
            ConfusionMatrix[,] statistics = new ConfusionMatrix[4, 5];
            //Merge the two data sets
            //and run kmeans
            RunKMeans(UtilityProvider.MergeArrays(file1DataRaw, file2DataRaw));

            this.Invoke(new Action(() => progressBar1.Value = 0));
            this.Invoke(new Action(() => progressBar1.Step *= 2));
            //Partition 5 times and run the algorithms
            for (int i = 0; i < 5; ++i)
            {

                this.Invoke(new Action(() => progressBar1.PerformStep()));
                //Partition its class to training and testing set
                var partitions = new DataPartition[] { UtilityProvider.Partition(class_1), UtilityProvider.Partition(class_2) };

                //Create the training data
                var trainingPair = UtilityProvider.CreateDataPair(partitions[0].Item1, partitions[1].Item1);
                var trainingSet = trainingPair.Item1;
                var trainingOutput = trainingPair.Item2;

                //Create the testing data
                var testingPair = UtilityProvider.CreateDataPair(partitions[0].Item2, partitions[1].Item2);
                var testingSet = testingPair.Item1;
                var testingOutput = testingPair.Item2;
                //Some functions need the training output to be a vector of doubles
                var doubleTO = trainingOutput
                    .Select(x => new[] { Convert.ToDouble(x) })
                    .ToArray();

                for (int k = 1; k < 3; ++k)
                {
                    if (BestKNN == null)
                    {
                        BestKNN = RunKNN(k, trainingSet, trainingOutput, testingSet, testingOutput);
                    }
                    else
                    {
                        var iter = RunKNN(k, trainingSet, trainingOutput, testingSet, testingOutput);
                        if (iter.Accuracy > BestKNN.Accuracy)
                            BestKNN = iter;
                    }
                }

                //Compute the confusion matrices for the four classifiers                 
                statistics[0, i] = RunPerceptron(trainingSet, doubleTO, testingSet, testingOutput);
                statistics[1, i] = RunLS(UtilityProvider.JaggedToMD(trainingSet), UtilityProvider.JaggedToMD(doubleTO), UtilityProvider.JaggedToMD(testingSet), testingOutput);
                //Use the most accurate K of KNN
                statistics[2, i] = BestKNN;
                statistics[3, i] = ParallelRunNN(trainingSet, doubleTO, testingSet, testingOutput);
                //RunAnotherNN(trainingSet, doubleTO, testingSet, testingOutput);
            }

            //Update the classifier lines in the charts
            //with the most accurate of the 5 iterations
            ChartUpdate(perceChart.Name, "classifier", MostAccuratePerceptron.Item1);
            ChartUpdate(nnChart.Name, "classifier1", MostAccurateNN.Item1);

            //Process the array with the Confusion Matrices
            //and update the list view
            var processed = UtilityProvider.ProcessStatistics(statistics);
            UpdateStatisticsListView(processed);
        }

        private Double[][] BSASFeatures(Double[][] data, int featuresNum = 3)
        {
            var ret = new Double[data.Length][];

            for (int i = 0; i < data.Length; ++i)
            {
                ret[i] = new Double[] { data[i][0], data[i][1], data[i][2] };
            }

            return ret;
        }
    }
}
