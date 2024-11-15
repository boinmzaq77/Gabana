using Gabana.ORM;
using Gabana.ORM.MerchantDB;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Manage
{
   public class AppConfigManage
    {
        public async Task<List<AppConfig>> GetAppConfig(string ConfigKey)
        {
            try
            {
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstappConfigs = new List<AppConfig>();
                    var Config = await db.AppConfigs.Where(x => x.CfgKey == ConfigKey).FirstOrDefaultAsync();                   
                    if (Config != null)
                    {
                        lstappConfigs = await db.AppConfigs.ToListAsync<AppConfig>();
                    }
                    return lstappConfigs;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertAppconfig(AppConfig app)
        {
            try
            {
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus =await db.InsertAsync<AppConfig>(app);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateAppconfig(AppConfig app)
        {
            try
            {
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.AppConfigs.Where(a => a.CfgKey == app.CfgKey).FirstOrDefault();
                    if (result != null)
                    {
                        //var UpdateAppconfig = db.AppConfigs.Where(x => x.CfgKey == app.CfgKey)
                        //     .Set(c => c.CfgString, app.CfgString ?? app.CfgString)
                        //     .Set(c => c.CfgInteger, app.CfgInteger ?? app.CfgInteger)
                        //     .Set(c => c.CfgFloat, app.CfgFloat ?? app.CfgFloat)
                        //     .Set(c => c.CfgDate, app.CfgDate ?? app.CfgDate)
                        //     .UpdateAsync();
                        var UpdateAppconfig = await db.UpdateAsync<AppConfig>(app);
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

        public async Task<bool> DeleteAppconfig(string Key)
        {
            try
            {
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.AppConfigs.Where(m => m.CfgKey == Key).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteAppconfig = await db.AppConfigs.Where(c => c.CfgKey == Key).DeleteAsync();
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
    }
}
