using Gabana.ORM;
using Gabana.ORM.MerchantDB;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Manage
{
    public class MerchantConfigManage
    {
        public async Task<List<MerchantConfig>> GetMerchantConfig(long merchantID,string key)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstappConfigs = new List<MerchantConfig>();
                    var Config = await db.MerchantConfigs.Where(x => x.MerchantID == merchantID & x.CfgKey == key).FirstOrDefaultAsync();                   
                    if (Config != null)
                    {
                        lstappConfigs = await db.MerchantConfigs.ToListAsync<MerchantConfig>();
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

        public async Task<bool> InsertMerchantConfig(MerchantConfig merchantConfig)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus =await db.InsertAsync<MerchantConfig>(merchantConfig);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertorReplacrMerchantConfig(MerchantConfig merchantConfig)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertOrReplaceAsync<MerchantConfig>(merchantConfig);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertorReplaceListMerchantConfig(List<MerchantConfig> lstmerchantConfig)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    int checkstatus = 0;
                    foreach (var item in lstmerchantConfig)
                    {
                        checkstatus = await db.InsertOrReplaceAsync<MerchantConfig>(item);                       
                    }
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateMerchantConfig(MerchantConfig  merchantConfig)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.MerchantConfigs.Where(a => a.MerchantID == merchantConfig.MerchantID).FirstOrDefault();
                    if (result != null)
                    {
                        var UpdateAppconfig = await db.UpdateAsync<MerchantConfig>(merchantConfig);
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

        public async Task<bool> DeleteMerchantConfig(long merchantID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.MerchantConfigs.Where(m => m.MerchantID == merchantID).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteAppconfig = await db.MerchantConfigs.Where(c => c.MerchantID == merchantID).DeleteAsync();
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
