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
    public class MemberTypeManage
    {
        public async Task<List<MemberType>> GetAllMemberType(int merchantID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lsttype = new List<MemberType>();
                    var result = await db.MemberTypes.Where(x => x.MerchantID == merchantID).FirstOrDefaultAsync();

                    if (result != null)
                    {
                        lsttype = await db.MemberTypes.ToListAsync<MemberType>();
                    }
                    return lsttype;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<MemberType> GetMemberType(int merchantID, int MemberTypeNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = await db.MemberTypes.Where(x => x.MerchantID == merchantID & x.MemberTypeNo == MemberTypeNo).FirstOrDefaultAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertMemberType(MemberType member)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertAsync<MemberType>(member);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertorReplaceListMemberType(List<MemberType> lstmember)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                int checkstatus = 0;
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    foreach (var item in lstmember)
                    {
                        checkstatus = await db.InsertOrReplaceAsync<MemberType>(item);
                    }
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertorReplacrMemberType(MemberType member)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    int check = 0;
                    //var checkstatus = await db.InsertOrReplaceAsync<MemberType>(member);
                    //return (checkstatus > 0 ? true : false);
                    var count = await db.MemberTypes.Where(x => x.MerchantID == member.MerchantID & x.MemberTypeNo == member.MemberTypeNo).CountAsync();

                    if (count > 0)
                    {
                        check = await db.MemberTypes.Where(x => x.MerchantID == member.MerchantID & x.MemberTypeNo == member.MemberTypeNo)
                                .Set(x => x.LinkProMaxxID, member.LinkProMaxxID)
                                .Set(x => x.MemberTypeName, member.MemberTypeName)
                                .Set(x => x.PercentDiscount, member.PercentDiscount)
                                .Set(x => x.DateModified, UtilsAll.TranDateformat(DateTime.UtcNow))
                                .UpdateAsync();
                        if (check != 1)
                        {
                            return (check > 0 ? true : false);
                        }
                    }
                    else
                    {
                        check = await db.InsertAsync<MemberType>(member);
                        if (check != 1)
                        {
                            return (check > 0 ? true : false);
                        }
                    }
                    return (check > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateMemberType(MemberType member)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.MemberTypes.Where(m => m.MerchantID == member.MerchantID && m.MemberTypeNo == member.MemberTypeNo).FirstOrDefault();
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
                        var UpdateMember = await db.UpdateAsync<MemberType>(member);
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

        public async Task<bool> DeleteMemberType(int merchantID, int MemberTypeNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.MemberTypes.Where(m => m.MerchantID == merchantID && m.MemberTypeNo == MemberTypeNo).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteMemberType = await db.MemberTypes.Where(c => c.MerchantID == merchantID && c.MemberTypeNo == MemberTypeNo).DeleteAsync();
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

        public async Task<bool> DeleteAllMemberType(int merchantID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.MemberTypes.Where(m => m.MerchantID == merchantID).ToList();
                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            await db.BeginTransactionAsync();
                            var Customers = await db.Customers.Where(x => x.MerchantID == merchantID & x.MemberTypeNo == item.MemberTypeNo).CountAsync();
                            if (Customers > 0)
                            {
                                var Update = await db.Customers.Where(x => x.MerchantID == merchantID & x.MemberTypeNo != item.MemberTypeNo).Set(x => x.MemberTypeNo, x => null).UpdateAsync();
                                if (Update < 0)
                                {
                                    await db.RollbackTransactionAsync();
                                    return false;
                                }
                            }
                            var DeleteMemberType = await db.MemberTypes.Where(c => c.MerchantID == merchantID && c.MemberTypeNo == item.MemberTypeNo).DeleteAsync();
                            if (DeleteMemberType < 1)
                            {
                                await db.RollbackTransactionAsync();
                                return false;
                            }
                            else
                            {
                                await db.CommitTransactionAsync();
                                return true;
                            }
                        }
                        return true;
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

    }
}
