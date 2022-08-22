using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL.IO.Thermo;

namespace Coon_Lab_MSn_Spectrum_Combiner
{
    public class ConsensusMS2
    {
        public double scanNumber;
        public List<ConsensusMzPeak> peaks;

        public ConsensusMS2()
        {
            this.scanNumber = -1;
            this.peaks = new List<ConsensusMzPeak>();
        }

        public void AddSpectrumToConsensus(ThermoSpectrum spectrum)
        {
            // find basepeak intensity
            double basepeakIntensity = -1;

            foreach (var mzPeak in spectrum)
            {
                if (basepeakIntensity < mzPeak.Intensity)
                {
                    basepeakIntensity = mzPeak.Intensity;
                }
            }

            foreach (var mzPeak in spectrum)
            {
                var newPeak = new ConsensusMzPeak(mzPeak.MZ, mzPeak.Intensity, 1000 * mzPeak.Intensity / basepeakIntensity);
                peaks.Add(newPeak);
            }

            peaks = peaks.OrderBy(peak => peak.mz).ToList();
        }
    }
}
