using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WinSCP;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Web;
using System.Runtime.InteropServices;
using System.Threading;

namespace Financial_Journal
{
    class Cloud_Services
    {
        private static string ftpUsername = "Guest";
        private static string ftpPassword = "";
        public static string localSavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Personal Banker";
        

        /// <summary>
        /// Download Cloud Synced file based on filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>bool uploadFail</returns>
        public static bool FTP_Upload_Synced(string ftpPath, string fileName)
        {
            Diagnostics.WriteLine("FTP Download start at " + DateTime.Now.TimeOfDay + " (path=" + ftpPath + ")");

            TimeSpan Start_Time = DateTime.Now.TimeOfDay;

            bool Upload_Fail = false;

            // Login log to FTP server
            try
            {

                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(ftpPath);
                requestDir.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                requestDir.Method = WebRequestMethods.Ftp.UploadFile;

                try
                {
                    // Copy the contents of the file to the request stream.  
                    StreamReader sourceStream = new StreamReader(Path.Combine(localSavePath, fileName));
                    byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                    sourceStream.Close();
                    requestDir.ContentLength = fileContents.Length;

                    Stream requestStream = requestDir.GetRequestStream();
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();

                    FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse();

                    Diagnostics.WriteLine("Upload File Complete, status {0}", response.StatusDescription);  

                    response.Close();
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ez)
            {
                Diagnostics.WriteLine("FTP ERROR : " + ez.ToString());
                // FTP Error
            }

            Diagnostics.WriteLine("FTP Download Thread end at " + DateTime.Now.TimeOfDay);

            return Upload_Fail;
        }

        /// <summary>
        /// Download Cloud Synced file based on filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>bool downloadFail</returns>
        public static string FTP_Read_Cloud(string ftpPath)
        {
            Diagnostics.WriteLine("FTP Reading Cloud start at " + DateTime.Now.TimeOfDay);

            TimeSpan Start_Time = DateTime.Now.TimeOfDay;

            // Login log to FTP server
            try
            {
                if (FTP_Check_File_Exists(ftpPath))
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                        // try to download log from client
                        try
                        {
                            byte[] newFileData = client.DownloadData(ftpPath);
                            string fileString = System.Text.Encoding.UTF8.GetString(newFileData);
                            return fileString;
                        }
                        catch (Exception ez)
                        {
                            Diagnostics.WriteLine("FTP ERROR : " + ez.ToString());
                            return "";
                        }
                    }
                }
            }
            catch (Exception ez)
            {
                Diagnostics.WriteLine("FTP ERROR : " + ez.ToString());
                return "";
                // FTP Error
            }

            Diagnostics.WriteLine("FTP Read End Thread end at " + DateTime.Now.TimeOfDay);

            return "";
        }

        /// <summary>
        /// Delete file with path ftpPath
        /// </summary>
        /// <param name="ftpPath"></param>
        /// <returns>whether or not delete happened</returns>
        public static bool FTP_Delete_Cloud(string ftpPath)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);

                //If you need to use network credentials
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                //additionally, if you want to use the current user's network credentials, just use:
                //System.Net.CredentialCache.DefaultNetworkCredentials

                request.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Diagnostics.WriteLine("Delete status: {0}", response.StatusDescription);
                response.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if file exists
        /// </summary>
        /// <param name="ftpPath"></param>
        /// <returns></returns>
        public static bool FTP_Check_File_Exists(string ftpPath)
        {

            #region Check if FTP Path exists
            var request = (FtpWebRequest)WebRequest.Create(ftpPath);
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response != null && response.StatusCode ==
                    FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }
            }

            return true;
            #endregion
        }

        public static List<string> FTP_List_Files(bool getLatestFileOnly = false, string fileNameComparison = "")
        {
            List<string> fileList = new List<string>();

            // Create WinSCP session
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = "robinli.asuscomm.com",
                UserName = ftpUsername,
                Password = ftpPassword
            };

            using (Session session = new Session())
            {
                try
                {
                    // Connect
                    session.Open(sessionOptions);

                    // Remove subdirectories using WinSCP flagging
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.FileMask = "|*/";

                    // Create local directory
                    if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Personal Banker"))
                    {
                        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Personal Banker");
                    }

                    // Set local path
                    string localTempPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Personal Banker";

                    // Download files
                    session.GetFiles("/Seagate_Backup_Plus_Drive/Personal Banker/Cloud_Sync/*", localTempPath + @"\*", false, transferOptions).Check();
                    
                    // Get list of all files  
                    fileList = Directory.GetFiles(localTempPath).ToList();

                    // Delete local files
                    fileList.ForEach(x => File.Delete(x));

                    fileList = fileList.OrderByDescending(x => File.GetLastWriteTime(x)).ToList();

                    // Get latest .cfg file
                    if (getLatestFileOnly && fileNameComparison.Length > 2)
                    {
                        for (int i = 0; i < fileList.Count; i++)
                        {
                            if (fileList[i].ToLower().Contains(fileNameComparison.ToLower()))
                            {
                                return new List<string>() { fileList[i] }; // return single file
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Diagnostics.WriteLine(e.ToString());
                }
            }

            if (getLatestFileOnly && fileNameComparison.Length > 2)
                return new List<string>();
            else
                return fileList;
        }


    }
}
