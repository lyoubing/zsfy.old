using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisModel
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

        private string certificate;

        public string Certificate
        {
            get { return certificate; }
            set { certificate = value; }
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

        private string interval;

        public string Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        private string ftpDIR;

        public string FtpDIR
        {
            get { return ftpDIR; }
            set { ftpDIR = value; }
        }

        private string ftpUser;

        public string FtpUser
        {
            get { return ftpUser; }
            set { ftpUser = value; }
        }
        private string ftpPwd;

        public string FtpPwd
        {
            get { return ftpPwd; }
            set { ftpPwd = value; }
        }

        private int querySeconds = 6;

        public int QuerySeconds
        {
            get { return querySeconds; }
            set { querySeconds = value; }
        }

        private string ccaURL;

        public string CCAURL
        {
            get { return ccaURL; }
            set { ccaURL = value; }
        }
    }
}
