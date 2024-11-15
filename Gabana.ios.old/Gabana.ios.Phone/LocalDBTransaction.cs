using Foundation;
using Gabana.ORM.Local;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UIKit;
using LinqToDB;
using LinqToDB.SqlQuery;
using System.Threading.Tasks;
using LinqToDB.Data;

namespace Gabana.ios.Phone
{
    public class LocalDBTransaction
    {
        public string path;
        public string sqliteFilename;
        public string docFolder;
        public string libFolder;
        Gabana.Controller.Connect_GabanaSQLite a;
        public  void ConnectLocalBase()
        {
            sqliteFilename = "GabanaLocalDB.db";
            docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            libFolder = Path.Combine(docFolder, "..", "Library");
            path = Path.Combine(libFolder, sqliteFilename);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            a = new Controller.Connect_GabanaSQLite(path);
        }
        public async Task<bool>  InsertToMerchantTable(Merchant mer)
        {
            try
            {
                using (var db = new GabanaLocalDB())
                {
                    var x = await db.InsertAsync(mer);
                  
                    return true;
                }
            }
          catch(Exception e){
                throw (e);
            }
        }
        public async Task<bool> UpdateDataTable(Merchant mer)
        {
            try
            {
                //using (var connection = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ProtectionNone | SQLiteOpenFlags.ReadWrite))
                //{
                //    var c = connection.Query<Merchant>("Update Merchant " +
                //                                      "SET " +
                //                                            "Name = '" + mer.Name+ "',"+
                //                                            "Logo =  '" + mer.Logo+ "' " +
                //                                            "WHERE MerchantID="+mer.MerchantID+";");
                //    //UPDATE table_name SET column1 = value1, column2 = value2....columnN = valueN [WHERE  CONDITION];
                //    connection.Close();
                //    return true;
                //}
                using (var db = new GabanaLocalDB())
                {
                    var c = await db.Merchants.Where(x=>x.MerchantID==mer.MerchantID)
                                                    .Set(x => x.Name,mer.Name)
                                                    .Set(x=>x.Logo,mer.Logo)
                                                    .UpdateAsync();
                    return true;
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
        public async Task<bool> DeleteDataInTable(int merID)
        {
            try
            {
                //using (var connection = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ProtectionNone | SQLiteOpenFlags.ReadWrite))
                //{
                //    connection.Query<Merchant>("Delete from Merchant where MerchantID = "+merID);
                //    //Merchant m = (from p in conncect.Table<Merchant>()
                //    //              where p.MerchantID == merID
                //    //              select p).FirstOrDefault();
                //    // if (m != null)
                //    //conncect.Delete<Merchant>(m.MerchantID);
                //    //  conncect.Delete("Merchant");
                //    connection.Close();
                //    return true;
                //}
                using (var db = new GabanaLocalDB())
                {
                    var c = db.Merchants.Where(x=>x.MerchantID == merID).DeleteAsync();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<Merchant> GetData(int mer)
        {
            try
            {
                //using (var connection = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ProtectionNone | SQLiteOpenFlags.ReadWrite))
                //{

                //    var c = connection.Table<Merchant>().ToList();
                //    connection.Close();
                //    return c;
                //} 
               // var a = TableExists();

                using (var db = new GabanaLocalDB())
                {
                    var c = await db.Merchants.Where(x => x.MerchantID == mer).FirstOrDefaultAsync();
                    return c;
                }
            }
            catch (Exception ex)
            {
                return null;
            };
        }

        public Boolean TableExists()
        {
            try
            {
                using (var db = new GabanaLocalDB())
                {

                    var tableInfo = db.Query(db, "SELECT count(*) FROM sqlite_master;");
                    int count = tableInfo.Count();
                    if (count != 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
                return false;
            }

        }
    }
}