﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.VSDesigner 4.0.30319.42000 版自动生成。
// 
#pragma warning disable 1591

namespace NetScape.AnalysisWork.MessageNote {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="MessageServiceSoapBinding", Namespace="http://www.gzsums.net/noticeService/")]
    public partial class MessageService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback messageQueryOperationCompleted;
        
        private System.Threading.SendOrPostCallback messageRecieveOperationCompleted;
        
        private System.Threading.SendOrPostCallback messageNoticeOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public MessageService() {
            this.Url = global::NetScape.AnalysisWork.Properties.Settings.Default.AnalysisWork_MessageNote_MessageService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event messageQueryCompletedEventHandler messageQueryCompleted;
        
        /// <remarks/>
        public event messageRecieveCompletedEventHandler messageRecieveCompleted;
        
        /// <remarks/>
        public event messageNoticeCompletedEventHandler messageNoticeCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.gzsums.net/noticeService/messageQuery", RequestNamespace="http://www.gzsums.net/noticeService/", ResponseNamespace="http://www.gzsums.net/noticeService/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Response messageQuery(Request req) {
            object[] results = this.Invoke("messageQuery", new object[] {
                        req});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void messageQueryAsync(Request req) {
            this.messageQueryAsync(req, null);
        }
        
        /// <remarks/>
        public void messageQueryAsync(Request req, object userState) {
            if ((this.messageQueryOperationCompleted == null)) {
                this.messageQueryOperationCompleted = new System.Threading.SendOrPostCallback(this.OnmessageQueryOperationCompleted);
            }
            this.InvokeAsync("messageQuery", new object[] {
                        req}, this.messageQueryOperationCompleted, userState);
        }
        
        private void OnmessageQueryOperationCompleted(object arg) {
            if ((this.messageQueryCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.messageQueryCompleted(this, new messageQueryCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.gzsums.net/noticeService/messageRecieve", RequestNamespace="http://www.gzsums.net/noticeService/", ResponseNamespace="http://www.gzsums.net/noticeService/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Response messageRecieve(Request req) {
            object[] results = this.Invoke("messageRecieve", new object[] {
                        req});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void messageRecieveAsync(Request req) {
            this.messageRecieveAsync(req, null);
        }
        
        /// <remarks/>
        public void messageRecieveAsync(Request req, object userState) {
            if ((this.messageRecieveOperationCompleted == null)) {
                this.messageRecieveOperationCompleted = new System.Threading.SendOrPostCallback(this.OnmessageRecieveOperationCompleted);
            }
            this.InvokeAsync("messageRecieve", new object[] {
                        req}, this.messageRecieveOperationCompleted, userState);
        }
        
        private void OnmessageRecieveOperationCompleted(object arg) {
            if ((this.messageRecieveCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.messageRecieveCompleted(this, new messageRecieveCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.gzsums.net/noticeService/messageNotice", RequestNamespace="http://www.gzsums.net/noticeService/", ResponseNamespace="http://www.gzsums.net/noticeService/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Response messageNotice(Request req) {
            object[] results = this.Invoke("messageNotice", new object[] {
                        req});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void messageNoticeAsync(Request req) {
            this.messageNoticeAsync(req, null);
        }
        
        /// <remarks/>
        public void messageNoticeAsync(Request req, object userState) {
            if ((this.messageNoticeOperationCompleted == null)) {
                this.messageNoticeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnmessageNoticeOperationCompleted);
            }
            this.InvokeAsync("messageNotice", new object[] {
                        req}, this.messageNoticeOperationCompleted, userState);
        }
        
        private void OnmessageNoticeOperationCompleted(object arg) {
            if ((this.messageNoticeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.messageNoticeCompleted(this, new messageNoticeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gzsums.net/noticeService/")]
    public partial class Request {
        
        private RequestHeader requestHeaderField;
        
        private string requestBodyField;
        
        /// <remarks/>
        public RequestHeader requestHeader {
            get {
                return this.requestHeaderField;
            }
            set {
                this.requestHeaderField = value;
            }
        }
        
        /// <remarks/>
        public string requestBody {
            get {
                return this.requestBodyField;
            }
            set {
                this.requestBodyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gzsums.net/noticeService/")]
    public partial class RequestHeader {
        
        private string senderField;
        
        private string receiverField;
        
        private string requestTimeField;
        
        private string msgTypeField;
        
        private string msgIdField;
        
        private string msgPriorityField;
        
        private string msgVersionField;
        
        /// <remarks/>
        public string sender {
            get {
                return this.senderField;
            }
            set {
                this.senderField = value;
            }
        }
        
        /// <remarks/>
        public string receiver {
            get {
                return this.receiverField;
            }
            set {
                this.receiverField = value;
            }
        }
        
        /// <remarks/>
        public string requestTime {
            get {
                return this.requestTimeField;
            }
            set {
                this.requestTimeField = value;
            }
        }
        
        /// <remarks/>
        public string msgType {
            get {
                return this.msgTypeField;
            }
            set {
                this.msgTypeField = value;
            }
        }
        
        /// <remarks/>
        public string msgId {
            get {
                return this.msgIdField;
            }
            set {
                this.msgIdField = value;
            }
        }
        
        /// <remarks/>
        public string msgPriority {
            get {
                return this.msgPriorityField;
            }
            set {
                this.msgPriorityField = value;
            }
        }
        
        /// <remarks/>
        public string msgVersion {
            get {
                return this.msgVersionField;
            }
            set {
                this.msgVersionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gzsums.net/noticeService/")]
    public partial class ResponseHeader {
        
        private string senderField;
        
        private string receiverField;
        
        private string requestTimeField;
        
        private string msgTypeField;
        
        private string msgIdField;
        
        private string errCodeField;
        
        private string errMessageField;
        
        private string msgPriorityField;
        
        private string msgVersionField;
        
        /// <remarks/>
        public string sender {
            get {
                return this.senderField;
            }
            set {
                this.senderField = value;
            }
        }
        
        /// <remarks/>
        public string receiver {
            get {
                return this.receiverField;
            }
            set {
                this.receiverField = value;
            }
        }
        
        /// <remarks/>
        public string requestTime {
            get {
                return this.requestTimeField;
            }
            set {
                this.requestTimeField = value;
            }
        }
        
        /// <remarks/>
        public string msgType {
            get {
                return this.msgTypeField;
            }
            set {
                this.msgTypeField = value;
            }
        }
        
        /// <remarks/>
        public string msgId {
            get {
                return this.msgIdField;
            }
            set {
                this.msgIdField = value;
            }
        }
        
        /// <remarks/>
        public string errCode {
            get {
                return this.errCodeField;
            }
            set {
                this.errCodeField = value;
            }
        }
        
        /// <remarks/>
        public string errMessage {
            get {
                return this.errMessageField;
            }
            set {
                this.errMessageField = value;
            }
        }
        
        /// <remarks/>
        public string msgPriority {
            get {
                return this.msgPriorityField;
            }
            set {
                this.msgPriorityField = value;
            }
        }
        
        /// <remarks/>
        public string msgVersion {
            get {
                return this.msgVersionField;
            }
            set {
                this.msgVersionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gzsums.net/noticeService/")]
    public partial class Response {
        
        private ResponseHeader responseHeaderField;
        
        private string responseBodyField;
        
        /// <remarks/>
        public ResponseHeader responseHeader {
            get {
                return this.responseHeaderField;
            }
            set {
                this.responseHeaderField = value;
            }
        }
        
        /// <remarks/>
        public string responseBody {
            get {
                return this.responseBodyField;
            }
            set {
                this.responseBodyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void messageQueryCompletedEventHandler(object sender, messageQueryCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class messageQueryCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal messageQueryCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void messageRecieveCompletedEventHandler(object sender, messageRecieveCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class messageRecieveCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal messageRecieveCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void messageNoticeCompletedEventHandler(object sender, messageNoticeCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class messageNoticeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal messageNoticeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591