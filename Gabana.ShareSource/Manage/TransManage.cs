using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using LinqToDB;
using Xamarin.Forms.Xaml;
using static LinqToDB.Reflection.Methods.LinqToDB.Insert;

namespace Gabana.ShareSource.Manage
{
    public class TransManage
    {
        public TranDetailItemManage TranDetailItemManage = new TranDetailItemManage();
        public TranDetailItemToppingManage TranDetailItemToppingManage = new TranDetailItemToppingManage();
        public TranPaymentManage TranPaymentManage = new TranPaymentManage();
        public TranTradDiscountManage TranTradDiscountManage = new TranTradDiscountManage();
        public DeviceTranRunningNoManage DeviceTranRunningNoManage = new DeviceTranRunningNoManage();
        public DeviceTranRunningNo DeviceTranRunningNo;

        public async Task<List<TransHistoryNew>> GetTranHistory(int merchantId, int SysBranchID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {

                    List<TransHistoryNew> transHistoryInDb = await (from t in db.Trans
                                                                    where t.MerchantID == merchantId & t.SysBranchID == SysBranchID & t.TranType != 'O'
                                                                    select new TransHistoryNew()
                                                                    {
                                                                        tranNo = t.TranNo,
                                                                        tranDate = t.TranDate,
                                                                        customerName = t.CustomerName,
                                                                        grandTotal = t.GrandTotal,
                                                                        fCancel = t.FCancel,
                                                                        paymentType = (from tp in db.TranPayments
                                                                                       where tp.TranNo == t.TranNo
                                                                                       select tp.PaymentType).FirstOrDefault(),
                                                                        FWaiting = (int)t.FWaitSending,
                                                                        TypeOfflineOrOnline = 'F',

                                                                    })
                                                                        .OrderByDescending(x => x.tranDate)
                                                                        .ThenByDescending(x => x.tranNo)
                                                                        .ToListAsync();
                    return transHistoryInDb;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<TransHistoryNew>();
            };
        }

        public async Task<List<TransHistoryNew>> GetTranHistoryNew(int merchantId, int SysBranchID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    List<TransHistoryNew> transHistoryInDb = await (from t in db.Trans
                                                                    where (t.MerchantID == merchantId & t.SysBranchID == SysBranchID & (t.FWaitSending == 1 || t.FWaitSending == 2) & t.TranType != 'O')
                                                                    select new TransHistoryNew()
                                                                    {
                                                                        tranNo = t.TranNo,
                                                                        tranDate = t.TranDate,
                                                                        customerName = t.CustomerName,
                                                                        grandTotal = t.GrandTotal,
                                                                        fCancel = t.FCancel,
                                                                        paymentType = (from tp in db.TranPayments
                                                                                       where tp.TranNo == t.TranNo
                                                                                       select tp.PaymentType).FirstOrDefault(),
                                                                        FWaiting = (int)t.FWaitSending,
                                                                        TypeOfflineOrOnline = 'F'
                                                                    })
                                                                        .OrderByDescending(x => x.tranDate)
                                                                        .ThenByDescending(x => x.tranNo)
                                                                        .ToListAsync();
                    return transHistoryInDb;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<TransHistoryNew>();
            };
        }

