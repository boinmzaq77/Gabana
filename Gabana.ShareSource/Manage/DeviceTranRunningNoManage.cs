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
    public  class DeviceTranRunningNoManage
    {
        public async Task<int> GetDeviceTranRunningNo(int MerchantID,int SysBranchID, int DeviceNo)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            try
            {
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var LastSysSeqNo = await db.DeviceTranRunningNoes.Where(x => x.MerchantID == MerchantID && x.DeviceNo == DeviceNo && x.SysBranchID == SysBranchID)
                                                                    .OrderByDescending(x => x.TranLastRunningNo)
                                                                    .Select(x => x.TranLastRunningNo)
                                                                    .FirstOrDefaultAsync();
                    return (int)LastSysSeqNo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            };
        }

        public async Task<List<DeviceTranRunningNo>> GetAllDeviceTranRunningNo(int MerchantID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var LastSysSeqNo = await db.DeviceTranRunningNoes.Where(x => x.MerchantID == MerchantID).ToListAsync();
                    return LastSysSeqNo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }


        public async Task<int> GetLastDeviceTranRunningNo(int MerchantID, int SysBranchID, int DeviceNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    //var TranLastRunningNo = db.DeviceTranRunningNoes.Where(x => x.MerchantID == MerchantID && x.DeviceNo == DeviceNo && x.SysBranchID == SysBranchID).Max(x=>x.TranLastRunningNo);
                    var TranLastRunningNo = await db.DeviceTranRunningNoes.Where(x => x.MerchantID == MerchantID && x.DeviceNo == DeviceNo && x.SysBranchID == SysBranchID).FirstOrDefaultAsync();                    
                    return (int)TranLastRunningNo.TranLastRunningNo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            };
        }

        public async Task<bool> InsertDeviceTranRunningNo(DeviceTranRunningNo deviceTranRunning)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var transection = await db.BeginTransactionAsync();
                    var checkstatus = await db.InsertAsync<DeviceTranRunningNo>(deviceTranRunning);
                    if (checkstatus != 1)
                    {
                        await transection.RollbackAsync();
                        return false;
                    }

                    await transection.CommitAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                //no such table main.device
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateDeviceTranRunningNo(DeviceTranRunningNo deviceTranRunning,MerchantDB db)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                var result2 = db.DeviceTranRunningNoes.ToList(); 
                var result = db.DeviceTranRunningNoes.Where(a => a.MerchantID == deviceTranRunning.MerchantID && a.DeviceNo == deviceTranRunning.DeviceNo && a.SysBranchID == deviceTranRunning.SysBranchID).FirstOrDefault();
                if (result != null)
                {
                    var UpdateDevice = await db.InsertOrReplaceAsync<DeviceTranRunningNo>(deviceTranRunning);
                    return true;
                }
                return false;            
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }

        public async Task<bool> InsertOrReplaceDeviceTranRunningNo(DeviceTranRunningNo deviceTranRunning)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var UpdateDevice = await db.InsertOrReplaceAsync<DeviceTranRunningNo>(deviceTranRunning);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }

        public async Task<bool> DeleteDeviceTranRunningNo(int MerchantID, int SysBranchID, int DeviceNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.DeviceTranRunningNoes.Where(m => m.MerchantID == MerchantID && m.DeviceNo == DeviceNo && m.SysBranchID == SysBranchID).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteDevice = await db.DeviceTranRunningNoes.Where(m => m.MerchantID == MerchantID && m.DeviceNo == DeviceNo && m.SysBranchID == SysBranchID).DeleteAsync();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }
    }
}
