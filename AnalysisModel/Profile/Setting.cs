using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NetScape.AnalysisModel.Profile
{
    public class ConfigSetting
    {
        #region 配置项列表

        private static ConfigObj configItem;
        private static object @lock = new object();

        /// <summary>
        /// 配置项列表
        /// </summary>
        public static ConfigObj ConfigItem
        {
            get
            {
                if (configItem == null)
                {
                    lock (@lock)
                    {
                        if (configItem == null)
                        {
                            configItem = GetSetting();
                        }
                    }
                }
                return configItem;
            }
            set { configItem = value; }
        }

        #endregion

        static string ConfigPath = AppDomain.CurrentDomain.BaseDirectory + "\\Config.xml";

        #region 读取配置文件函数

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <returns></returns>
        public static ConfigObj GetSetting()
        {
            if (configItem == null)
                configItem = new ConfigObj();

            if (!File.Exists(ConfigPath))
            {
                throw new Exception("没有找到配置文件Config.xml;路径：" + ConfigPath);
            }
            XElement root = XElement.Load(ConfigPath);

            if (root.Element("PDFTextReplacements") != null)
            {
                root.Element("PDFTextReplacements").Elements().ToList().ForEach(x =>
                {
                    var list = new List<KeyValuePair<string, string>>();
                    configItem.PDFTextReplacements.Add(x.Name.LocalName, list);

                    x.Elements("text").ToList().ForEach(text =>
                    {
                        var replace = new KeyValuePair<string, string>(text.Attribute("origin").Value, text.Attribute("dest").Value);
                        list.Add(replace);
                    });
                });
            }
            root.Element("Img").Elements("image").ToList().ForEach(x =>
            {
                ImgInfo info = new ImgInfo();
                info.ID = x.Attribute("id").Value.ToString();
                info.Name = x.Attribute("id").Value.ToString();
                info.Value = x.Value.ToString();
                info.LocationX = int.Parse(x.Attribute("px").Value);
                info.LocationY = int.Parse(x.Attribute("py").Value);
                info.Height = int.Parse(x.Attribute("height").Value);
                info.Width = int.Parse(x.Attribute("width").Value);
                info.BackColor = x.Attribute("backcolor").Value;
                configItem.ImgInfoSet.Add(info);
            });

            root.Element("Text").Elements("Txt").ToList().ForEach(x =>
            {
                TxtInfo inf = new TxtInfo();
                inf.ID = x.Attribute("id").Value.ToString();
                inf.Name = x.Attribute("id").Value.ToString();
                inf.Value = x.Value.ToString();
                inf.LocationX = int.Parse(x.Attribute("px").Value);
                inf.LocationY = int.Parse(x.Attribute("py").Value);
                inf.Size = int.Parse(x.Attribute("size").Value);
                inf.RowHeight = int.Parse(x.Attribute("rowheight").Value);
                inf.Bold = bool.Parse(x.Attribute("bold").Value);
                configItem.TxtInfoSet.Add(inf);
            });

            XElement hos = root.Element("Domain").Element("HOS_DOMAIN_INFO");
            if (hos != null)
            {
                configItem.HosDomain.ID = hos.Attribute("id").Value.Trim();
                configItem.HosDomain.Name = hos.Attribute("name").Value.Trim();
                configItem.HosDomain.DomainType = hos.Attribute("type").Value.Trim();
                configItem.HosDomain.Value = hos.Value.Trim();
            }
            XElement ident = root.Element("Domain").Element("IDENTIFIER_DOMAIN_INFO");
            if (ident != null)
            {
                configItem.IdenDomain.ID = ident.Attribute("id").Value.Trim();
                configItem.IdenDomain.Name = ident.Attribute("name").Value.Trim();
                configItem.IdenDomain.DomainType = ident.Attribute("type").Value.Trim();
                configItem.IdenDomain.Value = ident.Value.Trim();
            }
            XElement flow = root.Element("Domain").Element("IDENTIFIER_FLOW_DOMAIN_INFO");
            if (flow != null)
            {
                configItem.FlowDomain.ID = flow.Attribute("id").Value.Trim();
                configItem.FlowDomain.Name = flow.Attribute("name").Value.Trim();
                configItem.FlowDomain.DomainType = flow.Attribute("type").Value.Trim();
                configItem.FlowDomain.Value = flow.Value.Trim();
            }

            if (_urlList == null)
                _urlList = new List<PrintUrl>();
            List<XElement> _prints = root.Element("PrintUrl").Elements("Url").ToList();
            if (_prints != null && _prints.Count > 0)
            {
                foreach (var item in _prints)
                {
                    PrintUrl url = new PrintUrl();
                    url.ID = item.Attribute("id").Value.Trim();
                    url.MsgUrl = item.Attribute("Hl7Url").Value.Trim();
                    url.PdfOutUrl = item.Attribute("PdfUrl").Value.Trim();
                    url.Name = item.Value.Trim();
                    url.TempUrl = item.Attribute("TempUrl").Value.Trim();
                    _urlList.Add(url);
                }
            }

            XElement ect = root.Element("EctPath");
            if (ect != null)
            {
                _pdf = new PdfPath();
                _pdf.ID = ect.Attribute("id").Value.Trim();
                _pdf.FtpServer = ect.Attribute("ftp").Value.Trim();
                _pdf.Account = ect.Attribute("account").Value.Trim();
                _pdf.Pasword = ect.Attribute("pwd").Value.Trim();
                _pdf.Name = ect.Value.Trim();
                _pdf.Mark = ect.Attribute("backup").Value.Trim();
                _pdf.Value = ect.Value.Trim();
            }
            XElement paltConn = root.Element("PaltConnStr");
            if (paltConn != null)
            {
                PaltConnStr = paltConn.Value;
            }

            if (root.Element("IsPrintBarcode") != null)
            {
                IsPrintBarCode = root.Element("IsPrintBarcode").Value.Trim();
            }

            if (root.Element("QueryDays") != null)
            {
                QueryDays = root.Element("QueryDays").Value.Trim();
            }

            if (root.Element("ExecDept") != null)
            {
                EcgExecDept = root.Element("ExecDept").Value.Trim();
            }

            if (root.Element("MuseConn") != null)
            {
                MuseConnStr = root.Element("MuseConn").Value.Trim();
            }

            return configItem;
        }

        #endregion

        #region muse 打印目录列表

        private static List<PrintUrl> _urlList;

        /// <summary>
        /// muse 打印目录列表
        /// </summary>
        public static List<PrintUrl> PrintUrlList
        {
            get
            {
                if (_urlList == null)
                    GetSetting();
                return _urlList;
            }
            set { _urlList = value; }
        }

        #endregion

        #region 电生理报告路径

        private static PdfPath _pdf;

        /// <summary>
        /// 电生理报告路径
        /// </summary>
        public static PdfPath EctPdf
        {
            get
            {
                if (_pdf == null)
                    GetSetting();
                return _pdf;
            }
            set { _pdf = value; }
        }

        #endregion

        #region 平台数据连接字符串

        private static string _paltConnStr;

        /// <summary>
        /// 平台数据连接字符串
        /// </summary>
        public static string PaltConnStr
        {
            get
            {
                if (string.IsNullOrEmpty(_paltConnStr))
                    GetSetting();
                return _paltConnStr;
            }
            set { _paltConnStr = value; }
        }

        #endregion

        #region 客户端是否打印条码 1 打印，0 不打印

        private static string _Prt_bar;

        /// <summary>
        /// 客户端是否打印条码 1 打印，0 不打印
        /// </summary>
        public static string IsPrintBarCode
        {
            get
            {
                if (string.IsNullOrEmpty(_Prt_bar))
                    GetSetting();
                return _Prt_bar;
            }
            set { _Prt_bar = value; }
        }

        #endregion

        #region 心电执行科室

        private static string _ecgExecDept;

        /// <summary>
        /// 心电执行科室
        /// </summary>
        public static string EcgExecDept
        {
            get { return _ecgExecDept; }
            set { _ecgExecDept = value; }
        }

        #endregion

        #region 登记查询时长/天数

        private static string _queryDays = "60";

        /// <summary>
        /// 查询时长
        /// </summary>
        public static string QueryDays
        {
            get { return _queryDays; }
            set { _queryDays = value; }
        }

        #endregion

        #region Muse 连接字符串

        private static string _museStr;

        /// <summary>
        /// Muse 连接字符串
        /// </summary>
        public static string MuseConnStr
        {
            get
            {
                if (string.IsNullOrEmpty(_museStr))
                    GetSetting();
                return _museStr;
            }
            set { _museStr = value; }
        }



        #endregion

        #region 平台 各种域

        private static List<NetScape.AnalysisModel.Profile.DomainInfo> _domains;

        public static List<NetScape.AnalysisModel.Profile.DomainInfo> DomainInfo
        {
            get
            {
                if (_domains == null || _domains.Count == 0)
                    _domains = GetDomainConst();
                return _domains;
            }
            set { _domains = value; }
        }

        static List<NetScape.AnalysisModel.Profile.DomainInfo> GetDomainConst()
        {
            List<NetScape.AnalysisModel.Profile.DomainInfo> list = new List<AnalysisModel.Profile.DomainInfo>();
            list.Add(new AnalysisModel.Profile.DomainInfo()
            {
                ID = "2.16.840.1.113883.4.487.2.1",
                Name = "中山大学附属第一医院-岱嘉医疗信息交互平台",
                DomainType = "GZZSYKDXFSDYYY",
                Code = "1"
            });
            list.Add(new AnalysisModel.Profile.DomainInfo()
            {
                ID = "2.16.840.1.113883.4.487.2.1.4",
                Name = "医院信息系统-住院号",
                DomainType = "HIS",
                Code = "2"
            });

            list.Add(new AnalysisModel.Profile.DomainInfo()
            {
                ID = "2.16.840.1.113883.4.487.2.1.4.1",
                Name = "医院信息系统-门诊号",
                DomainType = "HIS-MZ",
                Code = "3"
            });

            list.Add(new AnalysisModel.Profile.DomainInfo()
            {
                ID = "2.16.840.1.113883.4.487.2.1.4.2",
                Name = "医院信息系统-门诊流水号",
                DomainType = "HIS-MZLS",
                Code = "4"
            });

            list.Add(new AnalysisModel.Profile.DomainInfo()
            {
                ID = "2.16.840.1.113883.4.487.2.1.4.4",
                Name = "医院信息系统-住院流水号",
                DomainType = "HIS-ZYLS",
                Code = "5"
            });

            list.Add(new AnalysisModel.Profile.DomainInfo()
            {
                ID = "2.16.840.1.113883.4.487.2.1.4.5",
                Name = "医院信息系统-体检号",
                Value = "HIS-TJ",
                Code = "6"
            });
            list.Add(new AnalysisModel.Profile.DomainInfo()
            {
                ID = "2.16.840.1.113883.4.487.2.1.4.6",
                Name = "医院信息系统-体检流水号",
                DomainType = "HIS-TJLS",
                Code = "7"
            });

            list.Add(new AnalysisModel.Profile.DomainInfo()
            {
                ID = "2.16.840.1.113883.4.487.2.1.40",
                Name = "体检新系统（广州医博）",
                DomainType = "YBTJXT",
                Code = "8"
            });
            list.Add(new AnalysisModel.Profile.DomainInfo()
            {
                ID = "2.16.840.1.113883.4.487.2.1.40.2",
                Name = "体检新系统（广州医博）-体检流水号",
                DomainType = "YBTJXT-TJLS",
                Code = "9"
            });


            list.Add(new AnalysisModel.Profile.DomainInfo()
            {
                ID = "2.16.840.1.113883.4.487.2.1.45",
                Name = "GEECG",
                DomainType = "ISO",
                Code = "10"
            });

            list.Add(new AnalysisModel.Profile.DomainInfo()
            {
                ID = "2.16.840.1.113883.4.487.2.1.45.2",
                Name = "GEECG-XDLS",
                DomainType = "ISO",
                Code = "11"
            });

            return list;
        }

        #endregion

    }

    public class ConfigObj
    {
        public ConfigObj()
        {
            ImgInfoSet = new List<ImgInfo>();
            TxtInfoSet = new List<TxtInfo>();
            HosDomain = new DomainInfo();
            IdenDomain = new DomainInfo();
            FlowDomain = new DomainInfo();
            PDFTextReplacements = new Dictionary<string, List<KeyValuePair<string, string>>>();
            //  ConfigPath = AppDomain.CurrentDomain.BaseDirectory + "\\Config.xml";

        }

        public Dictionary<string, List<KeyValuePair<string, string>>> PDFTextReplacements { private set; get; }

        private List<ImgInfo> _ImgInfoSet;

        /// <summary>
        /// 图片配置
        /// </summary>
        public List<ImgInfo> ImgInfoSet
        {
            get { return _ImgInfoSet; }
            set { _ImgInfoSet = value; }
        }



        private List<TxtInfo> _TextInfoSet;
        /// <summary>
        /// 文本配置
        /// </summary>  
        public List<TxtInfo> TxtInfoSet
        {
            get { return _TextInfoSet; }
            set { _TextInfoSet = value; }
        }

        private DomainInfo _hosInfo;
        /// <summary>
        /// 医院域信息
        /// </summary>
        public DomainInfo HosDomain
        {
            get { return _hosInfo; }
            set { _hosInfo = value; }
        }

        private DomainInfo _IdenDomain;
        /// <summary>
        /// 机构域信息
        /// </summary>
        public DomainInfo IdenDomain
        {
            get { return _IdenDomain; }
            set { _IdenDomain = value; }
        }

        private DomainInfo _flowDomain;

        /// <summary>
        /// 流水机构（在这里指的是心电系统）
        /// </summary>
        public DomainInfo FlowDomain
        {
            get { return _flowDomain; }
            set { _flowDomain = value; }
        }

    }


    public class ImgInfo : NetScape.AnalysisModel.BaseObj
    {



        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string BackColor { get; set; }
    }

    public class TxtInfo : NetScape.AnalysisModel.BaseObj
    {



        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public int Size { get; set; }
        public int RowHeight { get; set; }
        public bool Bold { get; set; }
    }

    public class PathInfo : NetScape.AnalysisModel.BaseObj
    {
        public string FullPath { get; set; }
    }

    public class PdfPath : NetScape.AnalysisModel.BaseObj
    {
        private string _server;

        public string FtpServer
        {
            get { return _server; }
            set { _server = value; }
        }

        private string _account;

        public string Account
        {
            get { return _account; }
            set { _account = value; }
        }

        private string _pwd;

        public string Pasword
        {
            get { return _pwd; }
            set { _pwd = value; }
        }


    }

    public class DomainInfo : NetScape.AnalysisModel.BaseObj
    {
        private string _type;

        public string DomainType
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _flowId;

        public string FlowId
        {
            get { return _flowId; }
            set { _flowId = value; }
        }

        private string _code;
        /// <summary>
        /// 机构域编码
        /// </summary>
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

    }

    public class PrintUrl : NetScape.AnalysisModel.BaseObj
    {
        private string _msgUrl;

        /// <summary>
        /// 监控Hl7打印消息目录
        /// </summary>
        public string MsgUrl
        {
            get { return _msgUrl; }
            set { _msgUrl = value; }
        }

        private string _pdfOutUrl;

        /// <summary>
        /// pdf报告输出路径
        /// </summary>
        public string PdfOutUrl
        {
            get { return _pdfOutUrl; }
            set { _pdfOutUrl = value; }
        }

        private string _temp;


        /// <summary>
        /// 临时存放pdf下载路径，处理完后删除
        /// </summary>
        public string TempUrl
        {
            get { return _temp; }
            set { _temp = value; }
        }


    }
}
