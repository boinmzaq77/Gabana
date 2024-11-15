using Gabana.Controller.GabanaSQLite.Framework;
using Gabana.ORM;
using Gabana.ORM.MerchantDB;
using Gabana3.JAM.Items;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Manage
{
    public class CustomerManage
    {
        DeviceSystemSeqNoManage DeviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        int sysItemId = initSystemID.SYSTEMID.Customer;

        public async Task<bool> InsertOrReplaceCustomer(Customer customer)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {

                    var checkstatus = await db.InsertOrReplaceAsync<Customer>(customer);
                    return (checkstatus > 0 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<List<Customer>> GetCustomerFwaiting()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var customer = await db.Customers.Where(x => x.FWaitSending == 2).ToListAsync();
                    return customer;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Customer>> GetAllCustomer()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstCustomer = new List<Customer>();
                    lstCustomer = await db.Customers.Where(x => x.DataStatus != 'D' && x.MerchantID == DataCashingAll.MerchantId).OrderBy(x => x.CustomerName).ToListAsync<Customer>();
                    return lstCustomer;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<Customer> GetCustomer(int merchant, int SysCustomerID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var MerchantID = await db.Customers.Where(x => x.MerchantID == merchant & x.SysCustomerID == SysCustomerID).FirstOrDefaultAsync();
                    return MerchantID;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<Customer> GetCustomerAndroid(int merchant, int SysCustomerID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    try
                    {
                        await db.BeginTransactionAsync();
                        var Customerdata = await db.Customers.Where(x => x.MerchantID == merchant & x.SysCustomerID == SysCustomerID).FirstOrDefaultAsync();
                        if (Customerdata != null)
                        {
                            Customerdata.FWaitSending = 1;
                        }

                        var UpdateCustomerdata = await db.UpdateAsync<Customer>(Customerdata);
                        await db.CommitTransactionAsync();
                        return Customerdata;
                    }
                    catch (Exception ex)
                    {
                        await db.RollbackTransactionAsync();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Customer>> GetCustomerSearch(int merchant, string Cusname)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var CustList = await db.Customers.Where(x => x.MerchantID == merchant & x.CustomerName.Contains(Cusname) | x.Mobile.Contains(Cusname) | x.CustomerID.Contains(Cusname) & x.DataStatus != 'D').OrderBy(x => x.CustomerName).ToListAsync();
                    return CustList;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Customer>> GetAllCustomerImageLoadnotComplete()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var lstCustomer = new List<Customer>();
                    lstCustomer = await db.Customers.Where(x => x.DataStatus != 'D' && x.MerchantID == DataCashingAll.MerchantId && !string.IsNullOrEmpty(x.PicturePath) && string.IsNullOrEmpty(x.ThumbnailLocalPath)).OrderBy(x => x.CustomerName).ToListAsync<Customer>();
                    return lstCustomer;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<bool> UpdateCustomerMembeytype(int merchant, int membertypeNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var count = await db.MemberTypes.Where(x => x.MerchantID == merchant & x.MemberTypeNo == membertypeNo).CountAsync();
                    if (count > 0)
                    {
                        var Customers = await db.Customers.Where(x => x.MerchantID == merchant & x.MemberTypeNo == membertypeNo).CountAsync();
                        if (Customers > 0)
                        {
                            var Update = await db.Customers.Where(x => x.MerchantID == merchant & x.MemberTypeNo == membertypeNo).Set(x => x.MemberTypeNo, x => null).UpdateAsync();
                            if (Update != 1)
                            {
                                return false;
                            }
                            return true;
                        }
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

        public async Task<bool> UpdateNullCustomerandDeleteMembeytype(int merchant, int membertypeNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var count = await db.MemberTypes.Where(x => x.MerchantID == merchant & x.MemberTypeNo == membertypeNo).CountAsync();
                    if (count > 0)
                    {
                        await db.BeginTransactionAsync();
                        var Customers = await db.Customers.Where(x => x.MerchantID == merchant & x.MemberTypeNo == membertypeNo).CountAsync();
                        if (Customers > 0)
                        {
                            var Update = await db.Customers.Where(x => x.MerchantID == merchant & x.MemberTypeNo == membertypeNo).Set(x => x.MemberTypeNo, x => null).UpdateAsync();
                            if (Update < 0)
                            {
                                await db.RollbackTransactionAsync();
                                return false;
                            }
                        }
                        var DeleteMemberType = await db.MemberTypes.Where(c => c.MerchantID == merchant && c.MemberTypeNo == membertypeNo).DeleteAsync();
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

        public async Task<bool> InsertCustomer(Customer customer)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            using (var db = new MerchantDB(DataCashingAll.Pathdb))
            {
                var transection = await db.BeginTransactionAsync();
                try
                {
                    var checkstatus = await db.InsertAsync<Customer>(customer);
                    if (checkstatus != 1)
                    {
                        await transection.RollbackAsync();
                        return false;
                    }

                    //Insert SeqNo;
                    var insertSeq = await DeviceSystemSeqNoManage.GetDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, sysItemId);
                    var lastSeqNo = insertSeq + 1;
                    DeviceSystemSeqNo DeviceSeq = new DeviceSystemSeqNo()
                    {
                        DeviceNo = DataCashingAll.DeviceNo,
                        MerchantID = DataCashingAll.MerchantId,
                        SystemID = sysItemId,
                        LastSysSeqNo = lastSeqNo
                    };
                    var checkResult = await DeviceSystemSeqNoManage.UpdateDeviceSystemSeqNo(DeviceSeq, db);
                    if (!checkResult)
                    {
                        await transection.RollbackAsync();
                        return false;
                    }

                    await transection.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transection.RollbackAsync();
                    throw ex;
                }
            }
        }

        public async Task<bool> UpdateCustomer(Customer customer)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.Customers.Where(m => m.MerchantID == customer.MerchantID & m.SysCustomerID == customer.SysCustomerID).FirstOrDefault();
                    if (result != null)
                    {
                        var UpdateCustomer = await db.UpdateAsync<Customer>(customer);
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

        public async Task<bool> DeleteCustomer(int MerID, int syscustomerID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.Customers.Where(m => m.MerchantID == MerID && m.SysCustomerID == syscustomerID).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteCustomer = await db.Customers.Where(c => c.MerchantID == MerID && c.SysCustomerID == syscustomerID).DeleteAsync();
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

        public async Task<bool> CheckCustomerName(string CustomerName)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))

                {
                    var lstcustomer = await db.Customers.Where(x => x.DataStatus != 'D' & x.CustomerName.Contains(CustomerName)).OrderBy(x => x.CustomerName).ToListAsync();
                    foreach (var item in lstcustomer)
                    {
                        var result = string.Equals(CustomerName, item.CustomerName, StringComparison.CurrentCultureIgnoreCase);
                        if (result)
                        {
                            return true;
                        }
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
