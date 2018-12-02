using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using System.Xml;
using Analysis.Service.InpatientPay.Data;
using NetScape.AnalysisModel;

namespace Analysis.Service.Inpatient
{
    /// <summary>
    /// EcgInpatientService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://www.gzsums.net/requisition/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class RequestNote : IRequestNoteSoap
    {
        // private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        NetScape.AnalysisWork.Funcs.Function func = new NetScape.AnalysisWork.Funcs.Function();
        NetScape.AnalysisWork.Work.MessageSend msgSend = new NetScape.AnalysisWork.Work.MessageSend();
        NetScape.AnalysisModel.SettingObject setObj = NetScape.AnalysisWork.Common.SettingHelper.setObj;
        NetScape.AnalysisWork.Work.PlatInterface palt = new NetScape.AnalysisWork.Work.PlatInterface();
        DataAccess dataMgr = new DataAccess();

        // string date { get { return TestDataAccess.TestGetDate(); } }

        // [WebMethod(Description = "心电检查住院批量推送接收服务")]
        // public requisition.Response RequestDataReceive(requisition.Request request)
        // {
        //     Neusoft.FrameWork.Function.HisLog.WriteLog("inp", date);
        //     string applyXml = request.requestBody;
        //     //  log.Debug("住院推送申请单入参：" + applyXml);
        //     Neusoft.FrameWork.Function.HisLog.WriteLog("inp", request.requestBody);
        //     string resultXml = string.Empty;
        //     ArrayList list = new ArrayList();
        //     if (applyXml.Contains("soap:"))
        //     {
        //         applyXml = applyXml.Replace("soap:", "");
        //         // obj = ele.ToString();


        //         XmlDocument doc = new XmlDocument();
        //         doc.LoadXml(applyXml);
        //         applyXml = doc.LastChild.LastChild.LastChild.LastChild.LastChild.InnerText;// node.ToString();
        //     }

        //     int result = palt.HandleApplyResult(applyXml, ref list);
        //     Neusoft.FrameWork.Function.HisLog.WriteLog("inp", "Result：" + result.ToString());
        //     Neusoft.FrameWork.Function.HisLog.WriteLog("inp", list.Count.ToString());
        //     if (list.Count < 0)
        //     {
        //         resultXml = Function.ResponseXml("1", "处理申请单信息失败！" + palt.errMsg).ToString();
        //         Neusoft.FrameWork.Function.HisLog.WriteLog("inp", "处理申请单信息失败！" + palt.errMsg);

        //         log.Debug("xml 转换实体order 结果为空！");
        //     }

        //     foreach (NetScape.AnalysisModel.Order item in list)
        //     {
        //         //ArrayList alMsg = func.ConvertOrderToHL7(item);

        //         //result = 0;
        //         //foreach (NetScape.AnalysisModel.HL7MSG hl7Msg in alMsg)
        //         //{
        //         //    result = msgSend.Send(setObj.GEIP, int.Parse(setObj.GEPort), hl7Msg);

        //         //    if (result < 0)
        //         //    {
        //         //        log.Debug("发送申请到到MUSE系统失败！" + hl7Msg);
        //         //        resultXml=Function.ResponseXml("1","发送申请到到MUSE系统失败！").ToString();
        //         //        //MessageBox.Show("");
        //         //        //return;
        //         //    }
        //         //}

        //         string patientXml = palt.SplitXmlByPatient(applyXml, item.Patient);
        //         Neusoft.FrameWork.Function.HisLog.WriteLog("inp", patientXml);
        //         if (palt.ExecApplyResult(item, patientXml) == -1)
        //         {
        //             Neusoft.FrameWork.Function.HisLog.WriteLog("inp", palt.errMsg);
        //         }
        //     }
        //     resultXml = Function.ResponseXml("0", "接收申请单成功!").ToString();

        //     requisition.Response resp = new requisition.Response();

        //     if (resp.responseHeader == null)
        //         resp.responseHeader = new requisition.ResponseHeader();
        //     resp.responseHeader.errCode = "0";
        //     resp.responseHeader.msgId = "ECGRP" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        //     resp.responseHeader.msgPriority = "Normal";
        //     resp.responseHeader.msgType = "RequestReceive";
        //     resp.responseHeader.msgVersion = "1.0.0";
        //     resp.responseHeader.receiver = "2.16.840.1.113883.4.487.2.1.45";
        //     resp.responseHeader.requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
        //     resp.responseHeader.sender = "2.16.840.1.113883.4.487.2.1.4";
        //     resp.responseBody = resultXml;
        //     return resp;
        // }

        //// [WebMethod(Description = "住院申请单费用状态变更")]
        // public requisition.Response RequisitionPayInfo(requisition.Request request)
        // {
        //     if (request == null || string.IsNullOrEmpty(request.requestBody))
        //     {
        //         return CreateResp("RequisitionPayInfo", request.requestBody, "1", "消息内容为空！！");
        //     }

        //     XElement root = XElement.Parse(request.requestBody);
        //     List<XElement> list = root.Elements("RequisitionPayInfo").ToList();
        //     if (list == null || list.Count == 0)
        //     {
        //         return CreateResp("RequisitionPayInfo", request.requestBody, "1", "查找结点RequisitionPayInfo失败 ！！");
        //     }
        //     XElement body = new XElement("List");

        //     foreach (var item in list)
        //     {
        //         string applyNo = item.Element("SheetID").Value;
        //         string state = item.Element("PayResult").Value;
        //         if (func.ApplyNoExists(applyNo))
        //         {
        //             if (func.UpdateFeeStateByApplyNo(applyNo, state) != -1)
        //             {
        //                 item.Element("PayResult").ReplaceWith(new XElement("Result", "0"));
        //             }
        //             else
        //                 item.Element("PayResult").ReplaceWith(new XElement("Result", "1"));
        //             body.Add(item);
        //         }
        //         else
        //         {
        //             item.Element("PayResult").ReplaceWith(new XElement("Result", "1"));
        //             body.Add(item);
        //         }
        //     }

        //     return CreateResp("RequisitionPayInfo", list.ToString(), "0", "d ！！");
        // }


        // [WebMethod(Description = "测试用")]
        // public string HelloWorld()
        // {
        //     requisition.Request request = new requisition.Request();
        //     XmlDocument doc = new XmlDocument();//.Load(@"D:\sample3.txt");
        //     doc.Load(@"D:\sample3.txt");
        //     request.requestBody = doc.InnerXml;
        //     //request.requestBody = root.ToString();
        //     RequestNote ser = new RequestNote();
        //     ser.RequestDataReceive(request);
        //     return "Hello World";
        // }

        string _log = "worker";

        public Response CreateResp(string msgType, string msgBody, string errCode, string errMsg)
        {
            Response resp = new Response();

            if (resp.responseHeader == null)
                resp.responseHeader = new ResponseHeader();
            resp.responseHeader.errCode = errCode;
            resp.responseHeader.errMessage = errMsg;
            resp.responseHeader.msgId = "ECGRP" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            resp.responseHeader.msgPriority = "Normal";
            resp.responseHeader.msgType = msgType;
            resp.responseHeader.msgVersion = "1.0.0";
            resp.responseHeader.receiver = "2.16.840.1.113883.4.487.2.1.45";
            resp.responseHeader.requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            resp.responseHeader.sender = "2.16.840.1.113883.4.487.2.1.4";
            resp.responseBody = msgBody;
            return resp;
        }

        [WebMethod]
        public Response RequestDataReceive(Request request)
        {
            if (string.IsNullOrWhiteSpace(request.requestBody))
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_log, "Request body is Empty.");
            }