        public async Task<TranWithDetailsLocal> GetTranHistoryDetail(int merchantId, int SysBranchID, String tranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    TranWithDetailsLocal TranWithDetails = await (from t in db.Trans
                                                                  where t.MerchantID == merchantId &
                                                                        t.SysBranchID == SysBranchID &
                                                                        t.TranNo == tranNo
                                                                  let tt = t
                                                                  select new TranWithDetailsLocal()
                                                                  {
                                                                      tran = t,
                                                                      tranDetailItemWithToppings = (from td in db.TranDetailItems
                                                                                                    where td.MerchantID == tt.MerchantID
                                                                                                            & td.SysBranchID == tt.SysBranchID
                                                                                                            & td.TranNo == tt.TranNo
                                                                                                    let topping = td
                                                                                                    select new TranDetailItemWithTopping
                                                                                                    {
                                                                                                        tranDetailItem = td as TranDetailItemNew,
                                                                                                        tranDetailItemToppings = (from tp in db.TranDetailItemToppings
                                                                                                                                  where tp.MerchantID == topping.MerchantID &
                                                                                                                                        tp.SysBranchID == topping.SysBranchID &
                                                                                                                                        tp.TranNo == topping.TranNo &
                                                                                                                                        tp.DetailNo == topping.DetailNo
                                                                                                                                  select tp).ToList()
                                                                                                    }).ToList(),
                                                                      tranPayments = (from TranPayment in db.TranPayments
                                                                                      where TranPayment.MerchantID == tt.MerchantID
                                                                                              & TranPayment.SysBranchID == tt.SysBranchID
                                                                                              & TranPayment.TranNo == tt.TranNo
                                                                                      select TranPayment).ToList(),
                                                                      tranTradDiscounts = (from TranTradDiscount in db.TranTradDiscounts
                                                                                           where TranTradDiscount.MerchantID == tt.MerchantID
                                                                                               & TranTradDiscount.SysBranchID == tt.SysBranchID
                                                                                               & TranTradDiscount.TranNo == tt.TranNo
                                                                                           select TranTradDiscount).ToList(),
                                                                  }
                                                            ).FirstOrDefaultAsync();
                    return TranWithDetails;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new TranWithDetailsLocal();
            };
        }

        public async Task<List<Tran>> GetAllTran(int merchantId, int SysBranchID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var trans = await db.Trans.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID)
                        .OrderByDescending(x => x.TranDate)
                        .ToListAsync();
                    return trans;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Tran>();
            };
        }

        public async Task<Tran> GetTran(int merchantId, int SysBranchID, string tranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var trans = await db.Trans.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranNo == tranNo).FirstOrDefaultAsync<Tran>();
                    return trans;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<Tran> GetTranAndroid(int merchantId, int SysBranchID, string tranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    try
                    {
                        await db.BeginTransactionAsync();
                        var trans = await db.Trans.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranNo == tranNo).FirstOrDefaultAsync<Tran>();
                        if (trans != null)
                        {
                            trans.FWaitSending = 1;
                        }
                        await db.UpdateAsync<Tran>(trans);
                        await db.CommitTransactionAsync();
                        return trans;
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

        public async Task<List<Tran>> GetAllTranOrder(int merchantId, int SysBranchID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var trans = await db.Trans.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranType == 'O' && x.Status == 100
                    && x.FWaitSending != 0 && x.TranDate.Between(DateTime.Now.AddDays(-30), DateTime.Now))
                    .ToListAsync<Tran>();
                    return trans;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<Tran> GetTranOrderBeforeClose(int merchantId, int SysBranchID)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    //var trans = await db.Trans.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranType == 'O' && x.Status == 100 && x.FWaitSending == 0 && x.LocalDataStatus == 'U').FirstOrDefaultAsync();
                    var trans = await db.Trans.Where(x => x.MerchantID == merchantId && x.SysBranchID == SysBranchID && x.TranType == 'O' && x.Status == 100 && x.FWaitSending == 0).FirstOrDefaultAsync();
                    return trans;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Tran>> GetTranFwaiting()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var trans = await db.Trans.Where(x => x.FWaitSending == 2).ToListAsync();
                    return trans;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Tran>> GetTranFwaitingOneTwo()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var trans = await db.Trans.Where(x => x.FWaitSending == 1 || x.FWaitSending == 2).ToListAsync();
                    return trans;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Tran>> GetTranFwaitingAndPrintCounter()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var trans = await db.Trans.Where(x => x.PrintCounterLocal > 0).ToListAsync();
                    return trans;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<Tran> GetTranSuccess(int merchantId, int SysBranchID, string TranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var trans = await db.Trans.Where(x => x.FWaitSending == 0 & x.MerchantID == merchantId & x.SysBranchID == SysBranchID & x.TranNo == TranNo).FirstOrDefaultAsync();
                    return trans;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<long> GetMaxTrans(long merchantId, int deviceNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    long max = 0;
                    var result = await db.Trans.Where(x => x.MerchantID == merchantId).ToListAsync();
                    if (result != null)
                    {
                        var tranno = result.Max(y => y.TranNo);

                        var arraydeviceNo = deviceNo.ToString().Split();
                        var a = arraydeviceNo.Length;
                        var b = tranno.ToString().Substring(a + 1);
                        var c = long.Parse(b);

                        max = c;
                    }
                    return max;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            };
        }

        public async Task<bool> InsertTran(TranWithDetailsLocal tranWithDetails)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            var tran = tranWithDetails.tran;
            var trandetailwithTopping = tranWithDetails.tranDetailItemWithToppings;
            var tranpayment = tranWithDetails.tranPayments;
            var tranDiscount = tranWithDetails.tranTradDiscounts;
            tranWithDetails.tran.TranDate = DateTime.UtcNow;

            if (string.IsNullOrEmpty(tranWithDetails.tran.MerchantID.ToString()))
            {
                throw new Exception("missing Merchantid");
            }
            if (tranWithDetails == null)
            {
                throw new Exception("tranWithDetails is null");
            }
            if (tranWithDetails.tran == null)
            {
                throw new Exception("tran is null");
            }
            if (string.IsNullOrEmpty(tranWithDetails.tran.TranNo))
            {
                throw new Exception("missing TranNo");
            }
            if (tranWithDetails.tran.TranDate == DateTime.MinValue) // ไม่ได้ระบุ datetime
            {
                throw new Exception("missing TranDate");
            }
            if (tranWithDetails.tran.DeviceNo <= 0)
            {
                throw new Exception("missing DeviceNo");
            }
            if (tranWithDetails.tranDetailItemWithToppings.Count == 0)
            {
                throw new Exception("tranDetailItemWithToppings count = 0");
            }

            string dateDB = tranWithDetails.tran.TranDate.ToString("yyyyMM"); // เลือก database จาก trandate
            using (var db = new MerchantDB(DataCashingAll.Pathdb))
            {
                var transection = await db.BeginTransactionAsync();
                try
                {
                    var checkDummy = trandetailwithTopping.FindAll(x => x.tranDetailItem.SysItemID == 0).ToList();
                    if (checkDummy.Count > 0)
                    {
                        foreach (var item in checkDummy.Where(x => x.tranDetailItem.SysItemID == 0))
                        {
                            item.tranDetailItem.SysItemID = null;
                        }
                    }

                    //30/06/2566 เพิ่ม getlast เพื่อเช้คล่าสุดอีกที //ปรับใหม่เพิ่มเงื่อนไขเช็คเฉพาะ B
                    if (tran.TranType == 'B')
                    {                        
                        var maxtranno = await DeviceTranRunningNoManage.GetLastDeviceTranRunningNo(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, DataCashingAll.DeviceNo);
                        maxtranno++;
                        tranWithDetails.tran.TranNo = "#" + DataCashingAll.DeviceNo.ToString() + "-" + maxtranno.ToString("D6");
                    }

                    int insertTrans = await db.InsertAsync<Tran>(tranWithDetails.tran);
                    if (insertTrans != 1)
                    {
                        await transection.RollbackAsync();
                        return false;
                    }

                    //Insert tranWithDetails.tranDetailItems
                    for (int i = 0; i < trandetailwithTopping.Count; i++)
                    {
                        var checkInsertTranDetail = await TranDetailItemManage.InsertTranDetailItem(trandetailwithTopping[i].tranDetailItem, db);
                        if (!checkInsertTranDetail)
                        {
                            await transection.RollbackAsync();
                            return false;
                        }

                        if (trandetailwithTopping[i].tranDetailItemToppings.Count == 0)
                        {
                            continue;
                        }

                        var checkInsertTranDetailTopping = await TranDetailItemToppingManage.InsertListTranDetailItemTopping(trandetailwithTopping[i].tranDetailItemToppings, db);
                        if (!checkInsertTranDetailTopping)
                        {
                            await transection.RollbackAsync();
                            return false;
                        }
                    }

                    //Insert tranWithDetails.tranPayments
                    for (int i = 0; i < tranpayment.Count; i++)
                    {
                        var checkInsertTranPayment = await TranPaymentManage.InsertTranPayment(tranpayment[i], db);
                        if (!checkInsertTranPayment)
                        {
                            await transection.RollbackAsync();
                            return false;
                        }
                    }

                    //Insert tranWithDetails.tranDiscount
                    for (int i = 0; i < tranDiscount.Count; i++)
                    {
                        var checkInsertTranPayment = await TranTradDiscountManage.InsertTranTradDiscount(tranDiscount[i], db);
                        if (!checkInsertTranPayment)
                        {
                            await transection.RollbackAsync();
                            return false;
                        }
                    }

                    if (tranWithDetails.tran.TranType == 'B')
                    {
                        var runningno = tran.TranNo.Substring(tran.TranNo.Length - 6);
                        var runningnolong = long.Parse(runningno);

                        DeviceTranRunningNo = new DeviceTranRunningNo
                        {
                            MerchantID = tranWithDetails.tran.MerchantID,
                            SysBranchID = DataCashingAll.SysBranchId,
                            DeviceNo = DataCashingAll.DeviceNo,
                            TranLastRunningNo = runningnolong
                        };

                        var updateRunnging = await DeviceTranRunningNoManage.UpdateDeviceTranRunningNo(DeviceTranRunningNo, db);
                        if (!updateRunnging)
                        {
                            await transection.RollbackAsync();
                            return false;
                        }
                    }

                    //หากทำทุกอย่าง success แล้วก็จะ commit transaction
                    await transection.CommitAsync();

                    // return เลขtranno ที่Insertสำเร็จ หาก tranno ที่return ไม่ตรงกับ tranno ที่ client ให้client ทำการupdate tranno ให้เหมือนกับที่ server
                    return true;
                }
                catch (Exception ex)
                {
                    await transection.RollbackAsync();
                    throw ex;
                }
            }
        }

        public async Task<bool> InsertOrReplaceTran(TranWithDetailsLocal tranWithDetails)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            using (var db = new MerchantDB(DataCashingAll.Pathdb))
            {
                await db.BeginTransactionAsync();
                var UpdateTran = await db.InsertOrReplaceAsync<Tran>(tranWithDetails.tran);
                if (UpdateTran != 1)
                {
                    await db.RollbackTransactionAsync();
                }

                //Insert tranWithDetails.tranDetailItems
                foreach (var itemDetailItems in tranWithDetails.tranDetailItemWithToppings)
                {
                    var UpdateTrandetail = await db.InsertOrReplaceAsync<TranDetailItem>(itemDetailItems.tranDetailItem);
                    if (UpdateTrandetail != 1)
                    {
                        await db.RollbackTransactionAsync();
                    }
                }

                //Insert tranWithDetails.tranPayments
                foreach (var itempaqyment in tranWithDetails.tranPayments)
                {
                    var UpdateTrandetail = await db.InsertOrReplaceAsync<TranPayment>(itempaqyment);
                    if (UpdateTrandetail != 1)
                    {
                        await db.RollbackTransactionAsync();
                    }
                }

                //หากทำทุกอย่าง success แล้วก็จะ commit transaction
                await db.CommitTransactionAsync();

                return true;
            }
        }

        public async Task<bool> UpdateTran(Tran tran)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.Trans.Where(m => m.MerchantID == tran.MerchantID && m.SysBranchID == tran.SysBranchID && m.TranNo == tran.TranNo).FirstOrDefault();
                    if (result == null)
                    {
                        return false;
                    }

                    var UpdateTran = await db.UpdateAsync<Tran>(tran);
                    if (UpdateTran != 1)
                    {
                        return false;
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
        public async Task<bool> UpdateTranWaitSendingTime(Tran tran)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = await db.Trans.Where(m => m.MerchantID == tran.MerchantID && m.SysBranchID == tran.SysBranchID && m.TranNo == tran.TranNo)
                                         .Set(x => x.WaitSendingTime, DateTime.UtcNow)
                                         .UpdateAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }

        public async Task<bool> UpdatePrintCounterTran(Tran tran)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.Trans.Where(m => m.MerchantID == tran.MerchantID && m.SysBranchID == tran.SysBranchID && m.TranNo == tran.TranNo).FirstOrDefault();
                    if (result == null)
                    {
                        return false;
                    }

                    if (result.LocalDataStatus == 'I' || result.LocalDataStatus == '\0')
                    {
                        result.PrintCounterLocal = tran.PrintCounterLocal;

                        if (result.FWaitSending == 2)
                        {
                            result.LocalDataStatus = 'U';
                        }

                        var UpdateTran = await db.UpdateAsync<Tran>(result);
                        if (UpdateTran != 1)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        var UpdateTran = await db.UpdateAsync<Tran>(tran);
                        if (UpdateTran != 1)
                        {
                            return false;
                        }
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

        public async Task<bool> UpdateTranSync(MerchantDB db, Tran tran)
        {
            try
            {
                var result = db.Trans.Where(m => m.MerchantID == tran.MerchantID && m.SysBranchID == tran.SysBranchID && m.TranNo == tran.TranNo).FirstOrDefault();
                if (result == null)
                {
                    return false;
                }

                var UpdateTran = await db.UpdateAsync<Tran>(tran);
                if (UpdateTran != 1)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> UpdateTranNo(string TranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var UpdateTran = await db.Trans.Where(m => m.MerchantID == DataCashingAll.MerchantId && m.SysBranchID == DataCashingAll.SysBranchId && m.TranNo == TranNo && m.FWaitSending != 0).Set(x => x.FWaitSending, 0).UpdateAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            };
        }

        public async Task<bool> DeleteTran(int merchantId, int SysBranchID, string tranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                using (var db = new MerchantDB(DataCashingAll.Pathdb))
                {
                    var result = db.Trans.Where(m => m.MerchantID == merchantId && m.SysBranchID == SysBranchID && m.TranNo == tranNo).FirstOrDefault();
                    if (result != null)
                    {
                        var DeleteTran = await db.Trans.Where(m => m.MerchantID == merchantId && m.SysBranchID == SysBranchID && m.TranNo == tranNo).DeleteAsync();
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

        public async Task DeleteTranOrder30Day(int merchantId, int SysBranchID)
        {
            using (var db = new MerchantDB(DataCashingAll.Pathdb))
            {
                try
                {
                    CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                    DateTime dateTimeNow = new DateTime();
                    await db.BeginTransactionAsync();
                    var lstalltranOrder = await GetAllTranOrder(merchantId, SysBranchID);
                    if (lstalltranOrder.Count > 0)
                    {
                        //มี Order ระยะเวลาที่ไม่ได้ชำระเงิน 30 วัน
                        foreach (var order in lstalltranOrder)
                        {
                            int day = (dateTimeNow.Date - order.TranDate.Date).Days;

                            //ลบข้อมูลทั้งหมด
                            if (day >= 30)
                            {
                                //Delete TranDetailItems
                                //Get All tranDetailItems ของ Trans นี้
                                var allTranDetailItems = await TranDetailItemManage.GetTranDetailItem(merchantId, SysBranchID, order.TranNo);
                                foreach (var TranDetailItems in allTranDetailItems)
                                {
                                    var allTranDetailItemsTopping = await TranDetailItemToppingManage.GetListTranDetailItemTopping((int)TranDetailItems.MerchantID, SysBranchID, order.TranNo);
                                    if (allTranDetailItemsTopping != null)
                                    {
                                        foreach (var item in allTranDetailItemsTopping)
                                        {
                                            var checkDeletetranDetailItemsTopping = await TranDetailItemManage.DeleteTranDetailItem((int)item.MerchantID, (int)item.SysBranchID, item.TranNo);
                                            if (!checkDeletetranDetailItemsTopping)
                                            {
                                                await db.RollbackTransactionAsync();
                                                break;
                                            }
                                        }
                                    }

                                    var checkDeletetranDetailItems = await TranDetailItemManage.DeleteTranDetailItem(merchantId, SysBranchID, TranDetailItems.TranNo);
                                    if (!checkDeletetranDetailItems)
                                    {
                                        await db.RollbackTransactionAsync();
                                        break;
                                    }
                                }

                                //Delete TranPayments
                                var allTranPayments = await TranPaymentManage.GetTranPayment(merchantId, SysBranchID, order.TranNo);
                                foreach (var TranPayments in allTranPayments)
                                {
                                    var checkDeletetranDetailItems = await TranPaymentManage.DeleteTranPayment(merchantId, SysBranchID, TranPayments.TranNo);
                                    if (!checkDeletetranDetailItems)
                                    {
                                        await db.RollbackTransactionAsync();
                                        break;
                                    }
                                }

                                //Delete TranDiscount
                                var allTranDiscount = await TranTradDiscountManage.GetTranTradDiscount(merchantId, SysBranchID, order.TranNo);
                                foreach (var TranTranDiscount in allTranDiscount)
                                {
                                    var checkDeletetranDetailItems = await TranPaymentManage.DeleteTranPayment(merchantId, SysBranchID, TranTranDiscount.TranNo);
                                    if (!checkDeletetranDetailItems)
                                    {
                                        await db.RollbackTransactionAsync();
                                        break;
                                    }
                                }

                                //Delete Tran
                                var checkDeleteTrans = await DeleteTran(merchantId, SysBranchID, order.TranNo);
                                if (!checkDeleteTrans)
                                {
                                    await db.RollbackTransactionAsync();
                                    break;
                                }

                                //หากทำทุกอย่าง success แล้วก็จะ commit transaction
                                await db.CommitTransactionAsync();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await db.RollbackTransactionAsync();
                    return;
                }
            }
        }


    }
}
