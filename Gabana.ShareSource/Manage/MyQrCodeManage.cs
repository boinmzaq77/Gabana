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
    public  class MyQrCodeManage
    {       
        public async Task<List<MyQrCode>> GetAllMyQrCode(int MerchantID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstMyQrCode = new List<MyQrCode>();
                    lstMyQrCode = await db.MyQrCodes.Where(x=>x.MerchantID == MerchantID).ToListAsync<MyQrCode>();                    
                    return lstMyQrCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;                
            };
        }
        
        public async Task<int> GetMyQrCodeNo(int MerchantID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    int value = 0;
                    value = await db.MyQrCodes.Where(x => x.MerchantID == MerchantID).MaxAsync(y => (int?)y.MyQrCodeNo) ?? 0;
                    return value + 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            };
        }

        public async Task<MyQrCode> GetMyQrCode(int MerchantID, long MyQrCodeNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var MyQrCode = new MyQrCode();
                    MyQrCode = await db.MyQrCodes.Where(x => x.MerchantID == MerchantID && x.MyQrCodeNo == MyQrCodeNo).FirstOrDefaultAsync();
                    return MyQrCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertMyQrCode(MyQrCode qrCode)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertAsync<MyQrCode>(qrCode);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertOrReplaceMyQrCode(MyQrCode qrCode)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertOrReplaceAsync<MyQrCode>(qrCode);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateMyQrCode(MyQrCode qrCode)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.MyQrCodes.Where(a => a.MerchantID == qrCode.MerchantID && a.MyQrCodeNo == qrCode.MyQrCodeNo).FirstOrDefault();
                    if (result != null)
                    {                        
                        var UpdateItem = await db.UpdateAsync<MyQrCode>(qrCode);
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

        public async Task<bool> DeleteMyQrCode(int MerchantID, long MyQrCodeNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.MyQrCodes.Where(m => m.MerchantID == MerchantID && m.MyQrCodeNo == MyQrCodeNo).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteItem = await db.MyQrCodes.Where(m => m.MerchantID == MerchantID && m.MyQrCodeNo == MyQrCodeNo).DeleteAsync();
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

        public async Task<bool> DeleteMyQrCodefromBranch(int MerchantID, int SysBranchID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.MyQrCodes.Where(m => m.MerchantID == MerchantID && m.SysBranchID == SysBranchID).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteItem = await db.MyQrCodes.Where(m => m.MerchantID == MerchantID && m.SysBranchID == SysBranchID).DeleteAsync();
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

        public async Task<bool> DeleteAllMyQrCode(int MerchantID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.MyQrCodes.Where(m => m.MerchantID == MerchantID).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteItem = await db.MyQrCodes.Where(m => m.MerchantID == MerchantID).DeleteAsync();
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
