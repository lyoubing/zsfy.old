using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using log4net;
using System.Threading;
using System.Collections;
using NetScape.AnalysisModel;

namespace NetScape.AnalysisWork.Work
{
    public class MessageSend
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string _errMsg = string.Empty;

        public int Send(string IP, int port, HL7MSG hl7msg)
        {
           // log.Debug("Start Send HL7 Message ..............");

            string result = string.Empty;
            //string hl7msg = string.Empty;
            Socket tmpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(IP), port);
                tmpSocket.Connect(remoteEP);

                //Get HL7 message
                //hl7msg = BatchSendOrder(i);
                //log.Info(hl7msg.CreateHL7ADTMsg());
                string hl7Str = hl7msg.CreateHL7ADTMsg();
                Neusoft.FrameWork.Function.HisLog.WriteLog("hl7Msg", hl7Str);

                byte[] hl7bytes = Encoding.Default.GetBytes(hl7Str);
                if(hl7bytes.Length==0)
                {
                    this._errMsg = "字节长度为空！";
                    return -1;
                    
                }

                tmpSocket.Send(hl7bytes, hl7bytes.Length, 0);

                Neusoft.FrameWork.Function.HisLog.WriteLog("hl7Msg", string.Format("$ Send Order OK --> PID={0},OrderNumber={1}", hl7msg.PatientID, hl7msg.OrderNumber));

                byte[] recvBytes = new byte[1024];
                int bytes;
                bytes = tmpSocket.Receive(recvBytes, recvBytes.Length, 0);
                result = Encoding.GetEncoding(0x3a8).GetString(recvBytes, 0, bytes);
            }
            catch (Exception ex)
            {
                //log.Debug(string.Format("$ Send Fail -->  msg ({0})", ex.Message));
                Neusoft.FrameWork.Function.HisLog.WriteLog("hl7Msg","Send Fail -->"+ ex.Message);
                //MessageBox.Show("Send message failed!");
                Thread.Sleep(30000);

                return -1;
            }
            finally
            {
                tmpSocket.Close();
            }

           // log.Debug("End Send HL7 Message ..............");

            return 1;            
        }

        public void BathSend(string IP, int port, ArrayList hl7Msgs)
        {
            foreach (HL7MSG msg in hl7Msgs)
            {
                this.Send(IP, port, msg);
            }
        }
    }
}
