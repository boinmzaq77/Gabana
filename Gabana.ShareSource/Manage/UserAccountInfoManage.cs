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
    public  class UserAccountInfoManage
    {
        public async Task<UserAccountInfo> GetUserAccount(int merchantId,string username)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    
                    var result = await db.UserAccountInfo.Where(x => x.MerchantID == merchantId && x.UserName.ToLower() == username.ToLower()).FirstOrDefaultAsync();                    
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<UserAccountInfo>> GetAllUserAccount()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstuser = new List<UserAccountInfo>();
                    lstuser = await db.UserAccountInfo.Where(x => x.MerchantID == DataCashingAll.MerchantId).ToListAsync();
                    if (lstuser.Count > 0)
                    {
                        lstuser = lstuser.OrderBy(x => x.UserName).ToList();
                    }
                    return lstuser;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);              
                //return null;                
                var lstuser = new List<UserAccountInfo>();
                var error = new UserAccountInfo {MerchantID = 0, Comments = ex.Message };
                lstuser.Add(error);
                return lstuser;
            };
        }

        public async Task<List<UserAccountInfo>> GetEmployeeSearch(int merchant, string EmployeeName)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var EmployeeList = await db.UserAccountInfo.Where(x => x.MerchantID == merchant & x.UserName.Contains(EmployeeName)).OrderBy(x => x.UserName).ToListAsync();
                    return EmployeeList;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertUserAccount(UserAccountInfo user)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertAsync<UserAccountInfo>(user);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertorReplaceUserAccount(UserAccountInfo user)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertOrReplaceAsync<UserAccountInfo>(user);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateUserAccount(UserAccountInfo  user)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.UserAccountInfo.Where(m => m.MerchantID == user.MerchantID && m.UserName.ToLower() == user.UserName.ToLower()).FirstOrDefault();
                    if (result != null)
                    {
                        var UpdateTran = await db.UpdateAsync<UserAccountInfo>(user);
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

        public async Task<bool> DeleteUserAccount(int merchantId,string username)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.UserAccountInfo.Where(m => m.MerchantID == merchantId && m.UserName.ToLower()  == username.ToLower()).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteUser = await db.UserAccountInfo.Where(m => m.MerchantID == merchantId && m.UserName.ToLower() == username.ToLower()).DeleteAsync();
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
