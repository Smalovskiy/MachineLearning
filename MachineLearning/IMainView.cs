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
    public interface IMainView
    {
        void ChartUpdate(string chartName, string seriesName, double[,] data);
        void ChartUpdate(string chartName, string seriesName, List<double[,]> data);
        void ClustersTextUpdate(string value);
        void ClearListView();
        void UpdateStatisticsListView(Double[,] statistics);
        void ShowTrainingData(Double[][] classA, Double[][] classB);
        void ZeroProgressBar();
        void StepProgressBar();
        void PerformStep();
        double ThetaStep { get; set; }
        int Iterations { get; set; }
    }
}
