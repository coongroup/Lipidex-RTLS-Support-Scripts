using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcatenatedLipidexSearchResults
{
    public class SpectralMatchSet
    {
        public string fileName;
        public List<LipidSpectralMatch> lipidSpectralMatches;

        public SpectralMatchSet(string fileName)
        {
            this.fileName = fileName;
            this.lipidSpectralMatches = new List<LipidSpectralMatch>();
        }
    }
}
