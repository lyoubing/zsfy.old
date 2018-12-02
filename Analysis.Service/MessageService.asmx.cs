using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

namespace Analysis.Service
{
    /// <summary>
    /// MessageService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://www.gzsums.net/noticeService/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
     //若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
     //[System.Web.Script.Services.ScriptService]
    public class MessageService : IMessageServiceSoap
    {
        public Response messageNotice(Request request)
        {
            Neusoft.FrameWork.Function.HisLog.WriteLog("worker", request.requestBody);
            Response resp = null;
            if (string.IsNullOrEmpty(request.requestBody))
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog("worker", "消息体为空！");
                resp = GetRespHeader();
                resp.responseBody = string.Format(respBody, string.Empty, "1", "消息体为空");
                return resp;
            }

            XElement root = XElement.Parse(request.requestBody);

            List<XElement> registers = root.Elements("OutPatientInfo").ToList();

            if (registers == null && registers.Count == 0)
            {

                Neusoft.FrameWork.Function.HisLog.WriteLog("worker", "消息体为空！");
                resp = GetRespHeader();
                resp.responseBody = string.Format(respBody, string.Empty, "1", "处理结果失败！");
                return resp;
            }

            XElement listNote = new XElement("List");

            foreach (var item in registers)
            {
                string clinicCode = item.Element("CLINIC_CODE").Value;
                string result = string.Empty;
                if (func.ReciveRegister(item) == 1)
                {
                    result = string.Format(restItem, clinicCode, "0", "处理成功");
                    listNote.Add(XElement.Parse(result));
                    continue;
                }
                else
                {
                    result = string.Format(restItem, clinicCode, "1", "接受患者信息失败！错误：" + func.ErrMsg);
                    listNote.Add(XElement.Parse(result));
                    continue;
                }

            }


            resp = GetRespHeader();
            resp.responseBody = listNote.ToString();
            Neusoft.FrameWork.Function.HisLog.WriteLog("worker", resp.responseBody);
            return resp;
        }

        //public Response messageQuery(Request req)
        //{
        //    throw new NotImplementedException();
        //}

        //public Response messageRecieve(Request req)
        //{
        //    throw new NotImplementedException();
        //}

        NetScape.AnalysisWork.Funcs.Function func = new NetScape.AnalysisWork.Funcs.Function();

     //   [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        /*
                /// <summary>
                ///  MessageService
                /// </summary>
                /// <returns></returns>
                [WebMethod(Description = "门诊患者注册")]
                public Response MessageRecive(Request request)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog("worker", request.requestBody);
                    Response resp = null;
                    if (string.IsNullOrEmpty(request.requestBody))
                    {
                        Neusoft.FrameWork.Function.HisLog.WriteLog("worker", "消息体为空！");
                        resp = GetRespHeader();
                        resp.responseBody = string.Format(respBody, string.Empty, "1", "消息体为空");
                        return resp;
                    }

                    XElement root = XElement.Parse(request.requestBody);

                    List<XElement> registers = root.Elements("OutPatientInfo").ToList();

                    if (registers == null && registers.Count == 0)
                    {

                        Neusoft.FrameWork.Function.HisLog.WriteLog("worker", "消息体为空！");
                        resp = GetRespHeader();
                        resp.responseBody = string.Format(respBody, string.Empty, "1", "处理结果失败！");
                        return resp;
                    }

                    XElement listNote = new XElement("List");

                    foreach (var item in registers)
                    {
                        string clinicCode = item.Element("CLINIC_CODE").Value;
                        string result = string.Empty;
                        if (func.ReciveRegister(item) == 1)
                        {
                            result = string.Format(restItem, clinicCode, "0", "处理成功");
                            listNote.Add(XElement.Parse(result));
                            continue;
                        }
                        else
                        {
                            result = string.Format(restItem, clinicCode, "-1", "接受患者信息失败！错误：" + func.ErrMsg);
                            listNote.Add(XElement.Parse(result));
                            continue;
                        }

                    }


                    resp = GetRespHeader();
                    resp.responseBody = listNote.ToString();
                    return resp;
                }
                */
        string respBody = @"<List>
