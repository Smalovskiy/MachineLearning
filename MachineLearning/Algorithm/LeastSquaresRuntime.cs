using System;
using System.Linq;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression.Linear;
using MachineLearning.Base;

namespace MachineLearning.Algorithm
{
    class LeastSquaresRuntime : AlgorithmRuntime
    {
        private double[,] trainingSet;
        private double[,] trainingOutput;
        private double[,] testSet;
        private int[] expected;

        public LeastSquaresRuntime(Double[,] trainingSet, Double[,] trainingOutput, Double[,] testSet, int[] expected)
        {
            this.trainingSet = trainingSet;
            this.trainingOutput = trainingOutput;
            this.testSet = testSet;
            this.expected = expected;
        }

        public override ConfusionMatrix Execute()
        {
            //The Least Squares algorithm
            //It uses a PartialLeastSquaresAnalysis library object using a non-linear iterative partial least squares algorithm
            //and runs on the mean-centered and standardized data

            //Create an analysis
            var pls = new PartialLeastSquaresAnalysis(trainingSet,
                trainingOutput,
                AnalysisMethod.Standardize,
                PartialLeastSquaresAlgorithm.NIPALS);

            pls.Compute();

            //After computing the analysis
            //create a linear model to predict new variables
            MultivariateLinearRegression regression = pls.CreateRegression();

            //This will hold the result of the classifications
            var predictedLifted = new int[testSet.GetLength(0)][];

            for (int i = 0; i < predictedLifted.Length; ++i)
            {
                predictedLifted[i] = regression
                    .Compute(testSet.GetRow(i)) //Retrieve the row vector of the test set
                    .Select(x => Convert.ToInt32(x))// Convert the result to int
                    .ToArray();
            }

            //Unlift the prediction vector
            var predicted = predictedLifted
                .SelectMany(x => x)
                .ToArray();

            //Create a new confusion matrix with the calculated parameters
            ConfusionMatrix cmatrix = new ConfusionMatrix(predicted, expected, POSITIVE, NEGATIVE);
            return cmatrix;
        }
    }
}
