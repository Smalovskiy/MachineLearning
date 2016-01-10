using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning.Clustering
{
    public class BSASFeatures
    {
        double[][] data;
        int featuresCount;

        public BSASFeatures(Double[][] data, int featuresNum = 3)
        {
            this.data = data;
            this.featuresCount = featuresNum;
        }

        public double[][] Select()
        {
            var ret = new Double[data.Length][];

            for (int i = 0; i < data.Length; ++i)
            {
                var record = new List<double>();
                for (int k = 0; k < featuresCount; k++)
                {
                    record.Add(data[i][k]);
                }
                ret[i] = record.ToArray();
            }
            return ret;
        }
    }
}
