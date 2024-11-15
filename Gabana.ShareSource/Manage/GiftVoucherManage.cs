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
    public  class GiftVoucherManage
    {       
        public async Task<List<GiftVoucher>> GetAllGiftVoucher()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstGiftVoucher = new List<GiftVoucher>();
                    lstGiftVoucher = await db.GiftVouchers.ToListAsync<GiftVoucher>();                    
                    return lstGiftVoucher;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }        

        public async Task<GiftVoucher> GetGiftVoucher(int MerchantID, string GiftVoucherCode)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var GiftVoucher = new GiftVoucher();
                    GiftVoucher = await db.GiftVouchers.Where(x => x.MerchantID == MerchantID && x.GiftVoucherCode == GiftVoucherCode).FirstOrDefaultAsync();
                    return GiftVoucher;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertGiftVoucher(GiftVoucher voucher)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertAsync<GiftVoucher>(voucher);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertOrReplaceGiftVoucher(GiftVoucher voucher)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertOrReplaceAsync<GiftVoucher>(voucher);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateGiftVoucher(GiftVoucher voucher)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.GiftVouchers.Where(a => a.MerchantID == voucher.MerchantID && a.GiftVoucherCode == voucher.GiftVoucherCode).FirstOrDefault();
                    if (result != null)
                    {                        
                        var UpdateItem = await db.UpdateAsync<GiftVoucher>(voucher);
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

        public async Task<bool> DeleteGiftVoucher(int MerchantID, string GiftVoucherCode)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.GiftVouchers.Where(m => m.MerchantID == MerchantID && m.GiftVoucherCode == GiftVoucherCode).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteItem = await db.GiftVouchers.Where(m => m.MerchantID == MerchantID && m.GiftVoucherCode == GiftVoucherCode).DeleteAsync();
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

        public async Task<bool> DeleteAllGiftVoucher(int MerchantID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.GiftVouchers.Where(m => m.MerchantID == MerchantID).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteItem = await db.GiftVouchers.Where(m => m.MerchantID == MerchantID).DeleteAsync();
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
