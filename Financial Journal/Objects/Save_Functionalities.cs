using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Objects;

namespace Financial_Journal
{
    public class Save_Functionalities
    {
        Receipt parent;
        public string Current_Serial_Hash_Value = "";
        public string Serial_Hash_Value = "";

        bool multiThreadLoad = true;

        // Using root function above, create more focused saving behaviours
        public int Save_Entries = 7;

        // Create respective backup files
        public void Save_Backup()
        {
            if (parent.Saving_In_Process) return;

            string backup_path = parent.localSavePath + "\\Backups";

            if (!Directory.Exists(backup_path)) Directory.CreateDirectory(backup_path);

            // Create mapping file
            string Combination_File_Name = backup_path + "\\personal_banker_backup_" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + ".cfg";

            // Delete existing mapping file
            File.Delete(Combination_File_Name);

            // Hash next serial value
            Serial_Hash_Value = Get_Next_Random() + Get_Next_Random() + Get_Next_Random() + Get_Next_Random() + Get_Next_Random() + Get_Next_Random();


            if (false && multiThreadLoad) // no multithreadubg for backup saving (for collision-proof back up)
            {
                Enumerable.Range(1, parent.SaveHelper.Save_Entries).Select(i =>
                {
                    Thread t = new Thread(() =>
                    {
                        Save_Section(backup_path, i, Serial_Hash_Value, false, true);
                    });
                    t.Start();
                    return t;
                }).ToList().ForEach(x => x.Join());
            }
            else
            {
                // Save all sections
                for (int i = 1; i <= Save_Entries; i++)
                {
                    Diagnostics.WriteLine(String.Format("[BACKUP] Saving with serial '{0}'", Serial_Hash_Value));
                    Save_Section(backup_path, i, Serial_Hash_Value, false, true);
                }
            }


            // Create new mapping file with new serial attached
            using (StreamWriter sw = File.CreateText(Combination_File_Name)) //
            {
                sw.Write(AESGCM.SimpleEncryptWithPassword((Serial_Hash_Value.Trim('_') + Environment.NewLine +
                    (parent.Settings_Dictionary.ContainsKey("AUTHENTICATION_REQ") && parent.Settings_Dictionary["AUTHENTICATION_REQ"] == "1" ? "PASSWORD_REQUIRED" : "")), "PASSWORDisHERE"));
                sw.Close();
            }

        }

        // Export information
        public void Export_Data(string path)
        {
            string userName = Environment.UserName;
            string Info_Path = path + "\\" + userName;

            int check_index = 1;
            while (Directory.Exists(Info_Path))
            {
                check_index++;
                Info_Path = path + "\\" + userName + "_" + check_index.ToString();
            }

            // Create available directory
            Check_Create_Directory(Info_Path);

            // Hash next serial value
            string int_Serial_Hash_Value = Get_Next_Random() + Get_Next_Random() + Get_Next_Random() + Get_Next_Random() + Get_Next_Random() + Get_Next_Random();

            // Create new mapping file with new serial attached
            using (StreamWriter sw = File.CreateText(Info_Path + "\\Personal_Banker.cfg")) //
            {
                sw.Write(AESGCM.SimpleEncryptWithPassword((int_Serial_Hash_Value.Trim('_') + Environment.NewLine +
                    (parent.Settings_Dictionary.ContainsKey("AUTHENTICATION_REQ") && parent.Settings_Dictionary["AUTHENTICATION_REQ"] == "1" ? "PASSWORD_REQUIRED" : "")), "PASSWORDisHERE"));
                sw.Close();
            }



            if (multiThreadLoad)
            {
                Enumerable.Range(1, parent.SaveHelper.Save_Entries).Select(i =>
                {
                    Diagnostics.WriteLine(String.Format("[EXPORT_SAVE] Saving with serial '{0}'", Serial_Hash_Value));
                    Thread t = new Thread(() =>
                    {
                        Save_Section(Info_Path, i, int_Serial_Hash_Value, false, false);
                    });
                    t.Start();
                    return t;
                }).ToList().ForEach(x => x.Join());
            }
            else
            {
                // Save all sections
                for (int i = 1; i <= Save_Entries; i++)
                {
                    Save_Section(Info_Path, i, int_Serial_Hash_Value, false, false);
                }
            }
            /*
            for (int i = 1; i <= Save_Entries; i++)
            {

                Save_Section(Info_Path, i, int_Serial_Hash_Value, false, false);
            }
            */
        }

