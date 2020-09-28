using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.IO;
using DataAccess_Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Business_Entities;

namespace Read_File_Processor
{
    public class ExecuteProcess
    {


        public int Execute_Excel_InitialTracker()
        {
            int Value = 0;
            DML_Utility objDML = new DML_Utility();
            try
            {
                string strFilePath = "";
                string strFileName = "";
                // Get HostName //
                Get_Data_Utility objGet = new Get_Data_Utility();
                List<tbl_config_value> lstConfig = objGet.Get_Cofig_Details("MISDATA");
                foreach (var ob in lstConfig)
                {
                    strFilePath = ob.configstring;
                    strFileName = ob.FileName;
                }

                string path = strFilePath + strFileName; // 
                //FilePath_Container.FilePath + FilePath_Container.FileName_InitationTracker;
                if (File.Exists(path))
                {
                    DataTable dt = Read_Excel("Sheet1", path);
                    if (dt.Rows.Count > 0)
                    {
                        // Add Rows Count into table //
                        long rowcount = dt.Rows.Count;
                        objDML.Add_Rows_Count_Data(path, rowcount);
                        Value = addData(dt);
                    }
                    else
                    {
                        Value = 9; // No data exist
                    }
                }
            }
            catch (Exception ex)
            {
                Value = -1;
                ////// Exception Log ///
                int iException = objDML.Add_Exception_Log("wipro173 exception : " + ex.Message, "");

            }
            return Value;
        }

