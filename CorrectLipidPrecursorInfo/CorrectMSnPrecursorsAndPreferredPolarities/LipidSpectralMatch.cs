using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace CorrectMSnPrecursorsAndPreferredPolarities
{
    public class LipidSpectralMatch
    {
        public int msnOrder = 1;
        public string lipidClass;
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
        public string precursorAdduct;

        public LipidSpectralMatch(CsvReader reader)
        {
            this.ms2 = reader[0];
            this.rt = Convert.ToDouble(reader[1]);
            this.rank = reader[2];
            this.id = reader[3];
            this.lipidClass = this.id.Split(' ')[0];
            if (this.lipidClass.Contains("CIDms2"))
            {
                this.msnOrder = 2;
            }
            if (this.lipidClass.Contains("CIDms3"))
            {
                this.msnOrder = 3;
            }
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

        public static void WriteSpectralMatchFileHeaders(StreamWriter writer)
        {
            writer.WriteLine("MS2 ID,Retention Time (min),Rank,Identification,Precursor Mass,Library Mass,Delta m/z,Dot Product," +
                "Reverse Dot Product,Purity,Spectral Components,Optimal Polarity,LipiDex Spectrum,Library,Potential Fragments");
        }

        public void WriteLipidSpectralMatch(StreamWriter writer)
        {
            writer.WriteLine(@"{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}", this.ms2, this.rt, this.rank, FormatIdentificationColumn(), this.precursorMz, this.libraryMz, this.deltaMz, 
                this.dotProduct, this.reverseDotProduct, this.purity, this.spectralComps, this.optimalPurity, this.lipidexSpectrum, this.libraryMz, this.potentialFragments);
        }

        private string FormatIdentificationColumn()
        {
            var splitID = this.id.Split(' ');

            if (string.IsNullOrWhiteSpace(this.precursorAdduct))
            {
                return string.Format("{0} {1} {2}", this.lipidClass, splitID[1], splitID[2]);
            }
            else
            {
                return string.Format("{0} {1} {2}", this.lipidClass, splitID[1], this.precursorAdduct);
            }
        }
    }
}
