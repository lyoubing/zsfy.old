using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisModel
{
    /// <summary>
    /// XML申请单信息
    /// </summary>
    public class EcgMsgFileInfo : BaseObj
    {

        private string patientid;
        /// <summary>
        /// 患者索引号
        /// </summary>
        public string PatientId
        {
            get
            {
                return patientid;
            }
            set
            {
                patientid = value;
            }
        }


        private string cardno;
        /// <summary>
        /// 患者卡号 
        /// </summary>
        public string CardNo
        {
            get
            {
                return cardno;
            }
            set
            {
                cardno = value;
            }
        }


        private string applyno;
        /// <summary>
        /// 申请单号
        /// </summary>
        public string ApplyNo
        {
            get
            {
                return applyno;
            }
            set
            {
                applyno = value;
            }
        }


        private string localurl;
        /// <summary>
        /// xml实体本地保存路径
        /// </summary>
        public string LocalUrl
        {
            get
            {
                return localurl;
            }
            set
            {
                localurl = value;
            }
        }


        private string xmlfilename;
        /// <summary>
        /// xml 实体名称
        /// </summary>
        public string XmlFileName
        {
            get
            {
                return xmlfilename;
            }
            set
            {
                xmlfilename = value;
            }
        }


        private string xmlmsg;
        /// <summary>
        /// 申请单 实体 XML
        /// </summary>
        public string XmlMsg
        {
            get
            {
                return xmlmsg;
            }
            set
            {
                xmlmsg = value;
            }
        }


        private string pdffilename;
        /// <summary>
        /// 
        /// </summary>
        public string PdfFileName
        {
            get
            {
                return pdffilename;
            }
            set
            {
                pdffilename = value;
            }
        }


        private string pdfurl;
        /// <summary>
        /// 
        /// </summary>
        public string PdfUrl
        {
            get
            {
                return pdfurl;
            }
            set
            {
                pdfurl = value;
            }
        }


        private string _xmlUrl;

        /// <summary>
        /// Xml报告的存储路径
        /// </summary>
        public string XmlReportUrl
        {
            get { return _xmlUrl; }
            set { _xmlUrl = value; }
        }

        private string _xmlLocalPath;

        /// <summary>
        /// Xml报告本地存储路径
        /// </summary>
        public string XmlReportLocalUrl
        {
            get { return _xmlLocalPath; }
            set { _xmlLocalPath = value; }
        }
        
        


        private string imgurl;
        /// <summary>
        /// 
        /// </summary>
        public string ImgUrl
        {
            get
            {
                return imgurl;
            }
            set
            {
                imgurl = value;
            }
        }


        private string imgfilename;
        /// <summary>
        /// 
        /// </summary>
        public string ImgFileName
        {
            get
            {
                return imgfilename;
            }
            set
            {
                imgfilename = value;
            }
        }


        private string image;
        /// <summary>
        /// 
        /// </summary>
        public string Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }


        private string opercode;
        /// <summary>
        /// 
        /// </summary>
        public string OperCode
        {
            get
            {
                return opercode;
            }
            set
            {
                opercode = value;
            }
        }


        private DateTime operdate;
        /// <summary>
        /// 
        /// </summary>
        public DateTime OperDate
        {
            get
            {
                return operdate;
            }
            set
            {
                operdate = value;
            }
        }


      

        private string _pkey;

        /// <summary>
        /// 文档主表的序列号
        /// </summary>
        public string P_Key
        {
            get {
                if (string.IsNullOrEmpty(_pkey))
                    _pkey = "ECGP" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                return _pkey; }
            set { _pkey = value; }
        }

        private string _key;
        /// <summary>
        /// 文档索引号
        /// </summary>
        public string UniqueKey
        {
            get {
                if (string.IsNullOrEmpty(_key))
                    _key = "ECGR" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                return _key; }
            set { _key = value; }
        }

        private string _fileType;

        public string FileType
        {
            get {
                if (string.IsNullOrEmpty(_fileType))
                    _fileType = "PATH-PDF";
                return _fileType; }
            set { _fileType = value; }
        }

        private string _loads;

        /// <summary>
        /// 荷载类型
        /// </summary>
        public string Loads
        {
            get {
                if (string.IsNullOrEmpty(_loads))
                    _loads = "XDS.GEECGBG";
                return _loads; }
            set { _loads = value; }
        }

        private string _reportType;

        public string ReportType
        {
            get 
            {
                if (string.IsNullOrEmpty(_reportType))
                {
                    _reportType = "PDF";
                }
                return _reportType; }
            set { _reportType = value; }
        }
        
        
    }

}
