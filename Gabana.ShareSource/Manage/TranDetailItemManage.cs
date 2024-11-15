using Gabana.Model;
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
    public class TranDetailItemManage
    {
        public async Task<List<TranDetailItem>> GetTranDetailItem(int merchantId, int SysBranchID, string tranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lsttran = new List<TranDetailItem>();
                    var result = await db.TranDetailItems.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranNo == tranNo).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        lsttran = await db.TranDetailItems.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranNo == tranNo).ToListAsync<TranDetailItem>();
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

        public async Task<TranDetailItem> GetOneTranDetailItem(int merchantId, int SysBranchID, string tranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lsttran = new TranDetailItem();
                    var result = await db.TranDetailItems.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranNo == tranNo).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        lsttran = await db.TranDetailItems.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranNo == tranNo).FirstOrDefaultAsync<TranDetailItem>();
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

        public async Task<bool> InsertTranDetailItem(TranDetailItem tranDetail, MerchantDB db)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                var checkstatus = await db.InsertAsync<TranDetailItem>(tranDetail);
                return (checkstatus > 0 ? true : false);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }

        public async Task<bool> UpdateTranDetailItem(TranDetailItem tranDetail)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.TranDetailItems.Where(m => m.MerchantID == tranDetail.MerchantID && m.SysBranchID == tranDetail.SysBranchID && m.TranNo == tranDetail.TranNo && m.DetailNo == tranDetail.DetailNo).FirstOrDefault();
                    if (result != null)
                    {
                        var UpdateItem = await db.UpdateAsync<TranDetailItem>(tranDetail);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }

        public async Task<bool> DeleteTranDetailItem(int merchantId, int SysBranchID, string tranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.TranDetailItems.Where(m => m.MerchantID == merchantId && m.SysBranchID == SysBranchID && m.TranNo == tranNo ).FirstOrDefault();
                    if (result != null)
                    {
                        var DeletetranDetail = await db.TranDetailItems.Where(m => m.MerchantID == merchantId && m.SysBranchID == SysBranchID && m.TranNo == tranNo ).DeleteAsync();
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
