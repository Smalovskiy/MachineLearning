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
