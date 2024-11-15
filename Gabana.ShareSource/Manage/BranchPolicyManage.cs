using Gabana.ORM;
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
    public class BranchPolicyManage
    {
        public async Task<List<BranchPolicy>> GetAllBranchPolicy(int merchantID)
        {            
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstbranch = new List<BranchPolicy>();
                    var result = await db.BranchPolicies.Where(x => x.MerchantID == merchantID).FirstOrDefaultAsync();

                    if (result != null)
                    {
                        lstbranch = await db.BranchPolicies.ToListAsync<BranchPolicy>();
                    }
                    return lstbranch;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<BranchPolicy>> GetlstBranchPolicy(int merchantID,  string username)
        {            
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = await db.BranchPolicies.Where(x => x.MerchantID == merchantID  & x.UserName.ToLower() == username.ToLower()).ToListAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertBranchPolicy(BranchPolicy branchPolicy)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertAsync<BranchPolicy>(branchPolicy);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertorReplacrBranchPolicy(BranchPolicy branchPolicy)
        {            
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var getdata = await db.BranchPolicies.Where(x => x.MerchantID == branchPolicy.MerchantID && x.UserName.ToLower() == branchPolicy.UserName.ToLower() && x.SysBranchID == branchPolicy.SysBranchID).ToListAsync();
                    //var getdata = await db.BranchPolicies.Where(x => x.MerchantID == branchPolicy.MerchantID & x.UserName.ToLower() == branchPolicy.UserName.ToLower()).ToListAsync();
                     if (getdata.Count == 0)
                    {
                        var checkstatus = await db.InsertAsync<BranchPolicy>(branchPolicy);
                        return (checkstatus > 0 ? true : false);
                    }
                    else
                    {
                        var checkstatus = await db.UpdateAsync<BranchPolicy>(branchPolicy);
                        return (checkstatus > 0 ? true : false);
                    }

                    //var checkstatus = await db.InsertOrReplaceAsync<BranchPolicy>(branchPolicy);
                    //return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateBranchPolicy(BranchPolicy branchPolicy)
        {            
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.BranchPolicies.Where(m => m.SysBranchID == branchPolicy.SysBranchID && m.MerchantID == branchPolicy.MerchantID & m.UserName == branchPolicy.UserName).FirstOrDefault();
                    if (result != null)
                    {
                        var checkstatus = await db.UpdateAsync<BranchPolicy>(branchPolicy);
                        return (checkstatus > 0 ? true : false);
                    }
                    else
                    {
                        var checkstatus = await db.InsertAsync<BranchPolicy>(branchPolicy);
                        return (checkstatus > 0 ? true : false);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> DeleteBranch(int merchantID, string username)
        {            
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var DeleteBranch = await db.BranchPolicies.Where(c => c.MerchantID == merchantID & c.UserName.ToLower() == username.ToLower()).DeleteAsync();
                    return (DeleteBranch > 0 ? true : false);
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
