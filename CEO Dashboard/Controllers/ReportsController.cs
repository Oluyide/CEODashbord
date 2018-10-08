using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using CEO_Dashboard.Models;
using Dapper;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using CEO_Dashboard.Logic;
using static CEO_Dashboard.Models.PayorMixMTDModel;
using static CEO_Dashboard.Models.PayorMixTablesModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace CEO_Dashboard.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {

        public static string constr = ConfigurationManager.ConnectionStrings["DashboardContext"].ConnectionString;
        SqlConnection con = new SqlConnection(constr);
        // GET: Reports

        string[] formats = { "MM-dd-yyyy", "MM/dd/yyyy", "MMM-dd-yyyy", "MMM/dd/yyyy", "MM-ddd-yyyy", "MM/ddd/yyyy", "MMM-ddd-yyyy", "MMM/ddd/yyyy", "dd/MM/yyyy" };
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
        [HttpGet]
        public ActionResult PayorMixReport()
        {

            MasterDetails CustData = new MasterDetails();
            ViewBag.Error = "Please Select Facility";
            List<MasterDetails> MasterData = GetMasterDetails().ToList();

            CustData.Facility = GetAllFacilities();
            CustData.PayorReportIPD = MasterData[0].PayorReportIPD;
            ViewBag.payorCountIPD = CustData.PayorReportIPD.Sum(x => x.PayorCountIPD);
            ViewBag.revSumIPD = CustData.PayorReportIPD.Sum(x => x.RevenueIPD);

            CustData.PayorReportOPD = MasterData[0].PayorReportOPD;
            ViewBag.payorCountOPD = CustData.PayorReportOPD.Sum(x => x.PayorCountOPD);
            ViewBag.revSumOPD = CustData.PayorReportOPD.Sum(x => x.RevenueOPD);

            CustData.PayorReportLMIPD = MasterData[0].PayorReportLMIPD;
            ViewBag.payorCountLMIPD = CustData.PayorReportLMIPD.Sum(x => x.PayorCountLMIPD);
            ViewBag.revSumLMIPD = CustData.PayorReportLMIPD.Sum(x => x.RevenueLMIPD);

            CustData.PayorReportLMOPD = MasterData[0].PayorReportLMOPD;
            ViewBag.payorCountLMOPD = CustData.PayorReportLMOPD.Sum(x => x.PayorCountLMOPD);
            ViewBag.revSumLMOPD = CustData.PayorReportLMOPD.Sum(x => x.RevenueLMOPD);
            CustData.EndTime = DateTime.Now.AddDays(0).ToString();

            //CustData.PayorRevenue = MasterData[0].PayorRevenue;

            return View(CustData);

        }
        [HttpPost]
        public ActionResult PayorMixReport(MasterDetails model)
        {

            if (model.FacilityIds == null)
            {
                ViewBag.Error = "Please Select Facility";


                return RedirectToAction("PayorMixReport");
            }
            else
            {

                var end = DateTime.Parse(model.EndTime).ToString("M/d/yyyy");
                MasterDetails CustData = new MasterDetails();

                List<MasterDetails> MasterData = GetMasterDetailsPost(model.FacilityIds, DateTime.Parse(end)).ToList();

                CustData.Facility = GetAllFacilities();
                CustData.PayorReportIPD = MasterData[0].PayorReportIPD;
                ViewBag.payorCountIPD = CustData.PayorReportIPD.Sum(x => x.PayorCountIPD);
                ViewBag.revSumIPD = CustData.PayorReportIPD.Sum(x => x.RevenueIPD);

                CustData.PayorReportOPD = MasterData[0].PayorReportOPD;
                ViewBag.payorCountOPD = CustData.PayorReportOPD.Sum(x => x.PayorCountOPD);
                ViewBag.revSumOPD = CustData.PayorReportOPD.Sum(x => x.RevenueOPD);

                CustData.PayorReportLMIPD = MasterData[0].PayorReportLMIPD;
                ViewBag.payorCountLMIPD = CustData.PayorReportLMIPD.Sum(x => x.PayorCountLMIPD);
                ViewBag.revSumLMIPD = CustData.PayorReportLMIPD.Sum(x => x.RevenueLMIPD);

                CustData.PayorReportLMOPD = MasterData[0].PayorReportLMOPD;
                ViewBag.payorCountLMOPD = CustData.PayorReportLMOPD.Sum(x => x.PayorCountLMOPD);
                ViewBag.revSumLMOPD = CustData.PayorReportLMOPD.Sum(x => x.RevenueLMOPD);
                CustData.EndTime = end;

                //CustData.PayorRevenue = MasterData[0].PayorRevenue;

                return View(CustData);
            }

        }

        [HttpGet]
        public ActionResult PayorMixMTDReport()
        {
            ViewBag.Error = "Please Select Facility";
            MasterMTDDetails CustData = new MasterMTDDetails();

            List<MasterMTDDetails> MasterData = GetMasterMTDDetails().ToList();
            CustData.Facility = GetAllFacilities();

            CustData.PayorMTDReportIPD = MasterData[0].PayorMTDReportIPD;
            ViewBag.payorMTDCountIPD = CustData.PayorMTDReportIPD.Sum(x => x.PayorMTDCountIPD);
            ViewBag.revMTDSumIPD = CustData.PayorMTDReportIPD.Sum(x => x.RevenueMTDIPD);

            CustData.PayorMTDReportOPD = MasterData[0].PayorMTDReportOPD;
            ViewBag.payorMTDCountOPD = CustData.PayorMTDReportOPD.Sum(x => x.PayorMTDCountOPD);
            ViewBag.revMTDSumOPD = CustData.PayorMTDReportOPD.Sum(x => x.RevenueMTDOPD);

            CustData.PayorLMReportIPD = MasterData[0].PayorLMReportIPD;
            ViewBag.payorLMCountIPD = CustData.PayorLMReportIPD.Sum(x => x.PayorLMCountIPD);
            ViewBag.revLMSumIPD = CustData.PayorLMReportIPD.Sum(x => x.RevenueLMIPD);

            CustData.PayorLMReportOPD = MasterData[0].PayorLMReportOPD;
            ViewBag.payorLMCountOPD = CustData.PayorLMReportOPD.Sum(x => x.PayorLMCountOPD);
            ViewBag.revLMSumOPD = CustData.PayorLMReportOPD.Sum(x => x.RevenueLMOPD);
            CustData.EndTime = DateTime.Now.AddDays(0).ToString();
          
            //CustData.PayorRevenue = MasterData[0].PayorRevenue;

            return View(CustData);

        }

        [HttpPost]
        public ActionResult PayorMixMTDReport(MasterMTDDetails model)
        {


            if (model.FacilityIds == null)
            {
                ViewBag.Error = "Please Select Facility";

                return RedirectToAction("PayorMixMTDReport");
            }
            else
            {
                var end = DateTime.Parse(model.EndTime).ToString("M/d/yyyy");
                MasterMTDDetails CustData = new MasterMTDDetails();

                List<MasterMTDDetails> MasterData = GetMasterMTDDetailsPost(model.FacilityIds, end).ToList();
                CustData.Facility = GetAllFacilities();

                CustData.PayorMTDReportIPD = MasterData[0].PayorMTDReportIPD;
                ViewBag.payorMTDCountIPD = CustData.PayorMTDReportIPD.Sum(x => x.PayorMTDCountIPD);
                ViewBag.revMTDSumIPD = CustData.PayorMTDReportIPD.Sum(x => x.RevenueMTDIPD);

                CustData.PayorMTDReportOPD = MasterData[0].PayorMTDReportOPD;
                ViewBag.payorMTDCountOPD = CustData.PayorMTDReportOPD.Sum(x => x.PayorMTDCountOPD);
                ViewBag.revMTDSumOPD = CustData.PayorMTDReportOPD.Sum(x => x.RevenueMTDOPD);

                CustData.PayorLMReportIPD = MasterData[0].PayorLMReportIPD;
                ViewBag.payorLMCountIPD = CustData.PayorLMReportIPD.Sum(x => x.PayorLMCountIPD);
                ViewBag.revLMSumIPD = CustData.PayorLMReportIPD.Sum(x => x.RevenueLMIPD);

                CustData.PayorLMReportOPD = MasterData[0].PayorLMReportOPD;
                ViewBag.payorLMCountOPD = CustData.PayorLMReportOPD.Sum(x => x.PayorLMCountOPD);
                ViewBag.revLMSumOPD = CustData.PayorLMReportOPD.Sum(x => x.RevenueLMOPD);
                CustData.EndTime = end;
                CustData.StartTime = model.StartTime;
                //CustData.PayorRevenue = MasterData[0].PayorRevenue;

                return View(CustData);
            }
        }


        [HttpGet]
        public ActionResult SpecializationPayorMixReport()
        {
            ViewBag.Error = "Please Select Facility";
            MasterDetails CustData = new MasterDetails();

            List<MasterDetails> MasterData = GetMasterSpecializationDetails().ToList();
            CustData.Facility = GetAllFacilities();
            CustData.SpecializeReportOPD = MasterData[0].SpecializeReportOPD;
            ViewBag.specialCountOPD = CustData.SpecializeReportOPD.Sum(x => x.SpecializationCountOPD);
            ViewBag.specialrevSumOPD = CustData.SpecializeReportOPD.Sum(x => x.RevenueOPD);

            CustData.SpecializeReportIPD = MasterData[0].SpecializeReportIPD;
            ViewBag.specialCountIPD = CustData.SpecializeReportIPD.Sum(x => x.SpecializationCountIPD);
            ViewBag.specialrevSumIPD = CustData.SpecializeReportIPD.Sum(x => x.RevenueIPD);

            CustData.SpecializeReportLMOPD = MasterData[0].SpecializeReportLMOPD;
            ViewBag.specialLMCountOPD = CustData.SpecializeReportLMOPD.Sum(x => x.SpecializationCountLMOPD);
            ViewBag.specialrevLMSumOPD = CustData.SpecializeReportLMOPD.Sum(x => x.RevenueLMOPD);

            CustData.SpecializeReportLMIPD = MasterData[0].SpecializeReportLMIPD;
            if (CustData.SpecializeReportLMIPD != null)
            {
                ViewBag.specialCountLMIPD = CustData.SpecializeReportLMIPD.Sum(x => x.SpecializationCountLMIPD);
                ViewBag.specialrevSumLMIPD = CustData.SpecializeReportLMIPD.Sum(x => x.RevenueLMIPD);
            }
            else
            {
                ViewBag.specialCountLMIPD = 0;
                ViewBag.specialrevSumLMIPD = 0;
            }
            CustData.EndTime = DateTime.Now.AddDays(0).ToString();
            return View(CustData);

        }

        [HttpPost]
        public ActionResult SpecializationPayorMixReport(MasterDetails model)
        {
            if (model.FacilityIds == null)
            {
                ViewBag.Error = "Facility cannot be Empty.Please Select Facility";
                var endy = DateTime.ParseExact(model.EndTime, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                ViewBag.time = DateTime.Parse(endy);
                return RedirectToAction("SpecializationPayorMixReport");
            }
            else
            {

                MasterDetails CustData = new MasterDetails();
                var end = DateTime.Parse(model.EndTime).ToString("M/d/yyyy");
                List<MasterDetails> MasterData = GetMasterSpecializationDetailsPost(model.FacilityIds, end).ToList();
                CustData.Facility = GetAllFacilities();

                CustData.SpecializeReportOPD = MasterData[0].SpecializeReportOPD;
                ViewBag.specialCountOPD = CustData.SpecializeReportOPD.Sum(x => x.SpecializationCountOPD);
                ViewBag.specialrevSumOPD = CustData.SpecializeReportOPD.Sum(x => x.RevenueOPD);

                CustData.SpecializeReportIPD = MasterData[0].SpecializeReportIPD;
                ViewBag.specialCountIPD = CustData.SpecializeReportIPD.Sum(x => x.SpecializationCountIPD);
                ViewBag.specialrevSumIPD = CustData.SpecializeReportIPD.Sum(x => x.RevenueIPD);

                CustData.SpecializeReportLMOPD = MasterData[0].SpecializeReportLMOPD;
                ViewBag.specialLMCountOPD = CustData.SpecializeReportLMOPD.Sum(x => x.SpecializationCountLMOPD);
                ViewBag.specialrevLMSumOPD = CustData.SpecializeReportLMOPD.Sum(x => x.RevenueLMOPD);

                CustData.SpecializeReportLMIPD = MasterData[0].SpecializeReportLMIPD;
                if (CustData.SpecializeReportLMIPD != null)
                {
                    ViewBag.specialCountLMIPD = CustData.SpecializeReportLMIPD.Sum(x => x.SpecializationCountLMIPD);
                    ViewBag.specialrevSumLMIPD = CustData.SpecializeReportLMIPD.Sum(x => x.RevenueLMIPD);
                }
                else
                {
                    ViewBag.specialCountLMIPD = 0;
                    ViewBag.specialrevSumLMIPD = 0;
                }
                CustData.EndTime = end;
                CustData.StartTime = DateTime.Parse(model.StartTime).ToString("M/d/yyyy");
                return View(CustData);
            }
        }

        [HttpGet]
        public ActionResult SpecializationPayorMixMTDReport()
        {
            ViewBag.Error = "Please Select Facility";
            MasterMTDDetails CustData = new MasterMTDDetails();

            List<MasterMTDDetails> MasterData = GetMasterMTDSpecializationDetails().ToList();

            CustData.Facility = GetAllFacilities();
            CustData.SpecializeMTDReportOPD = MasterData[0].SpecializeMTDReportOPD;
            ViewBag.specialMTDCountOPD = CustData.SpecializeMTDReportOPD.Sum(x => x.SpecializationMTDCountOPD);
            ViewBag.specialrevMTDSumOPD = CustData.SpecializeMTDReportOPD.Sum(x => x.RevenueMTDOPD);

            CustData.SpecializeMTDReportIPD = MasterData[0].SpecializeMTDReportIPD;
            ViewBag.specialMTDCountIPD = CustData.SpecializeMTDReportIPD.Sum(x => x.SpecializationMTDCountIPD);
            ViewBag.specialrevMTDSumIPD = CustData.SpecializeMTDReportIPD.Sum(x => x.RevenueMTDIPD);

            CustData.SpecializeLMReportOPD = MasterData[0].SpecializeLMReportOPD;
            ViewBag.specialLMCountOPD = CustData.SpecializeLMReportOPD.Sum(x => x.SpecializationLMCountOPD);
            ViewBag.specialrevLMSumOPD = CustData.SpecializeLMReportOPD.Sum(x => x.RevenueLMOPD);

            CustData.SpecializeLMReportIPD = MasterData[0].SpecializeLMReportIPD;
            ViewBag.specialLMCountIPD = CustData.SpecializeLMReportIPD.Sum(x => x.SpecializationLMCountIPD);
            ViewBag.specialrevLMSumIPD = CustData.SpecializeLMReportIPD.Sum(x => x.RevenueLMIPD);

            //CustData.PayorRevenue = MasterData[0].PayorRevenue;
            CustData.EndTime = DateTime.Now.AddDays(0).ToString();
            return View(CustData);

        }
        [HttpPost]
        public ActionResult SpecializationPayorMixMTDReport(MasterMTDDetails model)
        {
            if (model.FacilityIds == null)
            {
                ViewBag.Error = "Please Select Facility";

                return RedirectToAction("SpecializationPayorMixMTDReport");
            }
            else
            {

                MasterMTDDetails CustData = new MasterMTDDetails();
                var end = DateTime.Parse(model.EndTime).ToString("M/d/yyyy");
                List<MasterMTDDetails> MasterData = GetMasterMTDSpecializationDetailsPost(model.FacilityIds, end).ToList();

                CustData.Facility = GetAllFacilities();
                CustData.SpecializeMTDReportOPD = MasterData[0].SpecializeMTDReportOPD;
                ViewBag.specialMTDCountOPD = CustData.SpecializeMTDReportOPD.Sum(x => x.SpecializationMTDCountOPD);
                ViewBag.specialrevMTDSumOPD = CustData.SpecializeMTDReportOPD.Sum(x => x.RevenueMTDOPD);

                CustData.SpecializeMTDReportIPD = MasterData[0].SpecializeMTDReportIPD;
                ViewBag.specialMTDCountIPD = CustData.SpecializeMTDReportIPD.Sum(x => x.SpecializationMTDCountIPD);
                ViewBag.specialrevMTDSumIPD = CustData.SpecializeMTDReportIPD.Sum(x => x.RevenueMTDIPD);

                CustData.SpecializeLMReportOPD = MasterData[0].SpecializeLMReportOPD;
                ViewBag.specialLMCountOPD = CustData.SpecializeLMReportOPD.Sum(x => x.SpecializationLMCountOPD);
                ViewBag.specialrevLMSumOPD = CustData.SpecializeLMReportOPD.Sum(x => x.RevenueLMOPD);

                CustData.SpecializeLMReportIPD = MasterData[0].SpecializeLMReportIPD;
                ViewBag.specialLMCountIPD = CustData.SpecializeLMReportIPD.Sum(x => x.SpecializationLMCountIPD);
                ViewBag.specialrevLMSumIPD = CustData.SpecializeLMReportIPD.Sum(x => x.RevenueLMIPD);

                CustData.EndTime = end;
                CustData.StartTime = DateTime.Parse(model.StartTime).ToString("M/d/yyyy");

                return View(CustData);
            }

        }

        public IEnumerable<MasterDetails> GetMasterDetailsPost(int[] facilityId, DateTime End)
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
            var facilities = string.Join(",", facilityId);


            //var objDetails = SqlMapper.QueryMultiple(con, "PayorMixCountVsRevenueOPDTest", new { facilityId = facilities, endTime = End }, null,null,commandType: CommandType.StoredProcedure);
            var objDetails = SqlMapper.QueryMultiple(con, "RunningPayorMixCountVsRevenueOPDTest", new { facilityId = facilities, endTime = End }, null, null, commandType: CommandType.StoredProcedure);

            MasterDetails ObjMaster = new MasterDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMaster.PayorReportIPD = objDetails.Read<PayorMixCountVsRevenueIPDModel>().ToList();
            ObjMaster.PayorReportOPD = objDetails.Read<PayorMixCountVsRevenueOPDModel>().ToList();
            ObjMaster.PayorReportLMIPD = objDetails.Read<PayorMixCountVsRevenueLMIPDModel>().ToList();
            ObjMaster.PayorReportLMOPD = objDetails.Read<PayorMixCountVsRevenueLMOPDModel>().ToList();

            List<MasterDetails> CustomerObj = new List<MasterDetails>();
            //Add list of records into MasterDetails list  
            CustomerObj.Add(ObjMaster);
            con.Close();

            return CustomerObj;
        }


        public IEnumerable<MasterDetails> GetMasterDetails()
        {

            //var objDetails = SqlMapper.QueryMultiple(con, "PayorMixCountVsRevenueOPD", commandType: CommandType.StoredProcedure);
            var objDetails = SqlMapper.QueryMultiple(con, "RunningPayorMixCountVsRevenueOPD", commandType: CommandType.StoredProcedure);
            MasterDetails ObjMaster = new MasterDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMaster.PayorReportIPD = objDetails.Read<PayorMixCountVsRevenueIPDModel>().ToList();
            ObjMaster.PayorReportOPD = objDetails.Read<PayorMixCountVsRevenueOPDModel>().ToList();
            ObjMaster.PayorReportLMIPD = objDetails.Read<PayorMixCountVsRevenueLMIPDModel>().ToList();
            ObjMaster.PayorReportLMOPD = objDetails.Read<PayorMixCountVsRevenueLMOPDModel>().ToList();

            List<MasterDetails> CustomerObj = new List<MasterDetails>();
            //Add list of records into MasterDetails list  
            CustomerObj.Add(ObjMaster);
            con.Close();

            return CustomerObj;
        }
        public IEnumerable<MasterMTDDetails> GetMasterMTDDetails()
        {

            // var objMTDDetails = SqlMapper.QueryMultiple(con, "PayorMixMTDCountVsRevenueOPD", commandType: CommandType.StoredProcedure);
            var objMTDDetails = SqlMapper.QueryMultiple(con, "RunningPayorMixMTDCountVsRevenueOPD", commandType: CommandType.StoredProcedure);
            MasterMTDDetails ObjMTDMaster = new MasterMTDDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMTDMaster.PayorMTDReportIPD = objMTDDetails.Read<PayorMixMTDCountVsRevenueIPDModel>().ToList();
            ObjMTDMaster.PayorMTDReportOPD = objMTDDetails.Read<PayorMixMTDCountVsRevenueOPDModel>().ToList();
            ObjMTDMaster.PayorLMReportIPD = objMTDDetails.Read<PayorMixLMCountVsRevenueIPDModel>().ToList();
            ObjMTDMaster.PayorLMReportOPD = objMTDDetails.Read<PayorMixLMCountVsRevenueOPDModel>().ToList();

            List<MasterMTDDetails> CustomerMTDObj = new List<MasterMTDDetails>();
            //Add list of records into MasterDetails list  
            CustomerMTDObj.Add(ObjMTDMaster);
            con.Close();

            return CustomerMTDObj;
        }
        public IEnumerable<MasterMTDDetails> GetMasterMTDDetailsPost(int[] facilityId, string End)
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

            var facilities = string.Join(",", facilityId);
            var objMTDDetails = SqlMapper.QueryMultiple(con, "RunningPayorMixMTDCountVsRevenueOPDPost", new { facilityId = facilities, endTime = DateTime.Parse(End) }, null, null, commandType: CommandType.StoredProcedure);
            MasterMTDDetails ObjMTDMaster = new MasterMTDDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMTDMaster.PayorMTDReportIPD = objMTDDetails.Read<PayorMixMTDCountVsRevenueIPDModel>().ToList();
            ObjMTDMaster.PayorMTDReportOPD = objMTDDetails.Read<PayorMixMTDCountVsRevenueOPDModel>().ToList();
            ObjMTDMaster.PayorLMReportIPD = objMTDDetails.Read<PayorMixLMCountVsRevenueIPDModel>().ToList();
            ObjMTDMaster.PayorLMReportOPD = objMTDDetails.Read<PayorMixLMCountVsRevenueOPDModel>().ToList();

            List<MasterMTDDetails> CustomerMTDObj = new List<MasterMTDDetails>();
            //Add list of records into MasterDetails list  
            CustomerMTDObj.Add(ObjMTDMaster);
            con.Close();

            return CustomerMTDObj;
        }

        public IEnumerable<MasterDetails> GetMasterSpecializationDetails()
        {
            //var objMTDDetails = SqlMapper.QueryMultiple(con, "SpecializationMixCountVsRevenueOPD", commandType: CommandType.StoredProcedure);
            var objMTDDetails = SqlMapper.QueryMultiple(con, "RunningSpecializationMixCountVsRevenueOPD", commandType: CommandType.StoredProcedure);
            MasterDetails ObjMTDMaster = new MasterDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMTDMaster.SpecializeReportOPD = objMTDDetails.Read<SpecializationMixCountVsRevenueOPDModel>().ToList();
            ObjMTDMaster.SpecializeReportIPD = objMTDDetails.Read<SpecializationMixCountVsRevenueIPDModel>().ToList();
            ObjMTDMaster.SpecializeReportLMOPD = objMTDDetails.Read<SpecializationMixCountVsRevenueLMOPDModel>().ToList();
            ObjMTDMaster.SpecializeReportLMIPD = objMTDDetails.Read<SpecializationMixCountVsRevenueLMIPDModel>().ToList();

            List<MasterDetails> CustomerMTDObj = new List<MasterDetails>();
            //Add list of records into MasterDetails list  
            CustomerMTDObj.Add(ObjMTDMaster);
            con.Close();

            return CustomerMTDObj;
        }
        public IEnumerable<MasterDetails> GetMasterSpecializationDetailsPost(int[] facilityId, string end)
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

            var facilities = string.Join(",",list2.ToArray());
            //var objMTDDetails = SqlMapper.QueryMultiple(con, "SpecializationMixCountVsRevenueOPDPost", new { facilityId = facilities, endtime=DateTime.Parse(end) }, null, null, commandType: CommandType.StoredProcedure);
            var objMTDDetails = SqlMapper.QueryMultiple(con, "RunningSpecializationMixCountVsRevenueOPDPost", new { facilityId = facilities, endtime = DateTime.Parse(end) }, null, null, commandType: CommandType.StoredProcedure);
            MasterDetails ObjMTDMaster = new MasterDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMTDMaster.SpecializeReportOPD = objMTDDetails.Read<SpecializationMixCountVsRevenueOPDModel>().ToList();
            ObjMTDMaster.SpecializeReportIPD = objMTDDetails.Read<SpecializationMixCountVsRevenueIPDModel>().ToList();
            ObjMTDMaster.SpecializeReportLMOPD = objMTDDetails.Read<SpecializationMixCountVsRevenueLMOPDModel>().ToList();
            ObjMTDMaster.SpecializeReportLMIPD = objMTDDetails.Read<SpecializationMixCountVsRevenueLMIPDModel>().ToList();

            List<MasterDetails> CustomerMTDObj = new List<MasterDetails>();
            //Add list of records into MasterDetails list  
            CustomerMTDObj.Add(ObjMTDMaster);
            con.Close();

            return CustomerMTDObj;
        }

        public IEnumerable<MasterMTDDetails> GetMasterMTDSpecializationDetails()
        {

            //var objMTDDetails = SqlMapper.QueryMultiple(con, "SpecializationMixMTDCountVsRevenueOPD", commandType: CommandType.StoredProcedure);
            var objMTDDetails = SqlMapper.QueryMultiple(con, "RunningSpecializationMixMTDCountVsRevenueOPD", commandType: CommandType.StoredProcedure);
            MasterMTDDetails ObjMTDMaster = new MasterMTDDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMTDMaster.SpecializeMTDReportOPD = objMTDDetails.Read<SpecializationMixMTDCountVsRevenueOPDModel>().ToList();
            ObjMTDMaster.SpecializeMTDReportIPD = objMTDDetails.Read<SpecializationMixMTDCountVsRevenueIPDModel>().ToList();
            ObjMTDMaster.SpecializeLMReportOPD = objMTDDetails.Read<SpecializationMixLMCountVsRevenueOPDModel>().ToList();
            ObjMTDMaster.SpecializeLMReportIPD = objMTDDetails.Read<SpecializationMixLMCountVsRevenueIPDModel>().ToList();

            List<MasterMTDDetails> CustomerMTDObj = new List<MasterMTDDetails>();
            //Add list of records into MasterDetails list  
            CustomerMTDObj.Add(ObjMTDMaster);
            con.Close();

            return CustomerMTDObj;
        }


        public IEnumerable<MasterMTDDetails> GetMasterMTDSpecializationDetailsPost(int[] facilityId, string EndTime)
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

            //var objMTDDetails = SqlMapper.QueryMultiple(con, "SpecializationMixMTDCountVsRevenueOPDPost", new { facilityId = facilities,endtime=DateTime.Parse(EndTime) }, null, null, commandType: CommandType.StoredProcedure);
            var objMTDDetails = SqlMapper.QueryMultiple(con, "RunningSpecializationMixMTDCountVsRevenueOPDPost", new { facilityId = facilities, endtime = DateTime.Parse(EndTime) }, null, null, commandType: CommandType.StoredProcedure);
            MasterMTDDetails ObjMTDMaster = new MasterMTDDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMTDMaster.SpecializeMTDReportOPD = objMTDDetails.Read<SpecializationMixMTDCountVsRevenueOPDModel>().ToList();
            ObjMTDMaster.SpecializeMTDReportIPD = objMTDDetails.Read<SpecializationMixMTDCountVsRevenueIPDModel>().ToList();
            ObjMTDMaster.SpecializeLMReportOPD = objMTDDetails.Read<SpecializationMixLMCountVsRevenueOPDModel>().ToList();
            ObjMTDMaster.SpecializeLMReportIPD = objMTDDetails.Read<SpecializationMixLMCountVsRevenueIPDModel>().ToList();

            List<MasterMTDDetails> CustomerMTDObj = new List<MasterMTDDetails>();
            //Add list of records into MasterDetails list  
            CustomerMTDObj.Add(ObjMTDMaster);
            con.Close();

            return CustomerMTDObj;
        }
        public FileResult RevenueSummaryExcel()
        {

            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension = "xlsx";
            DataTable RevenuSummary = new DataTable();
            SqlCommand cmd = new SqlCommand("ReportsRevenueSummaryMTD", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter4 = new SqlDataAdapter(cmd);
            adapter4.Fill(RevenuSummary);
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/RevenueSummary.rdlc");
            localReport.DataSources.Add(new ReportDataSource("RevenueSummary", RevenuSummary));
            localReport.Refresh();


            var bytes = localReport.Render("Excel", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            Response.AddHeader("content-disposition", "attachment: filename=Url");
            MemoryStream s = new MemoryStream(bytes);
            s.Seek(0, SeekOrigin.Begin);
            return File(bytes, filenameExtension);
        }


        public FileResult PayorMixReportExcel()
        {

            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension = "xlsx";
            DataTable ResultTable1 = new DataTable();
            DataTable ResultTable2 = new DataTable();
            DataTable ResultTable3 = new DataTable();
            DataTable ResultTable4 = new DataTable();
            DataTable ResultTable5 = new DataTable();
            DataTable ResultTable6 = new DataTable();
            DataTable ResultTable7 = new DataTable();
            DataTable ResultTable8 = new DataTable();

            SqlCommand cmd1 = new SqlCommand("ReportIPDPayorMix", con);
            cmd1.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter1 = new SqlDataAdapter(cmd1);
            adapter1.Fill(ResultTable1);

            SqlCommand cmd2 = new SqlCommand("ReportOPDPayorMix", con);
            cmd2.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter2 = new SqlDataAdapter(cmd2);
            adapter2.Fill(ResultTable2);


            SqlCommand cmd3 = new SqlCommand("ReportAllFacilityLMIPD", con);
            cmd1.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter3 = new SqlDataAdapter(cmd3);
            adapter3.Fill(ResultTable3);


            SqlCommand cmd4 = new SqlCommand("ReportAllFacilityLMOPD", con);
            cmd4.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter4 = new SqlDataAdapter(cmd4);
            adapter4.Fill(ResultTable4);

            SqlCommand cmd5 = new SqlCommand("ReportRevenueMTDIPD", con);
            cmd1.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter5 = new SqlDataAdapter(cmd5);
            adapter5.Fill(ResultTable5);

            SqlCommand cmd6 = new SqlCommand("ReportsMTDRevenueOPD", con);
            cmd6.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter6 = new SqlDataAdapter(cmd6);
            adapter6.Fill(ResultTable6);

            SqlCommand cmd7 = new SqlCommand("ReportsLMRevenueIPD", con);
            cmd7.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter7 = new SqlDataAdapter(cmd7);
            adapter7.Fill(ResultTable7);

            SqlCommand cmd8 = new SqlCommand("ReportsLMRevenueOPD", con);
            cmd8.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter8 = new SqlDataAdapter(cmd8);
            adapter8.Fill(ResultTable8);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/PayorMixIPD.rdlc");
            localReport.DataSources.Add(new ReportDataSource("PayorMixIPD", ResultTable1));
            localReport.DataSources.Add(new ReportDataSource("OPDReport", ResultTable2));
            localReport.DataSources.Add(new ReportDataSource("PreviousMonthSummary", ResultTable3));
            localReport.DataSources.Add(new ReportDataSource("AllFacLMREVReport", ResultTable4));
            localReport.DataSources.Add(new ReportDataSource("MTDRevenue", ResultTable5));
            localReport.DataSources.Add(new ReportDataSource("RevenueReportOPDMTD", ResultTable6));
            localReport.DataSources.Add(new ReportDataSource("RevenueLMIPD", ResultTable7));
            localReport.DataSources.Add(new ReportDataSource("RevenueReportLMOPD", ResultTable8));
            localReport.Refresh();


            var bytes = localReport.Render("Excel", "", out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            Response.AddHeader("content-disposition", "attachment: filename=PayorMix" + filenameExtension);
            MemoryStream s = new MemoryStream(bytes);
            s.Seek(0, SeekOrigin.Begin);
            return File(bytes, filenameExtension);
        }


        public FileResult OccupancyReportExcel()
        {

            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension = "xlsx";
            DataTable ResultTable1 = new DataTable();
            DataTable ResultTable2 = new DataTable();
            DataTable ResultTable3 = new DataTable();
            DataTable ResultTable4 = new DataTable();


            SqlCommand cmd1 = new SqlCommand("ApapaOccupancy", con);
            cmd1.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter1 = new SqlDataAdapter(cmd1);
            adapter1.Fill(ResultTable1);

            SqlCommand cmd2 = new SqlCommand("IkejaOccupancy", con);
            cmd2.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter2 = new SqlDataAdapter(cmd2);
            adapter2.Fill(ResultTable2);


            SqlCommand cmd3 = new SqlCommand("Idejo", con);
            cmd3.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter3 = new SqlDataAdapter(cmd3);
            adapter3.Fill(ResultTable3);

            SqlCommand cmd4 = new SqlCommand("IkoyiOccupancy", con);
            cmd4.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter4 = new SqlDataAdapter(cmd4);
            adapter4.Fill(ResultTable4);



            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Occupancy.rdlc");
            localReport.DataSources.Add(new ReportDataSource("ApapaOccupancy", ResultTable1));
            localReport.DataSources.Add(new ReportDataSource("IkejaOccupancy", ResultTable2));
            localReport.DataSources.Add(new ReportDataSource("IdejoOccupancy", ResultTable3));
            localReport.DataSources.Add(new ReportDataSource("IkoyiOccupancy", ResultTable4));

            localReport.Refresh();


            var bytes = localReport.Render("Excel", "", out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            Response.AddHeader("content-disposition", "attachment: filename=Occupancy" + filenameExtension);
            MemoryStream s = new MemoryStream(bytes);
            s.Seek(0, SeekOrigin.Begin);
            return File(bytes, filenameExtension);
        }

        [HttpGet]
        public ActionResult RunningSpecialityRevenueData()
        {
            //ddDays(1).AddTicks(-1)
            var Fromdate = DateTime.Today;
            var ToDate = DateTime.Today.AddDays(1).AddTicks(-1);
            int[] facilityId = { 1, 2, 3, 4, 5, 6 };
            MasterDetails model = new MasterDetails();
            model.Facility = GetAllFacilities();
            var rIP = RunningRevenuePerAccountCat(Fromdate, ToDate, facilityId);
            var rOP = RunningRevenuePerAccountCatOP(Fromdate, ToDate, facilityId);
            var tOT = RunningRevenuePerAccountTot(Fromdate, ToDate, facilityId);
            model.ResultIP = rIP;
            model.ResultOP = rOP;
            model.TotalOPIP = tOT;
            ViewBag.revSumIPD = Math.Round(rIP.Select(x => x.IPNetAmt).Sum(),2);
            ViewBag.revSumOPD = Math.Round(rOP.Select(x => x.OPNetAmt).Sum(),2);

            model.EndTime = ToDate.ToString();
            model.StartTime = Fromdate.ToString();
            return View(model);
        }
        public IEnumerable<ResultOP> RunningRevenuePerAccountTot(DateTime Fromdate, DateTime ToDate, int[] facilityId)
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
                         select new ResultOP
                         {
                             CompanyTypeName = groupDt.Key.ToString(),
                             TotalNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["TotalNetAmt"].ToString()), 2)),
                             Counting = groupDt.Count(),
                         };
            con.Close();
            return result;
        }

        public IEnumerable<ResultIP> RunningRevenuePerAccountCat(DateTime Fromdate, DateTime ToDate, int[] facilityId)
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
            ObjMaster.RevenueByCat = objDetails.Read<RevenueByAccountCat>().ToList().Where(x=>x.OPNetAmt==0 ).ToList();
            //List<RevenueByAccountCat> IprevList = new List<RevenueByAccountCat>();

            var dt = ConvertToDataTable(ObjMaster.RevenueByCat);
            var result = from tab in dt.AsEnumerable()
                         group tab by tab["CompanyTypeName"]
                         into groupDt
                         select new ResultIP
                         {
                             CompanyTypeName = groupDt.Key.ToString(),
                             IPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["IPNetAmt"].ToString()),2)),
                             Counting=groupDt.Distinct().Select(x=>x.Field<string>("EncounterNo")).Count()
                         };
            con.Close();
            return result;
        }


        public IEnumerable<PayorMixTablesModels.ResultIP> RunningRevenueSpecialtyPerAccountCat(DateTime Fromdate, DateTime ToDate, int[] facilityId)
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
                             IPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["IPNetAmt"].ToString()),2)),
                             TotalNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["TotalNetAmt"].ToString()), 2)),
                             Counting = groupDt.Count(),
                         };
            con.Close();
            return result;
        }

        public IEnumerable<ResultOP> RunningRevenuePerAccountCatOP(DateTime Fromdate, DateTime ToDate, int[] facilityId)

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
            ObjMaster.RevenueByCat = objDetails.Read<RevenueByAccountCat>().Where(x=>x.IPNetAmt==0).ToList();

            var dt = ConvertToDataTable(ObjMaster.RevenueByCat);
            var result = from tab in dt.AsEnumerable()
                         group tab by tab["CompanyTypeName"]
                         into groupDt
                         select new ResultOP
                         {
                             CompanyTypeName = groupDt.Key.ToString(),
                             OPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["OPNetAmt"].ToString()),2)),
                             Counting = groupDt.Distinct().Select(x => x.Field<string>("EncounterNo")).Count(),

                             //Counting = groupDt.Count(),
                             TotalNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["TotalNetAmt"].ToString()), 2))
                         };

            return result;
        }

        public IEnumerable<ResultOP> RunningRevenuePerSpecialtyAccountCatOP(DateTime Fromdate, DateTime ToDate, int[] facilityId)

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
            ObjMaster.RevenueByCat = objDetails.Read<RevenueByAccountCat>()
                .ToList();
            var dt = ConvertToDataTable(ObjMaster.RevenueByCat);
            var result = from tab in dt.AsEnumerable()
                         group tab by tab["DoctorDepartment"]
                         into groupDt
                         select new ResultOP
                         {
                             DoctorDepartment = groupDt.Key.ToString().ToUpper(),
                             OPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["OPNetAmt"].ToString()), 2)),
                             TotalNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["TotalNetAmt"].ToString()), 2)),
                             Counting = groupDt.Count(),
                         };

            return result;
        }


        [HttpGet]
        public ActionResult RunningRevenueData()
        {
            //ddDays(1).AddTicks(-1)
            var Fromdate = DateTime.Today;
            var ToDate = DateTime.Today.AddDays(1).AddTicks(-1);
            int[] facilityId = { 1, 2, 3, 4, 5, 6 };
            MasterDetails model = new MasterDetails();
            model.Facility = GetAllFacilities();
            var rIP = RunningRevenuePerAccountCat(Fromdate, ToDate, facilityId);
            var rOP = RunningRevenuePerAccountCatOP(Fromdate, ToDate, facilityId);
            var tOT = RunningRevenuePerAccountTot(Fromdate, ToDate, facilityId);
            model.ResultIP = rIP;
            model.ResultOP = rOP;
            model.TotalOPIP = tOT;
            ViewBag.revCountIPD = rIP.Select(x => x.Counting).Sum();
            ViewBag.revCountOPD = rOP.Select(x => x.Counting).Sum();
            ViewBag.revSumIPD = Math.Round(rIP.Select(x => x.IPNetAmt).Sum(),2);
            ViewBag.revSumOPD = Math.Round(rOP.Select(x => x.OPNetAmt).Sum(),2);
            ViewBag.revTotalAmt = ViewBag.revSumOPD + ViewBag.revSumIPD;
            ViewBag.revTotalCount = ViewBag.revCountIPD + ViewBag.revCountOPD;
            model.EndTime = ToDate.ToString();
            model.StartTime = Fromdate.ToString();

            return View(model);
        }

        [HttpPost]
        public ActionResult RunningRevenueData(MasterDetails model)
        {

            if (model.FacilityIds == null)
            {
                ViewBag.Error = "Please Select Facility";


                return RedirectToAction("RunningRevenueData");
            }
            else
            {

                //ddDays(1).AddTicks(-1)
                var Fromdate = DateTime.Parse(model.StartTime);
                var ToDate = DateTime.Parse(model.EndTime).AddDays(1).AddTicks(-1);
                //int[] facilityId = { 1, 2, 3, 4, 5, 6 };
                model.Facility = GetAllFacilities();
                var rIP = RunningRevenuePerAccountCat(Fromdate, ToDate, model.FacilityIds);
                var rOP = RunningRevenuePerAccountCatOP(Fromdate, ToDate, model.FacilityIds);
                var tOT = RunningRevenuePerAccountTot(Fromdate, ToDate, model.FacilityIds);
                model.ResultIP = rIP;
                model.ResultOP = rOP;
                model.TotalOPIP = tOT;
                ViewBag.revCountIPD = rIP.Select(x => x.Counting).Sum();
                ViewBag.revCountOPD = rOP.Select(x => x.Counting).Sum();
                ViewBag.revSumIPD = Math.Round(rIP.Select(x => x.IPNetAmt).Sum(), 2);
                ViewBag.revSumOPD = Math.Round(rOP.Select(x => x.OPNetAmt).Sum(), 2);
                ViewBag.revTotalAmt = ViewBag.revSumOPD + ViewBag.revSumIPD;
                ViewBag.revTotalCount = ViewBag.revCountIPD + ViewBag.revCountOPD;
                model.EndTime = ToDate.ToString();
                model.StartTime = Fromdate.ToString();
            }
            return View(model);
        }


        [HttpPost]
        public ActionResult RunningRevenueSpecialtyData(MasterDetails model)
        {

            if (model.FacilityIds == null)
            {
                ViewBag.Error = "Please Select Facility";


                return RedirectToAction("RunningRevenueSpecialtyData");
            }
            else
            {

                //ddDays(1).AddTicks(-1)
                var Fromdate = DateTime.Parse(model.StartTime);
                var ToDate = DateTime.Parse(model.EndTime).AddDays(1).AddTicks(-1);
                //int[] facilityId = { 1, 2, 3, 4, 5, 6 };
                model.Facility = GetAllFacilities();
                var rIP = RunningRevenueSpecialtyPerAccountCat(Fromdate, ToDate, model.FacilityIds);
                var rOP = RunningRevenuePerSpecialtyAccountCatOP(Fromdate, ToDate, model.FacilityIds);
                var tOT = RunningRevenuePerAccountTot(Fromdate, ToDate, model.FacilityIds);
                model.ResultIP = rIP;
                model.ResultOP = rOP;
                model.TotalOPIP = tOT;
                ViewBag.revCountIPD = rIP.Select(x => x.Counting).Sum();
                ViewBag.revCountOPD = rOP.Select(x => x.Counting).Sum();
                ViewBag.revSumIPD = Math.Round(rIP.Select(x => x.IPNetAmt).Sum(), 2);
                ViewBag.revSumOPD = Math.Round(rOP.Select(x => x.OPNetAmt).Sum(), 2);
                ViewBag.revTotalAmt = ViewBag.revSumOPD + ViewBag.revSumIPD;
                ViewBag.revTotalCount = ViewBag.revCountIPD + ViewBag.revCountOPD;
                model.EndTime = ToDate.ToString();
                model.StartTime = Fromdate.ToString();
            }
            return View(model);
        }


        [HttpGet]
        public ActionResult RunningRevenueSpecialtyData()
        {
            //ddDays(1).AddTicks(-1)
            var Fromdate = DateTime.Today;
            var ToDate = DateTime.Today.AddDays(1).AddTicks(-1);
            int[] facilityId = { 1, 2, 3, 4, 5, 6 };
            MasterDetails model = new MasterDetails();
            model.Facility = GetAllFacilities();
            var rIP = RunningRevenueSpecialtyPerAccountCat(Fromdate, ToDate, facilityId);
            var rOP  = RunningRevenuePerSpecialtyAccountCatOP(Fromdate, ToDate, facilityId);
            model.ResultIP = rIP;
            model.ResultOP = rOP;
            ViewBag.revCountIPD = rIP.Select(x => x.Counting).Sum();
            ViewBag.revCountOPD = rOP.Select(x => x.Counting).Sum();
            ViewBag.revSumIPD = Math.Round(rIP.Select(x => x.IPNetAmt).Sum(), 2);
            ViewBag.revSumOPD = Math.Round(rOP.Select(x => x.OPNetAmt).Sum(), 2);
            ViewBag.revTotalAmt = Math.Round(rOP.Select(x => x.TotalNetAmt).Sum(), 2) + Math.Round(rIP.Select(x => x.IPNetAmt).Sum(), 2);
            model.EndTime = ToDate.ToString();
            model.StartTime = Fromdate.ToString();
            return View(model);
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




        // POST: Reports/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reports/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reports/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
