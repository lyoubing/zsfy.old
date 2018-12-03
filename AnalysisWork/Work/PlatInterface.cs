using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using log4net;
using NetScape.AnalysisModel;
using NetScape.AnalysisToolKit;
using System.Collections;
using System.Xml.Linq;

namespace NetScape.AnalysisWork.Work
{
    public class PlatInterface
    {
        public string errMsg = string.Empty;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region  海南项目

        public void QueryPatientCheckInfoHN(string url, string checkCode, string type, ref Order order, out int result)
        {
            string xml = @"<QBP JHIPMsgVersion=""1.0.1""><MSH><HOSP_AREA>43563145-0</HOSP_AREA><FROM_SYS>PACS</FROM_SYS><TO_SYS>{9}</TO_SYS><CREATE_DATE>{8}</CREATE_DATE></MSH><QPD><PAT_INPATIENTNO>{0}</PAT_INPATIENTNO><STU_INHOSPITALNO>{1}</STU_INHOSPITALNO><CASE_NO>{2}</CASE_NO><PAT_CLINICNO>{3}</PAT_CLINICNO><STU_DEPART>{4}</STU_DEPART><CHECKID>{5}</CHECKID><DOCTOR>{6}</DOCTOR><DOCTOR_ID>{7}</DOCTOR_ID></QPD></QBP>";

            DateTime dtNow = DateTime.Now;

            string toSys = "HIS";

            if (type == "1")
            {
                toSys = "TJZX";
            }

            string requestXml = string.Format(xml, "", "", "", "", "心电图MUSE", checkCode, "", "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), toSys);

            log.Info("调用webservice[getApply]传递的参数为：@" + requestXml);

            try
            {
                string[] args = { requestXml };

                object obj = Common.WebServiceHelper.InvokeWebService(url, "getApply", args, ref errMsg);

                if (obj == null)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[getApply]报错@" + errMsg);
                    result = -1;
                    return;
                }

                log.Info("调用webservice[getApply]返回的参数为：@" + obj.ToString());

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(obj.ToString());
                string resultCode = doc.SelectSingleNode("ORM/MSA/RESULT_CODE").InnerText.Trim();

                if (resultCode == "CA") //失败
                {
                    errMsg = doc.SelectSingleNode("ORM/MSA/RESULT_CONTENT").InnerText.Trim();
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[getApply]返回报错@" + errMsg);
                    result = -1;
                    return;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[getApply]返回成功");

                order = new Order();

                //患者信息
                order.Patient.ID = doc.SelectSingleNode("ORM/PID/PAT_INPATIENTNO").InnerText.Trim();
                order.Patient.PID.PatientNO = doc.SelectSingleNode("ORM/PID/STU_INHOSPITALNO").InnerText.Trim();
                order.Patient.PID.CaseNO = doc.SelectSingleNode("ORM/PID/CASE_NO").InnerText.Trim();
                order.Patient.PID.CardNO = doc.SelectSingleNode("ORM/PID/PAT_CLINICNO").InnerText.Trim();
                order.Patient.Name = doc.SelectSingleNode("ORM/PID/PAT_NAME").InnerText.Trim();
                order.Patient.Memo = doc.SelectSingleNode("ORM/PID/PAT_ENGNAME").InnerText.Trim();
                order.Patient.Sex = doc.SelectSingleNode("ORM/PID/PAT_SEX").InnerText.Trim();
                order.Patient.Birthday = NConvert.ToDateTime(doc.SelectSingleNode("ORM/PID/PAT_BIRTHDATE").InnerText.Trim());
                order.Patient.DIST = doc.SelectSingleNode("ORM/PID/PAT_PLACEOFBIRTH").InnerText.Trim();
                order.Patient.Address = doc.SelectSingleNode("ORM/PID/PAT_ADDRESS").InnerText.Trim();
                order.Patient.Nation = doc.SelectSingleNode("ORM/PID/PAT_RACE").InnerText.Trim();
                order.Patient.Profession = doc.SelectSingleNode("ORM/PID/PAT_PROFESSION").InnerText.Trim();
                order.Patient.MaritalStatus = doc.SelectSingleNode("ORM/PID/PAT_MARITALSTATUS").InnerText.Trim();
                order.Patient.CardType = doc.SelectSingleNode("ORM/PID/PAT_CARDTYPE").InnerText.Trim();
                order.Patient.IDCard = doc.SelectSingleNode("ORM/PID/PAT_IDCARDNO").InnerText.Trim();
                order.Patient.PhoneNumber = doc.SelectSingleNode("ORM/PID/PAT_TELEPHONE").InnerText.Trim();
                order.Patient.Degree = doc.SelectSingleNode("ORM/PID/PAT_DEGREE").InnerText.Trim();
                order.Patient.Pact = doc.SelectSingleNode("ORM/PID/PAT_COSTTYPE").InnerText.Trim();
                order.Patient.HomeZIP = doc.SelectSingleNode("ORM/PID/PAT_ZIPCODE").InnerText.Trim();


                //医嘱信息
                order.Patient.PatientType = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/ADM_TYPE").InnerText.Trim();
                order.Status = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STATUS").InnerText.Trim();
                order.ExecDept.Name = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STU_DEPART").InnerText.Trim();
                order.CheckNO = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STU_HISEXAMNO").InnerText.Trim();
                order.Patient.BedNO = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STU_BEDNO").InnerText.Trim();
                order.Patient.Age = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STU_AGE").InnerText.Trim();
                order.ExecDept.ID = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STU_LODGEHOSPITAL").InnerText.Trim();
                order.ReciptDept.Name = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STU_LODGESECTION").InnerText.Trim();
                order.ReciptDoctor.Name = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STU_LODGEDOCTOR").InnerText.Trim();
                string appDate = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STU_LODGEDATE").InnerText.Trim();
                string appTime = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STU_LODGETIME").InnerText.Trim();
                order.ApplyDate = NConvert.ToDateTime(appDate + " " + appTime);
                order.Patient.InSource = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STU_CLIISINPAT").InnerText.Trim();
                order.Patient.MainDiagnose = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STU_DIAGNOSIS").InnerText.Trim();
                order.HealthHistory.ID = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STU_SURGERYRESULT").InnerText.Trim();
                order.HealthHistory.Name = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STU_OTHERTESTRESULT").InnerText.Trim();
                order.HealthHistory.Memo = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/STU_CHECKPURPOSE").InnerText.Trim();

                ArrayList alOrderItems = new ArrayList();

                OrderItem item = new OrderItem();

                //医嘱明细信息
                item.ID = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/DETAIL/CHECKID").InnerText.Trim();
                item.ApplyNo = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/DETAIL/APPLICATION_NO").InnerText.Trim();
                item.OrderID = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/DETAIL/ORDER_NO").InnerText.Trim();
                item.SequenceNO = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/DETAIL/SEQ_NO").InnerText.Trim();
                item.Item.ID = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/DETAIL/ORDER_CODE").InnerText.Trim();
                item.Item.Name = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/DETAIL/ORDER_DESC").InnerText.Trim();
                item.CombNO = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/DETAIL/ORDERSET_GROUP_NO").InnerText.Trim();
                item.CheckPart.ID = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/DETAIL/STU_CLASSNAME").InnerText.Trim();
                item.CheckPart.Name = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/DETAIL/STU_PARTOFCHECK").InnerText.Trim();
                item.CheckPart.Memo = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/DETAIL/SER_DIRECTION").InnerText.Trim();
                item.IsEnhance = NConvert.ToBoolean(doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/DETAIL/SER_FLATORPOWER").InnerText.Trim());
                item.TotCost = NConvert.ToDecimal(doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/DETAIL/SER_FEE").InnerText.Trim());
                item.Sample = doc.SelectSingleNode("ORM/CHECKINFO/HISEXAMNO/DETAIL/STU_SPECIMEN").InnerText.Trim();

                alOrderItems.Add(item);

                order.OrderItems = alOrderItems;

                result = 1;
            }
            catch (Exception e)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[getApply]报错@" + e.Message);
                result = -1;
            }

        }
        string _logName = Funcs.Function._logName;
        public int ReportCheckHN(string url, Order order, string operType)
        {
            string xml = @"<SIU JHIPMsgVersion=""1.0.1""><MSH><HOSP_AREA>43563145-0</HOSP_AREA><FROM_SYS>PACS</FROM_SYS><TO_SYS>{10}</TO_SYS><CREATE_DATE>{9}</CREATE_DATE></MSH><ORCS><ORC><STATUS>{0}</STATUS><APPLICATION_NO>{1}</APPLICATION_NO><ORDER_DEPT_CODE>{2}</ORDER_DEPT_CODE><ORDER_DR_CODE>{3}</ORDER_DR_CODE><ORDER_DATE>{4}</ORDER_DATE><URGENT_FLG>{5}</URGENT_FLG><SEND_USER>{6}</SEND_USER><SEND_USER_NAME>{7}</SEND_USER_NAME><OBRS>{8}</OBRS></ORC></ORCS></SIU>";

            string innerXml = @"<OBR><APPLICATION_NO>{0}</APPLICATION_NO><ORDER_NO>{1}</ORDER_NO><SEQ_NO>{2}</SEQ_NO><SETMAIN_FLG>{3}</SETMAIN_FLG><ORDER_CODE>{4}</ORDER_CODE><ORDER_DESC>{5}</ORDER_DESC><OPT_ENG_DESC>{6}</OPT_ENG_DESC><OPTITEM_CODE>{7}</OPTITEM_CODE><OPTITEM_CHN_DESC>{8}</OPTITEM_CHN_DESC><DEV_CODE>{9}</DEV_CODE><RPTTYPE_CODE>{10}</RPTTYPE_CODE><AR_AMT>{11}</AR_AMT><OWN_AMT>{12}</OWN_AMT><ORDERSET_GROUP_NO>{13}</ORDERSET_GROUP_NO><ORDER_REMARK>{14}</ORDER_REMARK></OBR>";

            StringBuilder sb = new StringBuilder();

            string application_no = string.Empty;

            string type = "HIS";

            if (order.Patient.PatientType == "T")
            {
                type = "TJZX";
            }

            foreach (OrderItem item in order.OrderItems)
            {
                application_no = item.ApplyNo;

                sb.Append(string.Format(innerXml,
                    item.ApplyNo,
                    item.OrderID,
                    item.SequenceNO,
                    'N',
                    item.Item.ID,
                    item.Item.Name,
                    "",
                    "",
                    item.CheckPart.Name,
                    "",
                    "",
                    item.TotCost.ToString(),
                    item.TotCost.ToString(),
                    item.CombNO,
                    ""));
            }

            string requestXml = string.Format(xml, operType, application_no, order.ReciptDept.Name, order.ReciptDoctor.Name, order.ApplyDate.ToString(), 'N', "", "", sb.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), type);

            log.Info("调用webservice[CheckReport]传递的参数为:@" + requestXml);

            string[] args = { requestXml };

            try
            {
                object obj = Common.WebServiceHelper.InvokeWebService(url, "CheckReport", args, ref errMsg);

                if (obj == null)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[CheckReport]报错@" + errMsg);
                    return -1;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[CheckReport]返回参数为:@" + obj.ToString());

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(obj.ToString());

                string resultCode = doc.SelectSingleNode("ACK/MSA/RESULT_CODE").InnerText.Trim();

                if (resultCode == "CA") //失败
                {
                    errMsg = doc.SelectSingleNode("ACK/MSA/RESULT_CONTENT").InnerText.Trim();
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[CheckReport]返回报错@" + errMsg);
                    return -1;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[CheckReport]返回成功");

                return 1;

            }
            catch (Exception e)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[CheckReport]报错@" + e.Message);
                return -1;
            }
        }

        public int WriteBackReportHN(string url, FileConent fileConent)
        {
            string xml = @"<ORU JHIPMsgVersion=""1.0.1""><MSH><HOSP_AREA>43563145-0</HOSP_AREA><FROM_SYS>PACS</FROM_SYS><TO_SYS>{73}</TO_SYS><CREATE_DATE>{72}</CREATE_DATE></MSH><PID><ID_NO>{0}</ID_NO><CASE_NO>{1}</CASE_NO><MR_NO>{2}</MR_NO><PAT_NAME>{3}</PAT_NAME><SEX_CODE>{4}</SEX_CODE><BIRTH_DATE>{5}</BIRTH_DATE><ID_CODE>{6}</ID_CODE><ADDRESS>{7}</ADDRESS><POST_CODE>{8}</POST_CODE><TEL_NO>{9}</TEL_NO><COMPANY_NAME>{10}</COMPANY_NAME><MED_CARD>{11}</MED_CARD><IN_COUNT>{12}</IN_COUNT><SPECIES_CODE>{13}</SPECIES_CODE><SPECIES_NAME>{14}</SPECIES_NAME></PID><PV1><ADM_TYPE>{15}</ADM_TYPE><CLINIC_CODE>{16}</CLINIC_CODE><DEPT_CODE>{17}</DEPT_CODE><CLINICROOM_CODE>{18}</CLINICROOM_CODE><STATION_CODE>{19}</STATION_CODE><BED_NO>{20}</BED_NO><BED_NAME>{21}</BED_NAME><SUBJ_TEXT>{22}</SUBJ_TEXT><OBJ_TEXT>{23}</OBJ_TEXT><PHYSEXAM_REC>{24}</PHYSEXAM_REC><ISHAS_METAL>{25}</ISHAS_METAL><METAL_ING>{26}</METAL_ING><NS_PHYSEXAM_REC>{27}</NS_PHYSEXAM_REC></PV1><ORCS><ORC><STATUS>{28}</STATUS><APPLICATION_NO>{29}</APPLICATION_NO><ORDER_DEPT_CODE>{30}</ORDER_DEPT_CODE><ORDER_DR_CODE>{31}</ORDER_DR_CODE><ORDER_DATE>{32}</ORDER_DATE><URGENT_FLG>{33}</URGENT_FLG><SEND_USER>{34}</SEND_USER><SEND_USER_NAME>{35}</SEND_USER_NAME><OBRS><OBR><APPLICATION_NO>{36}</APPLICATION_NO><ORDER_NO>{37}</ORDER_NO><SEQ_NO>{38}</SEQ_NO><SETMAIN_FLG>{39}</SETMAIN_FLG><ORDER_CODE> {40}</ORDER_CODE><ORDER_DESC>{41}</ORDER_DESC><OPT_ENG_DESC>{42}</OPT_ENG_DESC><OPTITEM_CODE>{43}</OPTITEM_CODE><OPTITEM_CHN_DESC>{44}</OPTITEM_CHN_DESC><DEV_CODE>{45}</DEV_CODE><RPTTYPE_CODE>{46}</RPTTYPE_CODE><AR_AMT>{47}</AR_AMT><OWN_AMT>{48}</OWN_AMT><ORDERSET_GROUP_NO>{49}</ORDERSET_GROUP_NO><ORDER_REMARK>{50}</ORDER_REMARK><ReportPath>{51}</ReportPath><ImageFiles>{52}</ImageFiles><OBXS>{53}</OBXS><EMR><EXAM_CLASS>{54}</EXAM_CLASS><EXAM_SUB_CLASS>{55}</EXAM_SUB_CLASS><PERFORMED_BY>{56}</PERFORMED_BY><AUDIT_PHYSICIAN>{57}</AUDIT_PHYSICIAN><RESULT_STATUS>{58}</RESULT_STATUS><REPORT_DATE_TIME>{59}</REPORT_DATE_TIME><CREATED_DATE>{60}</CREATED_DATE><NOTE>{61}</NOTE><URL_TEXT>{62}</URL_TEXT><EXAM_EMIT_ID>{63}</EXAM_EMIT_ID><EXAM_TYPE>{64}</EXAM_TYPE><EXAM_METHOD>{65}</EXAM_METHOD><PERFORMED_BY1>{66}</PERFORMED_BY1><PERFORMED_BY2>{67}</PERFORMED_BY2><PERFORMED_BY3>{68}</PERFORMED_BY3><LOCAL_EXAM_PK>{69}</LOCAL_EXAM_PK><EXAM_ITEM>{70}</EXAM_ITEM><EXAM_ITEM_CODE>{71}</EXAM_ITEM_CODE></EMR></OBR></OBRS></ORC></ORCS></ORU>";

            string fileXml = @"<ImageFile>{0}</ImageFile>";

            string innerXml = @"<OBX><ITESTSET_SEQ>{0}</ITESTSET_SEQ><ITEST_TYPE>{1}</ITEST_TYPE><EXM_NO>{2}</EXM_NO><ITEST_VALUE>{3}</ITEST_VALUE><OUTCOME_TYPE>{4}</OUTCOME_TYPE><PAS_METHOD>{5}</PAS_METHOD><RPT_PATH>{6}</RPT_PATH><DEV_CODE>{7}</DEV_CODE><DEV_DESC>{8}</DEV_DESC><OPT_USER>{9}</OPT_USER><OPT_DATE>{10}</OPT_DATE></OBX>";

            string Rtype = "CE";
            string resultValue = string.Empty;
            string tmpXml = string.Empty;

            for (int i = 1; i <= 2; i++)
            {
                resultValue = fileConent.ResultText;

                if (i == 2)
                {
                    Rtype = "TX";
                    resultValue = fileConent.DiagECG;
                }


                fileConent.DiagECG.Replace(">", "&gt").Replace("<", "&lt");
                tmpXml += string.Format(innerXml,
                    i.ToString(),
                    Rtype,
                    fileConent.OrderItem.ID,
                    StringFormat.TakeOffSpecialChar(fileConent.DiagECG, new string[] { " ", "~" }),
                    "H", "", "",
                    fileConent.Device.ID,
                    fileConent.Device.Name,
                    fileConent.DiagDoct,
                    fileConent.DiagDate.ToString("yyyy-MM-dd HH:mm:ss"));

            }

            string toSys = "HIS";
            if (fileConent.Patient.PatientType == "T")
            {
                toSys = "TJZX";
            }

            string requestXml = string.Format(xml,
                fileConent.Patient.ID,
                fileConent.Patient.PID.CaseNO,
                fileConent.Patient.PID.CardNO,
                fileConent.Patient.Name,
                fileConent.Patient.Sex == "M" ? "男" : "女",
                fileConent.Patient.Birthday.ToString("yyyyMMdd"),
                fileConent.Patient.IDCard,
                fileConent.Patient.Address,
                fileConent.Patient.HomeZIP,
                fileConent.Patient.PhoneNumber,
                "", "",
                fileConent.Patient.PID.CaseNO,
                "", "",//--PID
                fileConent.Patient.PatientType,
                fileConent.Patient.PID.ID,
                fileConent.Patient.Dept.Name,
                "",
                fileConent.Patient.NurseStation.ID,
                "",
                fileConent.Patient.BedNO,
                "", "", "", "N", "", "",//PV
                "CA",
                fileConent.OrderItem.ID, //Weiyd
                fileConent.Patient.Dept.Name,
                fileConent.Patient.DoctorReceiver,
                "", "N", "",
                fileConent.DiagDoct, //orc
                fileConent.OrderItem.ID,
                "",
                "", "",
                fileConent.OrderItem.Item.ID,
                fileConent.OrderItem.Item.Name,
                "", "", "心电", "", "", "", "", "", "",
                fileConent.WEBURL,
                string.Format(fileXml, ""),
                tmpXml,
                "MUSE",//EMR
                "",
                fileConent.DiagDoct,
                fileConent.ReferredDoct,
                "F",
                fileConent.DiagDate.ToString("yyyy-MM-dd HH:mm:ss"),
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                "",
                fileConent.WEBURL,
                "", "", "", "", "", "", "", "", "",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                toSys
                );

            string[] args = { requestXml };

            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[ReportBack]传入参数为：" + requestXml);

            try
            {
                object obj = Common.WebServiceHelper.InvokeWebService(url, "ReportBack", args, ref errMsg);

                if (obj == null)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[ReportBack]报错@" + errMsg);
                    return -1;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[ReportBack]返回参数为：" + obj.ToString());

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(obj.ToString());
                string resultCode = doc.SelectSingleNode("Response/ResultCode").InnerText.Trim();

                if (resultCode == "1") //失败
                {
                    errMsg = doc.SelectSingleNode("Response/ResultContent").InnerText.Trim();
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[ReportBack]返回报错@" + errMsg);
                    return -1;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[ReportBack]返回成功");

                return 1;

            }
            catch (Exception e)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[ReportBack]报错@" + e.Message);
                return -1;
            }
        }

        public int Test(string url, string xml, string method)
        {
            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[" + method + "]入参@" + xml);

            string[] args = { xml };

            try
            {
                object obj = Common.WebServiceHelper.InvokeWebService(url, method, args, ref errMsg);

                if (obj == null)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[" + method + "]报错@" + errMsg);
                    return -1;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[" + method + "]返回参数为:@" + obj.ToString());

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(obj.ToString());

                string resultCode = string.Empty;

                if (method == "getApply")
                {
                    resultCode = doc.SelectSingleNode("ORM/MSA/RESULT_CODE").InnerText.Trim();
                    errMsg = doc.SelectSingleNode("ORM/MSA/RESULT_CONTENT").InnerText.Trim();
                }
                else
                {
                    resultCode = doc.SelectSingleNode("ACK/MSA/RESULT_CODE").InnerText.Trim();
                    errMsg = doc.SelectSingleNode("ACK/MSA/RESULT_CONTENT").InnerText.Trim();
                }

                if (resultCode == "CA") //失败
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[" + method + "]返回报错@" + errMsg);
                    return -1;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[" + method + "]返回成功");

                return 1;

            }
            catch (Exception e)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用WebService[" + method + "]报错@" + e.Message);
                return -1;
            }
        }

