using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using MachineLearning.Base;

namespace MachineLearning.Clustering
{
    public struct BsasArgs
    {
        private Tuple<double, double> _Thetavalues;
        private int _S;
        private double[][] _Data;
        private double _ThetaStepNum;
        /// <summary>
        /// Summary for BasicSequentialAlgorithmicSchemeArgs
        /// </summary>
        public BsasArgs(Tuple<double, double> thetavalues, int s, double[][] data, double thetaStepNum)
        {
            _Thetavalues = thetavalues;
            _S = s;
            _Data = data;
            _ThetaStepNum = thetaStepNum;
        }
        public Tuple<double, double> Thetavalues
        {
            get
            {
                return _Thetavalues;
            }
        }
        public int S
        {
            get
            {
                return _S;
            }
        }
        public double[][] Data
        {
            get
            {
                return _Data;
            }
        }
        public double ThetaStepNum
        {
            get
            {
                return _ThetaStepNum;
            }
        }
    }
}
