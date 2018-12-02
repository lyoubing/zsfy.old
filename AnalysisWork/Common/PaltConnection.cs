using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NetScape.AnalysisWork.Common
{
 public   class PaltConnection
    {
        public static string connStr = string.Empty;
        public static string ConnStr
        {
            get
            {
                if (!string.IsNullOrEmpty(connStr))
                {
                    connStr = GetConnSring();
                }
                return connStr;

            }
        }

        public static string GetConnSring()
        {
            string str = string.Empty;
            XDocument doc = null;
            try
            {
                doc = XDocument.Load(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\setting.xml");
                XElement root = XElement.Parse(doc.Root.Value);
                str = root.Element("SETTING").Element("PALTCONN").FirstAttribute.Value;
            }
            catch (Exception e)
            {
                return string.Empty;
            }

            return str;
        }
    }
}
