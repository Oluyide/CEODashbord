using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static CEO_Dashboard.Models.PayorMixTablesModels;


    
    public static class ExtensionMethods
    {
        public static string constr = ConfigurationManager.ConnectionStrings["DashboardContext"].ConnectionString;
        public static SqlConnection con = new SqlConnection(constr);

        /// <summary>
        /// Converts a List to a datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        
    }
