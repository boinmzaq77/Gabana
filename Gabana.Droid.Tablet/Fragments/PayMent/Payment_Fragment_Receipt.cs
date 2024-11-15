using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Payment;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.ClassStructure;
using Gabana.ShareSource.Manage;
using Java.Util;
using LinqToDB.Common;
using Newtonsoft.Json;
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

namespace Gabana.Droid.Tablet.Fragments.PayMent
{
    public class Payment_Fragment_Receipt : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        View view;
        public static TranWithDetailsLocal tranWithDetails;
        public static Payment_Fragment_Receipt NewInstance()
        {
            Payment_Fragment_Receipt frag = new Payment_Fragment_Receipt();
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.payment_fragment_receipt, container, false);
            try
            {
                tranWithDetails = PaymentActivity.tranWithDetails;
                ComBineUI();
                ShowDetail();
                assets = this.Activity.Assets;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
            return view;
        }
        public override void OnResume()
        {
            try
            {
                base.OnResume();

                if (!IsAdded)
                {
                    return;
                }

                //if (!IsVisible)
                //{
                //    return;
                //}

                CheckJwt();
                tranWithDetails = PaymentActivity.tranWithDetails;
                ShowDetail();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        LinearLayout lnBack, lnPrintCounter;
        TextView txtPrintCounter;
        FrameLayout frameBill;
        ImageView imgProfileMerchant;
        TextView textNameMerchant, textTranNo, textDate, textCustomerName;
        RecyclerView recyclerViewReceipt;
        TextView textTotalItems, textUnit, textSubtotalAmount;
        FrameLayout lnDiscountMember;
        TextView textMemberDiscount, textMemberDiscountAmount;
        FrameLayout lnVat;
        TextView textVat, textVatAmount;
        FrameLayout lnDiscount;
        TextView textDiscount, textDiscountAmount;
        FrameLayout lnService;
        TextView textService, textServiceAmount;
        TextView textTotal, textTotalAmount;
        RecyclerView recyclerViewCash;
        TextView textChange, textChangeAmount, textCashier, textCashierName;
        FrameLayout frameRemark;
        TextView textRemark, textRemarkDetail;
        LinearLayout lnReceiptImage;
        ImageView imageReceiptImage;
        FrameLayout frameLayoutReciept;
        LinearLayout lnShowButton, lnShowPrint, lnShowPDF, lnShowEmail, lnShowShar;
        FrameLayout lnPrint, lnPDF, lnEmail, lnShare;
        ScrollView scrollViewReciep;

        private void ComBineUI()
        {
            scrollViewReciep = view.FindViewById<ScrollView>(Resource.Id.scrollViewReciep);
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnPrintCounter = view.FindViewById<LinearLayout>(Resource.Id.lnPrintCounter);
            txtPrintCounter = view.FindViewById<TextView>(Resource.Id.txtPrintCounter);
            frameBill = view.FindViewById<FrameLayout>(Resource.Id.frameBill);
            imgProfileMerchant = view.FindViewById<ImageView>(Resource.Id.imgProfileMerchant);
            textNameMerchant = view.FindViewById<TextView>(Resource.Id.textNameMerchant);
            textTranNo = view.FindViewById<TextView>(Resource.Id.textTranNo);
            textDate = view.FindViewById<TextView>(Resource.Id.textDate);
            textCustomerName = view.FindViewById<TextView>(Resource.Id.textCustomerName);
            recyclerViewReceipt = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewReceipt);
            textTotalItems = view.FindViewById<TextView>(Resource.Id.textTotalItems);
            textUnit = view.FindViewById<TextView>(Resource.Id.textUnit);
            textSubtotalAmount = view.FindViewById<TextView>(Resource.Id.textSubtotalAmount);
            lnDiscountMember = view.FindViewById<FrameLayout>(Resource.Id.lnDiscountMember);
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
            recyclerViewCash = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewCash);
            textChange = view.FindViewById<TextView>(Resource.Id.textChange);
            textChangeAmount = view.FindViewById<TextView>(Resource.Id.textChangeAmount);
            textCashier = view.FindViewById<TextView>(Resource.Id.textCashier);
            textCashierName = view.FindViewById<TextView>(Resource.Id.textCashierName);
            frameRemark = view.FindViewById<FrameLayout>(Resource.Id.frameRemark);
            textRemark = view.FindViewById<TextView>(Resource.Id.textRemark);
            textRemarkDetail = view.FindViewById<TextView>(Resource.Id.textRemarkDetail);
            lnReceiptImage = view.FindViewById<LinearLayout>(Resource.Id.lnReceiptImage);
            imageReceiptImage = view.FindViewById<ImageView>(Resource.Id.imageReceiptImage);
            frameLayoutReciept = view.FindViewById<FrameLayout>(Resource.Id.frameLayoutReciept);
            lnShowButton = view.FindViewById<LinearLayout>(Resource.Id.lnShowButton);
            lnShowPrint = view.FindViewById<LinearLayout>(Resource.Id.lnShowPrint);
            lnPrint = view.FindViewById<FrameLayout>(Resource.Id.lnPrint);
            lnShowPDF = view.FindViewById<LinearLayout>(Resource.Id.lnShowPDF);
            lnPDF = view.FindViewById<FrameLayout>(Resource.Id.lnPDF);
            lnShowEmail = view.FindViewById<LinearLayout>(Resource.Id.lnShowEmail);
            lnEmail = view.FindViewById<FrameLayout>(Resource.Id.lnEmail);
            lnShowShar = view.FindViewById<LinearLayout>(Resource.Id.lnShowShar);
            lnShare = view.FindViewById<FrameLayout>(Resource.Id.lnShare);
            lnPrintCounter.Click += LnPrintCounter_Click;
            lnShowPrint.Click += LnPrint_Click;
            lnShowPDF.Click += LnPDF_Click;
            lnShowEmail.Click += LnEmail_Click;
            lnShowShar.Click += LnShare_Click;
        }
        private void LnPrintCounter_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this.Activity, GetString(Resource.String.receipt_activity_reprint) + txtPrintCounter.Text, ToastLength.Long).Show();
        }
        string CURRENCYSYMBOLS;
        Tran getTran;
        TransManage transManage = new TransManage();
        int localprintCounter = 0;
        double total = 0;
        decimal paymentAmount = 0;
        double disDiscont = 0.0;
        string txtshowlocal = string.Empty;

        private async void ShowDetail()
        {
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;
            if (CURRENCYSYMBOLS == null) CURRENCYSYMBOLS = "฿";
            txtshowlocal = Preferences.Get("Language", "");
            scrollViewReciep.ScrollTo(0, 0);
            TokenResult res = await TokenServiceBase.GetToken();
            if (!res.status)
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                this.Activity.Finish();
                return;
            }

            if (tranWithDetails != null)
            {
                getTran = await transManage.GetTran(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);
                if (getTran == null)
                {
                    localprintCounter = 0;
                }
                else
                {
                    localprintCounter = (int)getTran.PrintCounterLocal;

                }

                textNameMerchant.Text = DataCashingAll.MerchantLocal.Name;
                var path = DataCashingAll.MerchantLocal.LogoLocalPath;
                if (!string.IsNullOrEmpty(path))
                {
                    Android.Net.Uri uri = Android.Net.Uri.Parse(path);
                    imgProfileMerchant.SetImageURI(uri);
                }
                else
                {
                    imgProfileMerchant.SetBackgroundResource(Resource.Mipmap.LogoDefault);
                }

                textTranNo.Text = tranWithDetails.tran.TranNo;
                var timezoneslocal = TimeZoneInfo.Local;
                var date = tranWithDetails.tran.TranDate;
                textDate.Text = TimeZoneInfo.ConvertTimeFromUtc(date, timezoneslocal).ToString("dd/MM/yyyy HH:mm tt", new CultureInfo("en-US"));
                textCustomerName.Text = tranWithDetails.tran.CustomerName;

                Payment_Adapter_ReceiptItem receipt_adapter_item = new Payment_Adapter_ReceiptItem(tranWithDetails);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                recyclerViewReceipt.SetLayoutManager(gridLayoutManager);
                recyclerViewReceipt.HasFixedSize = true;
                int count = tranWithDetails.tranDetailItemWithToppings.Count + 1;
                recyclerViewReceipt.SetItemViewCacheSize(count);
                recyclerViewReceipt.SetAdapter(receipt_adapter_item);

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
                textTotalItems.Text = " " + tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity).ToString("#,##0");
                textCashierName.Text = tranWithDetails.tran.SellerName?.ToString();
                textSubtotalAmount.Text = Utils.DisplayDecimal(tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Amount));
                textVatAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);
                textTotalAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal);
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

                Payment_Adapter_ReceiptCash receipt_Adapter_Cash = new Payment_Adapter_ReceiptCash(paymenttypeamout);
                GridLayoutManager gridLayoutManager2 = new GridLayoutManager(this.Activity, 1, 1, false);
                recyclerViewCash.SetLayoutManager(gridLayoutManager2);
                recyclerViewCash.HasFixedSize = true;
                recyclerViewCash.SetItemViewCacheSize(20);
                recyclerViewCash.SetAdapter(receipt_Adapter_Cash);
                textChangeAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.Change);
                total = Convert.ToDouble(tranWithDetails.tran.GrandTotal);

                //Discount From Member
                double disMember = 0;
                if (DataCashing.SysCustomerID == 999)
                {
                    lnDiscountMember.Visibility = ViewStates.Gone;
                }
                else //Discount From Member 
                {
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
                        lnDiscountMember.Visibility = ViewStates.Visible;
                        disMember = Convert.ToDouble(tranTradDiscountMember.TradeDiscNoneVat + tranTradDiscountMember.TradeDiscHaveVat);
                        textMemberDiscountAmount.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disMember));
                    }
                    else
                    {
                        lnDiscountMember.Visibility = ViewStates.Gone;
                    }
                }

                //Vat
                string getvat = Utils.DisplayDouble(tranWithDetails.tran.TaxRate == null ? 0 : tranWithDetails.tran.TaxRate.Value);
                if (tranWithDetails.tran.TaxRate == 0)
                {
                    lnVat.Visibility = ViewStates.Gone;
                }

                textVat.Text = GetString(Resource.String.vat) + " " + getvat + "%";
                textVatAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);

                //Discount From Manual
                double discount;
                var tranTradDiscount = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                if (tranTradDiscount != null)
                {
                    lnDiscount.Visibility = ViewStates.Visible;
                    string CartDiscount;

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

                //Service Charge
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

                    var checkservice = tranWithDetails.tran.FmlServiceCharge.IndexOf('%');
                    if (checkservice == -1)
                    {
                        ServiceCharge = tranWithDetails.tran.FmlServiceCharge;
                        textService.Text = GetString(Resource.String.servicecharge) + " " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge));
                    }
                    else
                    {
                        ServiceCharge = tranWithDetails.tran.FmlServiceCharge.Remove(checkservice);
                        textService.Text = GetString(Resource.String.servicecharge) + " " + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)) + "%";
                    }
                    textServiceAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.ServiceCharge);
                }

                if (string.IsNullOrEmpty(tranWithDetails.tran.Comments))
                {
                    frameRemark.Visibility = ViewStates.Gone;
                }
                else
                {
                    textRemarkDetail.Text = tranWithDetails.tran.Comments?.ToString();
                }

                frameLayoutReciept.Click += FrameLayoutReciept_Click;

                var index = tranWithDetails.tranPayments.FindLastIndex(x => !string.IsNullOrEmpty(x.PicturePath));
                if (index != -1)
                {
                    //ใบเสร็จ แสดงรูปภาพจากเครื่อง
                    lnReceiptImage.Visibility = ViewStates.Visible;
                    var pathReciept = tranWithDetails.tranPayments[index].PicturePath;

                    Android.Net.Uri uri = Android.Net.Uri.Parse(pathReciept);
                    imageReceiptImage.SetImageURI(uri);
                }
                else
                {
                    lnReceiptImage.Visibility = ViewStates.Gone;
                }

                txtPrintCounter.Text = (tranWithDetails.tran.PrintCounter + tranWithDetails.tran.PrintCounterLocal).ToString("#,##0");
            }
        }
        private void LnPrint_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(PaymentActivity.payment_main.SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                lnShowPrint.Enabled = false;
                var perSetting = Preferences.Get("Setting", "");
                if (perSetting != "")
                {
                    var settingPrinter = JsonConvert.DeserializeObject<SettingPrinter>(perSetting);
                    DataCashingAll.setting = settingPrinter;
                    Print(tranWithDetails);
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
        int lengThName = 28;
        Android.Graphics.Bitmap bitmap;
        public static AssetManager assets;
        static byte[] bytesData;
        ORM.MerchantDB.Branch BranchDetail = new ORM.MerchantDB.Branch();
        internal async Task Print(TranWithDetailsLocal t)
        {
            try
            {
                TranWithDetailsLocal tran = t;
                if (DataCashingAll.setting.PRINTTYPE == "Image")
                {
                    List<List<KeyValuePair<string, string>>> items = new List<List<KeyValuePair<string, string>>>();
                    if (DataCashingAll.setting?.TYPEPAGE.Substring(0, 2) == "80")
                    {
                        lengThName = 35;
                    }
                    for (int i = 0; i < tran.tranDetailItemWithToppings.Count; i++)
                    {
                        string sizename = tran.tranDetailItemWithToppings[i].tranDetailItem.SizeName;
                        string name = "";
                        if (string.IsNullOrEmpty(sizename) || sizename == "Default Size")
                        {
                            name = tran.tranDetailItemWithToppings[i].tranDetailItem.ItemName?.ToString();
                        }
                        else
                        {
                            name = tran.tranDetailItemWithToppings[i].tranDetailItem.ItemName?.ToString() + " " + sizename;
                        }

                        if (name.Length > lengThName)
                        {

                            var itemNames = Utils.SplitItemName(lengThName, name);

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
                            if (!string.IsNullOrEmpty(itemNames[2]))
                            {
                                items.Add(new List<KeyValuePair<string, string>>()
                            {
                                new KeyValuePair<string, string>("@QuantityItem"," " ),
                                new KeyValuePair<string, string>("@ItemName",itemNames[2]),
                                new KeyValuePair<string, string>("@ItemPrice"," ")
                            });
                            }
                        }
                        else
                        {
                            items.Add(new List<KeyValuePair<string, string>>()
                            {
                                new KeyValuePair<string, string>("@QuantityItem",tran.tranDetailItemWithToppings[i].tranDetailItem.Quantity.ToString("#,##0") + "x " ),
                                new KeyValuePair<string, string>("@ItemName",name),
                                new KeyValuePair<string, string>("@ItemPrice",tran.tranDetailItemWithToppings[i].tranDetailItem.Amount.ToString("#,##0.00"))
                            });

                        }
                        var listTopping = tran.tranDetailItemWithToppings[i].tranDetailItemToppings;
                        foreach (var topping in listTopping)
                        {
                            items.Add(new List<KeyValuePair<string, string>>()
                            {
                                 new KeyValuePair<string, string>("@QuantityItem",""),
                                    new KeyValuePair<string, string>("@ItemName",topping.ItemName + " (" + Utils.DisplayDecimal((topping.Quantity * topping.ToppingPrice)) + ")"),
                                    new KeyValuePair<string, string>("@ItemPrice", "")
                            });
                        }
                        if (!string.IsNullOrEmpty(tran.tranDetailItemWithToppings[i].tranDetailItem.Comments))
                        {
                            items.Add(new List<KeyValuePair<string, string>>()
                            {
                                new KeyValuePair<string, string>("@QuantityItem"," " ),
                                new KeyValuePair<string, string>("@ItemName",tran.tranDetailItemWithToppings[i].tranDetailItem.Comments),
                                new KeyValuePair<string, string>("@ItemPrice"," ")
                            });
                        }
                    }
                    int quantity = 0;
                    foreach (var item in tran.tranDetailItemWithToppings)
                    {
                        quantity += (int)item.tranDetailItem.Quantity;
                    }

                    if (await GabanaAPI.CheckNetWork())
                    {
                        List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
                        cloudbranch = await GabanaAPI.GetDataBranch();
                        var local = cloudbranch.Where(x => x.SysBranchID == tranWithDetails.tran.SysBranchID).FirstOrDefault();
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

                    string merchantAddress1 = "";
                    string merchantAddress2 = "";
                    string merchantAddress3 = "";
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
                    string ReceiptName = "ใบเสร็จรับเงิน";

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

                    //Discount From Bill
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
                    new KeyValuePair<string, string>("@merchantName",DataCashingAll.MerchantLocal.Name),
                    new KeyValuePair<string, string>("@merchantAddress1", merchantAddress1),
                    new KeyValuePair<string, string>("@merchantAddress2", merchantAddress2),
                    new KeyValuePair<string, string>("@merchantAddress3", merchantAddress3),
                    new KeyValuePair<string, string>("@merchantTel", Tel),
                    new KeyValuePair<string, string>("@merchantTaxid",DataCashingAll.MerchantLocal.TaxID),

                    new KeyValuePair<string, string>("@branchTaxId", ""),

                    new KeyValuePair<string, string>("@Billno", tran.tran.TranNo),
                    new KeyValuePair<string, string>("@Date",  Utils.PrintDateTime(tran.tran.TranDate)),
                    new KeyValuePair<string, string>("@Time",  Utils.PrintDateTime(tran.tran.TranDate)),

                    new KeyValuePair<string, string>("@Person", tran.tran.CustomerName),
                    new KeyValuePair<string, string>("@Address1",""),
                    new KeyValuePair<string, string>("@Address2", ""),


                    new KeyValuePair<string, string>("@CountDetail", tran.tranDetailItemWithToppings.Count.ToString()),
                    new KeyValuePair<string, string>("@Quantity", quantity.ToString()),

                    new KeyValuePair<string, string>("@Cashier", "Cashier :" + tran.tran.SellerName),
                    new KeyValuePair<string, string>("@Vat1",vat1 ),
                    new KeyValuePair<string, string>("@Vat2",vat2 ),
                    new KeyValuePair<string, string>("@Vat3",vat3 ),
                    new KeyValuePair<string, string>("@Service1",service1 ),
                    new KeyValuePair<string, string>("@Service2",service2 ),
                    new KeyValuePair<string, string>("@Service3",service3 ),
                    new KeyValuePair<string, string>("@Member1",member1 ),
                    new KeyValuePair<string, string>("@Member2",member2 ),
                    new KeyValuePair<string, string>("@Member3",member3 ),
                    new KeyValuePair<string, string>("@Discount1",discountBill1 ),
                    new KeyValuePair<string, string>("@Discount2",discountBill2 ),
                    new KeyValuePair<string, string>("@Discount3",discountBill3) ,
                    new KeyValuePair<string, string>("@Total", tran.tran.Total.ToString("#,##0.00")),
                    new KeyValuePair<string, string>("@GrandTotal", tran.tran.GrandPayment.ToString("#,##0.00")),

                        },
                        Details = items
                    };

                    Graphic graphic = new Graphic();
                    AssetManager assets2 = assets;
                    //byte[] bytesData;
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

        private async void PostPrintcounter(TranWithDetailsLocal tranWithDetails)
        {
            try
            {
                TranWithDetailsLocal tran = tranWithDetails;
                //บันทึกการพิมพ์ที่ Local 
                if (getTran != null)
                {
                    localprintCounter = localprintCounter + 1;
                    getTran.PrintCounterLocal = localprintCounter;
                    await transManage.UpdateTran(getTran);

                    txtPrintCounter.Text = (localprintCounter).ToString("#,##0");

                    //เพิ่มฟังก์ชันสำหรับนับการพิมพ์ PrintCounter
                    UtilsAll.PostPrintCounter(DataCashingAll.SysBranchId, tran.tran.TranNo, Utils.ChangeDateTime(tran.tran.TranDate), 1, tran.tran);
                }
                else
                {
                    //เพิ่มฟังก์ชันสำหรับนับการพิมพ์ PrintCounter
                    UtilsAll.PostPrintCounter(DataCashingAll.SysBranchId, tran.tran.TranNo, Utils.ChangeDateTime(tran.tran.TranDate), 1, tran.tran);
                    txtPrintCounter.Text = (tranWithDetails.tran.PrintCounter).ToString("#,##0");
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private async void LnPDF_Click(object sender, EventArgs e)
        {
            MainDialog dialogLoading = new MainDialog();
            try
            {
                lnShowPDF.Enabled = false;
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(PaymentActivity.payment_main.SupportFragmentManager, nameof(DialogLoading));
                }
                await CreateBitmap();
                await CreatePDF("Receipt " + tranWithDetails.tran.TranNo);

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                lnShowPDF.Enabled = true;
            }
            catch (Exception)
            {
                lnShowPDF.Enabled = true;
                dialogLoading.Dismiss();
            }
        }
        private async void LnEmail_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                lnShowEmail.Enabled = false;
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(PaymentActivity.payment_main.SupportFragmentManager, nameof(DialogLoading));
                }
                await CreateBitmap();
                await SaveandShowEmail("Receipt " + tranWithDetails.tran.TranNo, "Receipt " + tranWithDetails.tran.TranNo + " " + Utils.ShowDate(tranWithDetails.tran.TranDate), "Receipt " + tranWithDetails.tran.TranNo);

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                lnShowEmail.Enabled = true;

            }
            catch (Exception)
            {
                lnShowEmail.Enabled = true;
                dialogLoading.Dismiss();
            }
        }
        private async void LnShare_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                lnShowShar.Enabled = false;
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(PaymentActivity.payment_main.SupportFragmentManager, nameof(DialogLoading));
                }
                await CreateBitmap();
                await SaveandShare("Receipt " + tranWithDetails.tran.TranNo, "Receipt " + tranWithDetails.tran.TranNo + " " + Utils.ShowDate(tranWithDetails.tran.TranDate), "Receipt " + tranWithDetails.tran.TranNo);

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                lnShowShar.Enabled = true;
            }
            catch (Exception)
            {
                lnShowShar.Enabled = true;
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
                string fullName = Path.Combine(filePath, fullpath);

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
                if (tranWithDetails.tran.SysCustomerID != 999)
                {
                    CustomerManage customerManage = new CustomerManage();
                    Customer customer = new Customer();
                    var listCustomer = new List<Customer>();
                    listCustomer = await customerManage.GetAllCustomer();
                    customer = listCustomer.Where(x => x.SysCustomerID == tranWithDetails.tran.SysCustomerID).FirstOrDefault();
                    if (!string.IsNullOrEmpty(customer?.EMail))
                    {
                        EmailAddress = customer.EMail;
                    }
                }
                #region OldCode
                //var intentemail = new Intent();
                //intentemail.SetAction(Intent.ActionSendto);
                //intentemail.SetData(Android.Net.Uri.Parse("mailto:" + Email));

                //var intent = new Intent();
                //intent.SetAction(Intent.ActionSendto);
                //intent.SetType("image/*");
                //intent.PutExtra(Intent.ExtraText, message ?? "");
                //intent.PutExtra(Intent.ExtraEmail, new String[] { Email } );
                //intent.PutExtra(Intent.ExtraSubject , title ?? "");
                //intent.PutExtra(Intent.ExtraStream, Attachments);
                //intent.SetFlags(ActivityFlags.GrantReadUriPermission);
                //intent.SetFlags(ActivityFlags.GrantWriteUriPermission);
                //intent.SetFlags(ActivityFlags.ClearWhenTaskReset);
                //intent.SetData(Android.Net.Uri.Parse("mailto:" + Email)); //ถ้าไม่ใช้จะไม่มีแอปสำหรับการแชร์
                //intent.Selector = intentemail;

                //var chooserIntent = Intent.CreateChooser(intent, "Share Email");
                //this.StartActivity(chooserIntent); 
                #endregion

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
                await Email.ComposeAsync(data);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowEmail at Bill Detail");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        async Task CreateBitmap()
        {
            try
            {
                int width = frameBill.Width;
                int height = frameBill.Height;

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
                frameBill.Draw(canvas);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CreateBitmap at Bill Detaial");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        public Task CreatePDF(string filename)
        {
            DialogLoading dialogLoading = new DialogLoading();
            dialogLoading.Cancelable = false;
            dialogLoading.Show(PaymentActivity.payment_main.SupportFragmentManager, nameof(DialogLoading));
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
                //string fullName = Path.Combine(filePath, fullpath);
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

                dialogLoading.Dismiss();

                var chooserIntent = Intent.CreateChooser(intent, "Open File");
                try
                {
                    this.StartActivity(chooserIntent);
                }
                catch (ActivityNotFoundException e)
                {
                    Toast.MakeText(this.Activity, "ไม่มีแอปพลิเคชันที่ใช้งานได้", ToastLength.Short).Show();
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OpenFile at Bill Detail");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return Task.FromResult(false);
            }
        }
        string AddTextTel(string value)
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
        private void FrameLayoutReciept_Click(object sender, EventArgs e)
        {
            NewSale();
        }
        private async void NewSale()
        {
            DataCashing.setQuantityToCart = 1;
            DataCashing.SysCustomerID = 999;
            //DataCashing.PaymentNo = 0;
            DeletePictureinFloder();

            //Initial ค่าใหม่หลังจากเปิดการขายรอบใหม่
            tranWithDetails = null;
            tranWithDetails = await Utils.initialData();
            MainActivity.tranWithDetails = tranWithDetails;
            POS_Fragment_Main.fragment_main.OnResume();
            POS_Fragment_Cart.fragment_cart.OnResume();
            PaymentActivity.tranWithDetails = tranWithDetails;
            PaymentActivity.payment_main.OnBackPressed();
        }
        async void DeletePictureinFloder()
        {
            try
            {
                if (string.IsNullOrEmpty(DataCashingAll.PathImageBill))
                {
                    return;
                }

                System.IO.DirectoryInfo di = new DirectoryInfo(DataCashingAll.PathImageBill);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DeletePictureinFloder at Bill Detaial");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    return;
                }
                Utils.AddNullValue();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckJwt at changePass");
            }
        }
    }
}