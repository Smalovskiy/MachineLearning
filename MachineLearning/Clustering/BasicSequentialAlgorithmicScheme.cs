using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using MachineLearning.Base;

namespace MachineLearning.Clustering
{
    class BasicSequentialAlgorithmicScheme
    {
        private Tuple<double, double> thetavalues;
        private int s;
        private double[][] data;
        private double thetaStepNum;

        public BasicSequentialAlgorithmicScheme(BsasArgs args)
        {
            this.thetavalues = args.Thetavalues;
            this.s = args.S;
            this.data = args.Data;
            this.thetaStepNum = args.ThetaStepNum;
        }

        public int CalculateClasses()
        {
            List<int> nClasses = new List<int>();

            for (double i = thetavalues.Item1; i < thetavalues.Item2; i += thetaStepNum)
            {
                for (int z = 0; z < s; z++)
                {
                    double[][] rndata = AlgorithmHelpers.RandomizeArray(data);
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
    }
}