            string applyXml = request.requestBody;

            string resultXml = string.Empty;

            if (applyXml.Contains("soap:"))
            {
                applyXml = applyXml.Replace("soap:", "");

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(applyXml);
                applyXml = doc.LastChild.LastChild.LastChild.LastChild.LastChild.InnerText;// node.ToString();
            }

            var list = palt.HandleApplyInpatientResult(applyXml);

            if (list.Count <= 0)
            {
                resultXml = Function.ResponseXml("1001", "处理申请单信息失败！申请单列表为空!").ToString();
                Neusoft.FrameWork.Function.HisLog.WriteLog(_log, "申请单列表为空！");
            }

            foreach (var pair in list)
            {
                Order order = pair.Value;
                string orderXml = pair.Key;

                if (palt.ExecApplyResult(order, orderXml) == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_log, palt.errMsg);
                }

                #region 发送HL7给Muse

                ArrayList alMsg = func.ConvertOrderToHL7(order);
                
                foreach (NetScape.AnalysisModel.HL7MSG hl7Msg in alMsg)
                {
                    int result = msgSend.Send(setObj.GEIP, int.Parse(setObj.GEPort), hl7Msg);                    

                    if (result < 0)
                    {
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_log, "发送申请到到MUSE系统失败！" + hl7Msg);
                        resultXml = Function.ResponseXml("1001", "发送申请到到MUSE系统失败！").ToString();
                    }
                    #region 判断是否15或18导, 若是则加发一个申请单.
                    if(result > 0)
                    {
                        //10733: 15导
                        //11087: 15导+床边
                        //22161: 18导
                        //22162: 18导+床边
                        if ("22161|||22162".Contains(hl7Msg.HisOrderType))
                        {
                            hl7Msg.OrderNumber += "-s";
                            msgSend.Send(setObj.GEIP, int.Parse(setObj.GEPort), hl7Msg);
                        }
                    }
                    #endregion
                }

                #endregion
            }
            if(string.IsNullOrEmpty(resultXml))
            resultXml = Function.ResponseXml("0", "接收申请单成功!").ToString();
            Neusoft.FrameWork.Function.HisLog.WriteLog(_log, resultXml);
            return CreateResp("RequestReceive", resultXml);
           
        }

        public Response CreateResp(string type,string body)
        {
            Response resp = new Response();

            if (resp.responseHeader == null)
                resp.responseHeader = new ResponseHeader();
            resp.responseHeader.errCode = "0";
            resp.responseHeader.msgId = "ECGRP" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            resp.responseHeader.msgPriority = "Normal";
            resp.responseHeader.msgType = type;
            resp.responseHeader.msgVersion = "1.0.0";
            resp.responseHeader.receiver = "2.16.840.1.113883.4.487.2.1.45";
            resp.responseHeader.requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            resp.responseHeader.sender = "2.16.840.1.113883.4.487.2.1.4";
            resp.responseBody = body;            
            return resp;
        }
    }
}
