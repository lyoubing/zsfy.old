
using Analysis.Service.InpatientPay.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

namespace Analysis.Service.InpatientPay
{
    /// <summary>
    /// ReciveFee 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://www.gzsums.net/requisitionPay/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class RequisitionPay : IRequisitionPaySoap
    {

       // private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        NetScape.AnalysisWork.Funcs.Function func = new NetScape.AnalysisWork.Funcs.Function();
        NetScape.AnalysisWork.Work.MessageSend msgSend = new NetScape.AnalysisWork.Work.MessageSend();
        NetScape.AnalysisModel.SettingObject setObj = NetScape.AnalysisWork.Common.SettingHelper.setObj;
        NetScape.AnalysisWork.Work.PlatInterface palt = new NetScape.AnalysisWork.Work.PlatInterface();
        DataAccess dataMgr = new DataAccess();

      //  [WebMethod(Description = "住院申请单费用状态变更")]
        


        //[WebMethod(Description = "测试用")]
        //public string HelloWorld()
        //{
        //    Request request = new Request();
        //    XmlDocument doc = new XmlDocument();//.Load(@"D:\sample3.txt");
        //    doc.Load(@"D:\sample3.txt");
        //    request.requestBody = doc.InnerXml;
        //    //request.requestBody = root.ToString();
        //    EcgInpatientService ser = new EcgInpatientService();
        //    ser.RequestDataReceive(request);
        //    return "Hello World";
        //}


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


        public Response RequisitionPayReceive(Request request)
        {
       
            if (request == null || string.IsNullOrEmpty(request.requestBody))
            {
                return CreateResp("RequisitionPayInfo", request.requestBody, "1", "消息内容为空！！");
            }

            XElement root = XElement.Parse(request.requestBody);
            List<XElement> list = root.Elements("RequisitionPayInfo").ToList();
            if (list == null || list.Count == 0)
            {
                return CreateResp("RequisitionPayInfo", request.requestBody, "1", "查找结点RequisitionPayInfo失败 ！！");
            }
            XElement body = new XElement("List");

            foreach (var item in list)
            {
                string applyNo = item.Element("SheetID").Value;
                string state = item.Element("PayResult").Value;
                if (func.ApplyNoExists(applyNo))
                {
                    if (func.UpdateFeeStateByApplyNo(applyNo, state) != -1)
                    {
                        item.Element("PayResult").ReplaceWith(new XElement("Result", "0"));
                    }
                    else
                        item.Element("PayResult").ReplaceWith(new XElement("Result", "1"));
                    body.Add(item);
                }
                else
                {
                    item.Element("PayResult").ReplaceWith(new XElement("Result", "1"));
                    body.Add(item);
                }
            }

            return CreateResp("RequisitionPayInfo", list.ToString(), "0", "d ！！");
        
        }

       //[WebMethod]
       // public string Test()
       // {
       //     return dataMgr.GetDateTimeFromSysDateTime().ToString();
       // }
    }
}
