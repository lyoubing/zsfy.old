﻿////------------------------------------------------------------------------------
//// <auto-generated>
////     此代码由工具生成。
////     运行时版本:4.0.30319.42000
////
////     对此文件的更改可能会导致不正确的行为，并且如果
////     重新生成代码，这些更改将会丢失。
//// </auto-generated>
////------------------------------------------------------------------------------

//// 
//// 此源代码由 wsdl 自动生成, Version=4.0.30319.33440。
//// 
//namespace requisition {
//    using System;
//    using System.Web.Services;
//    using System.Diagnostics;
//    using System.Web.Services.Protocols;
//    using System.Xml.Serialization;
//    using System.ComponentModel;
    
    
//    /// <remarks/>
//    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
//    [System.Diagnostics.DebuggerStepThroughAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Web.Services.WebServiceBindingAttribute(Name="RequestNoteSoap", Namespace="http://www.gzsums.net/requisition/")]
//    public partial class RequestNote : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
//        private System.Threading.SendOrPostCallback RequestDataReceiveOperationCompleted;
        
//        /// <remarks/>
//        public RequestNote() {
//            this.Url = "http://localhost:4463/RequestNote.asmx";
//        }
        
//        /// <remarks/>
//        public event RequestDataReceiveCompletedEventHandler RequestDataReceiveCompleted;
        
//        /// <remarks/>
//        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.gzsums.net/requisition/RequestDataReceive", RequestNamespace="http://www.gzsums.net/requisition/", ResponseNamespace="http://www.gzsums.net/requisition/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
//        public Response RequestDataReceive(Request req) {
//            object[] results = this.Invoke("RequestDataReceive", new object[] {
//                        req});
//            return ((Response)(results[0]));
//        }
        
//        /// <remarks/>
//        public System.IAsyncResult BeginRequestDataReceive(Request req, System.AsyncCallback callback, object asyncState) {
//            return this.BeginInvoke("RequestDataReceive", new object[] {
//                        req}, callback, asyncState);
//        }
        
//        /// <remarks/>
//        public Response EndRequestDataReceive(System.IAsyncResult asyncResult) {
//            object[] results = this.EndInvoke(asyncResult);
//            return ((Response)(results[0]));
//        }
        
//        /// <remarks/>
//        public void RequestDataReceiveAsync(Request req) {
//            this.RequestDataReceiveAsync(req, null);
//        }
        
//        /// <remarks/>
//        public void RequestDataReceiveAsync(Request req, object userState) {
//            if ((this.RequestDataReceiveOperationCompleted == null)) {
//                this.RequestDataReceiveOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRequestDataReceiveOperationCompleted);
//            }
//            this.InvokeAsync("RequestDataReceive", new object[] {
//                        req}, this.RequestDataReceiveOperationCompleted, userState);
//        }
        
//        private void OnRequestDataReceiveOperationCompleted(object arg) {
//            if ((this.RequestDataReceiveCompleted != null)) {
//                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
//                this.RequestDataReceiveCompleted(this, new RequestDataReceiveCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
//            }
//        }
        
//        /// <remarks/>
//        public new void CancelAsync(object userState) {
//            base.CancelAsync(userState);
//        }
//    }
    
//    /// <remarks/>
//    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
//    [System.SerializableAttribute()]
//    [System.Diagnostics.DebuggerStepThroughAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gzsums.net/requisition/")]
//    public partial class Request {
        
//        private RequestHeader requestHeaderField;
        
//        private string requestBodyField;
        
//        /// <remarks/>
//        public RequestHeader requestHeader {
//            get {
//                return this.requestHeaderField;
//            }
//            set {
//                this.requestHeaderField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string requestBody {
//            get {
//                return this.requestBodyField;
//            }
//            set {
//                this.requestBodyField = value;
//            }
//        }
//    }
    
//    /// <remarks/>
//    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
//    [System.SerializableAttribute()]
//    [System.Diagnostics.DebuggerStepThroughAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gzsums.net/requisition/")]
//    public partial class RequestHeader {
        
