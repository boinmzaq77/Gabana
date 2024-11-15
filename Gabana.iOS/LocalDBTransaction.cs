using Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UIKit;
using System.Threading.Tasks;
using Gabana.ORM;
using LinqToDB;
using Microsoft.Data.Sqlite;
using SQLite;
using LinqToDB.Data;
using Gabana.ShareSource;
using Gabana.ORM.MerchantDB;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public class LocalDBTransaction
    {
        public string p;
        SqliteConnection conn;
        public static string sqliteFilename;
        public static string docFolder;
        public static string libFolder;
        Connect_GabanaSQLite a;
        public static string pathdb;
        public static string existingDb;
        public  void CreateLocalBase(int merchantID)
        {
            sqliteFilename = DataCashingAll.sqliteMerchantDB;
            try
            {
                docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                libFolder = Path.Combine(docFolder, "..", "Library", merchantID.ToString());
                pathdb = Path.Combine(libFolder, sqliteFilename);
                //File.Delete(pathdb);
                existingDb = NSBundle.MainBundle.PathForResource("MerchantDB", "db");
                if (!Directory.Exists(libFolder))
                {
                    Directory.CreateDirectory(libFolder);
                }
                if (!File.Exists(pathdb))
                {
                    //Utils.ShowMessage("New");
                    File.Copy(existingDb, pathdb, true);
                }
                //Utils.ShowMessage("Old");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Console.WriteLine(ex.Message);
            }
            DataCashingAll.Pathdb = pathdb;
            DataCashingAll.PathdbPrototype = pathdb;
            DataConnection.DefaultSettings = new MySettings(pathdb);
            Preferences.Set("PathMerchantDB", pathdb);
            

        }
        public void CreatePoolDB(int merchantID)
        {
            sqliteFilename = DataCashingAll.sqlitePoolDB;
            var sqliteFilename2 = "PoolDBnew.db";
            try
            {

                docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                pathdb = Path.Combine(docFolder, sqliteFilename);
                var pathdb2 = Path.Combine(docFolder, sqliteFilename2);
                existingDb = NSBundle.MainBundle.PathForResource("PoolDB", "db");
                if (!File.Exists(pathdb))
                {
                    File.Copy(existingDb, pathdb, true);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Console.WriteLine(ex.Message);
            }

            DataCashingAll.PathdbPrototypepool = pathdb;
            DataCashingAll.Pathdbpool = pathdb;
            Preferences.Set("PathPoolDB", pathdb);
            //DataCashingAll.Pathdbpoolnew = pathdb2;

        }
    }
}