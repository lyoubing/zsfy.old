using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;


namespace Analysis.Service.Inpatient
{
    public class SoapExceptionHandler : System.Web.Services.Protocols.SoapExtension
    {
        Stream oldStream;
        MemoryStream newStream;

        public override Stream ChainStream(Stream stream)
        {
            oldStream = stream;
            newStream = new MemoryStream();
            return newStream;
        }
        public override void ProcessMessage(System.Web.Services.Protocols.SoapMessage message)
        {
            if (message.Stage == SoapMessageStage.AfterSerialize)
            {
                WriteOutput(message);

                if (message.Exception != null)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog("errors", message.Exception.InnerException.ToString());
                }
            }
            else if (message.Stage == SoapMessageStage.BeforeDeserialize)
            {
                WriteInput(message);
            }
        }

        public override object GetInitializer(Type serviceType)
        {
            return null;
        }

        public override object GetInitializer(
            LogicalMethodInfo methodInfo,
            SoapExtensionAttribute attribute)
        {
            return null;
        }

        public override void Initialize(object initializer)
        {
        }

        void WriteInput(SoapMessage message)
        {
            Copy(oldStream, newStream);

            newStream.Position = 0;

            StreamReader sr = new StreamReader(newStream);

            var request = sr.ReadToEnd();

            Neusoft.FrameWork.Function.HisLog.WriteLog("requests", request);

            newStream.Position = 0;
        }

        void WriteOutput(SoapMessage message)
        {
            newStream.Position = 0;

            StreamReader sr = new StreamReader(newStream);

            var response = sr.ReadToEnd();

            Neusoft.FrameWork.Function.HisLog.WriteLog("requests", response);

            newStream.Position = 0;
            Copy(newStream, oldStream);
        }

        void Copy(Stream fromStream, Stream toStream)
        {
            var reader = new StreamReader(fromStream);
            var writer = new StreamWriter(toStream);
            writer.WriteLine(reader.ReadToEnd());
            writer.Flush();
        }
    }
}
