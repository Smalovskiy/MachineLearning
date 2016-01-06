using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Analysis;
using AForge.Neuro;

namespace MachineLearning.Base
{
    public abstract class AlgorithmRuntime
    {
        protected const int POSITIVE = 0;
        protected const int NEGATIVE = 1;

        public event EventHandler<AlgorithmFinishedEventArgs> Finished;
        public void OnAlgorithmEnded(IEnumerable<ActivationNeuron> neurons, ConfusionMatrix matrix)
        {
            var handler = Finished;
            if (handler != null)
            {
                var e = new AlgorithmFinishedEventArgs()
                {
                    Neurons = neurons,
                    Matrix = matrix
                };
                handler(this, e);
            }
        }

        public abstract ConfusionMatrix Execute();
    }
}
