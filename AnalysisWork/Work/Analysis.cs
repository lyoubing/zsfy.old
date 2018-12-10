using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using log4net;
using NetScape.AnalysisModel;
using NetScape.AnalysisWork.Common;
using NetScape.AnalysisToolKit;
using System.Collections;

using Logger = Neusoft.FrameWork.Function.HisLog;


namespace NetScape.AnalysisWork.Work
{
    public class Analysis
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        PlatInterface plateInterface = new PlatInterface();

        MessageSend messageSend = new MessageSend();

        public static string _logName = "worker";
        #region 不要了

        //public static SettingObject setObj = new SettingObject();
        //private string settingFile = "setting.xml";

        //public int GetSetting()
        //{
        //    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"Start 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);

        //    XmlDocument doc = new XmlDocument();
        //    try
        //    {
        //        doc.Load(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + settingFile);
        //    }
        //    catch (Exception e)
        //    {
        //        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"Load XML 文件出错！" + e.Message);
        //        return -1;
        //    }

        //    XmlNode node;
        //    try
        //    {
        //        node = doc.SelectSingleNode(@"/SETTING/HOSPITAL");
        //        setObj.HospitalCode = node.Attributes[0].Value;
        //        setObj.HospitalName = node.Attributes[1].Value;

        //        node = doc.SelectSingleNode(@"/SETTING/DIR");
        //        setObj.FileDIR = node.Attributes[0].Value;
        //    }
        //    catch (Exception ex)
        //    {
        //        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"解析 XML 文件出错！" + ex.Message);
        //        return -1;
        //    }

        //    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"End 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
        //    return 0;
        //}
        #endregion

        //DateTime dtQuery = DateTime.Now;

