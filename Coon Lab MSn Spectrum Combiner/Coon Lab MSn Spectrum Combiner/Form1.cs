using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CSMSL;
using CSMSL.IO.Thermo;
using CSMSL.Proteomics;

namespace Coon_Lab_MSn_Spectrum_Combiner
{
    public partial class Form1 : Form
    {
        public bool filterLowAbundanceSpecies = true;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                textBox1.Text = folderDlg.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                if (numericUpDown1.Value <= 0)
                {
                    // Initializes the variables to pass to the MessageBox.Show method.
                    string message = "Start Retention Time must be greater than 0.";
                    string caption = "Error Detected in Input";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    // Displays the MessageBox.
                    result = MessageBox.Show(message, caption, buttons);
                    if (result == DialogResult.OK)
                    {
                        // Closes the parent form.
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            var basePath = textBox1.Text;
            var rawfiles = Directory.GetFiles(basePath, "*.raw");

            //use this if all raw files are in the same base path
            //var mgfPath = textBox1.Text + @"\..\MGFs\";
            var mgfPath = textBox1.Text + @"\MGFs\";

            if (!Directory.Exists(mgfPath))
            {
                Directory.CreateDirectory(mgfPath);
            }

            if (!Directory.Exists(mgfPath + @"HCD MS2s\"))
            {
                Directory.CreateDirectory(mgfPath + @"HCD MS2s\");
            }

            if (!Directory.Exists(mgfPath + @"Triggered MS2 - Modified RT\"))
            {
                Directory.CreateDirectory(mgfPath + @"Triggered MS2 - Modified RT\");
            }

            if (!Directory.Exists(mgfPath + @"Triggered MS3 - Modified RT\"))
            {
                Directory.CreateDirectory(mgfPath + @"Triggered MS3 - Modified RT\");
            }

            /*
            if (!Directory.Exists(mgfPath + @"All MS2s\"))
            {
                Directory.CreateDirectory(mgfPath + @"All MS2s\");
            }
            if (!Directory.Exists(mgfPath + @"Triggered MSn - Modified RT\"))
            {
                Directory.CreateDirectory(mgfPath + @"Triggered MSn - Modified RT\");
            }
            if (!Directory.Exists(mgfPath + @"Subbed CID MS2s\"))
            {
                Directory.CreateDirectory(mgfPath + @"Subbed CID MS2s\");
            }
            if (!Directory.Exists(mgfPath + @"MS3s\"))
            {
                Directory.CreateDirectory(mgfPath + @"MS3s\");
            }

            if (!Directory.Exists(mgfPath + @"CID MS2s"))
            {
                Directory.CreateDirectory(mgfPath + @"CID MS2s");
            }
            if (!Directory.Exists(mgfPath + @"Combined MSn Spectra"))
            {
                Directory.CreateDirectory(mgfPath + @"Combined MSn Spectra");
            }
            */

            foreach (var file in rawfiles)
            {
                WriteOutRawFileMGFs(mgfPath + @"HCD MS2s\", file, "HCD Only");
                WriteOutRawFileMGFs(mgfPath + @"Triggered MS2 - Modified RT\", file, "Triggered MS2 - Modified RT");
                WriteOutRawFileMGFs(mgfPath + @"Triggered MS3 - Modified RT\", file, "Triggered MS3 - Modified RT");
                
                //WriteOutRawFileMGFs(mgfPath + @"All MS2s\", file);
                //WriteOutRawFileMGFs(mgfPath + @"Triggered MSn - Modified RT\", file, "Triggered MSn - Modified RT");
                //WriteOutRawFileMGFs(mgfPath + @"Subbed CID MS2s\", file, "CID Subbed");
                //WriteOutRawFileMGFs(mgfPath + @"CID MS2s\", file, "CID MS2s");
                //WriteOutRawFileMGFs(mgfPath + @"MS3s\", file, "MS3 Only");
                //WriteOutRawFileMGFs(mgfPath + @"Combined MSn Spectra\", file, "Combined MSn");
            }
        }

        public void WriteOutRawFileMGFs(string basePath, string file, string mgfType = "")
        {
            var rawfile = new ThermoRawFile(file);
            rawfile.Open();
            var writer = new StreamWriter(basePath + Path.GetFileNameWithoutExtension(file) + ".mgf");

            switch (mgfType)
            {
                case "HCD Only":
                    // code block
                    var y = rawfile.FirstSpectrumNumber;
                    var lasty = rawfile.LastSpectrumNumber;

                    if (checkBox2.Checked)
                    {
                        y = rawfile.GetSpectrumNumber((double)numericUpDown1.Value);
                    }

                    for (y = y; y <= rawfile.LastSpectrumNumber; y++)
                    {
                        if (rawfile.GetMsnOrder(y) == 2)
                        {
                            if (rawfile.GetDissociationType(y).Equals(DissociationType.HCD))
                            {
                                writer.WriteLine("BEGIN IONS");
                                writer.Write("TITLE={0}.{1}.{2}.1", Path.GetFileNameWithoutExtension(file), y, y);
                                writer.WriteLine("File:\"{0}\", NativeID:\"controllerType=0 controllerNumber=1 scan={1}\"", Path.GetFileName(file), y);
                                writer.WriteLine("RTINSECONDS={0}", (rawfile.GetRetentionTime(y) * 60));
                                //writer.WriteLine("PEPMASS={0}", rawfile.GetPrecursorMz(i));
                                writer.WriteLine("PEPMASS={0}", GetMonoisotopicMz(rawfile, y));

                                foreach (var peak in rawfile.GetSpectrum(y))
                                {
                                    writer.WriteLine("{0} {1}", peak.MZ, peak.Intensity);
                                }

                                writer.WriteLine("END IONS");
                            }
                        }
                    }

                    break;
                   
                case "CID Subbed":
                    // code block
                    for (var i = rawfile.FirstSpectrumNumber; i <= rawfile.LastSpectrumNumber; i++)
                    {
                        if (rawfile.GetMsnOrder(i) == 2)
                        {
                            writer.WriteLine("BEGIN IONS");
                            writer.WriteLine("TITLE={0}.FTMS.HCD.{1}.{2}.3.RT_{3}_min_{4}_s", Path.GetFileNameWithoutExtension(file), i, i, Math.Round(rawfile.GetRetentionTime(i), 3), Math.Round(rawfile.GetRetentionTime(i) * 60, 1));
                            writer.WriteLine("SCANS={0}", i);
                            writer.WriteLine("RTINSECONDS={0}", Math.Round(rawfile.GetRetentionTime(i) * 60, 1));
                            writer.WriteLine("PEPMASS={0}", rawfile.GetPrecursorMz(i));
                            writer.WriteLine("CHARGE={0}+", rawfile.GetPrecusorCharge(i));
                            foreach (var peak in rawfile.GetSpectrum(i))
                            {
                                writer.WriteLine("{0} {1}", peak.MZ, peak.Intensity);
                            }

                            writer.WriteLine("END IONS");
                            writer.WriteLine("");
                        }
                       
                    }

                    break;
                /* 
            case "MS3 Only":
                // code block
                for (var i = rawfile.FirstSpectrumNumber; i <= rawfile.LastSpectrumNumber; i++)
                {
                    if (rawfile.GetMsnOrder(i) == 3)
                    {
                        writer.WriteLine("BEGIN IONS");
                        writer.Write("TITLE={0}.{1}.{2}.1 ", Path.GetFileNameWithoutExtension(file), i, i);
                        writer.WriteLine("File:\"{0}\", NativeID:\"controllerType=0 controllerNumber=1 scan={1}\"", Path.GetFileName(file), i);
                        writer.WriteLine("RTINSECONDS={0}", (rawfile.GetRetentionTime(i) * 60));
                        var precursor = rawfile.GetPrecursorMz(i, 3);
                        writer.WriteLine("PEPMASS={0}", rawfile.GetPrecursorMz(i, 3));
                        var spectrum = rawfile.GetSpectrum(1);
                        spectrum.GetNoise(1);

                        foreach (var peak in rawfile.GetSpectrum(i))
                        {
                            writer.WriteLine("{0} {1}", peak.MZ, peak.Intensity);
                        }

                        writer.WriteLine("END IONS");
                    }
                }
                break;
            case "CID MS2s":
                // code block
                for (var i = rawfile.FirstSpectrumNumber; i <= rawfile.LastSpectrumNumber; i++)
                {
                    if (rawfile.GetMsnOrder(i) == 2)
                    {
                        if (rawfile.GetDissociationType(i).Equals(DissociationType.CID))
                        {
                            writer.WriteLine("BEGIN IONS");
                            writer.Write("TITLE={0}.{1}.{2}.1", Path.GetFileNameWithoutExtension(file), i, i);
                            writer.WriteLine("File:\"{0}\", NativeID:\"controllerType=0 controllerNumber=1 scan={1}\"", Path.GetFileName(file), i);
                            writer.WriteLine("RTINSECONDS={0}", (rawfile.GetRetentionTime(i) * 60));
                            writer.WriteLine("PEPMASS={0}", rawfile.GetPrecursorMz(i));

                            foreach (var peak in rawfile.GetSpectrum(i))
                            {
                                writer.WriteLine("{0} {1}", peak.MZ, peak.Intensity);
                            }

                            writer.WriteLine("END IONS");
                        }
                    }
                }
                break;

            case "Combined MSn":
                // code block
                var j = rawfile.FirstSpectrumNumber;
                var last = rawfile.LastSpectrumNumber;
                if (checkBox2.Checked)
                {
                    j = rawfile.GetSpectrumNumber((double)numericUpDown1.Value);
                }

                for (j = j; j <= rawfile.LastSpectrumNumber; j++)
                {
                    if (rawfile.GetMsnOrder(j) == 2)
                    {
                        if (rawfile.GetDissociationType(j).Equals(DissociationType.HCD))
                        {
                            writer.WriteLine("BEGIN IONS");
                            writer.Write("TITLE={0}.{1}.{2}.1", Path.GetFileNameWithoutExtension(file), j, j);
                            writer.WriteLine("File:\"{0}\", NativeID:\"controllerType=0 controllerNumber=1 scan={1}\"", Path.GetFileName(file), j);
                            writer.WriteLine("RTINSECONDS={0}", (rawfile.GetRetentionTime(j) * 60));
                            writer.WriteLine("PEPMASS={0}", GetMonoisotopicMz(rawfile, j));
                            //writer.WriteLine("PEPMASS={0}", rawfile.GetPrecursorMz(j));

                            var outputSpectrum = CreateConsensusSpectrum(j, rawfile);

                            foreach (var peak in outputSpectrum.peaks)
                            {
                                writer.WriteLine("{0} {1}", peak.mz, peak.relativeIntensity);
                            }

                            writer.WriteLine("END IONS");
                        }
                    }
                }
                break;
            */
                //"Write out all Ms2 CID scans with triggered HCD MS2 RT"
                case "Triggered MS2 - Modified RT":
                    // code block
                    var l = rawfile.FirstSpectrumNumber;
                    var lastl = rawfile.LastSpectrumNumber;

                    if (checkBox2.Checked)
                    {
                        l = rawfile.GetSpectrumNumber((double)numericUpDown1.Value);
                    }

                    for (l = l; l <= rawfile.LastSpectrumNumber; l++)
                    {
                        // you can write out each scan independently. Just remember to always nab the parent spectrum RT
                        if (rawfile.GetMsnOrder(l) == 2 && rawfile.GetDissociationType(l).Equals(DissociationType.CID))
                        {
                            writer.WriteLine("BEGIN IONS");
                            writer.Write("TITLE={0}.{1}.{2}.1", Path.GetFileNameWithoutExtension(file), l, l);
                            writer.WriteLine("File:\"{0}\", NativeID:\"controllerType=0 controllerNumber=1 scan={1}\"", Path.GetFileName(file), l);

                            var parentSpectrumNum = RecursiveGetParentSpectrumNumber(l, rawfile);

                            writer.WriteLine("RTINSECONDS={0}", (rawfile.GetRetentionTime(parentSpectrumNum) * 60));
                            writer.WriteLine("PEPMASS={0}", GetMonoisotopicMz(rawfile, l));

                            foreach (var peak in rawfile.GetSpectrum(l))
                            {
                                writer.WriteLine("{0} {1}", peak.MZ, peak.Intensity);
                            }

                            writer.WriteLine("END IONS");
                        }
                    }
                    break;
                //"Write out all Msn with triggered HCD MS3 RT"
                case "Triggered MS3 - Modified RT":
                    // code block
                    var k = rawfile.FirstSpectrumNumber;
                    var lastk = rawfile.LastSpectrumNumber;
                    
                    if (checkBox2.Checked)
                    {
                        k = rawfile.GetSpectrumNumber((double)numericUpDown1.Value);
                    }

                    for (k = k; k <= rawfile.LastSpectrumNumber; k++)
                    {
                        // you can write out each scan independently. Just remember to always nab the parent spectrum RT
                        
                        if (rawfile.GetMsnOrder(k) == 3 && rawfile.GetDissociationType(k, 3).Equals(DissociationType.CID))
                        {
                            writer.WriteLine("BEGIN IONS");
                            writer.Write("TITLE={0}.{1}.{2}.1", Path.GetFileNameWithoutExtension(file), k, k);
                            writer.WriteLine("File:\"{0}\", NativeID:\"controllerType=0 controllerNumber=1 scan={1}\"", Path.GetFileName(file), k);

                            var parentSpectrumNum = RecursiveGetParentSpectrumNumber(k, rawfile);

                            writer.WriteLine("RTINSECONDS={0}", (rawfile.GetRetentionTime(parentSpectrumNum) * 60));
                            writer.WriteLine("PEPMASS={0}", rawfile.GetPrecursorMz(k, 3));



                            foreach (var peak in rawfile.GetSpectrum(k))
                            {
                                writer.WriteLine("{0} {1}", peak.MZ, peak.Intensity);
                            }

                            writer.WriteLine("END IONS");
                        }
                    }
                    break;

                default:
                    // code block
                    /*
                    for (var i = rawfile.FirstSpectrumNumber; i <= rawfile.LastSpectrumNumber; i++)
                    {
                        if (rawfile.GetMsnOrder(i) > 1)
                        {
                            writer.WriteLine("BEGIN IONS");
                            writer.Write("TITLE={0}.{1}.{2}.1", Path.GetFileNameWithoutExtension(file), i, i);
                            writer.WriteLine("File:\"{0}\", NativeID:\"controllerType=0 controllerNumber=1 scan={1}\"", Path.GetFileName(file), i);
                            writer.WriteLine("RTINSECONDS={0}", (rawfile.GetRetentionTime(i) * 60));
                            writer.WriteLine("PEPMASS={0}", rawfile.GetPrecursorMz(i));

                            foreach (var peak in rawfile.GetSpectrum(i))
                            {
                                writer.WriteLine("{0} {1}", peak.MZ, peak.Intensity);
                            }

                            writer.WriteLine("END IONS");
                        }
                    }
                    */
                    break;

            }

            writer.Close();
            writer.Dispose();

            rawfile.ClearCachedScans();
            rawfile.Dispose();
        }

        public int RecursiveGetParentSpectrumNumber(int spectrumNumber, ThermoRawFile rawfile)
        {
            var parentSpectrumNumber = rawfile.GetParentSpectrumNumber(spectrumNumber);

            if (rawfile.GetMsnOrder(parentSpectrumNumber) == 1)
            {
                return spectrumNumber;
            }
            else
            {
                return RecursiveGetParentSpectrumNumber(parentSpectrumNumber, rawfile);
            }
        }

        public List<int> SubCidSpectraForOgHcdSpectra(ThermoRawFile rawfile)
        {
            var returnList = new List<int>();

            for (var i = rawfile.FirstSpectrumNumber; i <= rawfile.LastSpectrumNumber; i++)
            {
                if (rawfile.GetMsnOrder(i) == 2)
                {
                    // HCD parent scans will always come before CID scans
                    if (rawfile.GetDissociationType(i).Equals(DissociationType.CID))
                    {
                        var parentNum = rawfile.GetParentSpectrumNumber(i);

                        if (returnList.Contains(parentNum))
                        {
                            var index = returnList.IndexOf(parentNum);
                            returnList[index] = i;
                        }
                    }
                    else
                    {
                        returnList.Add(i);
                    }
                }
            }

            return returnList;
        }

        public ConsensusMS2 CreateConsensusSpectrum(int hcdSpectrumNumber, ThermoRawFile rawfile)
        {
            var returnSpectrum = new ConsensusMS2();

            returnSpectrum.AddSpectrumToConsensus(rawfile.GetSpectrum(hcdSpectrumNumber));
            List<int> parentSpectra = new List<int>();
            parentSpectra.Add(hcdSpectrumNumber);

            for (var i = 1; (i <= 20) && (i + hcdSpectrumNumber <= rawfile.LastSpectrumNumber); i++)
            {
                if (parentSpectra.Contains(rawfile.GetParentSpectrumNumber(hcdSpectrumNumber + i)))
                {
                    parentSpectra.Add(hcdSpectrumNumber + i);
                    returnSpectrum.AddSpectrumToConsensus(rawfile.GetSpectrum(hcdSpectrumNumber + i));
                }
            }

            // filter out anything below 0.05 rel abundance if filterLowAbundancePeaks is checked.
            if (filterLowAbundanceSpecies)
            {
                for (var i = 0; i < returnSpectrum.peaks.Count; i++)
                {
                    if (returnSpectrum.peaks[i].relativeIntensity < 5)
                    {
                        returnSpectrum.peaks.RemoveAt(i--);
                    }
                }
            }

            return returnSpectrum;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            filterLowAbundanceSpecies = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = checkBox2.Checked;
        }

        private double GetMonoisotopicMz(ThermoRawFile rawfile, int scanNumber)
        {
            List<string> returnLabels = new List<string>();
            List<string> returnValues = new List<string>();

            rawfile.GetAllTrailingHeaderData(scanNumber, out returnLabels, out returnValues);

            var monoisotopeIndex = returnLabels.IndexOf("Monoisotopic M/Z:");

            if (monoisotopeIndex != -1)
            {
                var parsedMonoisotope = Convert.ToDouble(returnValues[monoisotopeIndex]);

                if (parsedMonoisotope != 0)
                {
                    return parsedMonoisotope;
                }
            }

            return rawfile.GetPrecursorMz(scanNumber);
        }
    }
}
