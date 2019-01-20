using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Xml.Linq;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Globalization;
using NetScape.AnalysisModel.Profile;
using iTextSharp.text.pdf;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Drawing;
using iTextSharp.text;
using System.Data.OracleClient;

namespace NetScape.AnalysisWork.Funcs
{
    public class Function
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Base.DataAccess dataMgr = new Base.DataAccess();

        public string ErrMsg = string.Empty;

        /// <summary>
        /// 根据实体转换成HL7格式的消息
        /// </summary>
        /// <param name="museOrder"></param>
        /// <returns></returns>
        public ArrayList ConvertOrderToHL7(NetScape.AnalysisModel.Order museOrder)
        {
            ArrayList al = new ArrayList();

            NetScape.AnalysisModel.HL7MSG msg = null;

            foreach (NetScape.AnalysisModel.OrderItem orderItem in museOrder.OrderItems)
            {
                if (!NetScape.AnalysisWork.Common.ConstManager.CheckItemNeedSendMuse(orderItem.Item.ID))
                {
                    continue;
                }
                msg = new NetScape.AnalysisModel.HL7MSG();
                msg.IsCancel = false;
                msg.PatientID = museOrder.Patient.PID.CardNO;
                msg.Name = museOrder.Patient.Name;
                msg.Gender = NetScape.AnalysisWork.Common.ConstManager.GetMuseSexCode(museOrder.Patient.Sex.Trim());// museOrder.Patient.Sex.Trim() == "男" ? "M" : "F";
                msg.BirthDate = museOrder.Patient.Birthday;
                msg.PatientLocation = museOrder.ReciptDept.Name;
                if (string.IsNullOrEmpty(museOrder.Patient.BedNO))
                {
                    msg.Room = museOrder.ReciptDept.Name;
                }
                else
                {
                    if (museOrder.Patient.BedNO.Length == 7)
                        museOrder.Patient.BedNO = museOrder.Patient.BedNO.Substring(4);
                    msg.Room = museOrder.ReciptDept.Name + museOrder.Patient.BedNO + "床";
                }

                msg.Bed = museOrder.Patient.BedNO;
                msg.HisOrderType = orderItem.Item.ID;
                msg.OrderDateTime = museOrder.ApplyTime;// museOrder.ApplyDate;

                //string OrderNO = item.ApplyNo; //item.ID + "-" + item.OrderID;
                //msg.OrderNumber = item.ApplyNo;
                msg.OrderNumber = orderItem.ApplyNo; //***
                //msg.VisitNumber = this.order.Patient.PID.CardNO;
                //msg.VisitNumber = item.OrderID;
                msg.VisitNumber = museOrder.Patient.PID.ID; //*****//***

                if (museOrder.Patient.PatientType == "0" || museOrder.Patient.PatientType == "1")
                {
                    msg.PatientType = "OUTPAT";
                    //msg.VisitNumber = museOrder.Patient.PID.CardNO;
                    //msg.PatientID = museOrder.Patient.PID.HealthNO;
                    msg.PatientID = "9" + msg.PatientID;
                }
                else if (museOrder.Patient.PatientType == "2")
                {
                    msg.PatientType = "INPAT";
                }
                else if (museOrder.Patient.PatientType == "3")
                {
                    msg.PatientType = "PREADT";
                    msg.PatientID = "8" + msg.PatientID;
                    msg.VisitNumber = msg.PatientID;
                }

                msg.OrderPhys = museOrder.ReciptDoctor.Name;
                msg.OrderTitle = orderItem.Item.Name;
                msg.TestType = "12 Lead ECG";
                msg.TestReason = museOrder.Patient.MainDiagnose;

                al.Add(msg);
            }


            return al;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assign"></param>
        /// <returns></returns>
        public NetScape.AnalysisModel.Order ConvertAssiagnTOMuseOrder(Neusoft.HISFC.Models.Nurse.Assign assign)
        {
            NetScape.AnalysisModel.Order order = new NetScape.AnalysisModel.Order();// = new Neusoft.HISFC.Models.Registration.Register();

            order.Patient.ID = assign.Register.ID;//门诊流水号
            order.Patient.PID.CardNO = assign.Register.PID.CardNO;
            order.Patient.PID.ID = assign.Register.InvoiceNO;
            order.Patient.PID.HealthNO = assign.Register.RecipeNO;
            order.Patient.PatientType = assign.Queue.Console.ID;
            //order.Patient.PatientType = assign.Register.InSource.ID;
            //if (assign.Register.InSource.ID.ToString() == "门诊")
            //{
            //    order.Patient.PatientType = assign.Queue.Console.ID;
            //}
            //else if (assign.Register.InSource.ID.ToString() == "住院")
            //{
            //    order.Patient.PatientType = "2";
            //}
            //else
            //{
            //    order.Patient.PatientType = "3";
            //} 
            order.Patient.PID.CaseNO = assign.Register.SSN;
            order.Patient.Name = assign.Register.Name;
            order.Patient.Sex = assign.Register.Sex.ID.ToString();
            order.Patient.Birthday = assign.Register.Birthday;
            order.Patient.IDCard = assign.Register.IDCard;
            order.Patient.Address = assign.Register.CompanyName;
            order.Patient.PhoneNumber = assign.Register.PhoneHome;

            ArrayList al = new ArrayList();
            NetScape.AnalysisModel.OrderItem item = null;
            foreach (string applyNo in assign.Queue.Console.Name.Split(','))
            {
                item = new NetScape.AnalysisModel.OrderItem();
                item.ApplyNo = applyNo;
                al.Add(item);
            }

            order.OrderItems = al;

            return order;
        }

        /// <summary>
        /// 保存申请单数据
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int InsertEcgOrder(NetScape.AnalysisModel.Order order)
        {
            #region sql
            string sql = @"
insert into ECGAPPLYBILL p(
P.CLINICCODE,--1
P.CARDNO,--2
P.PATIENTNO,--3
P.APPLYNO,--4
P.EXAMID,--5
P.CLINCDIAG,--6
P.MAINDIAG,--7
P.RECIPTNO,--8
P.RECIPTDEPT,--9
P.RECIPTDEPTNAME,--10
P.RECIPTDOCT,--11
P.RECIPTDOCTNAME,--12
P.EXECDEPT,--13
P.EXECDEPTNAME,--14
P.ITEMCODE,--15
P.ITEMNAME,--16
P.APPLYSTATE,--17
P.VALIDSTATE,--18
P.SYSCLASS,--19
P.CHECKPARTID,--20
P.CHECKPARTNAME,--21
P.CHECKPARTMEMO,--22
P.EMERGENTFLAG,--23
P.SPECIMENTYPE,--24
P.CHECKTYPE,--25
P.FEESTATE,--26
P.UNIT,--27
P.QTY,--28
P.UNITPRICE,--29
P.TOTCOST,--30
P.CHECKDESC,--31
P.CHECKRESULT,--32
P.REPORTID,--33
P.REPORTTYPE,--34
P.REPORTURL,--35
P.ISBOOK,--36
P.BOOKID,--37
P.OPERCODE,--38
P.OPERDATE,--39
P.MEMO,--40
P.MARK,--41
P.EXT1,--42
P.EXT2,--43
P.EXT3,--44
P.EXT4,--45
P.EXT5--46
)
values(
'{0}',--1
'{1}',--2
'{2}',--3
'{3}',--4
'{4}',--5
'{5}',--6
'{6}',--7
'{7}',--8
'{8}',--9
'{9}',--10
'{10}',--11
'{11}',--12
'{12}',--13
'{13}',--14
'{14}',--15
'{15}',--16
'{16}',--17
'{17}',--18
'{18}',--19
'{19}',--20
'{20}',--21
'{21}',--22
'{22}',--23
'{23}',--24
'{24}',--25
'{25}',--26
'{26}',--27
'{27}',--28
'{28}',--29
'{29}',--30
'{30}',--31
'{31}',--32
'{32}',--33
'{33}',--34
'{34}',--35
'{35}',--36
'{36}',--37
'{37}',--38
{38},--39
'{39}',--40
'{40}',--41
'{41}',--42
'{42}',--43
'{43}',--44
'{44}',--45
'{45}'--46
)";
            #endregion

            if (order == null)
            {
                return -1;
            }

            try
            {
                //if (this.InsertPatientInfo(order.Patient) == -1)
                //{
                //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //    return -1;
                //}

                foreach (NetScape.AnalysisModel.OrderItem item in order.OrderItems)
                {
                    string execStr = string.Format(sql, order.Patient.PID.ID, order.Patient.ID, order.Patient.PID.PatientNO, item.ApplyNo, item.CheckID, order.Patient.ClinicDiagnose,
                        order.Patient.MainDiagnose, order.ReciptNO, order.ReciptDept.ID, order.ReciptDept.Name, order.ReciptDoctor.ID, order.ReciptDoctor.Name, item.ExecDept.ID, item.ExecDept.Name,
                        item.Item.ID, item.Item.Name, order.Status, "1", item.SysClass.Name, item.CheckPart.ID, item.CheckPart.Name, item.CheckPart.Memo, Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsUrgent).ToString(),
                        item.Sample, item.SysClass.ID, order.FeeState, string.Empty, string.Empty, string.Empty, item.TotCost, item.Memo,
                        string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "009999", "sysdate",//39
                          string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty
                          );//46

                    if (dataMgr.ExecNoQuery(execStr) == -1)
                    {
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "保存医嘱明细项目失败失败！" + dataMgr.Err);
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, execStr);
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "保存医嘱失败！" + ex.Message);
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, sql);
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                return -1;
            }
            finally
            {
            }

            return 1;
        }

        /// <summary>
        ///  保存申请单患者信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int InsertPatientInfo(NetScape.AnalysisModel.Order order)
        {
            #region sql
            string sql = @"
INSERT INTO ECGPATIENTINFO P
(P.SERIALNUM,--1
P.PATIENTID,--2
P.CARDNO,--3
P.CLINICCODE,--4
P.IDCARDNO,--5
P.IDENNO,--6
P.NAME,--7
P.SPELLCODE,--8
P.WBCODE,--9
P.SEX,--10
P.BIRTHYDAY,--11
P.HOMETEL,--12
P.MOBILETEL,--13
P.HOMEADRRESS,--14
P.PATIENTTYPE,--15
P.COUNTRY,--16
P.NATION,--17
P.OPERCODE,--18
P.OPERDATE,--19
P.MARK,--20
P.EXT1,--21
P.EXT2,--22
P.EXT3,--23
P.STATE,
P.FEESTATE,
P.APPLYNO
)
VALUES (
{0},--1
'{1}',--2
'{2}',--3
'{3}',--4
'{4}',--5
'{5}',--6
'{6}',--7
'{7}',--8
'{8}',--9
'{9}',--10
TO_DATE('{10}','yyyy-mm-dd hh24:mi:ss'),--11
'{11}',--12
'{12}',--13
'{13}',--14
'{14}',--15
'{15}',--16
'{16}',--17
'{17}',--18
TO_DATE('{18}','yyyy-mm-dd hh24:mi:ss'),--19
'{19}',--20
'{20}',--21
'{21}',--22
'{22}',--23
'{23}',
'{24}',--23
'{25}'
)";
            #endregion

            try
            {
                //NetScape.AnalysisModel.PatientInfo info = order.Patient;
                sql = string.Format(sql, "EcgPatientSequence.Nextval", order.Patient.ID, order.Patient.PID.CardNO, order.Patient.PID.ID, order.Patient.IDCard, order.Patient.PID.IdenNo, order.Patient.Name, string.Empty, string.Empty, order.Patient.Sex, order.Patient.Birthday,//11
                    order.Patient.PhoneNumber, order.Patient.PhoneNumber, order.Patient.Address, order.Patient.PatientType, order.Patient.Nation, order.Patient.Nation, "009999", order.ApplyTime.ToString("yyyy-MM-dd HH:mm:ss"),//19
                    order.Patient.Mark, order.Patient.Ext1, order.Patient.Ext2, order.Patient.Value, order.Status, order.FeeState, order.ID);//24
                Neusoft.FrameWork.Function.HisLog.WriteLog(LogName._sql, sql);
                if (dataMgr.ExecNoQuery(sql) == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "保存患者信息失败！" + dataMgr.Err);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "保存患者信息失败！" + ex.Message);
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, sql);
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                return -1;
            }


            return 1;
        }

        /// <summary>
        /// xml 消息保存的sequence
        /// </summary>
        /// <returns></returns>
        public static int QueryMsgFileSeq()
        {
            string sql = "SELECT EcgMsgFileSequence.Nextval FROM DUAL ";
            string seq = dataMgr.ExecSqlReturnOne(sql);
            return Neusoft.FrameWork.Function.NConvert.ToInt32(seq);
        }

        /// <summary>
        /// 保存xml消息体
        /// </summary>
        /// <param name="order"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public int SaveMsgFile(NetScape.AnalysisModel.Order order, string xml)
        {
            #region sql

            string sql = @"INSERT INTO ECGMSGFILE P(P.SERIALNUM,P.XMLMSG,P.PATIENTID,P.CARDNO,P.APPLYNO,P.LOCALURL,P.XMLFILENAME,P.OPERCODE,P.OPERDATE)VALUES('{0}',{1},'{2}','{3}','{4}','{5}', '{6}','{7}',sysdate)";

            #endregion

            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\MsgFile\\" + DateTime.Now.ToString("yyyyMMdd")+ "\\"; ;
                byte[] content = System.Text.Encoding.Default.GetBytes(xml);

                string fileName = DateTime.Now.ToString("yyyyMMddHHmm") + "-" + order.Patient.PID.ID + ".xml";
                sql = string.Format(sql, order.Patient.Value, ":p", order.Patient.ID, order.Patient.PID.CardNO, order.ID, path + fileName, fileName, "009999");

                if (dataMgr.InputBlob(sql, content) == -1)
                {
                    this.ErrMsg = dataMgr.Err;
                    return -1;
                }

                XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
                XElement root = XElement.Parse(xml);
                doc.Add(root);
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                //save xml file at local dir whit msg
                doc.Save(path + fileName);
            }
            catch (Exception ex)
            {
                this.ErrMsg = ex.Message;
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 查询消息体
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public NetScape.AnalysisModel.EcgMsgFileInfo QueryMsgFile(string seqId)
        {
            #region sql

            string sql = @"SELECT  
P.SERIALNUM,--1
P.PATIENTID,--2
P.CARDNO,--3
P.LOCALURL,--4
P.XMLFILENAME,--5
P.XMLMSG,--6
P.PDFFILENAME,--7
P.PDFURL,--8
P.IMGURL,--9
P.IMGFILENAME,--10
P.IMAGE,--11
P.OPERCODE,--12
P.OPERDATE,--13
P.MEMO,--14
P.MARK,--15
P.EXT1,--16
P.EXT2,--17
P.EXT3--18
FROM ECGMSGFILE P 
WHERE SERIALNUM ='{0}'";

            #endregion

            try
            {
                sql = string.Format(sql, seqId);
                Neusoft.FrameWork.Function.HisLog.WriteLog(LogName._sql, sql);
                if (dataMgr.ExecQuery(sql) == -1)
                {
                    this.ErrMsg = dataMgr.Err;
                    return null;
                }

                NetScape.AnalysisModel.EcgMsgFileInfo info = null;
                while (dataMgr.Reader.Read())
                {
                    info = new AnalysisModel.EcgMsgFileInfo();
                    info.ID = dataMgr.Reader[0].ToString(); /*SERIALNUM[] */
                    info.PatientId = dataMgr.Reader[2].ToString(); /*PATIENTID[] */
                    info.CardNo = dataMgr.Reader[2].ToString(); /*CARDNO[] */
                    info.LocalUrl = dataMgr.Reader[3].ToString(); /*LOCALURL[] */
                    info.XmlFileName = dataMgr.Reader[4].ToString(); /*XMLFILENAME[] */
                    info.XmlMsg = dataMgr.Reader[5].ToString(); /*XMLMSG[] */
                    info.PdfFileName = dataMgr.Reader[6].ToString(); /*PDFFILENAME[] */
                    info.PdfUrl = dataMgr.Reader[7].ToString(); /*PDFURL[] */
                    info.ImgUrl = dataMgr.Reader[8].ToString(); /*IMGURL[] */
                    info.ImgFileName = dataMgr.Reader[9].ToString(); /*IMGFILENAME[] */
                    //=dataMgr.Reader[10].ToString(); /*IMAGE[] */
                    info.OperCode = dataMgr.Reader[11].ToString(); /*OPERCODE[] */
                    info.OperDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(dataMgr.Reader[12].ToString()); /*OPERDATE[] */
                    info.Memo = dataMgr.Reader[13].ToString(); /*MEMO[] */
                    info.Mark = dataMgr.Reader[14].ToString(); /*MARK[] */
                    //=dataMgr.Reader[15].ToString(); /*EXT1[] */
                    //=dataMgr.Reader[16].ToString(); /*EXT2[] */
                    //=dataMgr.Reader[17].ToString(); /*EXT3[] */
                    break;
                }
                if (info != null)
                {
                    string sqlContent = "select p.xmlmsg from  ECGMSGFILE P where p.serialnum='{0}'";
                    byte[] content = dataMgr.OutputBlob(string.Format(sqlContent, info.ID));
                    info.XmlMsg = System.Text.ASCIIEncoding.Default.GetString(content);
                    Neusoft.FrameWork.Function.HisLog.WriteLog(LogName._sql, sqlContent);
                }
                return info;

            }
            catch (Exception ex)
            {
                this.ErrMsg = ex.Message.ToString();
                return null;
            }

        }

        /// <summary>
        /// 报告回写完成后，把 报告地址回写到本地库，更新状态
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int UpdateMsgFileUrl(NetScape.AnalysisModel.EcgMsgFileInfo info)
        {
            #region sql

            string sql = @"update ecgmsgfile a
set a.pdffilename='{0}',
a.pdfurl='{1}',
a.memo='{2}'
where a.serialnum='{3}'";
            sql = string.Format(sql, info.PdfFileName, info.PdfUrl, "4", info.ID);

            if (dataMgr.ExecNoQuery(sql) == -1)
            {
                ErrMsg = "更新pdf路径失败！" + dataMgr.Err;
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, ErrMsg);
                return -1;
            }

            return 1;

            #endregion
        }

        public int InsertFileToPlat(string connStr, string filePath, string sql, string paraName)
        {
            string cnnstr = "provider=OraOLEDB.Oracle;data source=zlkj_kk;User Id=kk;Password=kk;";
            OleDbConnection con = new OleDbConnection(cnnstr);
            try
            {
                con.Open();
            }
            catch
            { }
            OleDbCommand cmd = new OleDbCommand(cnnstr, con);

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = cnnstr;

            string imgPath = @"d:\aa\a.jpg";//图片文件所在路径  
            FileStream file = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
            Byte[] imgByte = new Byte[file.Length];//把图片转成 Byte型 二进制流  
            file.Read(imgByte, 0, imgByte.Length);//把二进制流读入缓冲区  
            file.Close();


            cmd.CommandText = " insert into kk.kkzp ( xh,zp ) values ('17',:zp) ";//正常sql语句插入数据库  

            cmd.Parameters.Add("zp", System.Data.OleDb.OleDbType.Binary, imgByte.Length);
            cmd.Parameters[0].Value = imgByte;

            try
            {
                return cmd.ExecuteNonQuery();
                //MessageBox.Show("插入成功");
            }
            catch (System.Exception ex)
            {
                this.ErrMsg = "回传报告失败！" + ex.Message;
                return -1;
                //MessageBox.Show(e1.Message);
            }
        }

        /// <summary>
        /// 生成更新状态的消息体
        /// </summary>
        /// <param name="state"></param>
        /// <param name="order"></param>
        /// <param name="xmlMsg"></param>
        /// <returns></returns>
        public int GetCheckStateXml(string state, NetScape.AnalysisModel.Order order, ref string xmlMsg)
        {
            try
            {
                string msgFileId = GetMsgFileId(order.Patient.ID);
                if (string.IsNullOrEmpty(msgFileId))
                {
                    this.ErrMsg = "没有找到相应的Xml消息ID";
                    return -1;
                }
                NetScape.AnalysisModel.EcgMsgFileInfo info = QueryMsgFile(msgFileId);
                if (info == null || string.IsNullOrEmpty(info.XmlMsg))
                {
                    this.ErrMsg = "没有找到相应的xml 消息。患者：" + order.Patient.PID.ID;
                    return -1;
                }
                string xml = info.XmlMsg.Replace("RequestInfoResult", "RequestInfoAdd");
                xml = xml.Replace("RequestReceive", "RequestInfoAdd");
                //XElement request = Funcs.Function.CreateRequestHeader(new AnalysisModel.Base.MsgSendHeader("RequestInfoAdd"));

                //request.Add(new XElement("requestBody"));
                XElement root = XElement.Parse(xml);
                List<XElement> list = root.Element("RequestInfoAdd").Element("ExamList").Elements("Exam").ToList();
                // XElement root = new AcceptRejectRule("");
                foreach (XElement exam in list)
                {
                    string applyNo = exam.Element("SheetID").Value.Trim();
                    if (order.OrderItems.Cast<NetScape.AnalysisModel.OrderItem>().Where(x => x.ID == applyNo).Count() > 0)
                    {
                        exam.Element("ExamState").SetValue(state);
                    }
                    else
                    {
                        exam.Remove();
                    }
                }
                //  XElement node = root.Element("responseBody").Element("List");
                root.Element("RequestInfoAdd").Element("RequisitionStatus").SetValue(state);
                //xmlMsg = root.ToString();

                // XElement header = CreateRequestHeader(new AnalysisModel.Base.MsgSendHeader("RequestInfoAdd"));
                // XElement body =new XElement("requestBody");
                //body.Add(node);
                // header.Add(body);
                xmlMsg = root.ToString();

            }
            catch (Exception ex)
            {
                this.ErrMsg = "更新检查状态出错！" + ex.Message;
                //Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,ErrMsg);
                return -1;
            }
            return 1;
        }

        public int GetCheckStateXml(string state, string applyNo, ref string xmlMsg)
        {
            try
            {
                string msgFileId = GetMsgFileIdByApplyNo(applyNo);
                if (string.IsNullOrEmpty(msgFileId))
                {
                    this.ErrMsg = "没有找到相应的Xml消息ID,applyNo:"+applyNo;
                    return -1;
                }
                NetScape.AnalysisModel.EcgMsgFileInfo info = QueryMsgFile(msgFileId);
                if (info == null || string.IsNullOrEmpty(info.XmlMsg))
                {
                    this.ErrMsg = "没有找到相应的xml 消息。申请单号：" + applyNo;
                    return -1;
                }
                string xml = info.XmlMsg.Replace("RequestInfoResult", "RequestInfoAdd");
                xml = xml.Replace("RequestReceive", "RequestInfoAdd");

                if(!xml.StartsWith("<List>"))
                {
                    xml = string.Format("<List>{0}</List>", xml);
                }

                XElement root = XElement.Parse(xml);
                List<XElement> list = root.Element("RequestInfoAdd").Element("ExamList").Elements("Exam").ToList();
                foreach (XElement exam in list)
                {

                    if (applyNo == exam.Element("SheetID").Value.Trim())
                        exam.Element("ExamState").SetValue(state);
                    else
                        exam.Remove();
                }

                root.Element("RequestInfoAdd").Element("RequisitionStatus").SetValue(state);
                xmlMsg = root.ToString();
                return 1;
            }
            catch (Exception ex)
            {
                this.ErrMsg = "更新检查状态出错！" + ex.ToString();
                return -1;
            }
        }

        //public int GetUpdateXml(string state,NetScape.AnalysisModel.EcgMsgFileInfo info,ref string xmlm)

        /// <summary>
        /// 取消息体的sequence
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public string GetMsgFileId(string patientId)
        {
            #region sql

            string sql = @"select max(a.ext3) from ecgpatientinfo a
where a.cliniccode='{0}' or a.patientid='{0}'
and a.state<>'{1}'--状态位 ";

            sql = string.Format(sql, patientId, "09");

            return dataMgr.ExecSqlReturnOne(sql);
            #endregion
        }

        /// <summary>
        /// 取消息体的sequence
        /// 使用申请单号作索引
        /// </summary>
        /// <param name="applyNo"></param>
        /// <returns></returns>
        public string GetMsgFileIdByApplyNo(string applyNo)
        {
            #region sql

            string sql = @"select max(a.ext3) from ecgpatientinfo a where a.applyno='{0}' ";

            #endregion

            sql = string.Format(sql, applyNo);
            Neusoft.FrameWork.Function.HisLog.WriteLog(LogName._sql, sql);
            return dataMgr.ExecSqlReturnOne(sql);

        }

        /// <summary>
        /// 日期格式yyyyMMddHHmmss 转换yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(string time)
        {
            if (time.Length < 8)
                return DateTime.MinValue;
            try
            {
                DateTime dt = DateTime.MinValue;
                switch (time.Length)
                {
                    case 8:
                        dt = DateTime.ParseExact(time, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None);
                        break;
                    case 10:
                        dt = DateTime.ParseExact(time, "yyyyMMddHH", CultureInfo.CurrentCulture, DateTimeStyles.None);
                        break;
                    case 12:
                        dt = DateTime.ParseExact(time, "yyyyMMddHHmm", CultureInfo.CurrentCulture, DateTimeStyles.None);
                        break;
                    case 14:
                        dt = DateTime.ParseExact(time, "yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None);
                        break;
                    default:
                        dt = DateTime.MinValue;
                        break;
                }
                return dt;
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }


        /// <summary>
        /// 创建申请单请求消息头
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public static XElement CreateRequestHeader(AnalysisModel.Base.MsgSendHeader header)
        {
            return new XElement("Request",
                new XElement("requestHeader",
                    new XElement("sender", header.Sender),
                    new XElement("receiver", header.Receiver),
                      new XElement("sendTime", header.SendTime),
                    new XElement("msgType", header.MsgType),
                      new XElement("msgId", header.MsgId),
                    new XElement("msgPriority", header.MsgPriority),
                      new XElement("msgVersion", header.MsgVersion)
                    )
                );
        }

        Common.PaltDatabase paltMgr = new Common.PaltDatabase();

        /// <summary>
        /// 插入患者信息中间表
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        public int InsertPerson(NetScape.AnalysisModel.Order order)
        {
            #region sql
            try
            {
                string sql = @"
INSERT INTO PERSON 
(PERSON_ID,--ID
NAME,--患者姓名
DATE_OF_BIRTH,--3 出生日期 yyyy/MM/dd
GENDER_CD,--sexCode
HOME_ADDRESS,--地址
DATE_CREATED,--6 创建日期 yyyy/MM/dd hh24:mi:ss
HOSPITAL_DOMAIN_ID,--医院ID
IDENTIFIER_DOMAIN_NAME,--机构名称
IDENTIFIER_DOMAIN_ID,--机构域ID
IDENTIFIER_DOMAIN_TYPE,--10 机构类型
IDENTIFIER_ID,--机构内病人ID
CUSTOM4,--患者域ID
UUID, --13 外键，关联PERSONVISIT表
PERSON_STATUS, --状态
IDENTITY_NO,
CUSTOM14,
RELEVANCE_ID--relevance_id
)
VALUES({0},--0
'{1}',--1
to_date('{2}','yyyy-MM-dd'),--2
'{3}',--3
'{4}',--4
to_date('{5}','yyyy-MM-dd hh24:mi:ss'),--5
'{6}',--6
'{7}',--7
'{8}',--8
'{9}',--9
'{10}',--10
'{11}',--11
'{12}',--12
'{13}',
'{14}',
'{15}',
'{16}'
) ";
            #endregion
                
                order.Patient.Memo = this.GerPaltPersonSeq();
                order.Patient.ForeignKey = Guid.NewGuid().ToString("N");

                NetScape.AnalysisModel.Profile.DomainInfo _idenDomain = GetDomainByCode("10");
                NetScape.AnalysisModel.Profile.DomainInfo _PatientDomain = GetPatientDomain(order.Patient.PatientType);
                NetScape.AnalysisModel.Profile.ConfigObj obj = NetScape.AnalysisModel.Profile.ConfigSetting.ConfigItem;
                sql = string.Format(sql, order.Patient.Memo, order.Patient.Name, order.Patient.Birthday.ToString("yyyy/MM/dd"), Common.ConstManager.GetMuseSexCode(order.Patient.Sex), order.Patient.Address,
                    DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                    obj.HosDomain.ID, _idenDomain.Name, _idenDomain.ID, _idenDomain.DomainType, order.Patient.Value/*+order.ID*/,
                    _PatientDomain.ID, order.Patient.ForeignKey, "01"/*add:01;update:02*/, order.Patient.PID.IdenNo, order.Patient.ID,string.Empty/* order.Patient.Value*/);

                Neusoft.FrameWork.Function.HisLog.WriteLog(LogName._sql, sql);
                if (paltMgr.ExecNoQuery(sql) == -1)
                {
                    this.ErrMsg = paltMgr.Err;
                    return -1;

                }
            }
            catch (Exception ex)
            {
                this.ErrMsg = ex.Message;
                return -1;
            }

            return 1;
        }


        public string GerPaltPersonSeq()
        {
            string seqSql = "select  PERSON_SEQUENCE.nextval from dual";
            string seq = string.Empty;
            if (paltMgr.ExecQuery(seqSql) == -1)
            {
                this.ErrMsg = "查找主键失败！" + seqSql;
                return seq;
            }
            else
            {
                while (paltMgr.Reader.Read())
                {
                    seq = paltMgr.Reader[0].ToString();
                    // break;
                }
            }
            return seq;
        }

        /// <summary>
        /// 插入平台患者信息拓展表
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        public int InsertPersonVisit(NetScape.AnalysisModel.PatientInfo patient)
        {
            #region sql

            string sql = @"
INSERT INTO PATIENT_VISIT(
PATIENT_VISIT_ID,
PATIENT_ID,--2 ID
VISIT_FLOW_ID,--3 卡号或者住院号
NAME,--患者姓名
DATE_OF_BIRTH,--出生日期 yyyy/MM/dd
IDENTITY_NO,--6身份证
INSURANCE_NO,--社保号
GENDER_CD,--sexCode
HOME_ADDRESS,--9
HOME_PHONE,

HOSPITAL_DOMAIN_ID,--11 医院ID

IDENTIFIER_DOMAIN_NAME,--机构域名称
IDENTIFIER_DOMAIN_ID,--终端机构域ID 
IDENTIFIER_DOMAIN_TYPE,--机构域类型

IDENTIFIER_FLOW_DOMAIN_ID,--终端 机构域ID 
IDENTIFIER_FLOW_DOMAIN_NAME,--16 终端 机构名称
IDENTIFIER_Flow_DOMAIN_TYPE,--终端机构类型

PAT_CATEGORY,--18 患者类别
PAT_CURRENT_ROOM,
PAT_CURRENT_BED,
PAT_CUURENT_DEP,--21
--REG_DATE,--注册日期 yyyy/MM/dd hh24:mi:ss
--CREATE_DATE, --创建日期 yyyy/MM/dd hh24:mi:ss
OPER_CODE,
OPER_DATE,
UUID,-- 关联PERSON ID 
PATIENT_VISIT_STATUS,--状态
CUSTOM14 ---HIS号（如门诊，住院，体检号）
)
VALUES(PATIENT_VISIT_SEQUENCE.Nextval,--0
'{0}',--1
'{1}',--2
'{2}',--3
to_date('{3}','yyyy-MM-dd'),--4
'{4}',--5
'{5}',--6
'{6}',--7
'{7}',--8
'{8}',--9
'{9}',--10
'{10}',--11
'{11}',--12
'{12}',--13
'{13}',--14
'{14}',--15
'{15}',--16
'{16}',--17
'{17}',--18
'{18}',--19
'{19}',--20
'{20}',--21
timestamp'{21}',--22
'{22}',--23
'{23}',
'{24}'--custom14

)";
            #endregion

            try
            {
                NetScape.AnalysisModel.Profile.DomainInfo idenDomain = GetPatientDomain(patient.PatientType);

                //平台的类别有问题，这里特殊处理一下。修改时候要注意此段
                string rpPatType = patient.PatientType;
                if (rpPatType == "1")
                    rpPatType = "2";
                else if (rpPatType == "2")
                    rpPatType = "1";
                NetScape.AnalysisModel.Profile.DomainInfo _ecgDomain = GetDomainByCode("10");
                NetScape.AnalysisModel.Profile.DomainInfo _flowDomain = GetDomainByCode("11");
                //Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, _flowDomain.Name);
                NetScape.AnalysisModel.Profile.ConfigObj obj = NetScape.AnalysisModel.Profile.ConfigSetting.ConfigItem;
                sql = string.Format(sql,
                    patient.ID, patient.Value, patient.Name, patient.Birthday.ToString("yyyy/MM/dd"), patient.PID.IdenNo,
                    patient.IDCard, Common.ConstManager.GetMuseSexCode(patient.Sex), patient.Address, patient.PhoneNumber,
                    obj.HosDomain.ID,
                    _ecgDomain.Name, _ecgDomain.ID, _ecgDomain.DomainType,
                   _flowDomain.ID, _flowDomain.Name, _flowDomain.DomainType,
                   rpPatType,
                    patient.NurseStation.ID,
                    patient.BedNO,
                    patient.Dept.ID,
                    "",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    patient.ForeignKey,//存外键 与person表关联
                    patient.State,
                    patient.ID
                    );
                Neusoft.FrameWork.Function.HisLog.WriteLog(LogName._sql, sql);
                if (paltMgr.ExecNoQuery(sql) == -1)
                {
                    this.ErrMsg = paltMgr.Err;
                    return -1;

                }
            }
            catch (Exception ex)
            {
                this.ErrMsg = ex.Message;
                return -1;
            }


            return 1;
        }

        public DomainInfo GetPatientDomain(string type)
        {
            DomainInfo dInfo = new DomainInfo();
            if (type == "0" || type == "1")
                dInfo = NetScape.AnalysisModel.Profile.ConfigSetting.DomainInfo.Where(x => x.Code == "3").FirstOrDefault();
            if (type == "2")
                dInfo = NetScape.AnalysisModel.Profile.ConfigSetting.DomainInfo.Where(x => x.Code == "2").FirstOrDefault();
            if (type == "3")
                dInfo = NetScape.AnalysisModel.Profile.ConfigSetting.DomainInfo.Where(x => x.Code == "6").FirstOrDefault();

            return dInfo;
        }

        public DomainInfo GetPatientFlowDomain(string type)
        {
            DomainInfo dInfo = new DomainInfo();
            if (type == "0" || type == "1")
                dInfo = NetScape.AnalysisModel.Profile.ConfigSetting.DomainInfo.Where(x => x.Code == "4").FirstOrDefault();
            if (type == "2")
                dInfo = NetScape.AnalysisModel.Profile.ConfigSetting.DomainInfo.Where(x => x.Code == "5").FirstOrDefault();
            if (type == "3")
                dInfo = NetScape.AnalysisModel.Profile.ConfigSetting.DomainInfo.Where(x => x.Code == "7").FirstOrDefault();

            return dInfo;
        }

        public DomainInfo GetDomainByCode(string code)
        {
            DomainInfo dInfo = NetScape.AnalysisModel.Profile.ConfigSetting.DomainInfo.Where(x => x.Code == code).FirstOrDefault();
            return dInfo;
        }
        public int InsertReport(NetScape.AnalysisModel.EcgMsgFileInfo info, NetScape.AnalysisModel.PatientInfo p, byte[] content)
        {
            #region sql

            string sql = @"

INSERT INTO  DGATE_DOCUMENT_INFO P
(P.PK,--0
P.DOCUMENT_UNIQUE_ID,--1
P.DOCUMENT_DOMAIN_ID,--2
P.PATIENT_ID,--3
P.PATIENT_DOMAIN_ID,--4
P.FILE_TYPE,--5
P.PAY_LOAD_TYPE,--6
P.SUB_TYPE,--7
P.START_TIME,--8
P.REQUEST_NUMBER,--9
P.REQUEST_DOMAIN,--10
P.ORDER_NUMBER,--11
P.ORDER_DOMAIN,--12
P.PATIENT_TYPE,--13
P.HIUP_STATUS,--14
P.END_TIME,--15
P.TPOS_PATH,--tpos_path,--16
P.PAT_CATEGORY,--17
P.PAT_CATEGORY_SYSTEM,--18
P.PAT_NAME,--19
P.FILE_SYSTEM_FK,--20
P.BEFORE_STATUS,--21
effective_time
)
VALUES
('{0}',--0
'{1}',--1
'{2}',--2
'{3}',--3
'{4}',--4
'{5}',--5
'{6}',--6
'{7}',--7
{8},--8
'{9}',--9
'{10}',--10
'{11}',--11
'{12}',--12
'{13}',--13
'{14}',--14
timestamp'{15}',--15
'{16}',--16
'{17}',--17
'{18}',--18
'{19}',--19
'{20}',--20
'{21}',--21 BEFORE_STATUS
timestamp'{22}'--effective_time
)
";
            #endregion



            try
            {
                string PAT_CATEGORY = p.PatientType;
                if (PAT_CATEGORY == "2")
                {
                    PAT_CATEGORY = "1";
                }
                else if (PAT_CATEGORY == "1")
                {
                    PAT_CATEGORY = "2";
                }
                NetScape.AnalysisModel.Profile.DomainInfo sysDomain = this.GetDomainByCode("10");
                NetScape.AnalysisModel.Profile.DomainInfo patientDomain = GetPatientDomain(p.PatientType);
                info.PdfUrl = info.PdfUrl.Substring(1);
                while (info.PdfUrl.Contains("//"))
                {
                    info.PdfUrl = info.PdfUrl.Replace("//", "/");
                }

                sql = string.Format(sql, info.P_Key, //***0****/
info.ID,/*1*/
sysDomain.ID,
p.ID,
patientDomain.ID,
info.FileType,
info.Loads,
info.Ext3,
"sysdate"/**8**/,
info.ApplyNo,
sysDomain.ID,
p.ID,/**11**/
patientDomain.ID,
p.PatientType,
0,
DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),/****15***/
info.PdfUrl,
p.PatientType,
                    "2.16.840.1.113883.4.487.2.1.1.1.13",/*patientDomain.ID*//****18***/
p.Name,
3,/****20 file system key***/
info.ReportType,
DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                Neusoft.FrameWork.Function.HisLog.WriteLog(LogName._sql, sql);
                if (paltMgr.ExecNoQuery(sql) == -1)
                {
                    this.ErrMsg = paltMgr.Err;
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, paltMgr.Err + "\r\n" + sql);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, paltMgr.Err + ex.Message + ex.StackTrace);
                this.ErrMsg = ex.Message;
                return -1;
            }
            return 1;
        }


        public int InsertReportXml(NetScape.AnalysisModel.EcgMsgFileInfo info, NetScape.AnalysisModel.PatientInfo p, byte[] content)
        {
            #region sql

            string sql = @"

INSERT INTO  DGATE_DOCUMENT_INFO P
(P.PK,--0
P.DOCUMENT_UNIQUE_ID,--1
P.DOCUMENT_DOMAIN_ID,--2
P.PATIENT_ID,--3
P.PATIENT_DOMAIN_ID,--4
P.FILE_TYPE,--5
P.PAY_LOAD_TYPE,--6
P.SUB_TYPE,--7
P.START_TIME,--8
P.REQUEST_NUMBER,--9
P.REQUEST_DOMAIN,--10
P.ORDER_NUMBER,--11
P.ORDER_DOMAIN,--12
P.PATIENT_TYPE,--13
P.HIUP_STATUS,--14
P.END_TIME,--15
P.TPOS_PATH,--tpos_path,--16
P.PAT_CATEGORY,--17
P.PAT_CATEGORY_SYSTEM,--18
P.PAT_NAME,--19
P.FILE_SYSTEM_FK,--20
P.BEFORE_STATUS,--21
effective_time
)
VALUES
('{0}',--0
'{1}',--1
'{2}',--2
'{3}',--3
'{4}',--4
'{5}',--5
'{6}',--6
'{7}',--7
{8},--8
'{9}',--9
'{10}',--10
'{11}',--11
'{12}',--12
'{13}',--13
'{14}',--14
timestamp'{15}',--15
'{16}',--16
'{17}',--17
'{18}',--18
'{19}',--19
'{20}',--20
'{21}',--21 BEFORE_STATUS
timestamp'{22}'--effective_time
)
";
            #endregion



            try
            {
                string PAT_CATEGORY = p.PatientType;
                if (PAT_CATEGORY == "2")
                {
                    PAT_CATEGORY = "1";
                }
                else if (PAT_CATEGORY == "1")
                {
                    PAT_CATEGORY = "2";
                }
                NetScape.AnalysisModel.Profile.DomainInfo sysDomain = this.GetDomainByCode("10");
                NetScape.AnalysisModel.Profile.DomainInfo patientDomain = GetPatientDomain(p.PatientType);
                info.XmlReportUrl = info.XmlReportUrl.Substring(1);
                while (info.XmlReportUrl.Contains("//"))
                {
                    info.XmlReportUrl = info.XmlReportUrl.Replace("//", "/");
                }

                sql = string.Format(sql, info.P_Key, //***0****/
                        info.ID,/*1*/
                        sysDomain.ID,
                        p.ID,
                        patientDomain.ID,
                        "TRANS-PATH-XML", // info.FileType,
                        info.Loads,
                        info.Ext3,
                        "sysdate"/**8**/,
                        info.ApplyNo,
                        sysDomain.ID,
                        p.ID,/**11**/
                        patientDomain.ID,
                        p.PatientType,
                        0,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),/****15***/
                        info.XmlReportUrl,
                        p.PatientType,
                        "2.16.840.1.113883.4.487.2.1.1.1.13",/*patientDomain.ID*//****18***/
                        p.Name,
                        3,/****20 file system key***/
                        0,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                );

                Neusoft.FrameWork.Function.HisLog.WriteLog(LogName._sql, sql);
                if (paltMgr.ExecNoQuery(sql) == -1)
                {
                    this.ErrMsg = paltMgr.Err;
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, paltMgr.Err + "\r\n" + sql);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, paltMgr.Err + ex.Message + ex.StackTrace);
                this.ErrMsg = ex.Message;
                return -1;
            }
            return 1;
        }

        // 
        /// <summary>
        /// 平台报告信息表的Sequence
        /// </summary>
        /// <returns></returns>
        public string GetReportSeq()
        {
            string seqSql = "select DGATE_DOCUMENT_INFO_SEQUENCE.NextVal from dual";
            string seq = string.Empty;
            if (paltMgr.ExecQuery(seqSql) == -1)
            {
                this.ErrMsg = "查找主键失败！" + seqSql;
                return seq;
            }
            else
            {
                while (paltMgr.Reader.Read())
                {
                    seq = paltMgr.Reader[0].ToString();
                    // break;
                }
            }
            return seq;
        }

        public string GetReportDocId()
        {

            return "ECGD" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        }

        public int InsertReportExtend(NetScape.AnalysisModel.EcgMsgFileInfo info, NetScape.AnalysisModel.PatientInfo p)
        {
            #region sql

            string sql = @"INSERT INTO DGATE_EXTEND_ID_INFO(
PK,
DOCUMENT_FK,
ID,
DOMAIN_ID
)
VALUES(DGATE_EXTEND_ID_INFO_SEQUENCE.NextVal,
'{0}',--0
'{1}',--1
'{2}' --2
)";

            #endregion

            try
            {
                string execSql = string.Empty;
                int result = 0;

                #region 文档编号

                execSql = string.Format(sql, info.P_Key, info.ID, GetDomainByCode("10").ID);
                result = paltMgr.ExecNoQuery(execSql);
                Neusoft.FrameWork.Function.HisLog.WriteLog(LogName._sql, execSql);
                if (result == -1)
                {
                    this.ErrMsg = "插入报告文档拓展表失败！" + paltMgr.Err + "&&文档编号+主键:" + info.ID + "||" + info.P_Key + execSql;
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, ErrMsg);
                    return result;
                }

                #endregion

                #region 患者号

                execSql = string.Format(sql, info.P_Key, p.ID, GetPatientDomain(p.PatientType).ID);
                result = paltMgr.ExecNoQuery(execSql);
                Neusoft.FrameWork.Function.HisLog.WriteLog(LogName._sql, execSql);
                if (result == -1)
                {
                    this.ErrMsg = "插入报告文档拓展表失败！" + paltMgr.Err + "&&PatientId+主键:" + p.ID + "||" + info.P_Key;
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, ErrMsg);
                    return result;
                }

                #endregion

                #region 患者流水号
                if (!string.IsNullOrEmpty(p.PID.ID))
                {
                    execSql = string.Format(sql, info.P_Key, p.PID.ID, GetPatientFlowDomain(p.PatientType).ID);
                    result = paltMgr.ExecNoQuery(execSql);
                    Neusoft.FrameWork.Function.HisLog.WriteLog(LogName._sql, execSql);
                    if (result == -1)
                    {
                        this.ErrMsg = "插入报告文档拓展表失败！" + paltMgr.Err + "&&ClinicCode+主键:" + p.PID.ID + "||" + info.P_Key;
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, ErrMsg);
                        return result;
                    }
                }

                #endregion

                return result;
            }
            catch (Exception ex)
            {
                this.ErrMsg = "插入报告文档拓展表失败！错误：" + ex.Message;
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, ErrMsg);
                return -1;
            }

        }


        /// <summary>
        /// 从数据库中查找检查项目--住院用，门诊直接调用平台接口
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public NetScape.AnalysisModel.Order GetEcgOrderByPatientId(string patientId)
        {

            NetScape.AnalysisModel.PatientInfo patient = QueryPatientInfo(patientId);
            if (patient == null)
            {
                return null;
            }


            #region sql

            string sql = @"select 
P.CLINICCODE,--1
P.CARDNO,--2
P.PATIENTNO,--3
P.APPLYNO,--4
P.EXAMID,--5
P.CLINCDIAG,--6
P.MAINDIAG,--7
P.RECIPTNO,--8
P.RECIPTDEPT,--9
P.RECIPTDEPTNAME,--10
P.RECIPTDOCT,--11
P.RECIPTDOCTNAME,--12
P.EXECDEPT,--13
P.EXECDEPTNAME,--14
P.ITEMCODE,--15
P.ITEMNAME,--16
P.APPLYSTATE,--17
P.VALIDSTATE,--18
P.SYSCLASS,--19
P.CHECKPARTID,--20
P.CHECKPARTNAME,--21
P.CHECKPARTMEMO,--22
P.EMERGENTFLAG,--23
P.SPECIMENTYPE,--24
P.CHECKTYPE,--25
P.FEESTATE,--26
P.UNIT,--27
P.QTY,--28
P.UNITPRICE,--29
P.TOTCOST,--30
P.CHECKDESC,--31
P.CHECKRESULT,--32
P.REPORTID,--33
P.REPORTTYPE,--34
P.REPORTURL,--35
P.ISBOOK,--36
P.BOOKID,--37
P.OPERCODE,--38
P.OPERDATE,--39
P.MEMO,--40
P.MARK,--41
P.EXT1,--42
P.EXT2,--43
P.EXT3,--44
P.EXT4,--45
P.EXT5--46
FROM ECGAPPLYBILL P
WHERE ( P.CLINICCODE='{0}' or P.CARDNO='{0}')
AND P.APPLYSTATE IN ('0','01','02','03')
AND P.VALIDSTATE='1'
AND P.OPERDATE >SYSDATE -30 ";

            #endregion

            sql = string.Format(sql, patient.ID);


            NetScape.AnalysisModel.Order order = QueryEcgOrder(sql);
            if (order == null || order.OrderItems.Count == 0)
            {

                return null;
            }
            order.Patient = patient;
            return order;
        }

        /// <summary>
        /// 根据申请单号来查找申请单
        /// </summary>
        /// <param name="applyNo"></param>
        /// <returns></returns>
        public NetScape.AnalysisModel.Order QueryOrderByApplyNo(string applyNo)
        {
            string queryOrderByApplyNo = @"SELECT DISTINCT P.SERIALNUM,--1
            P.PATIENTID,--2
            P.CARDNO,--3
            P.CLINICCODE,--4
            P.IDCARDNO,--5
            P.IDENNO,--6
            P.NAME,--7
            P.SPELLCODE,--8
            P.WBCODE,--9
            P.SEX,--10
            P.BIRTHYDAY,--11
            P.HOMETEL,--12
            P.MOBILETEL,--13
            P.HOMEADRRESS,--14
            P.PATIENTTYPE,--15
            P.COUNTRY,--16
            P.NATION,--17
            P.OPERCODE,--18
            P.OPERDATE,--19
            P.MARK,--20
            P.EXT1,--21
            P.EXT2,--22
            P.EXT3,--23
            P.EXT4,--24
            P.STATE, --25 检查状态
            P.FEESTATE,--26 费用状态
            P.APPLYNO --27 申请单号
            FROM ECGPATIENTINFO P
            WHERE P.APPLYNO='{0}'";
            string sql = string.Format(queryOrderByApplyNo, applyNo);
            return QueryOrderByApplyNoBase(sql);
        }

        public NetScape.AnalysisModel.Order QueryOrderByApplyNoBase(string sql)
        {

            NetScape.AnalysisModel.Order order = null;
            NetScape.AnalysisModel.PatientInfo patient = null;
            try
            {
                //  sql = string.Format(sql, applyNo);

                if (dataMgr.ExecQuery(sql) == -1)
                {
                    return null;
                }
                while (dataMgr.Reader.Read())
                {
                    order = new AnalysisModel.Order();
                    patient = new NetScape.AnalysisModel.PatientInfo();
                    patient.Value = dataMgr.Reader[0].ToString(); /*SERIALNUM[] */
                    patient.ID = dataMgr.Reader[2].ToString(); /*PATIENTID[] */
                    patient.PID.CardNO = dataMgr.Reader[2].ToString(); /*CARDNO[] */
                    // patient.PID.PatientNO = dataMgr.Reader[2].ToString(); /*CARDNO[] */
                    patient.PID.ID = dataMgr.Reader[3].ToString(); /*CLINICCODE[] */
                    patient.PID.HealthNO = dataMgr.Reader[4].ToString(); /*IDCARDNO[] */
                    patient.PID.IdenNo = dataMgr.Reader[5].ToString(); /*IDENNO[] */
                    patient.Name = dataMgr.Reader[6].ToString(); /*NAME[] */
                    //=dataMgr.Reader[7].ToString(); /*SPELLCODE[] */
                    //=dataMgr.Reader[8].ToString(); /*WBCODE[] */
                    patient.Sex = dataMgr.Reader[9].ToString(); /*SEX[] */
                    patient.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(dataMgr.Reader[10].ToString()); /*BIRTHYDAY[] */
                    patient.PhoneNumber = dataMgr.Reader[11].ToString(); /*HOMETEL[] */
                    patient.PhoneNumber = dataMgr.Reader[12].ToString(); /*MOBILETEL[] */
                    patient.Address = dataMgr.Reader[13].ToString(); /*HOMEADRRESS[] */
                    patient.PatientType = dataMgr.Reader[14].ToString(); /*PATIENTTYPE[] */
                    patient.Nation = dataMgr.Reader[15].ToString(); /*COUNTRY[] */
                    patient.Nation = dataMgr.Reader[16].ToString(); /*NATION[] */
                    //=dataMgr.Reader[17].ToString(); /*OPERCODE[] */
                    order.ApplyTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(dataMgr.Reader[18].ToString()); /*OPERDATE[] */
                    patient.Mark = dataMgr.Reader[19].ToString(); /*MARK[] */
                    patient.Ext1 = dataMgr.Reader[20].ToString(); /*EXT1[] */
                    patient.Ext2 = dataMgr.Reader[21].ToString(); /*EXT2[] */
                    patient.Ext3 = dataMgr.Reader[22].ToString(); /*EXT3[存放消息文本表（ECGMSGFILE）的主键列] */
                    //=dataMgr.Reader[23].ToString(); /*EXT4[] */
                    order.Status = dataMgr.Reader[24].ToString(); /*STATE[检查状态位(1未报到; 2登记（已报到）; 3 已检查; 4报告发布;5退检)] */
                    order.FeeState = dataMgr.Reader[25].ToString(); /*FEESTATE[费用状态(1未收费;2已收费;1100已申请退费;1101退费完成)] */
                    order.ID = dataMgr.Reader[26].ToString(); /*APPLYNO[申请单，键值，检查闭环的根据。] */
                    order.Patient = patient;
                    break;
                }
            }
            catch (Exception ex)
            {
                this.ErrMsg = "查询申请单信息出错！" + ex.Message;
                return null;
            }

            return order;
        }



        /// <summary>
        /// 查询  检查项目
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public NetScape.AnalysisModel.Order QueryEcgOrder(string sql)
        {
            Base.DataAccess assignMgr = new Base.DataAccess();
            if (assignMgr.ExecQuery(sql) == -1)
            {
                return null;
            }
            List<NetScape.AnalysisModel.OrderItem> list = new List<NetScape.AnalysisModel.OrderItem>();
            NetScape.AnalysisModel.Order order = new NetScape.AnalysisModel.Order();
            while (assignMgr.Reader.Read())
            {
                NetScape.AnalysisModel.OrderItem item = new NetScape.AnalysisModel.OrderItem();
                //item. =assignMgr.Reader[0].ToString(); /*CLINICCODE[] */
                //=assignMgr.Reader[1].ToString(); /*CARDNO[] */
                //=assignMgr.Reader[2].ToString(); /*PATIENTNO[] */
                item.ApplyNo = assignMgr.Reader[3].ToString(); /*APPLYNO[] */
                item.CheckID = assignMgr.Reader[4].ToString(); /*EXAMID[] */
                if (list.Where(x => x.CheckID == item.CheckID).Count() > 0)
                    continue;
                //=assignMgr.Reader[5].ToString(); /*CLINCDIAG[] */
                //=assignMgr.Reader[6].ToString(); /*MAINDIAG[] */
                order.ReciptNO = assignMgr.Reader[7].ToString(); /*RECIPTNO[] */
                item.ApplyDept.ID = assignMgr.Reader[8].ToString(); /*RECIPTDEPT[] */
                item.ApplyDept.Name = assignMgr.Reader[9].ToString(); /*RECIPTDEPTNAME[] */
                if (order.ReciptDept == null || string.IsNullOrEmpty(order.ReciptDept.ID))
                {
                    order.ReciptDept = item.ApplyDept;
                    order.ReciptDoctor = item.ApplyOper;
                }
                item.ApplyOper.ID = assignMgr.Reader[10].ToString(); /*RECIPTDOCT[] */
                item.ApplyOper.Name = assignMgr.Reader[11].ToString(); /*RECIPTDOCTNAME[] */
                item.ExecDept.ID = assignMgr.Reader[12].ToString(); /*EXECDEPT[] */
                item.ExecDept.Name = assignMgr.Reader[13].ToString(); /*EXECDEPTNAME[] */
                item.Item.ID = assignMgr.Reader[14].ToString(); /*ITEMCODE[] */
                item.Item.Name = assignMgr.Reader[15].ToString(); /*ITEMNAME[] */
                //                =assignMgr.Reader[16].ToString(); /*APPLYSTATE[] */
                //=assignMgr.Reader[17].ToString(); /*VALIDSTATE[] */
                item.SysClass.Name = assignMgr.Reader[18].ToString(); /*SYSCLASS[] */
                item.CheckPart.ID = assignMgr.Reader[19].ToString(); /*CHECKPARTID[] */
                item.CheckPart.Name = assignMgr.Reader[20].ToString(); /*CHECKPARTNAME[] */
                item.CheckPart.Memo = assignMgr.Reader[21].ToString(); /*CHECKPARTMEMO[] */
                order.IsUrgent = Neusoft.FrameWork.Function.NConvert.ToBoolean(assignMgr.Reader[22].ToString()); /*EMERGENTFLAG[] */
                item.Sample = assignMgr.Reader[23].ToString(); /*SPECIMENTYPE[] */
                //=assignMgr.Reader[24].ToString(); /*CHECKTYPE[] */
                //=assignMgr.Reader[25].ToString(); /*FEESTATE[] */
                item.Unit = assignMgr.Reader[26].ToString(); /*UNIT[] */
                item.Qty = NetScape.AnalysisToolKit.NConvert.ToInt32(assignMgr.Reader[27].ToString()); /*QTY[] */
                item.UnitPrice = NetScape.AnalysisToolKit.NConvert.ToDecimal(assignMgr.Reader[28].ToString()); /*UNITPRICE[] */
                item.TotCost = NetScape.AnalysisToolKit.NConvert.ToDecimal(assignMgr.Reader[29].ToString()); /*TOTCOST[] */
                //=assignMgr.Reader[30].ToString(); /*CHECKDESC[] */
                //=assignMgr.Reader[31].ToString(); /*CHECKRESULT[] */
                //=assignMgr.Reader[32].ToString(); /*REPORTID[] */
                //=assignMgr.Reader[33].ToString(); /*REPORTTYPE[] */
                //=assignMgr.Reader[34].ToString(); /*REPORTURL[] */
                //=assignMgr.Reader[35].ToString(); /*ISBOOK[] */
                //=assignMgr.Reader[36].ToString(); /*BOOKID[] */
                //=assignMgr.Reader[37].ToString(); /*OPERCODE[] */
                //=assignMgr.Reader[38].ToString(); /*OPERDATE[] */
                //=assignMgr.Reader[39].ToString(); /*MEMO[] */
                //=assignMgr.Reader[40].ToString(); /*MARK[] */
                //=assignMgr.Reader[41].ToString(); /*EXT1[] */
                //=assignMgr.Reader[42].ToString(); /*EXT2[] */
                //=assignMgr.Reader[43].ToString(); /*EXT3[] */
                //=assignMgr.Reader[44].ToString(); /*EXT4[] */
                //=assignMgr.Reader[45].ToString(); /*EXT5[] */

                list.Add(item);
            }
            if (list == null || list.Count == 0)
                return null;
            order.OrderItems = new ArrayList(list);
            return order;
        }

        /// <summary>
        /// 查询患者信息
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public NetScape.AnalysisModel.PatientInfo QueryPatientInfo(string patientId)
        {
            // NetScape.AnalysisModel.PatientInfo patient = null;
            //   Base.DataAccess mgr = new Base.DataAccess();
            #region sql

            string sql = string.Empty;
            sql = string.Format(QueryPatientInfoSql, patientId, "'0','1','2','3','4','5'");
            #endregion


            return QueryPatientInfoBase(sql);
        }

        public NetScape.AnalysisModel.PatientInfo QueryInPatientInfo(string patientId, string type, string state)
        {

            #region sql
            string sql = @" 
WHERE (P.PATIENTID='{0}'or P.CLINICCODE='{0}')
AND P.STATE IN ({1})
AND P.PATIENTTYPE ='{2}'
/* AND P.OPERDATE >SYSDATE -10 */ 
ORDER BY P.OPERDATE DESC ";
            //string sql = string.Empty;
            sql = string.Format(sql, patientId, state, type);
            sql = ConstSql.PatientBaseSql + sql;
            #endregion


            return QueryPatientInfoBase(sql);
        }

        /// <summary>
        /// 查询电生理申请单信息
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public AnalysisModel.Order QueryRegisterConfirmedOrder(string applyno)
        {
            #region sql
            string sql = @"
SELECT DISTINCT P.SERIALNUM,--1
P.PATIENTID,--2
P.CARDNO,--3
P.CLINICCODE,--4
P.IDCARDNO,--5
P.IDENNO,--6
P.NAME,--7
P.SPELLCODE,--8
P.WBCODE,--9
P.SEX,--10
P.BIRTHYDAY,--11
P.HOMETEL,--12
P.MOBILETEL,--13
P.HOMEADRRESS,--14
P.PATIENTTYPE,--15
P.COUNTRY,--16
P.NATION,--17
P.OPERCODE,--18
P.OPERDATE,--19
P.MARK,--20
P.EXT1,--21
P.EXT2,--22
P.EXT3,--23
P.EXT4,--24
P.STATE, --25 检查状态
P.FEESTATE,--26 费用状态
P.APPLYNO --27 申请单号

FROM ECGPATIENTINFO P 
WHERE P.APPLYNO='{0}'
ORDER BY P.EXT3 DESC ";
            sql = string.Format(sql, applyno);
            #endregion

            NetScape.AnalysisModel.Order order = null;
            NetScape.AnalysisModel.PatientInfo patient = null;
            try
            {
                //  sql = string.Format(sql, applyNo);

                if (dataMgr.ExecQuery(sql) == -1)
                {
                    return null;
                }
                while (dataMgr.Reader.Read())
                {
                    order = new AnalysisModel.Order();
                    patient = new NetScape.AnalysisModel.PatientInfo();
                    patient.Value = dataMgr.Reader[0].ToString(); /*SERIALNUM[] */
                    patient.ID = dataMgr.Reader[2].ToString(); /*PATIENTID[] */
                    patient.PID.CardNO = dataMgr.Reader[2].ToString(); /*CARDNO[] */
                    // patient.PID.PatientNO = dataMgr.Reader[2].ToString(); /*CARDNO[] */
                    patient.PID.ID = dataMgr.Reader[3].ToString(); /*CLINICCODE[] */
                    patient.PID.HealthNO = dataMgr.Reader[4].ToString(); /*IDCARDNO[] */
                    patient.PID.IdenNo = dataMgr.Reader[5].ToString(); /*IDENNO[] */
                    patient.Name = dataMgr.Reader[6].ToString(); /*NAME[] */
                    //=dataMgr.Reader[7].ToString(); /*SPELLCODE[] */
                    //=dataMgr.Reader[8].ToString(); /*WBCODE[] */
                    patient.Sex = dataMgr.Reader[9].ToString(); /*SEX[] */
                    patient.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(dataMgr.Reader[10].ToString()); /*BIRTHYDAY[] */
                    patient.PhoneNumber = dataMgr.Reader[11].ToString(); /*HOMETEL[] */
                    patient.PhoneNumber = dataMgr.Reader[12].ToString(); /*MOBILETEL[] */
                    patient.Address = dataMgr.Reader[13].ToString(); /*HOMEADRRESS[] */
                    patient.PatientType = dataMgr.Reader[14].ToString(); /*PATIENTTYPE[] */
                    patient.Nation = dataMgr.Reader[15].ToString(); /*COUNTRY[] */
                    patient.Nation = dataMgr.Reader[16].ToString(); /*NATION[] */
                    //=dataMgr.Reader[17].ToString(); /*OPERCODE[] */
                    //=dataMgr.Reader[18].ToString(); /*OPERDATE[] */
                    patient.Mark = dataMgr.Reader[19].ToString(); /*MARK[] */
                    patient.Ext1 = dataMgr.Reader[20].ToString(); /*EXT1[] */
                    patient.Ext2 = dataMgr.Reader[21].ToString(); /*EXT2[] */
                    patient.Ext3 = dataMgr.Reader[22].ToString(); /*EXT3[存放消息文本表（ECGMSGFILE）的主键列] */
                    //=dataMgr.Reader[23].ToString(); /*EXT4[] */
                    order.Status = dataMgr.Reader[24].ToString(); /*STATE[检查状态位(1未报到; 2登记（已报到）; 3 已检查; 4报告发布;5退检)] */
                    order.FeeState = dataMgr.Reader[25].ToString(); /*FEESTATE[费用状态(1未收费;2已收费;1100已申请退费;1101退费完成)] */
                    order.ID = dataMgr.Reader[26].ToString(); /*APPLYNO[申请单，键值，检查闭环的根据。] */
                    order.Patient = patient;
                    break;
                }
            }
            catch (Exception ex)
            {
                string errMsg = string.Format("查询申请单信息出错！applyno[{0}], error:{1}." , applyno, ex.Message);
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, errMsg);
                return null;
            }

            return order;
        }

        public NetScape.AnalysisModel.PatientInfo QueryPatientInfo(string patientId, string state)
        {
            // NetScape.AnalysisModel.PatientInfo patient = null;
            //  Base.DataAccess mgr = new Base.DataAccess();
            #region sql

            string sql = string.Empty;
            sql = string.Format(QueryPatientInfoSql, patientId, state);

            #endregion


            return QueryPatientInfoBase(sql);
        }


        /// <summary>
        /// 通过门诊流水号或者住院流水号查找
        /// </summary>
        /// <param name="clinicCode">门诊流水号或者住院流水号</param>
        /// <returns></returns>
        public NetScape.AnalysisModel.PatientInfo QueryPatientInfoByClinicCode(string clinicCode)
        {
            #region sql

            string sql = @"   SELECT DISTINCT P.SERIALNUM,--1
P.PATIENTID,--2
P.CARDNO,--3
P.CLINICCODE,--4
P.IDCARDNO,--5
P.IDENNO,--6
P.NAME,--7
P.SPELLCODE,--8
P.WBCODE,--9
P.SEX,--10
P.BIRTHYDAY,--11
P.HOMETEL,--12
P.MOBILETEL,--13
P.HOMEADRRESS,--14
P.PATIENTTYPE,--15
P.COUNTRY,--16
P.STATE, --17
P.NATION,--17
P.OPERCODE,--18
P.OPERDATE,--19
P.MARK,--20
P.EXT1,--21
P.EXT2,--22
P.EXT3,--23
P.EXT4,--24
P.FEESTATE,--25
P.APPLYNO

FROM ECGPATIENTINFO P 
WHERE (P.PATIENTID='{0}'or P.CLINICCODE='{0})
/* AND P.OPERDATE >SYSDATE -10 */ 
ORDER BY P.OPERDATE DESC ";

            #endregion

            sql = string.Format(sql, clinicCode);

            return QueryPatientInfoBase(sql);

        }

        #region QueryPatientInfoSql

        string QueryPatientInfoSql = @"
SELECT DISTINCT P.SERIALNUM,--1
P.PATIENTID,--2
P.CARDNO,--3
P.CLINICCODE,--4
P.IDCARDNO,--5
P.IDENNO,--6
P.NAME,--7
P.SPELLCODE,--8
P.WBCODE,--9
P.SEX,--10
P.BIRTHYDAY,--11
P.HOMETEL,--12
P.MOBILETEL,--13
P.HOMEADRRESS,--14
P.PATIENTTYPE,--15
P.COUNTRY,--16
P.STATE, --17
P.NATION,--17
P.OPERCODE,--18
P.OPERDATE,--19
P.MARK,--20
P.EXT1,--21
P.EXT2,--22
P.EXT3,--23
P.EXT4,--24
P.FEESTATE,--25
P.APPLYNO

FROM ECGPATIENTINFO P 
WHERE (P.PATIENTID='{0}'or P.CLINICCODE='{0}' /*or P.IDENNO='{0}' or P.CARDNO='{0}'*/)
AND P.STATE IN ({1})
/* AND P.OPERDATE >SYSDATE -10 */ 
ORDER BY P.OPERDATE DESC ";

        #endregion

        public NetScape.AnalysisModel.PatientInfo QueryPatientInfoBase(string sql)
        {
            NetScape.AnalysisModel.PatientInfo patient = null;
            try
            {
                // sql = string.Format(sql, patientId);

                if (dataMgr.ExecQuery(sql) == -1)
                {
                    return null;
                }
                while (dataMgr.Reader.Read())
                {
                    patient = new NetScape.AnalysisModel.PatientInfo();
                    //  =dataMgr.Reader[0].ToString(); /*SERIALNUM[] */
                    patient.ID = dataMgr.Reader[2].ToString(); /*PATIENTID[] */
                    patient.PID.CardNO = dataMgr.Reader[2].ToString(); /*CARDNO[] */
                    // patient.PID.PatientNO = dataMgr.Reader[2].ToString(); /*CARDNO[] */
                    patient.PID.ID = dataMgr.Reader[3].ToString(); /*CLINICCODE[] */
                    patient.PID.HealthNO = dataMgr.Reader[4].ToString(); /*IDCARDNO[] */
                    patient.PID.IdenNo = dataMgr.Reader[5].ToString(); /*IDENNO[] */
                    patient.Name = dataMgr.Reader[6].ToString(); /*NAME[] */
                    //=dataMgr.Reader[7].ToString(); /*SPELLCODE[] */
                    //=dataMgr.Reader[8].ToString(); /*WBCODE[] */
                    patient.Sex = dataMgr.Reader[9].ToString(); /*SEX[] */
                    patient.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(dataMgr.Reader[10].ToString()); /*BIRTHYDAY[] */
                    patient.PhoneNumber = dataMgr.Reader[11].ToString(); /*HOMETEL[] */
                    patient.PhoneNumber = dataMgr.Reader[12].ToString(); /*MOBILETEL[] */
                    patient.Address = dataMgr.Reader[13].ToString(); /*HOMEADRRESS[] */
                    patient.PatientType = dataMgr.Reader[14].ToString(); /*PATIENTTYPE[] */
                    patient.Nation = dataMgr.Reader[15].ToString(); /*COUNTRY[] */
                    patient.State = dataMgr.Reader[16].ToString();
                    //=dataMgr.Reader[16].ToString(); /*NATION[] */
                    //=dataMgr.Reader[17].ToString(); /*OPERCODE[] */
                    //=dataMgr.Reader[18].ToString(); /*OPERDATE[] */
                    patient.Mark = dataMgr.Reader[20].ToString(); /*MARK[] */
                    patient.Ext1 = dataMgr.Reader[21].ToString(); /*EXT1[] */
                    patient.Ext2 = dataMgr.Reader[22].ToString(); /*EXT2[] */
                    patient.Ext3 = dataMgr.Reader[23].ToString(); /*EXT3[] */
                    //=dataMgr.Reader[24].ToString(); /*EXT42[] */
                    patient.Memo = dataMgr.Reader[25].ToString();
                    patient.Value = dataMgr.Reader[26].ToString();
                    break;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return patient;
        }

        /// <summary>
        /// 根据pdf的路径读取内容为byte[]
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] GetPdfContent(string url)
        {

            FileStream stream = new FileStream(url, FileMode.Open);
            byte[] content = new byte[stream.Length];
            stream.Read(content, 0, content.Length);
            stream.Close();
            return content;

        }

        NetScape.AnalysisModel.Profile.ConfigObj config = NetScape.AnalysisModel.Profile.ConfigSetting.ConfigItem;
        const string MultiReportStagePath = @".\MultiReportStage\";

        /// <summary>
        /// 报告处理流程
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pdfPath"></param>
        /// <returns></returns>
        public int HandleReportResult(NetScape.AnalysisModel.FileConent obj)
        {
            try
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "开始处理报告！patientId&&ApplyNo：" + obj.Patient.ID + "||" + obj.OrderItem.ID);

                #region 获取申请单相关信息****************

                NetScape.AnalysisModel.Order order = QueryOrderByApplyNo(obj.OrderItem.ID);
                if (order == null || string.IsNullOrEmpty(order.ID) || order.Patient == null || string.IsNullOrEmpty(order.Patient.ID))
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "没有找到匹配的患者信息!" + obj.Patient.ID);
                    return -1;
                }
                
                #endregion

                #region 获取消息内容**********************


                //string msgSeq = this.GetMsgFileId(order.Patient.PID.PatientNO);
                string msgSeq = this.GetMsgFileIdByApplyNo(obj.OrderItem.ID);
                NetScape.AnalysisModel.EcgMsgFileInfo msgFile = this.QueryMsgFile(msgSeq);
                if (msgFile == null || string.IsNullOrEmpty(msgFile.PatientId))
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "没有找到匹配的申请单消息!" + obj.Patient.ID);
                    return -1;
                }

                #endregion

                #region 下载pdf到本地缓存*****************

                string pdfPath = null;
                if (Common.FileHelper.DownLoadFtp(obj.WEBURL, obj.OrderItem.ID, ref pdfPath) == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(Funcs.Function._logName, "下载pdf失败！");
                }

                if (pdfPath == string.Empty)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(Funcs.Function._logName, "下载pdf失败！");
                    return -1;
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(Funcs.Function._logName, "下载pdf成功！" + pdfPath);

                #endregion

                #region 路径规则**************************

                // byte[] content = null;// GetPdfContent(outPath);
                string pathUrl = string.Empty;
                //string fileName = obj.Patient.ID + "_" + obj.OrderItem.ID + ".pdf";
                string fileName = msgFile.ID + ".pdf";
                // msgFile.UniqueKey = GetReportSeq();

                //平台存放pdf路径规则 ftp地址+/年/月/日/pk值/ +  文件名
                string paltPath = DateTime.Now.ToString("*yyyy*MM*dd*");// +msgFile.P_Key + "*";
                paltPath = paltPath.Replace("*", "/");

                #endregion

                #region 重置pdf 格式 *********************

                string outPath = AppDomain.CurrentDomain.BaseDirectory + "\\Report\\" + paltPath.Replace("/", "\\") + obj.Patient.ID + "\\";

                if (!Directory.Exists(outPath))
                {
                    Directory.CreateDirectory(outPath);
                }
                outPath = outPath + fileName;

                if (ResetPdtFormat(obj, pdfPath, outPath) == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "重置pdf  报告格式失败!");
                    return -1;
                }

                //路径保存起来，更新回数据库
                msgFile.PdfFileName = outPath;
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, outPath);

                #endregion

                #region 处理多报告合并, 例如15/18导检查

                if (!Directory.Exists(MultiReportStagePath)) Directory.CreateDirectory(MultiReportStagePath);

                if (obj.IsMultiReport)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "开始处理多报告...");

                    string stageFile = string.Format("{0}{1}_{2}.pdf", MultiReportStagePath, obj.OrderItem.ID, obj.ReportSequence);

                    File.Move(outPath, stageFile);

                    var files = Directory.GetFiles(MultiReportStagePath, obj.OrderItem.ID + "*");

                    if (files.Length >= 2)
                    {
                        //合并pdf报告
                        if (!MergeMultiECGReports(files, outPath)) return -1;

                        //删除 Stage 中的已处理pdf;
                        foreach (var file in files)
                        {
                            File.Delete(file);
                        }

                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "合并多报告成功.");
                    }
                    else
                    {
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "多报告移入Stage.");
                        return 1;
                    }
                }
                #endregion

                #region 报告主键**************************

                /**************************/
                msgFile.P_Key = GetReportSeq();
                msgFile.PdfUrl = string.Format("{0}{1}/{2}", paltPath, msgFile.P_Key, fileName);

                switch (obj.Patient.Sex)
                {
                    case "1":
                        obj.Patient.Sex = "M";
                        break;
                    case "2":
                        obj.Patient.Sex = "F";
                        break;
                    case "3":
                        obj.Patient.Sex = "U";
                        break;
                    case "9":
                        obj.Patient.Sex = "O";
                        break;
                    default:
                        break;
                }

                #endregion

                #region 插入平台患者信息 *****************

                if (order.Status != "4")
                {
                    if (InsertPerson(order) == -1)
                    {
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "插入平台患者信息失败!" + this.ErrMsg + obj.Patient.ID);
                        return -1;
                    }
                    if (InsertPersonVisit(order.Patient) == -1)
                    {
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "插入平台患者信息失败!" + this.ErrMsg + obj.Patient.ID);
                        return -1;
                    }
                }
                #endregion

                #region 测试代码 *************************

                //string testPath = @"D:\Code\Temp\Test.pdf";
                //if (Common.FileHelper.TestUpload(testPath, fileName, Common.SettingHelper.setObj.FtpDIR, paltPath, Common.SettingHelper.setObj.FtpUser, Common.SettingHelper.setObj.FtpPwd, ref pathUrl) == -1)
                //{
                //    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"上传报告失败!" + this.ErrMsg + obj.OrderItem.ID);
                //}
                //else
                //{
                //    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"上传报告成功!" + "|ApplyNo:|" + obj.OrderItem.ID + "|PaltUrl:|" + pathUrl);
                //}

                #endregion

                #region 上传Pdf报告 **********************


                paltPath += "//" + msgFile.P_Key + "//";
                if (Common.FileHelper.TestUpload(outPath, fileName, Common.SettingHelper.setObj.FtpDIR, paltPath, Common.SettingHelper.setObj.FtpUser, Common.SettingHelper.setObj.FtpPwd, ref pathUrl) == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "上传报告失败!" + this.ErrMsg + obj.OrderItem.ID);
                    return -1;
                }
                else
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "上传报告成功!" + "|ApplyNo:|" + obj.OrderItem.ID + "|PaltUrl:|" + pathUrl);

                }
                #endregion            

                #region 插入平台报告表 *****************

                //荷载子类型： 新增/修改
                if (order.Status == "4")
                {
                    msgFile.Ext3 = "UPDATE";
                }
                else
                {
                    msgFile.Ext3 = "ADD";
                }

                if (InsertReport(msgFile, order.Patient, null) == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "插入平台报告结果表失败!" + this.ErrMsg + obj.Patient.ID);
                    return -1;
                }

                if (InsertReportExtend(msgFile, order.Patient) == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "插入平台报告结果拓展表失败!" + this.ErrMsg + obj.Patient.ID);
                    return -1;
                }

                #endregion

                #region 上传XML报告 **********************

                var ret = UploadXml(obj, order, msgFile, outPath);

                #endregion


                #region 回写状态通知平台 *****************
                //旧系统的申请单不回传给平台
                if (order.Patient.PatientType == "2" || order.Patient.PatientType == "3")
                {
                    if (UpdateCheckState(Common.SettingHelper.setObj, "4", obj) == -1)
                    {
                        Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "更新检查报告已发布状态（4）失败!" + this.ErrMsg + obj.Patient.ID);
                        // return -1;
                    }
                }
                #endregion

                #region 更新本地申请单状态****************

                if (UpdateApplyState(order.ID, "4") == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "更新申请单状态（4）失败!" + this.ErrMsg + obj.Patient.ID);
                    return -1;
                }
                #endregion

                #region 回写pdf报告路径到本地数据库*******

                UpdateMsgFileUrl(msgFile);

                #endregion

                //if (UpdateOrderStateByPatientInfo(patient.ID, "09") == -1)
                //{
                //    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"更新检查单状态（09）失败!" + this.ErrMsg + obj.Patient.ID);
                //    return -1;
                //}

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "处理报告完成！patientId&&ApplyNo：" + obj.Patient.ID + "||" + obj.OrderItem.ID);
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "处理心电报告失败！" + ex.Message + ex.StackTrace);
                return -1;
            }

            return 1;
        }

        private int UploadXml(AnalysisModel.FileConent obj, AnalysisModel.Order order, AnalysisModel.EcgMsgFileInfo msgFile, string outPath)
        {
            try
            {
                string xmlName = msgFile.ID + ".xml";
                string xmlLocalPath = outPath.Replace("pdf", "xml");

                string key = GetReportSeq();
                string paltPath = DateTime.Now.ToString("'/'yyyy'/'MM'/'dd'/'") + key + "/";
                string xmlPathUrl = paltPath + xmlName;
                string pathUrl = null;

                CreateReportXml(order, obj, xmlLocalPath);

                if (Common.FileHelper.TestUpload(xmlLocalPath, xmlName, Common.SettingHelper.setObj.FtpDIR, paltPath, Common.SettingHelper.setObj.FtpUser, Common.SettingHelper.setObj.FtpPwd, ref pathUrl) == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "上传Xml报告失败!" + this.ErrMsg + obj.OrderItem.ID);
                    return -1;
                }
                else
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "上传Xml报告成功!" + "|ApplyNo:|" + obj.OrderItem.ID + "|PaltUrl:|" + pathUrl);
                }

                msgFile.XmlReportLocalUrl = xmlLocalPath;
                msgFile.XmlReportUrl = xmlPathUrl;
                msgFile.P_Key = key;

                if (InsertReportXml(msgFile, order.Patient, null) == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "上传Xml报告失败!" + this.ErrMsg + xmlLocalPath + xmlPathUrl);
                    return -1;
                }
                if (InsertReportExtend(msgFile, order.Patient) == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "插入平台报告结果拓展表失败!" + this.ErrMsg + obj.Patient.ID);
                    return -1;
                }
            }
            catch (Exception ex )
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "上传Xml报告失败!" + order.Patient.ID + ":" + ex.ToString());
                return -1;
            }

            return 1;
        }

        private bool MergeMultiECGReports(string[] files, string outMergeFile)
        {
            Document document = new Document(PageSize.A4);
            List<PdfReader> openedReaders = new List<PdfReader>();
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outMergeFile, FileMode.Create));

                document.Open();

                PdfContentByte cb = writer.DirectContent;

                for (int i = 0; i < files.Length; i++)
                {
                    var reader = new PdfReader(files[i]);
                    openedReaders.Add(reader);

                    int iPageNum = reader.NumberOfPages;
                    for (int j = 1; j <= iPageNum; j++)
                    {
                        document.NewPage();
                        writer.ViewerPreferences = PdfWriter.PrintScalingNone;      //打印时默认不缩放
                        writer.AddPageDictEntry(PdfName.ROTATE, PdfPage.LANDSCAPE); //横屏显示
                        PdfImportedPage newPage = writer.GetImportedPage(reader, j);
                        cb.AddTemplate(newPage, 0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "合并多报告失败!!!!!! error:" + ex.Message);
                return false;
            }
            finally
            {
                document.Close();
                openedReaders.ForEach(reader => reader.Close());
            }
            return true;
        }

        public static string _logName = "worker";


        /// <summary>
        /// 门诊旧系统接收到患者后保存到库
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public int ReciveRegister(XElement req)
        {
            try
            {
                if (req.Element("CLINIC_CODE") != null)
                {
                    string clinicCode = req.Element("CLINIC_CODE").Value.Trim();
                    if (PatientExists(clinicCode))
                        return 1;
                }

                NetScape.AnalysisModel.PatientInfo patient = GetRegisterInfo(req);
                if (patient == null)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "解析患者信息失败！");

                }

                NetScape.AnalysisModel.Order order = new AnalysisModel.Order();
                order.Patient = patient;
                order.Patient.PatientType = "0";
                order.Status = "0";
                if (InsertPatientInfo(order) == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, this.ErrMsg);
                    return -1;
                }

            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "处理过程异常！" + ex.Message + ex.StackTrace);
                return -1;
            }

            return 1;
        }


        /// <summary>
        /// 解析推送的门诊患者信息（门诊患者）
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public NetScape.AnalysisModel.PatientInfo GetRegisterInfo(XElement register)
        {
            NetScape.AnalysisModel.PatientInfo info = new AnalysisModel.PatientInfo();
            info.PID.ID = register.Element("CLINIC_CODE").Value.Trim();
            info.ID = register.Element("CARD_NO").Value.Trim();
            info.PID.CardNO = register.Element("CARD_NO").Value.Trim();
            info.Name = register.Element("NAME").Value.Trim();
            if (register.Element("IDENNO") != null)
                info.PID.IdenNo = register.Element("IDENNO").Value.Trim();
            if (register.Element("SEX_CODE") != null)
                info.Sex = register.Element("SEX_CODE").Value.Trim();
            switch (info.Sex)
            {
                case "M":
                    info.Sex = "1";
                    break;
                case "F":
                    info.Sex = "2";
                    break;
                case "U":
                    info.Sex = "3";
                    break;
                case "O":
                    info.Sex = "9";
                    break;
                default:
                    break;
            }
            info.Birthday = ConvertToDateTime(register.Element("BIRTHDAY").Value.Trim());
            if (register.Element("RELA_PHONE") != null)
                info.PhoneNumber = register.Element("RELA_PHONE").Value.Trim();
            //都先记为门诊的患者，有可能是急诊。不过不涉及回传报告，可以不纠结这个。
            info.PatientType = "0";
            info.Mark = "1";

            return info;
        }

        /// <summary>
        /// 根据流水号删除患者信息
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <returns></returns>
        public int RemovePatient(string clinicCode)
        {
            string sql = @"delete from ecgpatientinfo a where a.cliniccode='{0}' and a.state='0' ";
            return dataMgr.ExecNoQuery(string.Format(sql, clinicCode));
        }

        /// <summary>
        /// 通过流水号判断是否患者已存在。
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <returns></returns>
        public bool PatientExists(string clinicCode)
        {
            string sql = @" select count(1) cnt from ecgpatientinfo a where a.cliniccode='{0}' ";
            return int.Parse(dataMgr.ExecSqlReturnOne(string.Format(sql, clinicCode).ToString())) > 0;
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
            if (GetCheckStateXml(state, order, ref xml) == -1)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, ErrMsg);
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
                // string id = item.Element("").Value;
                string result = item.Element("Result").Value.Trim() == "0" ? "成功" : "失败";
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "更新状态结果：" + result);

            }
            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, xml);
            return 1;
        }

        /// <summary>
        /// 更新检查状态
        /// 走申请单号，比较准
        /// </summary>
        /// <param name="state"></param>
        /// <param name="applyNo"></param>
        /// <param name="clinicCode"></param>
        /// <returns></returns>
        public int UpdateCheckState(string state, string applyNo, string clinicCode)
        {
            string xml = string.Empty;
            if (GetCheckStateXml(state, applyNo, ref xml) == -1)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, ErrMsg);
                return -1;
            }
            //Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, applyNo+xml);
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

            //  Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, xml);
            WebNote.Response response = client.RequisitionAdd(request);

            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, response.responseBody);

            XElement root = XElement.Parse(response.responseBody);
            List<XElement> results = root.Elements("RequestInfoAddResult").ToList();
            foreach (XElement item in results)
            {
                // string id = item.Element("").Value;
                string result = item.Element("Result").Value.Trim() == "0" ? "成功" : "失败";
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "更新状态结果：" + result);

            }
            // Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, xml);
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
            return UpdateCheckState(state, content.OrderItem.ID, content.Patient.PID.ID);
        }

        public int UpdatePatientInfoStateBySeq(NetScape.AnalysisModel.PatientInfo patient, string state)
        {
            #region sql

            string sql = @"  update ecgpatientinfo a
  set a.state='{1}'
  where a.serialnum='{0}'";

            #endregion

            sql = string.Format(sql, patient.ID, state);

            return dataMgr.ExecNoQuery(sql);

        }

        public int UpdatePatientInfoStateByPatientId(NetScape.AnalysisModel.PatientInfo patient, string state)
        {
            #region sql

            string sql = @"  update ecgpatientinfo a
  set a.state='{1}'
  where (a.patientid='{0}' or a.cliniccode='{2}')";

            #endregion
            sql = string.Format(sql, patient.ID, state, patient.PID.ID);

            return dataMgr.ExecNoQuery(sql);

        }

        /// <summary>
        /// 更新患者状态。根据ApplyNo
        /// </summary>
        /// <param name="order"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public int UpdatePatientInfoStateByApplyNo(NetScape.AnalysisModel.Order order, string state)
        {

            return UpdateApplyState(order.ID, state);
        }

        public int UpdateApplyState(string applyNo, string state)
        {
            #region sql

            string sql = @"  update ecgpatientinfo a
  set a.state='{1}'
  where  a.applyno='{0}'";

            if(state == "4")
            {
                sql = @"  update ecgpatientinfo a
  set a.state='{1}', a.reported = sysdate
  where  a.applyno='{0}'";
            }
            #endregion
            sql = string.Format(sql, applyNo, state);

            return dataMgr.ExecNoQuery(sql);
        }

        public int UpdateApplyItemState(string applyNo, string state)
        {
            #region sql

            string sql = @" update ecgapplybill a set a.applystate='{0}' where a.applyno='{1}' ";

            #endregion
            sql = string.Format(sql, state, applyNo);

            return dataMgr.ExecNoQuery(sql);
        }

        /// <summary>
        /// 修改心电图报告格式
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tempPath"></param>
        /// <param name="pdfPath"></param>
        /// <returns></returns>
        public int ResetPdtFormat(NetScape.AnalysisModel.FileConent obj, string tempPath, string outPath, bool isExtend)
        {
            try
            {
                if (string.IsNullOrEmpty(tempPath))
                    return -1;

                using (Stream inputPdfStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (Stream outputPdfStream = new FileStream(outPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var reader = new PdfReader(inputPdfStream);
                    var stamper = new PdfStamper(reader, outputPdfStream);
                    var pdfContentByte = stamper.GetOverContent(1);

                    #region 15/18导联名称替换

                    if (obj.IsMultiReport && obj.ReportSequence == 2)
                    {
                        List<KeyValuePair<string, string>> replacements = null;

                        if(obj.HasChannel15Attribute && config.PDFTextReplacements.ContainsKey("channel15"))
                        {
                            replacements = config.PDFTextReplacements["channel15"];                            
                        }
                        else if(obj.HasChannel18Attribute && config.PDFTextReplacements.ContainsKey("channel18"))
                        {
                            replacements = config.PDFTextReplacements["channel18"];
                        }

                        if (replacements != null && replacements.Count > 0)
                        {
                            PdfDictionary dict = reader.GetPageN(1);
                            PdfObject conentObj = dict.GetDirectObject(PdfName.CONTENTS);

                            if (conentObj is PRStream)
                            {
                                PRStream stream = (PRStream)conentObj;

                                byte[] data = PdfReader.GetStreamBytes(stream);

                                Encoding encoding = Encoding.GetEncoding("GB2312");

                                StringBuilder sb = new StringBuilder(encoding.GetString(data));

                                string format = "Td ({0})";

                                replacements.ForEach(rep =>
                                {
                                    sb.Replace(string.Format(format, rep.Key), string.Format(format, rep.Value));
                                });
                                
                                stream.SetData(encoding.GetBytes(sb.ToString()));
                            }
                        }
                    }

                    #endregion

                    #region //顶部覆盖层

                    ImgInfo topinfo = config.ImgInfoSet.Where(x => x.Value == "top").FirstOrDefault();
                    if (topinfo != null)
                    {
                        Bitmap top = new Bitmap(topinfo.Width, topinfo.Height);
                        iTextSharp.text.Image topImg = iTextSharp.text.Image.GetInstance(top, BaseColor.WHITE);
                        topImg.SetAbsolutePosition(topinfo.LocationX, topinfo.LocationY);
                        pdfContentByte.AddImage(topImg);
                    }

                    #endregion

                    #region logo

                    ImgInfo logoInfo = config.ImgInfoSet.Where(x => x.Value == "logo").FirstOrDefault();
                    if (logoInfo != null)
                    {
                        //System.Drawing.Image img = Properties.Resources.logo;
                        //System.Drawing.Image logo = new Bitmap(Properties.Resources.logo, logoInfo.Width, logoInfo.Height);// Common.ImageHelper.GenThumbnail(Properties.Resources.logo, logoInfo.Width, logoInfo.Height);
                        iTextSharp.text.Image logoImg = iTextSharp.text.Image.GetInstance(Properties.Resources.logo, ImageFormat.Jpeg);
                        logoImg.SetAbsolutePosition(logoInfo.LocationX, logoInfo.LocationY);
                        pdfContentByte.AddImage(logoImg);
                    }


                    #endregion

                    #region  //黑线


                    ImgInfo lineInfo = config.ImgInfoSet.Where(x => x.Value == "line").FirstOrDefault();
                    Bitmap line = new Bitmap(lineInfo.Width, lineInfo.Height);
                    iTextSharp.text.Image lineImg = iTextSharp.text.Image.GetInstance(line, BaseColor.BLACK);
                    lineImg.SetAbsolutePosition(lineInfo.LocationX, lineInfo.LocationY);
                    pdfContentByte.AddImage(lineImg);

                    #endregion

                    #region //底部覆盖层



                    //ImgInfo bottomInfo = config.ImgInfoSet.Where(x => x.Value == "buttom").FirstOrDefault();
                    //Bitmap bottom = new Bitmap(bottomInfo.Width, bottomInfo.Height);     //底部覆盖层             
                    //iTextSharp.text.Image bottomImg = iTextSharp.text.Image.GetInstance(bottom, BaseColor.WHITE);
                    //bottomImg.SetAbsolutePosition(bottomInfo.LocationX, bottomInfo.LocationY);
                    //pdfContentByte.AddImage(bottomImg);

                    #endregion

                    //字体设置
                    BaseFont bfTimes = BaseFont.CreateFont(@"C:\Windows\Fonts\simhei.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    BaseFont bfsong = BaseFont.CreateFont(@"c:\windows\fonts\simfang.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                    pdfContentByte.BeginText();

                    #region //标题


                    TxtInfo title = config.TxtInfoSet.Where(x => x.ID == "Title").FirstOrDefault();
                    pdfContentByte.SetFontAndSize(bfTimes, title.Size);
                    pdfContentByte.ShowTextAligned(30, title.Value, title.LocationX, title.LocationY, 0);
                    // pdfContentByte.ShowTextAligned(30, "告诉", title.LocationX, title.LocationY, 0);
                    pdfContentByte.EndText();
                    // pdfContentByte.NewlineShowText("351303351333113351sjhgowejfowefmoe");
                    #endregion

                    //第一组信息
                    if (obj.Patient.PatientType == "3")
                    {
                        #region 第一组信息
                        //Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, obj.Patient.ID);
                        pdfContentByte.BeginText();
                        {
                            TxtInfo txt1 = config.TxtInfoSet.Where(x => x.ID == "Txt1").FirstOrDefault();
                            pdfContentByte.SetFontAndSize(bfTimes, txt1.Size);
                            int txtRowH1 = txt1.LocationY;
                            int txtRowH2 = txt1.LocationY;

                            string xx = "体检编号：";
                            pdfContentByte.ShowTextAligned(30, xx, txt1.LocationX, txtRowH1, 0);
                            txtRowH1 -= txt1.RowHeight;

                            pdfContentByte.ShowTextAligned(30, "申请时间：" /*+ obj.Patient.Name*/, txt1.LocationX, txtRowH1, 0);
                            txtRowH1 -= txt1.RowHeight;

                            pdfContentByte.ShowTextAligned(30, "姓    名：" /*+ obj.Patient.Name*/, txt1.LocationX, txtRowH1, 0);
                            txtRowH1 -= txt1.RowHeight;

                            NetScape.AnalysisModel.Base.Sex s = NetScape.AnalysisModel.Base.Sex.GetSex(obj.Patient.Sex);
                            string sex = s.Name;
                            pdfContentByte.ShowTextAligned(30, "性    别：" /*+ sex*/, txt1.LocationX, txtRowH1, 0);

                            string birthDay = string.Empty, age = string.Empty;
                            if (obj.Patient.Birthday > DateTime.MinValue)
                            {
                                birthDay = obj.Patient.Birthday.ToShortDateString();
                                age = GetAge(obj.Patient.Birthday);
                            }
                            txtRowH1 -= txt1.RowHeight;
                            //pdfContentByte.ShowTextAligned(30, "出生日期：" +birthDay , txt1.LocationX, txtRowH1, 0);
                            pdfContentByte.ShowTextAligned(30, "年    龄：", txt1.LocationX, txtRowH1, 0);
                            txtRowH1 -= txt1.RowHeight;
                            pdfContentByte.ShowTextAligned(30, "序    号：" /*+ obj.Patient.Dept.Name*/, txt1.LocationX, txtRowH1, 0);

                            //赋值

                            pdfContentByte.SetFontAndSize(bfsong, txt1.Size);
                            int lx = txt1.LocationX + 60;
                            txtRowH1 = txt1.LocationY;
                            pdfContentByte.ShowTextAligned(30, obj.Patient.ID, lx, txtRowH1, 0);

                            txtRowH1 -= txt1.RowHeight;
                            string applyTime = string.Empty;
                            if (obj.ApplyTime.HasValue)
                            {
                                applyTime = obj.ApplyTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            pdfContentByte.ShowTextAligned(30, applyTime, lx, txtRowH1, 0);

                            txtRowH1 -= txt1.RowHeight;
                            pdfContentByte.ShowTextAligned(30, obj.Patient.Name, lx, txtRowH1, 0);

                            txtRowH1 -= txt1.RowHeight;
                            pdfContentByte.ShowTextAligned(30, sex, lx, txtRowH1, 0);

                            txtRowH1 -= txt1.RowHeight;
                            pdfContentByte.ShowTextAligned(30, age, lx, txtRowH1, 0);

                            txtRowH1 -= txt1.RowHeight;
                            string sequence = string.Format("{0} ({1})", obj.TJSequence, obj.Patient.Mark);
                            var bfont = new iTextSharp.text.Font(bfTimes, 12, iTextSharp.text.Font.BOLD);
                            Phrase pbold = new Phrase(sequence, bfont);
                            ColumnText.ShowTextAligned(pdfContentByte, Element.ALIGN_LEFT, pbold, lx, txtRowH1, 0);

                        }

                        #endregion
                    }
                    else
                    {
                        #region 第一组信息
                        //Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, obj.Patient.ID);
                        pdfContentByte.BeginText();
                        {
                            TxtInfo txt1 = config.TxtInfoSet.Where(x => x.ID == "Txt1").FirstOrDefault();
                            pdfContentByte.SetFontAndSize(bfTimes, txt1.Size);
                            int txtRowH1 = txt1.LocationY;
                            int txtRowH2 = txt1.LocationY;

                            string xx = "患者编号：";
                            pdfContentByte.ShowTextAligned(30, xx, txt1.LocationX, txtRowH1, 0);
                            txtRowH1 -= txt1.RowHeight;

                            pdfContentByte.ShowTextAligned(30, "患者姓名：" /*+ obj.Patient.Name*/, txt1.LocationX, txtRowH1, 0);
                            txtRowH1 -= txt1.RowHeight;
                            NetScape.AnalysisModel.Base.Sex s = NetScape.AnalysisModel.Base.Sex.GetSex(obj.Patient.Sex);
                            // if (s != null)

                            string sex = s.Name;
                            pdfContentByte.ShowTextAligned(30, "性    别：" /*+ sex*/, txt1.LocationX, txtRowH1, 0);

                            string birthDay = string.Empty, age = string.Empty;
                            if (obj.Patient.Birthday > DateTime.MinValue)
                            {
                                birthDay = obj.Patient.Birthday.ToShortDateString();
                                age = GetAge(obj.Patient.Birthday);
                            }
                            txtRowH1 -= txt1.RowHeight;
                            //pdfContentByte.ShowTextAligned(30, "出生日期：" +birthDay , txt1.LocationX, txtRowH1, 0);
                            pdfContentByte.ShowTextAligned(30, "年    龄：", txt1.LocationX, txtRowH1, 0);
                            txtRowH1 -= txt1.RowHeight;
                            pdfContentByte.ShowTextAligned(30, "科    室：" /*+ obj.Patient.Dept.Name*/, txt1.LocationX, txtRowH1, 0);

                            //赋值

                            pdfContentByte.SetFontAndSize(bfsong, txt1.Size);
                            int lx = txt1.LocationX + 60;
                            txtRowH1 = txt1.LocationY;
                            pdfContentByte.ShowTextAligned(30, obj.Patient.ID, lx, txtRowH1, 0);

                            txtRowH1 -= txt1.RowHeight;
                            pdfContentByte.ShowTextAligned(30, obj.Patient.Name, lx, txtRowH1, 0);

                            txtRowH1 -= txt1.RowHeight;
                            pdfContentByte.ShowTextAligned(30, sex, lx, txtRowH1, 0);

                            txtRowH1 -= txt1.RowHeight;
                            pdfContentByte.ShowTextAligned(30, age, lx, txtRowH1, 0);

                            txtRowH1 -= txt1.RowHeight;
                            string dept = obj.Patient.Dept.Name;
                            if (!string.IsNullOrEmpty(obj.Patient.BedNO))
                                dept += obj.Patient.BedNO + "床";
                            pdfContentByte.ShowTextAligned(30, dept, lx, txtRowH1, 0);

                        }

                        #endregion
                    }

                    #region   第2组信息


                    {
                        TxtInfo txt1 = config.TxtInfoSet.Where(x => x.ID == "Txt2").FirstOrDefault();
                        pdfContentByte.SetFontAndSize(bfTimes, txt1.Size);
                        int txtRowH1 = txt1.LocationY;
                        pdfContentByte.ShowTextAligned(30, "心室率：", txt1.LocationX, txtRowH1, 0);
                        txtRowH1 -= txt1.RowHeight;
                        pdfContentByte.ShowTextAligned(30, "PR期间：", txt1.LocationX, txtRowH1, 0);
                        txtRowH1 -= txt1.RowHeight;
                        pdfContentByte.ShowTextAligned(30, "QRS期间：", txt1.LocationX, txtRowH1, 0);
                        txtRowH1 -= txt1.RowHeight;
                        pdfContentByte.ShowTextAligned(30, "QT/QTc：", txt1.LocationX, txtRowH1, 0);
                        txtRowH1 -= txt1.RowHeight;
                        pdfContentByte.ShowTextAligned(30, "P-R-T电轴：", txt1.LocationX, txtRowH1, 0);
                        txtRowH1 -= txt1.RowHeight;
                        pdfContentByte.ShowTextAligned(30, "RV5/SV1：", txt1.LocationX, txtRowH1, 0);

                        pdfContentByte.SetFontAndSize(bfsong, txt1.Size);
                        txtRowH1 = txt1.LocationY;
                        int rowX = txt1.LocationX + 65;
                        pdfContentByte.ShowTextAligned(30, obj.VentRate, rowX, txtRowH1, 0);
                        txtRowH1 -= txt1.RowHeight;
                        pdfContentByte.ShowTextAligned(30, obj.PRInt, rowX, txtRowH1, 0);
                        txtRowH1 -= txt1.RowHeight;
                        pdfContentByte.ShowTextAligned(30, obj.QRSDur, rowX, txtRowH1, 0);
                        txtRowH1 -= txt1.RowHeight;
                        pdfContentByte.ShowTextAligned(30, obj.QTInt + "/" + obj.QTcInt, rowX, txtRowH1, 0);
                        txtRowH1 -= txt1.RowHeight;
                        pdfContentByte.ShowTextAligned(30, obj.PRTAxes, rowX, txtRowH1, 0);
                        txtRowH1 -= txt1.RowHeight;
                        if (!string.IsNullOrEmpty(obj.RAmplitude))
                        {
                            decimal ra = System.Decimal.Round((Neusoft.FrameWork.Function.NConvert.ToDecimal(obj.RAmplitude) / 1000), 2);
                            decimal sa = System.Decimal.Round((Neusoft.FrameWork.Function.NConvert.ToDecimal(obj.SAmplitude) / 1000), 2);
                            decimal sum = ra + sa;
                            string sumStr = sum.ToString() + "mV";
                            if (ra <= 0 || sa <= 0)
                                sumStr = string.Empty;
                            pdfContentByte.ShowTextAligned(30, ra.ToString() + "/" + sa.ToString() + "mV" + string.Format(" (R+S:{0})", sumStr), rowX, txtRowH1, 0);
                        }
                    }
                    #endregion

                    #region 第3组信息


                    {
                        TxtInfo txt1 = config.TxtInfoSet.Where(x => x.ID == "Txt3").FirstOrDefault();
                        pdfContentByte.SetFontAndSize(bfTimes, txt1.Size);
                        int txtRowH1 = txt1.LocationY;
                        int txtRowHg = txt1.RowHeight;
                        pdfContentByte.ShowTextAligned(30, "诊断结论：", txt1.LocationX, txtRowH1, 0);
                        //txtRowH1 -= txt1.RowHeight;
                        // pdfContentByte.ShowTextAligned(30, "患者姓名：" + "测试", txt1.LocationX, txtRowH1, 0);
                        pdfContentByte.SetFontAndSize(bfsong, txt1.Size);
                        if (!string.IsNullOrEmpty(obj.DiagECG))
                        {
                            List<string> diag = obj.DiagECG.Split(',').ToList();
                            if (diag != null && diag.Count > 0)
                            {
                                if (diag.Count > 4)
                                    txtRowHg -= 4;
                                txtRowH1 -= txtRowHg;
                                pdfContentByte.ShowTextAligned(30, diag[0], txt1.LocationX, txtRowH1, 0);
                                txtRowH1 -= txtRowHg;
                                if (diag.Count > 1)
                                {

                                    pdfContentByte.ShowTextAligned(30, diag[1], txt1.LocationX, txtRowH1, 0);
                                }

                                if (diag.Count > 2)
                                {
                                    txtRowH1 -= txtRowHg;
                                    pdfContentByte.ShowTextAligned(30, diag[2], txt1.LocationX, txtRowH1, 0);
                                }

                                if (diag.Count > 3)
                                {
                                    txtRowH1 -= txtRowHg;
                                    // string diagts = string.Join(",", diag.ToArray(), 2, diag.Count - 2);
                                    pdfContentByte.ShowTextAligned(30, diag[3], txt1.LocationX, txtRowH1, 0);
                                }

                                if (diag.Count > 4)
                                {
                                    txtRowH1 -= txtRowHg;
                                    string diagts = string.Join(",", diag.ToArray(), 4, diag.Count - 4);
                                    pdfContentByte.ShowTextAligned(30, diagts, txt1.LocationX, txtRowH1, 0);
                                }

                            }
                        }
                    }

                    #endregion

                    #region 第四组信息（底部）


                    {
                        TxtInfo txt1 = config.TxtInfoSet.Where(x => x.ID == "Txt5").FirstOrDefault();
                        pdfContentByte.SetFontAndSize(bfTimes, txt1.Size);

                        int txtColW1 = txt1.LocationX;
                        string note1 = "25mm/s  10mm/mV  {0} ";
                        note1 = !string.IsNullOrEmpty(config.Frequency) ? string.Format(note1, config.Frequency) : string.Format(note1, "100Hz");
                        pdfContentByte.ShowTextAligned(20, note1, txtColW1, txt1.LocationY, 0);

                        txtColW1 += txt1.RowHeight - 20;
                        pdfContentByte.ShowTextAligned(20, "MUSE 9.0 SP6", txtColW1, txt1.LocationY, 0);

                        txtColW1 += txt1.RowHeight - 80;
                        string checkDate = string.Empty;
                        if (obj.CheckDate > DateTime.Now.AddYears(-100))
                        {
                            checkDate = obj.CheckDate.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        pdfContentByte.ShowTextAligned(20, "检查时间：" + checkDate, txtColW1, txt1.LocationY, 0);

                        txtColW1 += txt1.RowHeight + 20;
                        string diagDate = string.Empty;
                        if (obj.DiagDate > DateTime.Now.AddYears(-100))
                        {
                            diagDate = obj.DiagDate.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        pdfContentByte.ShowTextAligned(20, "诊断时间：" + diagDate, txtColW1, txt1.LocationY, 0);
                        //txtColW1 += txt1.RowHeight;
                        //pdfContentByte.ShowTextAligned(30, "P-R-T电轴：" + obj.PRTAxes, txt1.LocationX, txtColW1, 0);

                        txtColW1 += 180;
                        {
                            // pdfContentByte.SetFontAndSize(bfTimes, txt1.Size);
                            string textDoct = "诊断医生：" + obj.DiagDoct;
                            if (string.IsNullOrEmpty(obj.DiagDoct))
                            {
                                textDoct = "未确认";
                            }
                            pdfContentByte.ShowTextAligned(20, textDoct, txtColW1, txt1.LocationY, 0);
                        }
                    }

                    #endregion


                    pdfContentByte.EndText();

                    //释放资源
                    stamper.Close();
                    outputPdfStream.Close();
                    inputPdfStream.Close();


                    ////////////////保存为图片////////////
                    //PDFFile pdfFile = PDFFile.Open(outputName);
                    //Bitmap pageImage = pdfFile.GetPageImage(0, 100);
                    //pageImage.Save(@"D:\code\ddd3s.png", System.Drawing.Imaging.ImageFormat.Png);
                    //pageImage.Dispose();
                    //pdfFile.Dispose();
                }
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "重置pdf格式失败！" + ex.Message);
                return -1;
            }
            return 1;
        }

        public void HandleTJinfo(NetScape.AnalysisModel.FileConent obj)
        {
            if (obj.Patient.PatientType == "3" && !string.IsNullOrEmpty(obj.OrderItem.ID))
            {
                NetScape.AnalysisModel.Order order = QueryOrderByApplyNo(obj.OrderItem.ID);
                obj.ApplyTime = order.ApplyTime;
                obj.Patient.Mark = order.Patient.Mark;

                using (OracleConnection conn = new OracleConnection())
                {
                    conn.ConnectionString = "data source=HIS;password=zsymns;user id=mns";
                    conn.Open();
                    using (OracleCommand command = conn.CreateCommand())
                    {
                        command.CommandText = string.Format("select v.OrderNo from zshis.v_Met_Chk_Apply v where  v.hissheetid='{0}'", obj.OrderItem.ID);
                        var reader = command.ExecuteReader();
                        if(reader.Read())
                        {
                            obj.TJSequence = reader[0].ToString();
                        }
                    }
                }
            }
        }
        public int ResetPdtFormat(NetScape.AnalysisModel.FileConent obj, string path, string outPath)
        {
            try
            {
                HandleTJinfo(obj);

                //加一个临时目录存放中间文件
                string temp_path = AppDomain.CurrentDomain.BaseDirectory + "\\pdf_temp\\";// + System.IO.Path.GetFileNameWithoutExtension(path) + ".pdf";
                if (!Directory.Exists(temp_path))
                {
                    Directory.CreateDirectory(temp_path);
                }
                temp_path += System.IO.Path.GetFileNameWithoutExtension(path) + ".pdf";
                ResetPdtFormat(obj, path, temp_path, false);

                PdfReader reader = new PdfReader(temp_path);

                //Console.WriteLine("Original PDF width:{0},height:{1}",reader.GetPageSize(1).Width, reader.GetPageSize(1).Height);
                //iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(0, 0, 595, 842);

                Document document = new Document(PageSize.A4, 0, 0, 0, 0);//设置页面大小，边距
                FileStream fs = new FileStream(outPath, FileMode.Create, FileAccess.Write);
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                writer.ViewerPreferences = PdfWriter.PrintScalingNone;//打印时默认不缩放
                writer.AddPageDictEntry(PdfName.ROTATE, PdfPage.LANDSCAPE);//横屏显示
                document.Open();
                PdfContentByte pdfContentByte = writer.DirectContent;
                PdfImportedPage page = writer.GetImportedPage(reader, 1); //获取原始PDF文档第一页内容
                pdfContentByte.AddTemplate(page, 0, 57);//设置新页面模版属性，后面两个参数为复制的原始页面内容在新页面上显

                document.Close();
                reader.Close();

                return 1;
            }
            catch (Exception ex)
            {
                this.ErrMsg = ex.Message + ex.StackTrace;
                return -1;
            }
        }

        /// <summary>
        /// 计算年龄
        /// </summary>
        /// <param name="birthyDay"></param>
        /// <returns></returns>
        string GetAge(DateTime birthyDay)
        {
            TimeSpan ts = DateTime.Now.Subtract(birthyDay).Duration();

            double days = Math.Floor(ts.TotalDays);
            int year, month, day;
            year = (int)Math.Floor(days / 365);
            month = (int)(Math.Floor(days - year * 365) / 30);
            day = (int)(Math.Floor(days - year * 365 - month * 30));

            if (days < 0)
                return string.Empty;
            if (year <= 0)
            {
                if (month < 0)
                    return day.ToString() + "天";
                else
                    return month.ToString() + "月" + day.ToString() + "天";
            }
            if (year < 5)
            {
                if (month <= 0)
                    return year.ToString() + "岁";
                else
                    return year.ToString() + "岁" + month.ToString() + "月";
            }
            return year.ToString() + "岁";
        }
        //public string GetAge(DateTime birthyDay)
        //{
        //    TimeSpan ts = DateTime.Now.Subtract(birthyDay).Duration();
        //    double mounths = ts.TotalDays / 30;
        //    if (mounths > 60)
        //        return Math.Floor(mounths / 12).ToString() + "岁";
        //    else
        //    {
        //        if (mounths >= 12)
        //        {
        //            if ((mounths % 12) == 0)
        //                return Math.Floor(mounths / 12).ToString() + "岁";
        //            else
        //                return Math.Floor(mounths / 12).ToString() + "岁" + Math.Floor((mounths % 12)).ToString() + "月";
        //        }
        //        else
        //            return Math.Floor(mounths).ToString() + "月";
        //    }
        //}

        public string QueryOrderState(string applyNo, string clinicCode)
        {
            string sql = @" select max(a.state) state from ecgpatientinfo a where a.applyno='{0}' and a.clinicCode='{1}' ";
            sql = string.Format(sql, applyNo, clinicCode);
            return dataMgr.ExecSqlReturnOne(sql);
        }

        public int UpdatePatientState(string patientId, string state)
        {
            string sql1 = @"update ecgpatientinfo a
set a.state='{0}'
where a.patientid='{1}' or a.cliniccode='{1}'";
            sql1 = string.Format(sql1, state, patientId);
            return dataMgr.ExecNoQuery(sql1);


        }

        public int UpdateOrderStateByPatientInfo(string patientId, string state)
        {
            string sql1 = @"update ecgapplybill a
set a.applystate='{0}'
where a.cliniccode='{1}' or a.patientno='{1}'";
            sql1 = string.Format(sql1, state, patientId);
            return dataMgr.ExecNoQuery(sql1);
        }

        public int UpdateOrderStateByCheckId(string checkId, string state)
        {
            string sql1 = @"update ecgapplybill a
set a.applystate='{0}'
where a.examid='{1}'";
            sql1 = string.Format(sql1, state, checkId);
            return dataMgr.ExecNoQuery(sql1);
        }


        public bool ApplyNoExists(string applyNo)
        {
            string sql = @"select count(*) as cnt from ecgpatientinfo where applyno ='{0}' ";
            sql = string.Format(sql, applyNo);
            return Neusoft.FrameWork.Function.NConvert.ToInt32(dataMgr.ExecSqlReturnOne(sql)) > 0;
        }

        public int UpdateFeeStateByApplyNo(string applyNo, string state)
        {
            string sql = @" update ecgpatientinfo a set a.feestate='{0}' where a.applyno='{1}' ";
            sql = string.Format(sql, state, applyNo);
            return dataMgr.ExecNoQuery(sql);
        }


        /// <summary>
        /// 处理电生理报告。
        /// </summary>
        /// <param name="applyNo"></param>
        /// <param name="path"></param>
        /// <param name="newPath"></param>
        /// <returns></returns>
        public int HandleEctReport(string applyNo, string path, ref string newPath)
        {
            if (string.IsNullOrEmpty(applyNo) || string.IsNullOrEmpty(path))
            {
                ErrMsg = "申请单号为空！或者路径为空！";
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, ErrMsg);
                return -1;
            }

            NetScape.AnalysisModel.Order order = QueryRegisterConfirmedOrder(applyNo);

            if (order == null) return -1;
           
            #region 获取消息内容**********************


            string msgSeq = order.Patient.Ext3;
            NetScape.AnalysisModel.EcgMsgFileInfo msgFile = this.QueryMsgFile(msgSeq);
            if (msgFile == null)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "没有找到匹配的申请单消息!" + order.Patient.ID+this.ErrMsg);
                return -1;
            }

            #endregion

            #region 报告主键**************************

            /**************************/
            msgFile.P_Key = GetReportSeq();

            switch (order.Patient.Sex)
            {
                case "1": order.Patient.Sex = "M";
                    break;
                case "2": order.Patient.Sex = "F";
                    break;
                case "3": order.Patient.Sex = "U";
                    break;
                case "9": order.Patient.Sex = "O";
                    break;
                default:
                    break;
            }

            #endregion

            #region 路径规则**************************

            byte[] content = null;// GetPdfContent(outPath);
            string pathUrl = string.Empty;
            //string fileName = obj.Patient.ID + "_" + obj.OrderItem.ID + ".pdf";
            string fileName = msgFile.ID + ".pdf";
            // msgFile.UniqueKey = GetReportSeq();

            //平台存放pdf路径规则 ftp地址+/年/月/日/pk值/ +  文件名
            string paltPath = DateTime.Now.ToString("'/'yyyy'/'MM'/'dd'/'") +msgFile.P_Key + "/";
            string outPath = AppDomain.CurrentDomain.BaseDirectory + "\\Report\\" + paltPath.Replace("/", "\\") +order.Patient.ID  +"_Ect\\";
            #endregion

            #region 插入平台患者信息 *****************

            if (order.Status != "4")
            {
                if (InsertPerson(order) == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "插入平台患者信息失败!" + this.ErrMsg + order.Patient.ID);
                    return -1;
                }
                if (InsertPersonVisit(order.Patient) == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "插入平台患者信息失败!" + this.ErrMsg + order.Patient.ID);
                    return -1;
                }
            }

            #endregion

            #region 上传Pdf报告 **********************

            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);
            System.IO.File.Copy(path, outPath + fileName, true);

            msgFile.PdfUrl = paltPath + fileName;

            if (Common.FileHelper.TestUpload(outPath + fileName, fileName, Common.SettingHelper.setObj.FtpDIR, paltPath, Common.SettingHelper.setObj.FtpUser, Common.SettingHelper.setObj.FtpPwd, ref pathUrl) == -1)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "上传报告失败!" + this.ErrMsg + order.ID);
                return -1;
            }
            else
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "上传报告成功!" + "|ApplyNo:|" + order.ID + "|PaltUrl:|" + pathUrl);

            }

            #endregion

            #region 插入平台报告主表 *****************

            //荷载子类型： 新增/修改
            if (order.Status == "4")
            {
                msgFile.Ext3 = "UPDATE";
            }
            else
            {
                msgFile.Ext3 = "ADD";
            }





            if (InsertReport(msgFile, order.Patient, content) == -1)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "插入平台报告结果表失败!" + this.ErrMsg + order.Patient.ID);
                return -1;
            }

            #endregion

            #region 插入平台报告拓展表****************

            // msgFile.Memo = order.Patient.PatientType;
            if (InsertReportExtend(msgFile, order.Patient) == -1)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "插入平台报告结果拓展表失败!" + this.ErrMsg + order.Patient.ID);
                return -1;
            }

            #endregion

            //旧系统的申请单不回传给平台
            if (order.Patient.PatientType == "2" || order.Patient.PatientType == "3")
            {
                #region 回写状态通知平台 *****************

                if (UpdateCheckState("4", order.ID, order.Patient.PID.ID) == -1)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "更新检查报告已发布状态（4）失败!" + this.ErrMsg + order.Patient.ID);
                    //return -1;
                }

                #endregion
            }

            #region 更新本地申请单状态****************

            if (UpdatePatientInfoStateByApplyNo(order, "4") == -1)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, "更新患者状态（4）失败!" + this.ErrMsg + order.Patient.ID);
                return -1;
            }

            #endregion

            #region 回写pdf报告路径到本地数据库*******

            msgFile.PdfFileName = outPath + fileName;
            msgFile.PdfUrl = pathUrl;
            UpdateMsgFileUrl(msgFile);

            #endregion

            return 1;
        }

        public int QueryInpatientOrder(string inpNo, ref NetScape.AnalysisModel.Order order)
        {
            order = null;
            NetScape.AnalysisModel.PatientInfo info = QueryInPatientInfo(inpNo, "2", "'1'");
            if (info == null)
                return -1;
            string appNoList = QueryPatientApplyBillList(info.PID.ID);
            if (string.IsNullOrEmpty(appNoList))
                return -1;
            List<string> list = appNoList.Split(',').ToList();
            order = new AnalysisModel.Order();
            order.Patient = info;

            NetScape.AnalysisModel.OrderItem orderItem = null;
            foreach (var item in list)
            {
                orderItem = QueryOrderItemByApplyNo(item);
                order.OrderItems.Add(orderItem);
            }
            if (orderItem != null)
            {
                order.ReciptDept = orderItem.ApplyDept;
                order.ExecDept = orderItem.ExecDept;
                order.FeeState = info.Memo;
                // order.app
            }
            return 1;
        }

        public NetScape.AnalysisModel.OrderItem QueryOrderItemByApplyNo(string applyNo)
        {
            #region sql

            string sql = @"select 
P.CLINICCODE,--1
P.CARDNO,--2
P.PATIENTNO,--3
P.APPLYNO,--4
P.EXAMID,--5
P.CLINCDIAG,--6
P.MAINDIAG,--7
P.RECIPTNO,--8
P.RECIPTDEPT,--9
P.RECIPTDEPTNAME,--10
P.RECIPTDOCT,--11
P.RECIPTDOCTNAME,--12
P.EXECDEPT,--13
P.EXECDEPTNAME,--14
P.ITEMCODE,--15
P.ITEMNAME,--16
P.APPLYSTATE,--17
P.VALIDSTATE,--18
P.SYSCLASS,--19
P.CHECKPARTID,--20
P.CHECKPARTNAME,--21
P.CHECKPARTMEMO,--22
P.EMERGENTFLAG,--23
P.SPECIMENTYPE,--24
P.CHECKTYPE,--25
P.FEESTATE,--26
P.UNIT,--27
P.QTY,--28
P.UNITPRICE,--29
P.TOTCOST,--30
P.CHECKDESC,--31
P.CHECKRESULT,--32
P.REPORTID,--33
P.REPORTTYPE,--34
P.REPORTURL,--35
P.ISBOOK,--36
P.BOOKID,--37
P.OPERCODE,--38
P.OPERDATE,--39
P.MEMO,--40
P.MARK,--41
P.EXT1,--42
P.EXT2,--43
P.EXT3,--44
P.EXT4,--45
P.EXT5--46
FROM ECGAPPLYBILL P
WHERE P.APPLYNO='{0}'
AND P.OPERDATE >SYSDATE -30 ";

            #endregion

            sql = string.Format(sql, applyNo);

            if (dataMgr.ExecQuery(sql) == -1)
            {

            }
            while (dataMgr.Reader.Read())
            {

                NetScape.AnalysisModel.OrderItem item = new NetScape.AnalysisModel.OrderItem();
                //item. =assignMgr.Reader[0].ToString(); /*CLINICCODE[] */
                //=assignMgr.Reader[1].ToString(); /*CARDNO[] */
                //=assignMgr.Reader[2].ToString(); /*PATIENTNO[] */
                item.ApplyNo = dataMgr.Reader[3].ToString(); /*APPLYNO[] */
                item.CheckID = dataMgr.Reader[4].ToString(); /*EXAMID[] */
                //if (list.Where(x => x.CheckID == item.CheckID).Count() > 0)
                //    continue;
                //=assignMgr.Reader[5].ToString(); /*CLINCDIAG[] */
                //=assignMgr.Reader[6].ToString(); /*MAINDIAG[] */
                //order.ReciptNO = dataMgr.Reader[7].ToString(); /*RECIPTNO[] */
                item.ApplyDept.ID = dataMgr.Reader[8].ToString(); /*RECIPTDEPT[] */
                item.ApplyDept.Name = dataMgr.Reader[9].ToString(); /*RECIPTDEPTNAME[] */
                //if (order.ReciptDept == null || string.IsNullOrEmpty(order.ReciptDept.ID))
                //{
                //    order.ReciptDept = item.ApplyDept;
                //    order.ReciptDoctor = item.ApplyOper;
                //}

                item.ApplyOper.ID = dataMgr.Reader[10].ToString(); /*RECIPTDOCT[] */
                item.ApplyOper.Name = dataMgr.Reader[11].ToString(); /*RECIPTDOCTNAME[] */
                item.ExecDept.ID = dataMgr.Reader[12].ToString(); /*EXECDEPT[] */
                item.ExecDept.Name = dataMgr.Reader[13].ToString(); /*EXECDEPTNAME[] */
                item.Item.ID = dataMgr.Reader[14].ToString(); /*ITEMCODE[] */
                item.Item.Name = dataMgr.Reader[15].ToString(); /*ITEMNAME[] */
                //                =assignMgr.Reader[16].ToString(); /*APPLYSTATE[] */
                //=assignMgr.Reader[17].ToString(); /*VALIDSTATE[] */
                item.SysClass.Name = dataMgr.Reader[18].ToString(); /*SYSCLASS[] */
                item.CheckPart.ID = dataMgr.Reader[19].ToString(); /*CHECKPARTID[] */
                item.CheckPart.Name = dataMgr.Reader[20].ToString(); /*CHECKPARTNAME[] */
                item.CheckPart.Memo = dataMgr.Reader[21].ToString(); /*CHECKPARTMEMO[] */
                // order.IsUrgent = Neusoft.FrameWork.Function.NConvert.ToBoolean(assignMgr.Reader[22].ToString()); /*EMERGENTFLAG[] */
                item.Sample = dataMgr.Reader[23].ToString(); /*SPECIMENTYPE[] */
                //=assignMgr.Reader[24].ToString(); /*CHECKTYPE[] */
                //=assignMgr.Reader[25].ToString(); /*FEESTATE[] */
                item.Unit = dataMgr.Reader[26].ToString(); /*UNIT[] */
                item.Qty = NetScape.AnalysisToolKit.NConvert.ToInt32(dataMgr.Reader[27].ToString()); /*QTY[] */
                item.UnitPrice = NetScape.AnalysisToolKit.NConvert.ToDecimal(dataMgr.Reader[28].ToString()); /*UNITPRICE[] */
                item.TotCost = NetScape.AnalysisToolKit.NConvert.ToDecimal(dataMgr.Reader[29].ToString()); /*TOTCOST[] */
                //=assignMgr.Reader[30].ToString(); /*CHECKDESC[] */
                //=assignMgr.Reader[31].ToString(); /*CHECKRESULT[] */
                //=assignMgr.Reader[32].ToString(); /*REPORTID[] */
                //=assignMgr.Reader[33].ToString(); /*REPORTTYPE[] */
                //=assignMgr.Reader[34].ToString(); /*REPORTURL[] */
                //=assignMgr.Reader[35].ToString(); /*ISBOOK[] */
                //=assignMgr.Reader[36].ToString(); /*BOOKID[] */
                //=assignMgr.Reader[37].ToString(); /*OPERCODE[] */
                item.OperDate = DateTime.Parse(dataMgr.Reader[38].ToString()); /*OPERDATE[] */
                //=assignMgr.Reader[39].ToString(); /*MEMO[] */
                //=assignMgr.Reader[40].ToString(); /*MARK[] */
                //=assignMgr.Reader[41].ToString(); /*EXT1[] */
                //=assignMgr.Reader[42].ToString(); /*EXT2[] */
                //=assignMgr.Reader[43].ToString(); /*EXT3[] */
                //=assignMgr.Reader[44].ToString(); /*EXT4[] */
                //=assignMgr.Reader[45].ToString(); /*EXT5[] */
                return item;
            }

            return null;
        }

        public string QueryPatientApplyBillList(string inpId)
        {
            string sql = @"  select wm_concat(applyno)applyno from 
(select distinct a.applyno
from ecgpatientinfo a
where (a.patientid='{0}' or a.cliniccode='{0}')
and a.state in ('1','2')
and a.patienttype='2'
and a.operdate>sysdate-30 ) ";
            sql = string.Format(sql, inpId);


            return dataMgr.ExecSqlReturnOne(sql);
        }



        /// <summary>
        /// 删除患者检查单
        /// </summary>
        /// <param name="applyNo">申请单</param>
        /// <returns></returns>
        public int DeletePatientByApplyNo(string applyNo)
        {
            if (string.IsNullOrEmpty(applyNo))
            {
                return 1;
            }
            string sql = @"delete from ecgpatientinfo a where a.applyno='{0}' ";
            sql = string.Format(sql, applyNo);
            return dataMgr.ExecNoQuery(sql);
        }

        public int DeleteApplyDetail(string applyNo)
        {
            if (string.IsNullOrEmpty(applyNo))
            {
                return 1;
            }
            string sql = @"delete from ecgapplybill a where a.applyno='{0}' ";
            sql = string.Format(sql, applyNo);

            return dataMgr.ExecNoQuery(sql);
        }

        public string GetPrintBarCodeType()
        {
            return dataMgr.ExecSqlReturnOne("SELECT NAME FROM COM_DICTIONARY A WHERE A.TYPE='ECGPRINTBAR' AND CODE ='0001' ");
        }

        /// <summary>
        /// 生成Xml报告
        /// </summary>
        /// <param name="order">生清单信息</param>
        /// <param name="result">hl7结果</param>
        /// <param name="savePath">保存本地路径</param>
        /// <returns></returns>
        public string CreateReportXml(NetScape.AnalysisModel.Order order, NetScape.AnalysisModel.FileConent result, string savePath)
        {
            List<string> diags = result.DiagECG.Split(',').ToList();
            XElement diag = new XElement("ReportConclusion");
            foreach (var item in diags)
            {
                if (!string.IsNullOrEmpty(item))
                    diag.Add(new XElement("stm", item));
            }
            string rv5 = string.Empty;
            string sv1 = string.Empty;
            if (!string.IsNullOrEmpty(result.RAmplitude))
            {
                decimal ra = System.Decimal.Round((Neusoft.FrameWork.Function.NConvert.ToDecimal(result.RAmplitude) / 1000), 2);
                decimal sa = System.Decimal.Round((Neusoft.FrameWork.Function.NConvert.ToDecimal(result.SAmplitude) / 1000), 2);
                rv5 = ra > 0 ? ra.ToString() + " mV" : "";
                sv1 = sa > 0 ? sa.ToString() + " mV" : "";
            }

            XElement root = new XElement("ECGReport",
                new XElement("ReportII", order.Patient.Ext3),
                new XElement("RequestID", result.OrderItem.ID),
                new XElement("PatientID", order.Patient.ID),
                new XElement("VisitNumber", result.Patient.PID.ID),
                 new XElement("PatientName", result.Patient.Name),
                 new XElement("PatientSexCode", result.Patient.Sex),
                 new XElement("PatientSexName", NetScape.AnalysisModel.Base.Sex.GetSex(result.Patient.Sex).Name),
                 new XElement("PatientSexCodeSystem", "2.16.840.1.113883.4.487.2.1.1.1.9"),
                 new XElement("PatientAge", GetAge(result.Patient.Birthday)),
                 new XElement("PatientBirth", result.Patient.Birthday.ToString("yyyyMMdd")),
                 new XElement("IdentityNO", order.Patient.PID.IdenNo),
                 new XElement("Telephone", order.Patient.PhoneNumber),
                 new XElement("ClinicalDiagnose", order.Patient.MainDiagnose),
                 new XElement("ExamineEmployee", result.DiagDoct),
                 new XElement("ExamineOn", result.CheckDate.ToString("yyyy-MM-dd HH:mm:ss")),
                 new XElement("AuditEmployee", result.DiagDoct),
                 new XElement("AuthorDomainId", GetDomainByCode("10").ID),
                 new XElement("AuditOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                 new XElement("ReportOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                 new XElement("ReportNo", order.Patient.Ext3),
                 new XElement("RowVersion", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                 new XElement("EffectiveTime", DateTime.MaxValue.ToString("yyyy-MM-dd HH:mm:ss")),
                 new XElement("PatientTypeCode", order.Patient.PatientType),
                 new XElement("PatientType", Common.ConstManager.GetPatientType(order.Patient.PatientType)),
                 new XElement("PatientTypeCodeSystem", "2.16.840.1.113883.4.487.2.1.1.1.13"),
                 new XElement("PatientRegisterTime", string.Empty),
                 new XElement("StudyBedroom", result.Patient.BedNO),
                 new XElement("ApplyDepartmentsID", order.ReciptDept.ID),
                 new XElement("ApplyDepartmentsName", order.ReciptDept.Name),
                 new XElement("ApplyDoctorID", order.ReciptDoctor.ID),
                 new XElement("ApplyDoctorName", order.ReciptDoctor.Name),
                 new XElement("Title", "中山大学附属第一医院心电图报告"),
                 new XElement("ReportDesc",
                     new XElement("VentricularRate", result.VentRate),
                     new XElement("AtrialRate", result.AtrialRate),
                     new XElement("PRInterval", result.PRInt),
                     new XElement("QRSDuration", result.QRSDur),
                     new XElement("QTInterval", result.QTInt),
                     new XElement("QTCorrected", result.QTcInt),
                     new XElement("PAxis", result.PAxes + " " + result.AxesUnit),
                     new XElement("RAxis", result.RAxes + " " + result.AxesUnit),
                     new XElement("TAxis", result.TAxes + " " + result.AxesUnit),
                     new XElement("RV5", rv5),
                     new XElement("SV1", sv1)
                     ),
                 new XElement("IfOtherHospital", "0"),
                 new XElement("OtherHospitalName", "广州中山大学附属第一医院")
                );
            root.Add(diag);

            root.Save(savePath);

            return root.ToString();
        }


        public int QueryReciptDoct(NetScape.AnalysisModel.Order order)
        {
            string sql = @"select a.reciptdept,a.reciptdeptname,a.reciptdoct,a.reciptdoctname from ecgapplybill a where a.applyno='{0}' ";

            sql = string.Format(sql, order.ID);
            if (dataMgr.ExecQuery(sql) == -1)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName, sql + dataMgr.Err);
                return -1;
            }

            while (dataMgr.Reader.Read())
            {
                order.ReciptDept.ID = dataMgr.Reader[0].ToString();
                order.ReciptDept.Name = dataMgr.Reader[1].ToString();
                order.ReciptDoctor.ID = dataMgr.Reader[2].ToString();
                order.ReciptDoctor.Name = dataMgr.Reader[3].ToString();
                break;
            }
            return 1;
        }

    }
}
