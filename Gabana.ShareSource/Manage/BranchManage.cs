using Gabana.Model;
using Gabana.ORM;
using Gabana.ORM.MerchantDB;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Branch = Gabana.ORM.MerchantDB.Branch;

namespace Gabana.ShareSource.Manage
{
     public class BranchManage
    {
        public async Task<List<Branch>> GetAllBranch(int merchantID)
        {            
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstbranch = new List<Branch>();                    
                    var lstsortbranch = new List<Branch>();                    
                    lstbranch = await db.Branches.Where(x=>x.MerchantID == merchantID).ToListAsync<Branch>();
                    lstsortbranch = lstbranch.Where(x=>x.Status == 'A').OrderBy(x => x.SysBranchID).ToList();                    
                    return lstsortbranch;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<Branch> GetBranch(int merchantID,int SysBranchID)
        {           
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {                   
                    var result = await db.Branches.Where(x => x.MerchantID == merchantID & x.SysBranchID == SysBranchID).FirstOrDefaultAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        //public async Task<List<UserinBranch>> GetBranchByUserName(int merchantID, string username)
        //{
        //    try
        //    {
        //        using (var db = new MerchantDB(DataCashingAll.Pathdb))
        //        {
        //            List<UserinBranch> lstbranches= await db.Branches.Where(x => x.MerchantID == merchantID)
        //                                                                .Join(db.UserAccountInfo, m => m.MerchantID, x => x.MerchantID, (m, x) => new { m, x })
        //                                                                .Select(x => new UserinBranch()
        //                                                                {
        //                                                                    branchname = x.m.BranchName,
        //                                                                    sysbranch =x.m.SysBranchID,
        //                                                                    username = x.x.UserName
        //                                                                })
        //                                                                .OrderByDescending(x => x.sysbranch)
        //                                                                .ThenByDescending(x => x.sysbranch).ToListAsync();
        //            return lstbranches;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return null;
        //    };
        //}

        public async Task<List<Branch>> GetBranchFwaiting()
        {
            try
            {
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstbranch = new List<Branch>();
                    lstbranch = await db.Branches.ToListAsync<Branch>();                    
                    return lstbranch;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }


#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<int> GetLastBranch()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var Branchlast = db.Branches
                              .OrderByDescending(x => x.SysBranchID)
                              .Take(1)
                              .Select(x => x.SysBranchID)
                              .ToList()
                              .FirstOrDefault();
                    return Convert.ToInt32(Branchlast);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            };
        }

        public async Task<List<Branch>> GetBranchSearch(int merchant, string branch)
        {           
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var BranchList = await db.Branches.Where(x => x.MerchantID == merchant & x.BranchName.Contains(branch) & x.Status == 'A').OrderBy(x => x.BranchName).ToListAsync();
                    return BranchList;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertBranch(Branch branch)
        {            
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {

                    var checkstatus = await db.InsertAsync<Branch>(branch);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertorReplacrBranch(Branch branch)
        {            
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {

                    var checkstatus = await db.InsertOrReplaceAsync<Branch>(branch);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateBranch(Branch branch)
        {           
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.Branches.Where(m => m.BranchID == branch.BranchID && m.MerchantID == branch.MerchantID).FirstOrDefault();
                    if (result != null)
                    {
                        #region UpdateBranch
                        //var UpdateBranch = db.Branches.Where(x => x.BranchID == branch.BranchID && x.MerchantID == branch.MerchantID)                             
                        //     .Set(c => c.Ordinary, branch.Ordinary ?? branch.Ordinary)
                        //     .Set(c => c.BranchName , branch.BranchName ?? branch.BranchName)
                        //     .Set(c => c.Address , branch.Address ?? branch.Address)
                        //     .Set(c => c.ProvincesId , branch.ProvincesId ?? branch.ProvincesId)
                        //     .Set(c => c.AmphuresId, branch.AmphuresId ?? branch.AmphuresId)
                        //     .Set(c => c.DistrictsId,branch.DistrictsId ?? branch.DistrictsId)
                        //     .Set(c => c.DisplayLanguage.ToString(), branch.DisplayLanguage.ToString() ?? branch.DisplayLanguage.ToString())
                        //     .Set(c => c.Lat,branch.Lat ?? branch.Lat)
                        //     .Set(c => c.Lng, branch.Lng ?? branch.Lng)
                        //     .Set(c => c.Email, branch.Email ?? branch.Email)
                        //     .Set(c => c.Tel, branch.Tel ?? branch.Tel)
                        //     .Set(c => c.Line, branch.Line ?? branch.Line)
                        //     .Set(c => c.Facebook, branch.Facebook ?? branch.Facebook)
                        //     .Set(c => c.Instagram, branch.Instagram ?? branch.Instagram)
                        //     .Set(c => c.TaxBranchName , branch.TaxBranchName ?? branch.TaxBranchName)
                        //     .Set(c => c.TaxBranchID, branch.TaxBranchID ?? branch.TaxBranchID)
                        //     .Set(c => c.TaxID, branch.TaxID ?? branch.TaxID)
                        //     .Set(c => c.RegMark, branch.RegMark ?? branch.RegMark)
                        //     .Set(c => c.LinkProMaxxID, branch.LinkProMaxxID ?? branch.LinkProMaxxID)
                        //     .Set(c => c.Comments , branch.Comments ?? branch.Comments)
                        //     .UpdateAsync(); 
                        #endregion
                        var UpdateBranch = await db.UpdateAsync<Branch>(branch);
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

        public async Task<bool> DeleteBranch(int merchantID, int sysBranchID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    await db.BeginTransactionAsync();

                    var deleteBranchPolicies = await db.BranchPolicies.Where(x => x.MerchantID == merchantID && x.SysBranchID == sysBranchID).DeleteAsync();
                    if (deleteBranchPolicies < 0)
                    {
                        await db.RollbackTransactionAsync();
                    }
                    var deleteQRCode = await db.MyQrCodes.Where(x => x.MerchantID == merchantID & x.SysBranchID == sysBranchID).DeleteAsync();
                    if (deleteQRCode < 0)
                    {
                        await db.RollbackTransactionAsync();
                    }
                    var deleteBranches = await db.Branches.Where(x => x.MerchantID == merchantID & x.SysBranchID == sysBranchID)
                                                .Set(x => x.Status, 'D')
                                                .UpdateAsync();
                    if (deleteBranches != 1)
                    {
                        await db.RollbackTransactionAsync();
                    }

                    await db.CommitTransactionAsync();
                    return true;
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