        // Regular save using current directory path
        public void Regular_Save(bool isBGSave = false, bool isCloudSync = false, bool multiThreadSave = true)
        {
            // Prevent redundant saves
            if (!parent.Saving_In_Process || isBGSave)
            {
                string current_path = parent.localSavePath + "\\";

                // Create mapping file
                string Combination_File_Name = current_path + "Personal_Banker.cfg";

                // Delete existing mapping file
                deleteOldLocalRecordPaths.Add(Combination_File_Name);
                //File.Delete(Combination_File_Name);

                // Hash next serial value
                Serial_Hash_Value = Get_Next_Random() + Get_Next_Random() + Get_Next_Random() + Get_Next_Random() + Get_Next_Random() + Get_Next_Random();

                string previousCloudSerial = "";

                // if cloud sync, get original serial
                if (isCloudSync)
                {
                    var text = "";
                    string ftpPath = @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/Cloud_Sync/" + parent.Settings_Dictionary["LOGIN_EMAIL"] + "_" + parent.Settings_Dictionary["UNIQUE_IDENTIFIER"] + ".cfg";
                    text = Cloud_Services.FTP_Read_Cloud(ftpPath);

                    if (text.Length > 0)
                    {
                        string[] temp = AESGCM.SimpleDecryptWithPassword(text, "PASSWORDisHERE").Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        previousCloudSerial = temp[0];
                    }
                }



                // Existing files in path
                Check_Delete_Files(current_path + "1\\");
                Check_Delete_Files(current_path + "2\\");
                Check_Delete_Files(current_path + "3\\");
                Check_Delete_Files(current_path + "4\\");
                Check_Delete_Files(current_path + "5\\");
                Check_Delete_Files(current_path + "6\\");
                Check_Delete_Files(current_path + "7\\");

                int error_level = 0;

                //for (int i = 1; i <= Save_Entries; i++)
                //{
                //    error_level += Save_Section(current_path, i, Serial_Hash_Value, true, false, isCloudSync, previousCloudSerial);
                //}

                DateTime startTime = DateTime.Now;

                if (multiThreadSave)
                {
                    Enumerable.Range(1, parent.SaveHelper.Save_Entries).Select(i =>
                    {
                        Diagnostics.WriteLine(String.Format("[REGULAR_SAVE] Saving with serial '{0}'", Serial_Hash_Value));
                        Thread t = new Thread(() =>
                        {
                            error_level += Save_Section(current_path, i, Serial_Hash_Value, true, false, isCloudSync,
                                previousCloudSerial);
                        });
                        t.Start();
                        return t;
                    }).ToList().ForEach(x => x.Join());
                }
                else
                {
                    // Save all sections
                    for (int i = 1; i <= Save_Entries; i++)
                    {
                        error_level += Save_Section(current_path, i, Serial_Hash_Value, true, false, isCloudSync, previousCloudSerial);
                    }
                }

                TimeSpan span = DateTime.Now - startTime;
                int ms = (int)span.TotalMilliseconds;
                Diagnostics.WriteLine(String.Format("Save time: {0}ms", ms));

                // If any errors, revert to old
                if (error_level == 0)
                {
                    // Force update on current hash value
                    Current_Serial_Hash_Value = Serial_Hash_Value;

                    // Delete old files when successfully created ALL old files; else revert (Revert as in do not update to current values)
                    DeleteOldPaths();

                    // Create new mapping file with new serial attached
                    using (StreamWriter sw = File.CreateText(Combination_File_Name)) //
                    {
                        sw.Write(AESGCM.SimpleEncryptWithPassword((Serial_Hash_Value.Trim('_') + Environment.NewLine +
                                                                   (parent.Settings_Dictionary.ContainsKey("AUTHENTICATION_REQ") && parent.Settings_Dictionary["AUTHENTICATION_REQ"] == "1" ? "PASSWORD_REQUIRED" : "")), "PASSWORDisHERE"));
                        sw.Close();
                    }

                    // Create mapping file on FTP
                    if (isCloudSync)
                    {
                        string ftpPath = @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/Cloud_Sync/" + parent.Settings_Dictionary["LOGIN_EMAIL"] + "_" + parent.Settings_Dictionary["UNIQUE_IDENTIFIER"] + ".cfg";
                        Cloud_Services.FTP_Upload_Synced(ftpPath, Combination_File_Name);
                    }
                }
                else
                {
                    // If error, delete newly created files
                    DeleteNewPaths();
                }
            }

            // Let the file sit and system process complete
            if (isBGSave) System.Threading.Thread.Sleep(250);
        }

        private void Check_Delete_Files(string path)
        {
            if (Directory.Exists(path))
            {
                string[] File_in_dir = Directory.GetFiles(path, "*");
                foreach (string file in File_in_dir)
                {
                    try
                    {
                        deleteOldLocalRecordPaths.Add(file);
                        //File.Delete(file);
                    }
                    catch
                    {
                        //error deleting file
                    }
                }
            }
        }

        private Dictionary<int, bool> checkSavingSection = new Dictionary<int, bool>();

        static Saver staticSaver = new Saver();

        /// <summary>
        /// Create directories in root path and create the associated file for the backup. If file path exists, just dump file instead
        /// </summary>
        /// <param name="root_path"> root path, creates aggregate sub folders</param>
        /// <param name="group_ID"></param>
        public int Save_Section(string root_path, int group_ID, string hash_serial_value, bool delete_existing = false, bool isBackUp = false, bool isCloudSync = false, string oldSerial = "")
        {
            if (checkSavingSection[group_ID]) return 1;

            // Set to currently checking
            checkSavingSection[group_ID] = true;
             
            try
            {
                string save_path = root_path + "\\";

                if (save_path.EndsWith(@"\\\\")) save_path = save_path.Substring(0, save_path.Length - 2);

                // Delete older files using current hash values before trying to save new files
                if (!isBackUp || delete_existing)
                {

                    /*
                    Delete_File(save_path + "settings_files\\" + Current_Serial_Hash_Value);
                    Delete_File(save_path + "links_tax_files\\" + Current_Serial_Hash_Value);
                    Delete_File(save_path + "items_files\\" + Current_Serial_Hash_Value);
                    Delete_File(save_path + "hobby_files\\" + Current_Serial_Hash_Value);
                    Delete_File(save_path + "agenda_files\\" + Current_Serial_Hash_Value);
                    Delete_File(save_path + "calendar_files\\" + Current_Serial_Hash_Value);
                    Delete_File(save_path + "expenses_rp_files\\" + Current_Serial_Hash_Value);
                    */
                }

                switch (group_ID)
                {
                    case 1:
                        Check_Create_Directory(save_path + "1\\");
                        save_path += "1\\" + hash_serial_value;
                        break;
                    case 2:
                        Check_Create_Directory(save_path + "2\\");
                        save_path += "2\\" + hash_serial_value;
                        break;
                    case 3:
                        Check_Create_Directory(save_path + "3\\");
                        save_path += "3\\" + hash_serial_value;
                        break;
                    case 4:
                        Check_Create_Directory(save_path + "4\\");
                        save_path += "4\\" + hash_serial_value;
                        break;
                    case 5:
                        Check_Create_Directory(save_path + "5\\");
                        save_path += "5\\" + hash_serial_value;
                        break;
                    case 6:
                        Check_Create_Directory(save_path + "6\\");
                        save_path += "6\\" + hash_serial_value;
                        break;
                    case 7:
                        Check_Create_Directory(save_path + "7\\");
                        save_path += "7\\" + hash_serial_value;
                        break;
                }


                try
                {
                    File.Delete(save_path);
                    deleteNewLocalRecordPaths.Add(save_path);
                    //using (StreamWriter sw = File.CreateText(save_path))
                    //{
                    //
                    //   sw.Write(Get_Save_Lines(group_ID, true, hash_serial_value) + Environment.NewLine);
                    //    sw.Close();
                    //}

                    staticSaver.Save(Get_Save_Lines(group_ID, true, hash_serial_value) + Environment.NewLine, save_path);

                    if (isCloudSync)
                    {
                        // delete old sync
                        string ftpPath =
                            @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/Cloud_Sync/" +
                            group_ID + "\\" + oldSerial;

                        deleteOldFTPRecordPaths.Add(ftpPath);
                        //Cloud_Services.FTP_Delete_Cloud(ftpPath);
                        ftpPath =
                            @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/Cloud_Sync/" +
                            group_ID + "\\" + hash_serial_value;
                        Cloud_Services.FTP_Upload_Synced(ftpPath, save_path);
                        deleteNewFTPRecordPaths.Add(ftpPath);
                    }
                }
                catch (Exception ex)
                {
                    Diagnostics.WriteLine("Save failed: " + ex);
                    return 10; // return and process old style (not working)
                }
                checkSavingSection[group_ID] = false;
                return 0;
            }
            catch (Exception ex)
            {
                Diagnostics.WriteLine("Error: " + ex);
                return 1;
            }
        }

