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
    public class ItemOnBranchManage
    {
        public async Task<ItemOnBranch> GetItemOnBranch(int MerchantID, int SysBranchID, int sysItemId)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false); 
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var test = await db.ItemOnBranches.Where(x => x.MerchantID == MerchantID && x.SysBranchID == SysBranchID & x.SysItemID == sysItemId).FirstOrDefaultAsync();
                    var item = await db.ItemOnBranches.Where(x => x.MerchantID == MerchantID && x.SysBranchID == SysBranchID & x.SysItemID == sysItemId).Select(x => new ItemOnBranch()
                    {
                        BalanceStock = x.BalanceStock - (from t in db.Trans
                                                         from td in db.TranDetailItems.LeftJoin(x1 => x1.TranNo == t.TranNo)
                                                         where td.SysItemID == x.SysItemID & t.TranDate > x.LastDateBalanceStock & t.MerchantID == x.MerchantID & t.SysBranchID == x.SysBranchID & t.FCancel == 0
                                                         select td).Sum(x1 => x1.Quantity),

                        LastDateBalanceStock = x.LastDateBalanceStock,
                        MerchantID = x.MerchantID,
                        MinimumStock = x.MinimumStock,
                        SysBranchID = x.SysBranchID,
                        SysItemID = x.SysItemID,
                    }).FirstOrDefaultAsync();

                    return item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }
        public async Task<List<Gabana.Model.ItemMovement>> GetItemMovement(int MerchantID, int SysBranchID, int sysItemId)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {

                    var itemnew = await (from t in db.Trans
                                         from td in db.TranDetailItems.LeftJoin(x1 => x1.TranNo == t.TranNo)
                                         from iob in db.ItemOnBranches.LeftJoin(x2 => x2.SysItemID == td.SysItemID)
                                         where t.TranDate > iob.LastDateBalanceStock & t.FCancel == 0 & t.MerchantID == MerchantID & t.SysBranchID == SysBranchID & td.SysItemID == sysItemId
                                         select new Gabana.Model.ItemMovement() { SysItemID = (int)td.SysItemID , Quantity = td.Quantity 
                                         , MerchantID = (int)t.MerchantID , MovementDate = t.TranDate , MovementType = 'S' ,RefTranNo = t.TranNo ,
                                             SysBranchID = (int)t.SysBranchID , UserName = t.SellerName }).ToListAsync();
                    return itemnew;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<ItemOnBranch>> GetAllItemOnBranch(int MerchantID, int SysBranchID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var CalItemOnBranch = new List<ItemOnBranch>();
                    var x11 = await db.ItemOnBranches.Where(x => x.MerchantID == MerchantID & x.SysBranchID == SysBranchID).ToListAsync();
                    
                    CalItemOnBranch = await db.ItemOnBranches.Where(x => x.MerchantID == MerchantID & x.SysBranchID == SysBranchID).Select(x => new ItemOnBranch()
                    {
                        BalanceStock = x.BalanceStock - ((from t in db.Trans
                                                         from td in db.TranDetailItems.LeftJoin(x1 => x1.TranNo == t.TranNo)
                                                         where td.SysItemID == x.SysItemID & t.TranDate > x.LastDateBalanceStock & t.MerchantID == x.MerchantID & t.SysBranchID == x.SysBranchID & t.FCancel == 0
                                                          select td).Sum(x1 => x1.Quantity) + (from t in db.Trans
                                                                                               from td in db.TranDetailItemToppings.LeftJoin(x1 => x1.TranNo == t.TranNo)
                                                                                               where td.SysItemID == x.SysItemID & t.TranDate > x.LastDateBalanceStock & t.MerchantID == x.MerchantID & t.SysBranchID == x.SysBranchID & t.FCancel == 0
                                                                                               select td).Sum(x1 => x1.Quantity)) ,

                        LastDateBalanceStock = x.LastDateBalanceStock,
                        MerchantID = x.MerchantID,
                        MinimumStock = x.MinimumStock,
                        SysBranchID = x.SysBranchID,
                        SysItemID = x.SysItemID,
                    }).ToListAsync();
                    //var a1 = await db.ItemOnBranches.Where(x => x.MerchantID == MerchantID & x.SysBranchID == SysBranchID & x.SysItemID == 73000035).Select(x => new ItemOnBranch()
                    //{
                    //    BalanceStock = (from t in db.Trans
                    //                                      from td in db.TranDetailItems.LeftJoin(x1 => x1.TranNo == t.TranNo)
                    //                                      where td.SysItemID == 73000035 & t.TranDate > x.LastDateBalanceStock & t.MerchantID == x.MerchantID & t.SysBranchID == x.SysBranchID & t.FCancel == 0
                    //                                      select td).Sum(x1 => x1.Quantity),

                    //    LastDateBalanceStock = x.LastDateBalanceStock,
                    //    MerchantID = x.MerchantID,
                    //    MinimumStock = x.MinimumStock,
                    //    SysBranchID = x.SysBranchID,
                    //    SysItemID = x.SysItemID,
                    //}).ToListAsync();

                    //var a2 = await db.ItemOnBranches.Where(x => x.MerchantID == MerchantID & x.SysBranchID == SysBranchID & x.SysItemID == 73000035).Select(x => new ItemOnBranch()
                    //{
                    //    BalanceStock = 
                    //                                    (from t in db.Trans
                    //                                     from td in db.TranDetailItems.LeftJoin(x1 => x1.TranNo == t.TranNo)
                    //                                     where td.SysItemID == 73000035 & t.TranDate > x.LastDateBalanceStock & t.MerchantID == x.MerchantID & t.SysBranchID == x.SysBranchID & t.FCancel == 1
                    //                                     select td).Sum(x1 => x1.Quantity),

                    //    LastDateBalanceStock = x.LastDateBalanceStock,
                    //    MerchantID = x.MerchantID,
                    //    MinimumStock = x.MinimumStock,
                    //    SysBranchID = x.SysBranchID,
                    //    SysItemID = x.SysItemID,
                    //}).ToListAsync();


                    return CalItemOnBranch;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<ItemOnBranch>> GetAllItemOnBranchOnlyMerchantID(int MerchantID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var CalItemOnBranch = new List<ItemOnBranch>();
                    var x11 = await db.ItemOnBranches.Where(x => x.MerchantID == MerchantID).ToListAsync();

                    CalItemOnBranch = await db.ItemOnBranches.Where(x => x.MerchantID == MerchantID).Select(x => new ItemOnBranch()
                    {
                        BalanceStock = x.BalanceStock - ((from t in db.Trans
                                                          from td in db.TranDetailItems.LeftJoin(x1 => x1.TranNo == t.TranNo)
                                                          where td.SysItemID == x.SysItemID & t.TranDate > x.LastDateBalanceStock & t.MerchantID == x.MerchantID & t.SysBranchID == x.SysBranchID & t.FCancel == 0
                                                          select td).Sum(x1 => x1.Quantity) + (from t in db.Trans
                                                                                               from td in db.TranDetailItemToppings.LeftJoin(x1 => x1.TranNo == t.TranNo)
                                                                                               where td.SysItemID == x.SysItemID & t.TranDate > x.LastDateBalanceStock & t.MerchantID == x.MerchantID & t.SysBranchID == x.SysBranchID & t.FCancel == 0
                                                                                               select td).Sum(x1 => x1.Quantity)),

                        LastDateBalanceStock = x.LastDateBalanceStock,
                        MerchantID = x.MerchantID,
                        MinimumStock = x.MinimumStock,
                        SysBranchID = x.SysBranchID,
                        SysItemID = x.SysItemID,
                    }).ToListAsync();
                    //var a1 = await db.ItemOnBranches.Where(x => x.MerchantID == MerchantID & x.SysBranchID == SysBranchID & x.SysItemID == 73000035).Select(x => new ItemOnBranch()
                    //{
                    //    BalanceStock = (from t in db.Trans
                    //                                      from td in db.TranDetailItems.LeftJoin(x1 => x1.TranNo == t.TranNo)
                    //                                      where td.SysItemID == 73000035 & t.TranDate > x.LastDateBalanceStock & t.MerchantID == x.MerchantID & t.SysBranchID == x.SysBranchID & t.FCancel == 0
                    //                                      select td).Sum(x1 => x1.Quantity),

                    //    LastDateBalanceStock = x.LastDateBalanceStock,
                    //    MerchantID = x.MerchantID,
                    //    MinimumStock = x.MinimumStock,
                    //    SysBranchID = x.SysBranchID,
                    //    SysItemID = x.SysItemID,
                    //}).ToListAsync();

                    //var a2 = await db.ItemOnBranches.Where(x => x.MerchantID == MerchantID & x.SysBranchID == SysBranchID & x.SysItemID == 73000035).Select(x => new ItemOnBranch()
                    //{
                    //    BalanceStock = 
                    //                                    (from t in db.Trans
                    //                                     from td in db.TranDetailItems.LeftJoin(x1 => x1.TranNo == t.TranNo)
                    //                                     where td.SysItemID == 73000035 & t.TranDate > x.LastDateBalanceStock & t.MerchantID == x.MerchantID & t.SysBranchID == x.SysBranchID & t.FCancel == 1
                    //                                     select td).Sum(x1 => x1.Quantity),

                    //    LastDateBalanceStock = x.LastDateBalanceStock,
                    //    MerchantID = x.MerchantID,
                    //    MinimumStock = x.MinimumStock,
                    //    SysBranchID = x.SysBranchID,
                    //    SysItemID = x.SysItemID,
                    //}).ToListAsync();


                    return CalItemOnBranch;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertItemOnBranch(ItemOnBranch itmonBranch)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertAsync<ItemOnBranch>(itmonBranch);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertorReplaceItemOnBranch(ItemOnBranch itmonBranch)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertOrReplaceAsync<ItemOnBranch>(itmonBranch);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateItemOnBranch(ItemOnBranch itmonBranch)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.ItemOnBranches.Where(a => a.MerchantID == itmonBranch.MerchantID && a.SysBranchID == itmonBranch.SysBranchID && a.SysItemID == itmonBranch.SysItemID).FirstOrDefault();
                    if (result != null)
                    {
                        var UpdateItem = await db.UpdateAsync<ItemOnBranch>(itmonBranch);
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

        public async Task<bool> DeleteItemOnBranch(int MerchantID, int SysBranchID, int SysItemID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.ItemOnBranches.Where(m => m.MerchantID == MerchantID && m.SysBranchID == SysBranchID && m.SysItemID == SysItemID).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteItem = await db.ItemOnBranches.Where(m => m.MerchantID == MerchantID && m.SysBranchID == SysBranchID && m.SysItemID == SysItemID).DeleteAsync();
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
