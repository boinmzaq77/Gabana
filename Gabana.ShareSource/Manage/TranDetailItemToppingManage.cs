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
    public class TranDetailItemToppingManage
    {
        public async Task<List<TranDetailItemTopping>> GetTranDetailItemTopping(int merchantId, int SysBranchID, string tranNo, int DetailNo )
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lsttran = new List<TranDetailItemTopping>();
                    var result = await db.TranDetailItemToppings.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranNo == tranNo).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        lsttran = await db.TranDetailItemToppings.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranNo == tranNo & x.DetailNo == DetailNo).ToListAsync<TranDetailItemTopping>();
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

        public async Task<List<TranDetailItemTopping>> GetListTranDetailItemTopping(int merchantId, int SysBranchID, string tranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lsttrantopping = new List<TranDetailItemTopping>();
                    var result = await db.TranDetailItemToppings.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        lsttrantopping = await db.TranDetailItemToppings.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranNo == tranNo).ToListAsync<TranDetailItemTopping>();
                    }
                    return lsttrantopping;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertTranDetailItemTopping(TranDetailItemTopping tranDetail, MerchantDB db)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                var checkstatus = await db.InsertAsync<TranDetailItemTopping>(tranDetail);
                return (checkstatus > 0 ? true : false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertListTranDetailItemTopping(List<TranDetailItemTopping> tranDetail, MerchantDB db)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                int checkstatus = 0;
                foreach (var item in tranDetail)
                {
                    checkstatus = await db.InsertAsync<TranDetailItemTopping>(item);
                }                
                return (checkstatus > 0 ? true : false);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }

        public async Task<bool> UpdateTranDetailItemTopping(TranDetailItemTopping tranDetail)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.TranDetailItemToppings.Where(m => m.MerchantID == tranDetail.MerchantID && m.SysBranchID == tranDetail.SysBranchID && m.TranNo == tranDetail.TranNo && m.DetailNo == tranDetail.DetailNo).FirstOrDefault();
                    if (result != null)
                    {
                        var UpdateItem = await db.UpdateAsync<TranDetailItemTopping>(tranDetail);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }

        public async Task<bool> DeleteTranDetailItemTopping(int merchantId, int SysBranchID, string tranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.TranDetailItemToppings.Where(m => m.MerchantID == merchantId && m.SysBranchID == SysBranchID && m.TranNo == tranNo ).FirstOrDefault();
                    if (result != null)
                    {
                        var DeletetranDetail = await db.TranDetailItemToppings.Where(m => m.MerchantID == merchantId && m.SysBranchID == SysBranchID && m.TranNo == tranNo ).DeleteAsync();
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