        public void DOWork()
        {
            try
            {
                Logger.WriteLog(_logName, "开始 muse 报告回写处理.");

                SettingHelper.GetSetting();
                if (!Directory.Exists(SettingHelper.setObj.FileDIR)) Logger.WriteLog(_logName, "muse 报告回写的hl7消息文件夹不存在."); ;

                string[] hl7Files = Directory.GetFiles(SettingHelper.setObj.FileDIR);

                if (hl7Files == null || hl7Files.Length == 0)
                {
                    Logger.WriteLog(_logName, "*************************没有要处理的 muse 报告**********************");
                    return;
                }

                foreach (string hl7File in hl7Files)
                {
                    string hl7FileName = Path.GetFileName(hl7File);

                    Logger.WriteLog(_logName, "开始进行处理hl7文件@" + hl7FileName);
                    try
                    {
                        FileConent fileConent = this.GetResult(hl7File);

                        if (string.IsNullOrEmpty(fileConent.OrderItem.ID))
                        {
                            //ht.Add(fileName, fileName);
                            Logger.WriteLog(_logName, "此报告是用户自定义的，不用上传！@" + hl7FileName);
                            File.Delete(hl7File);
                            continue;
                        }
                        int result = new Funcs.Function().HandleReportResult(fileConent);

                        if (result > 0)
                        {
                            Logger.WriteLog(_logName, "hl7文件处理完成@" + hl7FileName);
                            //之后删除目录下的文件
                            File.Delete(hl7File);
                        }
                        else
                        {
                            Logger.WriteLog(_logName, "hl7文件处理失败@" + hl7FileName);
                            var failedFolder = SettingHelper.setObj.FileDIR.TrimEnd('\\') + "\\hl7Out_failed\\";
                            if (!Directory.Exists(failedFolder)) Directory.CreateDirectory(failedFolder);
                            var failedFile = failedFolder + hl7FileName;
                            File.Move(hl7File, failedFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog(_logName, string.Format("hl7文件处理失败@{0}, exception:{1}", hl7FileName, ex.ToString()));
                        var failedFolder = SettingHelper.setObj.FileDIR.TrimEnd('\\') + "\\hl7Out_failed\\";
                        if (!Directory.Exists(failedFolder)) Directory.CreateDirectory(failedFolder);
                        var failedFile = failedFolder + hl7FileName;
                        File.Move(hl7File, failedFile);
                    }
                }

                Logger.WriteLog(_logName, "结束 muse 报告回写处理.");
            }
            catch (Exception ex)
            {
                try
                {
                    Logger.WriteLog(_logName, "muse 报告回写处理异常:" + ex.ToString());
                }
                catch {
                    //keep do not throw any exceptions
                }
            }
        }

        /// <summary>
        /// 读hl7消息
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public FileConent GetResult(string file)
        {
            FileConent fileConent = new FileConent();
            PatientInfo patient = new PatientInfo();
            fileConent.Patient = patient;
            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs, Encoding.Default))
                {
                    string text = string.Empty;
                    string[] str;
                    while (!reader.EndOfStream)
                    {
                        text = reader.ReadLine();

                        if (text.Contains("PID"))
                        {
                            str = text.Split('|');

                            patient.ID = str[2]; //门诊号                            
                            patient.Name = StringFormat.TakeOffSpecialChar(str[5]); //姓名 要去掉符号^
                            patient.Birthday = Funcs.Function.ConvertToDateTime(str[7].ToString());//出生日期
                            patient.Sex = str[8];//性别
                            //str[19]; //412-12-9866
                        }
                        else if (text.Contains("PV1"))
                        {
                            str = text.Split('|');
                            // Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, patient.ID);
                            if (str[2].ToString() == "INPAT")
                            {
                                patient.PatientType = "2";
                            }
                            else if (str[2].ToString() == "PREADE")
                            {
                                patient.PatientType = "3";
                                if (patient.ID.Length > 10 && patient.ID.Substring(0, 1) == "8")
                                    patient.ID = patient.ID.Substring(1);
                            }
                            else
                            {
                                patient.PatientType = "0";
                                if (patient.ID.Length > 10 && patient.ID.Substring(0, 1) == "9")
                                    patient.ID = patient.ID.Substring(1);
                                // Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, patient.ID);
                            }

                            string[] positions = str[3].Split('^'); //患者位置
                            patient.NurseStation.Name = positions[1];
                            patient.Dept.Name = positions[6];
                            patient.BedNO = positions[2];
                            patient.PID.CaseNO = str[19]; //就诊号
                            patient.PID.ID = str[19];

                            if (!string.IsNullOrEmpty(str[8]))
                            {
                                positions = str[8].Split('^');
                                string flag = positions[10];
                                if (!string.IsNullOrEmpty(flag))
                                {
                                    fileConent.IsMultiReport = true;
                                    fileConent.ReportSequence = int.Parse(flag.Substring(flag.Length - 1));

                                    if (flag.StartsWith("15")) fileConent.SpecialAttributes |= SpecialAttribute.Channel15;
                                    else if (flag.StartsWith("18")) fileConent.SpecialAttributes |= SpecialAttribute.Channel18;
                                }
                            }
                            //fileConent.Patient.InDate = Convert.ToDateTime(str[44]); //入院时间
                            //fileConent.Patient.OutDate = Convert.ToDateTime(str[45]); //出院时间
                        }
                        else if (text.Contains("OBR"))
                        {
                            str = text.Split('|');

                            string applyno = str[2];
                            //处理15/18导时添加的后缀
                            if (applyno.EndsWith("-s"))
                            {
                                applyno = applyno.Remove(applyno.Length - 2, 2);
                            }
                            fileConent.OrderItem.ID = applyno;

                            fileConent.ID = str[20];

                            string[] result = str[4].Split('^');

                            fileConent.OrderItem.Item.ID = result[0].ToString();
                            fileConent.OrderItem.Item.Name = result[1].ToString();

                            try
                            {
                                // string chk = str[6];

                                fileConent.CheckDate = Funcs.Function.ConvertToDateTime(str[7].Split('^')[0]);
                                fileConent.DiagDate = Funcs.Function.ConvertToDateTime(str[14]);

                                fileConent.DiagDoct = str[str.Length - 2].Substring(1, 8).Replace("^", "");
                            }
                            catch
                            {
                                fileConent.CheckDate = DateTime.Now;
                                fileConent.DiagDate = DateTime.Now;
                            }
                        }


                        //else if (text.Contains("OBX") && text.Contains("TX"))
                        //{
                        //    if (text.Contains("Coded") || text.Contains("Class"))
                        //    {

                        //    }
                        //    else
                        //    {
                        //        str = text.Split('|');
                        //        fileConent.DiagECG = str[5].ToString().Replace('~', ',');
                        //    }
                        //}
                        //else if (text.Contains("OBX") && text.Contains("FT"))
                        //{
                        //    str = text.Split('|');

                        //    fileConent.ResultText = str[5].ToString().Substring(str[5].ToString().IndexOf("Vent"), str[5].ToString().IndexOf("Referred By") - str[5].ToString().IndexOf("Vent"));

                        //    string[] results = str[5].Split('~');

                        //    string[] res = DealResultString(results[2]);
                        //    fileConent.VentRate = res[0];
                        //    fileConent.AtrialRate = res[1];

                        //    res = DealResultString(results[3]);
                        //    fileConent.PRInt = res[0];
                        //    fileConent.QRSDur = res[1];

                        //    res = DealResultString(results[4]);
                        //    fileConent.QTInt = res[0];
                        //    fileConent.PRInt = res[1];

                        //    fileConent.QTcInt = results[5];
                        //    //fileConent.DiagDoct = results[7];
                        //    //fileConent.DiagECG = results[8];

                        //    //res = DealResultString(results[9]);
                        //    //fileConent.ReferredDoct = res[0];
                        //    //fileConent.DiagDoct = res[1];

                        //    string tmpDoct = str[5].ToString().Substring(str[5].ToString().IndexOf("诊断医生") + 5);
                        //    string[] docts = tmpDoct.Split(' ');

                        //    fileConent.DiagDoct = docts[1].ToString() + docts[0].ToString();

                        //}

                        //
                        //else if (text.Contains("ECG"))
                        //{
                        //    str = text.Split('|');
                        //    fileConent.DiagDate = str[11];// +" " + str[6];
                        //}
                        #region 诊断
                        //else if (text.Contains("OBX") && text.Contains("TX"))
                        else if (text.Contains("208.0^Diagnosis"))
                        {
                            str = text.Split('|');
                            fileConent.DiagECG = str[5].ToString().Replace('~', ',');
                        }
                        #endregion

                        #region PR 期间

                        else if (text.Contains("OBX|5|"))
                        {
                            str = text.Split('|');
                            fileConent.VentRate = str[5] + " " + str[6];
                        }

                        #endregion

                        #region AtrialRate

                        else if (text.Contains("OBX|6|"))
                        {
                            str = text.Split('|');
                            fileConent.AtrialRate = str[5] + " " + str[6];
                        }

                        #endregion

                        #region PR 期间
                        else if (text.Contains("OBX|7|"))
                        {
                            str = text.Split('|');
                            fileConent.PRInt = str[5] + " " + str[6];
                        }
                        #endregion

                        #region Qrs

                        else if (text.Contains("OBX|8|"))
                        {
                            str = text.Split('|');
                            fileConent.QRSDur = str[5] + " " + str[6];
                        }
                        #endregion

                        #region Qt
                        //qt
                        else if (text.Contains("OBX|9|"))
                        {
                            str = text.Split('|');
                            fileConent.QTInt = str[5] + " " + str[6];
                        }
                        #endregion

                        #region QTC
                        //qtc
                        else if (text.Contains("OBX|10|"))
                        {
                            str = text.Split('|');
                            fileConent.QTcInt = str[5] + " " + str[6];
                        }
                        #endregion

                        #region PRT 电轴
                        else if (text.Contains("OBX|12|"))
                        {
                            str = text.Split('|');
                            fileConent.PRTAxes = str[5] + " ";
                            fileConent.PAxes = str[5];
                        }

                        else if (text.Contains("OBX|14|"))
                        {
                            str = text.Split('|');
                            fileConent.PRTAxes += str[5] + " ";
                            fileConent.RAxes = str[5];
                        }
                        else if (text.Contains("OBX|16|"))
                        {
                            str = text.Split('|');
                            fileConent.TAxes = str[5];
                            fileConent.AxesUnit = str[6];
                            fileConent.PRTAxes += str[5] + " " + str[6];
                        }
                        #endregion

                        #region Pdf Url

                        else if (text.Contains("RP") && text.Contains("MUSEWebURL"))
                        {
                            string tmpURL = text.Substring(text.IndexOf("http"), text.IndexOf("PDF|") + 3 - text.IndexOf("http")).Replace(@"\T\", "&");
                            fileConent.WEBURL = tmpURL.Replace("XINDIAN-SERVER", SettingHelper.setObj.GEIP).Replace("无患者编号", "").Replace("+", "%2B");
                        }

                        #endregion

                        #region RV5

                        else if (text.Contains("RA^R"))
                        {
                            fileConent.RAmplitude = text.Split('|')[5].Split('^')[6];
                        }
                        else if (text.Contains("SA^S"))
                        {
                            fileConent.SAmplitude = text.Split('|')[5].Split('^')[2];
                        }

                        #endregion

                        //#region Diag

                        //else if (text.Contains("OBX|87"))
                        //{
                        //    str = text.Split('|');
                        //    fileConent.DiagECG = str[5].Replace("~", ",");//+ " ";
                        //}

                        //#endregion
                    }
                }
            }

            return fileConent;
        }

