using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;

namespace CorrectMSnPrecursorsAndPreferredPolarities
{
    public class SpectralMatchSet
    {
        public string lipidClass;
        public double rt;
        public string precursorMz;
        public LipidSpectralMatch[] matches = new LipidSpectralMatch[3];

        public SpectralMatchSet(LipidSpectralMatch match)
        {
            this.matches[0] = match;
            this.lipidClass = match.lipidClass;
            this.rt = match.rt;
            this.precursorMz = match.precursorMz;
        }

        public void AddNextMsnSeries(LipidSpectralMatch match)
        {
            if (match.id.Contains("CIDms2"))
            {
                if (this.matches[1] != null)
                {
                    throw new ArgumentException("This MS2 field has already been populated. Investigate what happened here.");
                }

                this.matches[1] = match;
            }
            else if (match.id.Contains("CIDms3"))
            {
                if (this.matches[2] != null)
                {
                    throw new ArgumentException("This MS3 field has already been populated. Investigate what happened here.");
                }

                this.matches[2] = match;
            }
            else
            {
                throw new ArgumentException("This object should not have been matched to this group of MSn. Investigate what happened here.");
            }
        }

        public bool CompareMsnClass(LipidSpectralMatch lsm)
        {
            if (lsm.msnOrder == 2)
            {
                lsm.lipidClass = lsm.lipidClass.Replace("-CIDms2", "");

                if (lsm.lipidClass.Equals(this.lipidClass))
                {
                    return true;
                }
            }

            if (lsm.msnOrder == 3)
            {
                lsm.lipidClass = lsm.lipidClass.Replace("-CIDms3", "");

                // now do some special comparisons to correct class names for GPLs and original lipid adduct
                if (lsm.lipidClass.Equals("GPL") && (this.lipidClass.Equals("PE") || this.lipidClass.Equals("PG") || this.lipidClass.Equals("PI") || this.lipidClass.Equals("PS")))
                {
                    lsm.lipidClass = this.lipidClass;
                    lsm.precursorAdduct = this.matches[0].id.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Last();
                    return true;
                }
                
                if (lsm.lipidClass.Equals(this.lipidClass))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
