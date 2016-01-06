using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Analysis;

namespace MachineLearning.Base
{
    public class AlgorithmRuntime
    {
        int k;
        Double[][] trainingSet;
        int[] trainingOutput;
        public AlgorithmRuntime(int k, Double[][] trainingSet, int[] trainingOutput, Double[][] testSet, int[] expected)
        {

        }
        public ConfusionMatrix Execute()
        {
            return null;
        }
    }
}
