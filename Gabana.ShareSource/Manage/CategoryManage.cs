using Gabana.Model;
using Gabana.ORM;
using Gabana.ORM.MerchantDB;
using Gabana3.JAM.Items;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Manage
{
    public class CategoryManage
    {
        DeviceSystemSeqNoManage DeviceSeqManage = new DeviceSystemSeqNoManage();
        int systemId = initSystemID.SYSTEMID.Category;

        public async Task<List<Category>> GetAllCategory()
        {            
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstCategory = await db.Categories.Where(x => x.DataStatus != 'D' && x.MerchantID == DataCashingAll.MerchantId).OrderBy(x => x.Name).ToListAsync<Category>();
                    return lstCategory;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Category>> GetAllCategoryhaveitem()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstCategory = await db.Categories.Where(x => x.DataStatus != 'D' && x.MerchantID == DataCashingAll.MerchantId)
                    .Join(db.Items.Where(m => m.DataStatus != 'D' && m.SaleItemType != 'T'), m => m.SysCategoryID, x => x.SysCategoryID, (m, x) => new { m, x })
                                                               .Select(x => x.m).Distinct().ToListAsync();
                    return lstCategory;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Category>> GetCategoryOption()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    //var lstCategory = await db.Categories.Where(x => x.DataStatus != 'D').OrderBy(x => x.Name).ToListAsync<Category>();

                    var listoption = await db.Categories.Where(x => x.DataStatus != 'D' && x.MerchantID == DataCashingAll.MerchantId)
                    .Join(db.Items.Where(m=> m.DataStatus != 'D' && m.SaleItemType == 'T' ), m => m.SysCategoryID, x => x.SysCategoryID,(m, x) => new { m, x })                    
                    .Select(x => x.m)
                    .OrderBy(x => x.Name).Distinct().ToListAsync();
                    return listoption;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Category>> GetCategorySearch(string categoryName)
        {            
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                List<Category> listcategory = new List<Category>();
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    if (!string.IsNullOrEmpty(categoryName)) //ItemName
                    {
                        listcategory = await db.Categories.Where(m => m.Name.Contains(categoryName) & m.DataStatus != 'D').ToListAsync();
                    }
                    return listcategory;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<Category> GetCategory(int merchant, int syscategoryID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = await db.Categories.Where(x => x.MerchantID == merchant && x.SysCategoryID == syscategoryID & x.DataStatus != 'D').FirstOrDefaultAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<Category> GetCategorySync(int merchant, int syscategoryID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = await db.Categories.Where(x => x.MerchantID == merchant && x.SysCategoryID == syscategoryID ).FirstOrDefaultAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<Category> GetCategorySyncAndroid(int merchant, int syscategoryID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    try
                    {
                        await db.BeginTransactionAsync();
                        var result = await db.Categories.Where(x => x.MerchantID == merchant && x.SysCategoryID == syscategoryID).FirstOrDefaultAsync();
                        if (result != null)
                        {
                            result.FWaitSending = 1;
                        }

                        var UpdateCategoey = await db.UpdateAsync<Category>(result);
                        await db.CommitTransactionAsync();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        await db.RollbackTransactionAsync();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Category>> GetCategoryFwaiting()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstcategory = new List<Category>();
                    lstcategory = await db.Categories.Where(x => x.FWaitSending == 2).ToListAsync();
                    return lstcategory;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Category>> GetCategoryTopping()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    List<Category> lstCategory = new List<Category>(); 
                    var item = await db.Items.Where(x => x.SaleItemType == 'T' & x.DataStatus != 'D').ToListAsync();
                    foreach (var i in item)
                    {
                        lstCategory = await db.Categories.Where(x => x.SysCategoryID == i.SysCategoryID).OrderBy(x => x.Name).ToListAsync<Category>();
                    }                    
                    return lstCategory;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<long> GetMaxCategoryID(long merchantId, int deviceNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    long max = 0;
                    var result = await db.Categories.Where(x => x.MerchantID == merchantId).ToListAsync();
                    if (result != null)
                    {
                        max = result.Max(y => y.SysCategoryID);
                        max.ToString();
                        var arraydeviceNo = deviceNo.ToString().Split();
                        var a = arraydeviceNo.Length;
                        var b = max.ToString().Substring(a);
                        var c = long.Parse(b);

                        max = c;
                    }
                    else
                    {
                        max = 0;
                    }
                    return max;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            };
        }
        
        public async Task<bool> InsertCategory(Category category)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            using (var db = new MerchantDB(DataCashingAll.Pathdb))
            {
                var transection = await db.BeginTransactionAsync();
                try
                {
                    var checkstatus = await db.InsertAsync<Category>(category);
                    if (checkstatus != 1)
                    {
                        await transection.RollbackAsync();
                        return false;
                    }

                    //Insert SeqNo;
                    var insertSeq = await DeviceSeqManage.GetDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, systemId);
                    var lastSeqNo = insertSeq + 1;
                    DeviceSystemSeqNo DeviceSeq = new DeviceSystemSeqNo()
                    {
                        DeviceNo = DataCashingAll.DeviceNo,
                        MerchantID = DataCashingAll.MerchantId,
                        SystemID = systemId,
                        LastSysSeqNo = lastSeqNo
                    };
                    var checkResult = await DeviceSeqManage.UpdateDeviceSystemSeqNo(DeviceSeq, db);
                    if (!checkResult)
                    {
                        await transection.RollbackAsync();
                        return false;
                    }

                    await transection.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transection.RollbackAsync();
                    throw ex;
                }
            } 
        }

        public async Task<bool> InsertOrReplaceCategory(Category category)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {

                    var checkstatus = await db.InsertOrReplaceAsync<Category>(category);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.Categories.Where(a => a.MerchantID == category.MerchantID && a.SysCategoryID == category.SysCategoryID).FirstOrDefault();
                    if (result != null)
                    {
                        var UpdateCategory = await db.UpdateAsync<Category>(category);
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

        public async Task<bool> DeleteCategory(int merchantId, int syscategory)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.Categories.Where(m => m.MerchantID == merchantId && m.SysCategoryID == syscategory).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteCategory = await db.Categories.Where(c => c.MerchantID == merchantId && c.SysCategoryID == syscategory).DeleteAsync();
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

        public async Task<bool> CheckCategoryName(string CategoryName)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstcategory = await db.Categories.Where(x => x.DataStatus != 'D' & x.Name.Contains(CategoryName)).OrderBy(x => x.Name).ToListAsync();
                    foreach (var item in lstcategory)
                    {
                        var result = string.Equals(CategoryName, item.Name, StringComparison.CurrentCultureIgnoreCase);
                        if (result)
                        {
                            return true;
                        }
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
