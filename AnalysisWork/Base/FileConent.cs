using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisWork.Base
{
    public class FileConent
    {
        private PatientInfo patient;

        public PatientInfo Patient
        {
            get { return patient; }
            set { patient = value; }
        }

        private OrderItem orderItem;

        public OrderItem OrderItem
        {
            get { return orderItem; }
            set { orderItem = value; }
        }

        private string webURL;

        public string WEBURL
        {
            get { return webURL; }
            set { webURL = value; }
        }


        private string ventRate;

        public string VentRate
        {
            get { return ventRate; }
            set { ventRate = value; }
        }
        private string atrialRate;

        public string AtrialRate
        {
            get { return atrialRate; }
            set { atrialRate = value; }
        }
        private string prInt;

        public string PRInt
        {
            get { return prInt; }
            set { prInt = value; }
        }
        private string qrs;

        public string QRSDur
        {
            get { return qrs; }
            set { qrs = value; }
        }
        private string qtInt;

        public string QTInt
        {
            get { return qtInt; }
            set { qtInt = value; }
        }
        private string prtAxes;

        public string PRTAxes
        {
            get { return prtAxes; }
            set { prtAxes = value; }
        }
        private string qtcInt;

        public string QTcInt
        {
            get { return qtcInt; }
            set { qtcInt = value; }
        }
        private string diagResult;

        public string DiagResult
        {
            get { return diagResult; }
            set { diagResult = value; }
        }
        private string diagECG;

        public string DiagECG
        {
            get { return diagECG; }
            set { diagECG = value; }
        }
        private string referredDoct;

        public string ReferredDoct
        {
            get { return referredDoct; }
            set { referredDoct = value; }
        }
        private string diagDoct;

        public string DiagDoct
        {
            get { return diagDoct; }
            set { diagDoct = value; }
        }

        private BaseObj device;

        public BaseObj Device
        {
            get { return device; }
            set { device = value; }
        }

        private DateTime diagDate;

        public DateTime DiagDate
        {
            get { return diagDate; }
            set { diagDate = value; }
        }
    }
}
