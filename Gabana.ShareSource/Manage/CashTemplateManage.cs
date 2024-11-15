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
    public class CashTemplateManage
    {
        public async Task<List<CashTemplate>> GetAllCashTemplate(int merchantID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lsttype = new List<CashTemplate>();
                    var result = await db.CashTemplates.Where(x => x.MerchantID == merchantID).FirstOrDefaultAsync();

                    if (result != null)
                    {
                        lsttype = await db.CashTemplates.ToListAsync<CashTemplate>();
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

        public async Task<CashTemplate> GetCashTemplate(int merchantID, int CashTemplateNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = await db.CashTemplates.Where(x => x.MerchantID == merchantID & x.CashTemplateNo == CashTemplateNo).FirstOrDefaultAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> InsertCashTemplate(CashTemplate cashTemplate)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var checkstatus = await db.InsertAsync<CashTemplate>(cashTemplate);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> InsertorReplaceListCashTemplate(List<CashTemplate> lstcashTemplates)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                int checkstatus = 0;
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    foreach (var item in lstcashTemplates)
                    {
                        checkstatus = await db.InsertOrReplaceAsync<CashTemplate>(item);
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

        public async Task<bool> InsertorReplaceCashTemplate(CashTemplate cashTemplate)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    int check = 0;
                    var count = await db.CashTemplates.Where(x => x.MerchantID == cashTemplate.MerchantID & x.CashTemplateNo == cashTemplate.CashTemplateNo).CountAsync();

                    if (count > 0)
                    {
                        check = await db.CashTemplates.Where(x => x.MerchantID == cashTemplate.MerchantID & x.CashTemplateNo == cashTemplate.CashTemplateNo)
                                .Set(x => x.MerchantID, cashTemplate.MerchantID)
                                .Set(x => x.CashTemplateNo, cashTemplate.CashTemplateNo)
                                .Set(x => x.Amount, cashTemplate.Amount)
                                .Set(x => x.DateModified, UtilsAll.TranDateformat(DateTime.UtcNow))
                                .UpdateAsync();
                        if (check != 1)
                        {
                            return (check > 0 ? true : false);
                        }
                    }
                    else
                    {
                        check = await db.InsertAsync<CashTemplate>(cashTemplate);
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

        public async Task<bool> UpdateCashTemplate(CashTemplate cashTemplate)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.CashTemplates.Where(m => m.MerchantID == cashTemplate.MerchantID && m.CashTemplateNo == cashTemplate.CashTemplateNo).FirstOrDefault();
                    if (result != null)
                    {
                        var UpdateCashTemplate = await db.UpdateAsync<CashTemplate>(cashTemplate);
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

        public async Task<bool> DeleteCashTemplate(int merchantID, int CashTemplateNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.CashTemplates.Where(m => m.MerchantID == merchantID && m.CashTemplateNo == CashTemplateNo).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteCashTemplate = await db.CashTemplates.Where(c => c.MerchantID == merchantID && c.CashTemplateNo == CashTemplateNo).DeleteAsync();
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

        public async Task<bool> DeleteAllCashTemplatee(int merchantID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.CashTemplates.Where(m => m.MerchantID == merchantID).ToList();
                    if (result != null)
                    {
                        var DeleteMemberType = await db.CashTemplates.DeleteAsync();
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
