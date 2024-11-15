using System;
using System.Collections.Generic;
using System.Text;
using LinqToDB.Data;
using Microsoft.Extensions.Configuration;

namespace Gabana.ORM.MerchantDB
{
    public class DbHelper
    {       
        public static string ConnectionString { get; set; }
        public static string MerchantDB(string pathDB)
        {
           // pathDB += ";DateTimeFormat=Ticks";
            DataConnection.DefaultSettings = new MySettings(pathDB);
            return ConnectionString = "Data Source = " + pathDB; //";DateTimeFormat=Ticks"; //";DateTimeKind=Utc";
        }

        public static string PoolDB(string pathDB)
        {
            DataConnection.DefaultSettings = new MySettings(pathDB);
            return ConnectionString = "Data Source = " + pathDB;
        }


    }
}
