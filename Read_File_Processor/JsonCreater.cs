﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Entities;
using DataAccess_Utility;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Read_File_Processor
{
    public class JsonCreater
    {
        public string getHeader(string processName, string processId, string stageId, string requestType, string task, string taskDesc,
            string taskGroupId, string requestId, string version, string attempt, string multiTask, string requestAuthToken)
        {
            string header = "\"metadata\":{";

            header = header + "\"processName\":\"" + processName + "\",";
            header = header + "\"processId\":\"" + processId + "\",";
            header = header + "\"stageId\":\"" + stageId + "\",";
            header = header + "\"requestType\":\"" + requestType + "\",";
            header = header + "\"task\":\"" + task + "\",";
            header = header + "\"taskDesc\":\"" + taskDesc + "\",";
            header = header + "\"taskGroupId\":\"" + taskGroupId + "\",";
            header = header + "\"requestDate\":\"" + DateTime.Now.ToString() + "\",";
            header = header + "\"requestId\":\"" + requestId + "\",";
            header = header + "\"version\":\"" + version + "\",";
            header = header + "\"attempt\":\"" + attempt + "\",";
            header = header + "\"multiTask\":\"" + multiTask + "\",";
            header = header + "\"requestAuthToken\":\"" + requestAuthToken + "\"";

            header = header + "}";

            return header;
        }

        public string getDetails(string jsonArray, string MessageId)
        {
            string header = getHeader("CSPi Touchless App", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "query", "DownloadData173", "Download Data from Customer Portal", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "1.0.0", "1", "no", "jwt.token");
            string status = getStatus("value", "Desc", "details");

            string finalJson = "{";
            finalJson = finalJson + header + ",\"Data\":" + jsonArray + "," + status + "}";
            return finalJson;
        }

        public string getDetails_FreshCase(string resumeNumber, string candidateName, string MessageId,string sublogin, string ServiceId = "FreshCase173")
        {
            string header = getHeader("CSPi Touchless App", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "query", "FreshCase173", "Download Data from Customer Portal", Guid.NewGuid().ToString(), MessageId, "1.0.0", "1", "no", "jwt.token");
            string status = getStatus("value", "Desc", "details");

            //string data = "\"Data\":{";
            //data = data + "\"Candidte_Id\":\"" + Candidte_Id + "\",";
            //data = data + "\"Employee_No\":\"" + Employee_No + "\",";
            //data = data + "\"Employee_Name\":\"" + Employee_Name + "\",";
            //data = data + "\"Mapping_Date\":\"" + Mapping_Date + "\",";
            //data = data + "\"Check_Initiated\":\"" + Check_Initiated + "\"";
            //data = data + "}";



            string data = "\"data\":[{";
            data = data + "\"taskName\":\"downloadCaseData173\",";
            data = data + "\"taskId\":\"" + Guid.NewGuid() + "\",";
            data = data + "\"taskSerialNo\":1,";
            data = data + "\"taskSpecs\":{\"downloadData173\":{\"customerName\":\"Wipro Technologies\",\"resumeNumber\":\"" + resumeNumber.Replace("\"", "").Trim() + "\",\"subLogin\":\""+ sublogin + "\",\"candidateName\":\"" + candidateName.Replace("\"", "").Trim() + "\"}}";
            data = data + "}]";

            string finalJson = "{";
            finalJson = finalJson + header + "," + data + "}";// "," + status +
            return finalJson;
        }

        public string getStatus(string value, string Des, string Details)
        {
            string status = "\"Status\":{"; ;
            status = status + "\"Value\":\"" + value + "\",";
            status = status + "\"Description\":\"" + Des + "\",";
            status = status + "\"Details\":\"" + Details + "\"";
            status = status + "}";
            return status;
        }

        public string getEmptyDataJson(string Details, string Response_Status = "", string FileName_YetToStart = "", string FilePathNew = "")
        {
            string data = "\"data\":[{";
            data = data + "\"taskName\":\"downloadData173\",";
            data = data + "\"taskId\":\"" + Guid.NewGuid() + "\",";
            data = data + "\"taskSerialNo\":1,";
            data = data + "\"taskSpecs\":{\"downloadData173\":{\"customerName\":\"Wipro Technologies\",\"action\":\"download\",\"dataType\":\"excel\"}}";
            data = data + "}]";
            return data;
        }

        public string getDownload(string MessageId, string ServiceID = "ServiceID")
        {
            string header = getHeader("CSPi Touchless App", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "query", "DownloadData173", "Download Data from Customer Portal", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "1.0.0", "1", "no", "jwt.token");
            string status = getStatus("", "", "");
            string data = getEmptyDataJson("Download Records");
            string finalJson = "{";
            finalJson = finalJson + header + "," + data + "}";//+ "," + status
            return finalJson;
        }

        public string getDownload_Response(string MessageId, string ServiceID = "ServiceID", string Response_Status = "N", string Details = "Download Excel")
        {
            //string header = getHeader("Response", "ResponseID", "DOWNLOAD", MessageId, DateTime.Now.ToString(), "DOWNLOAD", "0", "BOT1", "0");
            string header = getHeader("CSPi Touchless App", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "query", "DownloadData173", "Download Data from Customer Portal", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "1.0.0", "1", "no", "jwt.token");
            string status = getStatus("", "", "");

            string data = "\"data\":[{";
            data = data + "\"Details\":\"" + Details + "\",";
            data = data + "\"Status\":\"" + Response_Status + "\",";
            data = data + "\"FileName\":\"" + FilePath_Container.FileName_YetToStart + "\",";
            data = data + "\"FilePath\":\"" + FilePath_Container.FilePathNew + "\"";
            data = data + "}]";

            string finalJson = "{";
            finalJson = finalJson + header + "," + data + "," + status + "}";
            return finalJson;
        }

        public string getData_FreshCaseResponse()
        {
            string header = getHeader("CSPi Touchless App", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "query", "DownloadData173", "Download Data from Customer Portal", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "1.0.0", "1", "no", "jwt.token");
            string status = getStatus("", "", "");

            string data = "\"Data\":{";
            data = data + "\"cognizent_tech_solution\":\"cognizent_tech_solution\",";
            data = data + "\"clientcode\":\"clientcode\",";
            data = data + "\"candidate_name\":\"candidate_name\",";
            data = data + "\"client_ref_no\":\"client_ref_no\",";
            data = data + "\"bvg_type\":\"bvg_type\",";
            data = data + "\"package\":\"package\",";
            data = data + "\"specification\":\"specification\",";
            data = data + "\"project_id\":\"project_id\",";
            data = data + "\"project_name\":\"project_name\",";
            data = data + "\"doj\":\"doj\",";
            data = data + "\"request_id\":\"request_id\",";
            data = data + "\"associate_id\":\"associate_id\",";
            data = data + "\"candidate_id\":\"candidate_id\",";
            data = data + "\"employee_id\":\"employee_id\",";
            data = data + "\"account_name\":\"account_name\",";
            data = data + "\"tensse\":\"tensse\",";
            data = data + "\"actual_case_created\":\"actual_case_created\",";
            data = data + "\"First_Name\":\"First_Name\",";
            data = data + "\"Last_Name\":\"Last_Name\",";
            data = data + "\"Date_Of_Birth\":\"Date_Of_Birth\",";
            data = data + "\"Father_Name\":\"Father_Name\",";
            data = data + "\"Nationality\":\"Nationality\",";
            data = data + "\"Mobile_Number\":\"Mobile_Number\",";
            data = data + "\"Current_Address\":\"Current_Address\",";
            data = data + "\"Permanent_Address\":\"Permanent_Address\",";
            data = data + "\"Longest_Stay_Address\":\"Longest_Stay_Address\"";
            data = data + "}";

            string finalJson = "{";
            finalJson = finalJson + header + "," + data + "," + status + "}";
            return finalJson;

        }

        public string Final_Create_Case_Json(string MessageId, tbl_Wipro_Details CandidateDetails, string package, string Database_Sent, string PONumber, string fileupload, string clientID, string SBUID, string clientName, string Service_Id = "CaseCreation", string RequestID = "", string sbuName = "")
        {
            string header = getHeader("CSPi Touchless App", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "query", "DownloadData173", "Download Data from Customer Portal", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "1.0.0", "1", "no", "jwt.token");
            string status = getStatus("", "", "");
            string data = "\"Data\":{";
            data += "\"CaseCreation\":{";
            //data = data + Final_Create_Case_Json_DataTag(lstFreshCase, lstCollege);
            data = data + Final_Create_Case_Json_DataWipro(CandidateDetails, package, Database_Sent, PONumber, fileupload, clientID, SBUID, clientName, RequestID, sbuName);
            data = data + "}";

            string finalJson = "{";
            finalJson = finalJson + header + "," + data + "}," + status + "}";
            return finalJson.Replace("’", "");

        }
        public string Final_Create_Case_Json_DataWipro(tbl_Wipro_Details CandidateDetails, string package, string Database_Sent, string PONumber, string fileupload, string clientid,
            string SBUID, string ClientName, string RequestID, string sbuName)
        {
            ///////////////// Scraping Data Variables ////////////////////


            string LOA_Status = "";

            ///////////////// Yet2Start Data Variables ////////////////////


            ///////////////// MISLog Data Variables ////////////////////


            // Start Creating Json //
            string crnCreatedDate = "";//System.DateTime.Now.ToString();// "Need To Discuss"; // cuurent date
            string middleName = ""; //"Need To Discuss";
            string sbuEntitiesId = SBUID;
            string sbu = sbuName;
            string subjectDetails = "FADV";
            string subjectType = "Candidate";
            string typeOfCheck = "SRT";
            string authorizationLetter = "Yes";// LOA_Status;
            string packageType = "Soft Copy";
            string srtData = "SRT";
            string lOASubmited = "Yes";// LOA_Status; //
            string bVFSubmitted = "Yes";
            string isClientSpecificField = "true";
            //string request_field_id = "Need To Discuss";
            string miscrn = "";
            string last_name = "";
            string gender = ""; //"Need To Discuss";
            string Position = ""; //"Need To Discuss";
            string IdentityDocument = ""; //"Need To Discuss";

            string Unique_Identifier = ""; //"Need To Discuss";
            string comment = ""; //"Need To Discuss";
            string request_status = ""; //"Need To Discuss";
            string clientName = ClientName; //"Need To Discuss";
            string File_Upload = fileupload; //"Need To Discuss";
            string clientrefNo = "";

            if (sbuName.Contains("Laterals - Pre employment"))
            {
                Database_Sent = "";
                clientrefNo = CandidateDetails.Candidte_Id;
            }
            else if (sbuName.Contains("Laterals - Post Employment"))
            {

                clientrefNo = CandidateDetails.Employee_No;
            }
            else if (sbuName.Contains("Laterals - Pre Joining"))
            {
                clientrefNo = "";
            }
            DateTime dob = Convert.ToDateTime(CandidateDetails.Date_Of_Birth, CultureInfo.InvariantCulture);// DateTime.ParseExact(CandidateDetails.Date_Of_Birth, "MM/dd/yyyy",CultureInfo.InvariantCulture);
            DateTime doj = Convert.ToDateTime(CandidateDetails.Date_Of_Joining, CultureInfo.InvariantCulture);// DateTime.ParseExact(CandidateDetails.Date_Of_Birth, "MM/dd/yyyy",CultureInfo.InvariantCulture);
            string jsonDownload = "";
            jsonDownload = jsonDownload + "\"requestId\": \"" + RequestID + "\",";
            jsonDownload = jsonDownload + "\"clientId\": \"" + clientid + "\",";
            jsonDownload = jsonDownload + "\"crnNo\": \"" + miscrn + "\",";
            jsonDownload = jsonDownload + "\"crnCreatedDate\": \"" + crnCreatedDate + "\",";
            jsonDownload = jsonDownload + "\"firstName\": \"" + CandidateDetails.Employee_Name + "\",";
            jsonDownload = jsonDownload + "\"middleName\": \"" + middleName + "\",";
            jsonDownload = jsonDownload + "\"lastName\": \"" + last_name + "\",";
            jsonDownload = jsonDownload + "\"dateOfBirth\": \"" + dob.ToString("dd/MM/yyyy") + "\",";// Convert.ToDateTime(CandidateDetails.Date_Of_Birth).ToString("dd/MM/yyyy") 
            jsonDownload = jsonDownload + "\"startDate\": \"" + doj.ToString("dd/MM/yyyy") + "\",";

            jsonDownload = jsonDownload + "\"sbuEntitiesId\": \"" + sbuEntitiesId + "\",";
            jsonDownload = jsonDownload + "\"clientReferenceNo\": \"" + ((CandidateDetails.Candidte_Id != "0") ? CandidateDetails.Candidte_Id : CandidateDetails.Employee_No) + "\",";//client_ref_no

            //jsonDownload = jsonDownload + "\"clientReferenceNo\": \"" + "278916" + "\",";//client_ref_no
            jsonDownload = jsonDownload + "\"subjectDetails\": \"" + subjectDetails + "\",";
            jsonDownload = jsonDownload + "\"subjectType\": \"" + subjectType + "\",";
            jsonDownload = jsonDownload + "\"typeOfCheck\": \"" + typeOfCheck + "\",";
            jsonDownload = jsonDownload + "\"authorizationLetter\": \"" + authorizationLetter + "\",";
            jsonDownload = jsonDownload + "\"packageType\": \"" + packageType + "\",";
            jsonDownload = jsonDownload + "\"srtData\": \"" + srtData + "\",";
            jsonDownload = jsonDownload + "\"lOASubmited\": \"" + lOASubmited + "\",";
            jsonDownload = jsonDownload + "\"bVFSubmitted\": \"" + bVFSubmitted + "\",";
            jsonDownload = jsonDownload + "\"isClientSpecificField\": \"" + isClientSpecificField + "\",";
            jsonDownload = jsonDownload + "\"clientSpecificFields\":{";

            jsonDownload = jsonDownload + clientSpecificFields("Father’s Name", CandidateDetails.Fathers_Name, CandidateDetails.Fathers_Name, "43") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Candidate Identification Number", CandidateDetails.Candidte_Id, CandidateDetails.Candidte_Id, "81") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("PO Number", PONumber, PONumber, "83") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Database sent", Database_Sent, Database_Sent, "138") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("SOW Name", "", "", "216") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Location", CandidateDetails.Posting_Location, CandidateDetails.Posting_Location, "67") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Client Cost Code", PONumber, PONumber, "139") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Gender", "", "", "119") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Actual case created date", "", "", "143");

            jsonDownload = jsonDownload + "},";
            jsonDownload = jsonDownload + "\"comment\": \"" + comment + "\",";
            jsonDownload = jsonDownload + "\"request_status\": \"" + request_status + "\",";
            jsonDownload = jsonDownload + "\"packageList\": [\"" + package + "\"],"; // Need to Discuss
            jsonDownload = jsonDownload + "\"clientName\": \"" + clientName + "\",";
            jsonDownload = jsonDownload + "\"sbuEntitiesName\": \"" + sbu + "\",";
            jsonDownload = jsonDownload + "\"packageName\": \"" + package + "\","; // Need to Discuss
            jsonDownload = jsonDownload + "\"File_Upload\": \"" + File_Upload + "\"";
            return jsonDownload;
        }


        public string Final_Create_Case_Json_Data(List<tbl_yettostart_casecreation_data> lstFreshCase, List<tbl_college_details> lstCollege,
            List<tbl_input_request_data> lstYet2StartData, List<tbl_initiation_tracker> lstMISLog, string fileupload, string clientid,
            string SBUID, string ClientName, string RequestID, string sbuName)
        {
            ///////////////// Scraping Data Variables ////////////////////
            string cognizent_tech_solution = "";
            string clientcode = clientid;
            string candidate_name = "";
            string client_ref_no = "";
            string bvg_type = "";
            string package = "";
            string specification = "";
            string project_id = "";
            string project_name = "";
            string doj = "";
            string employee_id = "";
            string account_name = "";
            string tensse = "";
            string actual_case_created = "";
            string reference_type_ut = "";
            string reference_type_cvt = "";
            string company_name_ut = "";
            string company_name_cvt = "";
            string id_ut = "";
            string id_cvt = "";
            string employment_type_ut = "";
            string employment_type_cvt = "";
            string first_name = "";
            string last_name = "";
            string date_of_birth = "";
            string father_name = "";
            string nationality = "";
            string mobile_number = "";
            string current_address = "";
            string permanent_address = "";
            string longest_stay_address = "";
            string LOA_Status = "";

            foreach (var obj in lstFreshCase)
            {
                cognizent_tech_solution = obj.cognizent_tech_solution;
                clientcode = obj.clientcode;
                candidate_name = obj.candidate_name;
                client_ref_no = obj.client_ref_no;
                bvg_type = obj.bvg_type;
                package = obj.package;
                specification = obj.specification;
                project_id = obj.project_id;
                project_name = obj.project_name;
                doj = obj.doj;
                employee_id = obj.employee_id;
                account_name = obj.account_name;
                tensse = obj.tensse;
                //actual_case_created = obj.actual_case_created;
                reference_type_ut = obj.reference_type_ut;
                reference_type_cvt = obj.reference_type_cvt;
                company_name_ut = obj.company_name_ut;
                company_name_cvt = obj.company_name_cvt;
                id_ut = obj.id_ut;
                id_cvt = obj.id_cvt;
                employment_type_ut = obj.employment_type_ut;
                employment_type_cvt = obj.employment_type_cvt;
                first_name = obj.first_name;
                last_name = obj.last_name;
                date_of_birth = obj.date_of_birth;
                father_name = obj.father_name;
                nationality = obj.nationality;
                mobile_number = obj.mobile_number;
                current_address = obj.current_address;
                permanent_address = obj.permanent_address;
                longest_stay_address = obj.longest_stay_address;
                LOA_Status = obj.LOA_Status;
            }

            ///////////////// Yet2Start Data Variables ////////////////////
            string yetBGV_Type = "";
            string yetName = "";
            string yetDOJ = "";
            string yetBU = "";
            string yetDepartment = "";
            string yetAccount_Group = "";
            string yetAccount = "";
            string yetProject = "";
            string yetComponents = "";
            string yetPre_BGV_Initiated_Date = "";
            string yetCE_BGV_Initiated_Date = "";
            string yetCE_Available = "";
            string yetVendor_Status = "";
            string yetAdditional_Payment_Status = "";
            string yetHR_POC = "";
            string yetWork_Location = "";
            string yetReport_uploaded_date = "";
            string yetLast_Updated_On = "";
            string CTS_Request_ID = ""; //"Need To Discuss";
            string CTS_Associate_ID = ""; //"Need To Discuss";
            foreach (var objYet in lstYet2StartData)
            {
                yetBGV_Type = objYet.BGV_Type;
                yetName = objYet.Name;
                yetDOJ = objYet.DOJ;
                yetBU = objYet.BU;
                yetDepartment = objYet.BU;
                yetAccount_Group = objYet.Account_Group;
                yetAccount = objYet.Account;
                yetProject = objYet.Project;
                yetComponents = objYet.Components;
                yetPre_BGV_Initiated_Date = objYet.Pre_BGV_Initiated_Date;
                yetCE_BGV_Initiated_Date = objYet.CE_BGV_Initiated_Date;
                yetCE_Available = objYet.CE_Available;
                yetVendor_Status = objYet.Vendor_Status;
                yetAdditional_Payment_Status = objYet.Additional_Payment_Status;
                yetHR_POC = objYet.HR_POC;
                yetWork_Location = objYet.Work_Location;
                yetReport_uploaded_date = objYet.Report_uploaded_date;
                yetLast_Updated_On = objYet.Last_Updated_On;
                CTS_Request_ID = objYet.Request_ID;
                CTS_Associate_ID = objYet.Associate_Id;
                actual_case_created = objYet.Last_Updated_On;
            }

            ///////////////// MISLog Data Variables ////////////////////
            string misrequest_id = "";
            string misreq_date = "";
            string miscandidate_id = "";
            string misassociate_id = "";
            string misbgv_type = "";
            string mispackage = "";
            string misaccount = "";
            string misname = "";
            string misallocated_to = "";
            string misdocs_download = "";
            string misstatus = "";
            string miscrn = "";
            string miscreation_date = "";
            string misdrug_panel = "";
            string misreq_date1 = "";

            foreach (var objMIS in lstMISLog)
            {
                misrequest_id = objMIS.request_id;
                misreq_date = objMIS.req_date;
                miscandidate_id = objMIS.candidate_id;
                misassociate_id = objMIS.associate_id;
                misbgv_type = objMIS.bgv_type;
                mispackage = objMIS.package;
                misaccount = objMIS.account;
                misname = objMIS.name;
                misallocated_to = objMIS.allocated_to;
                misdocs_download = objMIS.docs_download;
                misstatus = objMIS.status;
                miscrn = objMIS.crn;
                miscreation_date = objMIS.creation_date;
                misdrug_panel = objMIS.drug_panel;
                misreq_date1 = objMIS.req_date1;
            }

            // Start Creating Json //
            string crnCreatedDate = "";//System.DateTime.Now.ToString();// "Need To Discuss"; // cuurent date
            string middleName = ""; //"Need To Discuss";
            string sbuEntitiesId = SBUID;
            string sbu = sbuName;
            string subjectDetails = "FADV";
            string subjectType = "Candidate";
            string typeOfCheck = "SRT";
            string authorizationLetter = LOA_Status;
            string packageType = "Soft Copy";
            string srtData = "SRT";
            string lOASubmited = LOA_Status; // "Yes";
            string bVFSubmitted = "Yes";
            string isClientSpecificField = "true";
            //string request_field_id = "Need To Discuss";

            string gender = ""; //"Need To Discuss";
            string Position = ""; //"Need To Discuss";
            string IdentityDocument = ""; //"Need To Discuss";

            string Unique_Identifier = ""; //"Need To Discuss";
            string comment = ""; //"Need To Discuss";
            string request_status = ""; //"Need To Discuss";
            string clientName = ClientName; //"Need To Discuss";
            string File_Upload = fileupload; //"Need To Discuss";

            string jsonDownload = "";
            jsonDownload = jsonDownload + "\"requestId\": \"" + RequestID + "\",";
            jsonDownload = jsonDownload + "\"clientId\": \"" + clientid + "\",";
            jsonDownload = jsonDownload + "\"crnNo\": \"" + miscrn + "\",";
            jsonDownload = jsonDownload + "\"crnCreatedDate\": \"" + crnCreatedDate + "\",";
            jsonDownload = jsonDownload + "\"firstName\": \"" + first_name + "\",";
            jsonDownload = jsonDownload + "\"middleName\": \"" + middleName + "\",";
            jsonDownload = jsonDownload + "\"lastName\": \"" + last_name + "\",";
            jsonDownload = jsonDownload + "\"dateOfBirth\": \"" + Convert.ToDateTime(date_of_birth).ToString("dd/MM/yyyy") + "\",";
            jsonDownload = jsonDownload + "\"sbuEntitiesId\": \"" + sbuEntitiesId + "\",";
            jsonDownload = jsonDownload + "\"clientReferenceNo\": \"" + ((miscandidate_id != "0") ? miscandidate_id : CTS_Associate_ID) + "\",";//client_ref_no

            //jsonDownload = jsonDownload + "\"clientReferenceNo\": \"" + "278916" + "\",";//client_ref_no
            jsonDownload = jsonDownload + "\"subjectDetails\": \"" + subjectDetails + "\",";
            jsonDownload = jsonDownload + "\"subjectType\": \"" + subjectType + "\",";
            jsonDownload = jsonDownload + "\"typeOfCheck\": \"" + typeOfCheck + "\",";
            jsonDownload = jsonDownload + "\"authorizationLetter\": \"" + authorizationLetter + "\",";
            jsonDownload = jsonDownload + "\"packageType\": \"" + packageType + "\",";
            jsonDownload = jsonDownload + "\"srtData\": \"" + srtData + "\",";
            jsonDownload = jsonDownload + "\"lOASubmited\": \"" + lOASubmited + "\",";
            jsonDownload = jsonDownload + "\"bVFSubmitted\": \"" + bVFSubmitted + "\",";
            jsonDownload = jsonDownload + "\"isClientSpecificField\": \"" + isClientSpecificField + "\",";
            jsonDownload = jsonDownload + "\"clientSpecificFields\":{";
            //jsonDownload = jsonDownload + clientSpecificFields("Fathers_Name", father_name, father_name, "43") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("CTS_Request_ID", CTS_Request_ID, CTS_Request_ID, "57") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("Gender", gender, gender, "119") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("Position", Position, Position, "49") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("Employee_Id", CTS_Associate_ID, CTS_Associate_ID, "63") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("Date_Joining", doj, doj, "55") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("Account_Name", account_name, account_name, "105") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("Department", yetDepartment, yetDepartment, "47") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("Candidate_Id", miscandidate_id, miscandidate_id, "61") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("Project_Name", project_name, project_name, "53") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("Tennessee_Date", tensse, tensse, "69") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("Identity_Document", IdentityDocument, IdentityDocument, "45") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("CTS_Associate_ID", CTS_Associate_ID, CTS_Associate_ID, "59") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("Unique_Identifier", Unique_Identifier, Unique_Identifier, "123") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("Project_Id", project_id, project_id, "51") + ",";
            //jsonDownload = jsonDownload + clientSpecificFields("Actual_Date", actual_case_created, actual_case_created, "143");// + ",";

            jsonDownload = jsonDownload + clientSpecificFields("Father’s Name", father_name, father_name, "43") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("CTS Request ID", CTS_Request_ID, CTS_Request_ID, "57") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Gender", gender, gender, "119") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Position", Position, Position, "49") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Employee ID", CTS_Associate_ID, CTS_Associate_ID, "63") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Date of Joining", doj, doj, "55") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Account Name", account_name, account_name, "105") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Department", yetDepartment, yetDepartment, "47") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Candidate ID", miscandidate_id, miscandidate_id, "61") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Project Name", project_name, project_name, "53") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Tennessee Stopped date", tensse, tensse, "69") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Identity Document", IdentityDocument, IdentityDocument, "45") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("CTS Associate ID", CTS_Associate_ID, CTS_Associate_ID, "59") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Unique Identifier", Unique_Identifier, Unique_Identifier, "123") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Project ID", project_id, project_id, "51") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Actual case created date", actual_case_created, actual_case_created, "143");// + ",";

            //jsonDownload = jsonDownload + clientSpecificFields("project_id", yetWork_Location, yetWork_Location, "67");
            jsonDownload = jsonDownload + "},";
            jsonDownload = jsonDownload + "\"comment\": \"" + comment + "\",";
            jsonDownload = jsonDownload + "\"request_status\": \"" + request_status + "\",";
            jsonDownload = jsonDownload + "\"packageList\": [\"" + package + "\"],"; // Need to Discuss
            jsonDownload = jsonDownload + "\"clientName\": \"" + clientName + "\",";
            jsonDownload = jsonDownload + "\"sbuEntitiesName\": \"" + sbu + "\",";
            jsonDownload = jsonDownload + "\"packageName\": \"" + package + "\","; // Need to Discuss
            jsonDownload = jsonDownload + "\"File_Upload\": \"" + File_Upload + "\"";
            return jsonDownload;
        }


        public string Final_Create_Case_JsonForRequests(string MessageId, tbl_Wipro_Details CandidateDetails, string package, string Database_Sent, string PONumber, string fileupload, string clientID, string SBUID, string clientName, string Service_Id = "CaseCreation", string RequestID = "", string sbuName = "")
        {
            string header = getHeader("CSPi Touchless App", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "query", "DownloadData173", "Download Data from Customer Portal", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "1.0.0", "1", "no", "jwt.token");
            string status = getStatus("", "", "");
            string data = "\"Data\":{";
            data += "\"CaseCreation\":{";
            //data = data + Final_Create_Case_Json_DataTag(lstFreshCase, lstCollege);
            data = data + Final_Create_Case_Json_DataForRequests(CandidateDetails, package, Database_Sent, PONumber, fileupload, clientID, SBUID, clientName, RequestID, sbuName);

            data = data + "}";

            string finalJson = "{";
            finalJson = finalJson + header + "," + data + "}," + status + "}";
            return finalJson.Replace("’", "");

        }
        public string Final_Create_Case_Json_DataForRequests(tbl_Wipro_Details CandidateDetails, string package, string Database_Sent, string PONumber, string fileupload, string clientid,
            string SBUID, string ClientName, string RequestID, string sbuName)

        {
            ///////////////// Scraping Data Variables ////////////////////


            string LOA_Status = "";

            ///////////////// Yet2Start Data Variables ////////////////////


            ///////////////// MISLog Data Variables ////////////////////


            // Start Creating Json //
            string crnCreatedDate = "";//System.DateTime.Now.ToString();// "Need To Discuss"; // cuurent date
            string middleName = ""; //"Need To Discuss";
            string sbuEntitiesId = SBUID;
            string sbu = sbuName;
            string subjectDetails = "FADV";
            string subjectType = "Candidate";
            string typeOfCheck = "SRT";
            string authorizationLetter = "Yes";// LOA_Status;
            string packageType = "Soft Copy";
            string srtData = "SRT";
            string lOASubmited = "Yes";// LOA_Status; //
            string bVFSubmitted = "Yes";
            string isClientSpecificField = "true";
            //string request_field_id = "Need To Discuss";
            string miscrn = "";
            string last_name = "";
            string gender = ""; //"Need To Discuss";
            string Position = ""; //"Need To Discuss";
            string IdentityDocument = ""; //"Need To Discuss";

            string Unique_Identifier = ""; //"Need To Discuss";
            string comment = ""; //"Need To Discuss";
            string request_status = ""; //"Need To Discuss";
            string clientName = ClientName; //"Need To Discuss";
            string File_Upload = fileupload; //"Need To Discuss";
            string clientrefNo = "";

            if (sbuName.Contains("Laterals - Pre employment"))
            {
                Database_Sent = "";
                clientrefNo = CandidateDetails.Candidte_Id;
            }
            else if (sbuName.Contains("Laterals - Post Employment"))
            {

                clientrefNo = CandidateDetails.Employee_No;
            }
            else if (sbuName.Contains("Laterals - Pre Joining"))
            {
                clientrefNo = "";
            }
            DateTime dob = Convert.ToDateTime(CandidateDetails.Date_Of_Birth, CultureInfo.InvariantCulture);// DateTime.ParseExact(CandidateDetails.Date_Of_Birth, "MM/dd/yyyy",CultureInfo.InvariantCulture);
            DateTime doj = Convert.ToDateTime(CandidateDetails.Date_Of_Joining, CultureInfo.InvariantCulture);// DateTime.ParseExact(CandidateDetails.Date_Of_Birth, "MM/dd/yyyy",CultureInfo.InvariantCulture);
            string jsonDownload = "";
            jsonDownload = jsonDownload + "\"requestId\": \"" + RequestID + "\",";
            jsonDownload = jsonDownload + "\"clientId\": \"" + clientid + "\",";
            jsonDownload = jsonDownload + "\"crnNo\": \"" + miscrn + "\",";
            jsonDownload = jsonDownload + "\"crnCreatedDate\": \"" + crnCreatedDate + "\",";
            jsonDownload = jsonDownload + "\"firstName\": \"" + CandidateDetails.Employee_Name + "\",";
            jsonDownload = jsonDownload + "\"middleName\": \"" + middleName + "\",";
            jsonDownload = jsonDownload + "\"lastName\": \"" + last_name + "\",";
            jsonDownload = jsonDownload + "\"dateOfBirth\": \"" + dob.ToString("dd/MM/yyyy") + "\",";// Convert.ToDateTime(CandidateDetails.Date_Of_Birth).ToString("dd/MM/yyyy") 
            jsonDownload = jsonDownload + "\"startDate\": \"" + doj.ToString("dd/MM/yyyy") + "\",";
            jsonDownload = jsonDownload + "\"sbuEntitiesId\": \"" + sbuEntitiesId + "\",";
            jsonDownload = jsonDownload + "\"clientReferenceNo\": \"" + ((CandidateDetails.Candidte_Id != "0") ? CandidateDetails.Candidte_Id : CandidateDetails.Employee_No) + "\",";//client_ref_no

            //jsonDownload = jsonDownload + "\"clientReferenceNo\": \"" + "278916" + "\",";//client_ref_no
            jsonDownload = jsonDownload + "\"subjectDetails\": \"" + subjectDetails + "\",";
            jsonDownload = jsonDownload + "\"subjectType\": \"" + subjectType + "\",";
            jsonDownload = jsonDownload + "\"typeOfCheck\": \"" + typeOfCheck + "\",";
            jsonDownload = jsonDownload + "\"authorizationLetter\": \"" + authorizationLetter + "\",";
            jsonDownload = jsonDownload + "\"packageType\": \"" + packageType + "\",";
            jsonDownload = jsonDownload + "\"srtData\": \"" + srtData + "\",";
            jsonDownload = jsonDownload + "\"lOASubmited\": \"" + lOASubmited + "\",";
            jsonDownload = jsonDownload + "\"bVFSubmitted\": \"" + bVFSubmitted + "\",";
            jsonDownload = jsonDownload + "\"isClientSpecificField\": \"" + isClientSpecificField + "\",";
            jsonDownload = jsonDownload + "\"clientSpecificFields\":{";

            jsonDownload = jsonDownload + clientSpecificFields("Fathers_Name", CandidateDetails.Fathers_Name, CandidateDetails.Fathers_Name, "43") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Candidate_Identification_Number", CandidateDetails.Candidte_Id, CandidateDetails.Candidte_Id, "81") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Po_Number", PONumber, PONumber, "83") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Database_Sent", Database_Sent, Database_Sent, "138") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Sow_Name", "", "", "216") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Location", CandidateDetails.Posting_Location, CandidateDetails.Posting_Location, "67") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Client_Code2", PONumber, PONumber, "139") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Gender", "", "", "119") + ",";
            jsonDownload = jsonDownload + clientSpecificFields("Actual_Date", "", "", "143");

            jsonDownload = jsonDownload + "},";
            jsonDownload = jsonDownload + "\"comment\": \"" + comment + "\",";
            jsonDownload = jsonDownload + "\"request_status\": \"" + request_status + "\",";
            jsonDownload = jsonDownload + "\"packageList\": [\"" + package + "\"],"; // Need to Discuss
            jsonDownload = jsonDownload + "\"clientName\": \"" + clientName + "\",";
            jsonDownload = jsonDownload + "\"sbuEntitiesName\": \"" + sbu + "\",";
            jsonDownload = jsonDownload + "\"packageName\": \"" + package + "\","; // Need to Discuss
            jsonDownload = jsonDownload + "\"File_Upload\": \"" + File_Upload + "\"";
            return jsonDownload;
        }


        public string clientSpecificFields(string JsonTag, string dataValue, string textValue, string request_field_id)
        {
            string csfJson = "\"" + JsonTag + "\":{";

            csfJson = csfJson + "\"request_field_id\":\"" + request_field_id + "\",";
            csfJson = csfJson + "\"data\":\"" + (string.IsNullOrEmpty(dataValue) ? " " : dataValue) + "\",";
            csfJson = csfJson + "\"text\":\"" + (string.IsNullOrEmpty(textValue) ? " " : textValue) + "\"";
            csfJson = csfJson + "}";

            return csfJson;
        }
        public string Final_Create_Case_Json_DataTag(List<tbl_yettostart_casecreation_data> lstFreshCase, List<tbl_college_details> lstCollege)
        {
            string jsonDownload = "";
            foreach (var obj in lstFreshCase)
            {
                jsonDownload = jsonDownload + "\"cognizent_tech_solution\": \"" + obj.cognizent_tech_solution.Trim() + "\",";
                jsonDownload = jsonDownload + "\"clientcode\": \"" + obj.clientcode.Trim() + "\",";
                jsonDownload = jsonDownload + "\"candidate_name\": \"" + obj.candidate_name.Trim() + "\",";
                jsonDownload = jsonDownload + "\"client_ref_no\": \"" + obj.client_ref_no.Trim() + "\",";
                jsonDownload = jsonDownload + "\"bvg_type\": \"" + obj.bvg_type.Trim() + "\",";
                jsonDownload = jsonDownload + "\"package\": \"" + obj.package.Trim() + "\",";
                jsonDownload = jsonDownload + "\"specification\": \"" + obj.specification.Trim() + "\",";
                jsonDownload = jsonDownload + "\"project_id\": \"" + obj.project_id.Trim() + "\",";
                jsonDownload = jsonDownload + "\"project_name\": \"" + obj.project_name.Trim() + "\",";
                jsonDownload = jsonDownload + "\"doj\": \"" + obj.doj.Trim() + "\",";
                jsonDownload = jsonDownload + "\"request_id\": \"" + obj.request_id.Trim() + "\",";
                jsonDownload = jsonDownload + "\"associate_id\": \"" + obj.associate_id.Trim() + "\",";
                jsonDownload = jsonDownload + "\"candidate_id\": \"" + obj.candidate_id.Trim() + "\",";
                jsonDownload = jsonDownload + "\"employee_id\": \"" + obj.employee_id.Trim() + "\",";
                jsonDownload = jsonDownload + "\"account_name\": \"" + obj.account_name.Trim() + "\",";
                jsonDownload = jsonDownload + "\"tensse\": \"" + obj.tensse.Trim() + "\",";
                jsonDownload = jsonDownload + "\"actual_case_created\": \"" + obj.actual_case_created.Trim() + "\",";
                jsonDownload = jsonDownload + "\"First_Name\": \"" + obj.first_name.Trim() + "\",";
                jsonDownload = jsonDownload + "\"Last_Name\": \"" + obj.last_name.Trim() + "\",";
                jsonDownload = jsonDownload + "\"Date_Of_Birth\": \"" + obj.date_of_birth.Trim() + "\",";
                jsonDownload = jsonDownload + "\"Father_Name\": \"" + obj.father_name.Trim() + "\",";
                jsonDownload = jsonDownload + "\"Nationality\": \"" + obj.nationality.Trim() + "\",";
                jsonDownload = jsonDownload + "\"Mobile_Number\": \"" + obj.mobile_number.Trim() + "\",";
                jsonDownload = jsonDownload + "\"Current_Address\": \"" + obj.current_address.Trim() + "\",";
                jsonDownload = jsonDownload + "\"Permanent_Address\": \"" + obj.permanent_address.Trim() + "\",";
                jsonDownload = jsonDownload + "\"Longest_Stay_Address\": \"" + obj.longest_stay_address.Trim() + "\",";
                jsonDownload = jsonDownload + "\"Academic\":[";
                string collegeJson = "";
                foreach (var objCollege in lstCollege)
                {
                    collegeJson = collegeJson + "{\"College\": \"" + objCollege.college.Trim() + "\",";
                    collegeJson = collegeJson + "\"Degree\": \"" + objCollege.degree.Trim() + "\",";
                    collegeJson = collegeJson + "\"Field_Source\": \"" + objCollege.field_source.Trim() + "\"},";
                }
                jsonDownload = jsonDownload + collegeJson.Remove(collegeJson.Length - 1, 1) + "],";

                jsonDownload = jsonDownload + "\"Reference_Type_CVT\": \"" + obj.reference_type_cvt.Trim() + "\",";
                jsonDownload = jsonDownload + "\"Reference_Type_UT\": \"" + obj.reference_type_ut.Trim() + "\",";
                jsonDownload = jsonDownload + "\"Company_Name_UT\": \"" + obj.company_name_ut.Trim() + "\",";
                jsonDownload = jsonDownload + "\"Company_Name_CVT\": \"" + obj.company_name_cvt.Trim() + "\",";
                jsonDownload = jsonDownload + "\"ID_UT\": \"" + obj.id_ut.Trim() + "\",";
                jsonDownload = jsonDownload + "\"ID_CVT\": \"" + obj.id_cvt.Trim() + "\",";
                jsonDownload = jsonDownload + "\"Employment_Type_CVT\": \"" + obj.employment_type_cvt.Trim() + "\",";
                jsonDownload = jsonDownload + "\"Employment_Type_UT\": \"" + obj.employment_type_ut.Trim() + "\"";
            }
            return jsonDownload;

        }
    }
}
