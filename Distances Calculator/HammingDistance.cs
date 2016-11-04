using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distances_Calculator
{
    class HammingDistance : DistaceCalculator
    {
        public double Distance(List<float> v1, List<float> v2)
        {
            if (v1.Count != v2.Count)
                throw new ArgumentException("Vectors must be of the same size");

            double result = 0;
            for (int i = 0; i < v1.Count; ++i)
            {
                if (!v1[i].Equals(v2[i]))
                    result++;
            }

            return result;
        }
    }
}
