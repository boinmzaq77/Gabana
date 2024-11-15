using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using CoreText;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Common;
using ObjCRuntime;

using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public class DumpView : UIView
    {
        nfloat posY;
        CGSize cGSize;
        private Branch BranchDetail;
        string LanguageShow = Preferences.Get("Language", "");

        public override void Draw(CGRect rect)
        {
            //posY = 0;
            //cGSize = new CGSize(rect.Width, rect.Height);

            ////CGSize cGSize = new CGSize(rect.Width, rect.Height);
            //UIGraphics.BeginImageContext(cGSize);
            //var context = UIGraphics.GetCurrentContext();

            //context.ScaleCTM(1, -1); // you flipped the context, now you must use negative Y values to draw "into" the view

            ////var textHeight = new CTFont("Arial", 16).CapHeightMetric; // lets use the actaul height of the font captials.

            //DrawTextt(context, "ซีเนียร์ซอฟท์ ดีเวลลอปเม้นท์", Align.Center, 0, posY, new CTFont("Arial", 30));
            ////posY += textHeight+10;
            //DrawTextt(context, "TAX#201910290001", Align.Left,  0, posY, new CTFont("Arial", 24));
            ////posY += textHeight + 10;
            //DrawTextt(context, "ใบเสร็จรับเงิน/ใบกำกับภาษีอย่างย่อ", Align.Center, 0, posY, new CTFont("Arial", 22));
            ////posY += textHeight + 10;

            //List<string> itemName = new List<string>()
            //{
            //    "1   จัมโบ้บิ๊กเปาหมูสับไข่เค็ม          30.00",
            //    "1   ดีน่างาดำ                    10.00",
            //    "1   เวเฟอร์ล็อคเกอร์               10.00",
            //    "2   เวเฟอร์ล็อคเกอร์               10.00",
            //    "3   เวเฟอร์ล็อคเกอร์               10.00",
            //    "4   เวเฟอร์ล็อคเกอร์               10.00",
            //    "5   เวเฟอร์ล็อคเกอร์               10.00",
            //    "6   เวเฟอร์ล็อคเกอร์               10.00",
            //    "7   เวเฟอร์ล็อคเกอร์               10.00",
            //    "8   เวเฟอร์ล็อคเกอร์               10.00",
            //    "9   เวเฟอร์ล็อคเกอร์               10.00",
            //    "10   เวเฟอร์ล็อคเกอร์              10.00",
            //    "11   เวเฟอร์ล็อคเกอร์              10.00",
            //    "5   S-point                   0.00",
            //    "    ยอดรวม                    50.00",
            //    "ยอดสุทธิ   3 ชิ้น                50.00",
            //    "เงินสด/เงินทอน                 100.00",
            //    "                                  ",
            //    "----------------------------------",
            //};
            //for (int i = 0; i < itemName.Count; i++)
            //{
            //    DrawTextt(context, itemName[i], Align.Left, 0, posY, new CTFont("Arial", 22));
            //}

            ////DrawText(context, "Hello", textHeight, 0, 0);
            ////DrawText(context, "How are you?", textHeight, 0, 20);
            ////DrawText(context, "Sincerely,", textHeight, 0, 40);
            ////DrawText(context, "StackOverflow,", textHeight, 0, 60);
        }

        public async Task<nfloat> Draw2(CGRect rect, TranWithDetailsLocal tran, List<string> address)
        {
            CGRect size1;
            posY = 0;
            string CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            int[] sizehead = { 30, 26 };
            int[] sizedetail = { 24, 18 };
            int[] itemLength = { 36, 28 };
            int choosesize = 0;
            if (rect.Width != 580)
            {
                choosesize = 1; 
            }
            var itemsnub = ItemDetail(tran.tranDetailItemWithToppings, itemLength[choosesize]);
            
            rect.Height = rect.Height + (itemsnub.Count * 36);
            cGSize = new CGSize(rect.Width, rect.Height);

            //CGSize cGSize = new CGSize(rect.Width, rect.Height);
            UIGraphics.BeginImageContext(cGSize);
            var context = UIGraphics.GetCurrentContext();

            context.ScaleCTM(1, -1);
            if (LanguageShow.ToLower() == "th")
            {
                size1 = DrawTexttnew(context, "ใบเสร็จรับเงิน", Align.Center, 0, posY, new CTFont("Arial", sizehead[choosesize]), sizehead[choosesize]);
            }
            else
            {
                size1 = DrawTexttnew(context, "Receipt", Align.Center, 0, posY, new CTFont("Arial", sizehead[choosesize]), sizehead[choosesize]);
            }
            
            Newline(size1,0);

            size1 = DrawTexttnew(context, MainController.merchantlocal.Name.ToString(), Align.Center, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1,0);

            
            if (address.Count > 0)
            {
                foreach (var item in address)
                {
                    size1 = DrawTexttnew(context, item, Align.Center, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                    Newline(size1, 0);
                }
            }
            //if (!(string.IsNullOrEmpty(tran.merchantTel))) 
            //{
            //    size1 = DrawTexttnew(context, tran.merchantTel?.ToString(), Align.Center, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            //    Newline(size1, 0);
            //}
            var timezoneslocal = TimeZoneInfo.Local;
            DrawTexttnew(context, tran.tran.TranNo, Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            size1 = DrawTexttnew(context, TimeZoneInfo.ConvertTimeFromUtc(tran.tran.TranDate, timezoneslocal).ToString("dd/MM/yyy HH:mm tt", new CultureInfo("en-US")), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1,0);

            DrawTexttnew(context, tran.tran.CustomerName, Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1, 0);
            //string nameperson = ""; 
            //if (tran.personId?.Trim() != "999")
            //{
            //    nameperson = tran.prename + " " + tran.fname + " " + tran.lname;
            //}
            //else
            //{
            //    nameperson = "เงินสด"; 
            //}
            //size1 = DrawTexttnew(context, nameperson, Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            ////size1 = DrawTexttnew(context, tran.tranDate.ToString("G", CultureInfo.CreateSpecificCulture("th-TH")), Align.Right, 0, posY, new CTFont("Arial", 24));
            ////Newline(size1,0);

            //if (!string.IsNullOrEmpty(tran.personAddressTax))
            //{

            //    size1 = DrawTexttnew(context, tran.personAddressTax, Align.Center, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            //    //size1 = DrawTexttnew(context, tran.tranDate.ToString("G", CultureInfo.CreateSpecificCulture("th-TH")), Align.Right, 0, posY, new CTFont("Arial", 24));
            //    Newline(size1,0);
            //}

            //Newline(size1, 2);
            var items = ItemDetail(tran.tranDetailItemWithToppings, itemLength[choosesize]);
            int j = 0;
            for (int i = 0; i < items.Count; i++)
            {
                var check = items[i].Substring(0, 2);
                if (!check.Trim().IsNullOrEmpty())
                {
                    DrawTexttnew(context, items[i], Align.Left, 20, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                    size1 = DrawTexttnew(context, tran.tranDetailItemWithToppings[j].tranDetailItem.Amount.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                    
                    j++;
                }
                else
                {
                    size1 =  DrawTexttnew(context, items[i].Trim(), Align.Left, 30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                    //size1 = DrawTexttnew(context, "", Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                    
                }
                if (i == items.Count - 1)
                {
                    Newline(size1, 2);
                }
                else
                {
                    Newline(size1, 0);
                }
            }
            //var items = ItemDetail(tran.tranDetailItemWithToppings, itemLength[choosesize]);
            //var x = 0;
            //for (int i = 0; i < items.Count; i++)
            //{

            //    if (items.Contains("x" + tran.tranDetailItemWithToppings[x].tranDetailItem.Quantity.ToString("###0")))
            //    {
            //       //data.Add(escPos.ReplaceSpacebar2(items[i], tran.tranDetailItemWithToppings[x].tranDetailItem.Amount.ToString("#,##0.00")));

            //        DrawTexttnew(context, items[i], Align.Left, 20, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            //        size1 = DrawTexttnew(context, tran.tranDetailItemWithToppings[x].tranDetailItem.Amount.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            //        if (i == items.Count - 1)
            //        {
            //            Newline(size1, 2);
            //        }
            //        else
            //        {
            //            Newline(size1, 0);
            //        }
            //        x++;
            //    }
            //    else
            //    {
            //        DrawTexttnew(context, tran.tran.CustomerName, Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            //        Newline(size1, 0);
            //    }
            //}

            if (LanguageShow.ToLower() == "th")
            {
                DrawTexttnew(context, "จำนวนรายการ : " + tran.tranDetailItemWithToppings.Count.ToString(), Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                size1 = DrawTexttnew(context, "รวมจำนวนชิ้น : " + tran.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity).ToString("N0"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            }
            else
            {
                DrawTexttnew(context, "Quantity : " + tran.tranDetailItemWithToppings.Count.ToString(), Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                size1 = DrawTexttnew(context, "Quantity Total : " + tran.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity).ToString("N0"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            }
            Newline(size1,0);

            if (LanguageShow.ToLower() == "th")
            {
                DrawTexttnew(context, "รวมเงิน", Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                DrawTexttnew(context, "รวมทั้งสิ้น", Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                size1 = DrawTexttnew(context, tran.tran.Total.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            }
            else
            {
                DrawTexttnew(context, "Subtotal", Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                DrawTexttnew(context, "Total", Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                size1 = DrawTexttnew(context, tran.tran.Total.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            }
            Newline(size1, 0);

            //if (tran.tran.TotalVat != 0)
            //{
                DrawTexttnew(context, "ภาษีมูลค่าเพิ่ม " + (tran.tran.TaxRate == 0 ? "" : tran.tran.TaxRate?.ToString("#,##0") + "%"), Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                DrawTexttnew(context, "Vat " + (tran.tran.TaxRate == 0 ? "" : tran.tran.TaxRate?.ToString("#,##0") + "%"), Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                size1 = DrawTexttnew(context, tran.tran.TotalVat.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                Newline(size1, 0);
            //}
            //if (tran.tran.ServiceCharge != 0)
            //{
            string servicetext = "";
            if (!string.IsNullOrEmpty(tran.tran.FmlServiceCharge))
            {
                var checkservice = tran.tran.FmlServiceCharge.IndexOf('%');
                if (checkservice == -1)
                {
                    var ServiceCharge = tran.tran.FmlServiceCharge;
                    if (Convert.ToDecimal(ServiceCharge) > 0)
                    {
                        servicetext =  CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge));
                    }
                }
                else
                {
                    string[] split = tran.tran.FmlServiceCharge.Split('%');
                    var ServiceCharge = split[0];
                    if (Convert.ToDecimal(ServiceCharge) > 0)
                    {
                        servicetext =  Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)) + "%";
                    }
                }
            }
            DrawTexttnew(context, "ค่าบริการ "+ servicetext, Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            DrawTexttnew(context, "Service "+ servicetext, Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            size1 = DrawTexttnew(context, tran.tran.ServiceCharge.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1, 0);
            //}
            double disMember = 0;
            var tranTradDiscountMember = tran.tranTradDiscounts?.Where(x => x.DiscountType == "PS").FirstOrDefault();
            if (tranTradDiscountMember != null)
            {
                DrawTexttnew(context, "สมาชิก", Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                DrawTexttnew(context, "Member", Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                size1 = DrawTexttnew(context, (tranTradDiscountMember.TradeDiscHaveVat+tranTradDiscountMember.TradeDiscNoneVat).ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                Newline(size1, 0);
            }
            else
            {
                DrawTexttnew(context, "สมาชิก", Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                DrawTexttnew(context, "Member", Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                size1 = DrawTexttnew(context, 0.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                Newline(size1, 0);

            }

            double disMD = 0;
            var tranTradDiscountMD = tran.tranTradDiscounts?.Where(x => x.DiscountType == "MD").FirstOrDefault();
            if (tranTradDiscountMD != null)
            {
                DrawTexttnew(context, "ส่วนลด", Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                DrawTexttnew(context, "Discount", Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                size1 = DrawTexttnew(context, (tranTradDiscountMD.TradeDiscHaveVat + tranTradDiscountMD.TradeDiscNoneVat).ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                Newline(size1, 0);
            }
            else
            {
                DrawTexttnew(context, "ส่วนลด", Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                DrawTexttnew(context, "Discount", Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                size1 = DrawTexttnew(context, 0.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                Newline(size1, 0);
            }

            //DrawTexttnew(context, "ส่วนลด", Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            //DrawTexttnew(context, "Discount" , Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            //size1 = DrawTexttnew(context, tran.tran.TotalProfit.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            //Newline(size1,0);

            DrawTexttnew(context, "รวมทั้งสิ้น" , Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            DrawTexttnew(context, "Grand Total", Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            size1 = DrawTexttnew(context, tran.tran.GrandTotal.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1,2);

            DrawTexttnew(context, "...THANK YOU...", Align.Center,0 , posY, new CTFont("Arial", sizehead[choosesize]), sizehead[choosesize]);
            Newline(size1,0);

            size1 = DrawTexttnew(context, "Cashier: " +tran.tran.SellerName.ToString(), Align.Center, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1, 2);

            size1 = DrawTexttnew(context, "Powered By Seniorsoft", Align.Center, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1, 0);

            var height = Newline(size1,0);
            cGSize.Height = height; 

            context.ConvertSizeToUserSpace(cGSize);
            return height;
           
        }

        public async Task< List<string>> Draw3(int size, TranWithDetailsLocal tran)
        {
            try
            {
                string CartDiscount = "";
                double disDiscont = 0.0;
                List<string> data = new List<string>();
                int[] itemLength = { 28, 40 };
                int[] itemLength2 = { 33, 48 };
           
                string CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                EscPosCommand escPos = new EscPosCommand(size);
                if (LanguageShow.ToLower() == "th")
                {
                    data.Add("ใบเสร็จรับเงิน");
                }
                else
                {
                    data.Add("Receipt");
                } 

                string mername = "";
                if (ThaiLength(DataCashingAll.MerchantLocal.Name) > itemLength[size - 1])
                {
                    mername = DataCashingAll.MerchantLocal.Name.Substring(0, itemLength[size - 1]);
                }
                else
                {
                    mername = DataCashingAll.MerchantLocal.Name;
                }
                data.Add(mername);
                if (await GabanaAPI.CheckNetWork())
                {
                    List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
                    cloudbranch = await GabanaAPI.GetDataBranch();
                    var local = cloudbranch.Where(x => x.SysBranchID == tran.tran.SysBranchID).FirstOrDefault();
                    BranchDetail = new ORM.MerchantDB.Branch()
                    {
                        Address = local.Address,
                        ProvincesId = local.ProvincesId,
                        AmphuresId = local.AmphuresId,
                        DistrictsId = local.DistrictsId,
                        Tel = local.Tel,
                    };
                }
                else
                {
                    BranchManage branchManage = new BranchManage();
                    BranchDetail = await branchManage.GetBranch(DataCashingAll.MerchantId, (int)tran.tran.SysBranchID);
                }
                var branchaddress = await Utils.SetTextAddressAsync(BranchDetail);
                data.Add(branchaddress);
                data.Add(escPos.ReplaceSpacebar2(tran.tran.TranNo.ToString(), Utils.PrintDateTime(tran.tran.TranDate)));
                data.Add(escPos.ReplaceSpacebar2(tran.tran.CustomerName, ""));
                var items = ItemDetail(tran.tranDetailItemWithToppings, itemLength[size - 1]);
                int j = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    var check = items[i].Substring(0, 2);
                    if (!check.Trim().IsNullOrEmpty())
                    {
                        data.Add(escPos.ReplaceSpacebar2(items[i], tran.tranDetailItemWithToppings[j].tranDetailItem.Amount.ToString("#,##0.00")));
                        j++;
                    }
                    else
                    {
                        data.Add(escPos.ReplaceSpacebar2(items[i], ""));
                    }
                }
                if (LanguageShow.ToLower() == "th")
                {
                    data.Add(escPos.ReplaceSpacebar2("จำนวนรายการ : " + tran.tranDetailItemWithToppings.Count.ToString(), "รวมจำนวนชิ้น : " + tran.tranDetailItemWithToppings.Sum(x2 => x2.tranDetailItem.Quantity).ToString("N0")));
                }
                else
                {
                    data.Add(escPos.ReplaceSpacebar2("Quantity : " + tran.tranDetailItemWithToppings.Count.ToString(), "รวมจำนวนชิ้น : " + tran.tranDetailItemWithToppings.Sum(x2 => x2.tranDetailItem.Quantity).ToString("N0")));
                }

                var total = Convert.ToDouble(tran.tran.GrandTotal);
                if (LanguageShow.ToLower() == "th")
                {
                    data.Add(escPos.ReplaceSpacebar2("รวมเงิน", tran.tran.Total.ToString("#,##0.00")));
                }
                else
                {
                    data.Add(escPos.ReplaceSpacebar2("Subtotal", tran.tran.Total.ToString("#,##0.00")));
                }
                if (tran.tran.TotalVat > 0)
                {
                    if (LanguageShow.ToLower() == "th")
                    {
                        data.Add(escPos.ReplaceSpacebar2("ภาษีมูลค่าเพิ่ม " + tran.tran.TaxRate?.ToString("#,##0") + "%", tran.tran.TotalVat.ToString("#,##0.00")));
                    }
                    else
                    {
                        data.Add(escPos.ReplaceSpacebar2("VAT " + tran.tran.TaxRate?.ToString("#,##0") + "%", tran.tran.TotalVat.ToString("#,##0.00")));
                    }
                }
                if (!string.IsNullOrEmpty(tran.tran.FmlServiceCharge))
                {
                    var checkservice = tran.tran.FmlServiceCharge.IndexOf('%');
                    if (checkservice == -1)
                    {
                        var ServiceCharge = tran.tran.FmlServiceCharge;
                        if (Convert.ToDecimal(ServiceCharge) > 0)
                        {
                            if (LanguageShow.ToLower() == "th")
                            {
                                data.Add(escPos.ReplaceSpacebar2("ค่าบริการ " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)), Convert.ToDecimal(tran.tran.ServiceCharge).ToString("#,##0.00")));
                            }
                            else
                            {
                                data.Add(escPos.ReplaceSpacebar2("Service Charge " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)), Convert.ToDecimal(tran.tran.ServiceCharge).ToString("#,##0.00")));
                            }
                        }
                    }
                    else
                    {
                        string[] split = tran.tran.FmlServiceCharge.Split('%');
                        var ServiceCharge = split[0];
                        if (Convert.ToDecimal(ServiceCharge) > 0)
                        {
                            if (LanguageShow.ToLower() == "th")
                            {
                                data.Add(escPos.ReplaceSpacebar2("ค่าบริการ " + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)) + "%", Convert.ToDecimal(tran.tran.ServiceCharge).ToString("#,##0.00")));
                            }
                            else
                            {
                                data.Add(escPos.ReplaceSpacebar2("Service Charge " + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)) + "%", Convert.ToDecimal(tran.tran.ServiceCharge).ToString("#,##0.00")));
                            }
                        }
                    }
                }

                double disMember = 0;
                var tranTradDiscountMember = tran.tranTradDiscounts.Where(x => x.DiscountType == "PS").FirstOrDefault();
                if (tranTradDiscountMember != null)
                {
                    var check = tranTradDiscountMember.FmlDiscount.IndexOf('%');
                    if (check == -1)
                    {
                        disMember = Convert.ToDouble(tranTradDiscountMember.TradeDiscNoneVat + tranTradDiscountMember.TradeDiscHaveVat);
                        if (LanguageShow.ToLower() == "th")
                        {
                            data.Add(escPos.ReplaceSpacebar2("สมาชิก " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Replace("%", ""))), "-" + Convert.ToDecimal(disMember).ToString("#,##0.00")));
                        }
                        else
                        {
                            data.Add(escPos.ReplaceSpacebar2("Member " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Replace("%", ""))), "-" + Convert.ToDecimal(disMember).ToString("#,##0.00")));
                        }
                    }
                    else
                    {
                        disMember = Convert.ToDouble(tranTradDiscountMember.TradeDiscNoneVat + tranTradDiscountMember.TradeDiscHaveVat);
                        if (LanguageShow.ToLower() == "th")
                        {
                            data.Add(escPos.ReplaceSpacebar2("สมาชิก " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Remove(check))) + "%", "-" + Convert.ToDecimal(disMember).ToString("#,##0.00")));
                        }
                        else
                        {
                            data.Add(escPos.ReplaceSpacebar2("Member " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Remove(check))) + "%", "-" + Convert.ToDecimal(disMember).ToString("#,##0.00")));
                        }
                    }
                }
                double discount;
                var tranTradDiscount = tran.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                if (tranTradDiscount != null)
                {
                    var check = tranTradDiscount.FmlDiscount.IndexOf('%');
                    if (check == -1)
                    {
                        CartDiscount = tranTradDiscount.FmlDiscount;
                        discount = Convert.ToDouble(CartDiscount);
                        //textDiscount.Text = GetString(Resource.String.cart_activity_discount) + " " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscount.FmlDiscount));
                        disDiscont = discount;
                    }
                    else
                    {

                        discount = Convert.ToDouble(tranTradDiscount.FmlDiscount.Remove(check));
                        //textDiscount.Text = GetString(Resource.String.cart_activity_discount) + " " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscount.FmlDiscount.Remove(check))) + "%";
                        discount = discount / 100;
                        disDiscont = total * discount;
                    }
                    //textDiscountAmount.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disDiscont));
                }

                var tranTradDiscount2 = tran.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                if (tranTradDiscount2 != null)
                {
                    if (LanguageShow.ToLower() == "th")
                    {
                        data.Add(escPos.ReplaceSpacebar2("ส่วนลด", "-" + Convert.ToDecimal(disDiscont).ToString("#,##0.00")));
                    }
                    else
                    {
                        data.Add(escPos.ReplaceSpacebar2("Discount", "-" + Convert.ToDecimal(disDiscont).ToString("#,##0.00")));
                    }
                }

                if (LanguageShow.ToLower() == "th")
                {
                    data.Add(escPos.ReplaceSpacebar2("รวมทั้งสิ้น", tran.tran.GrandPayment.ToString("#,##0.00")));
                }
                else
                {
                    data.Add(escPos.ReplaceSpacebar2("Total", tran.tran.GrandPayment.ToString("#,##0.00")));
                }
               
                data.Add("...THANK YOU...");
                data.Add("Cashier: " + tran.tran.SellerName.ToString());
                data.Add("Powered By Seniorsoft");
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int ThaiLength(string stringthai)
        {
            int len = 0;
            int l = stringthai.Length;
            for (int i = 0; i < l; ++i)
            {
                if (char.GetUnicodeCategory(stringthai[i]) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    ++len;
            }
            return len;
        }

        nfloat Newline(CGRect sizeOfText,int more)
        {
            posY += (sizeOfText.Height + 5)+more;
            return sizeOfText.Height;
        }

        public enum Align
        {
            Left = 0,
            Right = 1,
            Center = 2
        }
        List<string> ItemDetail2(List<TranDetailItemWithTopping> items, int choosesize)
        {
            List<string> itemName = new List<string>();
            for (int i = 0; i < items.Count; i++)
            {
                string res = "";
                var count1 = 4 - (int)items[i].tranDetailItem.Quantity.ToString("###0").Length;
                switch (count1)
                {
                    case 3:
                        res = items[i].tranDetailItem.Quantity.ToString("###0") + "   " + items[i].tranDetailItem.ItemName;
                        break;
                    case 2:
                        res = items[i].tranDetailItem.Quantity.ToString("###0") + "  " + items[i].tranDetailItem.ItemName;
                        break;
                    case 1:
                        res = items[i].tranDetailItem.Quantity.ToString("###0") + " " + items[i].tranDetailItem.ItemName;
                        break;
                    default:
                        break;
                }
                if (res.Length > choosesize)
                {
                    res = res.Substring(0, choosesize);
                }
                //var txt=  res.Substring(0, 35);
                itemName.Add("x"+ res);
            }
            return itemName;
        }
        static List<string> ItemDetail(List<Model.TranDetailItemWithTopping> items, int choosesize)
        {
            List<string> itemName = new List<string>();
            for (int i = 0; i < items.Count; i++)
            {
                string sizename = items[i].tranDetailItem.SizeName;
                string name = "";
                if (string.IsNullOrEmpty(sizename) || sizename == "Default Size")
                {
                    name = items[i].tranDetailItem.ItemName?.ToString();
                }
                else
                {
                    name = items[i].tranDetailItem.ItemName?.ToString() + " " + sizename;
                }
                string res = "";
                string res2 = "";
                string res3 = "";
                string space = "";
                var count1 = 4 - (int)items[i].tranDetailItem.Quantity.ToString("###0").Length;
                var itemNames = Utils.SplitItemName(choosesize, name);
                switch (count1)
                {
                    case 3:
                        res = items[i].tranDetailItem.Quantity.ToString("###0") + "   " + itemNames[0];
                        if (!itemNames[1].IsNullOrEmpty())
                        {
                            res2 = "   " + itemNames[1];
                        }
                        if (!itemNames[2].IsNullOrEmpty())
                        {
                            res3 = "   " + itemNames[2];
                        }
                        break;
                    case 2:
                        res = items[i].tranDetailItem.Quantity.ToString("###0") + "  " + items[i].tranDetailItem.ItemName;
                        if (!itemNames[1].IsNullOrEmpty())
                        {
                            res2 = "  " + itemNames[1];
                        }
                        if (!itemNames[2].IsNullOrEmpty())
                        {
                            res3 = "  " + itemNames[2];
                        }
                        break;
                    case 1:
                        res = items[i].tranDetailItem.Quantity.ToString("###0") + " " + items[i].tranDetailItem.ItemName;
                        if (!itemNames[1].IsNullOrEmpty())
                        {
                            res2 = " " + itemNames[1];
                        }
                        if (!itemNames[2].IsNullOrEmpty())
                        {
                            res3 = " " + itemNames[2];
                        }
                        break;
                    default:
                        break;
                }
                if (res.Length > choosesize)
                {
                    res = res.Substring(0, choosesize);
                }
                //var txt=  res.Substring(0, 35);
                itemName.Add("x" + res);
                if (!res2.IsNullOrEmpty())
                {
                    itemName.Add("  " + res2);
                }
                if (!res3.IsNullOrEmpty())
                {
                    itemName.Add("  " + res3);
                }
                var listTopping = items[i].tranDetailItemToppings;
                foreach (var topping in listTopping)
                {
                    itemName.Add("  " + space + topping.ItemName + " (" + Utils.DisplayDecimal((topping.Quantity * topping.ToppingPrice)) + ")");
                }
                if (!string.IsNullOrEmpty(items[i].tranDetailItem.Comments))
                {
                    itemName.Add("  " + space + items[i].tranDetailItem.Comments);
                }
            }
            return itemName;
        }
        CGRect DrawTexttnew(CGContext context, string text, Align align, nfloat x, nfloat y, CTFont font, nfloat Heightnew)
        {
            var textHeight = font.CapHeightMetric;
            context.SetFillColor(UIColor.Black.CGColor);
            //context.SetFillColorSpace(CGColorSpace.CreateDeviceGray());

            var attributedString = new NSAttributedString(text,
                new CTStringAttributes
                {
                    ForegroundColorFromContext = true,
                    Font = font//new CTFont("Arial", 16)
                });

            CGRect sizeOfText;
            using (var textLine = new CTLine(attributedString))
            {
                // CGSize cGSize = new CGSize(384, 200);
                CGRect trect = textLine.GetBounds(CTLineBoundsOptions.UseGlyphPathBounds);
                nfloat ofsx = 0;
                switch (align)
                {
                    case Align.Left:
                        ofsx = x;
                        break;
                    case Align.Right:
                        ofsx = ((cGSize.Width - trect.Width)) + x;
                        break;
                    case Align.Center:
                        ofsx = ((cGSize.Width - trect.Width) / 2) + x;
                        break;
                    default:
                        break;
                }

                //context.TranslateCTM(ofsx, -(y + trect.Height));
                context.TranslateCTM(ofsx, -(y + Heightnew));
                textLine.Draw(context);
                sizeOfText = textLine.GetBounds(CTLineBoundsOptions.UseOpticalBounds);
                // Reset the origin back to where is was
                //context.TranslateCTM(-(ofsx + sizeOfText.Width), y + trect.Height);
                context.TranslateCTM(-(ofsx + sizeOfText.Width), y + Heightnew);
                return sizeOfText;
            }

        }
        


        void DrawText(CGContext context, string text, nfloat textHeight, nfloat x, nfloat y)
        {
            //CGSize cGSize = new CGSize(384, 530);
            //UIGraphics.BeginImageContext(cGSize);

            context.TranslateCTM(-x, -(y + textHeight));
            context.SetFillColor(UIColor.Red.CGColor);

            context.SetFillColorSpace(CGColorSpace.CreateDeviceGray());

            var attributedString = new NSAttributedString(text,
                new CTStringAttributes
                {
                    ForegroundColorFromContext = true,
                    Font = new CTFont("Arial", 16)
                });

            CGRect sizeOfText;
            using (var textLine = new CTLine(attributedString))
            {
                CGSize cGSize = new CGSize(384, 200);
                CGRect trect = textLine.GetBounds(CTLineBoundsOptions.UseGlyphPathBounds);
                nfloat ofsx = (cGSize.Width - trect.Width) / 2;
                nfloat ofsy = (cGSize.Height - trect.Height) / 2;


                context.SaveState();
                context.TranslateCTM(ofsx, -(y + textHeight));
                //context.ScaleCTM(1, -1);
                textLine.Draw(context);
                context.RestoreState();

                //textLine.Draw(context);
                sizeOfText = textLine.GetBounds(CTLineBoundsOptions.UseOpticalBounds);
            }
            // Reset the origin back to where is was
            context.TranslateCTM(x - sizeOfText.Width, y + sizeOfText.Height);
        }

        public async Task<nfloat> Draw2TestPrint(CGRect rect, TranWithDetailsLocal tran, List<string> address)
        {
            CGRect size1;
            posY = 0;
            string CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            int[] sizehead = { 30, 26 };
            int[] sizedetail = { 24, 18 };
            int[] itemLength = { 36, 28 };
            int choosesize = 0;
            if (rect.Width != 580)
            {
                choosesize = 1;
            }
            var itemsnub = ItemDetail(tran.tranDetailItemWithToppings, itemLength[choosesize]);

            rect.Height = rect.Height + (itemsnub.Count * 36);
            cGSize = new CGSize(rect.Width, rect.Height);

            UIGraphics.BeginImageContext(cGSize);
            var context = UIGraphics.GetCurrentContext();

            context.ScaleCTM(1, -1);
            size1 = DrawTexttnew(context, "พิมพ์ทดสอบ", Align.Center, 0, posY, new CTFont("Arial", sizehead[choosesize]), sizehead[choosesize]);
            Newline(size1, 0);
            size1 = DrawTexttnew(context, "บริษัท ซีเนียร์ซอฟท์ ดีเวลลอปเม้นท์ จำกัด(สำนักงานใหญ่)", Align.Center, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1, 0);

            if (address.Count > 0)
            {
                foreach (var item in address)
                {
                    size1 = DrawTexttnew(context, item, Align.Center, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                    Newline(size1, 0);
                }
            }

            var timezoneslocal = TimeZoneInfo.Local;
            DrawTexttnew(context, tran.tran.TranNo, Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            size1 = DrawTexttnew(context, TimeZoneInfo.ConvertTimeFromUtc(tran.tran.TranDate, timezoneslocal).ToString("dd/MM/yyy HH:mm tt", new CultureInfo("en-US")), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1, 0);

            DrawTexttnew(context, tran.tran.CustomerName, Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1, 0);
           
            var items = ItemDetail(tran.tranDetailItemWithToppings, itemLength[choosesize]);
            int j = 0;
            for (int i = 0; i < items.Count; i++)
            {
                var check = items[i].Substring(0, 2);
                if (!check.Trim().IsNullOrEmpty())
                {
                    DrawTexttnew(context, items[i], Align.Left, 20, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                    size1 = DrawTexttnew(context, tran.tranDetailItemWithToppings[j].tranDetailItem.Amount.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                    j++;
                }
                else
                {
                    size1 = DrawTexttnew(context, items[i].Trim(), Align.Left, 30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                }
                if (i == items.Count - 1)
                {
                    Newline(size1, 2);
                }
                else
                {
                    Newline(size1, 0);
                }
            }

            DrawTexttnew(context, "จำนวนรายการ : " + tran.tranDetailItemWithToppings.Count.ToString(), Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            size1 = DrawTexttnew(context, "รวมจำนวนชิ้น : " + tran.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity).ToString("N0"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1, 0);

            DrawTexttnew(context, "รวมเงิน", Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            DrawTexttnew(context, "รวมทั้งสิ้น", Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            size1 = DrawTexttnew(context, tran.tran.Total.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1, 0);
            
            DrawTexttnew(context, "ภาษีมูลค่าเพิ่ม " + (tran.tran.TaxRate == 0 ? "" : tran.tran.TaxRate?.ToString("#,##0") + "%"), Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            DrawTexttnew(context, "Vat " + (tran.tran.TaxRate == 0 ? "" : tran.tran.TaxRate?.ToString("#,##0") + "%"), Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            size1 = DrawTexttnew(context, tran.tran.TotalVat.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1, 0);
            

            string servicetext = "";
            if (!string.IsNullOrEmpty(tran.tran.FmlServiceCharge))
            {
                var checkservice = tran.tran.FmlServiceCharge.IndexOf('%');
                if (checkservice == -1)
                {
                    var ServiceCharge = tran.tran.FmlServiceCharge;
                    if (Convert.ToDecimal(ServiceCharge) > 0)
                    {
                        servicetext = CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge));
                    }
                }
                else
                {
                    string[] split = tran.tran.FmlServiceCharge.Split('%');
                    var ServiceCharge = split[0];
                    if (Convert.ToDecimal(ServiceCharge) > 0)
                    {
                        servicetext = Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)) + "%";
                    }
                }
            }
            DrawTexttnew(context, "ค่าบริการ " + servicetext, Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            DrawTexttnew(context, "Service " + servicetext, Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            size1 = DrawTexttnew(context, tran.tran.ServiceCharge.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1, 0);
            

            double disMember = 0;
            var tranTradDiscountMember = tran.tranTradDiscounts?.Where(x => x.DiscountType == "PS").FirstOrDefault();
            if (tranTradDiscountMember != null)
            {
                DrawTexttnew(context, "สมาชิก", Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                DrawTexttnew(context, "Member", Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                size1 = DrawTexttnew(context, (tranTradDiscountMember.TradeDiscHaveVat + tranTradDiscountMember.TradeDiscNoneVat).ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                Newline(size1, 0);
            }
            else
            {
                DrawTexttnew(context, "สมาชิก", Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                DrawTexttnew(context, "Member", Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                size1 = DrawTexttnew(context, 0.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                Newline(size1, 0);

            }

            double disMD = 0;
            var tranTradDiscountMD = tran.tranTradDiscounts?.Where(x => x.DiscountType == "MD").FirstOrDefault();
            if (tranTradDiscountMD != null)
            {
                DrawTexttnew(context, "ส่วนลด", Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                DrawTexttnew(context, "Discount", Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                size1 = DrawTexttnew(context, (tranTradDiscountMD.TradeDiscHaveVat + tranTradDiscountMD.TradeDiscNoneVat).ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                Newline(size1, 0);
            }
            else
            {
                DrawTexttnew(context, "ส่วนลด", Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                DrawTexttnew(context, "Discount", Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                size1 = DrawTexttnew(context, 0.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
                Newline(size1, 0);
            }

            DrawTexttnew(context, "รวมทั้งสิ้น", Align.Left, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            DrawTexttnew(context, "Grand Total", Align.Left, cGSize.Width / 2, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            size1 = DrawTexttnew(context, tran.tran.GrandTotal.ToString("#,##0.00"), Align.Right, -30, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1, 2);

            DrawTexttnew(context, "...THANK YOU...", Align.Center, 0, posY, new CTFont("Arial", sizehead[choosesize]), sizehead[choosesize]);
            Newline(size1, 0);

            size1 = DrawTexttnew(context, "Cashier: " + tran.tran.SellerName.ToString(), Align.Center, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1, 2);

            size1 = DrawTexttnew(context, "Powered By Seniorsoft", Align.Center, 0, posY, new CTFont("Arial", sizedetail[choosesize]), sizedetail[choosesize]);
            Newline(size1, 0);

            var height = Newline(size1, 0);
            cGSize.Height = height;

            context.ConvertSizeToUserSpace(cGSize);
            return height;
        }

        public async Task<List<string>> Draw3TestPrint(int size, TranWithDetailsLocal tran)
        {
            try
            {
                string CartDiscount = "";
                double disDiscont = 0.0;
                List<string> data = new List<string>();
                int[] itemLength = { 28, 40 };
                int[] itemLength2 = { 33, 48 };

                string CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                EscPosCommand escPos = new EscPosCommand(size);
                data.Add("พิมพ์ทดสอบ");

                string mername = "";
                string MerchantMame = "บริษัท ซีเนียร์ซอฟท์ ดีเวลลอปเม้นท์ จำกัด(สำนักงานใหญ่)";
                if (ThaiLength(MerchantMame) > itemLength[size - 1])
                {
                    mername = MerchantMame.Substring(0, itemLength[size - 1]);
                }
                else
                {
                    mername = MerchantMame;
                }
                data.Add(mername);

                #region address
                var BranchDetail = new ORM.MerchantDB.Branch()
                {
                    Address = "2991/23-24 อาคารซีเนียร์ซอฟท์ โครงการวิสุทธานี ถนนลาดพร้าว แขวงคลองจั่น เขตบางกะปิ กรุงเทพมหานคร 10240",
                    ProvincesId = 1,
                    AmphuresId = 6,
                    DistrictsId = 100601,
                    Tel = "02-692-5899",
                };
                string branchaddress = "";
                if (BranchDetail != null)
                {
                    branchaddress = await Utils.SetTextAddressAsync(BranchDetail);
                }
                var address = Utils.SplitAddress(branchaddress);
                #endregion
                data.Add(branchaddress);

                data.Add(escPos.ReplaceSpacebar2(tran.tran.TranNo.ToString(), Utils.PrintDateTime(tran.tran.TranDate)));
                data.Add(escPos.ReplaceSpacebar2(tran.tran.CustomerName, ""));
                var items = ItemDetail(tran.tranDetailItemWithToppings, itemLength[size - 1]);
                int j = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    var check = items[i].Substring(0, 2);
                    if (!check.Trim().IsNullOrEmpty())
                    {
                        data.Add(escPos.ReplaceSpacebar2(items[i], tran.tranDetailItemWithToppings[j].tranDetailItem.Amount.ToString("#,##0.00")));
                        j++;
                    }
                    else
                    {
                        data.Add(escPos.ReplaceSpacebar2(items[i], "Customer"));
                    }
                }

                data.Add(escPos.ReplaceSpacebar2("จำนวนรายการ : " + tran.tranDetailItemWithToppings.Count.ToString(), "รวมจำนวนชิ้น : " + tran.tranDetailItemWithToppings.Sum(x2 => x2.tranDetailItem.Quantity).ToString("N0")));


                var total = Convert.ToDouble(tran.tran.GrandTotal);
                data.Add(escPos.ReplaceSpacebar2("รวมเงิน", tran.tran.Total.ToString("#,##0.00")));

                if (tran.tran.TotalVat > 0)
                {
                    data.Add(escPos.ReplaceSpacebar2("ภาษีมูลค่าเพิ่ม " + tran.tran.TaxRate?.ToString("#,##0") + "%", tran.tran.TotalVat.ToString("#,##0.00")));
                }

                if (!string.IsNullOrEmpty(tran.tran.FmlServiceCharge))
                {
                    var checkservice = tran.tran.FmlServiceCharge.IndexOf('%');
                    if (checkservice == -1)
                    {
                        var ServiceCharge = tran.tran.FmlServiceCharge;
                        if (Convert.ToDecimal(ServiceCharge) > 0)
                        {
                            data.Add(escPos.ReplaceSpacebar2("ค่าบริการ " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)), Convert.ToDecimal(tran.tran.ServiceCharge).ToString("#,##0.00")));
                        }
                    }
                    else
                    {
                        string[] split = tran.tran.FmlServiceCharge.Split('%');
                        var ServiceCharge = split[0];
                        if (Convert.ToDecimal(ServiceCharge) > 0)
                        {
                            data.Add(escPos.ReplaceSpacebar2("ค่าบริการ " + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)) + "%", Convert.ToDecimal(tran.tran.ServiceCharge).ToString("#,##0.00")));
                        }
                    }
                }

                double disMember = 0;
                var tranTradDiscountMember = tran.tranTradDiscounts.Where(x => x.DiscountType == "PS").FirstOrDefault();
                if (tranTradDiscountMember != null)
                {
                    var check = tranTradDiscountMember.FmlDiscount.IndexOf('%');
                    if (check == -1)
                    {
                        data.Add(escPos.ReplaceSpacebar2("สมาชิก " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Replace("%", ""))), "-" + Convert.ToDecimal(disMember).ToString("#,##0.00")));

                    }
                    else
                    {
                        data.Add(escPos.ReplaceSpacebar2("สมาชิก " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Remove(check))) + "%", "-" + Convert.ToDecimal(disMember).ToString("#,##0.00")));

                    }
                }

                double discount;
                var tranTradDiscount = tran.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                if (tranTradDiscount != null)
                {
                    var check = tranTradDiscount.FmlDiscount.IndexOf('%');
                    if (check == -1)
                    {
                        CartDiscount = tranTradDiscount.FmlDiscount;
                        discount = Convert.ToDouble(CartDiscount);
                        disDiscont = discount;
                    }
                    else
                    {

                        discount = Convert.ToDouble(tranTradDiscount.FmlDiscount.Remove(check));
                        discount = discount / 100;
                        disDiscont = total * discount;
                    }
                }

                var tranTradDiscount2 = tran.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                if (tranTradDiscount2 != null)
                {
                    data.Add(escPos.ReplaceSpacebar2("ส่วนลด", "-" + Convert.ToDecimal(disDiscont).ToString("#,##0.00")));
                }

                data.Add(escPos.ReplaceSpacebar2("รวมทั้งสิ้น", tran.tran.GrandPayment.ToString("#,##0.00")));


                data.Add("...THANK YOU...");
                data.Add("Cashier: " + tran.tran.SellerName.ToString());
                data.Add("Powered By Seniorsoft");
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}