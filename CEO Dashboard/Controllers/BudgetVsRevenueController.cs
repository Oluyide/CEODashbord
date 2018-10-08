using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CEO_Dashboard.Models.BudgetModels;
using static CEO_Dashboard.Models.PayorMixTablesModels;

namespace CEO_Dashboard.Controllers
{
    public class BudgetVsRevenueController : Controller
    {
        public static string constr = ConfigurationManager.ConnectionStrings["DashboardContext"].ConnectionString;
        SqlConnection con = new SqlConnection(constr);
        // GET: BudgetVsRevenue
       [HttpGet]
       public ActionResult DoctorPerformance()
       {
            ListofDoc doc = new ListofDoc();
            doc.Facility = GetAllFacilities();
            List<DoctorconsultationReport> doctorIdList = GetDoctorConsultReport().ToList();
            List<DoctorconsultationReport> admission = DoctorAdmissionReport().ToList();
      
            List<int> doctorsid = doctorIdList.Select(x => x.doctorId).Distinct().ToList();
            List<DoctorconsultationReport> doctorPerforms = new List<DoctorconsultationReport>();
           
            foreach (var docId in doctorsid)
            {
                DoctorconsultationReport model = new DoctorconsultationReport();
                var docName = doctorIdList.Where(x => x.doctorId == docId).Select(x => x.DoctorName).FirstOrDefault();
                var countPatient = doctorIdList.Where(x => x.doctorId == docId).Select(x => x.RegistrationId).Distinct().ToList().Count();
                var Outpatient = doctorIdList.Where(x => x.doctorId == docId && x.OPIP == "O").Select(x => x.RegistrationId).Distinct().ToList().Count();
                var Inpatient = admission.Where(x => x.doctorId == docId).Select(x => x.RegistrationId).Distinct().ToList().Count();
                var Facility= doctorIdList.Where(x => x.doctorId == docId).Select(x => x.FacilityName).FirstOrDefault();
                var facilityId =doctorIdList.Where(x => x.doctorId == docId).Select(x => x.FacilityId).FirstOrDefault();
                model.doctorId = docId;
                model.DoctorName = docName;
                model.countPatient = countPatient;
                model.OutpatientCount = Outpatient;
                model.InPatientCount = Inpatient;
                model.FacilityName = Facility;
                model.FacilityId = facilityId;
                model.startDate = doctorIdList.Select(x => x.startDate).FirstOrDefault();
                model.endDate = doctorIdList.Select(x => x.startDate).FirstOrDefault();

                doctorPerforms.Add(model);
            }
           
           
            doc.DoctorReportAnalysis = doctorPerforms;
            
            return View(doc);
        }
   
