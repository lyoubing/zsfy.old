using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using log4net;

namespace NetScape.AnalysisWork.Common
{
    public static class FtpFile
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       static string _logName = "worker";
        /// <summary>
        /// 上传FTP
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="filename">文件名称</param>
        /// <param name="ftpServerIP">FTP IP+端口</param>
        /// <param name="ftpUserID">FTP用户名</param>
        /// <param name="ftpPassword">FTP密码</param>
        /// <returns></returns>
        public static int UploadFtp(string filePath, string filename, string ftpServerIP, string ftpUserID, string ftpPassword, ref string ftpFilePath)
        {
            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"Start 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            //如果数据库文件目录已经存在斜杠：“\”，则下一句的"\\"需要去掉
            FileInfo fileInf = new FileInfo(filename);
            string uri = "ftp://" + ftpServerIP + "/" + filePath;
            
            //判断是否存在目录
            Uri existUri = new Uri("ftp://" + ftpServerIP + "/" + filePath + "/");
            Uri testUri = null;
            string[] dirs = filePath.Split('/');
            string curDir = "";
            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i];
                //如果是以/开始的路径,第一个为空    
                if (dir != null && dir.Length > 0)
                {
                    try
                    {
                        curDir += dir + "/";
                        testUri = new Uri("ftp://" + ftpServerIP + "/" + curDir);
                        if (!DirectoryIsExist(testUri, ftpUserID, ftpPassword))
                        {
                            CreateDirectory(testUri, ftpUserID, ftpPassword);
                        }
                    }
                    catch (Exception)
                    { }
                }
            }

            

            FtpWebRequest reqFTP;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri + "/" + fileInf.Name));//"ftp://" + ftpServerIP + "/" + fileInf.Name));
            try
            {
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UseBinary = true;
                reqFTP.ContentLength = fileInf.Length;
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;
                FileStream fs = fileInf.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                Stream strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                strm.Close();
                fs.Close();
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"End 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);

                ftpFilePath = uri + "/" + System.IO.Path.GetFileName(filename);
                return 1;
            }
            catch (Exception ex)
            {
                reqFTP.Abort();
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"FTP上传错误:" + ex.Message);
                ftpFilePath = "";
                return -1;
            }
        }

        public static string errMsg;

        public static bool CreateDirectory(Uri IP, string UserName, string UserPass)
        {
            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"Start 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            try
            {
                FtpWebRequest FTP = (FtpWebRequest)FtpWebRequest.Create(IP);
                FTP.Credentials = new NetworkCredential(UserName, UserPass);
                FTP.Proxy = null;
                FTP.KeepAlive = false;
                FTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                FTP.UseBinary = true;
                FtpWebResponse response = FTP.GetResponse() as FtpWebResponse;
                response.Close();
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"End 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch(Exception e)
            {
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"FTP获取文件路径错误：" + System.Reflection.MethodBase.GetCurrentMethod().Name + e.Message);
                return false;
            }
        }

        public static bool DirectoryIsExist(Uri pFtpServerIP, string pFtpUserID, string pFtpPW)
        {
            //Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"Start 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            string[] value = GetFileList(pFtpServerIP, pFtpUserID, pFtpPW);
           // Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"End 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (value == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string[] GetFileList(Uri pFtpServerIP, string pFtpUserID, string pFtpPW)
        {
            Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"Start 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            StringBuilder result = new StringBuilder();
            try
            {
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(pFtpServerIP);
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(pFtpUserID, pFtpPW);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();
                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"End 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                return result.ToString().Split('\n');
            }
            catch(Exception e)
            {
                if(e.Message.Contains("550"))
                {
                    //550代表当前文件路径不存在
                    Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"End 方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                }

                Neusoft.FrameWork.Function.HisLog.WriteLog(_logName,"FTP获取文件异常（若是）：" + System.Reflection.MethodBase.GetCurrentMethod().Name + e.Message);
                return null;
            }
        }
    }
}
