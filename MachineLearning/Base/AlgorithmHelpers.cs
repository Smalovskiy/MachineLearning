using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using Accord.Statistics.Analysis;

using DataPair = System.Tuple<System.Double[][], System.Int32[]>;
using DataPartition = System.Tuple<System.Double[][], System.Double[][]>;
using System.Globalization;

namespace MachineLearning.Base
{
    public static class AlgorithmHelpers
    {
        #region Utilites
        //Utility function that parses the data from the file
        public static double[][] ReadMatrixFromFile(string filename)
        {
            //Get the lines
            var lines = System.IO.File.ReadAllLines(filename);
            Double[][] dataClass = lines
                                .Select(line =>
                                        line.Split(' ') //Split the lines to words
                                        .Where(chr => !(chr.Equals(""))) //Filter the "" strings
                                        .Select(word => Double.Parse(word, new CultureInfo("en-us"))) //Parse its string to the real number it represents 
                                        .ToArray())
                                .ToArray();
            return dataClass;
        }

        //Function for merging 2D arrays
        public static T[][] MergeArrays<T>(T[][] front, T[][] back)
        {
            return front.Concat(back).ToArray();
        }

        //Function for merging 1D arrays
        public static T[] MergeArrays<T>(T[] front, T[] back)
        {
            return front.Concat(back).ToArray();
        }

        public static double GetNonZero(int num) { return num == 0 ? 0.1 : num; }

        //Function that takes the computed confusion matrices
        //and calculates:
        //Average Accuracy, Average Accuracy per class, Average Recall per class
        public static Double[,] ProcessStatistics(ConfusionMatrix[,] data)
        {
            Double[,] average = new Double[4, 5];
            //For each algorithm
            for (int i = 0; i < 4; i++)
            {
                //For each confusion matrix
                for (int j = 0; j < 5; j++)
                {
                    //Calculate
                    average[i, 0] += data[i, j].Accuracy; //General accuracy
                    average[i, 1] += data[i, j].PredictedPositives / GetNonZero(data[i, j].ActualPositives + data[i, j].FalsePositives); //Accuracy for class 1
                    average[i, 2] += data[i, j].PredictedNegatives / GetNonZero(data[i, j].ActualNegatives + data[i, j].FalseNegatives); //Accuracy for class 2
                    average[i, 3] += data[i, j].TruePositives / GetNonZero(data[i, j].TruePositives + data[i, j].FalseNegatives); //Recall for class 1
                    average[i, 4] += data[i, j].TrueNegatives / GetNonZero(data[i, j].TrueNegatives + data[i, j].FalsePositives); //Recall fro class 2 

                }
            }

            return average.Apply(x => x / 5);
        }

        public static double[][] RandomizeArray(double[][] data)
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            double[][] MyRandomArray = data.OrderBy(x => rnd.Next()).ToArray();
            return MyRandomArray;
        }


        //Utility function that partitions the data
        //into to 80-20 subarrays
        public static DataPartition Partition(double[][] array)
        {
            //getting random indexes and putting them on a list
            var random = new Random(Guid.NewGuid().GetHashCode());
            var indices = new List<int>();
            while (indices.Count < array.Length)
            {
                int index = random.Next(0, (array.Length));
                if (indices.Count == 0 || !indices.Contains(index))
                {
                    indices.Add(index);
                }
            }
            //shuffling the array using the random indexes and put them in a list 
            var imarray = new double[20];
            var rnglist = new List<double[]>();
            for (int i = 0; i < indices.Count; i++)
            {
                int randomIndex = indices[i];
                imarray = array[randomIndex];
                rnglist.Add(imarray);
            }

            //splitting the list into two arrays
            var trainining_half = rnglist
                                  .Take(rnglist.Count * 80 / 100)
                                  .ToArray();

            var testing_half = rnglist
                                .Skip(rnglist.Count * 80 / 100)
                                .Take(rnglist.Count * 20 / 100)
                                .ToArray();

            //returning the two arrays in a tuple
            return Tuple.Create(trainining_half, testing_half);
        }

        //Utility function that takes the two classes
        //and creates the training and the testing data sets
        public static DataPair CreateDataPair(Double[][] classA, Double[][] classB)
        {   //Create a list from class A so we can append
            //the data of the second class
            var resKey = classA.ToList();
            //Append each vector from class B
            //Order doesnt matter
            foreach (var vector in classB)
            {
                resKey.Add(vector);
            }

            //Associate each vector to its class
            //eg {0.0,0.1} -> 1
            int[] resPair = new int[classA.Length + classB.Length];

            for (int i = 0; i < classA.Length + classB.Length; ++i)
            {
                //if we are still in the first class assign 0
                //else (i.e we are in the second class) assign 1
                resPair[i] = (i < classA.Length ? 0 : 1);
            }

            return Tuple.Create(resKey.ToArray(), resPair);
        }

        //Create a new matrix with only two columns of the original
        public static Double[][] ChooseFeatures(Double[][] data, int featuresNum = 2)
        {
            var ret = new Double[data.Length][];

            for (int i = 0; i < data.Length; ++i)
            {
                ret[i] = new Double[] { data[i][0], data[i][1] };
            }

            return ret;
        }

        //Utlitiy function that pads number 1 to the end of each vector in the matrix
        public static Double[][] PaddDimension(Double[][] vector)
        {
            var ret = new Double[vector.Length][];
            var padding = 1.0;

            for (int i = 0; i < vector.Length; ++i)
            {
                var inter = vector[i].ToList();
                inter.Add(padding);
                ret[i] = inter.ToArray();
            }

            return ret;
        }

        //Utility function that transform a jagged array to a multidimensional 
        public static Double[,] JaggedToMD(Double[][] array)
        {
            var rowSize = array.Length;
            var colSize = array[0].Length;

            var ret = new Double[rowSize, colSize];
            for (int i = 0; i < rowSize; ++i)
            {
                for (int j = 0; j < colSize; ++j)
                {
                    ret[i, j] = array[i][j];
                }
            }
            return ret;
        }

        public static int[] GetLeastSquaresOutput(int[] arr)
        {
            int[] res = new int[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                res[i] = arr[i] == 0 ? -1 : 1;
            }
            return res;
        }

        // It scales down (yeah I know) 
        public static Double[][] ScaleDown(Double[][] array)
        {
            return array
                .Select(x =>
                    x.Select(y => Math.Log10(y)).ToArray()
                    ).ToArray();
        }
        #endregion

    }
}
