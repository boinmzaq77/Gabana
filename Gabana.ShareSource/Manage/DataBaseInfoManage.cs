using Gabana.ORM.MerchantDB;
using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Manage
{
    public class DataBaseInfoManage
    {      
        public async Task<DataBaseInfo> GetDatabaseInfo()
        {
            try
            {
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var DBinfo = await db.DataBaseInfo.Where(x => x.KeyDBInfo == "GabanaErVersion").FirstOrDefaultAsync();             
                    return DBinfo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertDatabaseInfo(DataBaseInfo dbinfo)
        {
            try
            {
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertAsync<DataBaseInfo>(dbinfo);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateDatabaseInfo(DataBaseInfo dbinfo)
        {
            try
            {
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.DataBaseInfo.Where(m => m.KeyDBInfo == dbinfo.KeyDBInfo).FirstOrDefault();
                    if (result != null)
                    {
                        var UpdateCustomer = await db.DataBaseInfo.Where(x => x.KeyDBInfo == dbinfo.KeyDBInfo)
                             .Set(c => c.DataDBInfo, dbinfo.DataDBInfo ?? dbinfo.DataDBInfo)                             
                             .UpdateAsync();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> DeleteDatabaseInfo(string Keydb)
        {
            try
            {
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.DataBaseInfo.Where(m => m.KeyDBInfo == Keydb).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteDatabaseInfo = await db.DataBaseInfo.Where(c => c.KeyDBInfo == Keydb).DeleteAsync();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }


        public async Task<bool> AlterDatabaseInfo(DataBaseInfo dbinfo, List<Gabana.ORM.Master.ScriptAlterMerchantDB> lstscriptAlters)
        {
            using (var db = new MerchantDB(DataCashingAll.Pathdb))
            {
                var transection = await db.BeginTransactionAsync();
                try
                {
                    var resultInfo = db.DataBaseInfo.Where(x => x.KeyDBInfo == "GabanaErVersion").FirstOrDefault();
                    if (resultInfo != null)
                    {
                        foreach (var scriptAlters in lstscriptAlters)
                        {
                            var result = await db.ExecuteAsync(scriptAlters.ScriptAlter);
                            if (result == -1)
                            {
                                await transection.RollbackAsync();
                                return false;
                            }
                        }

                        var Update = await db.DataBaseInfo.Where(x => x.KeyDBInfo == dbinfo.KeyDBInfo)
                             .Set(c => c.DataDBInfo, dbinfo.DataDBInfo ?? dbinfo.DataDBInfo)
                             .UpdateAsync();
                        if (Update != 1)
                        {
                            await transection.RollbackAsync();
                            return false;
                        }

                        await transection.CommitAsync();
                        return true;
                    }
                    await transection.RollbackAsync();
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await transection.RollbackAsync();
                    return false;
                };
            }           
        }        
    }
}
