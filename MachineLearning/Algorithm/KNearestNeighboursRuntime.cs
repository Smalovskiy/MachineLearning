using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning;
using Accord.Statistics.Analysis;
using MachineLearning.Base;


namespace MachineLearning.Algorithm
{
    public class KNearestNeighboursRuntime : AlgorithmRuntime
    {
        int k;
        double[][] trainingSet;
        int[] trainingOutput;
        double[][] testSet;
        int[] expected;

        public KNearestNeighboursRuntime(int k, double[][] trainingSet, int[] trainingOutput, double[][] testSet, int[] expected)
        {
            this.k = k;
            this.trainingSet = trainingSet;
            this.trainingOutput = trainingOutput;
            this.testSet = testSet;
            this.expected = expected;
        }
        public override ConfusionMatrix Execute()
        {
            //Create a knn classifer with 2 classes
            var knn = new KNearestNeighbors(k: k,
                classes: 2,
                inputs: trainingSet,
                outputs: trainingOutput);

            //Map the classifier over the test set
            //This wil return an array where index i is the classificatioon of the i-th vector
            //of the testSet
            var predicted = AlgorithmHelpers
                .MergeArrays(trainingSet, testSet)
                .Select(x => knn.Compute(x))
                .ToArray();

            //Create a new confusion matrix with the calculated parameters
            var cmatrix = new ConfusionMatrix(predicted, AlgorithmHelpers.MergeArrays(trainingOutput, expected), POSITIVE, NEGATIVE);
            return cmatrix;
        }
    }
}