        public IEnumerable<ResultIP> RunningRevenueSpecialtyPerAccountCat(DateTime Fromdate, DateTime ToDate, int[] facilityId)
        {

            List<int> list2 = facilityId.ToList();

            if (list2.Contains(2))
            {
                list2.Add(3);
            }
            if (list2.Contains(4))
            {
                list2.Add(5);
            }
            var facilities = string.Join(",", list2.ToArray());
            //var objDetails = SqlMapper.QueryMultiple(con, "PayorMixCountVsRevenueOPD", commandType: CommandType.StoredProcedure);
            var objDetails = SqlMapper.QueryMultiple(con, "RunningUspGetRevenuebyPerformingDoctor", new { Fromdate = Fromdate, ToDate = ToDate, @intFacilityId = facilities, inyHospitalLocationId = 1 }, null, 1000, commandType: CommandType.StoredProcedure);
         
            MasterDetails ObjMaster = new MasterDetails();
            //Assigning each Multiple tables data to specific single model class  
            ObjMaster.RevenueByCat = objDetails.Read<RevenueByAccountCat>().ToList();
            //List<RevenueByAccountCat> IprevList = new List<RevenueByAccountCat>();

            var dt = ConvertToDataTable(ObjMaster.RevenueByCat);
            var result = from tab in dt.AsEnumerable()
                         group tab by tab["DoctorDepartment"]
                         into groupDt
                         select new ResultIP
                         {
                             DoctorDepartment = groupDt.Key.ToString(),
                             IPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["IPNetAmt"].ToString()), 2)),
                             OPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["OPNetAmt"].ToString()),2)),
                             TotalNetAmt= groupDt.Sum((r) => Math.Round(decimal.Parse(r["TotalNetAmt"].ToString()), 2))
                         };
            objDetails.Dispose();
            con.Close();
            
            return result;
        }

        public IEnumerable<ResultIP> RunningRevenueSpecialtyPerAccountOya(DateTime Fromdate, DateTime ToDate, int[] facilityId)
        {

            List<int> list2 = facilityId.ToList();

            if (list2.Contains(2))
            {
                list2.Add(3);
            }
            if (list2.Contains(4))
            {
                list2.Add(5);
            }
            var facilities = string.Join(",", list2.ToArray());
            //var objDetails = SqlMapper.QueryMultiple(con, "PayorMixCountVsRevenueOPD", commandType: CommandType.StoredProcedure);
            var objDetails = SqlMapper.QueryMultiple(con, "RunningRevenueWeeklySummaryServiceWise", new { Fromdate = Fromdate, ToDate = ToDate, @intFacilityId = facilities, inyHospitalLocationId = 1 }, null, 1000, commandType: CommandType.StoredProcedure);

            MasterDetails ObjMaster = new MasterDetails();
            //Assigning each Multiple tables data to specific single model class  
            ObjMaster.RevenueByCat = objDetails.Read<RevenueByAccountCat>().ToList();
            //List<RevenueByAccountCat> IprevList = new List<RevenueByAccountCat>();

            var dt = ConvertToDataTable(ObjMaster.RevenueByCat);
            var result = from tab in dt.AsEnumerable()
                         group tab by tab["CompanyTypeName"]
                         into groupDt
                         select new ResultIP
                         {
                             DoctorDepartment = groupDt.Key.ToString(),
                             IPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["IPNetAmt"].ToString()), 2)),
                             OPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["OPNetAmt"].ToString()), 2)),
                             TotalNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["TotalNetAmt"].ToString()), 2))
                         };
            con.Close();
            return result;
        }
        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }


        [HttpPost]
        public ActionResult DoctorPerformance(ListofDoc param)
        {
            ListofDoc doc = new ListofDoc();
            doc.Facility = GetAllFacilities();
            List<DoctorconsultationReport> doctorIdList = GetDoctorConsultReportPost(param.FacilityIds, DateTime.Parse(param.StartTime), DateTime.Parse(param.EndTime)).ToList();
            List<DoctorconsultationReport> admission = DoctorAdmissionReportPost(param.FacilityIds, DateTime.Parse(param.EndTime)).ToList();

            List<int> doctorsid = doctorIdList.Select(x => x.doctorId).Distinct().ToList();
            List<DoctorconsultationReport> doctorPerforms = new List<DoctorconsultationReport>();

            foreach (var docId in doctorsid)
            {
                DoctorconsultationReport model = new DoctorconsultationReport();
                var docName = doctorIdList.Where(x => x.doctorId == docId).Select(x => x.DoctorName).FirstOrDefault();
                var countPatient = doctorIdList.Where(x => x.doctorId == docId).Select(x => x.RegistrationId).Distinct().ToList().Count();
                var Outpatient = doctorIdList.Where(x => x.doctorId == docId && x.OPIP == "O").Select(x => x.RegistrationId).Distinct().ToList().Count();
                var Inpatient = admission.Where(x => x.doctorId == docId).Select(x => x.RegistrationId).Distinct().ToList().Count();
                var Facility = doctorIdList.Where(x => x.doctorId == docId).Select(x => x.FacilityName).FirstOrDefault();
                var facilityId = doctorIdList.Where(x => x.doctorId == docId).Select(x => x.FacilityId).FirstOrDefault();
                model.doctorId = docId;
                model.DoctorName = docName;
                model.countPatient = countPatient;
                model.OutpatientCount = Outpatient;
                model.InPatientCount = Inpatient;
                model.FacilityName = Facility;
                model.FacilityId = facilityId;
                model.startDate = param.StartTime;
                model.endDate = param.EndTime;
                doctorPerforms.Add(model);
            }


            doc.DoctorReportAnalysis = doctorPerforms;

            return View(doc);
        }

        public ActionResult DoctorsDrillDown(int DoctId,string startDate,string endDate,int[] FacilityId)
        {
            ListofDoc doc = new ListofDoc();
            doc.Facility = GetAllFacilities();
           
            List<DoctorconsultationReport> doctorIdListDiagnosis = GetDoctorConsultReportPost(FacilityId, DateTime.Parse(startDate), DateTime.Parse(endDate)).ToList();
            List<DoctorconsultationReport> admissionDiagnosis = DoctorAdmissionReport().ToList();
          
            List<DoctorconsultationReport> doctorIdListInvestigation = GetDoctoritemOfServicesReport(FacilityId, DateTime.Parse(startDate), DateTime.Parse(endDate)).ToList();
            List<DoctorconsultationReport> admissioninvestigation = DoctorAdmissionReport().ToList();

            
            List<string> patientId = doctorIdListDiagnosis.Where(x=>x.doctorId==DoctId).Select(x => x.RegistrationId).Distinct().ToList();
            List<string> admittedpatientId = admissionDiagnosis.Where(x => x.doctorId == DoctId ).Select(x => x.RegistrationId).Distinct().ToList();
            patientId.AddRange(admittedpatientId);

            List<DoctorsReportDrills> doctorPerforms = new List<DoctorsReportDrills>();

            foreach (var Patient in patientId.Distinct().ToList())
            {
                DoctorsReportDrills model = new DoctorsReportDrills();

                var patId = doctorIdListDiagnosis.Where(x => x.RegistrationId == Patient && x.doctorId == DoctId).Select(x => x.RegistrationId).FirstOrDefault();
                var PaitientDiag = String.Join(",",doctorIdListDiagnosis.Where(x => x.RegistrationId == Patient && x.doctorId == DoctId).Select(x => x.Diagnosis).ToList());
                var investigation = String.Join(",",doctorIdListInvestigation.Where(x => x.RegistrationId == Patient && x.doctorId == DoctId).Select(x => x.ServiceName).ToList());

                model.PatientId = patId;
                model.Diagnosis = PaitientDiag;
                model.ServiceName = investigation;

               doctorPerforms.Add(model);
            }
            ViewBag.DoctorsName = doctorIdListDiagnosis.Where(x => x.doctorId == DoctId).Select(x => x.DoctorName).FirstOrDefault() ;
            
            doc.DoctorDiagItemAnalysis = doctorPerforms;

            return View(doc);
        }
        public ActionResult RevenueSummary()
        {
            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            ViewBag.Error = "Please Select Facility";
            MasterMTDDetails CustData = new MasterMTDDetails();

            
            //int[] facilityId = { 1, 2, 3, 4, 5, 6 };

            CustData.Facility = GetAllFacilities();
            List<MasterMTDDetails> MasterData = GetMasterMTDDetails().ToList();
            

            

            CustData.BudgetAnalysis = MasterData[0].BudgetAnalysis;
            CustData.EndTime = DateTime.Now.AddDays(0).ToString();
            
            //ViewBag.payorMTDCountIPD = CustData.PayorMTDReportIPD.Sum(x => x.PayorMTDCountIPD);
            //ViewBag.revMTDSumIPD = CustData.PayorMTDReportIPD.Sum(x => x.RevenueMTDIPD);
            return View(CustData);
        }

        [HttpPost]
        public ActionResult RevenueSummary(MasterMTDDetails model)
        {
            if (model.FacilityIds == null)
            {
                ViewBag.Error = "Facility cannot be Empty.Please Select Facility";
               
                return RedirectToAction("RevenueSummary");
            }
            else
            {
                var end = DateTime.Parse(model.EndTime).ToString("M/d/yyyy");

                ViewBag.time = DateTime.Parse(end);

                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                MasterMTDDetails CustData = new MasterMTDDetails();
                CustData.Facility = GetAllFacilities();
                List<MasterMTDDetails> MasterData = GetMasterMTDDetailsPost(model.FacilityIds, DateTime.Parse(end)).ToList();
                CustData.BudgetAnalysis = MasterData[0].BudgetAnalysis;
                CustData.EndTime = end;
                //ViewBag.payorMTDCountIPD = CustData.PayorMTDReportIPD.Sum(x => x.PayorMTDCountIPD);
                //ViewBag.revMTDSumIPD = CustData.PayorMTDReportIPD.Sum(x => x.RevenueMTDIPD);
                return View(CustData);
            }
        }

        //public JsonResult AdmissionDetails()
        //{

        //    //return;
        //}


        [HttpGet]
        public ActionResult AdmissionReports()
        {
            ViewBag.Error = "Please Select Facility";
            MasterMTDDetails CustData = new MasterMTDDetails();
            int[] facilityId = { 1, 2, 3, 4, 5, 6 };
            CustData.FacilityIds = facilityId;
            List<MasterMTDDetails> MasterData = GetAdmissionReport().ToList();
            CustData.Facility = GetAllFacilities();
            CustData.AdmissionReportAnalysis = MasterData[0].AdmissionReportAnalysis;
            CustData.PayorWiseReportAnalysis = MasterData[0].PayorWiseReportAnalysis;
            CustData.ProceduresReportAnalysis = MasterData[0].ProceduresReportAnalysis;
            CustData.AdmissionReportAnalysisMTD = MasterData[0].AdmissionReportAnalysisMTD;
            CustData.ProceduresReportAnalysisCAT = MasterData[0].ProceduresReportAnalysisCAT;

            ViewBag.PtNum = CustData.AdmissionReportAnalysis.Sum(x => x.PtNum);
            ViewBag.Private = CustData.AdmissionReportAnalysis.Sum(x => x.Private);
            ViewBag.Client = CustData.AdmissionReportAnalysis.Sum(x => x.Client);
            ViewBag.HMO = CustData.AdmissionReportAnalysis.Sum(x => x.HMO);
            ViewBag.NHIS = CustData.AdmissionReportAnalysis.Sum(x => x.NHIS);

            ViewBag.Staff = CustData.AdmissionReportAnalysis.Sum(x => x.Staff);

            //MTD Admission Reports
            ViewBag.PtNumMTD = CustData.AdmissionReportAnalysisMTD.Sum(x => x.PtNum);
            ViewBag.PrivateMTD = CustData.AdmissionReportAnalysisMTD.Sum(x => x.Private);
            ViewBag.ClientMTD = CustData.AdmissionReportAnalysisMTD.Sum(x => x.Client);
            ViewBag.HMOMTD = CustData.AdmissionReportAnalysisMTD.Sum(x => x.HMO);
            ViewBag.NHISMTD = CustData.AdmissionReportAnalysisMTD.Sum(x => x.NHIS);
            ViewBag.StaffMTD = CustData.AdmissionReportAnalysisMTD.Sum(x => x.Staff);


            ViewBag.Daycounterr = CustData.PayorWiseReportAnalysis.Sum(x => x.Daycounterr);

            ViewBag.MtdSum = CustData.PayorWiseReportAnalysis.Sum(x => x.MTDcounterj);

            ViewBag.todaysProce = CustData.ProceduresReportAnalysis.Sum(x => x.TodaysProc);
            ViewBag.MTDProce = CustData.ProceduresReportAnalysis.Sum(x => x.MTDProc);

            ViewBag.todaysProceCAT = CustData.ProceduresReportAnalysisCAT.Sum(x => x.TodaysProc);
            ViewBag.MTDProceCAT = CustData.ProceduresReportAnalysisCAT.Sum(x => x.MTDProc);

            CustData.EndTime = DateTime.Now.AddDays(0).ToString("MM/dd/yyyy");

            return View(CustData);
        }

        [HttpGet]
        public JsonResult AdmmisionDetails(int Id,string endDate,int[] Facilitys)
        {
            DateTime dt = DateTime.ParseExact(endDate.Substring(0, 24),"ddd MMM dd yyyy HH:mm:ss",CultureInfo.InvariantCulture);
            string formattedDate = dt.ToString("MM/dd/yyyy");
            //int[] result = System.Web.Helpers.Json.Decode<int[]>(Facilitys);
            MasterMTDDetails CustData = new MasterMTDDetails();
            CustData.Facility = GetAllFacilities();

            List<AdmissionDetails> MasterData = GetAdmissionDetails(Id, DateTime.Parse(formattedDate), Facilitys).ToList();
            return Json(MasterData, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult AdmmisionDetailsMTD(int Id, string endDate, int[] Facilitys)
        {
            DateTime dt = DateTime.ParseExact(endDate.Substring(0, 24),"ddd MMM dd yyyy HH:mm:ss",CultureInfo.InvariantCulture);
            string formattedDate = dt.ToString("MM/dd/yyyy");
            //int[] result = System.Web.Helpers.Json.Decode<int[]>(Facilitys);
            //MasterMTDDetails CustData = new MasterMTDDetails();
            //CustData.Facility = GetAllFacilities();

            List<AdmissionDetails> MasterData = GetAdmissionDetailsMTD(Id, DateTime.Parse(formattedDate), Facilitys).ToList();
            return Json(MasterData, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public ActionResult AdmissionReports(MasterMTDDetails model)
        {
            if (model.FacilityIds == null)
            {
                ViewBag.Error = "Facility cannot be Empty.Please Select Facility";
                
                return RedirectToAction("RevenueSummary");
            }
            else
            {
                var end = DateTime.Parse(model.EndTime).ToString("M/d/yyyy");

                ViewBag.time = DateTime.Parse(end);

                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                MasterMTDDetails CustData = new MasterMTDDetails();
                CustData.Facility = GetAllFacilities();
                List<MasterMTDDetails> MasterData = GetAdmissionReportPost(model.FacilityIds, DateTime.Parse(end)).ToList();
                CustData.AdmissionReportAnalysis = MasterData[0].AdmissionReportAnalysis;
                CustData.PayorWiseReportAnalysis = MasterData[0].PayorWiseReportAnalysis;
                CustData.ProceduresReportAnalysis = MasterData[0].ProceduresReportAnalysis;
                CustData.AdmissionReportAnalysisMTD = MasterData[0].AdmissionReportAnalysisMTD;
                CustData.ProceduresReportAnalysisCAT = MasterData[0].ProceduresReportAnalysisCAT;

                ViewBag.PtNum = CustData.AdmissionReportAnalysis.Sum(x => x.PtNum);
                ViewBag.Private = CustData.AdmissionReportAnalysis.Sum(x => x.Private);
                ViewBag.Client = CustData.AdmissionReportAnalysis.Sum(x => x.Client);
                ViewBag.HMO = CustData.AdmissionReportAnalysis.Sum(x => x.HMO);
                ViewBag.NHIS = CustData.AdmissionReportAnalysis.Sum(x => x.NHIS);

                ViewBag.Staff = CustData.AdmissionReportAnalysis.Sum(x => x.Staff);

                //MTD Admission Reports
                ViewBag.PtNumMTD = CustData.AdmissionReportAnalysisMTD.Sum(x => x.PtNum);
                ViewBag.PrivateMTD = CustData.AdmissionReportAnalysisMTD.Sum(x => x.Private);
                ViewBag.ClientMTD = CustData.AdmissionReportAnalysisMTD.Sum(x => x.Client);
                ViewBag.HMOMTD = CustData.AdmissionReportAnalysisMTD.Sum(x => x.HMO);
                ViewBag.NHISMTD = CustData.AdmissionReportAnalysisMTD.Sum(x => x.NHIS);
                ViewBag.StaffMTD = CustData.AdmissionReportAnalysisMTD.Sum(x => x.Staff);


                ViewBag.Daycounterr = CustData.PayorWiseReportAnalysis.Sum(x => x.Daycounterr);

                ViewBag.MtdSum = CustData.PayorWiseReportAnalysis.Sum(x => x.MTDcounterj);

                ViewBag.todaysProce = CustData.ProceduresReportAnalysis.Sum(x => x.TodaysProc);
                ViewBag.MTDProce = CustData.ProceduresReportAnalysis.Sum(x => x.MTDProc);

                ViewBag.todaysProceCAT = CustData.ProceduresReportAnalysisCAT.Sum(x => x.TodaysProc);
                ViewBag.MTDProceCAT = CustData.ProceduresReportAnalysisCAT.Sum(x => x.MTDProc);

                CustData.EndTime = end;
                CustData.FacilityIds = model.FacilityIds;


                return View(CustData);
            }
        }

        [HttpGet]
         public ActionResult SpecialtyMixReport()
          {

            MasterMTDDetails CustData = new MasterMTDDetails();

            var Fromdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var ToDate = DateTime.Now.AddDays(1).AddTicks(-1);
            int[] facilityId = { 1, 2, 3, 4, 5, 6 };
            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int day = DateTime.Today.Day;

            ViewBag.Error = "Please Select Facility";
           

            CustData.Facility = GetAllFacilities();
           

            //var Fromdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            //var ToDate = DateTime.Now.AddDays(1).AddTicks(-1);
            //int[] facilityId = { 1, 2, 3, 4, 5, 6 };
            //int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            //int day = DateTime.Today.Day;

            //ViewBag.Error = "Please Select Facility";
            //MasterMTDDetails CustData = new MasterMTDDetails();

            List<MasterMTDDetails> MasterData = GetSpecialtyDetails().ToList();
            //List<MasterMTDDetails> MasterData = GetSpecialtyDetailsRunning().ToList();
            CustData.SpecialtyAnalysis = MasterData[0].SpecialtyAnalysis;

            //ViewBag.BudgetSum = CustData.SpecialtyAnalysis.Select(x => x.Budget).Sum();
            //ViewBag.Prorata = CustData.SpecialtyAnalysis.Select(x => x.Proratedbudget).Sum();
            //var Feseet = RunningRevenueSpecialtyPerAccountCat(Fromdate, ToDate, facilityId);

            //CustData.SpecialtyRev = RunningRevenueSpecialtyPerAccountCat(Fromdate, ToDate, facilityId).Take(10).ToList();

            //List<ResultIP> specialtyValues = new List<ResultIP>();

            //foreach (var r in CustData.SpecialtyRev)
            //{
            //    foreach (var u in CustData.SpecialtyAnalysis)
            //    {
            //        r.Variance = Math.Round(((decimal.Parse(u.Budget.ToString()) / days) * day) - r.TotalNetAmt,2);
            //        r.MeetBudget = Math.Round((decimal.Parse(u.Budget.ToString()) - r.TotalNetAmt) / ((days - day)),2);

            //    }

            //    specialtyValues.Add(r);
            //}

            ////cast(((ISNULL(Budget, 0) - Revenue) / NullIF((NumOfDays - dateNum), 0)) as decimal(18, 2)) as MeetBudget
            ////cast((((ISNULL(Budget, 0) / NumOfDays) * dateNum) - Revenue) as decimal(18, 2)) as Variance

            //CustData.Facility = GetAllFacilities();
            //CustData.SpecialtyRev = specialtyValues;
            //ViewBag.SumVariance = specialtyValues.Select(x => x.Variance).Sum();
            //ViewBag.SumMeet = specialtyValues.Select(x => x.MeetBudget).Sum();
            //ViewBag.Actual = specialtyValues.Select(x => x.TotalNetAmt).Sum();

            //CustData.SumAnalysis = MasterData[0].SumAnalysis;
            CustData.EndTime = DateTime.Today.AddDays(1).AddTicks(-1).ToString();
            CustData.StartTime = DateTime.Today.AddDays(0).ToString();

            return View(CustData);
        }

        [HttpPost]
        public ActionResult SpecialtyMixReport(MasterMTDDetails model)
        {


            if (model.FacilityIds == null)
            {
                ViewBag.Error = "Please Select Facility";
                
                return RedirectToAction("SpecialtyMixReport");
            }
            else
            {
                
                var Fromdate = new DateTime(DateTime.Parse(model.EndTime).Year, DateTime.Parse(model.EndTime).Month, 1); ;
                var ToDate = DateTime.Parse(model.EndTime).AddDays(1).AddTicks(-1);
                //var end = DateTime.Parse(model.EndTime).AddDays(1).AddTicks(-1); ;

                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                int day = DateTime.Parse(model.EndTime).Day;
           
                MasterMTDDetails CustData = new MasterMTDDetails();
                List<MasterMTDDetails> MasterData = GetSpecialtyDetailsPost(model.FacilityIds,ToDate).ToList();
                CustData.SpecialtyAnalysis = MasterData[0].SpecialtyAnalysis;

                //ViewBag.BudgetSum = CustData.SpecialtyAnalysis.Select(x => x.Budget).Sum();
                //ViewBag.Prorata = CustData.SpecialtyAnalysis.Select(x => x.Proratedbudget).Sum();
                //var Feseet = RunningRevenueSpecialtyPerAccountCat(Fromdate, ToDate, model.FacilityIds);

                //CustData.SpecialtyRev = RunningRevenueSpecialtyPerAccountCat(Fromdate, ToDate, model.FacilityIds).Take(10).ToList();

                //List<ResultIP> specialtyValues = new List<ResultIP>();

                //foreach (var r in CustData.SpecialtyRev)
                //{
                //    foreach (var u in CustData.SpecialtyAnalysis)
                //    {
                //        r.Variance = Math.Round(((decimal.Parse(u.Budget.ToString()) / days) * day) - r.TotalNetAmt, 2);
                //        r.MeetBudget = Math.Round((decimal.Parse(u.Budget.ToString()) - r.TotalNetAmt) / ((days - day)), 2);
                //    }

                //    specialtyValues.Add(r);
                //}

                //cast(((ISNULL(Budget, 0) - Revenue) / NullIF((NumOfDays - dateNum), 0)) as decimal(18, 2)) as MeetBudget
                //cast((((ISNULL(Budget, 0) / NumOfDays) * dateNum) - Revenue) as decimal(18, 2)) as Variance

                CustData.Facility = GetAllFacilities();
                //CustData.SpecialtyRev = specialtyValues;
                //ViewBag.SumVariance = specialtyValues.Select(x => x.Variance).Sum();
                //ViewBag.SumMeet = specialtyValues.Select(x => x.MeetBudget).Sum();
                //ViewBag.Actual = specialtyValues.Select(x => x.TotalNetAmt).Sum();

                CustData.SumAnalysis = MasterData[0].SumAnalysis;
                CustData.EndTime = model.EndTime;
                CustData.StartTime = model.StartTime;

                return View(CustData);

                
            }
        }

        public ActionResult OccupancyReports()
        {
            MasterMTDDetails CustData = new MasterMTDDetails();

            List<MasterMTDDetails> MasterData = GetOccupancyReports().ToList();

            CustData.Facility = GetAllFacilities();
            CustData.AppaOccupancyAnalysis = MasterData[0].AppaOccupancyAnalysis;
            ViewBag.apT = CustData.AppaOccupancyAnalysis.Sum(x => x.TotalBed);
            var a= CustData.AppaOccupancyAnalysis.Sum(x => x.TotalBed);
            ViewBag.apO = CustData.AppaOccupancyAnalysis.Sum(x => x.OccupiedBed);
            var b = CustData.AppaOccupancyAnalysis.Sum(x => x.OccupiedBed);

            var d = Math.Round((Convert.ToDouble(b) / Convert.ToDouble(a)) * 100,2);
            ViewBag.apP = d;

            CustData.IkejaOccupancyAnalysis = MasterData[0].IkejaOccupancyAnalysis;
            ViewBag.ikT = CustData.IkejaOccupancyAnalysis.Sum(x => x.TotalBed);
            ViewBag.ikO = CustData.IkejaOccupancyAnalysis.Sum(x => x.OccupiedBed);
            ViewBag.ikP = Math.Round((Convert.ToDouble((CustData.IkejaOccupancyAnalysis.Sum(x => x.OccupiedBed))/ Convert.ToDouble((CustData.IkejaOccupancyAnalysis.Sum(x => x.TotalBed)))*100)),2);

            CustData.IdejoOccupancyAnalysis = MasterData[0].IdejoOccupancyAnalysis;
            ViewBag.idT = CustData.IdejoOccupancyAnalysis.Sum(x => x.TotalBed);
            ViewBag.idO = CustData.IdejoOccupancyAnalysis.Sum(x => x.OccupiedBed);
            ViewBag.idP = Math.Round((Convert.ToDouble((CustData.IdejoOccupancyAnalysis.Sum(x => x.OccupiedBed)) / Convert.ToDouble((CustData.IdejoOccupancyAnalysis.Sum(x => x.TotalBed))) * 100)), 2);


            CustData.IkoyiOccupancyAnalysis = MasterData[0].IkoyiOccupancyAnalysis;
            ViewBag.ikyT = CustData.IkoyiOccupancyAnalysis.Sum(x => x.TotalBed);
            ViewBag.ikyO = CustData.IkoyiOccupancyAnalysis.Sum(x => x.OccupiedBed);
            ViewBag.ikyP = Math.Round((Convert.ToDouble((CustData.IkoyiOccupancyAnalysis.Sum(x => x.OccupiedBed)) / Convert.ToDouble((CustData.IkoyiOccupancyAnalysis.Sum(x => x.TotalBed))) * 100)), 2);
            CustData.EndTime = DateTime.Now.AddDays(-1).ToString();
            CustData.StartTime = DateTime.Now.AddDays(-1).ToString();

            //CustData.PayorRevenue = MasterData[0].PayorRevenue;

            return View(CustData);
        }

        [HttpPost]
        public ActionResult OccupancyReports(MasterMTDDetails model)
        {
            MasterMTDDetails CustData = new MasterMTDDetails();

            List<MasterMTDDetails> MasterData = GetOccupancyReportsPost(DateTime.Parse(model.StartTime),DateTime.Parse(model.EndTime)).ToList();

            CustData.Facility = GetAllFacilities();
            CustData.AppaOccupancyAnalysis = MasterData[0].AppaOccupancyAnalysis;
            ViewBag.apT = CustData.AppaOccupancyAnalysis.Sum(x => x.TotalBed);
            var a = CustData.AppaOccupancyAnalysis.Sum(x => x.TotalBed);
            ViewBag.apO = CustData.AppaOccupancyAnalysis.Sum(x => x.OccupiedBed);
            var b = CustData.AppaOccupancyAnalysis.Sum(x => x.OccupiedBed);

            var d = Math.Round((Convert.ToDouble(b) / Convert.ToDouble(a)) * 100, 2);
            ViewBag.apP = d;

            CustData.IkejaOccupancyAnalysis = MasterData[0].IkejaOccupancyAnalysis;
            ViewBag.ikT = CustData.IkejaOccupancyAnalysis.Sum(x => x.TotalBed);
            ViewBag.ikO = CustData.IkejaOccupancyAnalysis.Sum(x => x.OccupiedBed);
            ViewBag.ikP = Math.Round((Convert.ToDouble((CustData.IkejaOccupancyAnalysis.Sum(x => x.OccupiedBed)) / Convert.ToDouble((CustData.IkejaOccupancyAnalysis.Sum(x => x.TotalBed))) * 100)), 2);

            CustData.IdejoOccupancyAnalysis = MasterData[0].IdejoOccupancyAnalysis;
            ViewBag.idT = CustData.IdejoOccupancyAnalysis.Sum(x => x.TotalBed);
            ViewBag.idO = CustData.IdejoOccupancyAnalysis.Sum(x => x.OccupiedBed);
            ViewBag.idP = Math.Round((Convert.ToDouble((CustData.IdejoOccupancyAnalysis.Sum(x => x.OccupiedBed)) / Convert.ToDouble((CustData.IdejoOccupancyAnalysis.Sum(x => x.TotalBed))) * 100)), 2);


            CustData.IkoyiOccupancyAnalysis = MasterData[0].IkoyiOccupancyAnalysis;
            ViewBag.ikyT = CustData.IkoyiOccupancyAnalysis.Sum(x => x.TotalBed);
            ViewBag.ikyO = CustData.IkoyiOccupancyAnalysis.Sum(x => x.OccupiedBed);
            ViewBag.ikyP = Math.Round((Convert.ToDouble((CustData.IkoyiOccupancyAnalysis.Sum(x => x.OccupiedBed)) / Convert.ToDouble((CustData.IkoyiOccupancyAnalysis.Sum(x => x.TotalBed))) * 100)), 2);
            CustData.EndTime = model.EndTime;
            CustData.StartTime = model.StartTime;
            //CustData.PayorRevenue = MasterData[0].PayorRevenue;

            return View(CustData);
        }


        public IEnumerable<MasterMTDDetails> GetAdmissionReport()
        {
            
            var objMTDDetails = SqlMapper.QueryMultiple(con, "CEOAdmissionReports", commandType: CommandType.StoredProcedure);
            MasterMTDDetails ObjMTDMaster = new MasterMTDDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMTDMaster.AdmissionReportAnalysis = objMTDDetails.Read<AdmissionReport>().ToList();
            ObjMTDMaster.PayorWiseReportAnalysis = objMTDDetails.Read<PayorWiseReport>().ToList();
            ObjMTDMaster.ProceduresReportAnalysis = objMTDDetails.Read<ProcedureReport>().ToList();
            ObjMTDMaster.AdmissionReportAnalysisMTD = objMTDDetails.Read<AdmissionReport>().ToList();
            ObjMTDMaster.ProceduresReportAnalysisCAT = objMTDDetails.Read<ProcedureReport>().ToList();

            List<MasterMTDDetails> CustomerMTDObj = new List<MasterMTDDetails>();
            //Add list of records into MasterDetails list  
            CustomerMTDObj.Add(ObjMTDMaster);
            con.Close();
            return  CustomerMTDObj;
      
         }

       
        public IEnumerable<MasterMTDDetails> GetAdmissionReportPost(int[] facilityId, DateTime End)
        {
            List<int> list2 = facilityId.ToList();

            if (list2.Contains(2))
            {
                list2.Add(3);
            }
            if (list2.Contains(4))
            {
                list2.Add(5);
            }
            var facilities = string.Join(",", list2.ToArray());
            var objMTDDetails = SqlMapper.QueryMultiple(con, "CEOAdmissionReportsPost", new { facilityId = facilities, endTime = End }, null, null, commandType: CommandType.StoredProcedure);

            MasterMTDDetails ObjMTDMaster = new MasterMTDDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMTDMaster.AdmissionReportAnalysis = objMTDDetails.Read<AdmissionReport>().ToList();
            ObjMTDMaster.PayorWiseReportAnalysis = objMTDDetails.Read<PayorWiseReport>().ToList();
            ObjMTDMaster.ProceduresReportAnalysis = objMTDDetails.Read<ProcedureReport>().ToList();
            ObjMTDMaster.AdmissionReportAnalysisMTD = objMTDDetails.Read<AdmissionReport>().ToList();
            ObjMTDMaster.ProceduresReportAnalysisCAT = objMTDDetails.Read<ProcedureReport>().ToList();
            List<MasterMTDDetails> CustomerMTDObj = new List<MasterMTDDetails>();
            //Add list of records into MasterDetails list  
            CustomerMTDObj.Add(ObjMTDMaster);
            con.Close();
            return CustomerMTDObj;

        }


        public IEnumerable<MasterMTDDetails> GetOccupancyReports()
        {
        var objMTDDetails = SqlMapper.QueryMultiple(con, "dailyOccupancyReports", commandType: CommandType.StoredProcedure);
        MasterMTDDetails ObjMTDMaster = new MasterMTDDetails();

        //Assigning each Multiple tables data to specific single model class  
        ObjMTDMaster.AppaOccupancyAnalysis = objMTDDetails.Read<AppaOccupancy>().ToList();
        ObjMTDMaster.IkejaOccupancyAnalysis = objMTDDetails.Read<IkejaHOccupancy>().ToList();
        ObjMTDMaster.IdejoOccupancyAnalysis = objMTDDetails.Read<IdejoHOccupancy>().ToList();
        ObjMTDMaster.IkoyiOccupancyAnalysis = objMTDDetails.Read<IkoyiHOccupancy>().ToList();

        List<MasterMTDDetails> CustomerMTDObj = new List<MasterMTDDetails>();
        //Add list of records into MasterDetails list  
        CustomerMTDObj.Add(ObjMTDMaster);
        con.Close();

        return CustomerMTDObj;
    }

        public IEnumerable<MasterMTDDetails> GetOccupancyReportsPost(DateTime start, DateTime End)
        {
            var objMTDDetails = SqlMapper.QueryMultiple(con, "dailyOccupancyReportsPost", new {startTime=start,endTime = End }, null, null, commandType: CommandType.StoredProcedure);
            MasterMTDDetails ObjMTDMaster = new MasterMTDDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMTDMaster.AppaOccupancyAnalysis = objMTDDetails.Read<AppaOccupancy>().ToList();
            ObjMTDMaster.IkejaOccupancyAnalysis = objMTDDetails.Read<IkejaHOccupancy>().ToList();
            ObjMTDMaster.IdejoOccupancyAnalysis = objMTDDetails.Read<IdejoHOccupancy>().ToList();
            ObjMTDMaster.IkoyiOccupancyAnalysis = objMTDDetails.Read<IkoyiHOccupancy>().ToList();

            List<MasterMTDDetails> CustomerMTDObj = new List<MasterMTDDetails>();
            //Add list of records into MasterDetails list  
            CustomerMTDObj.Add(ObjMTDMaster);
            con.Close();

            return CustomerMTDObj;
        }
        public IEnumerable<MasterMTDDetails> GetSpecialtyDetails()
        {
            var objMTDDetails = SqlMapper.QueryMultiple(con, "SpecialtyMix", commandType: CommandType.StoredProcedure);
            MasterMTDDetails ObjMTDMaster = new MasterMTDDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMTDMaster.SpecialtyAnalysis = objMTDDetails.Read<SpecialtyMix>().ToList();
            ObjMTDMaster.SumAnalysis = objMTDDetails.Read<TotalOnSpecialty>().ToList();


            List<MasterMTDDetails> CustomerMTDObj = new List<MasterMTDDetails>();
            //Add list of records into MasterDetails list  
            CustomerMTDObj.Add(ObjMTDMaster);
            con.Close();
            return CustomerMTDObj;
        }

        public IEnumerable<AdmissionDetails> GetAdmissionDetails(int Id,DateTime endDate,int[] facilityId)
        {
            List<int> list2 = facilityId.ToList();

            if (list2.Contains(2))
            {
                list2.Add(3);
            }
            if (list2.Contains(4))
            {
                list2.Add(5);
            }
            var facilities = string.Join(",", list2.ToArray());
            var objMTDDetails = SqlMapper.QueryMultiple(con, "AddmissionDetails", new {Id=Id, facilityId = facilities, endTime = endDate }, null, null, commandType: CommandType.StoredProcedure);

            MasterMTDDetails ObjMTDMaster = new MasterMTDDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMTDMaster.AdmissionDetailsAnalysis = objMTDDetails.Read<AdmissionDetails>().ToList();
            //ObjMTDMaster.SumAnalysis = objMTDDetails.Read<TotalOnSpecialty>().ToList();


            //List<AdmissionDetails> CustomerMTDObj = new List<AdmissionDetails>();
            ////Add list of records into MasterDetails list  
            //CustomerMTDObj.Add(ObjMTDMaster.AdmissionDetailsAnalysis);
            con.Close();
            return ObjMTDMaster.AdmissionDetailsAnalysis;
        }


        public IEnumerable<AdmissionDetails> GetAdmissionDetailsMTD(int Id, DateTime endDate, int[] facilityId)
        {
            List<int> list2 = facilityId.ToList();

            if (list2.Contains(2))
            {
                list2.Add(3);
            }
            if (list2.Contains(4))
            {
                list2.Add(5);
            }
            var facilities = string.Join(",", list2.ToArray());
            var objMTDDetails = SqlMapper.QueryMultiple(con, "AddmissionDetailsMTD", new { Id = Id, facilityId = facilities, endTime = endDate }, null, null, commandType: CommandType.StoredProcedure);

            MasterMTDDetails ObjMTDMaster = new MasterMTDDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMTDMaster.AdmissionDetailsAnalysis = objMTDDetails.Read<AdmissionDetails>().ToList();
            //ObjMTDMaster.SumAnalysis = objMTDDetails.Read<TotalOnSpecialty>().ToList();


            //List<AdmissionDetails> CustomerMTDObj = new List<AdmissionDetails>();
            ////Add list of records into MasterDetails list  
            //CustomerMTDObj.Add(ObjMTDMaster.AdmissionDetailsAnalysis);
            con.Close();
            return ObjMTDMaster.AdmissionDetailsAnalysis;
        }

        public IEnumerable<MasterMTDDetails> GetSpecialtyDetailsRunning()
        {
            var objMTDDetails = SqlMapper.QueryMultiple(con, "SpecialtyMix", commandType: CommandType.StoredProcedure);
            MasterMTDDetails ObjMTDMaster = new MasterMTDDetails();


            //var orr = RunningRevenueSpecialtyPerAccountOya(Fromdate, ToDate, facilityId);
            //Assigning each Multiple tables data to specific single model class  
            ObjMTDMaster.SpecialtyAnalysis = objMTDDetails.Read<SpecialtyMix>().ToList();
            //ObjMTDMaster.SumAnalysis = objMTDDetails.Read<TotalOnSpecialty>().ToList();


            List<MasterMTDDetails> CustomerMTDObj = new List<MasterMTDDetails>();
            //Add list of records into MasterDetails list  
            CustomerMTDObj.Add(ObjMTDMaster);
            con.Close();
            return CustomerMTDObj;
        }


        public IEnumerable<MasterMTDDetails> GetSpecialtyDetailsPost(int[] facilityId,DateTime End)
        {
            List<int> list2 = facilityId.ToList();

            if (list2.Contains(2))
            {
                list2.Add(3);
            }
            if (list2.Contains(4))
            {
                list2.Add(5);
            }
            var facilities = string.Join(",", list2.ToArray());
            var objMTDDetails = SqlMapper.QueryMultiple(con, "SpecialtyMixPOST", new { facilityId = facilities,endTime = End }, null, null, commandType: CommandType.StoredProcedure);
           
            MasterMTDDetails ObjMTDMaster = new MasterMTDDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMTDMaster.SpecialtyAnalysis = objMTDDetails.Read<SpecialtyMix>().ToList();
            ObjMTDMaster.SumAnalysis = objMTDDetails.Read<TotalOnSpecialty>().ToList();


            List<MasterMTDDetails> CustomerMTDObj = new List<MasterMTDDetails>();
            //Add list of records into MasterDetails list  
            CustomerMTDObj.Add(ObjMTDMaster);
            con.Close();
            return CustomerMTDObj;
        }


        public IEnumerable<MasterMTDDetails> GetMasterMTDDetails()
        {
            var Fromdate = DateTime.Today;
            var ToDate = DateTime.Now.AddDays(1).AddTicks(-1);
            int[] facilityId = { 1, 2, 3, 4, 5, 6 };
            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int day = DateTime.Today.Day;
            var objDetails = SqlMapper.QueryMultiple(con, "RevenueSummaryMTD", commandType: CommandType.StoredProcedure);
            MasterMTDDetails ObjMaster = new MasterMTDDetails();
            var firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

           
            //Assigning each Multiple tables data to specific single model class  

            ObjMaster.BudgetAnalysis = objDetails.Read<budgetRunrateCalculation>().ToList();
            foreach (var mc in ObjMaster.BudgetAnalysis)
            {
                mc.IPDProrata = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() / days) * day,2);
                mc.OPDProrata = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault() / days) * day,2);
                mc.IPDRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.RevenueIPDSumMTD).SingleOrDefault() / day),2);
                mc.OPDRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.RevenueOPDSumMTD).SingleOrDefault() / day),2);
                mc.IPDRunrateBasis = Math.Round(((ObjMaster.BudgetAnalysis.Select(x => x.RevenueIPDSumMTD).SingleOrDefault() / day)*days),2);
                mc.IPDToAchievebufdgetRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() - ObjMaster.BudgetAnalysis.Select(x => x.RevenueIPDSumMTD).SingleOrDefault()) / (days - day),2);
                mc.OPDRunrateBasis = Math.Round(((ObjMaster.BudgetAnalysis.Select(x => x.RevenueOPDSumMTD).SingleOrDefault() / day)*days), 2);
                mc.OPDToAchievebufdgetRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault() - ObjMaster.BudgetAnalysis.Select(x => x.RevenueOPDSumMTD).SingleOrDefault()) / (days - day), 2);
            }
            ViewBag.IPOPbudget = ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() + ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault();
            ViewBag.IPOPProRata = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() / days) * day, 2) + Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault() / days) * day, 2);

            var IPOPValues = RunningRevenueSpecialtyPerAccountCat(Fromdate, ToDate, facilityId);

            var IPOPValuesMTD = RunningRevenueSpecialtyPerAccountOya(firstDayOfMonth, ToDate, facilityId);
            //ViewBag.revSumIPD = IPOPValues.Select(x => x.TotalNetAmt).Sum();
            //ViewBag.revSumOPD = IPOPValues.Select(x => x.OPNetAmt).Sum();
           
            //mc.IPDRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.RevenueIPDSumMTD).SingleOrDefault() / day), 2);
            //mc.OPDRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.RevenueOPDSumMTD).SingleOrDefault() / day), 2);

            //ViewBag.IPDRunRate = ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() + ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault();
            //ViewBag.OPDRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() / days) * day, 2) + Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault() / days) * day, 2);



            ViewBag.revSumIPD = IPOPValues.Select(x => x.IPNetAmt).Sum();
            ViewBag.revSumOPD = IPOPValues.Select(x => x.OPNetAmt).Sum();

            ViewBag.revSumIPDMTD = IPOPValuesMTD.Select(x => x.IPNetAmt).Sum();
            ViewBag.revSumOPDMTD = IPOPValuesMTD.Select(x => x.OPNetAmt).Sum();

            ViewBag.IPDRunRate = Math.Round((IPOPValuesMTD.Select(x => x.IPNetAmt).Sum() / day), 2);
            ViewBag.OPDRunRate = Math.Round((IPOPValuesMTD.Select(x => x.OPNetAmt).Sum() / day), 2);

            ViewBag.IPDRunrateBasis = Math.Round(((IPOPValuesMTD.Select(x => x.IPNetAmt).Sum() / day) * days), 2);
            ViewBag.OPDRunrateBasis = Math.Round(((IPOPValuesMTD.Select(x => x.OPNetAmt).Sum() / day) * days), 2);


            ViewBag.IPDToAchievebufdgetRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() - IPOPValuesMTD.Select(x => double.Parse(x.IPNetAmt.ToString())).Sum()) / (days - day), 2);
            ViewBag.OPDToAchievebufdgetRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault() - IPOPValuesMTD.Select(x => double.Parse(x.OPNetAmt.ToString())).Sum()) / (days - day), 2);

            //ViewBag.IPOPActualDay = ObjMaster.BudgetAnalysis.Select(x => x.RevenueIPDSumDaily).SingleOrDefault() + ObjMaster.BudgetAnalysis.Select(x => x.RevenueOPDSumDaily).SingleOrDefault();
            ViewBag.IPOPActualDay = (IPOPValues.Select(x => x.TotalNetAmt).Sum());
            ViewBag.IPOPActualMTD = (IPOPValuesMTD.Select(x => x.TotalNetAmt).Sum());

            

            //ViewBag.IPOPCurrentRunRateMTD= Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.RevenueIPDSumMTD).SingleOrDefault() / day), 2) + Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.RevenueOPDSumMTD).SingleOrDefault() / day), 2);
            ViewBag.IPOPCurrentRunRateMTD= Math.Round((IPOPValuesMTD.Select(x => double.Parse(x.IPNetAmt.ToString())).Sum() / day), 2)  + Math.Round((IPOPValuesMTD.Select(x => double.Parse(x.OPNetAmt.ToString())).Sum() / day), 2);
            ViewBag.IPOPRunrateBasis = Math.Round(((IPOPValuesMTD.Select(x => x.TotalNetAmt).Sum() / day) * days), 2); 
            ViewBag.IPOPtoAcheiveRunRate = ViewBag.IPOPtoAcheiveRunRate = ViewBag.IPDToAchievebufdgetRunRate + ViewBag.OPDToAchievebufdgetRunRate;//Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() - double.Parse(IPOPValues.Select(x => x.IPNetAmt).Sum().ToString())) / (days - day), 2) + Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault() - double.Parse(IPOPValues.Select(x => x.OPNetAmt).Sum().ToString()) / (days - day)), 2);


            ViewBag.IPOPActualMTDR = double.Parse(IPOPValuesMTD.Select(x => x.TotalNetAmt).Sum().ToString());

            ViewBag.DayBudget = Math.Round(ViewBag.IPOPbudget/DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month),2);
            ViewBag.PerACHIEVEMENT = Math.Round((ViewBag.IPOPActualMTDR/ViewBag.IPOPProRata) * 100,2);
            ViewBag.DailyPerAchievement = Math.Round((double.Parse(ViewBag.IPOPActualDay.ToString())/ViewBag.DayBudget) * 100, 2);



            List <MasterMTDDetails> CustomerObj = new List<MasterMTDDetails>();
          
            //Add list of records into MasterDetails list  
            CustomerObj.Add(ObjMaster);
            
            con.Close();

            return CustomerObj;
        }


        public IEnumerable<MasterMTDDetails> GetMasterMTDDetailsPost(int[] facilityId, DateTime End)
        {
            List<int> list2 = facilityId.ToList();

            if (list2.Contains(2))
            {
                list2.Add(3);
            }
            if (list2.Contains(4))
            {
                list2.Add(5);
            }
            var facilities = string.Join(",", list2.ToArray());
            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int day = End.Day;
            var objDetails = SqlMapper.QueryMultiple(con, "RevenueSummaryMTDPost", new { facilityId = facilities, endTime = End }, null, null, commandType: CommandType.StoredProcedure);
            MasterMTDDetails ObjMaster = new MasterMTDDetails();
            var firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            //ViewBag.revSumIPD = IPOPValues.Select(x => x.TotalNetAmt).Sum();
            //ViewBag.revSumOPD = IPOPValues.Select(x => x.OPNetAmt).Sum();

            //Assigning each Multiple tables data to specific single model class  

            //Assigning each Multiple tables data to specific single model class  

            ObjMaster.BudgetAnalysis = objDetails.Read<budgetRunrateCalculation>().ToList();
            foreach (var mc in ObjMaster.BudgetAnalysis)
            {
                mc.IPDProrata = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() / days) * day, 2);
                mc.OPDProrata = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault() / days) * day, 2);
                mc.IPDRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.RevenueIPDSumMTD).SingleOrDefault() / day), 2);
                mc.OPDRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.RevenueOPDSumMTD).SingleOrDefault() / day), 2);
                mc.IPDRunrateBasis = Math.Round(((ObjMaster.BudgetAnalysis.Select(x => x.RevenueIPDSumMTD).SingleOrDefault() / day) * days), 2);
                mc.IPDToAchievebufdgetRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() - ObjMaster.BudgetAnalysis.Select(x => x.RevenueIPDSumMTD).SingleOrDefault()) / (days - day), 2);
                mc.OPDRunrateBasis = Math.Round(((ObjMaster.BudgetAnalysis.Select(x => x.RevenueOPDSumMTD).SingleOrDefault() / day) * days), 2);
                mc.OPDToAchievebufdgetRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault() - ObjMaster.BudgetAnalysis.Select(x => x.RevenueOPDSumMTD).SingleOrDefault()) / (days - day), 2);
            }
            ViewBag.IPOPbudget = ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() + ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault();
            ViewBag.IPOPProRata = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() / days) * day, 2) + Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault() / days) * day, 2);

            var IPOPValues = RunningRevenueSpecialtyPerAccountCat(End, End.AddDays(1).AddTicks(-1), facilityId);
            var IPOPValuesMTD = RunningRevenueSpecialtyPerAccountCat(firstDayOfMonth, End.AddDays(1).AddTicks(-1), facilityId);
            //ViewBag.revSumIPD = IPOPValues.Select(x => x.TotalNetAmt).Sum();
            //ViewBag.revSumOPD = IPOPValues.Select(x => x.OPNetAmt).Sum();

            //mc.IPDRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.RevenueIPDSumMTD).SingleOrDefault() / day), 2);
            //mc.OPDRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.RevenueOPDSumMTD).SingleOrDefault() / day), 2);

            //ViewBag.IPDRunRate = ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() + ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault();
            //ViewBag.OPDRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() / days) * day, 2) + Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault() / days) * day, 2);



            ViewBag.revSumIPD = IPOPValues.Select(x => x.IPNetAmt).Sum();
            ViewBag.revSumOPD = IPOPValues.Select(x => x.OPNetAmt).Sum();

            ViewBag.revSumIPDMTD = IPOPValuesMTD.Select(x => x.IPNetAmt).Sum();
            ViewBag.revSumOPDMTD = IPOPValuesMTD.Select(x => x.OPNetAmt).Sum();

            ViewBag.IPDRunRate = Math.Round((IPOPValuesMTD.Select(x => x.IPNetAmt).Sum() / day), 2);
            ViewBag.OPDRunRate = Math.Round((IPOPValuesMTD.Select(x => x.OPNetAmt).Sum() / day), 2);

            ViewBag.IPDRunrateBasis = Math.Round(((IPOPValuesMTD.Select(x => x.IPNetAmt).Sum() / day) * days), 2);
            ViewBag.OPDRunrateBasis = Math.Round(((IPOPValuesMTD.Select(x => x.OPNetAmt).Sum() / day) * days), 2);


            ViewBag.IPDToAchievebufdgetRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() - IPOPValuesMTD.Select(x => double.Parse(x.IPNetAmt.ToString())).Sum()) / (days - day), 2);
            ViewBag.OPDToAchievebufdgetRunRate = Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault() - IPOPValuesMTD.Select(x => double.Parse(x.OPNetAmt.ToString())).Sum()) / (days - day), 2);

            //ViewBag.IPOPActualDay = ObjMaster.BudgetAnalysis.Select(x => x.RevenueIPDSumDaily).SingleOrDefault() + ObjMaster.BudgetAnalysis.Select(x => x.RevenueOPDSumDaily).SingleOrDefault();
            ViewBag.IPOPActualDay = (IPOPValues.Select(x => x.TotalNetAmt).Sum());
            ViewBag.IPOPActualMTD = (IPOPValuesMTD.Select(x => x.TotalNetAmt).Sum());

            //ViewBag.IPOPCurrentRunRateMTD= Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.RevenueIPDSumMTD).SingleOrDefault() / day), 2) + Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.RevenueOPDSumMTD).SingleOrDefault() / day), 2);
            ViewBag.IPOPCurrentRunRateMTD = Math.Round((IPOPValuesMTD.Select(x => double.Parse(x.IPNetAmt.ToString())).Sum() / day), 2) + Math.Round((IPOPValuesMTD.Select(x => double.Parse(x.OPNetAmt.ToString())).Sum() / day), 2);
            ViewBag.IPOPRunrateBasis = Math.Round(((IPOPValuesMTD.Select(x => x.TotalNetAmt).Sum() / day) * days), 2);
            ViewBag.IPOPtoAcheiveRunRate = ViewBag.IPDToAchievebufdgetRunRate + ViewBag.OPDToAchievebufdgetRunRate;//Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.IPDbudget).SingleOrDefault() - double.Parse(IPOPValues.Select(x => x.IPNetAmt).Sum().ToString())) / (days - day), 2) + Math.Round((ObjMaster.BudgetAnalysis.Select(x => x.OPDBudget).SingleOrDefault() - double.Parse(IPOPValues.Select(x => x.OPNetAmt).Sum().ToString()) / (days - day)), 2);


            ViewBag.IPOPActualMTDR = double.Parse(IPOPValuesMTD.Select(x => x.TotalNetAmt).Sum().ToString());

            ViewBag.DayBudget = Math.Round(ViewBag.IPOPbudget / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 2);
            ViewBag.PerACHIEVEMENT = Math.Round((ViewBag.IPOPActualMTDR / ViewBag.IPOPProRata) * 100, 2);
            //ViewBag.DailyPerAchievement = Math.Round((ViewBag.DayBudget / double.ViewBag.IPOPActualDay) * 100, 2);
            ViewBag.DailyPerAchievement = Math.Round((double.Parse(ViewBag.IPOPActualDay.ToString()) / ViewBag.DayBudget) * 100, 2);

            List<MasterMTDDetails> CustomerObj = new List<MasterMTDDetails>();

            //Add list of records into MasterDetails list  
            CustomerObj.Add(ObjMaster);

            con.Close();

            return CustomerObj;
        }
        private List<SelectListItem> GetAllFacilities()
        {
            List<SelectListItem> allFac = new List<SelectListItem>();
            allFac.Add(new SelectListItem { Value = "1", Text = "Apapa" });
            allFac.Add(new SelectListItem { Value = "2", Text = "Ikeja" });
            //allFac.Add(new SelectListItem { Value = "3", Text = "Ikeja Clinic" });
            allFac.Add(new SelectListItem { Value = "4", Text = "VI" });
            //allFac.Add(new SelectListItem { Value = "5", Text = "LSS" });
            allFac.Add(new SelectListItem { Value = "6", Text = "Ikoyi" });

            return allFac.ToList();
        }

        public IEnumerable<DoctorconsultationReport> GetDoctorConsultReport()
        {
            var objDetails = SqlMapper.QueryMultiple(con, "DoctorConsultationReport", null, commandType: CommandType.StoredProcedure);
            var doctorIdList = objDetails.Read<DoctorconsultationReport>().ToList();
            con.Close();
            return doctorIdList.AsEnumerable();
        }


        public IEnumerable<DoctorconsultationReport> GetDoctorConsultReportPost(int[] facilityId , DateTime startDate, DateTime endDate)
        {
            List<int> list2 = facilityId.ToList();

            if (list2.Contains(2))
            {
                list2.Add(3);
            }
            if (list2.Contains(4))
            {
                list2.Add(5);
            }
            var facilities = string.Join(",", list2.ToArray());
            var objDetails = SqlMapper.QueryMultiple(con, "DoctorConsultationReportPost", new { facilityId = facilities, startDate= startDate, endDate = endDate }, null, null, commandType: CommandType.StoredProcedure);
            var doctorIdList = objDetails.Read<DoctorconsultationReport>().ToList();
            con.Close();
            return doctorIdList.AsEnumerable();
        }

        public IEnumerable<DoctorconsultationReport> DoctorAdmissionReportPost(int[] facilityId, DateTime theDate)
        {
            List<int> list2 = facilityId.ToList();

            if (list2.Contains(2))
            {
                list2.Add(3);
            }
            if (list2.Contains(4))
            {
                list2.Add(5);
            }
            var facilities = string.Join(",", list2.ToArray());
            var objDetails = SqlMapper.QueryMultiple(con, "DoctorsAdmissionsPost", new { facilityId = facilities, theDate = theDate }, null, null, commandType: CommandType.StoredProcedure);
            var doctorIdList = objDetails.Read<DoctorconsultationReport>().ToList();
            con.Close();
            return doctorIdList.AsEnumerable();
        }
        public IEnumerable<DoctorconsultationReport> DoctorAdmissionReport()
        {
            var objDetails = SqlMapper.QueryMultiple(con, "DoctorsAdmissions", null, commandType: CommandType.StoredProcedure);
            var doctorIdList = objDetails.Read<DoctorconsultationReport>().ToList();
            con.Close();
            return doctorIdList.AsEnumerable();
        }

        public IEnumerable<DoctorconsultationReport> GetDoctoritemOfServicesReport(int[] facilityId, DateTime startDate, DateTime endDate)
        {
            List<int> list2 = facilityId.ToList();

            if (list2.Contains(2))
            {
                list2.Add(3);
            }
            if (list2.Contains(4))
            {
                list2.Add(5);
            }
            var facilities = string.Join(",", list2.ToArray());
            var objDetails = SqlMapper.QueryMultiple(con, "DocotorsPatientsItemOfSevice", new { facilityId = facilities, startDate = startDate,endDate= endDate }, null, null, commandType: CommandType.StoredProcedure);

            var doctorIdList = objDetails.Read<DoctorconsultationReport>().ToList();
            con.Close();
            return doctorIdList.AsEnumerable();
        }

        public IEnumerable<DoctorconsultationReport> GetDoctorAdmissionitemOfServicesReport()
        {
            var objDetails = SqlMapper.QueryMultiple(con, "DocotorsPatientsAdmissionItemOfSevice", null, commandType: CommandType.StoredProcedure);
            var doctorIdList = objDetails.Read<DoctorconsultationReport>().ToList();
            con.Close();
            return doctorIdList.AsEnumerable();
        }
    }
}