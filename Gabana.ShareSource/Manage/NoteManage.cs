using Gabana.Model;
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
    public class NoteManage
    {
        DeviceSystemSeqNoManage DeviceSeqManage = new DeviceSystemSeqNoManage();
        int systemId = initSystemID.SYSTEMID.Note;

        public async Task<List<Note>> GetAllNote(int MerID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Notes.Where(c => c.MerchantID == MerID & c.DataStatus != 'D').OrderBy(x=>x.Message).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Note>> GetNoteOnlyNoneGroup(int MerID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Notes.Where(c => c.MerchantID == MerID & c.DataStatus != 'D' & c.SysNoteCategoryID == null ).OrderBy(x => x.Message).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Note>> GetNoteBYCategory(int MerID,int sysNoteCategory)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Notes.Where(c=>c.SysNoteCategoryID == sysNoteCategory && c.MerchantID == MerID & c.DataStatus != 'D').OrderBy(x=>x.Message).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Note>> GetNoteBYCategorySearch(int MerID, int sysNoteCategory,string txtSearch)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Notes.Where(c => c.SysNoteCategoryID == sysNoteCategory && c.MerchantID == MerID & c.DataStatus != 'D' && c.Message.Contains(txtSearch)).OrderBy(x => x.Message).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<int> GetLastItem()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var itemlast = db.Notes
                              .OrderByDescending(x => x.SysNoteID)
                              .Take(1)
                              .Select(x => x.SysNoteID)
                              .ToList()
                              .FirstOrDefault();
                    return Convert.ToInt32(itemlast);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            };
        }      

        public async Task<Note> GetNote(int merchantId, int SysNoteID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = new List<Note>();
                    var result = await db.Notes.Where(x => x.MerchantID == merchantId && x.SysNoteID == SysNoteID).FirstOrDefaultAsync();
                    
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<Note> GetNoteAndroid(int merchantId, int SysNoteID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    try
                    {
                        await db.BeginTransactionAsync();
                        var result = await db.Notes.Where(x => x.MerchantID == merchantId && x.SysNoteID == SysNoteID).FirstOrDefaultAsync();
                        if (result != null)
                        {
                            result.FWaitSending = 1;
                        }
                        await db.UpdateAsync<Note>(result);
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

        public async Task<List<Note>> GetNoteFwaiting()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = new List<Note>();
                    lstitem = await db.Notes.Where(x => x.FWaitSending == 2).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<long> GetMaxNoteID(long merchantId, int deviceNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    long max = 0;
                    var result = await db.Notes.Where(x => x.MerchantID == merchantId).ToListAsync();
                    if (result != null)
                    {
                        max = result.Max(y => y.SysNoteID);
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

        public async Task<bool> InsertNote(Note  note)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            using (var db = new MerchantDB(DataCashingAll.Pathdb))
            {
                var transection = await db.BeginTransactionAsync();
                try
                {
                    var checkstatus = await db.InsertAsync<Note>(note);
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

        public async Task<bool> InsertOrReplaceNote(Note note)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {

                    var checkstatus = await db.InsertOrReplaceAsync<Note>(note);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateNote(Note  note)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.Notes.Where(m => m.MerchantID == note.MerchantID && m.SysNoteID == note.SysNoteID).FirstOrDefault();
                    if (result != null)
                    {                       
                        var UpdateItem = await db.UpdateAsync<Note>(note);
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

        public async Task<bool> DeleteNote(int merchantId, int SysNoteID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = await db.Notes.Where(m => m.MerchantID == merchantId && m.SysNoteID == SysNoteID).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        var Deleteitem = await db.Notes.Where(m => m.MerchantID == merchantId && m.SysNoteID == SysNoteID).DeleteAsync();
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

        public async Task<bool> DeleteAllNoteByNoteCategory(int merchantId, int SysNoteCategoryID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = await db.Notes.Where(m => m.MerchantID == merchantId && m.SysNoteCategoryID == SysNoteCategoryID).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        var Deleteitem = await db.Notes.Where(m => m.MerchantID == merchantId && m.SysNoteCategoryID == SysNoteCategoryID).DeleteAsync();
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

        //Search
        public async Task<List<Note>> GetNoteSearch(string ItemName,int filter)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                List<Note> listNote = new List<Note>();
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    if (!string.IsNullOrEmpty(ItemName)) //ItemName
                    {
                        if (filter == 0)
                        {
                            listNote = await db.Notes.Where(m => m.Message.Contains(ItemName)).ToListAsync();
                        }
                        else
                        {
                            listNote = await db.Notes.Where(m => m.Message.Contains(ItemName) & m.SysNoteCategoryID == filter).ToListAsync();
                        }                        
                    }                   
                    return listNote;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> CheckNoteName(string NoteName)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstnote = await db.Notes.Where(x => x.DataStatus != 'D' & x.Message.Contains(NoteName)).OrderBy(x => x.Message).ToListAsync();
                    foreach (var item in lstnote)
                    {
                        var result = string.Equals(NoteName, item.Message, StringComparison.CurrentCultureIgnoreCase);
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

        //-----------------------------------------------------------------------
        //Note
        //------------------------------------------------------------------------

        //public async Task<List<showNoteData>> GetNoteDetail()
        //{
        //    try
        //    {
        //        using (var db = new MerchantDB(DataCashingAll.Pathdb))
        //        {
        //            var lstNote = await db.NoteCategories.Join(db.Notes,
        //                                            x => x.MerchantID & x.SysNoteCategoryID,
        //                                            y => y.MerchantID & y.SysNoteCategoryID,
        //                                            (x, y) => new { NoteCategory = x, Note = y })
        //                                    .Where(x => x.NoteCategory.MerchantID == DataCashingAll.MerchantId)
        //                                    .GroupBy(x => x.NoteCategory)
        //                                    .ToListAsync();
        //            return lstNote;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return null;
        //    };
        //}

        //public async Task<List<showNoteDetail>> GetAllNote()
        //{
        //    try
        //    {
        //        using (var db = new MerchantDB(DataCashingAll.Pathdb))
        //        {
        //            var lstNote = await db.Categories.Join(db.Items,
        //                                            x => x.MerchantID & x.SysCategoryID,
        //                                            y => y.MerchantID & y.SysCategoryID,
        //                                            (x, y) => new { categories = x, items = y })
        //                                    .Where(x => x.categories.MerchantID == DataCashingAll.MerchantId & x.items.SaleItemType == 'N')
        //                                    .GroupBy(x => x.categories)
        //                                    .Select(x => new showNoteDetail
        //                                    {
        //                                        SysCategoryID = x.Key.SysCategoryID,
        //                                        Name = x.Key.Name,
        //                                        totalItem = x.Where(y => y.items.SysCategoryID == x.Key.SysCategoryID).Count()
        //                                    }).ToListAsync();

        //            //var Categories = await db.Categories
        //            //                   .Where(x => x.MerchantID == merchantId)
        //            //                   .Select(x => new showNoteDetail
        //            //                   {
        //            //                       SysCategoryID = x.SysCategoryID,
        //            //                       Name = x.Name,
        //            //                       totalItem = db.Items.Where(y => y.MerchantID == x.MerchantID & y.SysCategoryID == x.SysCategoryID).Count()
        //            //                   }).ToListAsync();

        //            return lstNote;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return null;
        //    };
        //}
    }
}
