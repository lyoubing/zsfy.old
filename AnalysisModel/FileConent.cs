using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisModel
{
    [Flags]
    public enum SpecialAttribute
    {
        None = 0,
        Channel15 = 1,
        Channel18 = 2
    }
    public class FileConent : BaseObj
    {
        private PatientInfo patient = new PatientInfo();

        public PatientInfo Patient
        {
            get {
                if (patient == null)
                {
                    return new PatientInfo();
                }
                return patient; }
            set { patient = value; }
        }

        private OrderItem orderItem = new OrderItem();

        public OrderItem OrderItem
        {
            get
            {
                if (orderItem == null)
                {
                    return new OrderItem();
                }
                return orderItem; }
            set { orderItem = value; }
        }

        public DateTime? ApplyTime { get; set; }
        public string TJSequence { get; set; }

        private string webURL;

        public string WEBURL
        {
            get { return webURL; }
            set { webURL = value; }
        }

        private string ventRate;

        /// <summary>
        /// 心室率
        /// </summary>
        public string VentRate
        {
            get { return ventRate; }
            set { ventRate = value; }
        }
        private string atrialRate;

        /// <summary>
        /// 心房率
        /// </summary>
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

        /// <summary>
        /// PR 期间
        /// </summary>
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

        private string _PAxes;

        /// <summary>
        /// P电轴，单位degrees
        /// </summary>
        public string PAxes
        {
            get { return _PAxes; }
            set { _PAxes = value; }
        }

        private string _rAxes;

        /// <summary>
        /// R电轴，单位degrees
        /// </summary>
        public string RAxes
        {
            get { return _rAxes; }
            set { _rAxes = value; }
        }

        private string _tAxes;

        /// <summary>
        /// T电轴，单位degrees
        /// </summary>
        public string TAxes
        {
            get { return _tAxes; }
            set { _tAxes = value; }
        }

        private string _axesUnit;

        /// <summary>
        /// 电轴单位
        /// </summary>
        public string AxesUnit
        {
            get { return _axesUnit; }
            set { _axesUnit = value; }
        }



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

        private BaseObj device = new BaseObj();

        public BaseObj Device
        {
            get
            {
                if (device == null)
                {
                    return new BaseObj();
                }
                return device;
            }
            set { device = value; }
        }

        private DateTime diagDate;

        /// <summary>
        /// 诊断时间
        /// </summary>
        public DateTime DiagDate
        {
            get { return diagDate; }
            set { diagDate = value; }
        }

        private DateTime _checkDate;

        /// <summary>
        /// 检查时间
        /// </summary>
        public DateTime CheckDate
        {
            get { return _checkDate; }
            set { _checkDate = value; }
        }

        private string _pr;


        ///// <summary>
        ///// PR 期间
        ///// </summary>
        //public string PRDur
        //{
        //    get { return _pr; }
        //    set { _pr = value; }
        //}



        private string resultText;

        public string ResultText
        {
            get { return resultText; }
            set { resultText = value; }
        }

        private bool isUserDef = false;

        public bool IsUserDef
        {
            get { return isUserDef; }
            set { isUserDef = value; }
        }

        private string _SA;

        public string SAmplitude
        {
            get { return _SA; }
            set { _SA = value; }
        }

        private string _RA;

        public string RAmplitude
        {
            get { return _RA; }
            set { _RA = value; }
        }

        //是否多报告检查, 15/18导
        public bool IsMultiReport { get; set; }
        //多报告时, 报告合并的顺序
        public int ReportSequence { get; set; }

        public SpecialAttribute SpecialAttributes { get; set;}

        public bool HasChannel15Attribute
        {
            get
            {
                return SpecialAttribute.Channel15 == (this.SpecialAttributes & SpecialAttribute.Channel15);
            }
        }
        public bool HasChannel18Attribute
        {
            get
            {
                return SpecialAttribute.Channel18 == (this.SpecialAttributes & SpecialAttribute.Channel18);
            }
        }
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new FileConent Clone()
        {
            return this.MemberwiseClone() as FileConent;
        }
    }
}
