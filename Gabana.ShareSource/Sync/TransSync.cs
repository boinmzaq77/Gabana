using AutoMapper;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Sync
{
    static public class TransSync
    {
        static Gabana.ShareSource.Manage.TransManage transManage = new Gabana.ShareSource.Manage.TransManage();
        static Gabana.ShareSource.Manage.TranDetailItemManage tranDetailItemManage = new Gabana.ShareSource.Manage.TranDetailItemManage();
        static Gabana.ShareSource.Manage.TranPaymentManage tranPaymentManage = new Gabana.ShareSource.Manage.TranPaymentManage();
        static Gabana.ShareSource.Manage.TranDetailItemToppingManage toppingManage = new Gabana.ShareSource.Manage.TranDetailItemToppingManage();
        static Gabana.ShareSource.Manage.TranTradDiscountManage discountManage = new Manage.TranTradDiscountManage();
        static Gabana3.JAM.Trans.TranWithDetails JAMtranwithdetail = new Gabana3.JAM.Trans.TranWithDetails();

        static Gabana3.JAM.Trans.JsonOfTranwithDetailsWithFilePicture jsonOfTranwithDetailsWithFile = new Gabana3.JAM.Trans.JsonOfTranwithDetailsWithFilePicture();

        static public async Task SentTrans(int merchantid, int sysBranchid, string tranNo, byte[] ImageByte)
        {
            byte[] ImageByteArray = ImageByte;
            Tran tran = new Tran();
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                //int merchantId, int SysBranchID, string tranNo                  
                List<TranDetailItemWithTopping> lsttranDetailItemWithToppings = new List<TranDetailItemWithTopping>();
                Model.TranWithDetailsLocal TranWithDetailsLocal = new Model.TranWithDetailsLocal();
                TranDetailItemWithTopping detailItemWithTopping = new TranDetailItemWithTopping();
               
                tran = await transManage.GetTran(merchantid, sysBranchid, tranNo);
                if (tran == null)
                {
                    return;
                }

                #region TranWithDetailsLocal
                TranWithDetailsLocal.tran = tran;
                TranWithDetailsLocal.tran.TranDate = TranWithDetailsLocal.tran.TranDate;
                TranWithDetailsLocal.tran.LastDateModified = DateTime.UtcNow;
                TranWithDetailsLocal.tran.WaitSendingTime = DateTime.UtcNow;



                //List<Detail Item >
                List<TranDetailItem> tranDetail = new List<TranDetailItem>();
                tranDetail = await tranDetailItemManage.GetTranDetailItem(merchantid, sysBranchid, tranNo);
                foreach (var item in tranDetail)
                {
                    //Detail Item
                    TranDetailItemNew DetailItem = new TranDetailItemNew()
                    {
                        Amount = item.Amount,
                        Comments = item.Comments,
                        CumulativeSum = item.CumulativeSum,
                        DetailNo = item.DetailNo,
                        Discount = item.Discount,
                        DiscountPromotion = item.DiscountPromotion,
                        DiscountRedeem = item.DiscountRedeem,
                        EstimateCost = item.EstimateCost,
                        FmlDiscountRow = item.FmlDiscountRow,
                        FProcess = item.FProcess,
                        ItemName = item.ItemName,
                        MerchantID = item.MerchantID,
                        Price = item.Price,
                        PricePerWeight = item.PricePerWeight,
                        ProfitAmount = item.ProfitAmount,
                        Quantity = item.Quantity,
                        RedeemCode = item.RedeemCode,
                        SaleItemType = item.SaleItemType,
                        SizeName = item.SizeName,
                        SubAmount = item.SubAmount,
                        SumToppingEstimateCost = item.SumToppingEstimateCost,
                        SumToppingPrice = item.SumToppingPrice,
                        SysBranchID = item.SysBranchID,
                        SysItemID = item.SysItemID,
                        TaxBaseAmount = item.TaxBaseAmount,
                        TaxType = item.TaxType,
                        TotalPrice = item.TotalPrice,
                        TranNo = item.TranNo,
                        UnitName = item.UnitName,
                        VatAmount = item.VatAmount,
                        Weight = item.Weight,
                        WeightTranDisc = item.WeightTranDisc,
                        WeightUnitName = item.WeightUnitName,
                        ItemPrice = item.ItemPrice,
                    };

                    //Detail ItemTopping
                    List<TranDetailItemTopping> lstitemDetail = new List<TranDetailItemTopping>();
                    List<TranDetailItemTopping> lsttranTopping = new List<TranDetailItemTopping>();

                    lsttranTopping = await toppingManage.GetTranDetailItemTopping(merchantid, sysBranchid, tranNo, (int)item.DetailNo);
                    foreach (var itemtopping in lsttranTopping)
                    {
                        TranDetailItemTopping itemDetail = new TranDetailItemTopping()
                        {
                            MerchantID = itemtopping.MerchantID,
                            SysBranchID = itemtopping.SysBranchID,
                            TranNo = itemtopping.TranNo,
                            DetailNo = itemtopping.DetailNo,
                            ToppingNo = itemtopping.ToppingNo,
                            ItemName = itemtopping.ItemName,//toppping
                            SysItemID = itemtopping.SysItemID,
                            UnitName = itemtopping.UnitName,
                            RegularSizeName = itemtopping.RegularSizeName,
                            Quantity = itemtopping.Quantity,
                            ToppingPrice = itemtopping.ToppingPrice,
                            EstimateCost = itemtopping.EstimateCost,
                            Comments = itemtopping.Comments
                        };
                        lstitemDetail.Add(itemDetail);
                    }

                    detailItemWithTopping = new TranDetailItemWithTopping();
                    detailItemWithTopping.tranDetailItem = DetailItem;
                    detailItemWithTopping.tranDetailItemToppings = lstitemDetail;
                    lsttranDetailItemWithToppings.Add(detailItemWithTopping);

                    lstitemDetail = new List<TranDetailItemTopping>();
                }

                TranWithDetailsLocal.tranDetailItemWithToppings.AddRange(lsttranDetailItemWithToppings);

                //Tran Payment
                var tranPayment = await tranPaymentManage.GetTranPayment(merchantid, sysBranchid, tranNo);
                foreach (var item in tranPayment)
                {
                    TranWithDetailsLocal.tranPayments.Add(item);
                }

                //Tran Discount
                var tranDiscount = await discountManage.GetTranTradDiscount(merchantid, sysBranchid, tranNo);
                foreach (var itemDiscount in tranDiscount)
                {
                    TranWithDetailsLocal.tranTradDiscounts.Add(itemDiscount);
                }
                #endregion

                if (tran is null)
                {
                    return;
                }

                if (tran.FWaitSending == 0)
                {
                    return;
                }

                switch (tran.FCancel)
                {
                    case 0:
                        if (TranWithDetailsLocal.tran.LocalDataStatus == 'I')
                        {
                            InsertTrans(TranWithDetailsLocal, ImageByteArray);
                        }
                        else
                        {
                            UpdateTranOrder(TranWithDetailsLocal);
                        }
                        break;
                    case 1:
                        VoidTrans(TranWithDetailsLocal, ImageByteArray);
                        break;
                    default:
                        break;
                }
            }
            catch (WebException)
            {
                tran = await transManage.GetTran(merchantid, sysBranchid, tranNo);
                tran.FWaitSending = 2;
                await transManage.UpdateTran(tran);
            }
        }

        static public async Task SentTransAndroid(int merchantid, int sysBranchid, string tranNo, byte[] ImageByte)
        {
            byte[] ImageByteArray = ImageByte;
            Tran tran = new Tran();
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                //int merchantId, int SysBranchID, string tranNo                  
                List<TranDetailItemWithTopping> lsttranDetailItemWithToppings = new List<TranDetailItemWithTopping>();
                Model.TranWithDetailsLocal TranWithDetailsLocal = new Model.TranWithDetailsLocal();
                TranDetailItemWithTopping detailItemWithTopping = new TranDetailItemWithTopping();

                tran = await transManage.GetTranAndroid(merchantid, sysBranchid, tranNo);
                if (tran == null)
                {
                    return;
                }

                #region TranWithDetailsLocal
                TranWithDetailsLocal.tran = tran;
                TranWithDetailsLocal.tran.TranDate = TranWithDetailsLocal.tran.TranDate;
                TranWithDetailsLocal.tran.LastDateModified = DateTime.UtcNow;
                TranWithDetailsLocal.tran.WaitSendingTime = DateTime.UtcNow;

                //List<Detail Item >
                List<TranDetailItem> tranDetail = new List<TranDetailItem>();
                tranDetail = await tranDetailItemManage.GetTranDetailItem(merchantid, sysBranchid, tranNo);
                foreach (var item in tranDetail)
                {
                    //Detail Item
                    TranDetailItemNew DetailItem = new TranDetailItemNew()
                    {
                        Amount = item.Amount,
                        Comments = item.Comments,
                        CumulativeSum = item.CumulativeSum,
                        DetailNo = item.DetailNo,
                        Discount = item.Discount,
                        DiscountPromotion = item.DiscountPromotion,
                        DiscountRedeem = item.DiscountRedeem,
                        EstimateCost = item.EstimateCost,
                        FmlDiscountRow = item.FmlDiscountRow,
                        FProcess = item.FProcess,
                        ItemName = item.ItemName,
                        MerchantID = item.MerchantID,
                        Price = item.Price,
                        PricePerWeight = item.PricePerWeight,
                        ProfitAmount = item.ProfitAmount,
                        Quantity = item.Quantity,
                        RedeemCode = item.RedeemCode,
                        SaleItemType = item.SaleItemType,
                        SizeName = item.SizeName,
                        SubAmount = item.SubAmount,
                        SumToppingEstimateCost = item.SumToppingEstimateCost,
                        SumToppingPrice = item.SumToppingPrice,
                        SysBranchID = item.SysBranchID,
                        SysItemID = item.SysItemID,
                        TaxBaseAmount = item.TaxBaseAmount,
                        TaxType = item.TaxType,
                        TotalPrice = item.TotalPrice,
                        TranNo = item.TranNo,
                        UnitName = item.UnitName,
                        VatAmount = item.VatAmount,
                        Weight = item.Weight,
                        WeightTranDisc = item.WeightTranDisc,
                        WeightUnitName = item.WeightUnitName,
                        ItemPrice = item.ItemPrice,
                    };

                    //Detail ItemTopping
                    List<TranDetailItemTopping> lstitemDetail = new List<TranDetailItemTopping>();
                    List<TranDetailItemTopping> lsttranTopping = new List<TranDetailItemTopping>();

                    lsttranTopping = await toppingManage.GetTranDetailItemTopping(merchantid, sysBranchid, tranNo, (int)item.DetailNo);
                    foreach (var itemtopping in lsttranTopping)
                    {
                        TranDetailItemTopping itemDetail = new TranDetailItemTopping()
                        {
                            MerchantID = itemtopping.MerchantID,
                            SysBranchID = itemtopping.SysBranchID,
                            TranNo = itemtopping.TranNo,
                            DetailNo = itemtopping.DetailNo,
                            ToppingNo = itemtopping.ToppingNo,
                            ItemName = itemtopping.ItemName,//toppping
                            SysItemID = itemtopping.SysItemID,
                            UnitName = itemtopping.UnitName,
                            RegularSizeName = itemtopping.RegularSizeName,
                            Quantity = itemtopping.Quantity,
                            ToppingPrice = itemtopping.ToppingPrice,
                            EstimateCost = itemtopping.EstimateCost,
                            Comments = itemtopping.Comments
                        };
                        lstitemDetail.Add(itemDetail);
                    }

                    detailItemWithTopping = new TranDetailItemWithTopping();
                    detailItemWithTopping.tranDetailItem = DetailItem;
                    detailItemWithTopping.tranDetailItemToppings = lstitemDetail;
                    lsttranDetailItemWithToppings.Add(detailItemWithTopping);

                    lstitemDetail = new List<TranDetailItemTopping>();
                }

                TranWithDetailsLocal.tranDetailItemWithToppings.AddRange(lsttranDetailItemWithToppings);

                //Tran Payment
                var tranPayment = await tranPaymentManage.GetTranPayment(merchantid, sysBranchid, tranNo);
                foreach (var item in tranPayment)
                {
                    TranWithDetailsLocal.tranPayments.Add(item);
                }

                //Tran Discount
                var tranDiscount = await discountManage.GetTranTradDiscount(merchantid, sysBranchid, tranNo);
                foreach (var itemDiscount in tranDiscount)
                {
                    TranWithDetailsLocal.tranTradDiscounts.Add(itemDiscount);
                }
                #endregion

                if (tran is null)
                {
                    return;
                }

                if (tran.FWaitSending == 0)
                {
                    return;
                }

                switch (tran.FCancel)
                {
                    case 0:
                        if (TranWithDetailsLocal.tran.LocalDataStatus == 'I')
                        {
                            InsertTrans(TranWithDetailsLocal, ImageByteArray);
                        }
                        else
                        {
                            UpdateTranOrder(TranWithDetailsLocal);
                        }
                        break;
                    case 1:
                        VoidTrans(TranWithDetailsLocal, ImageByteArray);
                        break;
                    default:
                        break;
                }
            }
            catch (WebException)
            {
                tran = await transManage.GetTran(merchantid, sysBranchid, tranNo);
                tran.FWaitSending = 2;
                await transManage.UpdateTran(tran);
            }
        }

        private static async void InsertTrans(Model.TranWithDetailsLocal tranWithDetails, byte[] ImageByte)
        {
            try
            {
                #region Old Code
                var config = new MapperConfiguration(cfg =>
                {
                    //struct ของ table
                    cfg.CreateMap<Model.TranWithDetailsLocal, Gabana3.JAM.Trans.TranWithDetails>();
                    cfg.CreateMap<TranDetailItemWithTopping, Gabana3.JAM.Trans.TranDetailItemWithTopping>();
                    cfg.CreateMap<Tran, ORM.Period.Tran>();
                    cfg.CreateMap<TranDetailItemTopping, ORM.Period.TranDetailItemTopping>();
                    cfg.CreateMap<TranDetailItemNew, ORM.Period.TranDetailItem>();
                    cfg.CreateMap<TranTradDiscount, ORM.Period.TranTradDiscount>();
                    cfg.CreateMap<TranPayment, ORM.Period.TranPayment>();
                });

                // TranWithDetailsLocal
                var Imapper = config.CreateMapper();
                JAMtranwithdetail = Imapper.Map<Model.TranWithDetailsLocal, Gabana3.JAM.Trans.TranWithDetails>(tranWithDetails);

                if (JAMtranwithdetail == null)
                {
                    return;
                }

                //ตรวจสอบว่า tranNo ยิงไปแล้วหรือยัง ถ้ายิงแล้วให้ return ไม่ต้องยิงซ้ำ
                Tran tran = new Tran();
                tran = await transManage.GetTranSuccess((int)tranWithDetails.tran.MerchantID, (int)tranWithDetails.tran.SysBranchID, tranWithDetails.tran.TranNo);
                if (tran != null)
                {
                    return;
                }

                tranWithDetails.tran.FWaitSending = 0; //Set FWaitSending ให้สำเร็จก่อนเลย
                await transManage.UpdateTran(tranWithDetails.tran); //Update status 0 

                jsonOfTranwithDetailsWithFile = new Gabana3.JAM.Trans.JsonOfTranwithDetailsWithFilePicture();
                jsonOfTranwithDetailsWithFile.value = JsonConvert.SerializeObject(JAMtranwithdetail);

                var result = await GabanaAPI.PostDataTran(jsonOfTranwithDetailsWithFile, ImageByte);
                if (result.Status)
                {
                    tranWithDetails.tran.FWaitSending = 0;
                    if (JAMtranwithdetail.tran.TranType == 'B')
                    {
                        tranWithDetails.tran.Status = 30;
                        var TranPayment = tranWithDetails.tranPayments.Where(x => !string.IsNullOrEmpty(x.PicturePath)).FirstOrDefault();
                        if (!string.IsNullOrEmpty(TranPayment?.PicturePath))
                        {
                            if (System.IO.File.Exists(TranPayment?.PicturePath))
                            {
                                System.IO.File.Delete(TranPayment?.PicturePath);
                                TranPayment.PicturePath = null;

                                TranPaymentManage tranPayment = new TranPaymentManage();
                                var data = await tranPayment.UpdateTranPayment(TranPayment);
                            }
                        }
                    }
                    else
                    {
                        tranWithDetails.tran.Status = 110;
                    }

                    //Update TranNo ให้เป็นเลขล่าสุดจากข้างบน
                    string UpdateTranNo = result.Message;

                    //Tran
                    tranWithDetails.tran.TranNo = UpdateTranNo;

                    //tranWithDetails.tranDetailItemWithToppings 
                    foreach (var item in tranWithDetails.tranDetailItemWithToppings)
                    {
                        item.tranDetailItem.TranNo = UpdateTranNo;

                        foreach (var topping in item.tranDetailItemToppings)
                        {
                            topping.TranNo = UpdateTranNo;
                        }
                    }

                    //tranWithDetails.tranPayments
                    foreach (var payment in tranWithDetails.tranPayments)
                    {
                        payment.TranNo = UpdateTranNo;
                    }

                    //tranWithDetails.tranDiscount
                    foreach (var discount in tranWithDetails.tranTradDiscounts)
                    {
                        discount.TranNo = UpdateTranNo;
                    }
                }
                else
                {
                    tranWithDetails.tran.FWaitSending = 2;
                    if (JAMtranwithdetail.tran.TranType == 'B')
                    {
                        tranWithDetails.tran.Status = 20;
                    }
                    else
                    {
                        tranWithDetails.tran.Status = 100;
                    }
                }                

                await transManage.UpdateTran(tranWithDetails.tran);
                #endregion
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);               

                tranWithDetails.tran = await transManage.GetTran((int)JAMtranwithdetail.tran.MerchantID, (int)JAMtranwithdetail.tran.SysBranchID, JAMtranwithdetail.tran.TranNo);
                tranWithDetails.tran.FWaitSending = 2;
                if (JAMtranwithdetail.tran.TranType == 'B')
                {
                    tranWithDetails.tran.Status = 20;
                }
                else
                {
                    tranWithDetails.tran.Status = 100;
                }
                await transManage.UpdateTran(tranWithDetails.tran);
            }
        }

        private static async void UpdateTranOrder(Model.TranWithDetailsLocal tranWithDetails)
        {
            try
            {
                var config = new MapperConfiguration(cfg =>
                {
                    //struct ของ table
                    cfg.CreateMap<Model.TranWithDetailsLocal, Gabana3.JAM.Trans.TranWithDetails>();
                    cfg.CreateMap<TranDetailItemWithTopping, Gabana3.JAM.Trans.TranDetailItemWithTopping>();
                    cfg.CreateMap<Tran, ORM.Period.Tran>();
                    cfg.CreateMap<TranDetailItemTopping, ORM.Period.TranDetailItemTopping>();
                    cfg.CreateMap<TranDetailItemNew, ORM.Period.TranDetailItem>();
                    cfg.CreateMap<TranTradDiscount, ORM.Period.TranTradDiscount>();
                    cfg.CreateMap<TranPayment, ORM.Period.TranPayment>();
                });

                // TranWithDetailsLocal
                var Imapper = config.CreateMapper();
                JAMtranwithdetail = Imapper.Map<Model.TranWithDetailsLocal, Gabana3.JAM.Trans.TranWithDetails>(tranWithDetails);

                if (JAMtranwithdetail == null)
                {
                    return;
                }

                JAMtranwithdetail.tran.ServerDateTime = DateTime.UtcNow;
                var stringDate = UtilsAll.ChangeDateTime(JAMtranwithdetail.tran.TranDate);

                var result = await GabanaAPI.PutDataTran(JAMtranwithdetail.tran.SysBranchID, JAMtranwithdetail.tran.TranNo, stringDate);
                if (result.Status)
                {
                    tranWithDetails.tran.FWaitSending = 0;
                    tranWithDetails.tran.Status = 110;

                }
                else
                {
                    tranWithDetails.tran.FWaitSending = 1;
                    tranWithDetails.tran.Status = 100;
                }
                await transManage.UpdateTran(tranWithDetails.tran);
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                tranWithDetails.tran = await transManage.GetTran((int)JAMtranwithdetail.tran.MerchantID, (int)JAMtranwithdetail.tran.SysBranchID, JAMtranwithdetail.tran.TranNo);
                tranWithDetails.tran.FWaitSending = 2;
                if (JAMtranwithdetail.tran.TranType == 'B')
                {
                    tranWithDetails.tran.Status = 20;
                }
                else
                {
                    tranWithDetails.tran.Status = 100;
                }
                await transManage.UpdateTran(tranWithDetails.tran);
            }
        }

        private static async void VoidTrans(Model.TranWithDetailsLocal tranWithDetails, byte[] ImageByte)
        {
            try
            {
                var result = await GabanaAPI.DeleteDataTran(DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo, UtilsAll.ChangeDateTime(tranWithDetails.tran.TranDate));
                if (result.Status)
                {
                    tranWithDetails.tran.FWaitSending = 0;
                }
                else if (result.Message.ToLower() == "no content")
                {
                    //case no content
                    //tranWithDetails.tran.FCancel = 1;
                    InsertTrans(tranWithDetails, ImageByte);
                }
                else
                {
                    tranWithDetails.tran.FWaitSending = 2;
                }
                await transManage.UpdateTran(tranWithDetails.tran);
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                tranWithDetails.tran = await transManage.GetTran((int)tranWithDetails.tran.MerchantID, (int)JAMtranwithdetail.tran.SysBranchID, tranWithDetails.tran.TranNo);
                tranWithDetails.tran.FWaitSending = 2;
                await transManage.UpdateTran(tranWithDetails.tran);
            }
        }


        #region ตัด stock เมื่อบันทึก tran สำเร็จ
        //static async void CutStockFromTran(Gabana3.JAM.Trans.TranWithDetails custockTran)
        //{
        //    try
        //    {
        //        Gabana3.JAM.Trans.TranWithDetails Tranthis = new Gabana3.JAM.Trans.TranWithDetails();
        //        Tranthis = custockTran;

        //        ItemOnBranchManage itemOnBranchManage = new ItemOnBranchManage();
        //        ItemManage itemManage = new ItemManage();

        //        foreach (var item in Tranthis.tranDetailItemWithToppings)
        //        {
        //            // item
        //            var dataitem = await itemManage.GetItem(DataCashingAll.MerchantId, (int)item.tranDetailItem.SysItemID);
        //            if (dataitem == null)
        //            {
        //                break;
        //            }

        //            if (dataitem.FTrackStock == 1)
        //            {
        //                var stock = await GabanaAPI.GetDataStock(DataCashingAll.SysBranchId, (int)dataitem.SysItemID);
        //                if (stock != null)
        //                {
        //                    ItemOnBranch itemOnBranch = new ItemOnBranch()
        //                    {
        //                        MerchantID = stock.MerchantID,
        //                        SysBranchID = stock.SysBranchID,
        //                        SysItemID = stock.SysItemID,
        //                        BalanceStock = stock.BalanceStock,
        //                        MinimumStock = stock.MinimumStock
        //                    };
        //                    var updateitem = await itemOnBranchManage.UpdateItemOnBranch(itemOnBranch);
        //                }
        //            }

        //            //topping
        //            foreach (var topping in item.tranDetailItemToppings)
        //            {
        //                var datatopping = await itemManage.GetItem(DataCashingAll.MerchantId, (int)topping.SysItemID);
        //                if (datatopping == null)
        //                {
        //                    break;
        //                }

        //                if (datatopping.FTrackStock == 1)
        //                {
        //                    var stocktopping = await GabanaAPI.GetDataStock(DataCashingAll.SysBranchId, (int)datatopping.SysItemID);
        //                    if (stocktopping != null)
        //                    {
        //                        ItemOnBranch itemOnBranch = new ItemOnBranch()
        //                        {
        //                            MerchantID = stocktopping.MerchantID,
        //                            SysBranchID = stocktopping.SysBranchID,
        //                            SysItemID = stocktopping.SysItemID,
        //                            BalanceStock = stocktopping.BalanceStock,
        //                            MinimumStock = stocktopping.MinimumStock
        //                        };
        //                        var updatetopping = await itemOnBranchManage.UpdateItemOnBranch(itemOnBranch);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //} 
        #endregion
    }

}