        public string Read_Response()
        {
            DML_Utility objDML = new DML_Utility();
            string output = "";
            // Read RABITMQ for Response //
            string Response_Data = "";
            string Response_Error = "";
            try
            {
                RabbitMQ_Utility objQueue = new RabbitMQ_Utility();
                bool retResponse = objQueue.Receive(RabbitMQ_Utility.RabbitMQResponseQueue, out Response_Data, out Response_Error);
                //objDML.Add_Exception_Log(Response_Data, "");

                //objDML.Add_Exception_Log(Response_Data, "");
                if (!string.IsNullOrEmpty(Response_Data))
                {
                    string responce_MessageId = "";
                    string responce_ServiceId = "";

                    responce_MessageId = Read_Json_TagWise(Response_Data, "metadata", "requestId");
                    responce_ServiceId = Read_Json_Data_TagWise(Response_Data, "data", "taskName");
                    //long reqId = Convert.ToInt64(Read_Json_Data_TagWise(Response_Data, "data", "taskSerialNo"));
                    // Add Response into Database //
                    Get_Data_Utility obj = new Get_Data_Utility();
                    //DML_Utility objDML = new DML_Utility();
                    long reqId = obj.Get_Request_Id(responce_MessageId, responce_ServiceId);
                    //objDML.Add_Exception_Log("select * from tbl_request_details where messageid='" + responce_MessageId + "'", "");
                    //objDML.Add_Exception_Log(reqId.ToString(), "");

                    if (reqId > 0)
                    {
                        objDML.Add_Response_Json(reqId, Response_Data, responce_MessageId, responce_ServiceId);
                    }
                }
            }
            catch (Exception ex)
            {
                output = "exception : " + ex.Message.ToString();
                ////// Exception Log ///

                int iException = objDML.Add_Exception_Log("wipro173 exception : " + ex.Message, "Read_Response");
            }
            return output;
        }
        public string ConvertToXLSX(string filePath)
        {
            // logger.Info("Convert to XLSX started.");
            DML_Utility objDML = new DML_Utility();

            try
            {
                string directoryName = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string[] files = new string[1];
                string[] newFile = new string[1];
                files = Directory.GetFiles(directoryName, fileName + ".xls");
                var app = new Microsoft.Office.Interop.Excel.Application();
                //foreach (string file in files)
                //{
                var wb = app.Workbooks.Open(files[0].ToString());
                wb.SaveAs(Filename: files[0].ToString() + "x", FileFormat: Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
                wb.Close();
                app.Quit();
                File.Delete(filePath);
                newFile = Directory.GetFiles(directoryName, fileName + ".xlsx");
                filePath = Path.GetFullPath(newFile[0]).ToString();
                // }
                //logger.Info("Convert to XLSX completed.");
                return filePath;
            }
            catch (Exception ex)
            {
                objDML.Add_Exception_Log("wipro173 exception : " + "Before conversion", "");
                //logger.Error("Error occured while converting xls to xslx. Error message: " + ex.Message.ToString());
                throw ex;
                //return null;
            }

        }
        public string Execute_Excel_YetToStart_Process_Download()
        {
            string output = "";

            Get_Data_Utility obj = new Get_Data_Utility();
            DML_Utility objDML = new DML_Utility();
            //objDML.Add_Exception_Log("", "Execute_Excel_YetToStart_Process_Download");
            JsonCreater JsonCreater = new Read_File_Processor.JsonCreater();
            long ResId = 0;

            try
            {
                string strDateKey = System.DateTime.Now.ToString("yyyyMMddHHmmss");
                // GET RESONSE JSON TO BE PROCESSED 
                List<tbl_response_detail> lstResponse = obj.Get_Response_Data_ToBe_Process(FilePath_Container.ServiceId_WiproDownload);
                if (lstResponse.Count > 0)
                {
                    string MessageId = "";
                    string ServiceId = "";
                    long ReqId = 0;
                    string ResJson = "";
                    foreach (var res in lstResponse)
                    {
                        MessageId = res.message_id;
                        ServiceId = res.service_id;
                        ResId = res.id;
                        ReqId = (long)res.request_id;
                        ResJson = res.response_json;
                    }
                    //int iException1 = objDML.Add_Exception_Log(ResJson, "");

                    if (ResId > 0 && !string.IsNullOrEmpty(ResJson))
                    {
                        // Read Json and Excel Process
                        string responce_MessageId = "";
                        string responce_Status = "";
                        string responce_Module = "";
                        //string RequestType = "DOWNLOAD";
                        string result = Read_Json_Data_TagWise(ResJson, "data", "result");
                        string details = Read_Json_TagWise(result, "downloadDetails", "details");
                        //string Des = Read_Json_Case_Creation_TagValue(ResJson, "Status", "Description");
                        //objDML.Add_Exception_Log(Des, "Download Status Description");
                        if (details == "Campus type excel is downloaded")
                        {
                            string path = Read_Json_TagWise(result, "downloadDetails", "filePath"); ;// Read_Json(ResJson, out responce_MessageId, out responce_Status, out responce_Module);
                            if (!string.IsNullOrEmpty(path))
                            {

                                //ResJson = File.ReadAllText(path);
                                DataTable dt = Read_CSV(path);
                                //JObject json = JObject.Parse(ResJson);
                                //JArray data = JArray.Parse(json["Data"].ToString());

                                foreach (DataRow item in dt.Rows)
                                {
                                    string ResumeID = item["Resume ID"].ToString().Replace("'", "");
                                    string EmpID = item["Emp ID"].ToString().Replace("'", "");
                                    string EmpName = item["Emp Name"].ToString().Replace("'", "");
                                    string DateOfJoining = item["Date Of Joining"].ToString().Replace("'", "");
                                    string FatherName = item["Father Name"].ToString().Replace("'", "");
                                    string DateofBirth = item["Date of Birth"].ToString().Replace("'", "");
                                    string ContactNumber = item["Contact Number"].ToString().Replace("'", "");
                                    string EmailId = item["Email Id"].ToString().Replace("'", "");
                                    string Country = item["Country"].ToString().Replace("'", "");
                                    string Geo = item["Geo"].ToString().Replace("'", "");
                                    string Qualification = item["Qualification"].ToString().Replace("'", "");
                                    string UniversityName = item["University Name"].ToString().Replace("'", "");
                                    string CollegeName = item["College Name"].ToString().Replace("'", "");
                                    string ModeOfEducation = item["Mode Of Education"].ToString().Replace("'", "");
                                    string YearOfPassing = item["Year Of Passing"].ToString().Replace("'", "");
                                    string AccountName = item["Account Name"].ToString().Replace("'", "");
                                    string AgencyNumber = item["Agency Number"].ToString().Replace("'", "");
                                    string BGVInitiatedDate = item["BGV Initiated Date"].ToString().Replace("'", "");
                                    string BGVInsuffRaisedDate = item["BGV Insuff Raised Date"].ToString().Replace("'", "");
                                    string InsuffComponent = item["Insuff Component"].ToString().Replace("'", "");
                                    string InsuffRaisedRemarks = item["Insuff Raised Remarks"].ToString().Replace("'", "");
                                    string InsuffClearedRemarks = item["Insuff Cleared Remarks"].ToString().Replace("'", "");
                                    string BGVInsuffClearedDate = item["BGV Insuff Cleared Date"].ToString().Replace("'", "");
                                    string BGVAssignedDate = item["BGV Assigned Date"].ToString().Replace("'", "");
                                    string BGVQCRejectedDate = item["BGV QC Rejected Date"].ToString().Replace("'", "");
                                    string BGVQCRejectedRemarks = item["BGV QC Rejected Remarks"].ToString().Replace("'", "");
                                    string BGVSPOC = item["BGV SPOC"].ToString().Replace("'", "");
                                    string AG_REPORTED_STATUS_BGV = item["AG_REPORTED_STATUS_BGV"].ToString().Replace("'", "");
                                    string AG_REPORTED_DATE_BGV = item["AG_REPORTED_DATE_BGV"].ToString().Replace("'", "");
                                    string SLA = item["SLA"].ToString().Replace("'", "");
                                    string AgencytoreverifyDate = item["Agency to re-verify Date"].ToString().Replace("'", "");
                                    string Agencytoreverifyremarks = item["Agency to re-verify remarks"].ToString().Replace("'", "");
                                    string AgencyPlannedClosureDate = item["Agency Planned Closure Date"].ToString().Replace("'", "");
                                    string PriorityFlag = item["Priority Flag"].ToString().Replace("'", "");
                                    string Division = item["Division"].ToString().Replace("'", "");
                                    string BGVStatus = item["BGV Status"].ToString().Replace("'", "");
                                    string ActiveFlag = item["Active Flag"].ToString().Replace("'", "");
                                    string BGV_Final_status = item["BGV_Final_status"].ToString().Replace("'", "");

                                    objDML.InsertWiproCampusDetails(ResumeID, EmpID, EmpName, DateOfJoining, FatherName, DateofBirth, ContactNumber, EmailId, Country, Geo, Qualification, UniversityName, CollegeName, ModeOfEducation, YearOfPassing, AccountName, AgencyNumber, BGVInitiatedDate, BGVInsuffRaisedDate, InsuffComponent, InsuffRaisedRemarks, InsuffClearedRemarks, BGVInsuffClearedDate, BGVAssignedDate, BGVQCRejectedDate, BGVQCRejectedRemarks, BGVSPOC, AG_REPORTED_STATUS_BGV, AG_REPORTED_DATE_BGV, SLA, AgencytoreverifyDate, Agencytoreverifyremarks, AgencyPlannedClosureDate, PriorityFlag, Division, BGVStatus, ActiveFlag, BGV_Final_status);
                                }
                            }
                            objDML.Update_Response_Status(ResId);
                            #region OldCODE
                            //string path = Read_Json(ResJson, out responce_MessageId, out responce_Status, out responce_Module);
                            ////objDML.Add_Exception_Log(path, "");
                            ////objDML.Add_Exception_Log(responce_Status, "");

                            ////objDML.Add_Exception_Log(ResJson, "");
                            //if (responce_Status.ToLower() == "y" || responce_Status.ToLower() == "yes")
                            //{
                            //    //objDML.Add_Exception_Log(path, "");
                            //    if (File.Exists(path))
                            //    {
                            //        //string path = FilePath_Container.FilePath + FilePath_Container.FileName_YetToStart;
                            //        //objDML.Add_Exception_Log("Before conversion", "");
                            //        //string newpath = ConvertToXLSX(path);
                            //        //objDML.Add_Exception_Log("afetr conversion", "newpath"+ newpath);

                            //        //DataTable dt = Read_Excel(Path.GetFileNameWithoutExtension(path), path);
                            //        DataTable dt = ConvertToCSV(path);
                            //        //objDML.Add_Exception_Log("Before", "");
                            //        //DataTable dt = Import(path);
                            //        //objDML.Add_Exception_Log(dt.Rows.Count.ToString(), "");
                            //        if (dt.Rows.Count > 0)
                            //        {
                            //            output = addData_YetToStart(dt, strDateKey);
                            //            if (output == "OK")
                            //            {
                            //                // Get Data after compare for New Request Id
                            //                List<tbl_input_request_data> lst = new List<tbl_input_request_data>();
                            //                List<YetToStart> lstJson = new List<YetToStart>();
                            //                YetToStart objJson = new YetToStart();
                            //                lst = obj.Get_New_Request_Id_List(strDateKey);
                            //                int iCount = 0;

                            //                foreach (var objList in lst)
                            //                {
                            //                    string queueMessageId = Guid.NewGuid().ToString();
                            //                    string queueServiceId = FilePath_Container.FreshCase;
                            //                    output = JsonCreater.getDetails_FreshCase(objList.Request_ID, objList.Associate_Id, objList.Candidate_ID, objList.DOJ, queueMessageId, Path.GetFileNameWithoutExtension(path), objList.Name, queueServiceId);
                            //                    /////////////////////////// Add Json request into database //////////////
                            //                    int iDML = objDML.Add_Request_Json_Detail(queueMessageId, queueServiceId, output);
                            //                    if (iDML == 1)
                            //                    {
                            //                        bool ret = Write_JSON_TO_RABBIT_MQ(output);
                            //                        output = ret ? "Success" : "Failed";
                            //                    }
                            //                    iCount++;
                            //                }
                            //                //if (iCount == lst.Count)
                            //                {
                            //                    // Update Status of Response //
                            //                    objDML.Update_Response_Status(ResId);
                            //                }
                            //            }
                            //        }
                            //        else
                            //        {
                            //            output = "not exist";
                            //        }
                            //        //objDML.Add_Exception_Log(output, "");

                            //    }
                            //}
                            #endregion
                        }
                        else if (details == "GSH type excel is downloaded")
                        {
                            string path = Read_Json_TagWise(result, "downloadDetails", "filePath"); ;// Read_Json(ResJson, out responce_MessageId, out responce_Status, out responce_Module);
                            if (!string.IsNullOrEmpty(path))
                            {

                                //ResJson = File.ReadAllText(path);
                                DataTable dt = Read_CSV(path);
                                //JObject json = JObject.Parse(ResJson);
                                //JArray data = JArray.Parse(json["Data"].ToString());

                                foreach (DataRow item in dt.Rows)
                                {
                                    string ResumeNumber = item["Resume Number"].ToString().Replace("'", "");
                                    string CandidateName = item["Candidate Name"].ToString().Replace("'", "");
                                    string OverallStatus = item["Overall Status"].ToString().Replace("'", "");
                                    string FatherName = item["Father Name"].ToString().Replace("'", "");
                                    string DateofBirth = item["Date of Birth"].ToString().Replace("'", "");
                                    string ContractNumber = item["Contract Number"].ToString().Replace("'", "");
                                    string EmailId = item["Email Id"].ToString().Replace("'", "");
                                    string AccountName = item["Account Name"].ToString().Replace("'", "");
                                    string AgencyNumber = item["Agency Number"].ToString().Replace("'", "");
                                    string BGVInitiatedDate = item["BGV Initiated Date"].ToString().Replace("'", "");
                                    string BGV1AssignedDate = item["BGV1 Assigned Date"].ToString().Replace("'", "");
                                    string BGV2AssignedDate = item["BGV2 Assigned Date"].ToString().Replace("'", "");
                                    string BGV1QCRejectedDate = item["BGV1 QC Rejected Date"].ToString().Replace("'", "");
                                    string BGV2QCRejectedDate = item["BGV2 QC Rejected Date"].ToString().Replace("'", "");
                                    string Qualification = item["Qualification"].ToString().Replace("'", "");
                                    string UniversityName = item["University Name"].ToString().Replace("'", "");
                                    string CollegeName = item["College Name"].ToString().Replace("'", "");
                                    string ModeOfEducation = item["Mode Of Education"].ToString().Replace("'", "");
                                    string YearOfPassing = item["Year Of Passing"].ToString().Replace("'", "");
                                    string DateOfJoining = item["Date Of Joining"].ToString().Replace("'", "");
                                    string CurrentAddress = item["Current Address"].ToString().Replace("'", "");
                                    string PermanentAddress = item["Permanent Address"].ToString().Replace("'", "");
                                    string BGV1InsufRaisedDate = item["BGV1 Insuff Raised Date"].ToString().Replace("'", "");
                                    string BGV2InsufRaisedDate = item["BGV2 Insuff Raised Date"].ToString().Replace("'", "");
                                    string BGV1InsufClearedDate = item["BGV1 Insuff Cleared Date"].ToString().Replace("'", "");
                                    string BGV2InsufClearedDate = item["BGV2 Insuff Cleared Date"].ToString().Replace("'", "");
                                    string InsuffComponent = item["Insuff Component"].ToString().Replace("'", "");
                                    string BGVSPOC = item["BGV SPOC"].ToString().Replace("'", "");
                                    string BGV1_status = item["BGV1_status"].ToString().Replace("'", "");
                                    string BGV2_status = item["BGV2_status"].ToString().Replace("'", "");
                                    string BGV1_Final_status = item["BGV1_Final_status"].ToString().Replace("'", "");
                                    string BGV2_Final_status = item["BGV2_Final_status"].ToString().Replace("'", "");
                                    string Insuff_Raised_Remarks = item["Insuff Raised Remarks"].ToString().Replace("'", "");
                                    string Insuff_Cleared_Remarks = item["Insuff Cleared Remarks"].ToString().Replace("'", "");


                                    objDML.InsertWiproGSHDetails(ResumeNumber, CandidateName, OverallStatus, FatherName, DateofBirth, ContractNumber,
                                        EmailId, AccountName, AgencyNumber, BGVInitiatedDate, BGV1AssignedDate, BGV2AssignedDate, BGV1QCRejectedDate,
                                        BGV2QCRejectedDate, Qualification, UniversityName, CollegeName, ModeOfEducation, YearOfPassing, DateOfJoining,
                                        CurrentAddress, PermanentAddress, BGV1InsufRaisedDate, BGV2InsufRaisedDate, BGV1InsufClearedDate, BGV2InsufClearedDate,
                                        InsuffComponent, BGVSPOC, BGV1_status, BGV2_status, BGV1_Final_status, BGV2_Final_status, Insuff_Raised_Remarks, Insuff_Cleared_Remarks);
                                }
                            }
                            objDML.Update_Response_Status(ResId);
                        }
                        else if (details == "Internal type excel is downloaded")
                        {
                            string path = Read_Json_TagWise(result, "downloadDetails", "filePath"); ;// Read_Json(ResJson, out responce_MessageId, out responce_Status, out responce_Module);
                            if (!string.IsNullOrEmpty(path))
                            {

                                //ResJson = File.ReadAllText(path);
                                DataTable dt = Read_CSV(path);
                                //JObject json = JObject.Parse(ResJson);
                                //JArray data = JArray.Parse(json["Data"].ToString());

                                foreach (DataRow item in dt.Rows)
                                {
                                    string EmployeeNumber = item["Employee Number"].ToString().Replace("'", "");
                                    string OverallStatus = item["Overall Status"].ToString().Replace("'", "");
                                    string EmployeeName = item["Employee Name"].ToString().Replace("'", "");
                                    string ContratNumber = item["Contract Number"].ToString().Replace("'", "");
                                    string FatherName = item["Father Name"].ToString().Replace("'", "");
                                    string InsuffRaisedRemarks = item["Insuff Raised Remarks"].ToString().Replace("'", "");
                                    string InsuffClearedRemarks = item["Insuff Cleared Remarks"].ToString().Replace("'", "");
                                    string AgencyNumber = item["Agency Number"].ToString().Replace("'", "");
                                    string BGVSPOC = item["BGV SPOC"].ToString().Replace("'", "");
                                    string EmailId = item["Email Id"].ToString().Replace("'", "");
                                    string AccountName = item["Account Name"].ToString().Replace("'", "");
                                    string BGVInitiatedDate = item["BGV Initiated Date"].ToString().Replace("'", "");
                                    string BGVAssignedDate = item["BGV Assigned Date"].ToString().Replace("'", "");
                                    string BGVInsuffRaisedDate = item["BGV Insuff Raised Date"].ToString().Replace("'", "");
                                    string BGVInsuffClearedDate = item["BGV Insuff Cleared Date"].ToString().Replace("'", "");
                                    string BGVQCRejectedDate = item["BGV QC Rejected Date"].ToString().Replace("'", "");                                   
                                    string BGV1_STATUS = item["BGV1_status"].ToString().Replace("'", "");
                                    string BGV1_FINAL_STATUS = item["BGV1_Final_status"].ToString().Replace("'", "");



                                    objDML.InsertWiproInternalDetails(EmployeeNumber, OverallStatus,EmployeeName,ContratNumber,FatherName,InsuffRaisedRemarks
                                        ,InsuffClearedRemarks,AgencyNumber,BGVSPOC,EmailId,AccountName,BGVInitiatedDate,BGVAssignedDate,
                                        BGVInsuffRaisedDate,BGVInsuffClearedDate,BGVQCRejectedDate,BGV1_STATUS,BGV1_FINAL_STATUS);
                                }
                            }
                            objDML.Update_Response_Status(ResId);
                        }
                        else if (details == "Lateral type excel is downloaded")
                        {
                            string path = Read_Json_TagWise(result, "downloadDetails", "filePath"); ;// Read_Json(ResJson, out responce_MessageId, out responce_Status, out responce_Module);
                            if (!string.IsNullOrEmpty(path))
                            {
                                //ResJson = File.ReadAllText(path);
                                DataTable dt = Read_CSV(path);
                                //JObject json = JObject.Parse(ResJson);
                                //JArray data = JArray.Parse(json["Data"].ToString());

                                foreach (DataRow item in dt.Rows)
                                {
                                    string ResumeNumber = item["Resume Number"].ToString().Replace("'", "");
                                    string CandidateName = item["Candidate Name"].ToString().Replace("'", "");
                                    string FatherName = item["Father Name"].ToString().Replace("'", "");
                                    string DateofBirth = item["Date of Birth"].ToString().Replace("'", "");
                                    string ContractNumber = item["Contact Number"].ToString().Replace("'", "");
                                    string EmailId = item["Email Id"].ToString().Replace("'", "");
                                    string Qualification = item["Qualification"].ToString().Replace("'", "");
                                    string UniversityName = item["University Name"].ToString().Replace("'", "");
                                    string CollegeName = item["College Name"].ToString().Replace("'", "");
                                    string ModeOfEducation = item["Mode Of Education"].ToString().Replace("'", "");
                                    string YearOfPassing = item["Year Of Passing"].ToString().Replace("'", "");
                                    string AccountName = item["Account Name"].ToString().Replace("'", "");
                                    string AgencyNumber = item["Agency Number"].ToString().Replace("'", "");
                                    string BGVInitiatedDate = item["BGV Initiated Date"].ToString().Replace("'", "");
                                    string BGV1InsufRaisedDate = item["BGV1 Insuff Raised Date"].ToString().Replace("'", "");
                                    string InsuffComponent = item["Insuff Component"].ToString().Replace("'", "");
                                    string InsuffRaisedRemarks = item["Insuff Raised Remarks"].ToString().Replace("'", "");
                                    string InsuffClearedRemarks = item["Insuff Cleared Remarks"].ToString().Replace("'", "");
                                    string BGV1InsufClearedDate = item["BGV1 Insuff Cleared Date"].ToString().Replace("'", "");
                                    string BGV1AssignedDate = item["BGV1 Assigned Date"].ToString().Replace("'", "");
                                    string BGV1QCRejectedDate = item["BGV1 QC Rejected Date"].ToString().Replace("'", "");
                                    string BGV2AssignedDate = item["BGV2 Assigned Date"].ToString().Replace("'", "");
                                    string BGV2InsufRaisedDate = item["BGV2 Insuff Raised Date"].ToString().Replace("'", "");
                                    string BGV2InsuffComponent = item["BGV2 Insuff Component"].ToString().Replace("'", "");
                                    string BGV2InsuffRaisedRemarks = item["BGV2 Insuff Raised Remarks"].ToString().Replace("'", "");
                                    string BGV2InsuffClearedRemarks = item["BGV2 Insuff Cleared Remarks"].ToString().Replace("'", "");
                                    string BGV2InsufClearedDate = item["BGV2 Insuff Cleared Date"].ToString().Replace("'", "");
                                    string BGV2QCRejectedDate = item["BGV2 QC Rejected Date"].ToString().Replace("'", "");
                                    string DateOfJoining = item["Date Of Joining"].ToString().Replace("'", "");
                                    string BGVSPOC = item["BGV SPOC"].ToString().Replace("'", "");
                                    string AG_REPORTED_Status_BV1 = item["AG_REPORTED_Status_BV1"].ToString().Replace("'", "");
                                    string AG_REPORTED_Status_BV2 = item["AG_REPORTED_Status_BV2"].ToString().Replace("'", "");
                                    string AG_REPORT_date_BV1 = item["AG_REPORT_date_BV1"].ToString().Replace("'", "");
                                    string AG_REPORT_date_BV2 = item["AG_REPORT_date_BV2"].ToString().Replace("'", "");
                                    string priorityflag = item["priority flag"].ToString().Replace("'", "");                                   
                                    string BGV1_status = item["BGV1_status"].ToString().Replace("'", "");
                                    string BGV2_status = item["BGV2_status"].ToString().Replace("'", "");
                                    string BGV1_Final_status = item["BGV1_Final_status"].ToString().Replace("'", "");
                                    string BGV2_Final_status = item["BGV2_Final_status"].ToString().Replace("'", "");


                                    objDML.InsertWiproLateralDetails(ResumeNumber, CandidateName, FatherName, DateofBirth, ContractNumber,
                                        EmailId, Qualification, UniversityName, CollegeName, ModeOfEducation, YearOfPassing, AccountName,
                                        AgencyNumber, BGVInitiatedDate, BGV1InsufRaisedDate, InsuffComponent, InsuffRaisedRemarks,
                                        InsuffClearedRemarks, BGV1InsufClearedDate, BGV1AssignedDate, BGV1QCRejectedDate, BGV2AssignedDate,
                                        BGV2InsufRaisedDate, BGV2InsuffComponent, BGV2InsuffRaisedRemarks, BGV2InsuffClearedRemarks,
                                        BGV2InsufClearedDate, BGV2QCRejectedDate, DateOfJoining, BGVSPOC, AG_REPORTED_Status_BV1, AG_REPORTED_Status_BV2,
                                       AG_REPORT_date_BV1, AG_REPORT_date_BV2, priorityflag, "", BGV1_status,
                                       BGV2_status, BGV1_Final_status, BGV2_Final_status);
                                }
                            }
                            objDML.Update_Response_Status(ResId);
                        }
                        else if (details == "Contract type excel is downloaded")
                        {
                            string path = Read_Json_TagWise(result, "downloadDetails", "filePath"); ;// Read_Json(ResJson, out responce_MessageId, out responce_Status, out responce_Module);
                            if (!string.IsNullOrEmpty(path))
                            {
                                //ResJson = File.ReadAllText(path);
                                DataTable dt = Read_CSV(path);
                                //JObject json = JObject.Parse(ResJson);
                                //JArray data = JArray.Parse(json["Data"].ToString());

                                foreach (DataRow item in dt.Rows)
                                {
                                    string ResumeNumber = item["Resume Number"].ToString().Replace("'", "");
                                    string CandidateName = item["Candidate Name"].ToString().Replace("'", "");
                                    string OverallStatus = item["Overall Status"].ToString().Replace("'", "");
                                    string FatherName = item["Father Name"].ToString().Replace("'", "");
                                    string DateofBirth = item["Date of Birth"].ToString().Replace("'", "");
                                    string ContractNumber = item["Contract Number"].ToString().Replace("'", "");
                                    string EmailId = item["Email Id"].ToString().Replace("'", "");
                                    string AccountName = item["Account Name"].ToString().Replace("'", "");
                                    string AgencyNumber = item["Agency Number"].ToString().Replace("'", "");
                                    string BGVInitiatedDate = item["BGV Initiated Date"].ToString().Replace("'", "");
                                    string BGV1AssignedDate = item["BGV1 Assigned Date"].ToString().Replace("'", "");
                                    string BGV2AssignedDate = item["BGV2 Assigned Date"].ToString().Replace("'", "");
                                    string BGV1QCRejectedDate = item["BGV1 QC Rejected Date"].ToString().Replace("'", "");
                                    string BGV2QCRejectedDate = item["BGV2 QC Rejected Date"].ToString().Replace("'", "");
                                    string Qualification = item["Qualification"].ToString().Replace("'", "");
                                    string UniversityName = item["University Name"].ToString().Replace("'", "");
                                    string CollegeName = item["College Name"].ToString().Replace("'", "");
                                    string ModeOfEducation = item["Mode Of Education"].ToString().Replace("'", "");
                                    string YearOfPassing = item["Year Of Passing"].ToString().Replace("'", "");
                                    string DateOfJoining = item["Date Of Joining"].ToString().Replace("'", "");
                                    string CurrentAddress = item["Current Address"].ToString().Replace("'", "");
                                    string PermanentAddress = item["Permanent Address"].ToString().Replace("'", "");
                                    string BGV1InsufRaisedDate = item["BGV1 Insuff Raised Date"].ToString().Replace("'", "");
                                    string BGV2InsufRaisedDate = item["BGV2 Insuff Raised Date"].ToString().Replace("'", "");
                                    string BGV1InsufClearedDate = item["BGV1 Insuff Cleared Date"].ToString().Replace("'", "");
                                    string BGV2InsufClearedDate = item["BGV2 Insuff Cleared Date"].ToString().Replace("'", "");
                                    string InsuffComponent = item["Insuff Component"].ToString().Replace("'", "");
                                    string InsuffRaisedRemarks = item["Insuff Raised Remarks"].ToString().Replace("'", "");
                                    string InsuffClearedRemarks = item["Insuff Cleared Remarks"].ToString().Replace("'", "");
                                    string BGVSPOC = item["BGV SPOC"].ToString().Replace("'", "");
                                    string BGV1_status = item["BGV1_status"].ToString().Replace("'", "");
                                    string BGV2_status = item["BGV2_status"].ToString().Replace("'", "");
                                    string BGV1_Final_status = item["BGV1_Final_status"].ToString().Replace("'", "");
                                    string BGV2_Final_status = item["BGV2_Final_status"].ToString().Replace("'", "");


                                    objDML.InsertWiproContractDetails(ResumeNumber, CandidateName, OverallStatus, FatherName, DateofBirth, ContractNumber,
                                       EmailId, AccountName, AgencyNumber, BGVInitiatedDate, BGV1AssignedDate, BGV2AssignedDate, BGV1QCRejectedDate,
                                       BGV2QCRejectedDate, Qualification, UniversityName, CollegeName, ModeOfEducation, YearOfPassing, DateOfJoining,
                                       CurrentAddress, PermanentAddress, BGV1InsufRaisedDate, BGV2InsufRaisedDate, BGV1InsufClearedDate, BGV2InsufClearedDate,
                                       InsuffComponent, InsuffRaisedRemarks, InsuffClearedRemarks, BGVSPOC, BGV1_status, BGV2_status, BGV1_Final_status, BGV2_Final_status);
                                }
                            }
                            objDML.Update_Response_Status(ResId);
                        }
                        else
                        {
                            objDML.Update_Response_Status(ResId);
                            //if (Des != "Search yielded empty results")
                            //{
                            //    MessageId = Guid.NewGuid().ToString();
                            //    ServiceId = FilePath_Container.ServiceId_Download.ToString();// "DOWNLOAD";// Guid.NewGuid().ToString();
                            //                                                                 ///////////////////////// Add Json request into database //////////////
                            //    JsonCreater = new Read_File_Processor.JsonCreater();
                            //    string json = JsonCreater.getDownload(MessageId, ServiceId);
                            //    int iDML = objDML.Add_Request_Json_Detail(MessageId, ServiceId, json);  // Add_Request_Json(MessageId, "DOWNLOAD", json);
                            //                                                                            ///////////////////////// Add Json request into Rabbit MQ //////////////
                            //    if (iDML == 1)
                            //    {
                            //        Write_JSON_TO_RABBIT_MQ(json);
                            //        //output = ret ? "Success" : "Failed";
                            //    }
                            //}
                        }
                    }
                }
                List<tbl_Wipro173_campus_data> list = obj.Get_UnProcessedRequests();
                foreach (var item in list)
                {
                    if (item.ResumeID != "No records Found")
                        if (!obj.IsCampusDuplicate(item.ResumeID))
                        {
                            string queueMessageId = Guid.NewGuid().ToString();
                            string queueServiceId = FilePath_Container.FreshCase;
                            output = JsonCreater.getDetails_FreshCase(item.ResumeID, item.EmpName, queueMessageId, "Campus", queueServiceId);
                            int iDML = objDML.Add_Request_Json_Detail(queueMessageId, queueServiceId, output);
                            if (iDML == 1)
                            {
                                bool ret = Write_JSON_TO_RABBIT_MQ(output);
                                objDML.Add_Exception_Log("Wipro-173: 1st attempt for Resume No : " + item.ResumeID + " Sub- Login : Campus has been sent to rabbitMQ ", item.ResumeID);
                                output = ret ? "Success" : "Failed";
                            }
                        }
                    objDML.UpdateWiproDetails(item.ResumeID, item.EmpName);
                }

                List<tbl_Wipro173_gsh_data> list1 = obj.Get_UnProcessedGSHRequests();
                foreach (var item in list1)
                {
                    if (item.ResumeNumber != "No records Found")
                        if (!obj.IsGSHDuplicate(item.ResumeNumber))
                        {
                            string queueMessageId = Guid.NewGuid().ToString();
                            string queueServiceId = FilePath_Container.FreshCase;
                            output = JsonCreater.getDetails_FreshCase(item.ResumeNumber, item.CandidateName, queueMessageId, "GSH", queueServiceId);
                            int iDML = objDML.Add_Request_Json_Detail(queueMessageId, queueServiceId, output);
                            if (iDML == 1)
                            {
                                bool ret = Write_JSON_TO_RABBIT_MQ(output);
                                objDML.Add_Exception_Log("Wipro-173: 1st attempt for Resume No : " + item.ResumeNumber + " Sub- Login : GSH has been sent to rabbitMQ ", item.ResumeNumber);
                                output = ret ? "Success" : "Failed";
                            }
                        }
                    objDML.UpdateWiproGSHDetails(item.ResumeNumber, item.CandidateName);
                }
                List<tbl_Wipro173_internal_data> list2 = obj.Get_UnProcessedInternalRequests();
                foreach (var item in list2)
                {
                    if (item.EmployeeNumber != "No records Found")
                        if (!obj.IsInternalDuplicate(item.EmployeeNumber))
                        {
                            string queueMessageId = Guid.NewGuid().ToString();
                            string queueServiceId = FilePath_Container.FreshCase;
                            output = JsonCreater.getDetails_FreshCase(item.EmployeeNumber, item.EmployeeName, queueMessageId, "Internal", queueServiceId);
                            int iDML = objDML.Add_Request_Json_Detail(queueMessageId, queueServiceId, output);
                            if (iDML == 1)
                            {
                                bool ret = Write_JSON_TO_RABBIT_MQ(output);
                                objDML.Add_Exception_Log("Wipro-173: 1st attempt for Employee No : " + item.EmployeeNumber + " Sub- Login : Internal has been sent to rabbitMQ ", item.EmployeeNumber);
                                output = ret ? "Success" : "Failed";
                            }
                        }
                    objDML.UpdateWiproInternalDetails(item.EmployeeNumber, item.EmployeeName);
                }

                List<tbl_Wipro173_lateral_data> list3 = obj.Get_UnProcessedLateralRequests();
                foreach (var item in list3)
                {
                    if (item.ResumeNumber != "No records Found")
                        if (!obj.IsLateralDuplicate(item.ResumeNumber))
                        {
                            string queueMessageId = Guid.NewGuid().ToString();
                            string queueServiceId = FilePath_Container.FreshCase;
                            output = JsonCreater.getDetails_FreshCase(item.ResumeNumber, item.CandidateName, queueMessageId, "Lateral", queueServiceId);
                            int iDML = objDML.Add_Request_Json_Detail(queueMessageId, queueServiceId, output);
                            if (iDML == 1)
                            {
                                bool ret = Write_JSON_TO_RABBIT_MQ(output);
                                objDML.Add_Exception_Log("Wipro-173: 1st attempt for Employee No : " + item.ResumeNumber + " Sub- Login : Lateral has been sent to rabbitMQ ", item.ResumeNumber);
                                output = ret ? "Success" : "Failed";
                            }
                        }
                    objDML.UpdateWiproLateralDetails(item.ResumeNumber, item.CandidateName);
                }
                List<tbl_Wipro173_contract_data> list4 = obj.Get_UnProcessedContractRequests();
                foreach (var item in list4)
                {
                    if (item.ResumeNumber != "No records Found")
                        if (!obj.IscontractDuplicate(item.ResumeNumber))
                        {
                            string queueMessageId = Guid.NewGuid().ToString();
                            string queueServiceId = FilePath_Container.FreshCase;
                            output = JsonCreater.getDetails_FreshCase(item.ResumeNumber, item.CandidateName, queueMessageId, "Contract", queueServiceId);
                            int iDML = objDML.Add_Request_Json_Detail(queueMessageId, queueServiceId, output);
                            if (iDML == 1)
                            {
                                bool ret = Write_JSON_TO_RABBIT_MQ(output);
                                objDML.Add_Exception_Log("Wipro-173: 1st attempt for Employee No : " + item.ResumeNumber + " Sub- Login : Contract has been sent to rabbitMQ ", item.ResumeNumber);
                                output = ret ? "Success" : "Failed";
                            }
                        }
                    objDML.UpdateWiproContractDetails(item.ResumeNumber, item.CandidateName);
                }

            }
            catch (Exception ex)
            {
                output = "exception : " + ex.Message.ToString();
                ////// Exception Log ///
                //DML_Utility objDML = new DML_Utility();
                objDML.Update_Response_Status(ResId);

                int iException = objDML.Add_Exception_Log("wipro173 exception : " + ex.Message, "Execute_Excel_YetToStart_Process_Download");

            }
            return output;
        }

        public string Process_Excel_File()
        {
            string output = "";

            Get_Data_Utility obj = new Get_Data_Utility();
            DML_Utility objDML = new DML_Utility();
            JsonCreater JsonCreater = new Read_File_Processor.JsonCreater();
            try
            {

            }
            catch (Exception ex)
            {
                output = "exception : " + ex.Message.ToString();
                ////// Exception Log ///
                //DML_Utility objDML = new DML_Utility();
                int iException = objDML.Add_Exception_Log("wipro173 exception : " + ex.Message, "");
            }
            return output;

        }

        //public string Execute_Excel_YetToStart()
        //{
        //    string output = "";

        //    Get_Data_Utility obj = new Get_Data_Utility();
        //    DML_Utility objDML = new DML_Utility();
        //    JsonCreater JsonCreater = new Read_File_Processor.JsonCreater();
        //    try
        //    {
        //        //Get_Data_Utility obj = new Get_Data_Utility();
        //        //DML_Utility objDML = new DML_Utility();
        //        string strDateKey = System.DateTime.Now.ToString("yyyyMMddHHmmss");

        //        // Read RABITMQ for Response //
        //        string Response_Data = "";
        //        string Response_Error = "";
        //        RabbitMQ_Utility objQueue = new RabbitMQ_Utility();

        //        //bool retResponse = objQueue.Receive("Response", out Response_Data, out Response_Error);
        //        bool retResponse = objQueue.Receive(RabbitMQ_Utility.RabbitMQResponseQueue, out Response_Data, out Response_Error);

        //        if (!string.IsNullOrEmpty(Response_Data))
        //        {
        //            string responce_MessageId = "";
        //            string responce_Status = "";
        //            string responce_Module = "";
        //            //string RequestType = "DOWNLOAD";
        //            string path = Read_Json(Response_Data, out responce_MessageId, out responce_Status, out responce_Module);

        //            if (responce_Status.ToLower() == "y" || responce_Status.ToLower() == "yes")
        //            {
        //                // Add Response into Database //
        //                long reqId = obj.Get_Request_Id(responce_MessageId, "DOWNLOAD");
        //                if (reqId > 0)
        //                {
        //                    // Add Response to Database Table //
        //                    if (objDML.Add_Response_Json(reqId, Response_Data) > 0)
        //                    {
        //                        if (!string.IsNullOrEmpty(path))
        //                        {
        //                            if (File.Exists(path))
        //                            {
        //                                //string path = FilePath_Container.FilePath + FilePath_Container.FileName_YetToStart;
        //                                DataTable dt = Read_Excel("Sheet1", path);
        //                                if (dt.Rows.Count > 0)
        //                                {
        //                                    output = addData_YetToStart(dt, strDateKey);
        //                                    if (output == "OK")
        //                                    {
        //                                        // Get Data after compare for New Request Id

        //                                        List<tbl_input_request_data> lst = new List<tbl_input_request_data>();
        //                                        List<YetToStart> lstJson = new List<YetToStart>();
        //                                        YetToStart objJson = new YetToStart();
        //                                        lst = obj.Get_New_Request_Id_List(strDateKey);

        //                                        foreach (var objList in lst)
        //                                        {
        //                                            //lstJson = new List<YetToStart>();
        //                                            //objJson.Request_ID = objList.Request_ID;
        //                                            //objJson.Candidate_ID = objList.Candidate_ID;
        //                                            //objJson.Associate_Id = objList.Associate_Id;
        //                                            //lstJson.Add(objJson);

        //                                            //var json = JsonConvert.SerializeObject(lstJson);
        //                                            string MessageId = Guid.NewGuid().ToString();
        //                                            output = JsonCreater.getDetails_FreshCase(objList.Request_ID, objList.Associate_Id, objList.Candidate_ID, objList.DOJ, MessageId);
        //                                            /////////////////////////// Add Json request into database //////////////
        //                                            int iDML = objDML.Add_Request_Json_Detail(MessageId, "YET2START", output);
        //                                            if (iDML == 1)
        //                                            {
        //                                                bool ret = Write_JSON_TO_RABBIT_MQ(output);
        //                                                output = ret ? "Success" : "Failed";
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    output = "not exist";
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        output = "exception : " + ex.Message.ToString();
        //        ////// Exception Log ///
        //        //DML_Utility objDML = new DML_Utility();
        //        int iException = objDML.Add_Exception_Log(ex.Message, "");

        //    }
        //    return output;
        //}

        public string Read_Json_TagWise(string json, string TagName, string SubTagName)
        {
            string TagValue = "";
            try
            {
                JObject rss = JObject.Parse(json);
                TagValue = (string)rss[TagName][SubTagName];
            }
            catch (Exception ex)
            {
                TagValue = "";
                throw ex;
            }
            return TagValue;
        }
        public string Read_Json_Data_TagWise(string json, string TagName, string SubTagName)
        {
            string TagValue = "";
            try
            {
                JObject rss = JObject.Parse(json);
                JArray ja = JArray.Parse(rss[TagName].ToString());
                JObject rss1 = JObject.Parse(ja[0].ToString());
                TagValue = rss1[SubTagName].ToString();
            }
            catch (Exception ex)
            {
                TagValue = "";
                throw ex;
            }
            return TagValue;
        }
        public string Read_Json_Case_Creation_TagValue(string json, string Tag, string TagValue)
        {
            string retValue = "";
            try
            {
                JObject rss = JObject.Parse(json);
                retValue = (string)rss[Tag][TagValue];
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return retValue;
        }

        public string Read_Json(string json, out string MessageId, out string rssStatus, out string rssModule)
        {
            string FilePath = "";
            try
            {
                JObject rss = JObject.Parse(json);
                MessageId = (string)rss["Header"]["MessageId"];
                rssModule = (string)rss["Header"]["Module"];
                rssStatus = (string)rss["Data"]["Status"];
                string rssFilePath = (string)rss["Data"]["FilePath"];
                string rssFileName = (string)rss["Data"]["FileName"];

                FilePath = rssFilePath;// + rssFileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return FilePath;
        }

        public DataTable ConvertToCSV(string filePath)
        {
            //Will hold all rows of table as CSV
            string contents = File.ReadAllText(filePath);
            contents = contents.Replace("<table>", "");
            contents = contents.Replace("<tr>", "");
            contents = contents.Replace("</table>", "");
            string[] allrow = contents.Split(new string[] { "</tr>" }, StringSplitOptions.None);
            DataTable dt = new DataTable();
            foreach (var row in allrow)
            {
                if (row.Contains("<th>"))
                {
                    string[] allcols = row.Split(new string[] { "</th>" }, StringSplitOptions.None);
                    foreach (var col in allcols)
                    {
                        dt.Columns.Add(col.Replace(Environment.NewLine, "").Replace("<th>", "").Trim());
                    }
                }
                else
                {
                    string[] allcols = row.Split(new string[] { "</td>" }, StringSplitOptions.None);
                    DataRow dr = dt.NewRow();
                    int i = 0;
                    foreach (var col in allcols)
                    {
                        string val = col.Replace(Environment.NewLine, "").Replace("<td>", "").Replace("<text>", "").Replace("</text>", "").Trim();
                        dr[i] = val;
                        i++;
                    }
                    dt.Rows.Add(dr);

                }
            }
            return dt;
        }
        public DataTable Read_Excel(string sheetName, string path)
        {
            DataTable dt = new DataTable();
            try
            {
                using (OleDbConnection conn = new OleDbConnection())
                {
                    string Import_FileName = path;
                    string fileExtension = Path.GetExtension(Import_FileName);
                    if (fileExtension.ToLower() == ".xls")
                        conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Import_FileName + ";" + "Extended Properties='Excel 8.0;HDR=YES;IMEX=1;'";
                    else if (fileExtension == ".xlsx")
                        conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Import_FileName + ";" + "Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;'";
                    using (OleDbCommand comm = new OleDbCommand())
                    {
                        comm.CommandText = "Select * from [" + sheetName + "$]";

                        comm.Connection = conn;

                        using (OleDbDataAdapter da = new OleDbDataAdapter())
                        {
                            da.SelectCommand = comm;
                            da.Fill(dt);
                            //return dt;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                dt = null;
                throw ex;
            }
            return dt;
        }

        public DataTable Read_CSV(string path)
        {
            try
            {


                string contents = File.ReadAllText(path + ".csv");

                string[] allrow = contents.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                DataTable dt = new DataTable();
                bool IsHeader = true;
                foreach (var row in allrow)
                {
                    string[] allcols = row.Split(new string[] { "," }, StringSplitOptions.None);
                    DataRow dr = dt.NewRow();
                    int i = 0;
                    foreach (var col in allcols)
                    {
                        if (IsHeader)
                            dt.Columns.Add(col.Trim());
                        else
                        {
                            if (i < dt.Columns.Count)
                                dr[i] = col;
                            i++;
                        }
                    }
                    if (!string.IsNullOrEmpty(dr[0].ToString()))
                        dt.Rows.Add(dr);
                    IsHeader = false;
                }
                return dt;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public DataTable Import(String path)
        {
            DML_Utility objDML = new DML_Utility();

            try
            {


                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook workBook = app.Workbooks.Open(path, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                Microsoft.Office.Interop.Excel.Worksheet workSheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.ActiveSheet;

                int index = 0;
                object rowIndex = 2;
                int colindex = 1;

                DataTable dt = new DataTable();
                //dt.Columns.Add("FirstName");
                //dt.Columns.Add("LastName");
                //dt.Columns.Add("Mobile");
                //dt.Columns.Add("Landline");
                //dt.Columns.Add("Email");
                //dt.Columns.Add("ID");

                DataRow row;
                while (((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[1, colindex]).Value2 != null)
                {
                    ++colindex;
                }
                for (int i = 1; i < colindex; i++)
                {
                    dt.Columns.Add(Convert.ToString(((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[1, i]).Value2));
                    //dt.Columns.Add();
                }

                while (((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 1]).Value2 != null)
                {
                    rowIndex = 2 + index;
                    row = dt.NewRow();
                    for (int i = 1; i < colindex; i++)
                    {
                        row[i - 1] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, i]).Value2);
                    }


                    //row[0] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 1]).Value2);
                    //row[1] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 2]).Value2);
                    //row[2] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 3]).Value2);
                    //row[3] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 4]).Value2);
                    //row[4] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 5]).Value2);
                    index++;
                    dt.Rows.Add(row);
                }
                app.Workbooks.Close();
                return dt;
            }
            catch (Exception ex)
            {

                objDML.Add_Exception_Log("wipro173 exception : " + ex.Message, "");
                throw ex;
            }
        }

        public int addData(DataTable dt)
        {
            int intValue = 0;
            fadv_touchlessEntities entit = new fadv_touchlessEntities();
            tbl_initiation_tracker tbl = new tbl_initiation_tracker();
            List<tbl_initiation_tracker> lst = new List<tbl_initiation_tracker>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    tbl = new tbl_initiation_tracker();
                    tbl.request_id = dr["Request ID"].ToString().Trim();
                    tbl.req_date = dr["Date"].ToString().Trim();
                    tbl.candidate_id = dr["Candidate ID"].ToString().Trim();
                    tbl.associate_id = dr["AssociateId"].ToString().Trim();
                    tbl.bgv_type = dr["BGV Type"].ToString().Trim();
                    tbl.package = dr["Package"].ToString().Trim();
                    tbl.account = dr["Account"].ToString().Trim();
                    tbl.name = dr["Name"].ToString().Trim();
                    tbl.allocated_to = dr["Allocated To"].ToString().Trim();
                    tbl.docs_download = dr["Docs Downloaded"].ToString().Trim();
                    tbl.status = dr["Status"].ToString().Trim();
                    tbl.crn = dr["CRN"].ToString().Trim();
                    tbl.creation_date = dr["Creation Date"].ToString().Trim();
                    tbl.drug_panel = dr["drug panel"].ToString().Trim();
                    tbl.req_date1 = dr["Date1"].ToString().Trim();
                    lst.Add(tbl);
                }
                entit.tbl_initiation_tracker.AddRange(lst);
                entit.SaveChanges();
                intValue = 1;
            }
            catch (Exception ex)
            {
                intValue = -1;
                throw ex;
            }
            return intValue;
        }

        public string addData_YetToStart(DataTable dt, string strDateKey)
        {
            string Msg = "";
            DML_Utility objDML = new DML_Utility();

            fadv_touchlessEntities entit = new fadv_touchlessEntities();
            tbl_input_request_data tbl = new tbl_input_request_data();
            List<tbl_input_request_data> lst = new List<tbl_input_request_data>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    tbl = new tbl_input_request_data();
                    tbl.Account = dr["Account"].ToString().Trim();
                    tbl.Account_Group = dr["Account Group"].ToString().Trim();
                    tbl.Additional_Payment_Status = dr["Additional Payment Status"].ToString().Trim();
                    tbl.Associate_Id = dr["AssociateId"].ToString().Trim();
                    tbl.BGV_Type = dr["BGV Type"].ToString().Trim();
                    tbl.BU = dr["BU"].ToString().Trim();
                    tbl.Candidate_ID = dr["CandidateId"].ToString().Trim();
                    tbl.CE_Available = dr["CE Available(Yes/No)"].ToString().Trim();
                    tbl.CE_BGV_Initiated_Date = dr["CE-BGV Initiated date"].ToString().Trim();
                    tbl.Components = dr["Components"].ToString().Trim();
                    tbl.Department = dr["Department"].ToString().Trim();
                    tbl.DOJ = dr["DOJ"].ToString().Trim();
                    tbl.HR_POC = dr["HR POC"].ToString().Trim();
                    tbl.Last_Updated_On = dr["Last Updated On"].ToString().Trim();
                    tbl.Name = dr["Name"].ToString().Trim();
                    tbl.Pre_BGV_Initiated_Date = dr["Pre-BGV Initiated Date"].ToString().Trim();
                    tbl.Project = dr["Project"].ToString().Trim();
                    tbl.Report_uploaded_date = dr["Report uploaded date"].ToString().Trim();
                    //tbl.Request_Date = dr["Date1"].ToString().Trim();
                    tbl.Request_ID = dr["RequestId"].ToString().Trim();
                    tbl.Vendor_Status = dr["Vendor Status"].ToString().Trim();
                    tbl.Work_Location = dr["Work Location"].ToString().Trim();
                    tbl.ImportKey = Convert.ToInt64(strDateKey.ToString());

                    lst.Add(tbl);
                }
                entit.tbl_input_request_data.AddRange(lst);
                entit.SaveChanges();
                Msg = "OK";
            }
            catch (Exception ex)
            {
                Msg = "ex";
                objDML.Add_Exception_Log("wipro173 exception : " + ex.Message, "");
                throw ex;
            }
            return Msg;
        }


        #region Add JSON into RABBITMQ
        public bool Write_JSON_TO_RABBIT_MQ(string json)
        {
            bool ret = false;
            string error = "";
            try
            {
                RabbitMQ_Utility objQueue = new RabbitMQ_Utility();
                //ret = objQueue.Rabbit_Send(json, "Request", "localhost", out error);
                ret = objQueue.Rabbit_Send(json, RabbitMQ_Utility.RabbitMQRequestQueue, RabbitMQ_Utility.RabbitMQUrl, out error);
            }
            catch (Exception ex)
            {
                error = "ex";
                ////// Exception Log ///
                throw ex;
            }
            return ret;
        }

        public bool Write_JSON_TO_ServerRABBIT_MQ(string json)
        {
            bool ret = false;
            string error = "";
            try
            {
                RabbitMQ_Utility objQueue = new RabbitMQ_Utility();
                //ret = objQueue.Rabbit_Send(json, "Request", "localhost", out error);
                ret = objQueue.ServerRabbit_Send(json, RabbitMQ_Utility.ServerQueue, RabbitMQ_Utility.RabbitMQUrl, out error);
            }
            catch (Exception ex)
            {
                error = "ex";
                ////// Exception Log ///
                throw ex;
            }
            return ret;
        }


        public string Write_JSON_TO_Download()
        {
            bool ret = false;
            string output = "";
            string error = "";
            DML_Utility objDML = new DML_Utility();
            Get_Data_Utility objGet = new Get_Data_Utility();
            try
            {
                //objDML.Add_Exception_Log(ts.TotalMinutes.ToString(), "");
                // Check Last Request //
                int Flag = 1;
                List<tbl_request_details> lstObj = new List<tbl_request_details>();
                lstObj = objGet.Get_Last_Request(FilePath_Container.ServiceId_WiproDownload);

                if (lstObj.Count > 0)
                {
                    double timeMinutes = 0;
                    DateTime dtReq = (DateTime)lstObj[0].createdOn;
                    //DateTime dt = System.DateTime.Now;
                    DateTime dt = System.DateTime.Now.AddMinutes(0);
                    TimeSpan ts = dt - dtReq;
                    List<tbl_config_value> lstConfig = objGet.Get_Cofig_Details("Wipro173DOWNLOADREQUEST");
                    foreach (var ob in lstConfig)
                    {
                        double.TryParse(ob.configstring, out timeMinutes);
                    }
                    timeMinutes = timeMinutes <= 0 ? 59 : timeMinutes;
                    //int iException1 = objDML.Add_Exception_Log(ts.TotalMinutes.ToString(), dt.ToString());
                    if (ts.TotalMinutes > timeMinutes)
                    {
                        //objDML.Add_Exception_Log("Wipro log : " + (ts.TotalMinutes).ToString(), "Download request sent");
                        Flag = 1;
                    }
                    else
                    {
                        Flag = 0;
                    }
                }

                if (Flag > 0)
                {
                    string MessageId = Guid.NewGuid().ToString();
                    string ServiceId = FilePath_Container.ServiceId_WiproDownload.ToString();// "DOWNLOAD";// Guid.NewGuid().ToString();
                    ///////////////////////// Add Json request into database //////////////
                    JsonCreater JsonCreater = new Read_File_Processor.JsonCreater();
                    string json = JsonCreater.getDownload(MessageId, ServiceId);
                    int iDML = objDML.Add_Request_Json_Detail(MessageId, ServiceId, json);  // Add_Request_Json(MessageId, "DOWNLOAD", json);
                    ///////////////////////// Add Json request into Rabbit MQ //////////////
                    if (iDML == 1)
                    {
                        ret = Write_JSON_TO_RABBIT_MQ(json);
                        output = ret ? "Success" : "Failed";
                    }
                }
                else
                {
                    // READ RESPONSE QUEUE //
                    Read_Response();
                }
            }
            catch (Exception ex)
            {

                output = "wipro173 exception : " + ex.Message.ToString();
                ////// Exception Log ///
                //DML_Utility objDML = new DML_Utility();
                //int iException = objDML.Add_Exception_Log(ex.Message, "Write_JSON_TO_Download");
            }
            return output;
        }

        #endregion
    }
}

