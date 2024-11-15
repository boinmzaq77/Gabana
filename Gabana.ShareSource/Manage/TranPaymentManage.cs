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
    public class TranPaymentManage
    {
        public async Task<List<TranPayment>> GetTranPayment(int merchantId, int SysBranchID, string tranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lsttran = new List<TranPayment>();
                    var result = await db.TranPayments.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranNo == tranNo ).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        lsttran = await db.TranPayments.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranNo == tranNo ).ToListAsync<TranPayment>();
                    }
                    return lsttran;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }

        public async Task<bool> InsertTranPayment(TranPayment tranPayment,MerchantDB db)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                var checkstatus = await db.InsertAsync<TranPayment>(tranPayment);
                return (checkstatus > 0 ? true : false);                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }

        public async Task<bool> UpdateTranPayment(TranPayment tranPayment)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.TranPayments.Where(m => m.MerchantID == tranPayment.MerchantID && m.SysBranchID == tranPayment.SysBranchID && m.TranNo == tranPayment.TranNo && m.PaymentNo == tranPayment.PaymentNo).FirstOrDefault();
                    if (result != null)
                    {                        
                        var UpdateItem = await db.UpdateAsync<TranPayment>(tranPayment);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }

        public async Task<bool> DeleteTranPayment(int merchantId, int SysBranchID, string tranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.TranPayments.Where(m => m.MerchantID == merchantId && m.SysBranchID == SysBranchID && m.TranNo == tranNo ).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteTranPayment = await db.TranPayments.Where(m => m.MerchantID == merchantId && m.SysBranchID == SysBranchID && m.TranNo == tranNo).DeleteAsync();
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