//        private string senderField;
        
//        private string receiverField;
        
//        private string requestTimeField;
        
//        private string msgTypeField;
        
//        private string msgIdField;
        
//        private string msgPriorityField;
        
//        private string msgVersionField;
        
//        /// <remarks/>
//        public string sender {
//            get {
//                return this.senderField;
//            }
//            set {
//                this.senderField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string receiver {
//            get {
//                return this.receiverField;
//            }
//            set {
//                this.receiverField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string requestTime {
//            get {
//                return this.requestTimeField;
//            }
//            set {
//                this.requestTimeField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string msgType {
//            get {
//                return this.msgTypeField;
//            }
//            set {
//                this.msgTypeField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string msgId {
//            get {
//                return this.msgIdField;
//            }
//            set {
//                this.msgIdField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string msgPriority {
//            get {
//                return this.msgPriorityField;
//            }
//            set {
//                this.msgPriorityField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string msgVersion {
//            get {
//                return this.msgVersionField;
//            }
//            set {
//                this.msgVersionField = value;
//            }
//        }
//    }
    
//    /// <remarks/>
//    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
//    [System.SerializableAttribute()]
//    [System.Diagnostics.DebuggerStepThroughAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gzsums.net/requisition/")]
//    public partial class ResponseHeader {
        
//        private string senderField;
        
//        private string receiverField;
        
//        private string requestTimeField;
        
//        private string msgTypeField;
        
//        private string msgIdField;
        
//        private string msgPriorityField;
        
//        private string msgVersionField;
        
//        private string errCodeField;
        
//        private string errMessageField;
        
//        /// <remarks/>
//        public string sender {
//            get {
//                return this.senderField;
//            }
//            set {
//                this.senderField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string receiver {
//            get {
//                return this.receiverField;
//            }
//            set {
//                this.receiverField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string requestTime {
//            get {
//                return this.requestTimeField;
//            }
//            set {
//                this.requestTimeField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string msgType {
//            get {
//                return this.msgTypeField;
//            }
//            set {
//                this.msgTypeField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string msgId {
//            get {
//                return this.msgIdField;
//            }
//            set {
//                this.msgIdField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string msgPriority {
//            get {
//                return this.msgPriorityField;
//            }
//            set {
//                this.msgPriorityField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string msgVersion {
//            get {
//                return this.msgVersionField;
//            }
//            set {
//                this.msgVersionField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string errCode {
//            get {
//                return this.errCodeField;
//            }
//            set {
//                this.errCodeField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string errMessage {
//            get {
//                return this.errMessageField;
//            }
//            set {
//                this.errMessageField = value;
//            }
//        }
//    }
    
//    /// <remarks/>
//    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
//    [System.SerializableAttribute()]
//    [System.Diagnostics.DebuggerStepThroughAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gzsums.net/requisition/")]
//    public partial class Response {
        
//        private ResponseHeader responseHeaderField;
        
//        private string responseBodyField;
        
//        /// <remarks/>
//        public ResponseHeader responseHeader {
//            get {
//                return this.responseHeaderField;
//            }
//            set {
//                this.responseHeaderField = value;
//            }
//        }
        
//        /// <remarks/>
//        public string responseBody {
//            get {
//                return this.responseBodyField;
//            }
//            set {
//                this.responseBodyField = value;
//            }
//        }
//    }
    
//    /// <remarks/>
//    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
//    public delegate void RequestDataReceiveCompletedEventHandler(object sender, RequestDataReceiveCompletedEventArgs e);
    
//    /// <remarks/>
//    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
//    [System.Diagnostics.DebuggerStepThroughAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    public partial class RequestDataReceiveCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
//        private object[] results;
        
//        internal RequestDataReceiveCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
//                base(exception, cancelled, userState) {
//            this.results = results;
//        }
        
//        /// <remarks/>
//        public Response Result {
//            get {
//                this.RaiseExceptionIfNecessary();
//                return ((Response)(this.results[0]));
//            }
//        }
//    }
//}
