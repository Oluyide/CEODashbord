using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;


namespace CEO_Dashboard.Models
{
    public class PayorMixMTDModel
    {
        public class PayorMixMTDCountVsRevenueIPDModel
        {

            public string PayorMTDNameIPD { get; set; }
            public int PayorMTDCountIPD { get; set; }

            public double RevenueMTDIPD { get; set; }
            public string OPIP { get; set; }

        }
        public class PayorMixMTDCountVsRevenueOPDModel
        {
            public string PayorMTDNameOPD { get; set; }
            public int PayorMTDCountOPD { get; set; }
            public double RevenueMTDOPD { get; set; }
            public string OPIP { get; set; }
        }

        public class PayorMixLMCountVsRevenueIPDModel
        {
            public string PayorLMNameIPD { get; set; }
            public int PayorLMCountIPD { get; set; }
            public double RevenueLMIPD { get; set; }
            public string OPIP { get; set; }
        }

        public class PayorMixLMCountVsRevenueOPDModel
        {
            public string PayorLMNameOPD { get; set; }
            public int PayorLMCountOPD { get; set; }
            public double RevenueLMOPD { get; set; }
            public string OPIP { get; set; }
        }

        public class SpecializationMixMTDCountVsRevenueOPDModel
        {
            public string SpecializationMTDNameOPD { get; set; }
            public int SpecializationMTDCountOPD { get; set; }
            public double RevenueMTDOPD { get; set; }
            public string OPIP { get; set; }
        }


        public class SpecializationMixMTDCountVsRevenueIPDModel
        {
            public string SpecializationMTDNameIPD { get; set; }
            public int SpecializationMTDCountIPD { get; set; }
            public double RevenueMTDIPD { get; set; }
            public string OPIP { get; set; }
        }

        public class SpecializationMixLMCountVsRevenueOPDModel
        {
            public string SpecializationLMNameOPD { get; set; }
            public int SpecializationLMCountOPD { get; set; }
            public double RevenueLMOPD { get; set; }
            public string OPIP { get; set; }
        }


        public class SpecializationMixLMCountVsRevenueIPDModel
        {
            public string SpecializationLMNameIPD { get; set; }
            public int SpecializationLMCountIPD { get; set; }
            public double RevenueLMIPD { get; set; }
            public string OPIP { get; set; }
        }



        public class MasterMTDDetails
        {
            public List<PayorMixMTDCountVsRevenueIPDModel> PayorMTDReportIPD { get; set; }
            public List<PayorMixMTDCountVsRevenueOPDModel> PayorMTDReportOPD { get; set; }
            public List<PayorMixLMCountVsRevenueIPDModel> PayorLMReportIPD { get; set; }
            public List<PayorMixLMCountVsRevenueOPDModel> PayorLMReportOPD { get; set; }


            public List<SpecializationMixMTDCountVsRevenueOPDModel> SpecializeMTDReportOPD { get; set; }
            public List<SpecializationMixMTDCountVsRevenueIPDModel> SpecializeMTDReportIPD { get; set; }

            public List<SpecializationMixLMCountVsRevenueOPDModel> SpecializeLMReportOPD { get; set; }
            public List<SpecializationMixLMCountVsRevenueIPDModel> SpecializeLMReportIPD { get; set; }

           

            
            public int[] FacilityIds { get; set; }


           
            public string StartTime { get; set; }

            
            public string EndTime { get; set; }


        }
    }
}
