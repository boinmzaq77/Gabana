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
    public class ItemExSizeManage
    {
        public async Task<List<ItemExSize>> GetAllItemSize()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitemsize = await db.ItemExSizes.ToListAsync<ItemExSize>();
                    return lstitemsize;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<ItemExSize>> GetItemSize(int merchantId,int sysItemId)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitemsize = new List<ItemExSize>();
                    var result = await db.ItemExSizes.Where(x => x.MerchantID == merchantId && x.SysItemID == sysItemId ).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        lstitemsize = await db.ItemExSizes.Where(x => x.MerchantID == merchantId && x.SysItemID == sysItemId ).ToListAsync<ItemExSize>();
                    }
                    return lstitemsize;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }        

        public async Task<bool> InsertItemsize(ItemExSize item)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertAsync<ItemExSize>(item);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertListItemsize(List<ItemExSize> item)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                int checkstatus = 0;
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var transection = await db.BeginTransactionAsync();
                    foreach (var i in item)
                    {                        
                        checkstatus = await db.InsertAsync<ItemExSize>(i);
                        if (checkstatus == 0)
                        {
                            await transection.RollbackAsync();
                            return false;
                        }                       
                    }
                    await transection.CommitAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertOrReplaceItemSize(ItemExSize itemExSize)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {

                    var checkstatus = await db.InsertOrReplaceAsync<ItemExSize>(itemExSize);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }
        public async Task<bool> UpdateItemsize(List<ItemExSize> item)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    for (int i = 0; i < item.Count; i++)
                    {
                        var UpdateItemsize = await db.InsertOrReplaceAsync<ItemExSize>(item[i]);                        
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

        //public async Task<bool> DeleteItemsize(int merchantId, int sysItemId, int exsizeNo)
        //{
        //    try
        //    {
        //        using (var db = new MerchantDB(DataCashingAll.Pathdb))
        //        {
        //            var result = db.ItemExSizes.Where(m => m.MerchantID == merchantId && m.SysItemID == sysItemId && m.ExSizeNo == exsizeNo).FirstOrDefault();
        //            if (result != null)
        //            {
        //                var Deleteitemsize = await db.ItemExSizes.Where(m => m.MerchantID == merchantId && m.SysItemID == sysItemId && m.ExSizeNo == exsizeNo).DeleteAsync();
        //                return true;
        //            }
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return false;
        //    };
        //}

        public async Task<bool> DeleteItemsize(int merchantId, int sysItemId)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.ItemExSizes.Where(m => m.MerchantID == merchantId && m.SysItemID == sysItemId).FirstOrDefault();
                    if (result != null)
                    {
                        var transection = await db.BeginTransactionAsync();
                        var Deleteitemsize = await db.ItemExSizes.Where(m => m.MerchantID == merchantId && m.SysItemID == sysItemId).DeleteAsync();
                        if (Deleteitemsize == 0)
                        {
                            await transection.RollbackAsync();
                            return false;
                        }
                        await transection.CommitAsync();
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
