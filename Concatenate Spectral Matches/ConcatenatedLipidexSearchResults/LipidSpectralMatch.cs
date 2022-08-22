using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;

namespace ConcatenatedLipidexSearchResults
{
    public class LipidSpectralMatch
    {
        public string ms2;
        public double rt;
        public string rank;
        public string id;
        public string precursorMz;
        public string libraryMz;
        public string deltaMz;
        public string dotProduct;
        public string reverseDotProduct;
        public string purity;
        public string spectralComps;
        public string optimalPurity;
        public string lipidexSpectrum;
        public string lib;
        public string potentialFragments;

        public LipidSpectralMatch(CsvReader reader)
        {
            this.ms2 = reader[0];
            this.rt = Convert.ToDouble(reader[1]);
            this.rank = reader[2];
            this.id = reader[3];
            this.precursorMz = reader[4];
            this.libraryMz = reader[5];
            this.deltaMz = reader[6];
            this.dotProduct = reader[7];
            this.reverseDotProduct = reader[8];
            this.purity = reader[9];
            this.spectralComps = reader[10];
            this.optimalPurity = reader[11];
            this.lipidexSpectrum = reader[12];
            this.lib = reader[13];
            this.potentialFragments = reader[14];
        }
    }
}
