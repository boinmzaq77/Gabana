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
    public class TranTradDiscountManage
    {
        public async Task<List<TranTradDiscount>> GetTranTradDiscount(int merchantId, int SysBranchID, string tranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lsttran = new List<TranTradDiscount>();
                    var result = await db.TranTradDiscounts.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranNo == tranNo).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        lsttran = await db.TranTradDiscounts.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranNo == tranNo).ToListAsync<TranTradDiscount>();
                    }
                    return lsttran;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertTranTradDiscount(TranTradDiscount tran, MerchantDB db)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                var checkstatus = await db.InsertAsync<TranTradDiscount>(tran);
                return (checkstatus > 0 ? true : false);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);                
            };
        }

        public async Task<bool> InsertListTranTradDiscount(List<TranTradDiscount> tran, MerchantDB db)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    int checkstatus = 0;
                    foreach (var item in tran)
                    {
                        checkstatus = await db.InsertAsync<TranTradDiscount>(item);
                    }
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }

        public async Task<bool> UpdateTranTradDiscount(TranTradDiscount tran)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.TranTradDiscounts.Where(m => m.MerchantID == tran.MerchantID && m.SysBranchID == tran.SysBranchID && m.TranNo == tran.TranNo && m.TradDiscountNo == tran.TradDiscountNo).FirstOrDefault();
                    if (result != null)
                    {
                        var UpdateTran = await db.UpdateAsync<TranTradDiscount>(tran);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }

        public async Task<bool> DeleteTranTradDiscount(int merchantId, int SysBranchID, string tranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.TranTradDiscounts.Where(m => m.MerchantID == merchantId && m.SysBranchID == SysBranchID && m.TranNo == tranNo ).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteTran = await db.Trans.Where(m => m.MerchantID == merchantId && m.SysBranchID == SysBranchID && m.TranNo == tranNo).DeleteAsync();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }
    }
}
