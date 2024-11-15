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
    public class SystemInfoManage
    {       

        public async Task<List<SystemInfo>> GetSystemInfo()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstsystem = new List<SystemInfo>();
                    var result = await db.SystemInfo.FirstOrDefaultAsync();
                    if (result != null)
                    {
                        lstsystem = await db.SystemInfo.ToListAsync<SystemInfo>();
                    }
                    return lstsystem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertSystemInfo(List<SystemInfo> systemInfo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                var checkstatus = 0;
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    for (int i = 0; i < systemInfo.Count; i++)
                    {
                        checkstatus = await db.InsertAsync<SystemInfo>(systemInfo[i]);                        
                    }

                    if (checkstatus == 0)
                    {
                        return false;
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

        public async Task<bool> UpdateSystemInfo(SystemInfo systemInfo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.SystemInfo.Where(m =>  m.SystemID == systemInfo.SystemID).FirstOrDefault();
                    if (result != null)
                    {
                        var UpdateSysteminfo = await db.UpdateAsync<SystemInfo>(systemInfo);
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

        public async Task<bool> DeleteSystemInfo(int systemID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.SystemInfo.Where(m =>  m.SystemID == systemID).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteSystem = await db.SystemInfo.Where(c =>c.SystemID == systemID).DeleteAsync();
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
