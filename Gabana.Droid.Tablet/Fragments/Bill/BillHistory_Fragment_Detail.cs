using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Print;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AutoMapper;
using Gabana.Droid.Tablet.Adapter.Bill;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.ClassStructure;
using Gabana.ShareSource.Manage;
using iTextSharp.text.rtf.list;
using Java.Util;
using LinqToDB.Common;
using LinqToDB.SqlQuery;
using Newtonsoft.Json;
using Org.Apache.Commons.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;
using static Android.Provider.ContactsContract.CommonDataKinds;

namespace Gabana.Droid.Tablet.Fragments.Bill
{
    public class BillHistory_Fragment_Detail : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static BillHistory_Fragment_Detail NewInstance()
        {
            BillHistory_Fragment_Detail frag = new BillHistory_Fragment_Detail();
            return frag;
        }
        public static BillHistory_Fragment_Detail fragment_detail;
        View view;
        string LoginType, Username;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.billhistory_fragment_detail, container, false);

            try
            {
                fragment_detail = this;
                assets = this.Activity.Assets;
                LoginType = Preferences.Get("LoginType", "");
                Username = Preferences.Get("User", "");
                txtshowlocal = Preferences.Get("Language", "");
                CombineUI();
                SetUIEvent();
                tranhistory = DataCashing.billHistory;
                branchID = BillHistory_Fragment_Main.branchID;               
                return view;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        FrameLayout lnBack, lnPrintCounter;
        TextView txtPrintCounter, txtTranNo;
        FrameLayout lnBill;
        ImageView imgLogoMerchant;
        TextView textNameMerchant, textTranNo, textDate, textCustomerName;
        RecyclerView rcvListItem;
        TextView textTotalItems, textUnit, textSubtotalAmount;
        FrameLayout lnMemberDiscount;
        TextView textMemberDiscount, textMemberDiscountAmount;
        FrameLayout lnVat;
        TextView textVat, textVatAmount;
        FrameLayout lnDiscount;
        TextView textDiscount, textDiscountAmount;
        FrameLayout lnService;
        TextView textService, textServiceAmount,textTotal, textTotalAmount, textCash, textCashAmount;
        RecyclerView rcvListPayment;
        FrameLayout lnChange;
        TextView textChange, textChangeAmount, textCashier, textCashierName;
        FrameLayout lnRemark;
        TextView textRemark, textRemarkDetail;
        LinearLayout lnReceiptImage;
        ImageView imgReceiptImage;
        RelativeLayout lnStatusVoid;
        LinearLayout lnShowButton, lnShowPrint, lnShowPDF, lnShowEmail, lnShowShare, lnShowVoid;
        FrameLayout lnPrint, lnPDF, lnEmail, lnShare , lnVoid;


        private void CombineUI()
        {
            lnBack = view.FindViewById<FrameLayout>(Resource.Id.lnBack);
            lnPrintCounter = view.FindViewById<FrameLayout>(Resource.Id.lnPrintCounter); 
            txtPrintCounter = view.FindViewById<TextView>(Resource.Id.txtPrintCounter);
            txtTranNo = view.FindViewById<TextView>(Resource.Id.txtTranNo);
            lnBill = view.FindViewById<FrameLayout>(Resource.Id.lnBill);
            imgLogoMerchant = view.FindViewById<ImageView>(Resource.Id.imgLogoMerchant);
            textNameMerchant = view.FindViewById<TextView>(Resource.Id.textNameMerchant);
            textTranNo = view.FindViewById<TextView>(Resource.Id.textTranNo);
            textDate = view.FindViewById<TextView>(Resource.Id.textDate);
            textCustomerName = view.FindViewById<TextView>(Resource.Id.textCustomerName);
            rcvListItem = view.FindViewById<RecyclerView>(Resource.Id.rcvListItem);
            textTotalItems = view.FindViewById<TextView>(Resource.Id.textTotalItems);
            textUnit = view.FindViewById<TextView>(Resource.Id.textUnit);
            textSubtotalAmount = view.FindViewById<TextView>(Resource.Id.textSubtotalAmount);
            lnMemberDiscount = view.FindViewById<FrameLayout>(Resource.Id.lnMemberDiscount);
            textMemberDiscount = view.FindViewById<TextView>(Resource.Id.textMemberDiscount);
            textMemberDiscountAmount = view.FindViewById<TextView>(Resource.Id.textMemberDiscountAmount);
            lnVat = view.FindViewById<FrameLayout>(Resource.Id.lnVat);
            textVat = view.FindViewById<TextView>(Resource.Id.textVat);
            textVatAmount = view.FindViewById<TextView>(Resource.Id.textVatAmount);
            lnDiscount = view.FindViewById<FrameLayout>(Resource.Id.lnDiscount);
            textDiscount = view.FindViewById<TextView>(Resource.Id.textDiscount);
            textDiscountAmount = view.FindViewById<TextView>(Resource.Id.textDiscountAmount);
            lnService = view.FindViewById<FrameLayout>(Resource.Id.lnService);
            textService = view.FindViewById<TextView>(Resource.Id.textService);
            textServiceAmount = view.FindViewById<TextView>(Resource.Id.textServiceAmount);
            textTotal = view.FindViewById<TextView>(Resource.Id.textTotal);
            textTotalAmount = view.FindViewById<TextView>(Resource.Id.textTotalAmount);
            textCash = view.FindViewById<TextView>(Resource.Id.textCash);
            textCashAmount = view.FindViewById<TextView>(Resource.Id.textCashAmount);
            rcvListPayment = view.FindViewById<RecyclerView>(Resource.Id.rcvListPayment);
            lnChange = view.FindViewById<FrameLayout>(Resource.Id.lnChange);
            textChange = view.FindViewById<TextView>(Resource.Id.textChange);
            textChangeAmount = view.FindViewById<TextView>(Resource.Id.textChangeAmount);
            textCashier = view.FindViewById<TextView>(Resource.Id.textCashier);
            textCashierName = view.FindViewById<TextView>(Resource.Id.textCashierName);
            lnRemark = view.FindViewById<FrameLayout>(Resource.Id.lnRemark);
            textRemark = view.FindViewById<TextView>(Resource.Id.textRemark);
            textRemarkDetail = view.FindViewById<TextView>(Resource.Id.textRemarkDetail);
            lnReceiptImage = view.FindViewById<LinearLayout>(Resource.Id.lnReceiptImage);
            imgReceiptImage = view.FindViewById<ImageView>(Resource.Id.imgReceiptImage);
            lnStatusVoid = view.FindViewById<RelativeLayout>(Resource.Id.lnStatusVoid);
            lnShowButton = view.FindViewById<LinearLayout>(Resource.Id.lnShowButton);
            lnShowPrint = view.FindViewById<LinearLayout>(Resource.Id.lnShowPrint);
            lnPrint = view.FindViewById<FrameLayout>(Resource.Id.lnPrint);
            lnShowPDF = view.FindViewById<LinearLayout>(Resource.Id.lnShowPDF);
            lnPDF = view.FindViewById<FrameLayout>(Resource.Id.lnPDF);
            lnShowEmail = view.FindViewById<LinearLayout>(Resource.Id.lnShowEmail);
            lnEmail = view.FindViewById<FrameLayout>(Resource.Id.lnEmail);
            lnShowShare = view.FindViewById<LinearLayout>(Resource.Id.lnShowShare);
            lnShare = view.FindViewById<FrameLayout>(Resource.Id.lnShare);
            lnShowVoid = view.FindViewById<LinearLayout>(Resource.Id.lnShowVoid);
            lnVoid = view.FindViewById<FrameLayout>(Resource.Id.lnVoid);
        }

        public override async void OnResume()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                base.OnResume();

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }

                //if (!IsVisible)
                //{
                //    return;
                //}
                txtshowlocal = Preferences.Get("Language", "");
                UiNewBillHistory();
                tranhistory = DataCashing.billHistory;
                branchID = BillHistory_Fragment_Main.branchID;

                if (tranhistory != null)
                {
                    await ShowBillDetail();
                }

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading?.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Update at edit Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        int lengThName = 28, printCounter = 0;
        string SubTotal = "";
        double total = 0.00;
        string CartDiscount = "";
        double disDiscont = 0.0;
        string txtshowlocal = string.Empty;
        private async Task ShowBillDetail()
        {
            try
            {
                TranWithDetailsLocal tranWithDetails;
                if (DataCashing.CheckNet)
                {
                    if (tranhistory.TypeOfflineOrOnline == 'O' && tranhistory.FWaiting == 0)
                    {
                        tranWithDetails = await GetOnlineHistoryDetail();
                    }
                    else
                    {
                        tranWithDetails = await GetOfflineHistoryDetail();
                        lnShowVoid.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    tranWithDetails = await GetOfflineHistoryDetail();
                }

                if (tranWithDetails == null)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notdata), ToastLength.Short).Show();
                    return;
                }

                printCounter = (int)tranWithDetails.tran.PrintCounter + (int)tranWithDetails.tran.PrintCounterLocal;

                txtTranNo.Text = tranWithDetails.tran.TranNo;
                textNameMerchant.Text = DataCashingAll.MerchantLocal.Name;
                //offline Image     
                var path = DataCashingAll.MerchantLocal.LogoLocalPath;
                if (!string.IsNullOrEmpty(path))
                {
                    Android.Net.Uri uri = Android.Net.Uri.Parse(path);
                    imgLogoMerchant.SetImageURI(uri);
                }
                else
                {
                    imgLogoMerchant.SetBackgroundResource(Resource.Mipmap.LogoDefault);
                }
                textTranNo.Text = tranWithDetails.tran.TranNo;
                var timezoneslocal = TimeZoneInfo.Local;
                var date = tranWithDetails.tran.TranDate;
                textDate.Text = TimeZoneInfo.ConvertTimeFromUtc(date, timezoneslocal).ToString("dd/MM/yyy HH:mm tt", new CultureInfo("en-US"));
                //textDate.Text = TimeZoneInfo.ConvertTimeFromUtc(date, timezoneslocal).ToString("dd/MM/yyy", new CultureInfo("en-US")) + " " + tranWithDetails.tran.TranDate.ToString("HH:mm tt");
                textCustomerName.Text = tranWithDetails.tran.CustomerName;

                Bill_Adapter_Item bill_adapter_item = new Bill_Adapter_Item(tranWithDetails);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListItem.SetLayoutManager(gridLayoutManager);
                rcvListItem.HasFixedSize = true;
                rcvListItem.SetItemViewCacheSize(100);
                rcvListItem.SetAdapter(bill_adapter_item);

                if (DataCashing.Language == "th")
                {
                    textUnit.Text = " ";
                }
                else
                {
                    if (tranWithDetails.tranDetailItemWithToppings.Count == 1)
                    {
                        textUnit.Text = " item";
                    }
                    else
                    {
                        textUnit.Text = " items";
                    }
                }

                //จำนวนสินค้าทั้งหมด  
                textTotalItems.Text = " " + (int)tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity);
                textCashierName.Text = tranWithDetails.tran.SellerName?.ToString();
                SubTotal = Utils.DisplayDecimal(tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Amount));
                textSubtotalAmount.Text = SubTotal;
                txtPrintCounter.Text = (tranWithDetails.tran.PrintCounter + tranWithDetails.tran.PrintCounterLocal).ToString("#,##0");

                if (tranWithDetails.tranDetailItemWithToppings.Count > 0)
                {
                    total = Convert.ToDouble(tranWithDetails.tran.GrandTotal);

                    double disMember = 0;
                    var tranTradDiscountMember = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "PS").FirstOrDefault();
                    if (tranTradDiscountMember != null)
                    {
                        var check = tranTradDiscountMember.FmlDiscount.IndexOf('%');
                        if (check == -1)
                        {
                            textMemberDiscount.Text = "Member " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Replace("%", "")));
                        }
                        else
                        {
                            textMemberDiscount.Text = "Member " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Remove(check))) + "%";
                        }
                        lnMemberDiscount.Visibility = ViewStates.Visible;
                        disMember = Convert.ToDouble(tranTradDiscountMember.TradeDiscNoneVat + tranTradDiscountMember.TradeDiscHaveVat);
                        textMemberDiscountAmount.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disMember));
                    }
                    else
                    {
                        lnMemberDiscount.Visibility = ViewStates.Gone;
                    }

                    //Discount From Manual
                    double discount;
                    var tranTradDiscount = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                    if (tranTradDiscount != null)
                    {
                        lnDiscount.Visibility = ViewStates.Visible;

                        var check = tranTradDiscount.FmlDiscount.IndexOf('%');
                        if (check == -1)
                        {
                            CartDiscount = tranTradDiscount.FmlDiscount;
                            discount = Convert.ToDouble(CartDiscount);
                            textDiscount.Text = GetString(Resource.String.cart_activity_discount) + " " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscount.FmlDiscount));
                            disDiscont = discount;
                        }
                        else
                        {

                            discount = Convert.ToDouble(tranTradDiscount.FmlDiscount.Remove(check));
                            textDiscount.Text = GetString(Resource.String.cart_activity_discount) + " " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscount.FmlDiscount.Remove(check))) + "%";
                            discount = discount / 100;
                            disDiscont = total * discount;
                        }
                        textDiscountAmount.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disDiscont));
                    }
                    else
                    {
                        lnDiscount.Visibility = ViewStates.Gone;
                    }

                    //Vat
                    string getvat = Utils.DisplayDouble(tranWithDetails.tran.TaxRate == null ? 0 : tranWithDetails.tran.TaxRate.Value);
                    if (tranWithDetails.tran.TaxRate == 0)
                    {
                        lnVat.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnVat.Visibility = ViewStates.Visible;
                        textVat.Text = GetString(Resource.String.vat) + " " + getvat + "%";
                        textVatAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);
                    } 

                    textTotalAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal);

                    //Payment Cash
                    decimal payment = 0;
                    foreach (var item in tranWithDetails.tranPayments)
                    {
                        payment += item.PaymentAmount;
                    }

                    var listPayment = tranWithDetails.tranPayments;
                    List<PaymentTypeAmount> paymenttypeamout = new List<PaymentTypeAmount>();
                    foreach (var item in tranWithDetails.tranPayments)
                    {
                        var i = paymenttypeamout.FindIndex(x => x.TypePayment == item.PaymentType);
                        if (i == -1)
                        {
                            PaymentTypeAmount newpayment = new PaymentTypeAmount
                            {
                                TypePayment = item.PaymentType,
                                DetailType = Utils.SetPaymentName(item.PaymentType),
                                amount = item.PaymentAmount
                            };
                            paymenttypeamout.Add(newpayment);
                        }
                        else
                        {
                            paymenttypeamout[i].amount += item.PaymentAmount;
                        }
                    }

                    Bill_Adapter_Payment receipt_Adapter_Cash = new Bill_Adapter_Payment(paymenttypeamout);
                    GridLayoutManager gridLayoutManager2 = new GridLayoutManager(this.Activity, 1, 1, false);
                    rcvListPayment.SetLayoutManager(gridLayoutManager2);
                    rcvListPayment.HasFixedSize = true;
                    rcvListPayment.SetItemViewCacheSize(20);
                    rcvListPayment.SetAdapter(receipt_Adapter_Cash);

                    textCashAmount.Text = Utils.DisplayDecimal(payment);
                    textChangeAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.Change);

                    //Service charge
                    string ServiceCharge;
                    if (string.IsNullOrEmpty(tranWithDetails.tran.FmlServiceCharge))
                    {
                        lnService.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        if (tranWithDetails.tran.FmlServiceCharge == "0" | tranWithDetails.tran.FmlServiceCharge == "0.00" | tranWithDetails.tran.FmlServiceCharge == "0.0000")
                        {
                            lnService.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            lnService.Visibility = ViewStates.Visible;
                            var checkservice = tranWithDetails.tran.FmlServiceCharge.IndexOf('%');
                            if (checkservice == -1)
                            {
                                ServiceCharge = tranWithDetails.tran.FmlServiceCharge;
                                textService.Text = GetString(Resource.String.servicecharge) + " " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge));
                            }
                            else
                            {
                                string[] split = tranWithDetails.tran.FmlServiceCharge.Split('%');
                                ServiceCharge = split[0];
                                textService.Text = GetString(Resource.String.servicecharge) + " " + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)) + "%";
                            }
                            textServiceAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.ServiceCharge);
                        }                        
                    }

                    if (string.IsNullOrEmpty(tranWithDetails.tran.Comments))
                    {
                        lnRemark.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        textRemarkDetail.Text = tranWithDetails.tran.Comments?.ToString();
                        lnRemark.Visibility = ViewStates.Visible;
                    }
                }

                if (tranhistory.fCancel == 1)
                {
                    lnStatusVoid.Visibility = ViewStates.Visible;
                    lnShowButton.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnStatusVoid.Visibility = ViewStates.Gone;
                    lnShowButton.Visibility = ViewStates.Visible;
                }               
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowBillDetail at Bill Detaial");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        //Get TranWithDetails Online
        async Task<TranWithDetailsLocal> GetOnlineHistoryDetail()
        {
            try
            {
                Gabana3.JAM.Trans.TranWithDetails tranDetails = new Gabana3.JAM.Trans.TranWithDetails();
                var datetrans = Utils.ChangeDateTime(tranhistory.tranDate);
                string tranNo = tranhistory.tranNo;
                tranDetails = await GabanaAPI.GetDataTranDetail(Convert.ToInt32(branchID), tranNo, datetrans);
                if (tranDetails == null)
                {
                    tranDetails = new Gabana3.JAM.Trans.TranWithDetails();
                }

                var config = new MapperConfiguration(cfg =>
                {
                    //struct ของ table
                    cfg.CreateMap<Gabana3.JAM.Trans.TranWithDetails, Model.TranWithDetailsLocal>();
                    cfg.CreateMap<ORM.Period.Tran, Tran>();
                    cfg.CreateMap<ORM.Period.TranDetailItem, TranDetailItemNew>();
                    cfg.CreateMap<ORM.Period.TranDetailItemTopping, TranDetailItemTopping>();
                    cfg.CreateMap<ORM.Period.TranPayment, TranPayment>();
                    cfg.CreateMap<ORM.Period.TranTradDiscount, TranTradDiscount>();
                });


                var Imapper = config.CreateMapper();

                //Tran
                tranWithDetailsLocal = new TranWithDetailsLocal();

                tranWithDetailsLocal.tran = Imapper.Map<ORM.Period.Tran, Tran>(tranDetails.tran);

                //TranDetailItemWithTopping
                TranDetailItemNew tranDetailItem = new TranDetailItemNew();
                Model.TranDetailItemWithTopping tranDetailItemWithTopping = new TranDetailItemWithTopping();
                List<Model.TranDetailItemWithTopping> lsttrandetailTopping = new List<TranDetailItemWithTopping>();
                List<TranDetailItemTopping> lstitemToppings = new List<TranDetailItemTopping>();

                //TranDetailWithTopping
                foreach (var item in tranDetails.tranDetailItemWithToppings)
                {
                    //DetailItem
                    if (item.tranDetailItem != null)
                    {
                        tranDetailItem = Imapper.Map<ORM.Period.TranDetailItem, TranDetailItemNew>(item.tranDetailItem);
                    }

                    //ListDetailToppping
                    if (item.tranDetailItemToppings.Count != 0)
                    {
                        lstitemToppings = Imapper.Map<List<ORM.Period.TranDetailItemTopping>, List<TranDetailItemTopping>>(item.tranDetailItemToppings);
                    }

                    tranDetailItemWithTopping = new TranDetailItemWithTopping();
                    tranDetailItemWithTopping.tranDetailItem = tranDetailItem;
                    tranDetailItemWithTopping.tranDetailItemToppings = lstitemToppings;
                    lsttrandetailTopping.Add(tranDetailItemWithTopping);

                    lstitemToppings = new List<TranDetailItemTopping>();
                }
                tranWithDetailsLocal.tranDetailItemWithToppings.Clear();
                tranWithDetailsLocal.tranDetailItemWithToppings.AddRange(lsttrandetailTopping);

                //TranPayment
                TranPayment payment = new TranPayment();
                List<TranPayment> lsttranpayment = new List<TranPayment>();
                foreach (var item in tranDetails.tranPayments)
                {
                    if (item != null)
                    {
                        payment = Imapper.Map<ORM.Period.TranPayment, TranPayment>(item);
                        lsttranpayment.Add(payment);
                    }
                }
                tranWithDetailsLocal.tranPayments = lsttranpayment;

                //TranTradDiscount
                TranTradDiscount trandiscount = new TranTradDiscount();
                List<TranTradDiscount> lsttrandiscount = new List<TranTradDiscount>();
                foreach (var itemDiscount in tranDetails.tranTradDiscounts)
                {
                    if (itemDiscount != null)
                    {
                        trandiscount = Imapper.Map<ORM.Period.TranTradDiscount, TranTradDiscount>(itemDiscount);
                        lsttrandiscount.Add(trandiscount);
                    }
                }
                tranWithDetailsLocal.tranTradDiscounts = lsttrandiscount;

                var index = tranWithDetailsLocal.tranPayments.FindLastIndex(x => !string.IsNullOrEmpty(x.PicturePath));
                if (index != -1)
                {
                    //ประวัติบิล แสดงรูปภาพจากออนไลน์
                    var path = tranWithDetailsLocal.tranPayments[index].PicturePath;
                    Utils.SetImage(imgReceiptImage, path);
                }
                else
                {
                    lnReceiptImage.Visibility = ViewStates.Gone;
                }
                return tranWithDetailsLocal;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetOnlineHistoryDetail at Bill Detaial");
                return new TranWithDetailsLocal();
            }
        }

        //Get TranWithDetails Offline
        TransManage transManage;
        TranDetailItemManage tranDetailItemManage;
        TranPaymentManage tranPaymentManage;
        TranTradDiscountManage tranTradDiscountManage;
        TranDetailItemToppingManage toppingManage;

        async Task<TranWithDetailsLocal> GetOfflineHistoryDetail()
        {
            try
            {
                // CultureInfo.CurrentCulture = new CultureInfo("en-US");
                List<TranWithDetailsLocal> lst = new List<TranWithDetailsLocal>();
                List<TranDetailItem> tranDetail = new List<TranDetailItem>();
                List<TranDetailItemTopping> tranDetailTopping = new List<TranDetailItemTopping>();
                List<TranPayment> tranPayment = new List<TranPayment>();
                List<TranTradDiscount> tranDiscount = new List<TranTradDiscount>();
                Gabana.Model.TranDetailItemWithTopping detailItemWithTopping = new TranDetailItemWithTopping();
                List<TranDetailItemWithTopping> lstdetailItemWithTopping = new List<TranDetailItemWithTopping>();

                transManage = new TransManage();
                tranDetailItemManage = new TranDetailItemManage();
                tranPaymentManage = new TranPaymentManage();
                tranTradDiscountManage = new TranTradDiscountManage();
                toppingManage = new TranDetailItemToppingManage();

                tranWithDetailsLocal = new TranWithDetailsLocal();
                var Datatran = await transManage.GetTran(DataCashingAll.MerchantId, Convert.ToInt32(branchID), tranhistory.tranNo);

                tranDetail = await tranDetailItemManage.GetTranDetailItem(DataCashingAll.MerchantId, Convert.ToInt32(branchID), Datatran.TranNo);
                tranPayment = await tranPaymentManage.GetTranPayment(DataCashingAll.MerchantId, Convert.ToInt32(branchID), Datatran.TranNo);
                tranDiscount = await tranTradDiscountManage.GetTranTradDiscount(DataCashingAll.MerchantId, Convert.ToInt32(branchID), Datatran.TranNo);

                tranWithDetailsLocal.tran = Datatran;

                foreach (var item in tranDetail)
                {
                    tranDetailTopping = await toppingManage.GetTranDetailItemTopping(DataCashingAll.MerchantId, Convert.ToInt32(branchID), Datatran.TranNo, (int)item.DetailNo); // รอแก้ไข TranDetailItemTopping

                    TranDetailItemNew detailItem = new TranDetailItemNew()
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
                        ItemPrice = item.ItemPrice,
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
                    };
                    detailItemWithTopping = new TranDetailItemWithTopping()
                    {
                        tranDetailItem = detailItem,
                        tranDetailItemToppings = tranDetailTopping
                    };
                    tranWithDetailsLocal.tranDetailItemWithToppings.Add(detailItemWithTopping);
                }

                tranWithDetailsLocal.tranPayments = tranPayment;
                tranWithDetailsLocal.tranTradDiscounts = tranDiscount;

                return tranWithDetailsLocal;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetOfflineHistoryDetail at Bill Detaial");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return new TranWithDetailsLocal();
            }
        }

        public static TransHistoryNew tranhistory;
        public static void SetTranHistory(TransHistoryNew t)
        {
            tranhistory = t;
        }

        TranWithDetailsLocal tranWithDetailsLocal;
        string CURRENCYSYMBOLS;
        static string branchID;
        //FrameLayout lnPrint, lnPDF, lnEmail, lnShare, lnVoid;

        private void SetUIEvent()
        {
            lnStatusVoid.Visibility = ViewStates.Gone;
            lnShowButton.Visibility = ViewStates.Visible;
            lnPrintCounter.Click += LnPrintCounter_Click;
            lnShowPrint.Click += LnPrint_Click;
            lnShowPDF.Click += LnPDF_Click;
            lnShowEmail.Click += LnEmail_Click;
            lnShowShare.Click += LnShare_Click;
            lnShowVoid.Click += LnVoid_Click;
            lnBack.Click += LnBack_Click;

            textNameMerchant.Text = string.Empty;
            textTranNo.Text = string.Empty;
            textCustomerName.Text = string.Empty;
            textMemberDiscountAmount.Text = string.Empty;
            textVatAmount.Text = string.Empty;
            textDiscountAmount.Text = string.Empty;
            textTotalAmount.Text = string.Empty;
            textCashAmount.Text = string.Empty;
            textChangeAmount.Text = string.Empty;
            textCashierName.Text = string.Empty;

            lnBill.SetBackgroundColor(Android.Graphics.Color.White);

            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;
            if (CURRENCYSYMBOLS == null) CURRENCYSYMBOLS = "฿";

            branchID = BillHistory_Fragment_Main.branchID;
        }

        private void LnPrintCounter_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this.Activity, GetString(Resource.String.receipt_activity_reprint) + txtPrintCounter.Text, ToastLength.Long).Show();
        }

        private async void LnBack_Click(object sender, EventArgs e)
        {            
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnBill, "billhistory", "default");
        }
        
        private async void LnVoid_Click(object sender, EventArgs e)
        {
            try
            {
                lnShowVoid.Enabled = false;
                //Check Role ของผู้ที่ต้องการ void bill
                LoginType = Preferences.Get("LoginType", "");
                bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "void");
                if (check)
                {
                    try
                    {
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.void_dialog_main.ToString();
                        bundle.PutString("message", myMessage);
                        Void_Dialog_Main void_Dialog = Void_Dialog_Main.NewInstance();
                        void_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                    }
                    catch (Exception ex)
                    {
                        lnShowVoid.Enabled = true;
                        await TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("LnVoid_Click at Bill Detail : owner");
                        Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                }
                lnShowVoid.Enabled = true;

            }
            catch (Exception ex)
            {
                lnShowVoid.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnVoid_Click at Bill Detail");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void LnShare_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                lnShowShare.Enabled = false;


                //await CreateBitmap();
                toGrayscale();
                await SaveandShare("Receipt " + tranWithDetailsLocal.tran.TranNo, "Receipt " + tranWithDetailsLocal.tran.TranNo + " " + Utils.ShowDate(tranWithDetailsLocal.tran.TranDate), "Receipt " + tranWithDetailsLocal.tran.TranNo);

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                lnShowShare.Enabled = true;

            }
            catch (Exception)
            {
                dialogLoading.Dismiss();
            }
        }
        public Task SaveandShare(string title, string message, string filename)
        {
            try
            {
                var fullpath = filename + ".png";

                // Get my downloads path
                string filePath = DataCashingAll.PathImageBill;

                // Combine with filename
                string fullName = System.IO.Path.Combine(filePath, fullpath);

                // If the file exist on device delete it
                if (File.Exists(fullName))
                {
                    // Note: In the second run of this method, the file exists
                    File.Delete(fullName);
                }

                using (var os = new System.IO.FileStream(fullName, System.IO.FileMode.CreateNew))
                {
                    bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, os);
                }
                bitmap.Dispose();

                Java.IO.File file = new Java.IO.File(fullName);

                Android.Net.Uri imageUri;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    imageUri = Android.Support.V4.Content.FileProvider.GetUriForFile(Application.Context, Application.Context.ApplicationContext.PackageName + ".fileProvider", file);
                }
                else
                {
                    imageUri = Android.Net.Uri.FromFile(file);
                }

                var intent = new Intent();
                intent.SetAction(Intent.ActionSend);
                intent.SetType("image/*");
                intent.PutExtra(Intent.ExtraText, message);
                intent.PutExtra(Intent.ExtraSubject, title ?? string.Empty);
                intent.PutExtra(Intent.ExtraStream, imageUri);
                intent.SetFlags(ActivityFlags.GrantReadUriPermission);
                intent.SetFlags(ActivityFlags.ClearTop);
                intent.SetFlags(ActivityFlags.NewTask);
                intent.SetFlags(ActivityFlags.ClearWhenTaskReset);

                var chooserIntent = Intent.CreateChooser(intent, title);
                this.StartActivity(chooserIntent);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Show at Bill Detail");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return Task.FromResult(false);
            }
        }

        public void toGrayscale()
        {
            int width = lnBill.Width;
            int height = lnBill.Height;

            Android.Graphics.Bitmap bmpGrayscale = Android.Graphics.Bitmap.CreateBitmap(width, height, Android.Graphics.Bitmap.Config.Argb8888);

            Android.Graphics.Canvas canvas = new Android.Graphics.Canvas(bmpGrayscale);
            Android.Graphics.Paint paint = new Android.Graphics.Paint();

            ColorMatrix cm = new Android.Graphics.ColorMatrix();
            cm.SetSaturation(0);
            paint.SetColorFilter(new ColorMatrixColorFilter(cm));
            canvas.DrawBitmap(bitmap, width, height, paint);

            lnBill.Draw(canvas);

        }

        private async void LnEmail_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                await CreateBitmap();
                await SaveandShowEmail("Receipt " + tranWithDetailsLocal.tran.TranNo, "Receipt " + tranWithDetailsLocal.tran.TranNo + " " + Utils.ShowDate(tranWithDetailsLocal.tran.TranDate), "Receipt " + tranWithDetailsLocal.tran.TranNo);

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception)
            {
                dialogLoading.Dismiss();
            }
        }
        public async Task SaveandShowEmail(string title, string message, string filename)
        {
            try
            {
                var fullpath = filename + ".png";
                string filePath = DataCashingAll.PathImageBill;
                string fullName = filePath + fullpath;
                if (File.Exists(fullName))
                {
                    File.Delete(fullName);
                }

                using (var os = new System.IO.FileStream(fullName, System.IO.FileMode.CreateNew))
                {
                    bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, os);
                }

                Java.IO.File file = new Java.IO.File(fullName);

                //string PathFile = string.Empty;
                Android.Net.Uri uriAttachments;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    uriAttachments = Android.Support.V4.Content.FileProvider.GetUriForFile(Application.Context, Application.Context.ApplicationContext.PackageName + ".fileProvider", file);
                }
                else
                {
                    uriAttachments = Android.Net.Uri.FromFile(file);
                }

                string EmailAddress = "";
                if (tranWithDetailsLocal.tran.SysCustomerID != 999)
                {
                    CustomerManage customerManage = new CustomerManage();
                    Customer customer = new Customer();
                    var listCustomer = new List<Customer>();
                    listCustomer = await customerManage.GetAllCustomer();
                    customer = listCustomer.Where(x => x.SysCustomerID == tranWithDetailsLocal.tran.SysCustomerID).FirstOrDefault();
                    if (!string.IsNullOrEmpty(customer?.EMail))
                    {
                        EmailAddress = customer.EMail;
                    }
                }

                var data = new EmailMessage
                {
                    Subject = title,
                    Body = message,
                    To = new List<string> { EmailAddress },
                };

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N) // Android 7 ขึ้นไป  
                {
                    data.Attachments.Add(new EmailAttachment(file.AbsolutePath));
                }
                else // Android 6  
                {
                    data.Attachments.Add(new EmailAttachment(uriAttachments.Path));
                }
                await Xamarin.Essentials.Email.ComposeAsync(data);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowEmail at Bill Detail");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void LnPDF_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                await CreateBitmap();
                await CreatePDF("Receipt " + tranWithDetailsLocal.tran.TranNo);

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception)
            {
                dialogLoading.Dismiss();
            }
        }
        public Task CreatePDF(string filename)
        {
            try
            {   //Create image 
                var fullpathpng = filename + ".png";
                string filePathpng = DataCashingAll.PathImageBill;
                //string fullNamepng = Path.Combine(filePathpng, fullpathpng);
                string fullNamepng = filePathpng + fullpathpng;
                if (File.Exists(fullNamepng))
                {
                    File.Delete(fullNamepng);
                }

                using (var os = new System.IO.FileStream(fullNamepng, System.IO.FileMode.CreateNew))
                {
                    bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, os);
                }

                var ImageInfo = bitmap.GetBitmapInfo();

                //Create path PDF
                var fullpath = filename + ".pdf";
                string filePath = DataCashingAll.PathImageBill;
                string fullName = filePath + fullpath;

                if (File.Exists(fullName))
                {
                    File.Delete(fullName);
                }

                int newWidth, newHeight;
                newWidth = (int)ImageInfo.Width / 2;
                newHeight = (int)ImageInfo.Height / 2;

                iTextSharp.text.Document document = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(newWidth, newHeight));
                using (var stream = new FileStream(fullName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    iTextSharp.text.pdf.PdfWriter.GetInstance(document, stream);
                    document.Open();
                    using (var imageStream = new FileStream(fullNamepng, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var image = iTextSharp.text.Image.GetInstance(imageStream);
                        image.ScaleAbsolute(newWidth, newHeight);
                        image.SetAbsolutePosition(0, 0);
                        document.Add(image);
                    }
                    document.Close();
                }

                Java.IO.File file = new Java.IO.File(fullName);

                Android.Net.Uri pdfUri;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    pdfUri = Android.Support.V4.Content.FileProvider.GetUriForFile(Application.Context, Application.Context.ApplicationContext.PackageName + ".fileProvider", file);
                }
                else
                {
                    pdfUri = Android.Net.Uri.FromFile(file);
                }

                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(pdfUri, "application/pdf");
                intent.SetFlags(ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                intent.SetFlags(ActivityFlags.NoHistory);
                intent.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);

                var chooserIntent = Intent.CreateChooser(intent, "Open File");
                try
                {
                    this.StartActivity(chooserIntent);
                }
                catch (ActivityNotFoundException)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nonapplication), ToastLength.Short).Show();
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OpenFile at Bill Detail");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return Task.FromResult(false);
            }
        }
        Android.Graphics.Bitmap bitmap;
        async Task CreateBitmap()
        {
            try
            {
                int width = lnBill.Width;
                int height = lnBill.Height;

                bitmap = Android.Graphics.Bitmap.CreateBitmap(width, height, Android.Graphics.Bitmap.Config.Argb8888);
                Android.Graphics.Canvas canvas = new Android.Graphics.Canvas(bitmap);
                canvas.DrawColor(Android.Graphics.Color.White);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    canvas.DrawBitmap(bitmap, width, height, null);
                }
                else
                {
                    canvas.SetViewport(width, height);
                }
                lnBill.Draw(canvas);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CreateBitmap at Bill Detaial");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private async void LnPrint_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                lnShowPrint.Enabled = false;
                var perSetting = Preferences.Get("Setting", "");
                if (perSetting != "")
                {
                    var settingPrinter = JsonConvert.DeserializeObject<SettingPrinter>(perSetting);
                    DataCashingAll.setting = settingPrinter;

                    await Print(tranWithDetailsLocal);
                    lnShowPrint.Enabled = true;
                }
                else
                {
                    lnShowPrint.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.settingprinter), ToastLength.Short).Show();
                }
                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                lnShowPrint.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnPrint_Click at Bill Detaial");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        ORM.MerchantDB.Branch BranchDetail = new ORM.MerchantDB.Branch();
        public static AssetManager assets;
        static byte[] bytesData;
        internal async Task Print(TranWithDetailsLocal t)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    lnShowPrint.Enabled = false;
                });

                TranWithDetailsLocal tran = t;
                if (DataCashingAll.setting?.TYPEPAGE.Substring(0, 2) == "80")
                {
                    lengThName = 35;
                }
                List<List<KeyValuePair<string, string>>> items = new List<List<KeyValuePair<string, string>>>();
                for (int i = 0; i < tran.tranDetailItemWithToppings.Count; i++)
                {
                    if (tran.tranDetailItemWithToppings[i].tranDetailItem.ItemName.Length > lengThName)
                    {
                        var itemNames = Utils.SplitItemName(lengThName, tran.tranDetailItemWithToppings[i].tranDetailItem.ItemName);

                        items.Add(new List<KeyValuePair<string, string>>()
                        {
                        new KeyValuePair<string, string>("@QuantityItem",tran.tranDetailItemWithToppings[i].tranDetailItem.Quantity.ToString("#,##0") + "x " ),
                        new KeyValuePair<string, string>("@ItemName",itemNames[0]),
                        new KeyValuePair<string, string>("@ItemPrice",tran.tranDetailItemWithToppings[i].tranDetailItem.Amount.ToString("#,##0.00"))
                        });
                        items.Add(new List<KeyValuePair<string, string>>()
                        {
                        new KeyValuePair<string, string>("@QuantityItem"," " ),
                        new KeyValuePair<string, string>("@ItemName",itemNames[1]),
                        new KeyValuePair<string, string>("@ItemPrice"," ")
                        });
                    }
                    else
                    {
                        items.Add(new List<KeyValuePair<string, string>>()
                    {
                    new KeyValuePair<string, string>("@QuantityItem",tran.tranDetailItemWithToppings[i].tranDetailItem.Quantity.ToString("#,##0") + "x " ),
                    new KeyValuePair<string, string>("@ItemName",tran.tranDetailItemWithToppings[i].tranDetailItem.ItemName),
                    new KeyValuePair<string, string>("@ItemPrice",tran.tranDetailItemWithToppings[i].tranDetailItem.Amount.ToString("#,##0.00"))
                    });
                    }
                }
                int quantity = 0;
                foreach (var item in tran.tranDetailItemWithToppings)
                {
                    quantity += (int)item.tranDetailItem.Quantity;
                }

                if (DataCashing.CheckNet)
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

                string vat1 = "";
                string vat2 = "";
                string vat3 = "";
                string service1 = "ค่าบริการ";
                string service2 = "Service";
                string service3 = "0.00";
                string Tel = "";
                string member1 = "สมาชิก";
                string member2 = "Member";
                string member3 = "0.00";
                string discountBill1 = "ส่วนลด";
                string discountBill2 = "Discount";
                string discountBill3 = "0.00";
                string merchantAddress1 = "";
                string merchantAddress2 = "";
                string merchantAddress3 = "";
                string ReceiptName = "ใบเสร็จรับเงิน";

                //var branch = lstBranch.Where(x => x.SysBranchID == tran.tran.SysBranchID).FirstOrDefault();
                if (BranchDetail != null)
                {
                    string branchaddress = "";
                    branchaddress = await Utils.SetTextAddressAsync(BranchDetail);
                    var address = Utils.SplitAddress(branchaddress);

                    merchantAddress1 = address[0];
                    merchantAddress2 = address[1];
                    merchantAddress3 = address[2];

                    if (!string.IsNullOrEmpty(BranchDetail.Tel))
                    {
                        Tel = addTextTel(BranchDetail.Tel);
                    }
                }

                //Service charge
                if (!string.IsNullOrEmpty(tran.tran.FmlServiceCharge))
                {
                    var checkservice = tran.tran.FmlServiceCharge.IndexOf('%');
                    if (checkservice == -1)
                    {
                        var ServiceCharge = tran.tran.FmlServiceCharge;
                        if (Convert.ToDecimal(ServiceCharge) > 0)
                        {
                            service1 = "ค่าบริการ " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge));
                            service2 = "Service ";
                            service3 = Convert.ToDecimal(tran.tran.ServiceCharge).ToString("#,##0.00");
                        }
                    }
                    else
                    {
                        string[] split = tran.tran.FmlServiceCharge.Split('%');
                        var ServiceCharge = split[0];
                        if (Convert.ToDecimal(ServiceCharge) > 0)
                        {
                            service1 = "ค่าบริการ " + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)) + "%";
                            service2 = "Service ";
                            service3 = Convert.ToDecimal(tran.tran.ServiceCharge).ToString("#,##0.00");
                        }
                    }
                }

                //Discount From Member 
                double disMember = 0;
                var tranTradDiscountMember = tran.tranTradDiscounts.Where(x => x.DiscountType == "PS").FirstOrDefault();
                if (tranTradDiscountMember != null)
                {
                    var check = tranTradDiscountMember.FmlDiscount.IndexOf('%');
                    if (check == -1)
                    {
                        member1 = "สมาชิก " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Replace("%", "")));
                        member2 = "Member ";
                        disMember = Convert.ToDouble(tranTradDiscountMember.TradeDiscNoneVat + tranTradDiscountMember.TradeDiscHaveVat);
                        member3 = "-" + Convert.ToDecimal(disMember).ToString("#,##0.00");
                    }
                    else
                    {
                        member1 = "สมาชิก " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Remove(check))) + "%";
                        member2 = "Member ";
                        disMember = Convert.ToDouble(tranTradDiscountMember.TradeDiscNoneVat + tranTradDiscountMember.TradeDiscHaveVat);
                        member3 = "-" + Convert.ToDecimal(disMember).ToString("#,##0.00");
                    }
                }

                var tranTradDiscount = tran.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                if (tranTradDiscount != null)
                {
                    discountBill1 = "ส่วนลด";
                    discountBill2 = "Discount";
                    discountBill3 = "-" + Convert.ToDecimal(disDiscont).ToString("#,##0.00");
                }

                //Vat
                if (tran.tran.TotalVat > 0)
                {
                    vat1 = "ภาษีมูลค่าเพิ่ม " + tran.tran.TaxRate?.ToString("#,##0") + " %";
                    vat2 = "Vat " + tran.tran.TaxRate?.ToString("#,##0") + " %";
                    vat3 = tran.tran.TotalVat.ToString("#,##0.00");
                }

                //Receipt Eng
                if (txtshowlocal == "en" || txtshowlocal == "es")
                {
                    ReceiptName = "Receipt";
                }

                ParamSlip paramSlip = new ParamSlip()
                {
                    Header = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("@VoucherName", ReceiptName),
                        new KeyValuePair<string, string>("@merchantName", DataCashingAll.MerchantLocal.Name),
                        new KeyValuePair<string, string>("@merchantAddress1", merchantAddress1),
                        new KeyValuePair<string, string>("@merchantAddress2", merchantAddress2),
                        new KeyValuePair<string, string>("@merchantAddress3", merchantAddress3),
                        new KeyValuePair<string, string>("@merchantTel", Tel),
                        new KeyValuePair<string, string>("@merchantTaxid", DataCashingAll.MerchantLocal.TaxID),

                        new KeyValuePair<string, string>("@branchTaxId", ""),

                        new KeyValuePair<string, string>("@Billno", tran.tran.TranNo),
                        new KeyValuePair<string, string>("@Date", Utils.PrintDateTime(tran.tran.TranDate)),
                        new KeyValuePair<string, string>("@Time", Utils.PrintDateTime(tran.tran.TranDate)),

                        new KeyValuePair<string, string>("@Person", tran.tran.CustomerName),
                        new KeyValuePair<string, string>("@Address1", ""),
                        new KeyValuePair<string, string>("@Address2", ""),


                        new KeyValuePair<string, string>("@CountDetail", tran.tranDetailItemWithToppings.Count.ToString()),
                        new KeyValuePair<string, string>("@Quantity", quantity.ToString()),

                        new KeyValuePair<string, string>("@Cashier", "Cashier :" + tran.tran.SellerName),
                        new KeyValuePair<string, string>("@Vat1", vat1),
                        new KeyValuePair<string, string>("@Vat2", vat2),
                        new KeyValuePair<string, string>("@Vat3", vat3),
                        new KeyValuePair<string, string>("@Service1", service1),
                        new KeyValuePair<string, string>("@Service2", service2),
                        new KeyValuePair<string, string>("@Service3", service3),
                        new KeyValuePair<string, string>("@Member1", member1),
                        new KeyValuePair<string, string>("@Member2", member2),
                        new KeyValuePair<string, string>("@Member3", member3),
                        new KeyValuePair<string, string>("@Discount1", discountBill1),
                        new KeyValuePair<string, string>("@Discount2", discountBill2),
                        new KeyValuePair<string, string>("@Discount3", discountBill3),
                        new KeyValuePair<string, string>("@Total", SubTotal),
                        new KeyValuePair<string, string>("@GrandTotal", tran.tran.GrandPayment.ToString("#,##0.00")),

                    },
                    Details = items,
                    Footer = items
                };

                Graphic graphic = new Graphic();
                AssetManager assets2 = assets;
                bytesData = null;
                if (DataCashingAll.setting.TYPEPAGE == "80mm" || DataCashingAll.setting.TYPEPAGE == "80 มม.")
                {
                    if (txtshowlocal == "en" || txtshowlocal == "es")
                    {
                        bytesData = graphic.DrawImageFromXml("SlipTrans80mm_eng.xml", paramSlip, assets);
                    }
                    else
                    {
                        bytesData = graphic.DrawImageFromXml("SlipTrans80mm.xml", paramSlip, assets);
                    }
                }
                else
                {
                    if (txtshowlocal == "en" || txtshowlocal == "es")
                    {
                        bytesData = graphic.DrawImageFromXml("SlipTrans58mm_eng.xml", paramSlip, assets);
                    }
                    else
                    {
                        bytesData = graphic.DrawImageFromXml("SlipTrans58mm.xml", paramSlip, assets);
                    }
                }
                PrintController c = new PrintController();
                if (DataCashingAll.setting.USE == "Wifi")
                {
                    try
                    {
                        await c.PrintWifi(bytesData);
                        Thread.Sleep(10000);
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }
                else
                {
                    BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
                    List<BluetoothDevice> listdevices = new List<BluetoothDevice>();
                    listdevices = adapter.BondedDevices.ToList();
                    if (listdevices == null || listdevices.Count == 0)
                    {
                        Toast.MakeText(this.Activity, "Bluetooth DisConnected", ToastLength.Short).Show();
                        return;
                    }
                    BluetoothDevice printer = listdevices.Where(x => x.Name == DataCashingAll.setting.BLUETOOTH1 && x.Address == DataCashingAll.setting.IPADDRESS).FirstOrDefault();
                    if (printer == null)
                    {
                        Toast.MakeText(this.Activity, "Bluetooth DisConnected", ToastLength.Short).Show();
                        return;
                    }
                    else
                    {
                        BluetoothDevice mDevice = adapter.GetRemoteDevice(printer.Address);
                        BluetoothSocket mmsSocket = null;
                        Java.Util.UUID uuidfromname = null;
                        if (mmsSocket == null || DataCashingAll.addresssame != DataCashingAll.setting.IPADDRESS)
                        {


                            if (printer.Address == "00:11:22:33:44:55")
                            {
                                uuidfromname = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
                                mmsSocket = printer.CreateInsecureRfcommSocketToServiceRecord(uuidfromname);
                            }
                            else
                            {
                                ParcelUuid uuid = printer.GetUuids().ElementAt(0);
                                uuidfromname = UUID.FromString(uuid.ToString());
                                mmsSocket = printer.CreateInsecureRfcommSocketToServiceRecord(uuidfromname);

                            }
                        }
                        if (DataCashingAll.setting.COMMAND != "Epson Command")
                        {
                            try
                            {
                                if (!mmsSocket.IsConnected)
                                {
                                    mmsSocket.Connect();
                                    DataCashingAll.addresssame = DataCashingAll.setting.IPADDRESS;
                                }
                            }
                            catch (Exception)
                            {
                                mmsSocket.Close();
                                ParcelUuid uuid = printer.GetUuids().ElementAt(0);
                                uuidfromname = UUID.FromString(uuid.ToString());
                                mmsSocket = null;
                                mmsSocket = printer.CreateInsecureRfcommSocketToServiceRecord(uuidfromname);
                            }
                        }
                        try
                        {

                            if (!mmsSocket.IsConnected)
                            {
                                mmsSocket.Connect();
                                DataCashingAll.addresssame = DataCashingAll.setting.IPADDRESS;
                            }

                            if (mmsSocket.IsConnected)
                            {
                                var datastream = mmsSocket.OutputStream;
                                datastream.Flush();

                                var typeprint = DataCashingAll.setting.PRINTTYPE;
                                if (typeprint != "Image")
                                {
                                    List<byte> bytelist = new List<byte>();
                                    bytelist.Add((byte)27);
                                    bytelist.Add((byte)97);
                                    bytelist.Add((byte)49);

                                    bytelist.Add((byte)29);
                                    bytelist.Add((byte)33);
                                    bytelist.Add((byte)0);

                                    await datastream.WriteAsync(bytelist.ToArray());
                                    var imageforprinttext = await DrawString(tran);
                                    foreach (string txt in imageforprinttext)
                                    {
                                        var txt1 = txt;
                                        var x2 = ThaiLength(txt);
                                        var enc = System.Text.Encoding.GetEncoding("Windows-874");
                                        byte[] bytes = enc.GetBytes(txt1);
                                        var x = txt1.Length;
                                        await datastream.WriteAsync(bytes);
                                        await datastream.WriteAsync(new byte[] { (byte)10 });
                                    }
                                    byte[] byteArray = Encoding.ASCII.GetBytes("\n\n");
                                    await datastream.WriteAsync(byteArray, 0, byteArray.Length);
                                }
                                else
                                {
                                    await datastream.WriteAsync(bytesData, 0, bytesData.Length);
                                    byte[] byteArray = Encoding.ASCII.GetBytes("\n\n");
                                    await datastream.WriteAsync(byteArray, 0, byteArray.Length);
                                    if (printer.Address != "00:11:22:33:44:55")
                                    {
                                        Thread.Sleep(4000);

                                        if (DataCashingAll.setting.TYPEPAGE == "80mm" || DataCashingAll.setting.TYPEPAGE == "80 มม.")
                                        {
                                            Thread.Sleep(4000);
                                        }
                                        if (tran.tranDetailItemWithToppings.Count > 5)
                                        {
                                            int sleep = (tran.tranDetailItemWithToppings.Count - 5) * 700;
                                            Thread.Sleep(sleep);
                                        }
                                    }
                                }
                                byte[] b = new byte[]
                                {
                                    (byte)29,
                                    (byte)86,
                                    (byte)66,
                                    (byte)50
                                };
                                await datastream.WriteAsync(b);
                                if (DataCashingAll.setting.COMMAND == "Epson Command")
                                {
                                    mmsSocket.Close();
                                }

                                PostPrintcounter(tran);
                                //PostPrintcounter(tran);
                            }
                            else
                            {
                                Toast.MakeText(this.Activity, "Bluetooth DisConnected", ToastLength.Short).Show();
                            }
                        }
                        catch (IOException ee)
                        {
                            Toast.MakeText(this.Activity, ee.Message, ToastLength.Short).Show();
                        }
                        catch (Exception)
                        {
                            if (mmsSocket != null)
                            {
                                try
                                {
                                    mmsSocket.Close();
                                }
                                catch (IOException)
                                {

                                }
                                mmsSocket = null;
                            }
                        }
                    }
                }

                Activity.RunOnUiThread(() =>
                {
                    lnShowPrint.Enabled = true;
                });

            }
            catch (Exception ex)
            {
                Activity.RunOnUiThread(() =>
                {
                    lnShowPrint.Enabled = true;
                });
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Print at Bill Detaial");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
        public async Task<List<string>> DrawString(TranWithDetailsLocal tran)
        {

            int size;
            if (DataCashingAll.setting?.TYPEPAGE.Substring(0, 2) == "58")
            {
                size = 1;

            }
            else
            {
                size = 2;
            }
            var list = await Draw4(size, tran);
            return list;
        }
        public static int ThaiLength(string stringthai)
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
        public async Task<List<string>> Draw4(int size, TranWithDetailsLocal tran)
        {
            List<string> data = new List<string>();
            int[] itemLength = { 28, 40 };
            int[] itemLength2 = { 33, 48 };

            EscPosCommand escPos = new EscPosCommand(size);
            if (txtshowlocal == "en" || txtshowlocal == "es")
            {
                data.Add("Receipt");
            }
            else
            {
                data.Add("ใบเสร็จรับเงิน");
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
            if (BranchDetail != null)
            {
                string branchaddress = "";
                branchaddress = await Utils.SetTextAddressAsync(BranchDetail);
                var address = Utils.SplitAddress(branchaddress);

                data.Add(address[0]);
                data.Add(address[1]);
                data.Add(address[2]);

                if (!string.IsNullOrEmpty(BranchDetail.Tel))
                {
                    var Tel = addTextTel(BranchDetail.Tel);
                    data.Add(Tel);
                }
            }
            data.Add(escPos.ReplaceSpacebar2(tran.tran.TranNo.ToString(), Utils.PrintDateTime(tran.tran.TranDate)));
            data.Add(escPos.ReplaceSpacebar2(tran.tran.CustomerName, ""));
            var items = ItemDetail(tran.tranDetailItemWithToppings, itemLength[size - 1]);
            int j = 0;
            for (int i = 0; i < items.Count; i++)
            {
                var check = items[i].Substring(0, 2);
                if (!string.IsNullOrEmpty(check.Trim()))
                {
                    data.Add(escPos.ReplaceSpacebar2(items[i], tran.tranDetailItemWithToppings[j].tranDetailItem.Amount.ToString("#,##0.00")));
                    j++;
                }
                else
                {
                    data.Add(escPos.ReplaceSpacebar2(items[i], ""));
                }
            }

            if (txtshowlocal == "en" || txtshowlocal == "es")
            {
                data.Add(escPos.ReplaceSpacebar2("Quantity : " + tran.tranDetailItemWithToppings.Count.ToString(), "Quantity Total : " + tran.tranDetailItemWithToppings.Sum(x2 => x2.tranDetailItem.Quantity).ToString("N0")));
            }
            else
            {
                data.Add(escPos.ReplaceSpacebar2("จำนวนรายการ : " + tran.tranDetailItemWithToppings.Count.ToString(), "รวมจำนวนชิ้น : " + tran.tranDetailItemWithToppings.Sum(x2 => x2.tranDetailItem.Quantity).ToString("N0")));
            }

            if (txtshowlocal == "en" || txtshowlocal == "es")
            {
                data.Add(escPos.ReplaceSpacebar2("Suntotal", tran.tran.Total.ToString("#,##0.00")));
            }
            else
            {
                data.Add(escPos.ReplaceSpacebar2("รวมเงิน", tran.tran.Total.ToString("#,##0.00")));
            }

            if (tran.tran.TotalVat > 0)
            {
                if (txtshowlocal == "en" || txtshowlocal == "es")
                {
                    data.Add(escPos.ReplaceSpacebar2("VAT " + tran.tran.TaxRate?.ToString("#,##0") + " %", tran.tran.TotalVat.ToString("#,##0.00")));
                }
                else
                {
                    data.Add(escPos.ReplaceSpacebar2("ภาษีมูลค่าเพิ่ม " + tran.tran.TaxRate?.ToString("#,##0") + " %", tran.tran.TotalVat.ToString("#,##0.00")));
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
                        if (txtshowlocal == "en" || txtshowlocal == "es")
                        {
                            data.Add(escPos.ReplaceSpacebar2("Service Charge " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)), Convert.ToDecimal(tran.tran.ServiceCharge).ToString("#,##0.00")));
                        }
                        else
                        {
                            data.Add(escPos.ReplaceSpacebar2("ค่าบริการ " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)), Convert.ToDecimal(tran.tran.ServiceCharge).ToString("#,##0.00")));
                        }
                    }
                }
                else
                {
                    string[] split = tran.tran.FmlServiceCharge.Split('%');
                    var ServiceCharge = split[0];
                    if (Convert.ToDecimal(ServiceCharge) > 0)
                    {
                        if (txtshowlocal == "en" || txtshowlocal == "es")
                        {
                            data.Add(escPos.ReplaceSpacebar2("Service Charge " + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)) + "%", Convert.ToDecimal(tran.tran.ServiceCharge).ToString("#,##0.00")));
                        }
                        else
                        {
                            data.Add(escPos.ReplaceSpacebar2("ค่าบริการ " + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)) + "%", Convert.ToDecimal(tran.tran.ServiceCharge).ToString("#,##0.00")));
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
                    disMember = Convert.ToDouble(tranTradDiscountMember.TradeDiscNoneVat + tranTradDiscountMember.TradeDiscHaveVat);
                    if (txtshowlocal == "en" || txtshowlocal == "es")
                    {
                        data.Add(escPos.ReplaceSpacebar2("Member " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Replace("%", ""))), "-" + Convert.ToDecimal(disMember).ToString("#,##0.00")));
                    }
                    else
                    {
                        data.Add(escPos.ReplaceSpacebar2("สมาชิก " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Replace("%", ""))), "-" + Convert.ToDecimal(disMember).ToString("#,##0.00")));
                    }
                }
                else
                {
                    disMember = Convert.ToDouble(tranTradDiscountMember.TradeDiscNoneVat + tranTradDiscountMember.TradeDiscHaveVat);
                    if (txtshowlocal == "en" || txtshowlocal == "es")
                    {
                        data.Add(escPos.ReplaceSpacebar2("Member " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Remove(check))) + "%", "-" + Convert.ToDecimal(disMember).ToString("#,##0.00")));
                    }
                    else
                    {
                        data.Add(escPos.ReplaceSpacebar2("สมาชิก " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Remove(check))) + "%", "-" + Convert.ToDecimal(disMember).ToString("#,##0.00")));
                    }
                }
            }

            var tranTradDiscount = tran.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
            if (tranTradDiscount != null)
            {
                if (txtshowlocal == "en" || txtshowlocal == "es")
                {
                    data.Add(escPos.ReplaceSpacebar2("Discount", "-" + Convert.ToDecimal(disDiscont).ToString("#,##0.00")));
                }
                else
                {
                    data.Add(escPos.ReplaceSpacebar2("ส่วนลด", "-" + Convert.ToDecimal(disDiscont).ToString("#,##0.00")));
                }
            }


            if (txtshowlocal == "en" || txtshowlocal == "es")
            {
                data.Add(escPos.ReplaceSpacebar2("Total", tran.tran.GrandPayment.ToString("#,##0.00")));
            }
            else
            {
                data.Add(escPos.ReplaceSpacebar2("รวมทั้งสิ้น", tran.tran.GrandPayment.ToString("#,##0.00")));
            }

            data.Add("...THANK YOU...");
            data.Add("Cashier: " + tran.tran.SellerName.ToString());
            data.Add("Powered By Seniorsoft");
            return data;

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
                        if (!string.IsNullOrEmpty(itemNames[1]))
                        {
                            res2 = "   " + itemNames[1];
                        }
                        if (!string.IsNullOrEmpty(itemNames[2]))
                        {
                            res3 = "   " + itemNames[2];
                        }
                        space = "   ";
                        break;
                    case 2:
                        res = items[i].tranDetailItem.Quantity.ToString("###0") + "  " + items[i].tranDetailItem.ItemName;
                        if (!string.IsNullOrEmpty(itemNames[1]))
                        {
                            res2 = "  " + itemNames[1];
                        }
                        if (!string.IsNullOrEmpty(itemNames[2]))
                        {
                            res3 = "  " + itemNames[2];
                        }
                        space = "  ";
                        break;
                    case 1:
                        res = items[i].tranDetailItem.Quantity.ToString("###0") + " " + items[i].tranDetailItem.ItemName;
                        if (!string.IsNullOrEmpty(itemNames[1]))
                        {
                            res2 = " " + itemNames[1];
                        }
                        if (!string.IsNullOrEmpty(itemNames[2]))
                        {
                            res3 = " " + itemNames[2];
                        }
                        space = " ";
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
                if (!string.IsNullOrEmpty(res2.Trim()))
                {
                    itemName.Add("  " + res2);
                }
                if (!string.IsNullOrEmpty(res3.Trim()))        
                {
                    itemName.Add("  " + res3);
                }

                var listTopping = items[i].tranDetailItemToppings;
                foreach (var topping in listTopping)
                {
                    itemName.Add("  " + space + topping.ItemName + " (" + Utils.DisplayDecimal((topping.Quantity * topping.ToppingPrice)) + ")");
                }
                if (!string.IsNullOrEmpty(items[i].tranDetailItem.Comments?.Trim()))
                {
                    itemName.Add("  " + space + items[i].tranDetailItem.Comments);
                }

            }
            return itemName;
        }



        string addTextTel(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }

                if (value.Length == 9 & value.StartsWith("02"))
                {
                    var Phone = string.Empty;
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i == 2)
                        {
                            Phone += "-";
                        }
                        Phone += value[i];
                    }
                    return Phone;
                }

                if (value.Length == 9 & value.StartsWith("03") | value.StartsWith("04") | value.StartsWith("05") | value.StartsWith("07"))
                {
                    var Phone = string.Empty;
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i == 3)
                        {
                            Phone += "-";
                        }
                        Phone += value[i];
                    }
                    return Phone;
                }

                if (value.Length == 10)
                {
                    var Phone = string.Empty;
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i == 3)
                        {
                            Phone += "-";
                        }
                        Phone += value[i];
                    }
                    return Phone;
                }
                return value;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("addTextTel at Receipt");
                return value;
            }
        }
        Tran getTran = new Tran();
        private async void PostPrintcounter(TranWithDetailsLocal tranWithDetails)
        {
            try
            {
                TranWithDetailsLocal tran = tranWithDetails;
                //บันทึกการพิมพ์ที่ Local    
                if (getTran != null)
                {
                    printCounter = printCounter + 1;
                    getTran.PrintCounterLocal = printCounter;
                    transManage = new TransManage();
                    var updatePrint = await transManage.UpdateTran(getTran);
                    txtPrintCounter.Text = (printCounter).ToString("#,##0");

                    //เพิ่มฟังก์ชันสำหรับนับการพิมพ์ PrintCounter
                    UtilsAll.PostPrintCounter(Convert.ToInt32(branchID), tran.tran.TranNo, Utils.ChangeDateTime(tran.tran.TranDate), 1, tran.tran);
                }
                else
                {
                    //เพิ่มฟังก์ชันสำหรับนับการพิมพ์ PrintCounter
                    UtilsAll.PostPrintCounter(Convert.ToInt32(branchID), tran.tran.TranNo, Utils.ChangeDateTime(tran.tran.TranDate), 1, tran.tran);
                    txtPrintCounter.Text = (tranWithDetails.tran.PrintCounter).ToString("#,##0");
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public void UiNewBillHistory()
        {
            try
            {
                txtTranNo.Text = string.Empty;
                textNameMerchant.Text = string.Empty;
                textTranNo.Text = string.Empty;
                textDate.Text = string.Empty;
                textCustomerName.Text = string.Empty;
                textTotalItems.Text = string.Empty;
                textUnit.Text = string.Empty;
                textSubtotalAmount.Text = string.Empty;
                textMemberDiscountAmount.Text = string.Empty;
                textVatAmount.Text = string.Empty;
                textDiscountAmount.Text = string.Empty;
                textServiceAmount.Text = string.Empty;
                textTotalAmount.Text = string.Empty;
                textCashAmount.Text = string.Empty;
                textChangeAmount.Text = string.Empty;
                textCashierName.Text = string.Empty;
                textRemarkDetail.Text = string.Empty;
                lnStatusVoid.Visibility = ViewStates.Gone;
                lnMemberDiscount.Visibility = ViewStates.Gone;
                lnVat.Visibility = ViewStates.Gone;
                lnDiscount.Visibility = ViewStates.Gone;
                lnService.Visibility = ViewStates.Gone;
                lnRemark.Visibility = ViewStates.Gone;
                lnShowButton.Visibility = ViewStates.Gone;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity,ex.Message,ToastLength.Short).Show();
            }
        }
    }
}