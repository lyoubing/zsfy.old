﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

namespace Analysis.Service
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public void HelloWorld()
        {
            MessageService ser = new MessageService();
            Request req = new Request();
            req.requestBody = @"<List><OutPatientInfo><CARD_NO>0022359376</CARD_NO><DEPT_NAME>儿科急诊</DEPT_NAME><SEENO>102</SEENO><REGLEVL_CODE>21</REGLEVL_CODE><YNFR>0</YNFR><ORDER_NO>14067</ORDER_NO><PACT_NAME>现金</PACT_NAME><SEE_DOCD>002K52</SEE_DOCD><SEE_DPCD>2069</SEE_DPCD><SEX_CODE>M</SEX_CODE><APPEND_FLAG>0</APPEND_FLAG><END_TIME>20180102235900</END_TIME><PAYKIND_CODE>01</PAYKIND_CODE><RELA_PHONE>13922263867</RELA_PHONE><BIRTHDAY>20130709000000</BIRTHDAY><VALID_FLAG>1</VALID_FLAG><DEPT_CODE>2069</DEPT_CODE><SCHEMA_NO>2906348</SCHEMA_NO><REG_DATE>20180102120000</REG_DATE><PACT_CODE>1</PACT_CODE><RECIPE_NO>97275614</RECIPE_NO><OPER_CODE>06273F</OPER_CODE><OPER_DATE>20180102190301</OPER_DATE><NAME>王梓诺</NAME><YNREGCHRG>0</YNREGCHRG><CANCEL_DATE>00010101000000</CANCEL_DATE><BEGIN_TIME>20180102120000</BEGIN_TIME><REGLEVL_NAME>急诊</REGLEVL_NAME><YNBOOK>0</YNBOOK><CLINIC_CODE>40448714</CLINIC_CODE><NOON_CODE>2</NOON_CODE></OutPatientInfo><OutPatientInfo><CARD_NO>0022217300</CARD_NO><DEPT_NAME>儿科急诊</DEPT_NAME><SEENO>103</SEENO><REGLEVL_CODE>21</REGLEVL_CODE><YNFR>0</YNFR><ORDER_NO>14068</ORDER_NO><PACT_NAME>越秀区统筹(20%)</PACT_NAME><SEE_DOCD>002K52</SEE_DOCD><SEE_DPCD>2069</SEE_DPCD><SEX_CODE>M</SEX_CODE><APPEND_FLAG>0</APPEND_FLAG><END_TIME>20180102235900</END_TIME><PAYKIND_CODE>03</PAYKIND_CODE><RELA_PHONE>18620501067</RELA_PHONE><BIRTHDAY>20120521000000</BIRTHDAY><VALID_FLAG>1</VALID_FLAG><DEPT_CODE>2069</DEPT_CODE><SCHEMA_NO>2906348</SCHEMA_NO><REG_DATE>20180102120000</REG_DATE><PACT_CODE>1220</PACT_CODE><MCARD_NO>12027690</MCARD_NO><RECIPE_NO>97275615</RECIPE_NO><OPER_CODE>06273F</OPER_CODE><OPER_DATE>20180102190404</OPER_DATE><NAME>莫子骞</NAME><YNREGCHRG>0</YNREGCHRG><CANCEL_DATE>00010101000000</CANCEL_DATE><BEGIN_TIME>20180102120000</BEGIN_TIME><REGLEVL_NAME>急诊</REGLEVL_NAME><YNBOOK>0</YNBOOK><CLINIC_CODE>40448715</CLINIC_CODE><NOON_CODE>2</NOON_CODE></OutPatientInfo></List>";
            ser.messageNotice(req);
        }
    }
}
