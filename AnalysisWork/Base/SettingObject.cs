using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisWork.Base
{
    public class SettingObject
    {
        private string hospitalCode = string.Empty;

        public string HospitalCode
        {
            get { return hospitalCode; }
            set { hospitalCode = value; }
        }

        private string hospitalName = string.Empty;

        public string HospitalName
        {
            get { return hospitalName; }
            set { hospitalName = value; }
        }

        private string fileDIR;

        public string FileDIR
        {
            get { return fileDIR; }
            set { fileDIR = value; }
        }

        private string platURL;

        public string PlatURL
        {
            get { return platURL; }
            set { platURL = value; }
        }

        private string geIP;

        public string GEIP
        {
            get { return geIP; }
            set { geIP = value; }
        }

        private string gePort;

        public string GEPort
        {
            get { return gePort; }
            set { gePort = value; }
        }
    }
}
