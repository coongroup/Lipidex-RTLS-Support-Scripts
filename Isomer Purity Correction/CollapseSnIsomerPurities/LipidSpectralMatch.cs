using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;

namespace CollapseSnIsomerPurities
{
    public class LipidSpectralMatch
    {
        public string msID;
        public string rt;
        public string rank;
        public string id;
        public string precursorMz;
        public string libraryMz;
        public string deltaMz;
        public string dotProd;
        public string revDotProd;
        public double purity;
        public List<LipidComponent> spectralComponents;
        public string optimalPolarity;
        public string lipidexSpectrum;
        public string library;
        public string potentialFragments;

        public LipidSpectralMatch(CsvReader reader)
        {
            this.msID = reader[0];
            this.rt = reader[1];
            this.rank = reader[2];
            this.id = reader[3];
            this.precursorMz = reader[4];
            this.libraryMz = reader[5];
            this.deltaMz = reader[6];
            this.dotProd = reader[7];
            this.revDotProd = reader[8];
            this.purity = Convert.ToDouble(reader[9]);
            this.spectralComponents = ParseSpectralComponents(reader[10]);
            this.optimalPolarity = reader[11];
            this.lipidexSpectrum = reader[12];
            this.library = reader[13];
            this.potentialFragments = reader[14];
        }

        private List<LipidComponent> ParseSpectralComponents(string inputString)
        {
            var returnList = new List<LipidComponent>();

            var splitString = inputString.Split('/');

            if (this.id.Equals("Cer[AS] d18:2_18:1 [M-H]-;"))
            {
                var t = "";
            }
            foreach (var chunk in splitString)
            {
                returnList.Add(new LipidComponent(chunk.Trim()));
            }

            return returnList;
        }

        public string ConstructSpectralComponents()
        {
            var returnString = "";

            if (this.spectralComponents.Count > 0)
            {
                returnString += spectralComponents[0].CreateComponentString();

                for (var i = 1; i < spectralComponents.Count; i++)
                {
                    returnString += " / " + spectralComponents[i].CreateComponentString();
                }
            }

            return returnString;
        }
    }
}
