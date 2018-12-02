using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace NetScape.AnalysisWork.Common
{
    public class FileHelper
    {
        static WebClient client = new WebClient();

        static string path = string.Empty;
        static StringBuilder sb = null;
        static string reportPath = string.Empty;

        public static string SetFileToLocalFTP(string fileURL, string orderID)
        {
            try
            {
                sb = new StringBuilder();
                sb.Append(SettingHelper.setObj.FtpDIR);
                sb.Append(DateTime.Now.ToString("yyyy"));
                sb.Append(@"\");
                sb.Append(DateTime.Now.ToString("MM"));
                sb.Append(@"\");
                sb.Append(DateTime.Now.ToString("dd"));
                sb.Append(@"\");

                path = sb.ToString();

                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                //string receiveDIR = @"D:\ReportToCCA\";
                string fileName = orderID + ".pdf";
                client.DownloadFile(fileURL, path + fileName);

                sb = new StringBuilder();

                sb.Append("ftp://");
                sb.Append(SettingHelper.setObj.FtpUser);
                sb.Append(":");
                sb.Append(SettingHelper.setObj.FtpPwd);
                sb.Append("@");
                sb.Append(SettingHelper.setObj.GEIP);
                sb.Append("/");
                sb.Append(DateTime.Now.ToString("yyyy"));
                sb.Append(@"/");
                sb.Append(DateTime.Now.ToString("MM"));
                sb.Append(@"/");
                sb.Append(DateTime.Now.ToString("dd"));
                sb.Append(@"/");
                sb.Append(fileName);

                reportPath = sb.ToString();

                //string reportPath = "ftp://" + SettingHelper.setObj.FtpUser + ":" + SettingHelper.setObj.FtpPwd + "@" + SettingHelper.setObj.GEIP + "/" + fileName;
                //return "ftp://muse:123456@10.10.240.116/" + fileName;

                return reportPath;
            }
            catch
            {
                return "";
            }

        }

        public static string SetFileToRemoteFtp(string fileURL, string fileName, string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            try
            {
                sb = new StringBuilder();
                sb.Append(DateTime.Now.ToString("yyyy"));
                sb.Append(@"/");
                sb.Append(DateTime.Now.ToString("MM"));
                sb.Append(@"/");
                sb.Append(DateTime.Now.ToString("dd"));

                path = sb.ToString(); // 2018/01/18

                string fileNameTmp = System.IO.Path.GetDirectoryName(fileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(fileName) + ".pdf";
                client.DownloadFile(fileURL, fileNameTmp);

                string ftpFilePath = string.Empty;

                int result = FtpFile.UploadFtp(path, fileNameTmp, ftpServerIP, ftpUserID, ftpPassword, ref ftpFilePath);

                if (result <= 0)
                {
                    return "";
                }

                File.Delete(fileNameTmp);

                return ftpFilePath;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 我们建议按照   ftp://168.160.76.102/年/月/日/pk值/xxxxxxxx.pdf pk值为数据写入到dgate_document_info表里字段pk的值
        /// </summary>
        /// <param name="fileURL"></param>
        /// <param name="fileName"></param>
        /// <param name="ftpServerIP"></param>
        /// <param name="srvPath">ftp 服务器</param>
        /// <param name="ftpUserID"></param>
        /// <param name="ftpPassword"></param>
        /// <returns></returns>
        public static int UpLoadReport(string fileURL, string fileName, string ftpServerIP, string srvPath, string ftpUserID, string ftpPassword, ref string url)
        {
            try
            {

                path = srvPath; // 2018/01/18

                string fileNameTmp = System.IO.Path.GetDirectoryName(fileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(fileName) + ".pdf";
                client.DownloadFile(fileURL, fileNameTmp);

                // string ftpFilePath = string.Empty;
                Neusoft.FrameWork.Function.HisLog.WriteLog(Funcs.Function._logName, fileURL + fileNameTmp);
                int result = FtpFile.UploadFtp(path, fileNameTmp, ftpServerIP, ftpUserID, ftpPassword, ref url);

                if (result <= 0)
                {
                    return -1; ;
                }

                File.Delete(fileNameTmp);

                return 1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        /// <summary>
        /// 测试上传pdf 到平台 ftp 服务器
        /// </summary>
        /// <param name="localFile"></param>
        /// <param name="fileName"></param>
        /// <param name="ftpServerIP"></param>
        /// <param name="srvPath"></param>
        /// <param name="ftpUserID"></param>
        /// <param name="ftpPassword"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static int TestUpload(string localFile, string fileName, string ftpServerIP, string srvPath, string ftpUserID, string ftpPassword, ref string url)
        {
            path = srvPath;
            int result = FtpFile.UploadFtp(path, localFile, ftpServerIP, ftpUserID, ftpPassword, ref url);
            return result;
        }

        /// <summary>
        /// 下载pdf
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <param name="localPath"></param>
        /// <returns></returns>
        public static int DownLoadFtp(string url, string fileName, ref string localPath)
        {
            using (WebClient ftpClient = new WebClient())
            {
                try
                {

                    string basePath = AppDomain.CurrentDomain.BaseDirectory + "\\OutPath\\Download\\";
                    if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

                    if (string.IsNullOrEmpty(fileName))
                    {
                        fileName = "ECG" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                    }
                    string fileNameTmp = basePath + System.IO.Path.GetFileNameWithoutExtension(fileName) + ".pdf";
                    Neusoft.FrameWork.Function.HisLog.WriteLog("worker", url + "||" + fileName);
                    ftpClient.DownloadFile(url, fileNameTmp);
                    localPath = fileNameTmp;
                    return 1;
                }
                catch (Exception ex)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog("worker", url + "||" + fileName);
                    Neusoft.FrameWork.Function.HisLog.WriteLog("worker", ex.Message + ex.StackTrace);

                    return -1;
                }
            }
        }

        /// <summary>
        /// Download a pdf file to .\OutPath\Download\ path, via ftp.
        /// </summary>
        /// <returns>null if dowload failed, otherwise the full file name with GUID formating.</returns>
        public static string DownLoadFtp(string url)
        {
            using (WebClient ftpClient = new WebClient())
            {
                try
                {

                    string basePath = AppDomain.CurrentDomain.BaseDirectory + "\\OutPath\\Download\\";
                    if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
                    
                    string fileName = basePath + Guid.NewGuid().ToString() + ".pdf";
                    
                    ftpClient.DownloadFile(url, fileName);

                    return fileName;
                }
                catch (Exception ex)
                {
                    Neusoft.FrameWork.Function.HisLog.WriteLog("error", string.Format("error on ftp download. url: {0}, exception: {1}", url, ex.ToString()));

                    return null;
                }
            }
        }
    }


}