<Result>
<clinic_code>{0}</clinic_code>
<status>{1}</status>
<message>{2}</message>
</Result>
</List>
";

        string restItem = @" <Result>
<clinic_code>{0}</clinic_code>
<status>{1}</status>
<message>{2}</message>
</Result>";


        public Response GetRespHeader()
        {
            Response resp = new Response();
            if (resp.responseHeader == null)
                resp.responseHeader = new ResponseHeader();
            resp.responseHeader.errCode = "0";
            resp.responseHeader.msgId = "ECGRP" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            resp.responseHeader.msgPriority = "Normal";
            resp.responseHeader.msgType = "RequestReceive";
            resp.responseHeader.msgVersion = "1.0.0";
            resp.responseHeader.receiver = "2.16.840.1.113883.4.487.2.1.45";
            resp.responseHeader.requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            resp.responseHeader.sender = "2.16.840.1.113883.4.487.2.1.4";
            //  resp.responseBody = resultXml;
            return resp;
        }


        public Request GetReqHeader()
        {
            Request resp = new Request();
            if (resp.requestHeader == null)
                resp.requestHeader = new RequestHeader();
            // resp.requestHeader.errCode = "0";
            resp.requestHeader.msgId = "ECGRP" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            resp.requestHeader.msgPriority = "Normal";
            resp.requestHeader.msgType = "RequestReceive";
            resp.requestHeader.msgVersion = "1.0.0";
            resp.requestHeader.receiver = "2.16.840.1.113883.4.487.2.1.45";
            resp.requestHeader.requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            resp.requestHeader.sender = "2.16.840.1.113883.4.487.2.1.4";
            return resp;
        }


       // [WebMethod(Description = "测试接口")]

        public object TestMsgRecive(string xml)
        {
            Request resp = GetReqHeader();
            resp.requestBody = "<List><OutPatientInfo><CARD_NO>0022052549</CARD_NO><DEPT_NAME>生殖医学中心</DEPT_NAME><SEENO>4</SEENO><REGLEVL_CODE>2</REGLEVL_CODE><YNFR>1</YNFR><ORDER_NO>2150</ORDER_NO><PACT_NAME>现金</PACT_NAME><SEE_DOCD>002724</SEE_DOCD><SEE_DPCD>0551</SEE_DPCD><SEX_CODE>F</SEX_CODE><APPEND_FLAG>0</APPEND_FLAG><END_TIME>oracle.sql.TIMESTAMP@1a7fcdce</END_TIME><PAYKIND_CODE>01</PAYKIND_CODE><RELA_PHONE>15818912189</RELA_PHONE><BIRTHDAY>19770806000000</BIRTHDAY><VALID_FLAG>1</VALID_FLAG><DEPT_CODE>0551</DEPT_CODE><SCHEMA_NO>3036647</SCHEMA_NO><REG_DATE>oracle.sql.TIMESTAMP@b0bc73f</REG_DATE><PACT_CODE>1</PACT_CODE><RECIPE_NO>R001158292</RECIPE_NO><OPER_CODE>ATM109</OPER_CODE><OPER_DATE>oracle.sql.TIMES TAMP@648c5b63</OPER_DATE><NAME>詹春妍</NAME><YNREGCHRG>0</YNREGCHRG><CANCEL_DATE>or acle.sql.TIMESTAMP@1823ab6a</CANCEL_DATE><BEGIN_TIME>oracle.sql.TIMESTAMP@7c5604 1d</BEGIN_TIME><IDENNO>440923197708060787</IDENNO><REGLEVL_NAME>专科</REGLEVL_NAME><ADDRESS>广东省电白县观珠镇教管会宿舍</ADDRESS><YNBOOK>0</YNBOOK><CLINIC_CODE>41923913</CLINIC_CODE><NOON_CODE>2</NOON_CODE></OutPatientInfo></List>";

         //   return this.MessageRecive(resp).responseBody;
            return string.Empty;
        }




        public Response messageQuery(Request req)
        {
            throw new NotImplementedException();
        }

        public Response messageRecieve(Request req)
        {
            throw new NotImplementedException();
        }
    }






}
