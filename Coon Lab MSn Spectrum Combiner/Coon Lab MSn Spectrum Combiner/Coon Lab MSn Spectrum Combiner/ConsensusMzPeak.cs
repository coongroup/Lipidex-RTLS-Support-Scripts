using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coon_Lab_MSn_Spectrum_Combiner
{
    public class ConsensusMzPeak
    {
        public double mz;
        public double absoluteIntensity;
        public double relativeIntensity;

        public ConsensusMzPeak(double mz, double absoluteIntensity, double relativeIntensity)
        {
            this.mz = mz;
            this.absoluteIntensity = absoluteIntensity;
            this.relativeIntensity = relativeIntensity;
        }
    }
}
