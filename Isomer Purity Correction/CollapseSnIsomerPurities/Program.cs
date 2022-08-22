using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;
using System.IO;

namespace CollapseSnIsomerPurities
{
    class Program
    {
        static void Main(string[] args)
        {
            var basePath = @"T:\File_exchange\Dain\From Yuchen\NewSearch\Peak Finder\ConcatenatedMultiSearch\";

            var resultFiles = Directory.GetFiles(basePath, "*.csv");

            foreach (var file in resultFiles)
            {
                var reader = new CsvReader(new StreamReader(file));

                var lipidSpectralMatches = new List<LipidSpectralMatch>();

                while (reader.ReadNextRecord())
                {
                    lipidSpectralMatches.Add(new LipidSpectralMatch(reader));
                }

                // now combine sn positional isomer purities if they exist
                foreach (var match in lipidSpectralMatches)
                {
                    if (match.spectralComponents.Count > 1)
                    {
                        for (var i = 0; i < match.spectralComponents.Count; i++)
                        {
                            for (var j = i + 1; j < match.spectralComponents.Count; j++)
                            {
                                if (match.spectralComponents[i].IsPositionalIsomer(match.spectralComponents[j]))
                                {
                                    //Cer[AS] d14:1_22:2 [M-H]-(60) / Cer[AS] d18:0_18:3 [M-H]-(23) / Cer[AS] d18:1_18:2 [M-H]-(17)
                                    match.spectralComponents[i].purity += match.spectralComponents[j].purity;
                                    match.purity = match.spectralComponents[i].purity;
                                    match.spectralComponents.RemoveAt(j--);
                                }
                            }
                        }
                    }
                }

                // go through all spectral matches. Reselect most abundant spectral component if that has changed after re-selection
                foreach (var match in lipidSpectralMatches)
                {
                    if (match.spectralComponents.Count > 1)
                    {
                        var originalFirst = match.spectralComponents.First();

                        match.spectralComponents = match.spectralComponents.OrderByDescending(component => component.purity).ToList();

                        match.id = match.spectralComponents.First().CreateIdentificationString();
                        match.purity = match.spectralComponents.First().purity;
                    }
                }
                    
                // now write out updated purities
                if (!Directory.Exists(basePath + @"..\CollapsedPosIsos\"))
                {
                    Directory.CreateDirectory(basePath + @"..\CollapsedPosIsos\");
                }

                StreamWriter writer = new StreamWriter(basePath + @"..\CollapsedPosIsos\" + Path.GetFileName(file));
                // write headers
                writer.WriteLine("MS2 ID,Retention Time (min),Rank,Identification,Precursor Mass,Library Mass,Delta m/z," +
                    "Dot Product,Reverse Dot Product,Purity,Spectral Components,Optimal Polarity,LipiDex Spectrum," +
                    "Library,Potential Fragments");

                foreach (var lsm in lipidSpectralMatches)
                {
                    writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
                        lsm.msID, lsm.rt, lsm.rank, lsm.id, lsm.precursorMz, lsm.libraryMz, lsm.deltaMz, lsm.dotProd, lsm.revDotProd,
                        lsm.purity, lsm.ConstructSpectralComponents(), lsm.optimalPolarity, lsm.lipidexSpectrum, lsm.library, lsm.potentialFragments);
                }

                writer.Close();
                writer.Dispose();
            }
        }
    }
}
