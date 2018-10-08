using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CEO_Dashboard.Models
{
    public class PayorMixTablesModels
    {
        public class PayorMixCountVsRevenueIPDModel
        {

            public string PayorNameIPD { get; set; }
            public int PayorCountIPD { get; set; }

            public double RevenueIPD { get; set; }
            public string OPIP { get; set; }

        }




        public class RevenueByAccountCat
        {

            public decimal OPNetAmt { get; set; }
            public decimal IPNetAmt { get; set; }
            public decimal TotalNetAmt { get; set; }
            public string CompanyTypeName { get; set; }
            public string ProvidingDoctor { get; set; }
            public string DoctorDepartment { get; set; }
            public string DisplayGroup { get; set; }
            public int RegistrationNo { get; set; }
            public int DepartmentID { get; set; }
            public int ProvidingDoctorid { get; set; }

            public string EncounterNo { get; set; }
            public string InvoiceNo { get; set; }


        }



        public class ResultIP
        {
            public string ProvidingDoctor { get; set; }
            public int DepartmentID { get; set; }
            public string CompanyTypeName { get; set; }
            public string DoctorDepartment { get; set; }
            public decimal OPNetAmt { get; set; }
            public decimal IPNetAmt { get; set; }
            public decimal TotalNetAmt { get; set; }
            public int Counting { get; set; }
            public decimal Variance { get; set; }
           public decimal MeetBudget { get; set; }
           public int ProvidingDoctorid { get; set; }
            public string ProvidingDoctoridentity { get; set; }

            public string departmentIdentity { get; set; }
        }


        public class NestedTables
        {
            public ResultIP SpelcialtyValues { get; set; }
            public List<ResultIP> DoctorsDetails { get; set; }

        }
        public class ForTheGRID
        {
            public string ProvidingDoctor { get; set; }
            public int DepartmentID { get; set; }
            public string CompanyTypeName { get; set; }
            public string DoctorDepartment { get; set; }
            public decimal OPNetAmt { get; set; }
            public decimal IPNetAmt { get; set; }
            public decimal TotalNetAmt { get; set; }
            public int Counting { get; set; }
            public decimal Variance { get; set; }
            public decimal MeetBudget { get; set; }

            public string departmentIdentity { get; set; }
        }

        public class ResultOP
        {
            public string CompanyTypeName { get; set; }
            public string DoctorDepartment { get; set; }
            public decimal OPNetAmt { get; set; }
            public decimal TotalNetAmt { get; set; }
            public int Counting { get; set; }
        }


        public class PayorMixCountVsRevenueOPDModel
        {
            public string PayorNameOPD { get; set; }
            public int PayorCountOPD { get; set; }
            public double RevenueOPD { get; set; }
            public string OPIP { get; set; }
        }

        public class PayorMixCountVsRevenueLMIPDModel
        {
            public string PayorNameLMIPD { get; set; }
            public int PayorCountLMIPD { get; set; }
            public double RevenueLMIPD { get; set; }
            public string OPIP { get; set; }
        }

        public class PayorMixCountVsRevenueLMOPDModel
        {
            public string PayorNameLMOPD { get; set; }
            public int PayorCountLMOPD { get; set; }
            public double RevenueLMOPD { get; set; }
            public string OPIP { get; set; }
        }


        public class SpecializationMixCountVsRevenueOPDModel
        {
            public string SpecializationNameOPD { get; set; }
            public int SpecializationCountOPD { get; set; }
            public double RevenueOPD { get; set; }
            public string OPIP { get; set; }
        }


        public class SpecializationMixCountVsRevenueIPDModel
        {
            public string SpecializationNameIPD { get; set; }
            public int SpecializationCountIPD { get; set; }
            public double RevenueIPD { get; set; }
            public string OPIP { get; set; }
        }

        public class SpecializationMixCountVsRevenueLMOPDModel
        {
            public string SpecializationNameLMOPD { get; set; }
            public int SpecializationCountLMOPD { get; set; }
            public double RevenueLMOPD { get; set; }
            public string OPIP { get; set; }
        }


        public class SpecializationMixCountVsRevenueLMIPDModel
        {
            public string SpecializationNameLMIPD { get; set; }
            public int SpecializationCountLMIPD { get; set; }
            public double RevenueLMIPD { get; set; }
            public string OPIP { get; set; }
        }

        public class FacilityModel
        {
            public List<SelectListItem> Facility { get; set; }
            public int[] FacilityIds { get; set; }

        }

        public class MasterDetails
        {
            public List<PayorMixCountVsRevenueIPDModel> PayorReportIPD { get; set; }
            public List<object[]>  RevenueData { get; set; }
            public IEnumerable<ResultIP> ResultIP { get; set; }
            public IEnumerable<ResultOP> ResultOP { get; set; }
            public IEnumerable<ResultOP> TotalOPIP { get; set; }
            public IEnumerable<NestedTables> NestedValues { get; set; }
            public List<PayorMixCountVsRevenueOPDModel> PayorReportOPD { get; set; }
            public List<PayorMixCountVsRevenueLMIPDModel> PayorReportLMIPD { get; set; }
            public List<PayorMixCountVsRevenueLMOPDModel> PayorReportLMOPD { get; set; }

            public List<SpecializationMixCountVsRevenueOPDModel> SpecializeReportOPD { get; set; }
            public List<SpecializationMixCountVsRevenueIPDModel> SpecializeReportIPD { get; set; }

            public List<SpecializationMixCountVsRevenueLMOPDModel> SpecializeReportLMOPD { get; set; }
            public List<SpecializationMixCountVsRevenueLMIPDModel> SpecializeReportLMIPD { get; set; }
            public List<RevenueByAccountCat> RevenueByCat { get; set; }
            public List<SelectListItem> Facility { get; set; }

            [Display(Name = "Facility:")]
            public int[] FacilityIds { get; set; }
            
            [Display(Name = "From:")]
            public string StartTime { get; set; }

            [Display(Name = "Date:")]
            public string EndTime { get; set; }


        }
    }
}