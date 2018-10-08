using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CEO_Dashboard.Models.PayorMixTablesModels;

namespace CEO_Dashboard.Models
{
    public class BudgetModels
    {

        public class budgetRunrateCalculation
        {
            public double IPDProrata {get; set;}
            public double IPDRunRate { get; set; }
            public double IPDRunrateBasis { get; set;}
            public double IPDToAchievebufdgetRunRate { get; set; }


            public double OPDProrata { get; set; }
            public double OPDRunRate { get; set; }
            public double OPDRunrateBasis { get; set; }
            public double OPDToAchievebufdgetRunRate { get; set; }


            public double IPDbudget { get; set; }
            public double OPDBudget { get; set; }

            public double RevenueIPDSumDaily { get; set; }
            public double RevenueIPDSumMTD { get; set; }

            public double RevenueOPDSumDaily { get; set; }
            public double RevenueOPDSumMTD { get; set; }

        }

        public class SpecialtyMix
        {
            public string Specialization { get; set; }
            public double Budget { get; set; }
            public double Revenue { get; set;}
            public double Proratedbudget { get; set; }
            public double Variance { get; set; }
            public double MeetBudget { get; set; }
        }


        public class AdmissionDetails
        {
            public string RegistrationNo { get; set; }
            public DateTime AdmissionDate { get; set; }
            public DateTime DischargeDate { get; set; }
            
        }

        public class AppaOccupancy
        {
            public int FacilityID { get; set; }
            public string FacilityName { get; set; }
            public string BedcategoryName { get; set; }
            public int TotalBed { get; set; }
            public int OccupiedBed { get; set; }
            public double Percentage { get; set; }
        }
        public class IkejaHOccupancy
        {
            public int FacilityID { get; set; }
            public string FacilityName { get; set; }
            public string BedcategoryName { get; set; }
            public int TotalBed { get; set; }
            public int OccupiedBed { get; set; }
            public double Percentage { get; set; }
        }

        public class IdejoHOccupancy
        {
            public int FacilityID { get; set; }
            public string FacilityName { get; set; }
            public string BedcategoryName { get; set; }
            public int TotalBed { get; set; }
            public int OccupiedBed { get; set; }
            public double Percentage { get; set; }
        }
        public class IkoyiHOccupancy
        {
            public int FacilityID { get; set; }
            public string FacilityName { get; set; }
            public string BedcategoryName { get; set; }
            public int TotalBed { get; set; }
            public int OccupiedBed { get; set; }
            public double Percentage { get; set; }
        }


        public class TotalOnSpecialty
        {
            public double SumBudget { get; set; }
            public double SumRevenue { get; set; }
            public double SumProratedBudget { get; set; }
            public double SumVariance { get; set; }
            public double SumMeetBudget { get; set; }
           
        }

        public class AdmissionReport
        {
           
            public int identityId { get; set;}
            public string Specialty { get; set; }
            public int PtNum { get; set; }
            public  int HMO { get; set; }
            public int NHIS { get; set; }
            public int Private { get; set; }
            public int Staff { get; set; }
            public int Client { get; set; }
        }

        public class PayorWiseReport
        {

            public string Payor { get; set; }
            public int Daycounterr { get; set; }
            public int MTDcounterj { get; set; }
           
        }
        public class ProcedureReport
        {

            public string Procedures { get; set; }
            public int TodaysProc { get; set; }
            public int MTDProc { get; set; }

        }

        public class ListofDoc
        {
            public List<SelectListItem> Facility { get; set; }
            [Required]
            [Display(Name = "Facility:")]
            public int[] FacilityIds { get; set; }
            [Display(Name = "From:")]
            public string StartTime { get; set; }

            [Display(Name = "Date:")]
            public string EndTime { get; set; }
            public List<DoctorconsultationReport> DoctorReportAnalysis { get; set; }
            public List<DoctorsReportDrills> DoctorDiagItemAnalysis { get; set; }
        }
        public class DoctorconsultationReport
        {
           public int doctorId { get; set; }
            public int countPatient { get; set; }
            public string DoctorName { get; set;}
            public int OutpatientCount { get; set; }
            public int InPatientCount { get; set; }
            public string RegistrationId { get; set; }
            public string Diagnosis { get; set; }
            public string ServiceName { get; set; }
            public string OPIP { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string FacilityName { get; set; }
            public string FacilityId { get; set; }
        }

        public class DoctorsReportDrills
        {
            public string PatientId { get; set; }
            public string Diagnosis { get; set; }
            public string ServiceName { get; set; }
        }

        public class ResultForSpecialty
        {
            public string CompanyTypeName { get; set; }
            public string DoctorDepartment { get; set; }
            public decimal OPNetAmt { get; set; }
            public decimal IPNetAmt { get; set; }
            public decimal TotalNetAmt { get; set; }
        }

        public class SpecialtyVolume
        {
            public int ID { get; set; }
            public  string specialty { get; set; }
            public  int OPVolume { get; set; }
            public int IPVolume { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public int[] FacilityId { get; set; }
        }

        public class MasterMTDDetails
        {
            public List<budgetRunrateCalculation> BudgetAnalysis { get; set; }
            public List<SpecialtyVolume> SpecialtyVolume {get; set;}
            public List<ResultIP> SpecialtyRev { get; set; }
            public List<SpecialtyMix> SpecialtyAnalysis { get; set; }
            public List<AdmissionDetails> AdmissionDetailsAnalysis { get; set; }
            public List<TotalOnSpecialty> SumAnalysis { get; set; }
            public List<AppaOccupancy> AppaOccupancyAnalysis { get; set; }
            public List<IkejaHOccupancy> IkejaOccupancyAnalysis { get; set; }
            public List<IdejoHOccupancy> IdejoOccupancyAnalysis { get; set; }
            public List<IkoyiHOccupancy> IkoyiOccupancyAnalysis { get; set; }

            public List<AdmissionReport> AdmissionReportAnalysis { get; set; }
            public List<AdmissionReport> AdmissionReportAnalysisMTD { get; set; }
            public List<PayorWiseReport> PayorWiseReportAnalysis { get; set; }
            public List<ProcedureReport> ProceduresReportAnalysis { get; set; }
            public List<ProcedureReport> ProceduresReportAnalysisCAT { get; set; }
            public List<DoctorconsultationReport> DoctorReportAnalysis { get; set; }
            public List<SelectListItem> Facility { get; set; }

            [Required]
            [Display(Name = "Facility:")]
            public int[] FacilityIds { get; set; }
            [Display(Name = "From:")]
            public string StartTime { get; set; }

            [Display(Name = "Date:")]
            public string EndTime { get; set; }

        }
    }
}