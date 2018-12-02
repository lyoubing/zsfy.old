using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisModel.Base
{
    public class MsgSendHeader
    {
        public MsgSendHeader(string msgType)
        {
            this.Sender = "2.16.840.1.113883.4.487.2.1.45";
            this.Receiver = "2.16.840.1.113883.4.487.2.1";
            this.MsgType = msgType;
        }

        public MsgSendHeader(string sender, string receiver, string msgType)
        {
            this.Sender = sender;
            this.Receiver = receiver;
            this.MsgType = MsgType;
        }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string SendTime
        {
            get { return DateTime.Now.ToString("yyyyMMddHHmmss"); }
        }
        public string MsgType { get; set; }
        public string MsgId
        {
            get
            {
                Random rn = new Random();
                return DateTime.Now.ToString("yyyyMMddHHmmssfff") + rn.Next(1, 1000).ToString();
            }
        }
        public string ErrCode { get; set; }

        public string ErrMessage { get; set; }

        string prior = "Normal";
        public string MsgPriority { get { return prior; } set { prior = value; } }

        public string MsgVersion { get { return "1.0.0"; } }

    }


}