        #endregion

        #region  珠海项目

        string msgHeader1 = @"<?xml version=""1.0"" encoding=""utf-8""?><root><serverName>{0}</serverName><format>xml</format><callOperator>1</callOperator><certificate>{1}</certificate></root>";

        public ArrayList QueryPatientCheckInfo(string url, string certificate, string type)
        {
            string xml = @"<root><applyNo>{0}</applyNo><visitType>{1}</visitType></root>";

            string requestXml = string.Format(xml, "", type);

            string msgHeader = string.Empty;

            try
            {
                msgHeader = string.Format(msgHeader1, "GetExamApplyInfo", certificate);

                string[] args = { msgHeader, requestXml };

                log.Info("调用CallInterface[GetExamApplyInfo]传递的参数为：msgHeader==>" + msgHeader + "  msgBody==>" + requestXml);

                object obj = Common.WebServiceHelper.InvokeWebService(url, "CallInterface", args, ref errMsg);

                if (obj == null)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[GetExamApplyInfo]报错@" + errMsg);
                    return null;
                }

                log.Info("调用CallInterface[GetExamApplyInfo]返回的参数为：@" + obj.ToString());

                if (obj.ToString().StartsWith("error"))
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[GetExamApplyInfo]报错@" + obj.ToString());
                    return null;
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(obj.ToString());

                ArrayList al = new ArrayList();

                Order order = null;
                OrderItem item = null;
                string key = string.Empty;
                Hashtable htApplyNo = new Hashtable();
                ArrayList alOrderItems = null;

                XmlNodeList nodeLists = doc.SelectNodes("root/returnContents/returnContent");

                foreach (XmlNode node in nodeLists)
                {
                    order = new Order();
                    //患者信息  注意字段修改之后 要修改好几个地方
                    order.Patient.ID = node.SelectSingleNode("prescribeNo").InnerText.Trim();
                    order.Patient.PID.CardNO = node.SelectSingleNode("visitNo").InnerText.Trim();
                    order.Patient.PID.ID = node.SelectSingleNode("patientId").InnerText.Trim();
                    order.Patient.PatientType = node.SelectSingleNode("visitType").InnerText.Trim();
                    order.Patient.PID.CaseNO = node.SelectSingleNode("safetyNo").InnerText.Trim();
                    order.Patient.PID.HealthNO = node.SelectSingleNode("icCardNo").InnerText.Trim();
                    order.Patient.Name = node.SelectSingleNode("patientName").InnerText.Trim();
                    order.Patient.Sex = node.SelectSingleNode("patientSex").InnerText.Trim();
                    order.Patient.Age = node.SelectSingleNode("patientAge").InnerText.Trim();
                    order.Patient.Birthday = NetScape.AnalysisToolKit.NConvert.ToDateTime(node.SelectSingleNode("patientBirthDay").InnerText.Trim());
                    order.Patient.IDCard = node.SelectSingleNode("identityCardNo").InnerText.Trim();
                    order.Patient.Address = node.SelectSingleNode("employerName").InnerText.Trim();
                    order.Patient.Address = node.SelectSingleNode("contactAddress").InnerText.Trim();
                    order.Patient.PhoneNumber = node.SelectSingleNode("telephone").InnerText.Trim();
                    order.Patient.Memo = node.SelectSingleNode("patientCountry").InnerText.Trim();
                    order.Patient.Nation = node.SelectSingleNode("patientNation").InnerText.Trim();
                    order.Patient.BedNO = node.SelectSingleNode("bedNo").InnerText.Trim();
                    order.ReciptDept.ID = node.SelectSingleNode("visitDeptCode").InnerText.Trim();
                    order.ReciptDept.Name = node.SelectSingleNode("visitDept").InnerText.Trim();
                    //order.ReciptDoctor.Name = 
                    try
                    {
                        string[] doctInfo = node.SelectSingleNode("visitOperator").InnerText.Trim().Split('/');
                        order.ReciptDoctor.ID = doctInfo[1].ToString();
                        order.ReciptDoctor.Name = doctInfo[0].ToString();
                    }
                    catch (Exception e)
                    {

                    }
                    order.Patient.ClinicDiagnose = node.SelectSingleNode("diagnoseCode").InnerText.Trim();
                    order.Patient.MainDiagnose = node.SelectSingleNode("diagnoseName").InnerText.Trim();
                    order.HealthHistory.ID = node.SelectSingleNode("abstractHistory").InnerText.Trim();
                    order.HealthHistory.Name = node.SelectSingleNode("clinicInfo").InnerText.Trim();
                    order.HealthHistory.Memo = node.SelectSingleNode("sickSynptom").InnerText.Trim();

                    XmlNodeList nodeList = node.SelectNodes("examItems/examItem");

                    alOrderItems = new ArrayList();
                    foreach (XmlNode nd in nodeList)
                    {
                        key = nd.SelectSingleNode("applyNo").InnerText.Trim();

                        if (!htApplyNo.ContainsKey(key))
                        {
                            item = new OrderItem();

                            item.ApplyNo = key;
                            item.IsEnhance = NetScape.AnalysisToolKit.NConvert.ToBoolean(nd.SelectSingleNode("emergencyFlag").InnerText.Trim());
                            item.Item.ID = nd.SelectSingleNode("clinicItemID").InnerText.Trim();
                            item.Item.Name = nd.SelectSingleNode("clinicItemName").InnerText.Trim();
                            item.TotCost = NetScape.AnalysisToolKit.NConvert.ToDecimal(nd.SelectSingleNode("charges").InnerText.Trim());
                            item.OperDate = NetScape.AnalysisToolKit.NConvert.ToDateTime(nd.SelectSingleNode("applyDate").InnerText.Trim());
                            item.ApplyDept.ID = nd.SelectSingleNode("applyDept").InnerText.Trim();
                            item.ApplyDept.Name = nd.SelectSingleNode("applyDeptName").InnerText.Trim();
                            item.ApplyOper.ID = nd.SelectSingleNode("applyOperator").InnerText.Trim();
                            item.SysClass.Name = nd.SelectSingleNode("examClass").InnerText.Trim();
                            item.ExecDept.ID = nd.SelectSingleNode("executeDept").InnerText.Trim();
                            item.ExecDept.Name = nd.SelectSingleNode("executeDeptName").InnerText.Trim();
                            item.CheckPart.Name = nd.SelectSingleNode("examPart").InnerText.Trim();
                            item.CheckPart.Memo = nd.SelectSingleNode("examComments").InnerText.Trim();
                            item.Memo = nd.SelectSingleNode("examMethodName").InnerText.Trim(); //方法
                            item.CheckPart.ID = nd.SelectSingleNode("deviceTypeName").InnerText.Trim(); //设备

                            alOrderItems.Add(item);

                            htApplyNo.Add(key, "key");
                        }
                    }

                    htApplyNo.Clear();

                    order.OrderItems = alOrderItems;

                    al.Add(order);
                }

                return al;
            }
            catch (Exception e)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[GetExamApplyInfo]报错@" + e.Message);
                return null;
            }
        }

        public ArrayList QueryPEPatientCheckInfo(string url, string certificate, string type)
        {
            string xml = @"<root><applyNo>{0}</applyNo><visitType>{1}</visitType></root>";

            string requestXml = string.Format(xml, "", type);

            string msgHeader = string.Empty;

            try
            {
                msgHeader = string.Format(msgHeader1, "GetExamApplyInfo", certificate);

                string[] args = { msgHeader, requestXml };

                log.Info("调用CallInterface[GetExamApplyInfo]传递的参数为：msgHeader==>" + msgHeader + "  msgBody==>" + requestXml);

                object obj = Common.WebServiceHelper.InvokeWebService(url, "CallInterface", args, ref errMsg);

                if (obj == null)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[GetExamApplyInfo]报错@" + errMsg);
                    return null;
                }

                log.Info("调用CallInterface[GetExamApplyInfo]返回的参数为：@" + obj.ToString());

                if (obj.ToString().StartsWith("error"))
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[GetExamApplyInfo]报错@" + obj.ToString());
                    return null;
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(obj.ToString());

                ArrayList al = new ArrayList();

                Order order = null;
                OrderItem item = null;
                string key = string.Empty;
                Hashtable htApplyNo = new Hashtable();
                ArrayList alOrderItems = null;

                XmlNodeList nodeLists = doc.SelectNodes("root/returnContents/returnContent");

                foreach (XmlNode node in nodeLists)
                {
                    order = new Order();
                    //患者信息  注意字段修改之后 要修改好几个地方
                    order.Patient.ID = node.SelectSingleNode("prescribeNo").InnerText.Trim();
                    order.Patient.PID.CardNO = node.SelectSingleNode("visitNo").InnerText.Trim();
                    order.Patient.PID.ID = node.SelectSingleNode("patientId").InnerText.Trim();
                    order.Patient.PatientType = node.SelectSingleNode("visitType").InnerText.Trim();
                    order.Patient.PID.CaseNO = node.SelectSingleNode("safetyNo").InnerText.Trim();
                    order.Patient.PID.HealthNO = node.SelectSingleNode("icCardNo").InnerText.Trim();
                    order.Patient.Name = node.SelectSingleNode("patientName").InnerText.Trim();
                    order.Patient.Sex = node.SelectSingleNode("patientSex").InnerText.Trim();
                    order.Patient.Age = node.SelectSingleNode("patientAge").InnerText.Trim();
                    order.Patient.Birthday = NetScape.AnalysisToolKit.NConvert.ToDateTime(node.SelectSingleNode("patientBirthDay").InnerText.Trim());
                    order.Patient.IDCard = node.SelectSingleNode("identityCardNo").InnerText.Trim();
                    order.Patient.Address = node.SelectSingleNode("employerName").InnerText.Trim();
                    order.Patient.Address = node.SelectSingleNode("contactAddress").InnerText.Trim();
                    order.Patient.PhoneNumber = node.SelectSingleNode("telephone").InnerText.Trim();
                    order.Patient.Memo = node.SelectSingleNode("patientCountry").InnerText.Trim();
                    order.Patient.Nation = node.SelectSingleNode("patientNation").InnerText.Trim();
                    order.Patient.BedNO = node.SelectSingleNode("bedNo").InnerText.Trim();
                    order.ReciptDept.ID = node.SelectSingleNode("visitDeptCode").InnerText.Trim();
                    order.ReciptDept.Name = node.SelectSingleNode("visitDept").InnerText.Trim();
                    //order.ReciptDoctor.Name = 
                    try
                    {
                        string[] doctInfo = node.SelectSingleNode("visitOperator").InnerText.Trim().Split('/');
                        order.ReciptDoctor.ID = doctInfo[1].ToString();
                        order.ReciptDoctor.Name = doctInfo[0].ToString();
                    }
                    catch (Exception e)
                    {

                    }
                    order.Patient.ClinicDiagnose = node.SelectSingleNode("diagnoseCode").InnerText.Trim();
                    order.Patient.MainDiagnose = node.SelectSingleNode("diagnoseName").InnerText.Trim();
                    order.HealthHistory.ID = node.SelectSingleNode("abstractHistory").InnerText.Trim();
                    order.HealthHistory.Name = node.SelectSingleNode("clinicInfo").InnerText.Trim();
                    order.HealthHistory.Memo = node.SelectSingleNode("sickSynptom").InnerText.Trim();

                    order.Patient.ID = order.Patient.PID.ID;
                    order.Patient.PID.CardNO = order.Patient.PID.ID;

                    XmlNodeList nodeList = node.SelectNodes("examItems/examItem");
                    alOrderItems = new ArrayList();

                    foreach (XmlNode nd in nodeList)
                    {
                        key = nd.SelectSingleNode("applyNo").InnerText.Trim();

                        if (!htApplyNo.ContainsKey(key))
                        {
                            item = new OrderItem();

                            item.ApplyNo = key;
                            item.IsEnhance = NetScape.AnalysisToolKit.NConvert.ToBoolean(nd.SelectSingleNode("emergencyFlag").InnerText.Trim());
                            item.Item.ID = nd.SelectSingleNode("clinicItemID").InnerText.Trim();
                            item.Item.Name = nd.SelectSingleNode("clinicItemName").InnerText.Trim();
                            item.TotCost = NetScape.AnalysisToolKit.NConvert.ToDecimal(nd.SelectSingleNode("charges").InnerText.Trim());
                            item.OperDate = NetScape.AnalysisToolKit.NConvert.ToDateTime(nd.SelectSingleNode("applyDate").InnerText.Trim());
                            item.ApplyDept.ID = nd.SelectSingleNode("applyDept").InnerText.Trim();
                            item.ApplyDept.Name = nd.SelectSingleNode("applyDeptName").InnerText.Trim();
                            item.ApplyOper.ID = nd.SelectSingleNode("applyOperator").InnerText.Trim();
                            item.SysClass.Name = nd.SelectSingleNode("examClass").InnerText.Trim();
                            item.ExecDept.ID = nd.SelectSingleNode("executeDept").InnerText.Trim();
                            item.ExecDept.Name = nd.SelectSingleNode("executeDeptName").InnerText.Trim();
                            item.CheckPart.Name = nd.SelectSingleNode("examPart").InnerText.Trim();
                            item.CheckPart.Memo = nd.SelectSingleNode("examComments").InnerText.Trim();
                            item.Memo = nd.SelectSingleNode("examMethodName").InnerText.Trim(); //方法
                            item.CheckPart.ID = nd.SelectSingleNode("deviceTypeName").InnerText.Trim(); //设备

                            alOrderItems.Add(item);

                            htApplyNo.Add(key, "key");
                        }
                    }

                    order.Patient.PID.CardNO = key;

                    htApplyNo.Clear();

                    order.OrderItems = alOrderItems;

                    al.Add(order);
                }

                return al;
            }
            catch (Exception e)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[GetExamApplyInfo]报错@" + e.Message);
                return null;
            }

        }

        public int QueryPatientCheckInfo(string url, string certificate, string checkCode, string type, ref Order order)
        {
            string xml = @"<root><applyNo>{0}</applyNo><visitType>{1}</visitType></root>";

            string requestXml = string.Format(xml, checkCode, type);

            string msgHeader = string.Empty;

            try
            {
                msgHeader = string.Format(msgHeader1, "GetExamApplyInfo", certificate);

                string[] args = { msgHeader, requestXml };

                log.Info("调用CallInterface[GetExamApplyInfo]传递的参数为：msgHeader==>" + msgHeader + "  msgBody==>" + requestXml);

                object obj = Common.WebServiceHelper.InvokeWebService(url, "CallInterface", args, ref errMsg);

                if (obj == null)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[GetExamApplyInfo]报错@" + errMsg);
                    return -1;
                }

                log.Info("调用CallInterface[GetExamApplyInfo]返回的参数为：@" + obj.ToString());

                if (obj.ToString().StartsWith("error"))
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[GetExamApplyInfo]报错@" + obj.ToString());
                    return -1;
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(obj.ToString());
                order = new Order();

                //患者信息  注意字段修改之后 要修改好几个地方
                order.Patient.ID = doc.SelectSingleNode("root/returnContents/returnContent/prescribeNo").InnerText.Trim();
                order.Patient.PID.CardNO = doc.SelectSingleNode("root/returnContents/returnContent/visitNo").InnerText.Trim();
                order.Patient.PID.ID = doc.SelectSingleNode("root/returnContents/returnContent/patientId").InnerText.Trim();
                order.Patient.PatientType = doc.SelectSingleNode("root/returnContents/returnContent/visitType").InnerText.Trim();
                order.Patient.PID.CaseNO = doc.SelectSingleNode("root/returnContents/returnContent/safetyNo").InnerText.Trim();
                order.Patient.PID.HealthNO = doc.SelectSingleNode("root/returnContents/returnContent/icCardNo").InnerText.Trim();
                order.Patient.Name = doc.SelectSingleNode("root/returnContents/returnContent/patientName").InnerText.Trim();
                order.Patient.Sex = doc.SelectSingleNode("root/returnContents/returnContent/patientSex").InnerText.Trim();
                order.Patient.Age = doc.SelectSingleNode("root/returnContents/returnContent/patientAge").InnerText.Trim();
                order.Patient.Birthday = NetScape.AnalysisToolKit.NConvert.ToDateTime(doc.SelectSingleNode("root/returnContents/returnContent/patientBirthDay").InnerText.Trim());
                order.Patient.IDCard = doc.SelectSingleNode("root/returnContents/returnContent/identityCardNo").InnerText.Trim();
                order.Patient.Address = doc.SelectSingleNode("root/returnContents/returnContent/employerName").InnerText.Trim();
                order.Patient.Address = doc.SelectSingleNode("root/returnContents/returnContent/contactAddress").InnerText.Trim();
                order.Patient.PhoneNumber = doc.SelectSingleNode("root/returnContents/returnContent/telephone").InnerText.Trim();
                order.Patient.Memo = doc.SelectSingleNode("root/returnContents/returnContent/patientCountry").InnerText.Trim();
                order.Patient.Nation = doc.SelectSingleNode("root/returnContents/returnContent/patientNation").InnerText.Trim();
                order.Patient.BedNO = doc.SelectSingleNode("root/returnContents/returnContent/bedNo").InnerText.Trim();
                order.ReciptDept.ID = doc.SelectSingleNode("root/returnContents/returnContent/visitDeptCode").InnerText.Trim();
                order.ReciptDept.Name = doc.SelectSingleNode("root/returnContents/returnContent/visitDept").InnerText.Trim();
                //order.ReciptDoctor.Name = 
                try
                {
                    string[] doctInfo = doc.SelectSingleNode("root/returnContents/returnContent/visitOperator").InnerText.Trim().Split('/');
                    order.ReciptDoctor.ID = doctInfo[1].ToString();
                    order.ReciptDoctor.Name = doctInfo[0].ToString();
                }
                catch (Exception e)
                {

                }
                order.Patient.ClinicDiagnose = doc.SelectSingleNode("root/returnContents/returnContent/diagnoseCode").InnerText.Trim();
                order.Patient.MainDiagnose = doc.SelectSingleNode("root/returnContents/returnContent/diagnoseName").InnerText.Trim();
                order.HealthHistory.ID = doc.SelectSingleNode("root/returnContents/returnContent/abstractHistory").InnerText.Trim();
                order.HealthHistory.Name = doc.SelectSingleNode("root/returnContents/returnContent/clinicInfo").InnerText.Trim();
                order.HealthHistory.Memo = doc.SelectSingleNode("root/returnContents/returnContent/sickSynptom").InnerText.Trim();

                XmlNodeList nodeLists = doc.SelectNodes("root/returnContents/returnContent/examItems");

                ArrayList alOrderItems = new ArrayList();
                OrderItem item = null;
                foreach (XmlNode node in nodeLists)
                {
                    item = new OrderItem();
                    item.ApplyNo = node.SelectSingleNode("examItem/applyNo").InnerText.Trim();
                    item.IsEnhance = NetScape.AnalysisToolKit.NConvert.ToBoolean(node.SelectSingleNode("examItem/emergencyFlag").InnerText.Trim());
                    item.Item.ID = node.SelectSingleNode("examItem/clinicItemID").InnerText.Trim();
                    item.Item.Name = node.SelectSingleNode("examItem/clinicItemName").InnerText.Trim();
                    item.TotCost = NetScape.AnalysisToolKit.NConvert.ToDecimal(node.SelectSingleNode("examItem/charges").InnerText.Trim());
                    //item.Item.Name = node.SelectSingleNode("examItem/IsFee").InnerText.Trim();
                    //item.CombNO = node.SelectSingleNode("examItem/applyStatus").InnerText.Trim();
                    item.OperDate = NetScape.AnalysisToolKit.NConvert.ToDateTime(node.SelectSingleNode("examItem/applyDate").InnerText.Trim());
                    item.ApplyDept.ID = node.SelectSingleNode("examItem/applyDept").InnerText.Trim();
                    item.ApplyDept.Name = node.SelectSingleNode("examItem/applyDeptName").InnerText.Trim();
                    item.ApplyOper.ID = node.SelectSingleNode("examItem/applyOperator").InnerText.Trim();
                    item.SysClass.Name = node.SelectSingleNode("examItem/examClass").InnerText.Trim();
                    item.ExecDept.ID = node.SelectSingleNode("examItem/executeDept").InnerText.Trim();
                    item.ExecDept.Name = node.SelectSingleNode("examItem/executeDeptName").InnerText.Trim();
                    item.CheckPart.Name = node.SelectSingleNode("examItem/examPart").InnerText.Trim();
                    item.CheckPart.Memo = node.SelectSingleNode("examItem/examComments").InnerText.Trim();
                    item.Memo = node.SelectSingleNode("examItem/examMethodName").InnerText.Trim(); //方法
                    item.CheckPart.ID = node.SelectSingleNode("examItem/deviceTypeName").InnerText.Trim(); //设备

                    alOrderItems.Add(item);
                }

                order.OrderItems = alOrderItems;

                return 1;
            }
            catch (Exception e)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[GetExamApplyInfo]报错@" + e.Message);
                return -1;
            }

        }

        public int ReportCheck(string url, string certificate, Order order)
        {
            string inxml = @"<?xml version=""1.0"" encoding=""utf-8""?><root><patientId>{0}</patientId><visitNo>{1}</visitNo><visitType>{2}</visitType><operator>{3}</operator><patientName>{4}</patientName><operatorTime>{5}</operatorTime><examineMan>{6}</examineMan><adtExamineDate>{7}</adtExamineDate><reportNo>{8}</reportNo><examDevice>{9}</examDevice><examDeviceName>{10}</examDeviceName><applyInfos>{11}</applyInfos></root>";

            string innerxml = @"<applyInfo><applyNo>{0}</applyNo><applyStatus>{1}</applyStatus></applyInfo>";

            string msgHeader = string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (OrderItem item in order.OrderItems)
            {
                sb.Append(string.Format(innerxml, item.ApplyNo, "3"));
            }

            ArrayList alTmp = order.OrderItems;

            OrderItem itemTmp = alTmp[0] as OrderItem;

            string requestXml = string.Format(inxml, order.Patient.PID.ID, order.Patient.PID.CardNO, order.Patient.PatientType, "", order.Patient.Name, DateTime.Now.ToString(), "", "", itemTmp.ApplyNo, "", "", sb.ToString());

            msgHeader = string.Format(msgHeader1, "UpdateExamApplyStatus", certificate);

            string[] args = { msgHeader, requestXml };

            log.Info("调用CallInterface[UpdateExamApplyStatus]传递的参数为:msgHeader ==>" + msgHeader + "  msgBody ==> " + requestXml);

            try
            {
                object obj = Common.WebServiceHelper.InvokeWebService(url, "CallInterface", args, ref errMsg);

                if (obj == null)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[UpdateExamApplyStatus]报错@" + errMsg);
                    return -1;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[UpdateExamApplyStatus]返回参数为:@" + obj.ToString());

                if (obj.ToString().StartsWith("error"))
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[UpdateExamApplyStatus]报错:@" + obj.ToString());
                    return -1;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[UpdateExamApplyStatus]返回成功");

                return 1;

            }
            catch (Exception e)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[UpdateExamApplyStatus]报错@" + e.Message);
                return -1;
            }
        }

        public int WriteBackReport(string url, string certificate, FileConent fileConent)
        {
            string inxml = @"<?xml version='1.0' encoding='utf-8'?><root><patientId>{0}</patientId><visitNo>{1}</visitNo>
                            <visitType>{2}</visitType><emergencyFlag>{3}</emergencyFlag><patientName>{4}</patientName>
                            <patientSex>{5}</patientSex><patientBirthDay>{6}</patientBirthDay><patientAge>{7}</patientAge>
                            <bedNo>{8}</bedNo><diagnoseCode>{9}</diagnoseCode><diagnoseName>{10}</diagnoseName>
                            <medicalHistory>{11}</medicalHistory><applyNos><applyNo>{12}</applyNo>
                            </applyNos><applyOperator>{14}</applyOperator><applyDeptCode>{15}</applyDeptCode>
                            <applyDeptName>{16}</applyDeptName><applyTime>{17}</applyTime><reportNo>{18}</reportNo>
                            <reportTitle>{19}</reportTitle><subjectClass>{20}</subjectClass><itemCode>{21}</itemCode>
                            <itemName>{22}</itemName><examMethod>{23}</examMethod><examPart>{24}</examPart>
                            <examPartDesc>{25}</examPartDesc><visitStateDesc>{26}</visitStateDesc><examImgDesc>{27}</examImgDesc>
                            <examPurpose>{28}</examPurpose><examDevice>{29}</examDevice><examDeviceName>{30}</examDeviceName>
                            <registerTime>{31}</registerTime><registerOperator>{32}</registerOperator><examTime>{33}</examTime>
                            <examDept>{34}</examDept><examOperator>{35}</examOperator><reportTime>{36}</reportTime>
                            <reportOperator>{37}</reportOperator><auditTime>{38}</auditTime><auditOperator>{39}</auditOperator>
                            <printTime>{40}</printTime><printOperator>{41}</printOperator><remark>{42}</remark><parmDetails>
                            <parmDetail parmItemNo='' parmEngName='' parmChiName='' parmValue='' parmSortNo='' remark=''></parmDetail>
                            </parmDetails><reportPdfurl>{43}</reportPdfurl><images><image code='' sub='' typeCode='' remark=''>
                            </image></images><abnormalFlag>{44}</abnormalFlag></root>";


            string msgHeader = string.Empty;

            string requestXml = string.Format(inxml,
                fileConent.Patient.PID.CaseNO,
                fileConent.Patient.ID,
                fileConent.Patient.PatientType,
                "0",
                fileConent.Patient.Name,
                fileConent.Patient.Sex == "M" ? "2" : "1",
                fileConent.Patient.Birthday.ToString("yyyyMMdd"),
                "",//年龄
                fileConent.Patient.BedNO,//床号
                "",//诊断代码
                "",//诊断名称
                "",//病史
                fileConent.OrderItem.ID,//申请单号 ***
                "",
                "",//申请人
                "",//申请科室
                fileConent.Patient.Dept.Name,//申请科室
                "",//申请时间
                "",//报告单号
                "心电图报告",//报告标题
                "020206", //心电检查
                fileConent.OrderItem.Item.ID,
                fileConent.OrderItem.Item.Name,
                "", "", "", "", "", "", "", "", "", "",//检查方法、检查部位、检查部位描述、病情描述、影像描述、检查目的、检查设备、检查设备名称、登记时间、登记人
                fileConent.DiagDate.ToString(), "心电图室", fileConent.DiagDoct,
                fileConent.DiagDate.ToString(), fileConent.DiagDoct,
                "", "", "", "", fileConent.DiagECG,//审核时间、审核人、打印时间、打印人员、备注
                fileConent.WEBURL,
                "0");

            msgHeader = string.Format(msgHeader1, "SendPacsReport", certificate);

            string[] args = { msgHeader, requestXml };

            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[SendPacsReport]传入参数为：msgHeader ==> " + msgHeader + " msgBody ==> " + requestXml);

            try
            {
                object obj = Common.WebServiceHelper.InvokeWebService(url, "CallInterface", args, ref errMsg);

                if (obj == null)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[SendPacsReport]报错@" + errMsg);
                    return -1;
                }


                if (obj.ToString().StartsWith("error"))
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[SendPacsReport]报错：" + obj.ToString());

                    return -1;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[SendPacsReport]");


                return 1;

            }
            catch (Exception e)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[SendPacsReport]报错@" + e.Message);
                return -1;
            }
        }

        #endregion

        #region 中大附一

        /*
          <Request>
    <requestHeader>
      <sender> 2.16.840.1.113883.4.487.2.1.5</sender>
      <receiver> 2.16.840.1.113883.4.487.2.1.3 </receiver>
      <sendTime>20140909130101</sendTime>
      <msgType>RequisitionFind</msgType>
      <msgId>SPEC20140909000009</msgId>
      <msgPriority>Normal</msgPriority>
      <msgVersion>1.0.0</msgVersion>
    </requestHeader>
    <requestBody>
          <RequisitionFind>
	<SheetID>电子申请单号</SheetID>
<ExamID>电子申请单检查项目流水号(标本号)</ExamID>
<PatientId>病人号</PatientId>
<PatientDomainId>病人域ID</PatientDomainId>
<RequisitionType>申请单类型，1-病理、2-放射、3-超声、4-内镜、5-输血、6-心电图、7-耳鼻喉、8-其他专科检查、9-心电生理、10-神经电生理、11-呼吸</RequisitionType>
	</RequisitionFind>
    </requestBody>
  </Request>

         */

        // string msgHeaderZDFY = @"<?xml version=""1.0"" encoding=""utf-8""?><root><serverName>{0}</serverName><format>xml</format><callOperator>1</callOperator><certificate>{1}</certificate></root>";

        Funcs.Function fun = new Funcs.Function();

        /// <summary>
        /// <SheetID>电子申请单号</SheetID>
        ///<ExamID>电子申请单检查项目流水号(标本号)</ExamID>
        ///<PatientId>病人号</PatientId>
        ///<PatientDomainId>病人域ID</PatientDomainId>
        ///<RequisitionType>申请单类型，1-病理、2-放射、3-超声、4-内镜、5-输血、6-心电图、7-耳鼻喉、8-其他专科检查、9-心电生理、10-神经电生理、11-呼吸</RequisitionType>
        /// </summary>
        public int RequisitionFind(string url, string Method, string patientId, string examId, string applyNo, string PatientDomainId, string RequisitionType, string type, ref ArrayList al)
        {
            //string xml = @"<root><applyNo>{0}</applyNo><visitType>{1}</visitType></root>";

            //string requestXml = string.Format(xml, "", type);

            string msgHeader = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(applyNo) && !string.IsNullOrEmpty(patientId))
                {

                }


                //msgHeader = string.Format(msgHeader1, "GetExamApplyInfo", certificate);

                // string[] args = { msgHeader, requestXml };
                /*
                AnalysisModel.Base.MsgSendHeader header = new AnalysisModel.Base.MsgSendHeader("RequisitionFind");

                XElement requestHeader = Funcs.Function.CreateRequestHeader(header);

                XElement requestBody = new XElement("requestBody",
                    new XElement("RequisitionFind",
                    new XElement("SheetID", SheetId),
                     new XElement("ExamID", examId),
                      new XElement("PatientId", patientId),
                     new XElement("PatientDomainId", PatientDomainId),
                      new XElement("RequisitionType", RequisitionType)
                    ));

                log.Info("调用CallInterface[GetExamApplyInfo]传递的参数为：msgHeader==>" + requestHeader.ToString() + "  msgBody==>" + requestBody.ToString());

                requestHeader.Add(requestBody);*/

                string requestBody = @"<RequisitionFind>
