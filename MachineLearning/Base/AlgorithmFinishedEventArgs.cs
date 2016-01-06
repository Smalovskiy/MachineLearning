using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Analysis;
using AForge.Neuro;

namespace MachineLearning.Base
{
    public class AlgorithmFinishedEventArgs : EventArgs
    {
        public IEnumerable<ActivationNeuron> Neurons { get; set; }
        public ConfusionMatrix Matrix { get; set; }
    }
}
