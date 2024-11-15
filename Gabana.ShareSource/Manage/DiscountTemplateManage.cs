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
    public  class DiscountTemplateManage
    {
        DeviceSystemSeqNoManage DeviceSeqManage = new DeviceSystemSeqNoManage();
        int systemId = 40;

        public async Task<List<DiscountTemplate>> GetAllDiscountTemplate()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstDiscountTemplate = new List<DiscountTemplate>();
                    lstDiscountTemplate = await db.DiscountTemplates.ToListAsync<DiscountTemplate>();                    
                    return lstDiscountTemplate;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<DiscountTemplate>> SearchDiscountTemplate(string discountName)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                List<DiscountTemplate> listdiscount = new List<DiscountTemplate>();
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    if (!string.IsNullOrEmpty(discountName)) //DiscountName
                    {
                        listdiscount = await db.DiscountTemplates.Where(m => m.TemplateName.Contains(discountName)).ToListAsync();
                    }
                    return listdiscount;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<DiscountTemplate> GetDiscountTemplate(int MerchantID, int SysDiscountTemplate)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var DiscountTemplate = new DiscountTemplate();
                    var Config = await db.DiscountTemplates.Where(x => x.MerchantID == MerchantID && x.SysDiscountTemplate == SysDiscountTemplate).FirstOrDefaultAsync();
                    if (Config != null)
                    {
                        DiscountTemplate = await db.DiscountTemplates.Where(x => x.MerchantID == MerchantID && x.SysDiscountTemplate == SysDiscountTemplate).FirstOrDefaultAsync<DiscountTemplate>();
                    }
                    return DiscountTemplate;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertDiscountTemplate(DiscountTemplate  discountTemplate)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var transection = await db.BeginTransactionAsync();
                    var checkstatus = await db.InsertAsync<DiscountTemplate>(discountTemplate);
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
                        await db.RollbackTransactionAsync();
                        return false;
                    }

                    await transection.CommitAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertOrReplaceDiscountTemplate(DiscountTemplate discountTemplate)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {

                    var checkstatus = await db.InsertOrReplaceAsync<DiscountTemplate>(discountTemplate);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateDiscountTemplate(DiscountTemplate  discountTemplate)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.DiscountTemplates.Where(a => a.MerchantID == discountTemplate.MerchantID && a.SysDiscountTemplate == discountTemplate.SysDiscountTemplate).FirstOrDefault();
                    if (result != null)
                    {                        
                        var UpdateItem = await db.UpdateAsync<DiscountTemplate>(discountTemplate);
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

        public async Task<bool> DeleteDiscountTemplate(int MerchantID, int SysDiscountTemplate )
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.DiscountTemplates.Where(m => m.MerchantID == MerchantID && m.SysDiscountTemplate == SysDiscountTemplate ).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteItem = await db.DiscountTemplates.Where(m => m.MerchantID == MerchantID && m.SysDiscountTemplate == SysDiscountTemplate).DeleteAsync();
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
