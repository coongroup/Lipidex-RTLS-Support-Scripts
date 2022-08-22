using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CollapseSnIsomerPurities
{
    public class LipidComponent
    {
        public string componentString;
        public string lipidClass;
        public string adduct;
        public List<string> FAs;
        public int purity;

        public LipidComponent(string component)
        {
            if (string.IsNullOrWhiteSpace(component))
            {
                componentString = component;
            }
            else
            {
                var firstSplit = component.Split(' ');

                this.lipidClass = firstSplit[0];
                this.FAs = firstSplit[1].Split('_').ToList();
                var reggie1 = new Regex(@".+\((.+)\)");
                var reggie2 = new Regex(@".+(\[.+\].)\(");
                this.purity = Convert.ToInt32(reggie1.Match(component).Groups[1].Value);
                this.adduct = reggie2.Match(component).Groups[1].Value;
            }         
        }

        public string CreateComponentString()
        {
            if (string.IsNullOrWhiteSpace(lipidClass))
            {
                return string.Empty; 
            }
            else
            {
                return string.Format("{0} {1} {2}({3})", this.lipidClass, this.CollapseFAs(), this.adduct, this.purity);
            }
        }

        public string CreateIdentificationString()
        {
            if (string.IsNullOrWhiteSpace(lipidClass))
            {
                return string.Empty;
            }
            else
            {
                return string.Format("{0} {1} {2};", this.lipidClass, this.CollapseFAs(), this.adduct);
            }
        }

        private string CollapseFAs()
        {
            return String.Join("_", FAs.ToArray());
        }

        public bool IsPositionalIsomer(LipidComponent other)
        {
            if (this.lipidClass.Equals(other.lipidClass) && this.adduct.Equals(other.adduct))
            {
                var couldBeIsomer = true;
                foreach (var fa in FAs)
                {
                    if (couldBeIsomer)
                    {
                        if (other.FAs.Contains(fa) && other.FAs.Where(x => x != null && x.Equals(fa)).Count() == this.FAs.Where(x => x != null && x.Equals(fa)).Count())
                        {
                            continue;
                        }
                        else
                        {
                            couldBeIsomer = false;
                            break;
                        }
                    }
                }

                return couldBeIsomer;
            }
            else
            {
                return false;
            }
        }
    }

}
