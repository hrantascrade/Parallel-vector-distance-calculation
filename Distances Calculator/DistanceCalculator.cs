using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distances_Calculator
{
    interface DistaceCalculator
    {
        double Distance(List<float> v1, List<float> v2);
    }
}