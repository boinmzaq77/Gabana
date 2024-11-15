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
using System.Transactions;

namespace Gabana.ShareSource.Manage
{
    public class ItemManage
    {
        DeviceSystemSeqNoManage DeviceSeqManage = new DeviceSystemSeqNoManage();
        ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
        int systemId = initSystemID.SYSTEMID.Item;

        public async Task<List<Item>> GetAllItem()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Items.Where(x => x.SaleItemType != 'T' & x.DataStatus != 'D' && x.MerchantID == DataCashingAll.MerchantId).OrderBy(x => x.ItemName).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Item>> GetAllItemType()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Items.Where(x => x.DataStatus != 'D' && x.MerchantID == DataCashingAll.MerchantId).OrderBy(x => x.ItemName).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Item>> GetToppingItem()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Items.Where(x => x.SaleItemType == 'T' & x.DataStatus != 'D' && x.MerchantID == DataCashingAll.MerchantId).OrderBy(x => x.ItemName).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Item>> GetToppingOnlyNoneGroup()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Items.Where(x => x.SaleItemType == 'T' & x.DataStatus != 'D' & x.SysCategoryID == null && x.MerchantID == DataCashingAll.MerchantId).OrderBy(x => x.ItemName).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }
        //getExtraToppingSearch
        public async Task<List<Item>> getExtraToppingSearch(string searchTopping)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Items.Where(x => x.SaleItemType == 'T' && x.ItemName.Contains(searchTopping) & x.DataStatus != 'D').OrderBy(x => x.ItemName).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Item>> GetToppingItemByCategory(int sysCatID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Items.Where(x => x.SaleItemType == 'T' && (int)x.SysCategoryID == sysCatID & x.DataStatus != 'D' && x.MerchantID == DataCashingAll.MerchantId).OrderBy(x => x.ItemName).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Item>> GetToppingItemByCategoryall()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Items.Where(x => x.SaleItemType == 'T' && x.DataStatus != 'D').OrderBy(x => x.ItemName).ToListAsync();
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
                    var itemlast = db.Items
                              .OrderByDescending(x => x.SysItemID)
                              .Take(1)
                              .Select(x => x.SysItemID)
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

        public async Task<List<Item>> GetItembyCategory(int merchantId, int SysCategoryID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = new List<Item>();
                        lstitem = await db.Items.Where(x => x.MerchantID == merchantId && x.SysCategoryID == SysCategoryID & x.DataStatus != 'D' && x.MerchantID == DataCashingAll.MerchantId).OrderBy(x => x.ItemName).ToListAsync<Item>();
                     return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<Item> GetItem(int merchantId, int SysItemID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {                    
                    var result = await db.Items.Where(x => x.MerchantID == merchantId && x.SysItemID == SysItemID & x.DataStatus != 'D' && x.MerchantID == DataCashingAll.MerchantId).FirstOrDefaultAsync();                    
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<Item> GetItemforSync(int merchantId, int SysItemID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = await db.Items.Where(x => x.MerchantID == merchantId && x.SysItemID == SysItemID).FirstOrDefaultAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<Item> GetItemforSyncAndroid(int merchantId, int SysItemID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    try
                    {
                        await db.BeginTransactionAsync();
                        var items = await db.Items.Where(x => x.MerchantID == merchantId && x.SysItemID == SysItemID).FirstOrDefaultAsync();
                        if (items != null)
                        {
                            items.FWaitSending = 1;
                        }

                        var UpdateItem = await db.UpdateAsync<Item>(items);
                        await db.CommitTransactionAsync();
                        return items;
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

        public async Task<List<Item>> GetItemFwaiting()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = new List<Item>();
                    lstitem = await db.Items.Where(x => x.FWaitSending == 2 && x.MerchantID == DataCashingAll.MerchantId).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Item>> GetItemFwaiting1()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = new List<Item>();
                    lstitem = await db.Items.Where(x => x.FWaitSending == 1 && x.MerchantID == DataCashingAll.MerchantId).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<long> GetMaxItemID(long merchantId, int deviceNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);              
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    long max = 0;
                    var result = await db.Items.Where(x => x.MerchantID == merchantId).ToListAsync();
                    if (result != null)
                    {
                        max = result.Max(y => y.SysItemID);
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

        public async Task<List<Item>> GetItemStatusD(int merchantId)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Items.Where(x => x.DataStatus == 'D' && x.MerchantID == merchantId).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Item>();
            };
        }

        public async Task<List<Item>> GetAll(int merchantId)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Items.Where(x => x.MerchantID == merchantId).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Item>();
            };
        }

        public async Task<List<Item>> GetAllItemImageLoadnotComplete()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Items.Where(x => x.DataStatus != 'D' && x.MerchantID == DataCashingAll.MerchantId && !string.IsNullOrEmpty(x.PicturePath) && string.IsNullOrEmpty(x.ThumbnailLocalPath)).OrderBy(x => x.ItemName).ToListAsync();
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> CheckNameItem(string ItemName)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = await db.Items.Where(x => x.DataStatus != 'D' & x.ItemName.Contains(ItemName) && x.MerchantID == DataCashingAll.MerchantId).OrderBy(x => x.ItemName).ToListAsync();
                    //var lstitem = await db.Items.Where(x => x.DataStatus != 'D' & x.ItemCode.Trim().ToUpper().Equals(ItemName) && x.MerchantID == DataCashingAll.MerchantId).OrderBy(x => x.ItemName).ToListAsync();
                    foreach (var item in lstitem)
                    {                        
                        var result = string.Equals(ItemName, item.ItemName, StringComparison.CurrentCultureIgnoreCase);
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

        public async Task<bool> CheckItemCode(string ItemCode)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                if (!string.IsNullOrEmpty(ItemCode))
                {
                    using (var db = new MerchantDB(DataCashingAll.Pathdb))
                    {
                        var lstitem = await db.Items.Where(x => x.DataStatus != 'D' & x.ItemCode.Contains(ItemCode) && x.MerchantID == DataCashingAll.MerchantId).OrderBy(x => x.ItemName).ToListAsync();
                        //var lstitem = await db.Items.Where(x => x.DataStatus != 'D' & x.ItemCode.Trim().ToUpper().Equals(ItemCode) && x.MerchantID == DataCashingAll.MerchantId).OrderBy(x => x.ItemName).ToListAsync();
                        foreach (var item in lstitem)
                        {
                            string.Equals(ItemCode, item.ItemCode, StringComparison.CurrentCultureIgnoreCase);
                            return true;
                        }
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertItem(Item items,ItemOnBranch itemOnBranch, List<ItemExSize> lstitemExSize)
        {
            using (var db = new MerchantDB(DataCashingAll.Pathdb))
            {
                var transection = await db.BeginTransactionAsync();
                try
                {
                    CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                    //insert Item
                    var checkstatus = await db.InsertAsync<Item>(items); 
                    if (checkstatus != 1)
                    {
                        await transection.RollbackAsync();
                        return false;
                    }

                    //Insert Stock
                    if (itemOnBranch != null)
                    {
                        var checkinsertStock = await db.InsertAsync<ItemOnBranch>(itemOnBranch);
                        if (checkinsertStock != 1)
                        {
                            await transection.RollbackAsync();
                            return false;
                        } 
                    }

                    //Insert ItemExSize 
                    if (lstitemExSize != null)
                    {
                        foreach (var exSize in lstitemExSize)
                        {
                            checkstatus = await db.InsertAsync<ItemExSize>(exSize);
                            if (checkstatus == 0)
                            {
                                await transection.RollbackAsync();
                                return false;
                            }
                        }
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

        public async Task<bool> InsertOrReplaceItem(Item item)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {                   
                    var checkstatus = await db.InsertOrReplaceAsync<Item>(item);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateItem(Item items)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.Items.Where(m => m.MerchantID == items.MerchantID && m.SysItemID == items.SysItemID).FirstOrDefault();
                    if (result != null)
                    {
                        if (result.FWaitSending != 0 && result.DataStatus == 'I')
                        {
                            items.DataStatus = 'I';
                        }
                        var UpdateItem = await db.UpdateAsync<Item>(items);
                        return (UpdateItem > 0 ? true : false);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //return false;
                throw;
            };
        }

        public async Task<bool> DeleteItem(int merchantId, int sysItemId)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = await db.Items.Where(m => m.MerchantID == merchantId && m.SysItemID == sysItemId).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        var Deleteitem = await db.Items.Where(m => m.MerchantID == merchantId && m.SysItemID == sysItemId).DeleteAsync();
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

        //POS หน้า Scan Barcode Where ItemCode = XXX
        public async Task<List<Item>> GetItemPOSfScanBarcode(string ItemCode)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false );
                List<Item> listitem = new List<Item>();
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var items = await db.Items.Where(m => m.ItemCode == (string.IsNullOrEmpty(ItemCode) ? m.ItemCode.ToLower() : ItemCode.ToLower())).OrderBy(x => x.ItemName.ToLower()).FirstOrDefaultAsync();
                    if (items != null)
                    {
                        listitem = await db.Items.Where(m => m.ItemCode == (string.IsNullOrEmpty(ItemCode) ? m.ItemCode.ToLower() : ItemCode.ToLower())).OrderBy(x => x.ItemName.ToLower()).ToListAsync();
                    }
                    return listitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        //POS function Search where ItemName = XXX | ItemCode = XXX
        public async Task<List<Item>> GetItemPOSfSearch(string ItemName, string ItemCode)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                List<Item> listitem = new List<Item>();
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var items = await db.Items.Where(m => m.ItemName == ItemName | m.ItemCode == (string.IsNullOrEmpty(ItemCode) ? m.ItemCode : ItemCode)).OrderBy(x => x.ItemName).FirstOrDefaultAsync();
                    if (items != null)
                    {
                        listitem = await db.Items.Where(m => m.ItemName == ItemName | m.ItemCode == (string.IsNullOrEmpty(ItemCode) ? m.ItemCode : ItemCode)).OrderBy(x => x.ItemName).ToListAsync();
                    }
                    return listitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        //Item function Search ItemName like XXX | ItemCode = XXX
        public async Task<List<Item>> GetItemSearch(string keySearch)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                List<Item> listitem = new List<Item>();
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    if (!string.IsNullOrEmpty(keySearch)) //ItemName
                    {
                        listitem = await db.Items.Where(m => (m.ItemName.Contains(keySearch)  | m.ItemCode.Contains(keySearch)) & m.SaleItemType != 'T' & m.DataStatus != 'D').OrderBy(x => x.ItemName).ToListAsync();
                    }                    
                    return listitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        //Item function Search ItemName like XXX | ItemCode = XXX
        public async Task<List<Item>> GetToppingSearch(string keySearch)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                List<Item> listitem = new List<Item>();
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    if (!string.IsNullOrEmpty(keySearch)) //ItemName
                    {
                        listitem = await db.Items.Where(m => (m.ItemName.Contains(keySearch) | m.ItemCode.Contains(keySearch)) & m.SaleItemType == 'T' & m.DataStatus != 'D').OrderBy(x => x.ItemName).ToListAsync();
                    }                    
                    return listitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Item>> SearchItembyCategory(int merchantId, int SysCategoryID, string SearchitemName)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = new List<Item>();
                    if (SysCategoryID == 0)
                    {
                        lstitem = await db.Items.Where(x => x.MerchantID == merchantId &( x.ItemName.Contains(SearchitemName) | x.ItemCode.Contains(SearchitemName)) & x.DataStatus != 'D' & x.SaleItemType != 'T').OrderBy(x => x.ItemName).ToListAsync<Item>();
                    }
                    else if (SysCategoryID == -2)
                    {
                        lstitem = await db.Items.Where(x => x.MerchantID == merchantId &( x.ItemName.Contains(SearchitemName) | x.ItemCode.Contains(SearchitemName) )& x.DataStatus != 'D' & x.FavoriteNo > 0 ).OrderBy(x => x.ItemName).ToListAsync<Item>();
                    }
                    else if (SysCategoryID == -3)
                    {
                        lstitem = await db.Items.Where(x => x.MerchantID == merchantId & (x.ItemName.Contains(SearchitemName) | x.ItemCode.Contains(SearchitemName)) & x.DataStatus != 'D'  & x.SaleItemType == 'T').OrderBy(x => x.ItemName).ToListAsync<Item>();
                    }
                    else
                    {
                        lstitem = await db.Items.Where(x => x.MerchantID == merchantId && x.SysCategoryID == SysCategoryID &( x.ItemName.Contains(SearchitemName) | x.ItemCode.Contains(SearchitemName) )& x.DataStatus != 'D' & x.SaleItemType != 'T').OrderBy(x => x.ItemName).ToListAsync<Item>();
                    }
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Item>> SearchToppingbyCategory(int merchantId, int SysCategoryID, string SearchitemName)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = new List<Item>();
                    if (SysCategoryID == 0)
                    {
                        lstitem = await db.Items.Where(x => x.MerchantID == merchantId & (x.ItemName.Contains(SearchitemName) | x.ItemCode.Contains(SearchitemName)) & x.DataStatus != 'D' & x.SaleItemType == 'T').OrderBy(x => x.ItemName).ToListAsync<Item>();
                    }
                    else if (SysCategoryID == -2)
                    {
                        lstitem = await db.Items.Where(x => x.MerchantID == merchantId & (x.ItemName.Contains(SearchitemName) | x.ItemCode.Contains(SearchitemName)) & x.DataStatus != 'D' & x.FavoriteNo > 0 & x.SaleItemType == 'T').OrderBy(x => x.ItemName).ToListAsync<Item>();
                    }
                    else
                    {
                        lstitem = await db.Items.Where(x => x.MerchantID == merchantId && x.SysCategoryID == SysCategoryID & (x.ItemName.Contains(SearchitemName) | x.ItemCode.Contains(SearchitemName)) & x.DataStatus != 'D' & x.SaleItemType == 'T').OrderBy(x => x.ItemName).ToListAsync<Item>();
                    }
                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Item>> SearchItembyCategorynew(int merchantId, int SysCategoryID, string SearchitemName)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstitem = new List<Item>();
                    if (SysCategoryID == 0)
                    {
                        lstitem = await db.Items.Where(x => x.MerchantID == merchantId & x.ItemName.Contains(SearchitemName) | x.ItemCode.Contains(SearchitemName) & x.DataStatus != 'D').OrderBy(x => x.ItemName).ToListAsync<Item>();
                    }
                    else
                    {
                        lstitem = await db.Items.Where(x => x.MerchantID == merchantId && x.SysCategoryID == SysCategoryID & x.ItemName.Contains(SearchitemName) | x.ItemCode.Contains(SearchitemName) & x.DataStatus != 'D').OrderBy(x => x.ItemName).ToListAsync<Item>();
                    }

                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Item>> GetStockSearch(string keySearch)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                List<Item> listitem = new List<Item>();
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    if (!string.IsNullOrEmpty(keySearch)) //ItemName
                    {
                        listitem = await db.Items.Where(m => (m.ItemName.Contains(keySearch) | m.ItemCode.Contains(keySearch))  & m.FTrackStock == 1 & m.DataStatus != 'D').OrderBy(x => x.ItemName).ToListAsync();
                    }
                    return listitem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public int CountItem()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    int countItem = 0;
                    countItem =  db.Items.Where(x => x.DataStatus != 'D' && x.MerchantID == DataCashingAll.MerchantId).Count();
                    return countItem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            };
        }

    }
}
