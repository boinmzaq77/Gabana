using Gabana.Controller.GabanaSQLite.Framework;
using Gabana.ORM.MerchantDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gabana.ShareSource
{
    public class Connect_GabanaSQLite
    {
        public Connect_GabanaSQLite(string pathDB)
        {
            //DataConnection.DefaultSettings = new MySettings(pathDB);//Connection *** DATABASE SQLITE ***
            //Gabana.ORM.MerchantDB.DbHelper.Getpathdb = pathDB;
        }

    }
}