        public void BatchWork()
        {
            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "Start 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);

            DateTime dtMax = Common.SettingHelper.GetMaxTimeSetting();

            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "获取申请项目的最大时间是" + dtMax.ToString());

            DateTime dtTmp = DateTime.MinValue;

            ArrayList al = plateInterface.QueryPatientCheckInfo(SettingHelper.setObj.PlatURL, SettingHelper.setObj.Certificate, "1");

            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "返回" + al.Count + "条记录！");
            int updateCount = 0;
            OrderItem itemTmp = null;

            foreach (NetScape.AnalysisModel.Order order in al)
            {
                itemTmp = order.OrderItems[0] as OrderItem;

                if (dtMax >= itemTmp.OperDate)
                {
                    continue;
                }

                if (updateCount == 0)
                {
                    dtTmp = itemTmp.OperDate;
                }

                if (dtTmp < itemTmp.OperDate)
                {
                    dtTmp = itemTmp.OperDate;
                }

                ArrayList alMsg = this.ConvertOrderToHL7(order);

                int result = 0;
                foreach (NetScape.AnalysisModel.HL7MSG hl7Msg in alMsg)
                {
                    result = messageSend.Send(SettingHelper.setObj.GEIP, NConvert.ToInt32(SettingHelper.setObj.GEPort), hl7Msg);

                    if (result < 0)
                    {
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "发送到MUSE系统出错！");
                        return;
                    }
                }

                result = plateInterface.ReportCheck(SettingHelper.setObj.PlatURL, SettingHelper.setObj.Certificate, order);

                result = plateInterface.ChargeItem(SettingHelper.setObj.PlatURL, SettingHelper.setObj.Certificate, order, "1");

                if (result <= 0)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "设置收费标志出错！");
                }
                updateCount++;
            }

            if (dtTmp != DateTime.MinValue)
            {
                Common.SettingHelper.SetMaxTimeSetting(dtTmp);
            }

            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "传送至MUSE系统" + updateCount + "条记录");
            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "End 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        public void PEBatchWork()
        {
            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "Start方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);

            DateTime dtMax = Common.SettingHelper.GetMaxTimeSetting();

            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "获取申请项目的最大时间是" + dtMax.ToString());

            DateTime dtTmp = DateTime.MinValue;

            ArrayList al = plateInterface.QueryPEPatientCheckInfo(SettingHelper.setObj.PlatURL, SettingHelper.setObj.Certificate, "3");

            int result = 0;

            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "返回" + al.Count + "条记录！");
            int updateCount = 0;
            OrderItem itemTmp = null;

            foreach (NetScape.AnalysisModel.Order order in al)
            {
                try
                {
                    itemTmp = order.OrderItems[0] as OrderItem;

                    if (dtMax >= itemTmp.OperDate)
                    {
                        continue;
                    }

                    if (updateCount == 0)
                    {
                        dtTmp = itemTmp.OperDate;
                    }

                    if (dtTmp < itemTmp.OperDate)
                    {
                        dtTmp = itemTmp.OperDate;
                    }

                    ArrayList alMsg = this.ConvertOrderToHL7(order);

                    foreach (NetScape.AnalysisModel.HL7MSG hl7Msg in alMsg)
                    {
                        result = messageSend.Send(SettingHelper.setObj.GEIP, NConvert.ToInt32(SettingHelper.setObj.GEPort), hl7Msg);

                        if (result < 0)
                        {
                            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "传送给MUSE接口失败！");
                            return;
                        }
                    }

                    updateCount++;

                    //result = plateInterface.ChargeItem(SettingHelper.setObj.PlatURL, SettingHelper.setObj.Certificate, order);

                    //if (result < 0)
                    //{
                    //    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"设置收费标志失败！");
                    //    continue;
                    //}
                }
                catch (Exception e)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "出现错误！" + e.Message);
                }
            }

            if (dtTmp != DateTime.MinValue)
            {
                Common.SettingHelper.SetMaxTimeSetting(dtTmp);
            }

            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "传送至MUSE系统" + updateCount + "条记录");

            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "End 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
        }


        private ArrayList ConvertOrderToHL7(NetScape.AnalysisModel.Order museOrder)
        {
            ArrayList al = new ArrayList();

            NetScape.AnalysisModel.HL7MSG msg = null;

            foreach (NetScape.AnalysisModel.OrderItem orderItem in museOrder.OrderItems)
            {
                msg = new NetScape.AnalysisModel.HL7MSG();
                msg.IsCancel = false;
                msg.PatientID = museOrder.Patient.PID.CardNO;
                msg.Name = museOrder.Patient.Name;
                msg.Gender = museOrder.Patient.Sex == "1" ? "M" : "F";
                msg.BirthDate = museOrder.Patient.Birthday;
                msg.PatientLocation = museOrder.ReciptDept.Name;
                if (string.IsNullOrEmpty(museOrder.Patient.BedNO))
                {
                    msg.Room = museOrder.ReciptDept.Name;
                }
                else
                {
                    msg.Room = museOrder.ReciptDept.Name + museOrder.Patient.BedNO + "床";
                }
                msg.Bed = museOrder.Patient.BedNO;
                msg.HisOrderType = "";
                msg.OrderDateTime = orderItem.OperDate;// museOrder.ApplyDate;

                if (museOrder.Patient.PatientType == "1")
                {
                    msg.PatientType = "OUTPAT";
                }
                else if (museOrder.Patient.PatientType == "2")
                {
                    msg.PatientType = "INPAT";
                }
                else if (museOrder.Patient.PatientType == "3")
                {
                    msg.PatientType = "PREADT";
                }
                //string OrderNO = item.ApplyNo; //item.ID + "-" + item.OrderID;
                //msg.OrderNumber = item.ApplyNo;
                msg.OrderNumber = orderItem.ApplyNo; //***
                //msg.VisitNumber = this.order.Patient.PID.CardNO;
                //msg.VisitNumber = item.OrderID;
                msg.VisitNumber = museOrder.Patient.PID.ID; //*****//***
                msg.OrderPhys = museOrder.ReciptDoctor.Name;
                msg.OrderTitle = orderItem.Item.Name;
                msg.TestType = "12 Lead ECG";
                msg.TestReason = museOrder.Patient.MainDiagnose;

                al.Add(msg);
            }


            return al;

        }

        public string[] DealResultString(string resultStr)
        {
            string[] result = new string[2];
            int index = resultStr.TrimStart().IndexOf("  ");

            if (index == -1)
            {
                return result;
            }
            result[0] = resultStr.Substring(0, index).Split(':')[1];
            result[1] = resultStr.Substring(index, resultStr.Length - index - 1).Trim().Split(':')[1];

            return result;

        }

        private DateTime ConvertToDate(string dateFormat)
        {
            if (string.IsNullOrEmpty(dateFormat))
            {
                return DateTime.Now;
            }

            int hour = 0;
            int min = 0;
            int sec = 0;

            if (dateFormat.Length > 8)
            {
                hour = NConvert.ToInt32(dateFormat.Substring(8, 2));
                min = NConvert.ToInt32(dateFormat.Substring(10, 2));
                sec = NConvert.ToInt32(dateFormat.Substring(12, 2));
            }

            return new DateTime(NConvert.ToInt32(dateFormat.Substring(0, 4)), NConvert.ToInt32(dateFormat.Substring(4, 2)), NConvert.ToInt32(dateFormat.Substring(6, 2)), hour, min, sec);
        }

        Funcs.Function func = new Funcs.Function();

        /// <summary>
        /// /循环监控多个路径下生成的Hl7消息，为其生成新的pdf报告格式
        /// </summary>
        public void HandleGEPrint()
        {
            try
            {

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "Start 方法名: HandleGEPrint");

                //循环监控多个路径下生成的Hl7消息，为其生成新的pdf报告格式
                foreach (var item in NetScape.AnalysisModel.Profile.ConfigSetting.PrintUrlList)
                {
                    // Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, string.Format("********************监控路径：{0}******************", item.MsgUrl));
                    if (!Directory.Exists(item.MsgUrl))
                    {
                        // Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, string.Format("********************不存在的路径：{0}******************", item.MsgUrl));
                        continue;
                    }
                    string[] rootfiArr = Directory.GetFiles(item.MsgUrl);

                    if (rootfiArr == null || rootfiArr.Length == 0)
                    {
                        //Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, string.Format("*******没有要处理的报告结果*****路径：" + item.MsgUrl));
                        continue;
                    }

                    foreach (string hl7FileName in rootfiArr)
                    {
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "开始进行处理打印文件@" + hl7FileName);
                        try
                        {
                            FileConent fileConent = this.GetResult(hl7FileName);


                            //下载pdf报告到本地的缓存目录
                            string pdfFileName = FileHelper.DownLoadFtp(fileConent.WEBURL);

                            int result = 1;

                            if (string.IsNullOrEmpty(pdfFileName))
                            {
                                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, string.Format("下载报告失败！! hl7 file name: {0}, PDF url: {1}.", hl7FileName, fileConent.WEBURL));
                                result = -1;
                            }

                            if (result > 0)
                            {
                                string newPdfFileName = item.PdfOutUrl + Path.GetFileNameWithoutExtension(hl7FileName) + ".pdf";

                                //重置pdf 格式
                                result = func.ResetPdtFormat(fileConent, pdfFileName, newPdfFileName);

                                //删除下载的临时pdf文件
                                File.Delete(pdfFileName);

                                if(result > 0) Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "处理报告打印格式已完成！! " + newPdfFileName);
                            }

                            if (result > 0)
                            {
                                //成功之后删除目录下的文件
                                File.Delete(hl7FileName);
                            }
                            else
                            {
                                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "处理报告打印格式失败！！" + pdfFileName + " || " + func.ErrMsg);
                                var failedFolder = item.MsgUrl.TrimEnd('\\') + "\\failed\\";
                                if (!Directory.Exists(failedFolder)) Directory.CreateDirectory(failedFolder);
                                var failedFile = failedFolder + Path.GetFileName(hl7FileName);
                                File.Move(hl7FileName, failedFile);
                            }

                        }
                        catch (Exception ex)
                        {
                            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "程序异常：处理报告打印格式失败！！！" + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "程序异常：HandleGEPrint失败！！！" + ex.Message);
                }
                catch { }
            }
        }


        public DateTime GetFileTime(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || fileName.Length < 10)
            {
                return DateTime.MinValue;
            }
            List<string> strs = fileName.Split('_').ToList();
            if (strs.Count < 3)
            {
                return DateTime.MinValue;
            }

            return Funcs.Function.ConvertToDateTime(strs[1] + strs[2]);
        }

        public string GetPdfPrintPath(string id, DateTime dt)
        {
            // string path = string.Empty;
            dt = dt.AddHours(-8);//muse的是格林乔治时间
            string sql = @"select computername from dbo.logTransmit a where a.testid='{0}' and  CONVERT(CHAR(24), a.DateTime_UTC, 120)='{1}'--cast('{1}' as datetime)--convert('{1}','YYYY-MM-DD HH24:MI:SS')";
            sql = string.Format(sql, id, dt.ToString("yyyy-MM-dd HH:mm:ss"));
            var path = Common.SqlHelper.ExecuteScalar(sql);
            if (path != null)
                return path.ToString();
            else
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "查询pdf打印路径错误：" + sql);

            return string.Empty;
        }

        /// <summary>
        /// 循环监控电生理pdf报告路径
        /// </summary>
        public void HandleEctPrint()
        {
            try
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "Start 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                if (!Directory.Exists(NetScape.AnalysisModel.Profile.ConfigSetting.EctPdf.Name))
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, string.Format("********************不存在的路径：{0}******************", NetScape.AnalysisModel.Profile.ConfigSetting.EctPdf.Name));
                    // continue;
                }
                string[] rootfiArr = Directory.GetFiles(NetScape.AnalysisModel.Profile.ConfigSetting.EctPdf.Name);

                if (rootfiArr == null || rootfiArr.Length == 0)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, string.Format("*******没有要处理的报告结果*****路径：" + NetScape.AnalysisModel.Profile.ConfigSetting.EctPdf.Name));
                    // continue;
                }
                //FileConent fileConent = null;
                //缓存路径
                string pdfFileName = string.Empty;
                //pdf报告全路径
                string reportPath = string.Empty;
                //报告名称，不包含路径
                string reportName = string.Empty;

                //fileName 这里指的是监控GE系统打印目录下生成的Hl7消息文件
                foreach (string fileName in rootfiArr)
                {

                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "开始进行处理电生理报告@" + fileName);
                    reportName = System.IO.Path.GetFileNameWithoutExtension(fileName);

                    //pdf报告名称
                    //  pdfFileName = "ECG" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");                 

                    //电生理报告处理流程
                    int result = func.HandleEctReport(reportName, fileName, ref pdfFileName);

                    if (result > 0)
                    {
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "处理电生理报告已完成！! " + reportPath);
                        //之后删除目录下的文件
                        File.Delete(fileName);
                    }
                    else
                    {
                        string _back = NetScape.AnalysisModel.Profile.ConfigSetting.EctPdf.Mark + reportName + ".pdf";
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "处理电生理报告失败！！" + pdfFileName + " |" + _back + "| " + func.ErrMsg);
                        if (File.Exists(_back)) File.Delete(_back);
                        File.Move(fileName, _back);

                    }

                }
            }
            catch (Exception ex)
            {
                try
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "程序异常：处理电生理报告失败！！！" + ex.Message);
                }
                catch { }
            }
        }
    }
}
