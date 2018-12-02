using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisModel
{
    public class DiagNose
    {
        private string icd_code;

        public string ICD_Code
        {
            get { return icd_code; }
            set { icd_code = value; }
        }
        private string icd_name;

        public string ICD_Name
        {
            get { return icd_name; }
            set { icd_name = value; }
        }
        private string dType;

        public string DType
        {
            get { return dType; }
            set { dType = value; }
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new DiagNose Clone()
        {
            return this.MemberwiseClone() as DiagNose;
        }
    }
}