        public void DeleteOldPaths()
        {

            foreach (string path in deleteOldLocalRecordPaths)
            {
                File.Delete(path);
            }

            foreach (string path in deleteOldFTPRecordPaths)
            {
                Cloud_Services.FTP_Delete_Cloud(path);
            }

            deleteOldLocalRecordPaths = new List<string>();
            deleteOldFTPRecordPaths = new List<string>();
        }

        public void DeleteNewPaths()
        {

            foreach (string path in deleteNewLocalRecordPaths)
            {
                File.Delete(path);
            }

            foreach (string path in deleteNewFTPRecordPaths)
            {
                Cloud_Services.FTP_Delete_Cloud(path);
            }

            deleteNewLocalRecordPaths = new List<string>();
            deleteNewFTPRecordPaths = new List<string>();
        }

        // Olds
        private List<string> deleteOldLocalRecordPaths = new List<string>();
        private List<string> deleteOldFTPRecordPaths = new List<string>();

        // News
        private List<string> deleteNewLocalRecordPaths = new List<string>();
        private List<string> deleteNewFTPRecordPaths = new List<string>();

        public Save_Functionalities(Receipt _parent)
        {
            parent = _parent;

            checkSavingSection = new Dictionary<int, bool>();
            for (int i = 1; i <= Save_Entries; i++)
                checkSavingSection.Add(i, false);
        }

        // Check if directory exists, if not create it
        private void Check_Create_Directory(string directory_path)
        {
            if (!Directory.Exists(directory_path))
            {
                Directory.CreateDirectory(directory_path);
            }
        }

