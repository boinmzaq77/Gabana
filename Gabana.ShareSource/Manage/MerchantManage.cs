//using Android.Util;
using Gabana.ORM.MerchantDB;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Gabana.ShareSource.Manage
{
    public class MerchantManage
    {      
        
        public async Task<List<Merchant>> GetAllMerchant()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstmerchant = await db.Merchants.ToListAsync<Merchant>();
                    return lstmerchant;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<Merchant> GetMerchant(int merchantId)
        {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {                   
                    var result = await db.Merchants.Where(x => x.MerchantID == merchantId).FirstOrDefaultAsync();
                    if (result == null)
                    {
                        throw new Exception();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                //Merchant merchant = new Merchant();
                //merchant.Name = ex.Message + " merchantId: " + merchantId + " DataCashingAll.Pathdb :" + DataCashingAll.Pathdb;
                //return merchant;
                return null;
            };
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        }

        public async Task<bool> InsertMerchant(Merchant merchant)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                    var checkstatus = await db.InsertAsync<Merchant>(merchant);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateMerchant(Merchant merchant)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                    var result = await db.Merchants.Where(m => m.MerchantID == merchant.MerchantID).FirstOrDefaultAsync();
                    if (result != null)
                    {   
                        var UpdateItem = await db.UpdateAsync<Merchant>(merchant);
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

        public async Task<bool> DeleteMerchant(int merchantId)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.Merchants.Where(m => m.MerchantID == merchantId).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteMerchant = await db.Merchants.Where(m => m.MerchantID == merchantId).DeleteAsync();
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
