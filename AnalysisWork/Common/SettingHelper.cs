using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using log4net;
using NetScape.AnalysisModel;

namespace NetScape.AnalysisWork.Common
{
    public class SettingHelper
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static SettingObject _obj = null;
        public static SettingObject setObj {
            get
            {
                if (_obj == null)
                {
                    _obj = new SettingObject();
                    GetSetting();
                }
                return _obj;
            }
            set { _obj = value; }
        }

        private static string settingFile = "setting.xml";

        public static int GetSetting()
        {
            log.Debug("Start 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + settingFile);
            }
            catch (Exception e)
            {
                log.Debug("Load XML 文件出错！" + e.Message);
                return -1;
            }

            XmlNode node;
            try
            {
                node = doc.SelectSingleNode(@"/SETTING/HOSPITAL"); //医院信息
                setObj.HospitalCode = node.Attributes[0].Value;
                setObj.HospitalName = node.Attributes[1].Value;

                node = doc.SelectSingleNode(@"/SETTING/DIR"); //结果文件目录
                setObj.FileDIR = node.Attributes[0].Value;

                node = doc.SelectSingleNode(@"/SETTING/PLAT"); //平台的webservice地址
                setObj.PlatURL = node.Attributes[0].Value;
                setObj.Certificate = node.Attributes[1].Value;

                node = doc.SelectSingleNode(@"/SETTING/GE");//GE提供的IP Port
                setObj.GEIP = node.Attributes[0].Value;
                setObj.GEPort = node.Attributes[1].Value;

                node = doc.SelectSingleNode(@"/SETTING/SIL");//GE提供的IP Port
                setObj.Interval = node.Attributes[0].Value;

                node = doc.SelectSingleNode(@"/SETTING/FTP");//GE提供的IP Port
                setObj.FtpDIR = node.Attributes[0].Value;
                setObj.FtpUser = node.Attributes[1].Value;
                setObj.FtpPwd = node.Attributes[2].Value;

                node = doc.SelectSingleNode(@"/SETTING/CCAURL");//GE提供的IP Port
                setObj.CCAURL = node.Attributes[0].Value;

                try
                {
                    node = doc.SelectSingleNode(@"/SETTING/QUERYSECONDS");//GE提供的IP Port
                    setObj.QuerySeconds = Convert.ToInt32(node.Attributes[0].Value);
                }
                catch
                { 
                
                }
            }
            catch (Exception ex)
            {
                log.Debug("解析 XML 文件出错！" + ex.Message);
                return -1;
            }

            log.Debug("End 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            return 0;
        }

        public static DateTime GetMaxTimeSetting()
        {
            log.Debug("Start 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + settingFile);
            }
            catch (Exception e)
            {
                log.Debug("Load XML 文件出错！" + e.Message);
                return DateTime.MinValue;
            }

            DateTime dt = DateTime.MinValue;
            XmlNode node;
            try
            {
                node = doc.SelectSingleNode(@"/SETTING/MAXQueryTime");//GE提供的IP Port
                dt = NetScape.AnalysisToolKit.NConvert.ToDateTime(node.InnerText);
            }
            catch (Exception ex)
            {
                log.Debug("解析 XML 文件出错！" + ex.Message);
                return DateTime.MinValue;
            }

            log.Debug("End 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            return dt;
        }

        public static int SetMaxTimeSetting(DateTime dtMaxTime)
        {
            log.Debug("Start 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name + dtMaxTime.ToString());

            XmlDocument doc = new XmlDocument();
            string fileName = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + settingFile;

            try
            {
                doc.Load(fileName);
            }
            catch (Exception e)
            {
                log.Debug("Load XML 文件出错！" + e.Message);
                return -1;
            }

            XmlNode node;
            try
            {
                node = doc.SelectSingleNode(@"/SETTING/MAXQueryTime");//GE提供的IP Port
                node.InnerText = dtMaxTime.ToString();

                doc.Save(fileName);
            }
            catch (Exception ex)
            {
                log.Debug("解析 XML 文件出错！" + ex.Message);
                return -1;
            }

            log.Debug("End 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            return 1;
        }
    }
}
