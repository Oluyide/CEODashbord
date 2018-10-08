using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DashboardLogic.Controls
{
    public class DashboardsClasses
    {
        public static string constr = ConfigurationManager.ConnectionStrings["DashboardContext"].ConnectionString;
        SqlConnection con = new SqlConnection(constr);
        public IEnumerable<ResultIP> RunningRevenuePerAccountCat(DateTime Fromdate, DateTime ToDate, int[] facilityId)
        {
            var facilities = string.Join(",", facilityId);
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
                             CompanyTypeName = groupDt.Key.ToString(),
                             IPNetAmt = groupDt.Sum((r) => Math.Round(decimal.Parse(r["IPNetAmt"].ToString()), 2))

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

    }
}
