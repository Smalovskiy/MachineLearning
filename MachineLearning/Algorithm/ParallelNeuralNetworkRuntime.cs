using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Accord.Statistics.Analysis;
using AForge.Neuro;
using AForge.Neuro.Learning;
using MachineLearning.Base;

namespace MachineLearning.Algorithm
{
    class ParallelNeuralNetworkRuntime : AlgorithmRuntime
    {
        private readonly Double[][] trainingSet;
        private readonly Double[][] trainingOutput;
        private readonly Double[][] testSet;
        private readonly int[] expected;

        public ParallelNeuralNetworkRuntime(Double[][] trainingSet, Double[][] trainingOutput, Double[][] testSet, int[] expected)
        {
            this.trainingSet = trainingSet;
            this.trainingOutput = trainingOutput;
            this.testSet = testSet;
            this.expected = expected;
        }
        public override ConfusionMatrix Execute()
        {
            var richTesting = testSet;

            ConcurrentDictionary<double, ActivationNetwork> networks = new ConcurrentDictionary<double, ActivationNetwork>();

            Parallel.For(0, 2000, (int i) =>
            {
                //Create an activation network
                ThreadLocal<ActivationNetwork> network = new ThreadLocal<ActivationNetwork>(() =>
                {
                    return new ActivationNetwork(new SigmoidFunction(2), 2, 2, 1);
                });

                ThreadLocal<ResilientBackpropagationLearning> teachear = new ThreadLocal<ResilientBackpropagationLearning>(() =>
                {
                    return new ResilientBackpropagationLearning(network.Value);
                });

                ThreadLocal<int> iter = new ThreadLocal<int>(() => { return 0; });
                ThreadLocal<double> error = new ThreadLocal<double>(() => { return 0; });

                while (networks.IsEmpty)
                {
                    error.Value = teachear.Value.RunEpoch(trainingSet, trainingOutput);
                    iter.Value++;
                    if (iter.Value == 1000) break;
                }
                if (!networks.ContainsKey(error.Value)) networks.TryAdd(error.Value, network.Value);
            }
            );

            Dictionary<ConfusionMatrix, ActivationNetwork> cms = new Dictionary<ConfusionMatrix, ActivationNetwork>();
            foreach (var keyv in networks)
            {
                var p = richTesting
                   .Select(x => keyv.Value.Compute(x))
                   .Select(x => Convert.ToInt32(x[0]))
                   .ToArray();

                ConfusionMatrix cm = new ConfusionMatrix(p, expected, POSITIVE, NEGATIVE);
                cms.Add(cm, keyv.Value);
            }

            var kv = (from x in cms
                      orderby x.Key.Accuracy descending
                      select x).First();

            var neurons = from neuron in kv.Value.Layers[0].Neurons
                          select neuron as ActivationNeuron;

            OnAlgorithmEnded(neurons, kv.Key);
            return kv.Key;
        }
    }
}
