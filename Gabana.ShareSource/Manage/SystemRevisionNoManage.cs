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
    public class SystemRevisionNoManage
    {
        public async Task<List<SystemRevisionNo>> GetAllSystemRevisionNo()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstsystem = new List<SystemRevisionNo>();                   
                        lstsystem = await db.SystemRevisionNoes.ToListAsync<SystemRevisionNo>();                   
                    return lstsystem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<SystemRevisionNo> GetSystemRevisionNo(int merchantID, int systemID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var RevisionNo = new SystemRevisionNo();
                    var result = await db.SystemRevisionNoes.Where(x => x.MerchantID == merchantID && x.SystemID == systemID).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        RevisionNo = await db.SystemRevisionNoes.Where(x => x.MerchantID == merchantID && x.SystemID == systemID).FirstOrDefaultAsync<SystemRevisionNo>();
                    }
                    return RevisionNo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertSystemRevisionno(SystemRevisionNo   systemno)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertAsync<SystemRevisionNo>(systemno);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateSystemReviosion(SystemRevisionNo systemno)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.SystemRevisionNoes.Where(m => m.MerchantID == systemno.MerchantID && m.SystemID == systemno.SystemID).FirstOrDefault();
                    if (result != null)
                    {
                        var UpdateBranch = await db.UpdateAsync<SystemRevisionNo>(systemno);
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

        public async Task<bool> DeleteSystemReviosion(int merchantID, int systemID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.SystemRevisionNoes.Where(m => m.MerchantID == merchantID && m.SystemID == systemID).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteSystem = await db.SystemRevisionNoes.Where(c => c.MerchantID == merchantID && c.SystemID == systemID).DeleteAsync();
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
