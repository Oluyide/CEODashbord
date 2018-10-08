using Dapper;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CEO_Dashboard.Models.BudgetModels;
using static CEO_Dashboard.Models.PayorMixTablesModels;
using Kendo.Mvc.Extensions;
//using Kendo.Mvc.UI;




namespace CEO_Dashboard.Controllers
{
    public class DoctorsRevenueController : Controller
    {
        public static string constr = ConfigurationManager.ConnectionStrings["DashboardContext"].ConnectionString;
        SqlConnection con = new SqlConnection(constr);

        [HttpGet]
        public ActionResult DoctorsPerformance()
        {
            var Fromdate = DateTime.Today;
            var ToDate = DateTime.Today.AddDays(1).AddTicks(-1);
            MasterDetails model = new MasterDetails();
            int[] facilityId = { 1, 2, 3, 4, 5, 6 };
            model.Facility = GetAllFacilities();
            var rIP = RunningRevenueSpecialtyPerAccountCat(Fromdate, ToDate, facilityId);
            model.EndTime = DateTime.Now.AddDays(0).ToString();
            model.ResultIP = rIP;
            model.StartTime = DateTime.Now.AddDays(0).ToString();

            ViewBag.revSumIPD = Math.Round(rIP.Select(x => x.IPNetAmt).Sum(), 2);
            ViewBag.revSumOPD = Math.Round(rIP.Select(x => x.OPNetAmt).Sum(), 2);
            ViewBag.revTotalAmt = Math.Round(rIP.Select(x => x.TotalNetAmt).Sum(), 2);

            return View(model);
            //MasterMTDDetails CustData = new MasterMTDDetails();

            //CustData.Facility = GetAllFacilities();
            //var y= SpecialtyVolume().ToList();
            //int[] facilityId = { 1, 2, 3, 4, 5, 6 };
            //List<SpecialtyVolume> volume = new List<SpecialtyVolume>();
            //foreach (var v in y)
            //{
            //    SpecialtyVolume sp = new SpecialtyVolume();
            //    sp.End = DateTime.Today.AddDays(1).AddTicks(-1);
            //    sp.Start = DateTime.Today;
            //    sp.FacilityId = facilityId;
            //    sp.IPVolume = v.IPVolume;
            //    sp.OPVolume = v.OPVolume;
            //    sp.ID = v.ID;
            //    sp.specialty = v.specialty;


            //    volume.Add(sp);
            //}
            //CustData.SpecialtyVolume = volume;
            //CustData.EndTime = DateTime.Now.AddDays(0).ToString();
            //return View(CustData);

        }



        [HttpGet]
        public ActionResult SpecialtyDataMajor()
        {
            var Fromdate = DateTime.Today;
            var ToDate = DateTime.Today.AddDays(1).AddTicks(-1);
            MasterDetails model = new MasterDetails();
            List<NestedTables> allData= new List<NestedTables>();
            int[] facilityId = { 1, 2, 3, 4, 5, 6 };
            model.Facility = GetAllFacilities();
            var rIP = SpecialtyReportData(Fromdate, ToDate, facilityId);
            model.EndTime = DateTime.Now.AddDays(0).ToString();
            model.NestedValues = rIP;
            model.StartTime = DateTime.Now.AddDays(0).ToString();

            //ViewBag.revSumIPD = Math.Round(rIP.Select(x => x.IPNetAmt).Sum(), 2);
            //ViewBag.revSumOPD = Math.Round(rIP.Select(x => x.OPNetAmt).Sum(), 2);
            //ViewBag.revTotalAmt = Math.Round(rIP.Select(x => x.TotalNetAmt).Sum(), 2);

            return View(model);
           

        }

        [HttpPost]
        public ActionResult SpecialtyDataMajor(MasterDetails model)
        {
            var Fromdate = DateTime.Parse(model.StartTime);
            var ToDate = DateTime.Parse(model.EndTime).AddDays(1).AddTicks(-1);

            
            model.Facility = GetAllFacilities();
            var rIP = SpecialtyReportData(Fromdate, ToDate, model.FacilityIds);
            model.EndTime = model.EndTime;
          
            model.NestedValues = rIP;
            model.StartTime = model.StartTime;

            //ViewBag.revSumIPD = Math.Round(rIP.Select(x => x.IPNetAmt).Sum(), 2);
            //ViewBag.revSumOPD = Math.Round(rIP.Select(x => x.OPNetAmt).Sum(), 2);
            //ViewBag.revTotalAmt = Math.Round(rIP.Select(x => x.TotalNetAmt).Sum(), 2);

            return View(model);


        }

        [HttpPost]
        public ActionResult DoctorsPerformance(MasterDetails model)

        {
            var Fromdate = DateTime.Parse(model.StartTime);
            var ToDate = DateTime.Parse(model.EndTime).AddDays(1).AddTicks(-1);
            
            int[] facilityId = { 1, 2, 3, 4, 5, 6 };
            model.Facility = GetAllFacilities();
            var rIP = RunningRevenueSpecialtyPerAccountCat(Fromdate, ToDate, model.FacilityIds);
            model.EndTime = model.EndTime;
            model.ResultIP = rIP.OrderByDescending(x=>x.DoctorDepartment);
            model.StartTime = model.StartTime;

            ViewBag.revSumIPD = Math.Round(rIP.Select(x => x.IPNetAmt).Sum(), 2);
            ViewBag.revSumOPD = Math.Round(rIP.Select(x => x.OPNetAmt).Sum(), 2);
            ViewBag.revTotalAmt = Math.Round(rIP.Select(x => x.TotalNetAmt).Sum(), 2);

            return View(model);
            //MasterMTDDetails CustData = new MasterMTDDetails();

            //CustData.Facility = GetAllFacilities();
            //var y= SpecialtyVolume().ToList();
            //int[] facilityId = { 1, 2, 3, 4, 5, 6 };
            //List<SpecialtyVolume> volume = new List<SpecialtyVolume>();
            //foreach (var v in y)
            //{
            //    SpecialtyVolume sp = new SpecialtyVolume();
            //    sp.End = DateTime.Today.AddDays(1).AddTicks(-1);
            //    sp.Start = DateTime.Today;
            //    sp.FacilityId = facilityId;
            //    sp.IPVolume = v.IPVolume;
            //    sp.OPVolume = v.OPVolume;
            //    sp.ID = v.ID;
            //    sp.specialty = v.specialty;


            //    volume.Add(sp);
            //}
            //CustData.SpecialtyVolume = volume;
            //CustData.EndTime = DateTime.Now.AddDays(0).ToString();
            //return View(CustData);

        }

