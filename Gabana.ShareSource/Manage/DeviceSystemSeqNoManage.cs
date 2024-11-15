using Gabana.ORM.MerchantDB;
using Gabana3.JAM.Merchant;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Manage
{
    public class DeviceSystemSeqNoManage
    {
        public async Task<int> GetDeviceSystemSeqNo(int MerchantID,int DeviceNo,int SystemID)
        {    
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            using (var db = new MerchantDB(DataCashingAll.Pathdb))
            {
                try
                {
                    var LastSysSeqNo = await db.DeviceSystemSeqNoes.Where(x => x.MerchantID == MerchantID && x.DeviceNo == DeviceNo && x.SystemID == SystemID)
                                                                                .OrderByDescending(x => x.LastSysSeqNo)
                                                                                .Select(x =>  x.LastSysSeqNo)
                                                                                .FirstOrDefaultAsync();
                    return (int)LastSysSeqNo;
                }
                catch (Exception)
                {
                    List<ORM.Master.DeviceSystemSeqNo> systemSeqNos = new List<ORM.Master.DeviceSystemSeqNo>();
                    systemSeqNos = await GabanaAPI.GetDataDeviceSeqNo(DataCashingAll.DeviceNo);
                    foreach (var item in systemSeqNos)
                    {
                        ORM.MerchantDB.DeviceSystemSeqNo deviceSystemSeqNo = new ORM.MerchantDB.DeviceSystemSeqNo()
                        {
                            MerchantID = item.MerchantID,
                            SystemID = item.SystemID,
                            DeviceNo = item.DeviceNo,
                            LastSysSeqNo = item.LastSysSeqNo
                        };
                        await InsertOrReplaceDeviceSystemSeqNo(deviceSystemSeqNo);
                    }

                    var LastSysSeqNo = await db.DeviceSystemSeqNoes.Where(x => x.MerchantID == MerchantID && x.DeviceNo == DeviceNo && x.SystemID == SystemID)
                                                                                .OrderByDescending(x => x.LastSysSeqNo)
                                                                                .Select(x => x.LastSysSeqNo)
                                                                                .FirstOrDefaultAsync();
                    return (int)LastSysSeqNo;
                };
            }
            
            
        }

        public async Task<List<DeviceSystemSeqNo>> GetDeviceSystemSeqNoAll(int MerchantID, int DeviceNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    List<DeviceSystemSeqNo> seqNos = new List<DeviceSystemSeqNo>();
                    seqNos = await db.DeviceSystemSeqNoes.Where(x => x.MerchantID == MerchantID && x.DeviceNo == DeviceNo).ToListAsync();
                    return seqNos;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<DeviceSystemSeqNo>();
            };
        }

        public async Task<int> GetLastDeviceSystemSeqNo(int MerchantID, int DeviceNo, int SystemID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var LastSysSeqNo = await db.DeviceSystemSeqNoes.Where(x => x.MerchantID == MerchantID && x.DeviceNo == DeviceNo && x.SystemID == SystemID).ToListAsync();
                    if(LastSysSeqNo.Count==0)
                    {
                        return 0;
                    }
                    else
                    {
                        var SeqNo = LastSysSeqNo.Max(x => x.LastSysSeqNo);
                        return (int)SeqNo;
                    }

                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            };
        }

        public async Task<bool> InsertDeviceSystemSeqNo(DeviceSystemSeqNo DeviceSystemSeqNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertAsync<DeviceSystemSeqNo>(DeviceSystemSeqNo);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                //no such table main.device
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateDeviceSystemSeqNo(DeviceSystemSeqNo DeviceSystemSeqNo, MerchantDB db)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                var result = db.DeviceSystemSeqNoes.Where(a => a.MerchantID == DeviceSystemSeqNo.MerchantID && a.DeviceNo == DeviceSystemSeqNo.DeviceNo && a.SystemID == DeviceSystemSeqNo.SystemID).FirstOrDefault();
                if (result != null)
                {
                    var UpdateDevice = await db.InsertOrReplaceAsync<DeviceSystemSeqNo>(DeviceSystemSeqNo);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
                //Console.WriteLine(ex.Message);
                //return false;
            };
        }

        public async Task<bool> InsertOrReplaceDeviceSystemSeqNo(DeviceSystemSeqNo DeviceSystemSeqNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                   var UpdateDevice = await db.InsertOrReplaceAsync<DeviceSystemSeqNo>(DeviceSystemSeqNo);
                }
                return true;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> DeleteDeviceSystemSeqNo(int MerchantID, int DeviceNo, int SystemID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.DeviceSystemSeqNoes.Where(m => m.MerchantID == MerchantID && m.DeviceNo == DeviceNo && m.SystemID == SystemID).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteDevice = await db.DeviceSystemSeqNoes.Where(m => m.MerchantID == MerchantID && m.DeviceNo == DeviceNo && m.SystemID == SystemID).DeleteAsync();
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