<SheetID>{0}</SheetID>
<ExamID>{1}</ExamID>
<PatientId>{2}</PatientId>
<PatientDomainId>{3}</PatientDomainId>
<RequisitionType>{4}</RequisitionType>
</RequisitionFind>";
                requestBody = string.Format(requestBody, applyNo, examId, patientId, PatientDomainId, RequisitionType);

                #region 测试

                /*  requestBody = @"<RequisitionFind>
<SheetID>10601520268</SheetID>
<ExamID></ExamID>
<PatientId></PatientId>
<PatientDomainId></PatientDomainId>
<RequisitionType>2</RequisitionType>
</RequisitionFind>";*/

                /*        requestBody = @"<RequisitionFind>
        <SheetID>20601520252</SheetID>
        <ExamID></ExamID>
        <PatientId></PatientId>
        <PatientDomainId></PatientDomainId>
        <RequisitionType>6</RequisitionType>
        </RequisitionFind> ";*/


                /*   WebNote.RequestNote client = new WebNote.RequestNote();
              
                  // object obj = Common.WebServiceHelper.InvokeWebService(url, "CallInterface", new object[] { requestHeader.ToString() }, ref errMsg);
                   WebNote.Request request = new WebNote.Request();
                   //WebNote.RequestHeader he= new WebNote.RequestHeader();
                   NetScape.AnalysisModel.Base.MsgSendHeader header=new AnalysisModel.Base.MsgSendHeader("RequisitionFind");
                   request.requestHeader = new WebNote.RequestHeader();
                   request.requestHeader.msgId = header.MsgId;
                   request.requestHeader.msgType = header.MsgType;
                   request.requestHeader.msgVersion = header.MsgVersion;
                   request.requestHeader.receiver = header.Receiver;
                   request.requestHeader.sender = header.Sender;
                   request.requestHeader.requestTime = header.SendTime;
                   request.requestBody = requestBody;
                   WebNote.Response response = client.RequisitionFind(request);
                   #region test
                   //string filepath = @"D:\Code\中大附一\ZDFY\中大附一\文档\MUSE\平台消息样例\放射—申请单.txt";
                   //object obj = XDocument.Load(filepath).Root.ToString();

                   //object obj = docE.FirstNode.ToString();
                   //object obj = @" ";
                  string obj = response.responseBody;*/

                   #endregion

                WebNote.RequestNote client = new WebNote.RequestNote();

                // object obj = Common.WebServiceHelper.InvokeWebService(url, "CallInterface", new object[] { requestHeader.ToString() }, ref errMsg);
                WebNote.Request request = new WebNote.Request();
                //WebNote.RequestHeader he= new WebNote.RequestHeader();
                NetScape.AnalysisModel.Base.MsgSendHeader header = new AnalysisModel.Base.MsgSendHeader("RequisitionFind");
                request.requestHeader = new WebNote.RequestHeader();
                request.requestHeader.msgId = header.MsgId;
                request.requestHeader.msgType = header.MsgType;
                request.requestHeader.msgVersion = header.MsgVersion;
                request.requestHeader.receiver = header.Receiver;
                request.requestHeader.sender = header.Sender;
                request.requestHeader.requestTime = header.SendTime;
                request.requestHeader.msgPriority = header.MsgPriority;
                request.requestBody = requestBody;
                WebNote.Response response = client.RequisitionFind(request);

                Neusoft.FrameWork.Function.HisLog.WriteLog("apply", response.responseBody);


                /*       requestBody = @"<RequisitionFind>
       <SheetID>20601520253</SheetID>
       <ExamID></ExamID>
       <PatientId></PatientId>
       <PatientDomainId></PatientDomainId>
       <RequisitionType>6</RequisitionType>
       </RequisitionFind>";
                       request.requestBody = requestBody;
                       response = client.RequisitionFind(request);
                       Neusoft.FrameWork.Function.HisLog.WriteLog("apply", response.responseBody);*/

                // object obj = response.responseBody;
                #region test
                //string filepath = @"D:\Code\中大附一\ZDFY\中大附一\文档\MUSE\平台消息样例\放射—申请单.txt";
                //object obj = XDocument.Load(filepath).Root.ToString();

                //object obj = docE.FirstNode.ToString();
                //object obj = @" ";

                //object[] args = new object[] { request };
                //Common.WebServiceHelper.InvokeWebService(url, "RequisitionFind", args, ref errMsg);

                #endregion

                #region 处理xml

                string obj = response.responseBody;

                if (obj == null || string.IsNullOrEmpty(obj))
                {
                    this.errMsg = "调用[RequisitionFind]取申请单报错@" + errMsg + response.responseHeader.errMessage;
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, errMsg);
                    return -1;
                }

                log.Info("调用[RequisitionFind]取申请单返回的参数为：@" + obj.ToString());

                //if (obj.ToString().StartsWith("error"))
                //{
                //    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"调用CallInterface[GetExamApplyInfo]报错@" + obj.ToString());
                //    return -1;
                //}

                //XmlDocument doc = new XmlDocument();
                //doc.LoadXml(obj.ToString());
                ArrayList allist = new ArrayList();
                if (this.HandleApplyResult(obj.ToString(), ref al) == -1)
                {
                    return -1;
                }

                #endregion

                #region 回写相关结果到数据库

                foreach (Order order in al)
                {
                    //防止已推送过来了。先删掉
                    fun.DeletePatientByApplyNo(order.ID);
                    fun.DeleteApplyDetail(order.ID);

                    if (ExecApplyResult(order, obj.ToString()) == -1)
                    {
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "保存申请单失败！!!" + order.ID);
                    }
                }



                #endregion

                return 1;

            }
            catch (Exception e)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[GetExamApplyInfo]报错@" + e.Message);
                return -1;
            }
            //  return 1;
        }


        /// <summary>
        /// 保存申请单结果到数据库
        /// </summary>
        /// <param name="order"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public int ExecApplyResult(Order order, string xml)
        {
            try
            {
                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                Funcs.Function funMgr = new Funcs.Function();
                // foreach (Order order in al)
                {
                    // int seq = Funcs.Function.QueryMsgFileSeq() ;
                    order.Patient.Value = Funcs.Function.QueryMsgFileSeq().ToString();
                    order.Patient.Ext1 = order.Status;//检查状态
                    order.Patient.Ext2 = order.FeeState;
                    //if (order.Patient.Mark != "1")
                    {
                        if (funMgr.InsertPatientInfo(order) == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            return -1;
                        }
                    }
                    if (funMgr.InsertEcgOrder(order) == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        return -1;
                    }

                    if (funMgr.SaveMsgFile(order, xml) == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        return -1;
                    }

                    //funMgr.SaveMsgFile(order, obj.ToString());
                }
            }
            catch (Exception ex)
            {
                this.errMsg = ex.Message;
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                return -1;
            }
            Neusoft.FrameWork.Management.PublicTrans.Commit();
            return 1;
        }



        /// <summary>
        /// 更新检查状态
        /// </summary>
        /// <param name="setObj"></param>
        /// <param name="state"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public int UpdateCheckState(NetScape.AnalysisModel.SettingObject setObj, string state, NetScape.AnalysisModel.Order order)
        {
            string xml = string.Empty;
            if (fun.GetCheckStateXml(state, order, ref xml) == -1)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, fun.ErrMsg);
                return -1;
            }
            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, xml);
            //string[] args = { xml };
            //object obj = Common.WebServiceHelper.InvokeWebService(setObj.PlatURL, "RequestInfoAdd", args, ref errMsg);
            WebNote.RequestNote client = new WebNote.RequestNote();

            // object obj = Common.WebServiceHelper.InvokeWebService(url, "CallInterface", new object[] { requestHeader.ToString() }, ref errMsg);
            WebNote.Request request = new WebNote.Request();
            //WebNote.RequestHeader he= new WebNote.RequestHeader();
            NetScape.AnalysisModel.Base.MsgSendHeader header = new AnalysisModel.Base.MsgSendHeader("RequisitionAdd");
            request.requestHeader = new WebNote.RequestHeader();
            request.requestHeader.msgId = header.MsgId;
            request.requestHeader.msgType = header.MsgType;
            request.requestHeader.msgVersion = header.MsgVersion;
            request.requestHeader.receiver = header.Receiver;
            request.requestHeader.sender = header.Sender;
            request.requestHeader.requestTime = header.SendTime;
            request.requestBody = xml;

            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, xml);
            WebNote.Response response = client.RequisitionAdd(request);

            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, response.responseBody);

            XElement root = XElement.Parse(response.responseBody);
            List<XElement> results = root.Elements("RequestInfoAddResult").ToList();
            foreach (XElement item in results)
            {
                //string id = item.Element("").Value;
                string result = item.Element("Result").Value.Trim() == "0" ? "成功" : "失败";
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "更新状态结果：" + result);

            }
            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, xml);
            return 1;
        }

        /// <summary>
        /// 更新检查状态
        /// </summary>
        /// <param name="setObj"></param>
        /// <param name="state"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public int UpdateCheckState(NetScape.AnalysisModel.SettingObject setObj, string state, NetScape.AnalysisModel.FileConent content)
        {
            NetScape.AnalysisModel.Order order = new Order();
            order.Patient = content.Patient.Clone();
            ArrayList orderItems = new ArrayList();
            orderItems.Add(content.OrderItem);

            return UpdateCheckState(setObj, state, order);
        }

        /// <summary>
        /// 解析申请单xml ，返回申请单实体列表
        /// </summary>
        /// <param name="obj">xml结构信息</param>
        /// <param name="al">结果实体列表</param>
        /// <returns></returns>
        public int HandleApplyResult(string obj, ref ArrayList al)
        {


            al = new ArrayList();

            Order order = null;
            OrderItem item = null;
            string key = string.Empty;
            Hashtable htApplyNo = new Hashtable();
            List<OrderItem> alOrderItems = null;

            XElement root = XElement.Parse(obj.ToString());

            //  XmlNodeList nodeLists = doc.SelectNodes("root/returnContents/returnContent");

            List<XElement> nodeLists = root.Elements("RequestInfoResult").ToList();
            foreach (XElement node in nodeLists)
            {
                if (node.Element("RequisitionType") != null && !"8||6||9||10||".Contains(node.Element("RequisitionType").Value.Trim()))
                    continue;
                order = new Order();
                //患者信息  注意字段修改之后 要修改好几个地方
                order.Patient.PID.ID = node.Element("JZLSH").Value.Trim();

                order.Patient.PatientType = node.Element("PatientStyle").Value.Trim();
                if ("##0##1##".Contains(order.Patient.PatientType))
                {
                    if (node.Element("OutHospitalID") != null)
                    {
                        order.Patient.ID = node.Element("OutHospitalID").Value.Trim();
                        //order.Patient.PID.CaseNO = node.Element("PatientKH").Value.Trim();//病历号==门诊卡号？？
                        //order.Patient.PID.HealthNO = node.Element("PatientKH").Value.Trim();//健康卡==门诊卡号？？
                        order.Patient.PID.CardNO = node.Element("OutHospitalID").Value.Trim();
                        //order.Patient.PID.PatientNO = node.Element("OutHospitalID").Value.Trim();
                    }
                }
                //else if (order.Patient.PatientType == "1")
                //{
                //    order.Patient.PID.ID = node.Element("OutHospitalID").Value.Trim();
                //    order.Patient.PID.CardNO = node.Element("OutHospitalID").Value.Trim();
                //    order.Patient.PID.PatientNO = node.Element("OutHospitalID").Value.Trim();

                //}
                else if (order.Patient.PatientType == "2")
                {
                    if (node.Element("InHospitalID") != null)
                    {
                        order.Patient.ID = node.Element("InHospitalID").Value.Trim();
                        //order.Patient.PID.PatientNO = node.Element("InHospitalID").Value.Trim();
                        order.Patient.PID.CardNO = node.Element("InHospitalID").Value.Trim();
                    }
                    if (node.Element("PatientBedNum") != null)
                        order.Patient.BedNO = node.Element("PatientBedNum").Value.Trim();
                    if (node.Element("PatientAddress") != null)
                        order.Patient.Address = node.Element("PatientAddress").Value.Trim();
                }

                if (node.Element("PatientName") != null)
                    order.Patient.Name = node.Element("PatientName").Value.Trim();
                if (node.Element("PatientSex") != null)
                    order.Patient.Sex = node.Element("PatientSex").Value.Trim();
                if (node.Element("Patientage") != null)
                    order.Patient.Age = node.Element("Patientage").Value.Trim();
                if (node.Element("Patientage") != null)
                    order.Patient.Birthday = Funcs.Function.ConvertToDateTime(node.Element("Patientage").Value.Trim());
                if (node.Element("IdentityNO") != null)
                    order.Patient.PID.IdenNo = node.Element("IdentityNO").Value.Trim();
                //order.Patient.Address = node.Element("employerName").Value.Trim();
                // order.Patient.Address = node.Element("PatientAddress").Value.Trim();
                if (node.Element("PatientTel") != null)
                    order.Patient.PhoneNumber = node.Element("PatientTel").Value.Trim();
                // 住院暂时没发现这个
                //order.Patient.Memo = node.Element("Nativeplace").Value.Trim();
                //order.Patient.Nation = node.Element("Occupation").Value.Trim();//中大附一接口没有民族，暂存职业
                if (node.Element("SheetID") != null)
                    order.ID = node.Element("SheetID").Value.Trim();
                if (node.Element("DepartMentID") != null)
                    order.ReciptDept.ID = node.Element("DepartMentID").Value.Trim();
                if (node.Element("DepartMent") != null)
                    order.ReciptDept.Name = node.Element("DepartMent").Value.Trim();
                if (node.Element("ReqSheetDoctorID") != null)
                    order.ReciptDoctor.ID = node.Element("ReqSheetDoctorID").Value.Trim();
                if (node.Element("ReqSheetDoctor") != null)
                    order.ReciptDoctor.Name = node.Element("ReqSheetDoctor").Value.Trim();
                if (node.Element("RequisitionType") != null)
                    order.Patient.Ext1 = node.Element("RequisitionType").Value.Trim();

                ////order.ReciptDoctor.Name = 
                //try
                //{
                //    string[] doctInfo = node.Element("visitOperator").Value.Trim().Split('/');
                //    order.ReciptDoctor.ID = doctInfo[1].ToString();
                //    order.ReciptDoctor.Name = doctInfo[0].ToString();
                //}
                //catch (Exception e)
                //{

                //}
                if (node.Element("RequisitionStatus") != null)
                    order.Status = node.Element("RequisitionStatus").Value.Trim();
                if (node.Element("LinChuangZhenDuan") != null)
                    order.Patient.ClinicDiagnose = node.Element("LinChuangZhenDuan").Value.Trim();
                if (node.Element("LinChuangZhenDuan") != null)
                    order.Patient.MainDiagnose = node.Element("LinChuangZhenDuan").Value.Trim();
                //order.HealthHistory.ID = node.Element("abstractHistory").Value.Trim();
                //order.HealthHistory.Name = node.Element("clinicInfo").Value.Trim();
                //order.HealthHistory.Memo = node.Element("sickSynptom").Value.Trim();
                if (node.Element("PAYRESULT") != null)
                    order.FeeState = node.Element("PAYRESULT").Value.Trim();
                if (string.IsNullOrEmpty(order.FeeState))
                    order.FeeState = "1";
                List<XElement> itemList = node.Element("ExamList").Elements("Exam").ToList();
                List<XElement> FeeList = node.Element("FeeList").Elements("Fee").ToList();
                alOrderItems = new List<OrderItem>();
                foreach (XElement nd in itemList)
                {
                    if (nd.Element("SheetID") != null)
                        key = nd.Element("SheetID").Value.Trim();
                    //if (!htApplyNo.ContainsKey(key))
                    {
                        item = new OrderItem();
                        if (nd.Element("id") != null)
                            item.ID = nd.Element("id").Value.Trim();
                        item.ApplyNo = key;
                        //item.IsEnhance = NetScape.AnalysisToolKit.NConvert.ToBoolean(nd.Element("emergencyFlag").Value.Trim());
                        //item.Item.ID = nd.Element("clinicItemID").Value.Trim();
                        //item.Item.Name = nd.Element("clinicItemName").Value.Trim();
                        //item.TotCost = NetScape.AnalysisToolKit.NConvert.ToDecimal(nd.Element("charges").Value.Trim());
                        if (nd.Element("ExamID") != null)
                            item.CheckID = nd.Element("ExamID").Value.Trim();

                        if (node.Element("ReqSheetDate") != null && node.Element("ReqSheetTime") != null)
                            item.OperDate = Funcs.Function.ConvertToDateTime(node.Element("ReqSheetDate").Value.Trim() + node.Element("ReqSheetTime").Value.Trim());

                        if (node.Element("DepartMentID") != null)
                            item.ApplyDept.ID = node.Element("DepartMentID").Value.Trim();

                        if (node.Element("DepartMent") != null)
                            item.ApplyDept.Name = node.Element("DepartMent").Value.Trim();

                        if (node.Element("ReqSheetDoctorID") != null)
                            item.ApplyOper.ID = node.Element("ReqSheetDoctorID").Value.Trim();

                        if (node.Element("RequisitionType") != null)
                            item.SysClass.Name = node.Element("RequisitionType").Value.Trim();//申请单类型？

                        if (node.Element("ExamKSDM") != null)
                            item.ExecDept.ID = node.Element("ExamKSDM").Value.Trim();

                        if (node.Element("ExamKSMC") != null)
                            item.ExecDept.Name = node.Element("ExamKSMC").Value.Trim();

                        if (nd.Element("ExamBodyPart") != null)
                            item.CheckPart.Name = nd.Element("ExamBodyPart").Value.Trim();

                        if (nd.Element("Description") != null)
                            item.CheckPart.Memo = nd.Element("Description").Value.Trim();

                        if (nd.Element("ExamDetails") != null)
                            item.CheckPart.Memo += nd.Element("ExamDetails").Value.Trim(); //描述加部位详细描述

                        if (nd.Element("ExamModality") != null)
                            item.Memo = nd.Element("ExamModality").Value.Trim(); //ExamModality

                        if (nd.Element("ExamBodyNum") != null)
                            item.CheckPart.ID = nd.Element("ExamBodyNum").Value.Trim(); //检查左右（0-左，1-右，2-左右，-1-无）

                        if (node.Element("RequisitionStatus") != null)
                            item.OrderState = node.Element("RequisitionStatus").Value.Trim();//存检查状态
                        alOrderItems.Add(item);

                        // htApplyNo.Add(key, "key");
                    }
                }

                foreach (var fee in FeeList)
                {
                    string sheetId = string.Empty, checkId = string.Empty; ;
                    if (fee.Element("SheetID") != null)
                        sheetId = fee.Element("SheetID").Value.Trim();
                    if (fee.Element("ExamID") != null)
                        checkId = fee.Element("ExamID").Value.Trim();
                    var feeItem = alOrderItems.Where(x => x.CheckID == checkId && x.ApplyNo == sheetId).FirstOrDefault();
                    if (feeItem != null)
                    {
                        if (node.Element("IsEmergent") != null)
                            feeItem.IsEnhance = NetScape.AnalysisToolKit.NConvert.ToBoolean(node.Element("IsEmergent").Value.Trim());
                        if (fee.Element("ItemCode") != null)
                            feeItem.Item.ID = fee.Element("ItemCode").Value.Trim();
                        if (fee.Element("FEENAME") != null)
                            feeItem.Item.Name = fee.Element("FEENAME").Value.Trim();
                        if (node.Element("FeePrice") != null && node.Element("FEENum") != null)
                            feeItem.TotCost = NetScape.AnalysisToolKit.NConvert.ToDecimal(fee.Element("FeePrice").Value.Trim()) * NetScape.AnalysisToolKit.NConvert.ToDecimal(fee.Element("FEENum").Value.Trim());
                        // feeItem.FeeState = fee.Element("FeeState").Value.Trim();
                    }
                }

                htApplyNo.Clear();

                order.OrderItems = new ArrayList(alOrderItems);

                al.Add(order);
            }

            return 1;
        }

        /// <summary>
        /// 解析住院推送申请单xml ，返回申请单实体列表//
        /// </summary>
        /// <param name="obj">xml结构信息</param>
        /// <param name="al">结果实体列表</param>
        /// <returns></returns>
        public List<KeyValuePair<string, Order>> HandleApplyInpatientResult(string obj)
        {
            var result = new List<KeyValuePair<string, Order>>();
            Order order = null;
            string orderXml = null;
            OrderItem item = null;
            string key = string.Empty;
            Hashtable htApplyNo = new Hashtable();
            List<OrderItem> alOrderItems = null;

            XElement root = XElement.Parse(obj.ToString());
            List<XElement> nodeLists = root.Elements("RequestReceive").ToList();


            foreach (XElement node in nodeLists)
            {
                if (node.Element("requisitiontype") == null || !"8||6||9||10||".Contains(node.Element("requisitiontype").Value.Trim()))
                    continue;
                order = new Order();
                
                orderXml = node.ToString();
                orderXml = string.Format("<List>{0}</List>", orderXml);

                //患者信息  注意字段修改之后 要修改好几个地方
                if (node.Element("JZLSH") != null)
                    order.Patient.PID.ID = node.Element("JZLSH").Value.Trim();
                //Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, order.Patient.PID.ID);
                //Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "order.Patient.ID:" + order.Patient.ID + "||order.Patient.PID.ID:" + order.Patient.PID.ID+"  "+order.Patient.PID.ID.Equals(order.Patient.ID).ToString());
                if (node.Element("PatientStyle") != null)
                    order.Patient.PatientType = node.Element("PatientStyle").Value.Trim();
                if (order.Patient.PatientType == "0" || order.Patient.PatientType == "1" || order.Patient.PatientType  == "3")
                {
                    if (node.Element("OutHospitalID") != null)
                    {
                        order.Patient.ID = node.Element("OutHospitalID").Value.Trim();
                        //order.Patient.PID.CaseNO = node.Element("PatientKH").Value.Trim();//病历号==门诊卡号？？
                        //order.Patient.PID.HealthNO = node.Element("PatientKH").Value.Trim();//健康卡==门诊卡号？？
                        order.Patient.PID.CardNO = node.Element("OutHospitalID").Value.Trim();
                        //order.Patient.PID.PatientNO = node.Element("OutHospitalID").Value.Trim();
                    }
                }
                else if (order.Patient.PatientType == "2")
                {
                    if (node.Element("InHospitalID") != null)
                    {
                        order.Patient.ID = node.Element("InHospitalID").Value.Trim();
                        //order.Patient.PID.PatientNO = node.Element("InHospitalID").Value.Trim();
                        order.Patient.PID.CardNO = node.Element("InHospitalID").Value.Trim();
                    }

                    if (node.Element("PatientBedNum") != null)
                        order.Patient.BedNO = node.Element("PatientBedNum").Value.Trim();
                    // Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, order.Patient.PID.ID);
                    //Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "order.Patient.ID:" + order.Patient.ID + "||order.Patient.PID.ID:" + order.Patient.PID.ID + "  " + order.Patient.PID.ID.Equals(order.Patient.ID).ToString());
                }
                if (node.Element("PatientAddress") != null)
                    order.Patient.Address = node.Element("PatientAddress").Value.Trim();

                if (node.Element("PatientName") != null)
                    order.Patient.Name = node.Element("PatientName").Value.Trim();

                if (node.Element("PatientSex") != null)
                    order.Patient.Sex = node.Element("PatientSex").Value.Trim();

                if (node.Element("Patientage") != null)
                    order.Patient.Age = node.Element("Patientage").Value.Trim();

                if (node.Element("Patientage") != null)
                    order.Patient.Birthday = Funcs.Function.ConvertToDateTime(node.Element("Patientage").Value.Trim());

                if (node.Element("IdentityNO") != null)
                    order.Patient.PID.IdenNo = node.Element("IdentityNO").Value.Trim();
                //order.Patient.Address = node.Element("employerName").Value.Trim();
                // order.Patient.Address = node.Element("PatientAddress").Value.Trim();

                if (node.Element("PatientTel") != null)
                    order.Patient.PhoneNumber = node.Element("PatientTel").Value.Trim();

                // 住院暂时没发现这个
                //order.Patient.Memo = node.Element("Nativeplace").Value.Trim();
                //order.Patient.Nation = node.Element("Occupation").Value.Trim();//中大附一接口没有民族，暂存职业
                if (node.Element("SheetID") != null)
                    order.ID = node.Element("SheetID").Value.Trim();

                if (node.Element("DepartMentID") != null)
                    order.ReciptDept.ID = node.Element("DepartMentID").Value.Trim();

                if (node.Element("DepartMent") != null)
                    order.ReciptDept.Name = node.Element("DepartMent").Value.Trim();

                if (node.Element("ReqSheetDoctorID") != null)
                    order.ReciptDoctor.ID = node.Element("ReqSheetDoctorID").Value.Trim();

                if (node.Element("ReqSheetDoctor") != null)
                    order.ReciptDoctor.Name = node.Element("ReqSheetDoctor").Value.Trim();

                if (node.Element("requisitiontype") != null)
                    order.Patient.Ext1 = node.Element("requisitiontype").Value.Trim();

                if (node.Element("RequisitionStatus") != null)
                    order.Status = node.Element("RequisitionStatus").Value.Trim();

                if (node.Element("LinChuangZhenDuan") != null)
                {
                    order.Patient.ClinicDiagnose = node.Element("LinChuangZhenDuan").Value.Trim();
                    order.Patient.MainDiagnose = node.Element("LinChuangZhenDuan").Value.Trim();
                }
                //order.HealthHistory.ID = node.Element("abstractHistory").Value.Trim();
                //order.HealthHistory.Name = node.Element("clinicInfo").Value.Trim();
                //order.HealthHistory.Memo = node.Element("sickSynptom").Value.Trim();
                if (node.Element("PAYRESULT") != null)
                    order.FeeState = node.Element("PAYRESULT").Value.Trim();
                if (string.IsNullOrEmpty(order.FeeState))
                    order.FeeState = "1";
                List<XElement> itemList = node.Element("ExamList").Elements("Exam").ToList();
                List<XElement> FeeList = node.Element("FeeList").Elements("Fee").ToList();
                alOrderItems = new List<OrderItem>();

                foreach (XElement nd in itemList)
                {
                    if (nd.Element("SheetID") != null)
                        key = nd.Element("SheetID").Value.Trim();
                    //if (!htApplyNo.ContainsKey(key))
                    {
                        item = new OrderItem();
                        if (nd.Element("id") != null)
                            item.ID = nd.Element("id").Value.Trim();
                        item.ApplyNo = key;
                        //item.IsEnhance = NetScape.AnalysisToolKit.NConvert.ToBoolean(nd.Element("emergencyFlag").Value.Trim());
                        //item.Item.ID = nd.Element("clinicItemID").Value.Trim();
                        //item.Item.Name = nd.Element("clinicItemName").Value.Trim();
                        //item.TotCost = NetScape.AnalysisToolKit.NConvert.ToDecimal(nd.Element("charges").Value.Trim());
                        if (nd.Element("ExamID") != null)
                            item.CheckID = nd.Element("ExamID").Value.Trim();

                        if (node.Element("ReqSheetDate") != null && node.Element("ReqSheetTime") != null)
                            item.OperDate = Funcs.Function.ConvertToDateTime(node.Element("ReqSheetDate").Value.Trim() + node.Element("ReqSheetTime").Value.Trim());

                        if (node.Element("DepartMentID") != null)
                            item.ApplyDept.ID = node.Element("DepartMentID").Value.Trim();

                        if (node.Element("DepartMent") != null)
                            item.ApplyDept.Name = node.Element("DepartMent").Value.Trim();

                        if (node.Element("ReqSheetDoctorID") != null)
                            item.ApplyOper.ID = node.Element("ReqSheetDoctorID").Value.Trim();

                        if (node.Element("RequisitionType") != null)
                            item.SysClass.Name = node.Element("RequisitionType").Value.Trim();//申请单类型？

                        if (node.Element("ExamKSDM") != null)
                            item.ExecDept.ID = node.Element("ExamKSDM").Value.Trim();

                        if (node.Element("ExamKSMC") != null)
                            item.ExecDept.Name = node.Element("ExamKSMC").Value.Trim();

                        if (nd.Element("ExamBodyPart") != null)
                            item.CheckPart.Name = nd.Element("ExamBodyPart").Value.Trim();

                        if (nd.Element("Description") != null)
                            item.CheckPart.Memo = nd.Element("Description").Value.Trim();

                        if (nd.Element("ExamDetails") != null)
                            item.CheckPart.Memo += nd.Element("ExamDetails").Value.Trim(); //描述加部位详细描述

                        if (nd.Element("ExamModality") != null)
                            item.Memo = nd.Element("ExamModality").Value.Trim(); //ExamModality

                        if (nd.Element("ExamBodyNum") != null)
                            item.CheckPart.ID = nd.Element("ExamBodyNum").Value.Trim(); //检查左右（0-左，1-右，2-左右，-1-无）

                        if (node.Element("RequisitionStatus") != null)
                            item.OrderState = node.Element("RequisitionStatus").Value.Trim();//存检查状态
                        alOrderItems.Add(item);

                        // htApplyNo.Add(key, "key");
                    }
                }

                foreach (var fee in FeeList)
                {
                    string sheetId = string.Empty, checkId = string.Empty; ;
                    if (fee.Element("SheetID") != null)
                        sheetId = fee.Element("SheetID").Value.Trim();
                    if (fee.Element("ExamID") != null)
                        checkId = fee.Element("ExamID").Value.Trim();
                    var feeItem = alOrderItems.Where(x => x.CheckID == checkId && x.ApplyNo == sheetId).FirstOrDefault();
                    if (feeItem != null)
                    {
                        if (node.Element("IsEmergent") != null)
                            feeItem.IsEnhance = NetScape.AnalysisToolKit.NConvert.ToBoolean(node.Element("IsEmergent").Value.Trim());
                        if (fee.Element("ItemCode") != null)
                            feeItem.Item.ID = fee.Element("ItemCode").Value.Trim();
                        if (fee.Element("FEENAME") != null)
                            feeItem.Item.Name = fee.Element("FEENAME").Value.Trim();
                        if (node.Element("FeePrice") != null && node.Element("FEENum") != null)
                            feeItem.TotCost = NetScape.AnalysisToolKit.NConvert.ToDecimal(fee.Element("FeePrice").Value.Trim()) * NetScape.AnalysisToolKit.NConvert.ToDecimal(fee.Element("FEENum").Value.Trim());
                        // feeItem.FeeState = fee.Element("FeeState").Value.Trim();
                    }
                }

                htApplyNo.Clear();

                order.OrderItems = new ArrayList(alOrderItems);
                result.Add(new KeyValuePair<string, Order>(orderXml, order));
            }

            return result;
        }

        public int RebackFeeState(string xml)
        {
            XElement root = XElement.Parse(xml);

            return 1;
        }

        public int QueryApplyBill(string clinicCode, string cardNo, ref Order order, ref string xml)
        {
            if (string.IsNullOrEmpty(clinicCode)&&string.IsNullOrEmpty(cardNo))
            {
                this.errMsg = "门诊号不能为空！请重新扫描输入！";
                return -1;
            }


            string requestBody = @"<FLOW_ID>{0}</FLOW_ID> <PATIENT_ID>{1}</PATIENT_ID><EXEC_DPCD>{2}</EXEC_DPCD><BEGIN_TIME>{3}</BEGIN_TIME><END_TIME>{4}</END_TIME>
";
            int days = 60;
            if (!string.IsNullOrEmpty(NetScape.AnalysisModel.Profile.ConfigSetting.QueryDays))
            {
                days = Neusoft.FrameWork.Function.NConvert.ToInt32(NetScape.AnalysisModel.Profile.ConfigSetting.QueryDays);
            }
            string end = DateTime.Now.AddDays(1).ToString("yyyyMMddHHmmss");
            string begin = DateTime.Now.AddDays(-days).ToString("yyyyMMddHHmmss");
            requestBody = string.Format(requestBody, clinicCode, cardNo, NetScape.AnalysisModel.Profile.ConfigSetting.EcgExecDept, begin, end);

            try
            {
                MessageNote.MessageService client = new MessageNote.MessageService();

                NetScape.AnalysisModel.Profile.DomainInfo reciver = fun.GetPatientDomain("1");//门诊

                // object obj = Common.WebServiceHelper.InvokeWebService(url, "CallInterface", new object[] { requestHeader.ToString() }, ref errMsg);
                MessageNote.Request request = new MessageNote.Request();
                //WebNote.RequestHeader he= new WebNote.RequestHeader();
                NetScape.AnalysisModel.Base.MsgSendHeader header = new AnalysisModel.Base.MsgSendHeader("OutRecipeInfo");
                header.Receiver = reciver.ID;
                request.requestHeader = new MessageNote.RequestHeader();
                request.requestHeader.msgId = header.MsgId;
                request.requestHeader.msgType = header.MsgType;
                request.requestHeader.msgVersion = header.MsgVersion;
                request.requestHeader.receiver = header.Receiver;
                request.requestHeader.sender = header.Sender;
                request.requestHeader.requestTime = header.SendTime;
                request.requestHeader.msgPriority = header.MsgPriority;
                request.requestBody = requestBody;

                Neusoft.FrameWork.Function.HisLog.WriteLog(LogName._msg, requestBody);

                MessageNote.Response response = client.messageQuery(request);


                if (response.responseHeader.errCode != "0")
                {
                    this.errMsg = "查询申请单失败！" + response.responseHeader.errMessage;
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, errMsg);
                }
                string patientId=cardNo;
                if(string.IsNullOrEmpty(cardNo))
                    patientId=clinicCode;

                Neusoft.FrameWork.Function.HisLog.WriteLog(LogName._msg, response.responseBody);
                return this.ResolveRequestXml(response.responseBody, ref order, ref xml,patientId);
            }
            catch (Exception ex)
            {
                this.errMsg = ex.Message;
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, ex.Message);
                return -1;
            }

            //return 1;

        }

        /// <summary>
        /// 解析申请单Xml,返回检查项目实体（旧系统）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int ResolveRequestXml(string reqXml, ref Order order, ref string applyXml,string patientId)
        {
            if (string.IsNullOrEmpty(reqXml))
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "Xml 为空！！！");
                return -1;
            }

            OrderItem item = null;

            if (order == null)
            {
                order = new Order();
            }

            try
            {
                XElement root = XElement.Parse(reqXml);
                List<XElement> recipess = root.Elements("OutRecipeInfo").ToList();
                if (recipess == null || recipess.Count == 0)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "查找结点错误:没有找到Xml元素| root.Element(‘responseBody‘).Element(‘List‘).Elements(‘OutRecipeInfo‘).ToList()");
                    return -1;
                }
                if (order.OrderItems == null)
                    order.OrderItems = new ArrayList();
                if (order.Patient == null)
                    order.Patient = new PatientInfo();
                XElement temp = new XElement("List");

                List<string> _group = new List<string>();

                foreach (var obj in recipess)
                {
                    if (obj.Element("CLINIC_CODE") == null || obj.Element("MO_ORDER") == null)
                    {
                        obj.Remove();
                        continue;
                    }
                    if (obj.Element("CLASS_CODE") == null)
                    {
                        obj.Remove();
                        continue;
                    }

                    if (obj.Element("CLASS_CODE").Value.Trim() != "UC")
                    {
                        obj.Remove();
                        continue;
                    }

                    //退费的去掉
                    if (obj.Element("CANCEL_FLAG") != null)
                    {
                        if (obj.Element("CANCEL_FLAG").Value.Trim() != "0")
                        {
                            obj.Remove();
                            continue;
                        }
                    }

                    if (obj.Element("COMB_NO") != null)
                    {
                        if (_group.Contains(obj.Element("COMB_NO").Value.Trim()))
                        {
                            obj.Remove();
                            continue;
                        }
                        else
                        {
                            _group.Add(obj.Element("COMB_NO").Value.Trim());
                        }
                    }


                    string itemCode = string.Empty;
                    string itemName = string.Empty;
                    if (obj.Element("PACKAGE_CODE") != null)
                    {
                        itemCode = obj.Element("PACKAGE_CODE").Value.Trim();
                        itemName = obj.Element("PACKAGE_NAME").Value.Trim();
                    }
                    else
                    {
                        itemCode = obj.Element("ITEM_CODE").Value.Trim();
                        itemName = obj.Element("ITEM_NAME").Value.Trim();
                    }
                    string execDeptCode = obj.Element("EXEC_DPCD").Value.Trim();
                    if (string.IsNullOrEmpty(itemCode))
                    {
                        obj.Remove();
                        continue;
                    }

                    if (!Common.ConstManager.CheckEcgItemsExists(itemCode))
                    {
                        obj.Remove();
                        continue;
                    }

                    //if (!Common.ConstManager.CheckEcgExecDeptExists(execDeptCode))
                    //{
                    //    obj.Remove();
                    //    continue;
                    //}
                    string clinicCode = obj.Element("CLINIC_CODE").Value;
                    string applyNo = obj.Element("MO_ORDER").Value;
                    if (true)
                    {
                        string state = fun.QueryOrderState(applyNo, clinicCode);
                        if (!string.IsNullOrEmpty(state))
                            if ("#3#4#5".Contains(state))
                            {
                                obj.Remove();
                                continue;
                            }
                    }

                    string card_no = obj.Element("CARD_NO").Value;
                    if (card_no!=patientId)
                    {
                        obj.Remove();
                        continue;
                    }

                    item = new OrderItem();
                    order.ID = applyNo;
                    order.Patient.PID.ID = clinicCode; //    <CLINIC_CODE>门诊号</CLINIC_CODE>
                    order.Patient.PID.CardNO = card_no;//<CARD_NO>病历卡号</CARD_NO>
                    order.Patient.ID = card_no;//<CARD_NO>病历卡号</CARD_NO>
                    if (obj.Element("DOCT_CODE") != null)
                    {
                        order.ReciptDoctor.ID = obj.Element("DOCT_CODE").Value;//<DOCT_CODE>开方医师</DOCT_CODE>
                        item.ApplyOper.ID = order.ReciptDoctor.ID;
                    }
                    if (obj.Element("REG_DPCD") != null)
                    {
                        order.ReciptDept.ID = obj.Element("REG_DPCD").Value;//<DOCT_DEPT>开方医师所在科室</DOCT_DEPT>
                        item.ApplyDept.ID = obj.Element("REG_DPCD").Value;
                    }
                    else if (obj.Element("DOCT_DEPT") != null)
                    {
                        order.ReciptDept.ID = obj.Element("DOCT_DEPT").Value;//<DOCT_DEPT>开方医师所在科室</DOCT_DEPT>
                        item.ApplyDept.ID = obj.Element("DOCT_DEPT").Value;
                    }
                    item.ExecDept.ID = obj.Element("EXEC_DPCD").Value;
                    item.ExecDept.Name = obj.Element("EXEC_DPNM").Value;

                    //item.ApplyDept.Name = obj.Element("").Value;
                    item.Item.ID = itemCode;// obj.Element("ITEM_CODE").Value;//<ITEM_CODE>复合项目代码</ITEM_CODE>
                    item.Item.Name = itemName;// obj.Element("ITEM_NAME").Value;//<ITEM_NAME>复合项目名称</ITEM_NAME>
                    item.IsEnhance = Neusoft.FrameWork.Function.NConvert.ToBoolean(obj.Element("EMC_FLAG").Value);
                    item.ID = applyNo; //<MO_ORDER>医嘱项目流水号或者体检项目流水号</MO_ORDER>
                    item.ApplyNo = applyNo; //<BILL_NO>申请单号</BILL_NO>
                    if (obj.Element("FEE_DATE") != null)
                    {
                        item.OperDate = Funcs.Function.ConvertToDateTime(obj.Element("FEE_DATE").Value.Trim());

                    }
                    // if (obj.Element("PAY_FLAG").Value.Trim() == "2")
                    {
                        order.FeeState = "2";
                    }
                    XElement el = new XElement("List");
                    el.Add(obj);
                    item.Value = el.ToString();
                    order.OrderItems.Add(item);
                    temp.Add(obj);

                }
                if (order.OrderItems != null && order.OrderItems.Count > 0)
                {
                    applyXml = temp.ToString();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                this.errMsg = "解析xml过程异常：" + ex.Message;
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, errMsg);
                return -1;
            }
            this.errMsg = "没有查找到该患者的心电检查项目！";// +ex.Message;
            return -1;
        }


        /// <summary>
        /// 拆分多个患者的申请单消息
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public string SplitXmlByPatient(string xml, PatientInfo info, string type)
        {
            XElement root = XElement.Parse(xml);
            List<XElement> bodyList = root.Elements(type).ToList();
            foreach (XElement item in bodyList)
            {
                if (item.Element("JZLSH").Value.Trim() == info.ID)
                {
                    XElement r = new XElement("List");
                    r.Add(item);
                    return r.ToString();
                }
            }

            return string.Empty;

        }
        /// <summary>
        /// 拆分多个患者的申请单消息
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public string SplitXmlByApplyNo(string xml, string applyNo, string type)
        {
            XElement root = XElement.Parse(xml);
            List<XElement> bodyList = root.Elements(type).ToList();
            foreach (XElement item in bodyList)
            {
                if (item.Element("SheetID").Value.Trim() == applyNo)
                {
                    XElement r = new XElement("List");
                    r.Add(item);
                    return r.ToString();
                }
            }
            return string.Empty;
        }


        public int CheckBill(string clinicCode, string cardNo, ref ArrayList al)
        {
            Order order = null;
            string xml = string.Empty;
            if (QueryApplyBill(clinicCode, cardNo, ref order, ref xml) == -1)
            {
                return -1;
            }
            PatientInfo p = fun.QueryPatientInfo(order.Patient.ID, "'0','1'");
            if (p == null)
            {
                this.errMsg = "没有找到匹配的患者基本信息！";
                return -1;
            }
            order.Patient = p;



            NetScape.AnalysisModel.Constant _item = null;
            if (order.ReciptDept != null && string.IsNullOrEmpty(order.ReciptDept.Name))
            {
                _item = Common.ConstManager.GetDeptItem(order.ReciptDept.ID);
                if (_item != null)
                    order.ReciptDept.Name = _item.Name;
            }
            if (order.ReciptDoctor != null && string.IsNullOrEmpty(order.ReciptDoctor.Name))
            {
                order.ReciptDoctor.Name = Common.ConstManager.GetEmployee(order.ReciptDoctor.ID).Name;
            }

            if (fun.RemovePatient(p.PID.ID) == -1)
            {
                this.errMsg = " 更新患者信息失败";
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "更新患者信息失败");
            }
            foreach (NetScape.AnalysisModel.OrderItem item in order.OrderItems)
            {
                item.ApplyDept.Name = order.ReciptDept.Name;
                item.ApplyOper.Name = order.ReciptDoctor.Name;

                Order obj = new Order();
                obj.Patient = order.Patient.Clone();
                obj.Patient.State = "0";
                obj.Status = "1";
                obj.FeeState = order.FeeState;
                obj.ReciptDept = order.ReciptDept.Clone();
                obj.ReciptDoctor = order.ReciptDoctor.Clone();
                obj.ID = item.ApplyNo;
                obj.FeeState = "2";
                obj.Patient.Mark = "1";
                if (obj.OrderItems == null)
                {
                    obj.OrderItems = new ArrayList();
                }
                obj.OrderItems.Add(item);
                // XElement root = new XElement("List");

                if (this.ExecApplyResult(obj, item.Value) == -1)
                {
                    return -1;
                }
            }



            if (al == null)
            {
                al = new ArrayList();
            }
            al.Add(order);
            return 1;
        }


                #endregion

        #region CCA
        public int RegisterPatient(string url, FileConent fileConent)
        {
            string inxml = @"<root><PatientMPIID>{0}</PatientMPIID><PatientName>{1}</PatientName>
                            <PatientBirthday>{2}</PatientBirthday><Sex>{3}</Sex><SSN>{4}</SSN>
                            <Address>{5}</Address><HomeTel>{6}</HomeTel></root>";

            inxml = string.Format(inxml, fileConent.Patient.ID, fileConent.Patient.Name, fileConent.Patient.Birthday.ToString("yyyy-MM-dd"),
                                    fileConent.Patient.Sex, "", "", "");

            string[] args = { inxml };

            string errMsg = string.Empty;
            log.Info("调用GE[adt_a04]传递的参数为: " + inxml);

            try
            {
                object obj = Common.WebServiceHelper.InvokeWebService(url, "adt_a04", args, ref errMsg);

                if (obj == null)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用GE[adt_a04]报错@" + errMsg);
                    return -1;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用GE[adt_a04]返回参数为:@" + obj.ToString());

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(obj.ToString());

                string result = doc.SelectSingleNode("Response/ResultCode").InnerText.ToString();

                if (result == "1")
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用GE[adt_a04]报错:@" + obj.ToString());
                    return -1;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用GE[adt_a04]返回成功");

                return 1;

            }
            catch (Exception e)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用GE[adt_a04]报错@" + e.Message);
                return -1;
            }

        }

        public int RuturnPathToCCA(string url, FileConent fileConent, ref string errorInfo)
        {
            string inxml = @"<root><ReportID>{0}</ReportID><PatientMPIID>{1}</PatientMPIID><AccessionNumber>{2}</AccessionNumber>
                            <DepartmentCode>{3}</DepartmentCode><DepartmentName>{4}</DepartmentName><PatientName>{5}</PatientName><PatientBirthday>{6}</PatientBirthday>
                            <Sex>{7}</Sex><AdmNo>{8}</AdmNo><HISOrderID>{9}</HISOrderID><ApproveName>{10}</ApproveName><ApproveDate>{11}</ApproveDate>
                            <ReportName>{12}</ReportName><ReportDate>{13}</ReportDate><ImagePath>{14}</ImagePath><ReportPath>{15}</ReportPath>
                            <ExamResult>{16}</ExamResult><PatientSource>{17}</PatientSource><ExamSource>{18}</ExamSource></root>";

            string type = "O";
            if (fileConent.Patient.PatientType == "1")
            {
                type = "O";
            }
            else if (fileConent.Patient.PatientType == "2")
            {
                type = "I";
            }
            else if (fileConent.Patient.PatientType == "3")
            {
                type = "P";
            }

            try
            {
                inxml = string.Format(inxml, fileConent.OrderItem.ID, fileConent.Patient.ID, "", "", fileConent.Patient.Dept.Name,
                    fileConent.Patient.Name, fileConent.Patient.Birthday.ToString("yyyy-MM-dd"), fileConent.Patient.Sex,
                    fileConent.Patient.ID, fileConent.OrderItem.ID, fileConent.DiagDoct, fileConent.DiagDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    fileConent.DiagDoct, fileConent.DiagDate.ToString("yyyy-MM-dd HH:mm:ss"), fileConent.WEBURL, fileConent.WEBURL,
                    "", type, "ECG");
            }
            catch (Exception e)
            {
                errorInfo = "参数赋值出错！" + e.Message;
                return -1;
            }
            string[] args = { inxml };

            string errMsg = string.Empty;
            log.Info("调用GE[submitReport]传递的参数为: " + inxml);

            try
            {
                object obj = Common.WebServiceHelper.InvokeWebService(url, "submitReport", args, ref errMsg);

                if (obj == null)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用GE[submitReport]报错@" + errMsg);
                    return -1;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用GE[submitReport]返回参数为:@" + obj.ToString());

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(obj.ToString());

                string result = doc.SelectSingleNode("Response/ResultCode").InnerText.ToString();

                if (result == "1")
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用GE[submitReport]报错:@" + obj.ToString());
                    return -1;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用GE[submitReport]返回成功");

                return 1;

            }
            catch (Exception e)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用GE[submitReport]报错@" + e.Message);
                return -1;
            }

        }
        #endregion

        #region 自动扣费
        public int ChargeItem(string url, string certificate, Order order, string transType)
        {
            string inxml = @"<?xml version=""1.0"" encoding=""utf-8""?><root><applyNo>{0}</applyNo><patientName>{1}</patientName><visitNo>{2}</visitNo><visitType>{3}</visitType><operator>{4}</operator><operatorStatus>{5}</operatorStatus><executeDept>{6}</executeDept></root>";
            string requestXml = string.Empty;

            string msgHeader = string.Empty;
            foreach (OrderItem item in order.OrderItems)
            {
                requestXml = string.Format(inxml, item.ApplyNo, order.Patient.Name, order.Patient.PID.CardNO, order.Patient.PatientType, "8888", transType, "心电图");

                msgHeader = string.Format(msgHeader1, "UpdateExamRegister", certificate);

                string[] args = { msgHeader, requestXml };

                log.Info("调用CallInterface[UpdateExamRegister]传递的参数为:msgHeader ==>" + msgHeader + "  msgBody ==> " + requestXml);

                try
                {
                    object obj = Common.WebServiceHelper.InvokeWebService(url, "CallInterface", args, ref errMsg);

                    if (obj == null)
                    {
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[UpdateExamRegister]报错@" + errMsg);
                        return -1;
                    }

                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[UpdateExamRegister]返回参数为:@" + obj.ToString());

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(obj.ToString());

                    string result = doc.SelectSingleNode("root/return").InnerText;

                    if (result == "00")
                    {
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[UpdateExamRegister]报错:@" + obj.ToString());
                        return -1;
                    }

                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[UpdateExamRegister]返回成功");
                }
                catch (Exception e)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "调用CallInterface[UpdateExamRegister]报错@" + e.Message);
                    return -1;
                }
            }

            return 1;
        }
        #endregion
    }
}
