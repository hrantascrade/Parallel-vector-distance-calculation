using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distances_Calculator
{
    class L1Distance : DistaceCalculator
    {
        public double Distance(List<float> v1, List<float> v2)
        {
            if (v1.Count != v2.Count)
                throw new ArgumentException("Vectors must be of the same size");

            double result = 0;
            for (int i = 0; i < v1.Count; ++i)
            {
                result += Math.Abs(v1[i] + v2[i]);
            }

            return result;
        }
    }
}
