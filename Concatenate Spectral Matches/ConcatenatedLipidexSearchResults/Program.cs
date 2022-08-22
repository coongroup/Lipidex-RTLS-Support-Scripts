using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LumenWorks.Framework.IO.Csv;


namespace ConcatenatedLipidexSearchResults
{
    class Program
    {
        static void Main(string[] args)
        {
            var basePath = @"T:\File_exchange\Dain\From Yuchen\NewSearch\Peak Finder\";

            Dictionary<string, SpectralMatchSet> matches = new Dictionary<string, SpectralMatchSet>();
            var hcdScanDir = @"HCD MS2s";
            var triggeredMs2Dir = @"Triggered MS2 - Modified RT";
            var triggeredMs3Dir = @"Triggered MS3 - Modified RT";
            

            var files = Directory.GetFiles(basePath + hcdScanDir, "*.csv")
                .Concat(Directory.GetFiles(basePath + triggeredMs2Dir, "*.csv"))
                .Concat(Directory.GetFiles(basePath + triggeredMs3Dir, "*.csv"))
                .ToList(); 

            foreach (var file in files)
            {
                var reader = new CsvReader(new StreamReader(file), true);
                var key = Path.GetFileNameWithoutExtension(file);

                // create dictionary entry if it doesn't exist yet
                if (!matches.ContainsKey(key))
                {
                    matches.Add(key, new SpectralMatchSet(key));
                }
                
                // cram all spectral matches into dictionary
                while (reader.ReadNextRecord())
                {
                    matches[key].lipidSpectralMatches.Add(new LipidSpectralMatch(reader));
                }
            }
           
            // now sort matches by rt, then ms2 ID, then write out results
            foreach (var value in matches.Values)
            {
                value.lipidSpectralMatches = value.lipidSpectralMatches.OrderBy(lsm => lsm.rt).ToList();

                if (!Directory.Exists(basePath + "ConcatenatedMultiSearch"))
                {
                    Directory.CreateDirectory(basePath + "ConcatenatedMultiSearch");
                }

                var writer = new StreamWriter(basePath + @"ConcatenatedMultiSearch\" + value.fileName + ".csv");

                // write headers
                writer.WriteLine("MS2 ID," +
                    "Retention Time (min)," +
                    "Rank," +
                    "Identification," +
                    "Precursor Mass," +
                    "Library Mass," +
                    "Delta m/z," +
                    "Dot Product," +
                    "Reverse Dot Product," +
                    "Purity," +
                    "Spectral Components," +
                    "Optimal Polarity," +
                    "LipiDex Spectrum," +
                    "Library," +
                    "Potential Fragments");

                foreach (var lsm in value.lipidSpectralMatches)
                {
                    writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}", lsm.ms2, lsm.rt, lsm.rank, lsm.id, lsm.precursorMz, lsm.libraryMz, lsm.deltaMz, lsm.dotProduct,
                        lsm.reverseDotProduct, lsm.purity, lsm.spectralComps, lsm.optimalPurity, lsm.lipidexSpectrum, lsm.lib, lsm.potentialFragments);
                }

                writer.Close();
                writer.Dispose();
            }
        }
    }
}
