using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning;
using Accord.Math;

namespace MachineLearning.Clustering
{
    public class KMeansClustering
    {
        BSASFeatures features;
        int m_iterations;
        double m_thetaStepNum;
        public KMeansClustering(double[][] data, int iterations, double thetaStepNum)
        {
            features = new BSASFeatures(data);
            m_iterations = iterations;
            m_thetaStepNum = thetaStepNum;
        }

        public int[] Classify()
        {
            var clusterData = features.Select();
            var theta = MinMaxTheta(clusterData);

            var args = new BsasArgs(theta, m_iterations, clusterData, m_thetaStepNum);

            var bsas = new BasicSequentialAlgorithmicScheme(args);
            var clusters = bsas.CalculateClasses();
            var kmeans = new KMeans(clusters, Distance.Euclidean);
            var idx = kmeans.Compute(clusterData);

            return idx;
        }

        private Tuple<double, double> MinMaxTheta(double[][] data)
        {
            var features = new BSASFeatures(data);
            var clusterData = features.Select();
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
    }
}