        // Delete with exception
        private void Delete_File(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        private List<string> Random_Numbers = new List<string>();
        Random r = new Random();

        // Return next random serial segment
        public string Get_Next_Random(List<string> collisionHash = null)
        {
            List<string> ref_List = collisionHash != null ? collisionHash : Random_Numbers;
            // Seed on initilization
            int randNum = r.Next(100000000, 999999999);
            while (ref_List.Contains(randNum.ToString()))
            {
                Diagnostics.WriteLine("Number-Generator Collision found");
                ref_List.Add(randNum.ToString());
                randNum = r.Next(100000000, 999999999);
            }
            return randNum.ToString();
        }

        public void Save_All_Sections(string save_path)
        {
            try
            {
                File.Delete(save_path);
                using (StreamWriter sw = File.CreateText(save_path)) // Create translator file
                {
                    string ongoing = "";
                    for (int i = 1; i <= Save_Entries; i++)
                    {
                        ongoing += Get_Save_Lines(i, false) + Environment.NewLine;
                    }
                    sw.Write(ongoing);
                    sw.Close();
                }
            }
            catch
            {
                // Save failed
            }
        }

        // Return respective save lines
        private string Get_Save_Lines(int group_ID, bool encrypt = true, string hashvalue = "")
        {
            StringBuilder line = new StringBuilder(hashvalue + Environment.NewLine);

            switch (group_ID)
            {
                case 1:
                    line.Append(Save_Settings());
                    line.Append(Save_Manual_Income());
                    line.Append(Save_Warnings());
                    line.Append(Save_Tiers());
                    line.Append(Save_Savings());
                    line.Append(Save_SMS_Alerts());
                    line.Append(Save_Sync_Associations());
                    break;
                case 2:
                    line.Append(Save_Links());
                    line.Append(Save_Tax_Info());
                    line.Append(Save_Contacts());
                    line.Append(Save_Expirations());
                    line.Append(Save_Asset_Info());
                    line.Append(Save_Category_Grouping());
                    break;
                case 3:
                    line.Append(Save_Item_Information());
                    line.Append(Save_Orders());
                    break;
                case 4:
                    line.Append(Save_Hobby_Information());
                    break;
                case 5:
                    line.Append(Save_Agenda());
                    line.Append(Save_Tracking());
                    break;
                case 6:
                    line.Append(Save_Calendar_Events());
                    line.Append(Save_Budget_Allocations());
                    break;
                case 7: 
                    line.Append(Save_Expenses());
                    line.Append(Save_GC());
                    line.Append(Save_Accounts());
                    line.Append(Save_Payments());
                    line.Append(Save_Payment_Options());
                    line.Append(Save_Investments());
                    line.Append(Save_Cash_History());
                    break;
                default:
                    /*          REMEMBER TO ADD TO INDEX ABOVE!!!!!!!!!!
                     *          REMEMBER TO ADD TO INDEX ABOVE!!!!!!!!!! 
                     *          REMEMBER TO ADD TO INDEX ABOVE!!!!!!!!!!
                     *          REMEMBER TO ADD TO INDEX ABOVE!!!!!!!!!! 
                     *          IN SAVE_ALL_SECTIONS()
                     */
                    return "";
            }
            line.Append(hashvalue + Environment.NewLine);

            return encrypt ? line.Length > 0 ? AESGCM.SimpleEncryptWithPassword(line.ToString(), "PASSWORDisHERE") : line.ToString() : line.ToString();
        }


        // This method accepts two strings the represent two files to 
        // compare. A return value of 1 indicates that the contents of the files
        // are the same. A return value of any other value indicates that the
        // files are not the same.
        private bool FileCompare(string file1, string file2)
        {
            try
            {
                string[] lines = new string[] { };
                var text = File.ReadAllText(file1).Trim();
                if (text.Length > 0)
                {
                    lines = AESGCM.SimpleDecryptWithPassword(text, "PASSWORDisHERE").Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    text = String.Join(Environment.NewLine, lines);
                }
                var text2 = File.ReadAllText(file2).Trim();
                if (text2.Length > 0)
                {
                    lines = AESGCM.SimpleDecryptWithPassword(text2, "PASSWORDisHERE").Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    text2 = String.Join(Environment.NewLine, lines);
                }
                if (text.Trim() == text2.Trim()) return true;
                return false;
            }
            catch
            {
                // Comparison fail
                Diagnostics.WriteLine("Comparison failed");
                return false;
            }
        }

        public bool Check_Difference()
        {
            for (int i = 1; i <= Save_Entries; i++)
            {
                if (!Check_Group_Difference(parent.localSavePath, i, Current_Serial_Hash_Value)) return false;
            }
            return true;
        }

        /// <summary>
        /// Create directories in root path and create the associated file for the backup. If file path exists, just dump file instead
        /// </summary>
        /// <param name="root_path"> root path, creates aggregate sub folders</param>
        /// <param name="group_ID"></param>
        public bool Check_Group_Difference(string root_path, int group_ID, string hash_serial_value, bool delete_existing = false, bool isBackUp = false)
        {
            string save_path = root_path + "\\";

            switch (group_ID)
            {
                case 1:
                    Check_Create_Directory(save_path + "1\\");
                    save_path += "1\\" + Current_Serial_Hash_Value + "_temp";
                    break;
                case 2:
                    Check_Create_Directory(save_path + "2\\");
                    save_path += "2\\" + Current_Serial_Hash_Value + "_temp";
                    break;
                case 3:
                    Check_Create_Directory(save_path + "3\\");
                    save_path += "3\\" + Current_Serial_Hash_Value + "_temp";
                    break;
                case 4:
                    Check_Create_Directory(save_path + "4\\");
                    save_path += "4\\" + Current_Serial_Hash_Value + "_temp";
                    break;
                case 5:
                    Check_Create_Directory(save_path + "5\\");
                    save_path += "5\\" + Current_Serial_Hash_Value + "_temp";
                    break;
                case 6:
                    Check_Create_Directory(save_path + "6\\");
                    save_path += "6\\" + Current_Serial_Hash_Value + "_temp";
                    break;
                case 7:
                    Check_Create_Directory(save_path + "7\\");
                    save_path += "7\\" + Current_Serial_Hash_Value + "_temp";
                    break;
            }


            try
            {
                File.Delete(save_path);
                string Lines = Get_Save_Lines(group_ID, true, Current_Serial_Hash_Value);
                using (StreamWriter sw = File.CreateText(save_path))
                {
                    sw.Write(Lines.Length > 0 ? Lines + Environment.NewLine : "");
                    sw.Close();
                }
            }
            catch
            {
                // Save failed
            }

            return FileCompare(save_path.Substring(0, save_path.IndexOf("_temp")), save_path);
        }

        public void Delete_Temps(string root_path)
        {
            string save_path = root_path + "\\";
            Delete_File(save_path + "1\\" + Current_Serial_Hash_Value + "_temp");
            Delete_File(save_path + "2\\" + Current_Serial_Hash_Value + "_temp");
            Delete_File(save_path + "3\\" + Current_Serial_Hash_Value + "_temp");
            Delete_File(save_path + "4\\" + Current_Serial_Hash_Value + "_temp");
            Delete_File(save_path + "5\\" + Current_Serial_Hash_Value + "_temp");
            Delete_File(save_path + "6\\" + Current_Serial_Hash_Value + "_temp");
            Delete_File(save_path + "7\\" + Current_Serial_Hash_Value + "_temp");
            Delete_File(save_path + "temp.txt");
        }


        #region Save sections

        public StringBuilder Save_Payments()
        {
            StringBuilder line = new StringBuilder();
            //StringBuilder line = new StringBuilder();
            foreach (Payment payment in parent.Payment_List)
            {
                line.Append("[PAYMENT_TYPE]=" + payment.Payment_Type +
                            "||[LAST_FOUR]=" + payment.Last_Four +
                            "||[COMPANY]=" + payment.Company +
                            "||[BANK_NAME]=" + payment.Bank +
                            "||[BALANCE]=" + payment.Balance +
                            "||[CARD_LIMIT]=" + payment.Limit +
                            "||[BILLING_START]=" + payment.Billing_Start +
                            "||[EMERGENCY_NO]=" + payment.Emergency_No +
                            "||[LAST_UPDATE_DATE]=" + payment.Last_Reset_Date.ToShortDateString() +
                            "||[ALERT_A]=" + (payment.Alerts[0].Active ? "1:" : "0:") +
                            (payment.Alerts[0].Repeat ? "1" : "0") +
                            "||[ALERT_B]=" + (payment.Alerts[1].Active ? "1:" : "0:") +
                            (payment.Alerts[1].Repeat ? "1" : "0") +
                            "||[ALERT_C]=" + (payment.Alerts[2].Active ? "1:" : "0:") +
                            (payment.Alerts[2].Repeat ? "1" : "0") +
                            "||[ALERT_D]=" + (payment.Alerts[3].Active ? "1:" : "0:") +
                            (payment.Alerts[3].Repeat ? "1" : "0") + Environment.NewLine);
            }
            return line;
        }

        public StringBuilder Save_GC()
        {
            StringBuilder line = new StringBuilder();
            foreach (GC GCard in parent.GC_List)
            {
                line.Append("[GC_AMOUNT]=" + GCard.Amount +
                            "||[GC_ASC_ORD]=" + String.Join(",", GCard.Associated_Orders) +
                            "||[GC_LAST_FOUR]=" + GCard.Last_Four +
                            //"||[GC_DATE_ADDED]=" + (GCard.Amount == 0 ? DateTime.Now.AddYears(-1).ToShortDateString() : GCard.Date_Added.ToShortDateString()) +
                            "||[GC_DATE_ADDED]=" + GCard.Date_Added.ToShortDateString() +
                            "||[GC_LOCATION]=" + GCard.Location + Environment.NewLine);
            }

            return line;
        }

        public StringBuilder Save_Agenda()
        {
            StringBuilder line = new StringBuilder();

            if (parent.Agenda_Item_List.Count > 0)
            {
                foreach (Agenda_Item AI in parent.Agenda_Item_List)
                {
                    line.Append("[AGENDA_ITEM]" +
                                "||[A_NAME]=" + AI.Name +
                                "||[A_DATE]=" + AI.Date.ToShortDateString() +
                                "||[HASH_VALUE]=" + AI.Hash_Value +
                                "||[C_HASH_VALUE]=" + AI.Contact_Hash_Value +
                                "||[A_ID]=" + AI.ID.ToString() +
                                "||[TIME_SET]=" + ((AI.Time_Set) ? "1" : "0") +
                                "||[A_CALENDAR_DATE]=" + AI.Calendar_Date.ToString() +
                                "||[A_CHECK_STATE]=" + ((AI.Check_State) ? "1" : "0") + Environment.NewLine);

                    foreach (Shopping_Item SI in AI.Shopping_List)
                    {
                        line.Append("[SHOPPING_ITEM]" +
                                    "||[S_NAME]=" + SI.Name +
                                    "||[HASH_VALUE]=" + SI.Hash_Value +
                                    "||[C_HASH_VALUE]=" + SI.Contact_Hash_Value +
                                    "||[S_CHECK_STATE]=" + ((SI.Check_State) ? "1" : "0") +
                                    "||[S_DATE]=" + SI.Calendar_Date.ToString() +
                                    "||[TIME_SET]=" + ((SI.Time_Set) ? "1" : "0") +
                                    "||[S_ID]=" + AI.ID.ToString() + Environment.NewLine);
                    }
                }
            }
            return line;
        }

        public StringBuilder Save_Hobby_Information()
        {
            StringBuilder line = new StringBuilder();
            // Save hobby items
            foreach (Hobby_Item HI in parent.Master_Hobby_Item_List)
            {
                line.Append("[HO_NA_]=" + HI.Name +
                            "||[HO_CA_]=" + HI.Category +
                            "||[HO_ID_]=" + HI.OrderID +
                            "||[HO_PR_]=" + HI.Price.ToString() +
                            "||[HO_CO_]=" + HI.Container_ID.ToString() +
                            "||[HO_PN_]=" + (HI.Profile_Number == "" || HI.Profile_Number.Length > 3 ? "1" : HI.Profile_Number) +
                            "||[HO_UN_]=" + HI.Unique_ID + Environment.NewLine);
            }

            if (parent.Master_Hobby_Item_List.Count == 0)
                return line;

            foreach (KeyValuePair<string, List<Container>> KVP in parent.Master_Container_Dict)
            {
                line.Append("[CONTAINER_PROFILE_NUMBER]=" + (KVP.Key == "" ? "1" : KVP.Key) + "||");
                foreach (Container c in KVP.Value)
                {
                    line.Append("[CONTAINER_NAME" + c.ID + "]=" + c.Name + "||");
                }
                line.Length -= 2; // remove last 2
                line.Append(Environment.NewLine);
            }

            int count = 1;
            foreach (string c in parent.Hobby_Profile_List)
            {
                line.Append("[HOBBY_PROFILE" + count++ + "]=" + c + "||");
            }

            // Save container information (realistically only the title since ID is sequential)
            line.Length -= 2; // remove last 22
            line.Append(Environment.NewLine);

            return line;
        }

        public StringBuilder Save_Payment_Options()
        {
            StringBuilder line = new StringBuilder();
            foreach (Payment_Options payment in parent.Payment_Options_List)
            {
                line.Append("[PO_TY_]=" + payment.Type +
                            "||[PO_LA_]=" + payment.Payment_Last_Four +
                            "||[PO_CO_]=" + payment.Payment_Company +
                            "||[PO_BA_]=" + payment.Payment_Bank +
                            "||[PO_EN_]=" + payment.Ending_Balance +
                            "||[PO_HI_]=" + payment.Hidden_Note +
                            "||[PO_DA_]=" + payment.Date.ToString() +
                            "||[PO_NO_]=" + payment.Note +
                            "||[PO_AM_]=" + payment.Amount + Environment.NewLine);
            }

            return line;
        }

        // Save items from Master Item List
        public StringBuilder Save_Item_Information()
        {
            StringBuilder line = new StringBuilder();
            foreach (Item item in parent.Master_Item_List.OrderBy(x => x.Date))
            {
                line.Append("[IT_DE_]=" + item.Name +
                            "||[IT_LO]=" + item.Location +
                            "||[IT_ST_]=" + item.Status +
                            "||[IT_CA_]=" + item.Category +
                            "||[IT_QU_]=" + item.Quantity +
                            "||[IT_PR_]=" + item.Price +
                            "||[IT_DI_]=" + item.Discount_Amt +
                            "||[IT_DA_]=" + item.Date.ToString() +
                            "||[IT_RD_]=" + item.Refund_Date.ToString() +
                            "||[IT_PA_]=" + item.Payment_Type +
                            "||[IT_ID_]=" + item.OrderID +
                            "||[IT_RE_]=" + (item.RefundAlert ? "1" : "0") +
                            "||[IT_CO_]=" + item.consumedStatus +
                            "||[IT_ME_]=" + item.Memo + Environment.NewLine);
            }
            return line;
        }

        public StringBuilder Save_Tiers()
        {
            if (parent.Tier_Format.Count > 0)
            {
                StringBuilder line =
                    new StringBuilder("[TIER_STRUCTURE]=" + String.Join(",", parent.Tier_Format) + Environment.NewLine);
                return line;
            }
            return new StringBuilder();
        }

        public StringBuilder Save_Links()
        {
            StringBuilder line = new StringBuilder();
            foreach (KeyValuePair<string, string> Key in parent.Link_Location)
            {
                if (parent.Location_List.Any(x => x.Name == Key.Key))
                    line.Append("[LINK_SOURCE]=" + Key.Key + "||[LINK_DESTINATION]=" + Key.Value + "||[REFUND_DAYS]=" + 
                        parent.Location_List.FirstOrDefault(x => x.Name == Key.Key).Refund_Days + Environment.NewLine);
            }
            return line;
        }

        public StringBuilder Save_Cash_History()
        {
            StringBuilder line = new StringBuilder();
            foreach (Objects.CashHistory CH in Cash.GetHistories())
            {
                line.Append("[CH_DA_]=" + CH.GetDate().ToShortDateString() +
                            "||[CH_ME_]=" + CH.GetMemo() +
                            "||[CH_NE_]=" + CH.GetAmount() +
                            "||[CH_ID_]=" + CH.GetID() + Environment.NewLine);
            }
            return line;
        }

        public StringBuilder Save_Contacts()
        {
            StringBuilder line = new StringBuilder();
            foreach (Contact C in parent.Contact_List)
            {
                line.Append("[CO_FI_]=" + C.First_Name +
                            "||[CO_LA_]=" + C.Last_Name +
                            "||[CO_EF_]=" + C.Email +
                            "||[CO_ES_]=" + C.Email_Second +
                            "||[CO_PR_]=" + C.Phone_No_Primary +
                            "||[CO_SE_]=" + C.Phone_No_Second +
                            "||[CO_AS_]=" + C.Association +
                            "||[CO_HA_]=" + C.Hash_Value + Environment.NewLine);
            }
            return line;
        }

        public StringBuilder Save_Expirations()
        {
            StringBuilder line = new StringBuilder();
            foreach (Expiration_Entry C in parent.Expiration_List)
            {
                line.Append("[EXPIRATION_NAME]=" + C.Item_Name +
                            "||[EXPIRATION_DAY_COUNT]=" + C.Exp_Date_Count +
                            "||[EXPIRATION_WARNING_COUNT]=" + C.Warning_Date_Count +
                            "||[EXPIRATION_LAST_WARN_DATE]=" + C.Last_Warn_Date.ToShortDateString() +
                            "||[EXPIRATION_LOCATION]=" + C.Location + Environment.NewLine);
            }
            return line;
        }

        public StringBuilder Save_Tax_Info()
        {
            StringBuilder line = new StringBuilder();
            foreach (KeyValuePair<string, string> Key in parent.Tax_Rules_Dictionary)
            {
                line.Append("[TAX_CATEGORY]=" + Key.Key + "||[TAX_RATE]=" + Key.Value + Environment.NewLine);
            }
            return line;
        }

        public StringBuilder Save_Settings()
        {
            StringBuilder line = new StringBuilder("[PERSONAL_SETTINGS]");
            foreach (KeyValuePair<string, string> Key in parent.Settings_Dictionary)
            {
                line.Append("||[" + Key.Key + "]=" + Key.Value);
            }
            line.Append(Environment.NewLine);
            return line;
        }

        public StringBuilder Save_Asset_Info()
        {
            StringBuilder line = new StringBuilder();
            foreach (Asset_Item Iv in parent.Asset_List)
            {
                line.Append("[AS_NA_]=" + Iv.Name);
                line.Append("||[AS_PR_]=" + Iv.Cost);
                line.Append("||[AS_SA_]=" + Iv.Selling_Amount);
                line.Append("||[AS_SE_]=" + Iv.Serial_Identification);
                line.Append("||[AS_ID_]=" + Iv.OrderID);
                line.Append("||[AS_NO_]=" + Iv.Note);
                line.Append("||[AS_CA_]=" + Iv.Asset_Category);
                line.Append("||[AS_LO_]=" + Iv.Purchase_Location);
                line.Append("||[AS_PU_]=" + Iv.Purchase_Date.ToShortDateString());
                line.Append("||[AS_RE_]=" + Iv.Remove_Date.ToShortDateString() + Environment.NewLine);
            }
            line.Append(Environment.NewLine);
            return line;
        }

        public StringBuilder Save_Category_Grouping()
        {
            StringBuilder line = new StringBuilder();
            foreach (GroupedCategory GC in parent.GroupedCategoryList)
            {
                line.Append("[CATEGORY_GROUP]||[PROF_NAME]=" + GC._ProfileName);
                line.Append("||[GRP_NAME]=" + GC._GroupName);
                line.Append("||[SUB_CATEGORIES]=" + String.Join("~", GC.SubCategoryList));
                line.Append("||[SUB_EXPENSES]=" + String.Join("~", GC.SubExpenseList) + Environment.NewLine);
            }
            line.Append(Environment.NewLine);
            return line;
        }

        public StringBuilder Save_SMS_Alerts()
        {
            StringBuilder line = new StringBuilder();
            foreach (SMSAlert SMSA in parent.SMSAlert_List)
            {
                line.Append("SMS_ALERT||[NAME]=" + SMSA.Name);
                line.Append("||[REPEAT]=" + (SMSA.Repeat ? "1" : "0"));
                line.Append("||[TIME]=" + SMSA.Time + Environment.NewLine);
            }
            line.Append(Environment.NewLine);
            return line;
        }

        public StringBuilder Save_Sync_Associations()
        {
            StringBuilder line = new StringBuilder();
            foreach (Association association in parent.AssociationList)
            {
                line.Append("SYNC_ASSOCIATION||[TYPE]=" + association.InfoType);
                line.Append("||[SOURCE]=" + association.LinkSource);
                line.Append("||[DESTINATION]=" + association.LinkDestination + Environment.NewLine);
            }
            line.Append(Environment.NewLine);
            return line;
        }

        public StringBuilder Save_Investments()
        {
            StringBuilder line = new StringBuilder();
            foreach (Investment Iv in parent.Investment_List)
            {
                line.Append("[INVESTMENT_NAME]=" + Iv.Name);
                line.Append("||[ACTIVE]=" + (Iv.Active ? "1" : "0"));
                line.Append("||[PRINCIPAL]=" + Iv.Principal);
                line.Append("||[IRATE]=" + Iv.IRate);
                line.Append("||[FREQUENCY]=" + Iv.Frequency);
                line.Append("||[START_DATE]=" + Iv.Start_Date.ToShortDateString());
                line.Append("||[SEQUENCE]=");
                Iv.Balance_Sequence.ForEach(x => line.Append(x.Date.ToShortDateString() + "," + x.Entry_No + "," + x.Principal_Carry_Over + ","));
                line.Length--; // remove last ,
                line.Append("||[END_DATE]=" + Iv.End_Date.ToShortDateString() + Environment.NewLine);
            }
            line.Append(Environment.NewLine);
            return line;
        }

        public StringBuilder Save_Manual_Income()
        {
            StringBuilder line = new StringBuilder();
            foreach (CustomIncome CI in parent.Income_Company_List)
            {
                line.Append("[MANUAL_INCOME_COMPANY]=" + CI.Company +
                            "||[MANUAL_FIRST_DATE]=" + CI.First_Period_Date.ToShortDateString() +
                            "||[MANUAL_STOP_DATE]=" + CI.Stop_Date.ToShortDateString() +
                            "||[MANUAL_FREQUENCY]=" + CI.Frequency +
                            "||[MANUAL_DEPOSIT_ACC]=" + CI.Deposit_Account +
                            "||[MANUAL_DEFAULT]=" + (CI.Default ? "1" : "0") +
                            "||[MANUAL_INTERVALS]=" + CI.Export_Intervals() + Environment.NewLine);
            }
            line.Append(Environment.NewLine);
            return line;
        }

        public StringBuilder Save_Savings()
        {
            StringBuilder line = new StringBuilder("[SAVINGS_SETTINGS]");
            line.Append("||[STRUCTURE]=" + parent.Savings.Structure);
            line.Append("||[AMOUNT]=" + parent.Savings.Ref_Value.ToString());
            line.Append("||[ALERT_1]=" + (parent.Savings.Alert_1 ? "1" : "0"));
            line.Append("||[ALERT_2]=" + (parent.Savings.Alert_2 ? "1" : "0"));
            line.Append("||[ALERT_3]=" + (parent.Savings.Alert_3 ? "1" : "0"));
            line.Append(Environment.NewLine);
            return line;
        }

        public StringBuilder Save_Calendar_Events()
        {
            StringBuilder line = new StringBuilder();
            foreach (Calendar_Events CE in parent.Calendar_Events_List)
            {
                line.Append("[CA_TI_]=" + CE.Title);
                line.Append("||[CA_AC_]=" + CE.Is_Active);
                line.Append("||[CA_HA_]=" + CE.Hash_Value);
                line.Append("||[CA_MU_]=" + CE.MultiDays);
                line.Append("||[CA_CO_]=" + CE.Contact_Hash_Value);
                line.Append("||[CA_DE_]=" + CE.Description.Replace(Environment.NewLine, "~~"));
                line.Append("||[CA_IM_]=" + CE.Importance.ToString());
                line.Append("||[CA_TI_]=" + (CE.Time_Set ? "1" : "0"));
                line.Append("||[CA_DA_]=" + CE.Date.ToString());
                if (CE.Alert_Dates.Count > 0)
                {
                    line.Append("||[CA_AL_]=");
                    line.Append(String.Join("~", CE.Alert_Dates));
                    /*
                    foreach (DateTime DT in CE.Alert_Dates.GetRange(1, CE.Alert_Dates.Count - 1))
                    {
                        line.Append("~" + DT.ToShortDateString() ;
                    }
                    */
                }
                line.Append(Environment.NewLine);
            }
            return line;
        }

        public StringBuilder Save_Expenses()
        {
            StringBuilder line = new StringBuilder();
            foreach (Expenses expense in parent.Expenses_List)
            {

                line.Append("[EXPENSE_TYPE]=" + expense.Expense_Type +
                            "||[EXPENSE_NAME]=" + expense.Expense_Name +
                            "||[EXPENSE_PAYEE]=" + expense.Expense_Payee +
                            "||[EXPENSE_FREQUENCY]=" + expense.Expense_Frequency +
                            "||[EXPENSE_DATE_SEQUENCE]=" + String.Join(",", expense.Date_Sequence) +
                            "||[EXPENSE_STATUS]=" + expense.Expense_Status +
                            "||[EXPENSE_START_DATE]=" + expense.Expense_Start_Date.ToString() +
                            "||[EXPENSE_LAST_PAY_DATE]=" + expense.Last_Pay_Date.ToString() +
                            "||[EXPENSE_ALERT_DATE]=" + expense.Alert_Off_Date.ToString() +
                            "||[EXPENSE_AMOUNT]=" + expense.Expense_Amount +
                            "||[EXPENSE_AUTODEBIT]=" + expense.AutoDebit +
                            "||[EXPENSE_PAYMENT_COMPANY]=" + expense.Payment_Company +
                            "||[EXPENSE_PAYMENT_LAST_FOUR]=" + expense.Payment_Last_Four + Environment.NewLine);
            }
            return line;
        }

        public StringBuilder Save_Orders()
        {
            StringBuilder line = new StringBuilder();
            foreach (Order order in parent.Order_List.OrderBy(x => x.Date))
            {
                line.Append("[OR_LO_]=" + order.Location +
                            "||[OR_QU_]=" + order.Order_Quantity +
                            "||[OR_PP_]=" + order.Order_Total_Pre_Tax +
                            "||[OR_DI_]=" + order.Order_Discount_Amt +
                            "||[OR_GC_]=" + order.GC_Amount +
                            "||[OR_TA_]=" + order.Order_Taxes +
                            "||[OR_ME_]=" + order.OrderMemo +
                            "||[OR_TO_]=" + (order.Tax_Overridden ? "1" : "0") +
                            "||[OR_DA_]=" + order.Date.ToString() +
                            "||[OR_PA_]=" + order.Payment_Type +
                            "||[OR_ID_]=" + order.OrderID + Environment.NewLine);
            }
            return line;
        }

        public StringBuilder Save_Accounts()
        {
            StringBuilder line = new StringBuilder();
            foreach (Account acc in parent.Account_List)
            {
                line.Append("[ACCOUNT_TYPE]=" + acc.Type +
                            "||[ACCOUNT_PAYER]=" + acc.Payer +
                            "||[ACCOUNT_REMARK]=" + acc.Remark +
                            "||[ACCOUNT_ALERT_ACTIVE]=" + acc.Alert_Active +
                            "||[ACCOUNT_AMOUNT]=" + acc.Amount +
                            "||[ACCOUNT_STATUS]=" + acc.Status.ToString() +
                            "||[ACCOUNT_INACTIVE]=" + acc.Inactive_Date.ToString() +
                            "||[ACCOUNT_START]=" + acc.Start_Date.ToString() + Environment.NewLine);
            }
            return line;
        }

        public StringBuilder Save_Warnings()
        {
            StringBuilder line = new StringBuilder();
            foreach (KeyValuePair<string, Warning> Key in parent.Warnings_Dictionary)
            {
                line.Append("[WARNING_CATEGORY]=" + Key.Key +
                            "||[WARNING_FIRST]=" + Key.Value.First_Level +
                            "||[WARNING_SECOND]=" + Key.Value.Second_Level +
                            "||[WARNING_FINAL]=" + Key.Value.Final_Level +
                            "||[WARNING_TYPE]=" + Key.Value.Warning_Type +
                            "||[WARNING_AMOUNT]=" + Key.Value.Warning_Amt + Environment.NewLine);
            }
            return line;
        }

        public StringBuilder Save_Tracking()
        {
            StringBuilder line = new StringBuilder();
            // Ignore null tracking
            foreach (Shipment_Tracking ST in parent.Tracking_List.Where(x => x.Ref_Order_Number != "999999999").ToList())
            {
                line.Append("[TRK_NUMBER]=" + ST.Tracking_Number +
                            "||[TRK_REF_ORDER_NUMBER]=" + ST.Ref_Order_Number +
                            "||[TRK_EXP_DATE]=" + ST.Expected_Date.ToShortDateString() +
                            "||[TRK_REC_DATE]=" + ST.Received_Date.ToShortDateString() +
                            "||[TRK_ALERT_DATE]=" + ST.Last_Alert_Date.ToShortDateString() +
                            "||[TRK_ALERT_ACTIVE]=" + (ST.Alert_Active ? "1" : "0") +
                            "||[TRK_EMAIL_ACTIVE]=" + (ST.Email_Active ? "1" : "0") +
                            "||[TRK_STATUS]=" + ST.Status + Environment.NewLine);
            }
            return line;
        }

        public StringBuilder Save_Budget_Allocations()
        {
            StringBuilder line = new StringBuilder();
            // Ignore null tracking
            foreach (BudgetEntry BE in parent.BudgetEntryList)
            {
                line.Append("[BU_MY_]=" + BE.Month.ToString("D2") + BE.Year.ToString().Substring(2, 2) +
                            "||[BU_IN_]=" + (BE.IncomeMode == IncomeMode.Automatic ? "A" : "M") + BE.TargetBudget +
                            "||[BU_IT_]=");

                string budgetCategoryLines = "";

                foreach (BudgetCategory BC in BE.GetCategoryList())
                {
                    switch (BC.GetBCType())
                    {
                        case BCType.Categorical:
                        {
                            budgetCategoryLines += String.Format("{0}~~{1}~~{2}~~", "C", BC.GetName(), BC.TargetAmount);
                            break;
                        }
                        case BCType.Recurring:
                        {
                            budgetCategoryLines += String.Format("{0}~~{1}~~{2}~~", "R", BC.GetName(), BC.TargetAmount);
                            break;
                        }
                        case BCType.Extraneous:
                        {
                            budgetCategoryLines += String.Format("{0}~~{1}~~{2}~~", "N", BC.GetName(), BC.TargetAmount);
                            break;
                        }
                    }
                }

                if (budgetCategoryLines.Length > 0)
                    line.Append(budgetCategoryLines.Substring(0, budgetCategoryLines.Length - 2));

                line.Append(Environment.NewLine);
            }
            return line;
        }

        #endregion
    }

    public class Saver
    {
        public readonly object _locker = new object();

        public void Save(string msg, string filePath)
        {

            TextWriter writer = new StreamWriter(filePath);
            StreamToFile(writer, msg);

            if (writer != null)
            {
                writer.Flush();
                writer.Dispose();
                writer.Close();
            }
        }

        public void StreamToFile(TextWriter sw, string msg)
        {
            sw.Write(msg);
        }
    }
}
