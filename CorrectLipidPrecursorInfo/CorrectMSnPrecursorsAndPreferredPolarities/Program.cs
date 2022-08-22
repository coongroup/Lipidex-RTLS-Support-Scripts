using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace CorrectMSnPrecursorsAndPreferredPolarities
{
    class Program
    {
        static void Main(string[] args)
        {
            var basePath = @"T:\File_exchange\Dain\From Yuchen\NewSearch\Peak Finder\CollapsedPosIsos\";

            var files = Directory.GetFiles(basePath, "*.csv").ToList();

            foreach (var file in files)
            {
                List<SpectralMatchSet> matches = new List<SpectralMatchSet>();

                var reader = new CsvReader(new StreamReader(file, true));

                while (reader.ReadNextRecord())
                {
                    var lsm = new LipidSpectralMatch(reader);

                    if (lsm.msnOrder == 1)
                    {
                        matches.Add(new SpectralMatchSet(lsm));
                    }
                    else
                    {
                        foreach (var match in matches)
                        {
                            if (match.rt == lsm.rt && match.CompareMsnClass(lsm))
                            {
                                match.matches[lsm.msnOrder - 1] = lsm;

                                // if lsm order is 3, fix the precursor mz & lipid adduct
                                if (lsm.msnOrder == 3)
                                {
                                    lsm.precursorMz = match.matches[0].precursorMz;
                                    lsm.libraryMz = match.matches[0].libraryMz;
                                }
                            } 
                        }
                    }
                }

                // now that spectral match trees are grouped, write out the nested spectral matches!
                string outPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), @"..\FinalLipidSpectralMatches\"));

                if (!Directory.Exists(outPath))
                {
                    Directory.CreateDirectory(outPath);
                }

                var writer = new StreamWriter(outPath + Path.GetFileName(file));

                // write headers
                LipidSpectralMatch.WriteSpectralMatchFileHeaders(writer);

                foreach (var match in matches)
                {
                    foreach (var spectralMatch in match.matches)
                    {
                        if(spectralMatch != null)
                        {
                            spectralMatch.WriteLipidSpectralMatch(writer);
                        }
                    }
                }

                writer.Close();
                writer.Dispose();
            }
        }
    }
}
