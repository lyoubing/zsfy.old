using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisWork.Base
{
    public class HL7MSG
    {
        //患者唯一编号
        public string PatientID { get; set; }

        //患者姓名
        public string Name { get; set; }

        //医嘱号
        public string OrderNumber { get; set; }

        //下医嘱时间
        public DateTime OrderDateTime { get; set; }

        //HIS医嘱类型ID
        public string HisOrderType { get; set; }
        //医嘱名称
        public string OrderTitle { get; set; }

        //登记号
        public string VisitNumber { get; set; }

        //性别，只能是以下三个值之一
        //M     ----男
        //F     ----女
        //U     ----未知
        public string Gender { get; set; }

        //年龄，年龄小于2岁半的医嘱将无法与检查匹配
        public DateTime BirthDate { get; set; }

        //科室名
        public string PatientLocation { get; set; }

        //床号
        public string Bed { get; set; }

        //房间号，由于在国内主要以病床号来区分，所以建议将 科室名称+病床号 拼接后赋值给Room,方便在界面上查看
        public string Room { get; set; }

        //检查类型，只能是三种值
        //12 Lead ECG   ----表示常规静息心电图
        //Holter        ----表示24小时动态心电图
        //Stress Test   ----表示运动平板实验
        public string TestType { get; set; }

        //检查原因或临床诊断
        public string TestReason { get; set; }

        //下医嘱医生姓名
        public string OrderPhys { get; set; }

        private static UInt32 MsgCtrlID { get; set; }

        static HL7MSG()
        {
            MsgCtrlID = 0;
        }

        private static char MESSAGE_END
        {
            get
            {
                return '\u001c';
            }
        }

        /// <summary>
        /// Gets character indicating the start of an HL7 message
        /// </summary>
        private static char MESSAGE_START
        {
            get
            {
                return '\u000b';
            }
        }

        /// <summary>
        /// Gets the final character of a message : a carriage return
        /// </summary>
        private static char LAST_CHARACTER
        {
            get
            {
                return '\r';
            }
        }

        public string CreateHL7ADTMsg()
        {
            string msg = "";
            StringBuilder builder = new StringBuilder();

            if ((PatientID == "") || (Name == "") || (OrderNumber == ""))
                return msg;

            builder.AppendLine(string.Format("MSH|^~\\&|SCM|AGH|MUSE|SITE0001|{0}||ORM^O01|{1}|P|2.4", DateTime.Now.ToString("yyyyMMddHHmmss"), GenMsgCtrlID()));
            builder.AppendLine(string.Format("PID|1||{0}||{1}||{2}|{3}||U|||||||||", PatientID, Name, BirthDate.ToString("yyyyMMdd"), Gender));
            builder.AppendLine(string.Format("PV1|1|INPAT|{0}^{1}^{2}^^^^^^||||||||||||||||{3}|||||||||||||||||||||||||||||||", PatientLocation, Room, Bed, VisitNumber));
            builder.AppendLine(string.Format("ORC|NW|{0}|||||1^^^{1}^^Routine|||||^{2}^", OrderNumber, OrderDateTime, OrderPhys));
            builder.AppendLine(string.Format("OBR|1|||{0}^{1}^^^{2}|||||||||||||||||||||||||||^{3}", HisOrderType, OrderTitle, TestType, TestReason));
            msg = string.Format("{0}{1}{2}{3}", MESSAGE_START, builder.ToString(), MESSAGE_END, LAST_CHARACTER);
            //msg = builder.ToString();
            msg = msg.Replace("\r\n", "\r");

            return msg;
        }

        public string CancelHL7ADTMsg()
        {
            string msg = "";
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("MSH|^~\\&|SCM|AGH|MUSE|SITE0001|{0}||ORM^O01|{1}|P|2.4", DateTime.Now.ToString("yyyyMMddHHmmss"), GenMsgCtrlID()));
            builder.AppendLine(string.Format("PID|1||{0}||{1}||{2}|{3}||U|||||||||", PatientID, Name, BirthDate.ToString("yyyyMMdd"), Gender));
            builder.AppendLine(string.Format("PV1|1|INPAT|{0}^{1}^{2}^^^^^^||||||||||||||||{3}|||||||||||||||||||||||||||||||", PatientLocation, Room, Bed, VisitNumber));
            builder.AppendLine(string.Format("ORC|CA|{0}|||||1^^^{1}^^Routine|||||^{2}^", OrderNumber, OrderDateTime, OrderPhys));
            builder.AppendLine(string.Format("OBR|1|||{0}^{1}^^^{2}|||||||||||||||||||||||||||^{3}", HisOrderType, OrderTitle, TestType, TestReason));
            msg = string.Format("{0}{1}{2}{3}", MESSAGE_START, builder.ToString(), MESSAGE_END, LAST_CHARACTER);
            msg = msg.Replace("\r\n", "\r");

            return msg;
        }

        private UInt32 GenMsgCtrlID()
        {
            return MsgCtrlID++;
        }

        public void Clear()
        {
            PatientID = "";
            Name = "";
            OrderNumber = "";
            //OrderDateTime = DateTime.Now();
            HisOrderType = "";
            OrderTitle = "";
            VisitNumber = "";
            Gender = "";
            PatientLocation = "";
            Bed = "";
            Room = "";
            TestType = "";
            TestReason = "";
            OrderPhys = "";
        }
    }
}
