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
    public class NoteCategoryManage
    {
        DeviceSystemSeqNoManage DeviceSeqManage = new DeviceSystemSeqNoManage();
        int systemId = initSystemID.SYSTEMID.NoteCategory;

        public async Task<List<NoteCategory>> GetAllNoteCategory()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.NoteCategories.Where(x=>x.DataStatus != 'D')
                        .OrderBy(x=>x.Name).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<NoteCategory>> GetNoteCategoryOption()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    //var lstitem = await db.NoteCategories.Where(x => x.DataStatus != 'D')
                    //    .OrderBy(x => x.Name).ToListAsync();

                    var listoption = await db.NoteCategories.Where(x=>x.DataStatus != 'D' && x.MerchantID == DataCashingAll.MerchantId)
                                                                .Join(db.Notes , m=> m.SysNoteCategoryID,x=>x.SysNoteCategoryID,(m,x)=>new { m,x})
                                                                .Select( x=> new NoteCategory()
                                                                { 
                                                                    MerchantID = x.m.MerchantID,
                                                                    SysNoteCategoryID = x.m.SysNoteCategoryID,
                                                                    Ordinary =x.m.Ordinary,
                                                                    Name = x.m.Name,
                                                                    DateModified = x.m.DateModified,
                                                                    DataStatus = x.m.DataStatus,
                                                                    FWaitSending = x.m.FWaitSending,
                                                                    WaitSendingTime = x.m.WaitSendingTime

                                                                })
                                                                .OrderBy(x=>x.Name).Distinct().ToListAsync();

                    return listoption;
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
                    var result = await db.NoteCategories.Where(x => x.MerchantID == merchantId).ToListAsync();
                    if (result != null)
                    {
                        max = result.Max(y => y.SysNoteCategoryID);
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

        public async Task<bool> InsertNoteCategory(NoteCategory category)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            using (var db = new MerchantDB(DataCashingAll.Pathdb))
            {
                var transection = await db.BeginTransactionAsync();
                try
                {
                    var checkstatus = await db.InsertAsync<NoteCategory>(category);
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
                };
            }
        }

        public async Task<List<NoteCategory>> GetCategorySearch(string categoryName)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                List<NoteCategory> listcategory = new List<NoteCategory>();
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    if (!string.IsNullOrEmpty(categoryName)) //ItemName
                    {
                        listcategory = await db.NoteCategories.Where(m => m.Name.Contains(categoryName)).ToListAsync();
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

        public async Task<NoteCategory> GetNoteCategory(int merchant, int SysNoteCategoryID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = await db.NoteCategories.Where(x => x.MerchantID == merchant && x.SysNoteCategoryID == SysNoteCategoryID).FirstOrDefaultAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<NoteCategory> GetNoteCategoryAndroid(int merchant, int SysNoteCategoryID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    try
                    {
                        await db.BeginTransactionAsync();
                        var result = await db.NoteCategories.Where(x => x.MerchantID == merchant && x.SysNoteCategoryID == SysNoteCategoryID).FirstOrDefaultAsync();
                        if (result != null)
                        {
                            result.FWaitSending = 1;
                        }

                        await db.UpdateAsync<NoteCategory>(result);
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

        public async Task<List<NoteCategory>> GetNoteCategoryFwaiting()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstnotecategory = new List<NoteCategory>();
                    lstnotecategory = await db.NoteCategories.Where(x => x.FWaitSending == 2).ToListAsync();
                    return lstnotecategory;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertOrReplaceCategory(NoteCategory category)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {

                    var checkstatus = await db.InsertOrReplaceAsync<NoteCategory>(category);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateNoteCategory(NoteCategory category)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.NoteCategories.Where(a => a.MerchantID == category.MerchantID && a.SysNoteCategoryID == category.SysNoteCategoryID).FirstOrDefault();
                    if (result != null)
                    {                       
                        var UpdateCategory = await db.UpdateAsync<NoteCategory>(category);
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

        public async Task<bool> DeleteNoteCategory(int merchantId, int SysNoteCategoryID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.NoteCategories.Where(m => m.MerchantID == merchantId && m.SysNoteCategoryID == SysNoteCategoryID).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteCategory = await db.NoteCategories.Where(c => c.MerchantID == merchantId && c.SysNoteCategoryID == SysNoteCategoryID).DeleteAsync();
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

        public async Task<bool> CheckNoteCategoryName(string NoteCategoryName)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))

                {
                    var lstnoteCate = await db.NoteCategories.Where(x => x.DataStatus != 'D' & x.Name.Contains(NoteCategoryName)).OrderBy(x => x.Name).ToListAsync();
                    foreach (var item in lstnoteCate)
                    {
                        var result = string.Equals(NoteCategoryName, item.Name, StringComparison.CurrentCultureIgnoreCase);
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