        public ActionResult DoctorsPerfDrillDown(int specId, DateTime startDate, DateTime endDate, int[] FacilityId)
        {
            MasterDetails model = new MasterDetails();
            int[] facilityId = { 1, 2, 3, 4, 5, 6 };
            
            var rIP = RunningRevenueSpecialtyPerAccountCat(startDate, endDate, facilityId).Where(x => x.DepartmentID == specId).ToList();
            model.EndTime = DateTime.Now.AddDays(0).ToString();
            model.ResultIP = rIP;
            ViewBag.revSumIPD = Math.Round(rIP.Select(x => x.IPNetAmt).Sum(), 2);
            ViewBag.revSumOPD = Math.Round(rIP.Select(x => x.OPNetAmt).Sum(), 2);
            ViewBag.revTotalAmt = Math.Round(rIP.Select(x => x.TotalNetAmt).Sum(), 2);

            return View(model);
        }

        public IEnumerable<ResultIP> RunningRevenueSpecialtyPerAccountCat(DateTime Fromdate, DateTime ToDate, int[] facilityId)
        {

            List<int> list2 = facilityId.ToList();

            if (list2.Contains(2))
            {
                list2.Add(3);
            }
            if(list2.Contains(4))
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
                         group tab by tab["ProvidingDoctor"]
                         into groupDt
                         select new ResultIP
                         {
                             ProvidingDoctor = groupDt.Key.ToString(),
                             DepartmentID = groupDt.Select((r) => r.Field<int>("DepartmentID")).FirstOrDefault(),
                             DoctorDepartment = groupDt.Select((r) => r.Field<string>("DoctorDepartment")).FirstOrDefault(),
                             OPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["OPNetAmt"].ToString()), 2)),
                             IPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["IPNetAmt"].ToString()), 2)),
                             TotalNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["TotalNetAmt"].ToString()), 2)),
                             Counting = groupDt.Count(),
                         };
            con.Close();
            return result;
        }
        public IEnumerable<NestedTables> SpecialtyReportData(DateTime Fromdate, DateTime ToDate, int[] facilityId)
        {
            List<NestedTables> DetailResults = new List<NestedTables>();
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

            List<RevenueByAccountCat> RawDataList = new List<RevenueByAccountCat>();
            RawDataList = ObjMaster.RevenueByCat;
            var dt = ConvertToDataTable(ObjMaster.RevenueByCat);
            var result = from tab in dt.AsEnumerable()
                         group tab by tab["DepartmentID"]
                         into groupDt
                         select new ResultIP
                         {
                             departmentIdentity = groupDt.Key.ToString(),
                             DepartmentID = groupDt.Select((r) => r.Field<int>("DepartmentID")).FirstOrDefault(),
                             DoctorDepartment = groupDt.Select((r) => r.Field<string>("DoctorDepartment")).FirstOrDefault(),
                             OPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["OPNetAmt"].ToString()), 2)),
                             IPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["IPNetAmt"].ToString()), 2)),
                             TotalNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["TotalNetAmt"].ToString()), 2))
                             //Counting = groupDt.Count(),
                         };

            foreach( var  Specialtyresult in result)
            {
                var ForDoctorDetails = RawDataList.Where(x=>x.DepartmentID==Specialtyresult.DepartmentID).ToList();

                var Doctordt = ConvertToDataTable(ForDoctorDetails);
                var DoctorsResult = from cob in Doctordt.AsEnumerable()
                                    group cob by cob["ProvidingDoctorId"]
                         into groupDt
                                    select new ResultIP
                                    {
                                        ProvidingDoctoridentity = groupDt.Key.ToString(),
                                        ProvidingDoctor = groupDt.Select((r) => r.Field<string>("ProvidingDoctor")).FirstOrDefault(),
                                        OPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["OPNetAmt"].ToString()), 2)),
                                        IPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["IPNetAmt"].ToString()), 2)),
                                        TotalNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["TotalNetAmt"].ToString()), 2))
                                        //Counting = groupDt.Count(),
                                    };

                DetailResults.Add(new NestedTables { SpelcialtyValues = Specialtyresult, DoctorsDetails = DoctorsResult.ToList() });
            }
            con.Close();
            return DetailResults;
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

        public IEnumerable<SpecialtyVolume> SpecialtyVolume()
        {
            var objDetails = SqlMapper.QueryMultiple(con, "SpecialtyVolume", null, commandType: CommandType.StoredProcedure);
            var doctorIdList = objDetails.Read<SpecialtyVolume>().ToList();
            con.Close();
            return doctorIdList.AsEnumerable();
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
        // GET: DoctorsRevenue/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DoctorsRevenue/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: DoctorsRevenue/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DoctorsRevenue/Edit/5
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

        // GET: DoctorsRevenue/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DoctorsRevenue/Delete/5
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
