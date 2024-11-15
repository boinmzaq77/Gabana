using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using Xunit;

namespace XUnitTestGabana
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

            var tranWithDetails = new TranWithDetailsLocal();
            tranWithDetails.tran = new Gabana.ORM.MerchantDB.Tran();
            tranWithDetails.tran.MerchantID = 10001;
            tranWithDetails.tran.SysBranchID = 0;
            var maxtranno = 1;
            maxtranno++;
            tranWithDetails.tran.TranNo = "#1000001";
            tranWithDetails.tran.TranDate = DateTime.UtcNow;

            tranWithDetails.tran.Status = 10;
            tranWithDetails.tran.DeviceNo = 1;
            tranWithDetails.tran.SysCustomerID = 1;
            tranWithDetails.tran.CustomerName = "Got";//
            tranWithDetails.tran.SellerName = "Got";
            tranWithDetails.tran.LastDateModified = DateTime.UtcNow;
            tranWithDetails.tran.LastUserModified = "Got";
            tranWithDetails.tran.FCancel = 0;
            tranWithDetails.tran.TranTaxType = 'I';
            tranWithDetails.tran.CountTradDisc = 0;
            tranWithDetails.tran.SubTotalNoneVat = 0;
            tranWithDetails.tran.TotalTradDiscNoneVat = 0;
            tranWithDetails.tran.TotalNoneVat = 0;
            tranWithDetails.tran.SubTotalHaveVat = 0;
            tranWithDetails.tran.TotalTradDiscHaveVat = 0;
            tranWithDetails.tran.TotalHaveVat = 0;
            tranWithDetails.tran.Total = 0;
            tranWithDetails.tran.ServiceCharge = 0;
            tranWithDetails.tran.TotalVat = 0;
            tranWithDetails.tran.GrandTotal = 0;
            tranWithDetails.tran.PaymentFractional = 0;
            tranWithDetails.tran.GrandPayment = 0;
            tranWithDetails.tran.ServiceCharge = 0;
            tranWithDetails.tran.TotalVat = 0;
            tranWithDetails.tran.GrandTotal = 0;
            tranWithDetails.tran.PaymentFractional = 0;
            tranWithDetails.tran.GrandPayment = 0;
            tranWithDetails.tran.SummaryPayment = 0;
            tranWithDetails.tran.Change = 0;
            tranWithDetails.tran.Tips = 0;
            tranWithDetails.tran.TotalPointEarning = 0;
            tranWithDetails.tran.PrintCounter = 0;

            Item item = new Item()
            {
                SysItemID = 1000001,
                ItemName = "1000001",
                SaleItemType = 'U',
                TaxType = 'V',
                Price = 1000,
                EstimateCost = 900 , 
            };

            Item item2 = new Item()
            {
                SysItemID = 1000002,
                ItemName = "1000001",
                SaleItemType = 'U',
                TaxType = 'V',
                Price = 1000,
                EstimateCost = 900,
            };

            Item item3 = new Item()
            {
                SysItemID = 1000003,
                ItemName = "1000001",
                SaleItemType = 'U',
                TaxType = 'V',
                Price = 1000,
                EstimateCost = 900,
            };

            Item item4 = new Item()
            {
                SysItemID = 1000004,
                ItemName = "1000001",
                SaleItemType = 'U',
                TaxType = 'V',
                Price = 1000,
                EstimateCost = 900,
            };

            Item item5 = new Item()
            {
                SysItemID = 1000005,
                ItemName = "1000001",
                SaleItemType = 'U',
                TaxType = 'V',
                Price = 1000,
                EstimateCost = 900,
            };

            Item item6 = new Item()
            {
                SysItemID = 1000006,
                ItemName = "1000001",
                SaleItemType = 'U',
                TaxType = 'V',
                Price = 1000,
                EstimateCost = 900,
            };

            Item item7 = new Item()
            {
                SysItemID = 1000007,
                ItemName = "1000001",
                SaleItemType = 'U',
                TaxType = 'V',
                Price = 1000,
                EstimateCost = 900,
            };

            Item item8 = new Item()
            {
                SysItemID = 1000008,
                ItemName = "1000001",
                SaleItemType = 'U',
                TaxType = 'V',
                Price = 1000,
                EstimateCost = 900,
            };

            Item item9 = new Item()
            {
                SysItemID = 1000009,
                ItemName = "1000001",
                SaleItemType = 'U',
                TaxType = 'V',
                Price = 1000,
                EstimateCost = 900,
            };

            Item item10 = new Item()
            {
                SysItemID = 1000010,
                ItemName = "1000001",
                SaleItemType = 'U',
                TaxType = 'V',
                Price = 1000,
                EstimateCost = 900,
            };

            Item item11 = new Item()
            {
                SysItemID = 1000011,
                ItemName = "1000001",
                SaleItemType = 'U',
                TaxType = 'V',
                Price = 1000,
                EstimateCost = 900,
            };


            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails,item);
            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item);
            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item2);
            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item3);
            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item4);
            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item5);
            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item6);
            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item7);
            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item8);
            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item9);
            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item9);
            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item10);
            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item11);
            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item11);
            tranWithDetails = Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item11);
            tranWithDetails =  Gabana.ShareSource.BLTrans.ChooseItemTran(tranWithDetails, item11);
            tranWithDetails = Gabana.ShareSource.BLTrans.Caltran(tranWithDetails);
            TranWithDetailsLocalResult result = new TranWithDetailsLocalResult(); 
            for (int i = 0; i < tranWithDetails.tranDetailItems.Count; i++)
            {
                Assert.Equal(result.tranDetailItems[i].Quantity, tranWithDetails.tranDetailItems[i].Quantity);
                Assert.Equal(result.tranDetailItems[i].Price, tranWithDetails.tranDetailItems[i].Price);
                Assert.Equal(result.tranDetailItems[i].SubAmount, tranWithDetails.tranDetailItems[i].SubAmount);
                Assert.Equal(result.tranDetailItems[i].Discount, tranWithDetails.tranDetailItems[i].Discount);
                Assert.Equal(result.tranDetailItems[i].DiscountPromotion, tranWithDetails.tranDetailItems[i].DiscountPromotion);
                Assert.Equal(result.tranDetailItems[i].DiscountRedeem, tranWithDetails.tranDetailItems[i].DiscountRedeem);
                Assert.Equal(result.tranDetailItems[i].Amount, tranWithDetails.tranDetailItems[i].Amount);
                Assert.Equal(result.tranDetailItems[i].VatAmount, tranWithDetails.tranDetailItems[i].VatAmount);
                Assert.Equal(result.tranDetailItems[i].TaxBaseAmount, tranWithDetails.tranDetailItems[i].TaxBaseAmount);
                Assert.Equal(result.tranDetailItems[i].WeightTranDisc, tranWithDetails.tranDetailItems[i].WeightTranDisc);
                Assert.Equal(result.tranDetailItems[i].TotalPrice, tranWithDetails.tranDetailItems[i].TotalPrice);
                Assert.Equal(result.tranDetailItems[i].ProfitAmount, tranWithDetails.tranDetailItems[i].ProfitAmount);
            }

            Assert.Equal(tranWithDetails.tran.SubTotalNoneVat, tranWithDetails.tran.SubTotalNoneVat);
            Assert.Equal(result.tran.TotalTradDiscNoneVat, tranWithDetails.tran.TotalTradDiscNoneVat);
            Assert.Equal(result.tran.TotalNoneVat, tranWithDetails.tran.TotalNoneVat);
            Assert.Equal(result.tran.SubTotalHaveVat, tranWithDetails.tran.SubTotalHaveVat);
            Assert.Equal(result.tran.TotalTradDiscHaveVat, tranWithDetails.tran.TotalTradDiscHaveVat);
            Assert.Equal(result.tran.TotalHaveVat, tranWithDetails.tran.TotalHaveVat);
            Assert.Equal(result.tran.Total, tranWithDetails.tran.Total);
            Assert.Equal(result.tran.ServiceCharge, tranWithDetails.tran.ServiceCharge);
            Assert.Equal(result.tran.TotalVat, tranWithDetails.tran.TotalVat);
            Assert.Equal(result.tran.GrandTotal, tranWithDetails.tran.GrandTotal);
            Assert.Equal(result.tran.TotalProfit, tranWithDetails.tran.TotalProfit);
            Assert.Equal(result.tran.GrandPayment, tranWithDetails.tran.GrandPayment);

        }
    }
}
 