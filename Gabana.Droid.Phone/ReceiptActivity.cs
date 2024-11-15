using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
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

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class ReceiptActivity : AppCompatActivity
    {
        public static TranWithDetailsLocal tranWithDetails;
        FrameLayout lnDiscountMember, lnDiscount, lnVat, lnService, frameBill;
        string CURRENCYSYMBOLS;
        static byte[] bytesData;
        Android.Graphics.Bitmap bitmap;
        public static AssetManager assets;
        double total = 0;
        DialogLoading dialogLoading = new DialogLoading();
        ScrollView scrollViewReciep;
        int lengThName = 28;
        LinearLayout lnShowPrint, lnShowPDF, lnShowEmail, lnShowShar, lnReceiptImage, lnBack, lnPrintCounter;
        double disDiscont = 0.0;
        int localprintCounter = 0;
        ORM.MerchantDB.Branch BranchDetail = new ORM.MerchantDB.Branch();
        //ImageView imgProfileMerchant, imageReceiptImage;
        RecyclerView recyclerviewReceipt;
        TextView txtPrintCounter, textNameMerchant, textTranNo, textDate, textCustomerName, textTotalItems, textUnit, textSubtotalAmount, textMemberDiscount,
        textMemberDiscountAmount, textVat, textVatAmount, textDiscount, textDiscountAmount, textTotalAmount, textCashAmount, textChangeAmount,
            textCashierName, textService, textServiceAmount, textRemarkDetail;
        FrameLayout frameLayoutReciept, frameRemark;
        string txtshowlocal = string.Empty;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.receipt_activity_main);

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                assets = this.Assets;
                InitializeUIElements();
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;
                if (CURRENCYSYMBOLS == null) CURRENCYSYMBOLS = "฿";
                txtshowlocal = Preferences.Get("Language", "");

                frameBill.SetBackgroundColor(Android.Graphics.Color.White);
                scrollViewReciep.ScrollTo(0, 0);

                CheckJwt();
                CheckPermission();

                if (tranWithDetails != null)
                {
                    await LoadTranDetailsAsync();
                    SetupMerchantInfo();
                    SetupTransactionDetails();
                    SetupReceiptAdapterItem();
                    SetupPaymentDetails();
                    SetupDiscount();
                    SetupVat();
                    SetupServiceCharge();
                    SetupCommentsAndImages();
                    txtPrintCounter.Text = (tranWithDetails.tran.PrintCounter + tranWithDetails.tran.PrintCounterLocal).ToString("#,##0");
                }
                else
                {
                    if (dialogLoading != null)
                    {
                        dialogLoading.DismissAllowingStateLoss();
                        dialogLoading.Dismiss();
                    }
                    NewSale();
                }

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                _ = TinyInsights.TrackPageViewAsync("OnCreate : ReceiptActivity");
            }
            catch (Exception ex)
            {
                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Receipt");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void SetupCommentsAndImages()
        {
            if (string.IsNullOrEmpty(tranWithDetails.tran.Comments))
            {
                frameRemark.Visibility = ViewStates.Gone;
            }
            else
            {
                textRemarkDetail.Text = tranWithDetails.tran.Comments?.ToString();
            }

            var index = tranWithDetails.tranPayments.FindLastIndex(x => !string.IsNullOrEmpty(x.PicturePath));
            if (index != -1)
            {
                //ใบเสร็จ แสดงรูปภาพจากเครื่อง
                lnReceiptImage.Visibility = ViewStates.Visible;
                var pathReciept = tranWithDetails.tranPayments[index].PicturePath;

                using (var imageReceiptImage = FindViewById<ImageView>(Resource.Id.imageReceiptImage))
                {
                    Android.Net.Uri uri = Android.Net.Uri.Parse(pathReciept);
                    imageReceiptImage.SetImageURI(uri);
                }
            }
            else
            {
                lnReceiptImage.Visibility = ViewStates.Gone;
            }
        }

        private void SetupPaymentDetails()
        {
            decimal PaymentAmount = 0;
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

            Receipt_Adapter_Cash receipt_Adapter_Cash = new Receipt_Adapter_Cash(paymenttypeamout);
            RecyclerView recyclerViewCash = FindViewById<RecyclerView>(Resource.Id.recyclerViewCash);
            GridLayoutManager gridLayoutManager2 = new GridLayoutManager(this, 1, 1, false);
            recyclerViewCash.SetLayoutManager(gridLayoutManager2);
            recyclerViewCash.HasFixedSize = true;
            recyclerViewCash.SetItemViewCacheSize(20);
            recyclerViewCash.SetAdapter(receipt_Adapter_Cash);
            textCashAmount.Text = Utils.DisplayDecimal(PaymentAmount);
            textChangeAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.Change);
            total = Convert.ToDouble(tranWithDetails.tran.GrandTotal);
        }

        private void SetupReceiptAdapterItem()
        {
            Receipt_Adapter_Item receipt_adapter_item = new Receipt_Adapter_Item(tranWithDetails);
            GridLayoutManager gridLayoutManager = new GridLayoutManager(this, 1, 1, false);
            recyclerviewReceipt.SetLayoutManager(gridLayoutManager);
            recyclerviewReceipt.HasFixedSize = true;
            int count = tranWithDetails.tranDetailItemWithToppings.Count + 1;
            recyclerviewReceipt.SetItemViewCacheSize(count);
            recyclerviewReceipt.SetAdapter(receipt_adapter_item);

            textUnit.Text = (DataCashing.Language == "th") ? " " :
            (tranWithDetails.tranDetailItemWithToppings.Count == 1) ? " item" : " items";

            //จำนวนสินค้าทั้งหมด
            textTotalItems.Text = " " + tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity).ToString("#,##0");
            textCashierName.Text = tranWithDetails.tran.SellerName?.ToString();
            textSubtotalAmount.Text = Utils.DisplayDecimal(tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Amount));
            textVatAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);
            textTotalAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal);
        }

        private void SetupTransactionDetails()
        {
            textTranNo.Text = tranWithDetails.tran.TranNo;
            var timezoneslocal = TimeZoneInfo.Local;
            var date = tranWithDetails.tran.TranDate;
            textDate.Text = TimeZoneInfo.ConvertTimeFromUtc(date, timezoneslocal).ToString("dd/MM/yyyy HH:mm tt", new CultureInfo("en-US"));
            textCustomerName.Text = tranWithDetails.tran.CustomerName;
        }

        private void SetupMerchantInfo()
        {
            textNameMerchant.Text = DataCashingAll.MerchantLocal.Name;
            var path = DataCashingAll.MerchantLocal.LogoLocalPath;
            using (var imgProfileMerchant = FindViewById<ImageView>(Resource.Id.imgProfileMerchant))
            {
                if (!string.IsNullOrEmpty(path))
                {
                    Android.Net.Uri uri = Android.Net.Uri.Parse(path);
                    imgProfileMerchant.SetImageURI(uri);
                }
                else
                {
                    imgProfileMerchant.SetBackgroundResource(Resource.Mipmap.LogoDefault);
                }
            }
        }

        private async Task LoadTranDetailsAsync()
        {
            TransManage transManage = new TransManage();
            Tran getTran = await transManage.GetTran(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);
            localprintCounter = (getTran == null) ? 0 : (int)getTran.PrintCounterLocal;
        }

        private void SetupServiceCharge()
        {
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
        }

        private void SetupVat()
        {
            //Vat
            string getvat = Utils.DisplayDouble(tranWithDetails.tran.TaxRate == null ? 0 : tranWithDetails.tran.TaxRate.Value);
            if (tranWithDetails.tran.TaxRate == 0)
            {
                lnVat.Visibility = ViewStates.Gone;
            }

            textVat.Text = GetString(Resource.String.vat) + " " + getvat + "%";
            textVatAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);
        }

        private void SetupDiscount()
        {
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
        }

        private void InitializeUIElements()
        {
            //imgProfileMerchant = FindViewById<ImageView>(Resource.Id.imgProfileMerchant);
            recyclerviewReceipt = FindViewById<RecyclerView>(Resource.Id.recyclerViewReceipt);
            textNameMerchant = FindViewById<TextView>(Resource.Id.textNameMerchant);
            textTranNo = FindViewById<TextView>(Resource.Id.textTranNo);
            textDate = FindViewById<TextView>(Resource.Id.textDate);
            textCustomerName = FindViewById<TextView>(Resource.Id.textCustomerName);
            textTotalItems = FindViewById<TextView>(Resource.Id.textTotalItems);
            textUnit = FindViewById<TextView>(Resource.Id.textUnit);
            textSubtotalAmount = FindViewById<TextView>(Resource.Id.textSubtotalAmount);
            textMemberDiscount = FindViewById<TextView>(Resource.Id.textMemberDiscount);
            textMemberDiscountAmount = FindViewById<TextView>(Resource.Id.textMemberDiscountAmount);
            textVat = FindViewById<TextView>(Resource.Id.textVat);
            textVatAmount = FindViewById<TextView>(Resource.Id.textVatAmount);
            textDiscount = FindViewById<TextView>(Resource.Id.textDiscount);
            textDiscountAmount = FindViewById<TextView>(Resource.Id.textDiscountAmount);
            textTotalAmount = FindViewById<TextView>(Resource.Id.textTotalAmount);
            textCashAmount = FindViewById<TextView>(Resource.Id.textCashAmount);
            textChangeAmount = FindViewById<TextView>(Resource.Id.textChangeAmount);
            textCashierName = FindViewById<TextView>(Resource.Id.textCashierName);
            frameLayoutReciept = FindViewById<FrameLayout>(Resource.Id.frameLayoutReciept);
            frameRemark = FindViewById<FrameLayout>(Resource.Id.frameRemark);
            textService = FindViewById<TextView>(Resource.Id.textService);
            textServiceAmount = FindViewById<TextView>(Resource.Id.textServiceAmount);
            textRemarkDetail = FindViewById<TextView>(Resource.Id.textRemarkDetail);
            lnReceiptImage = FindViewById<LinearLayout>(Resource.Id.lnReceiptImage);
            /*imageReceiptImage = FindViewById<ImageView>(Resource.Id.imageReceiptImage)*/
            lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnPrintCounter = FindViewById<LinearLayout>(Resource.Id.lnPrintCounter);
            txtPrintCounter = FindViewById<TextView>(Resource.Id.txtPrintCounter);
            lnService = FindViewById<FrameLayout>(Resource.Id.lnService);
            lnVat = FindViewById<FrameLayout>(Resource.Id.lnVat);
            lnDiscountMember = FindViewById<FrameLayout>(Resource.Id.lnDiscountMember);
            lnDiscount = FindViewById<FrameLayout>(Resource.Id.lnDiscount);
            lnShowPrint = FindViewById<LinearLayout>(Resource.Id.lnShowPrint);
            lnShowPDF = FindViewById<LinearLayout>(Resource.Id.lnShowPDF);
            lnShowEmail = FindViewById<LinearLayout>(Resource.Id.lnShowEmail);
            lnShowShar = FindViewById<LinearLayout>(Resource.Id.lnShowShar);
            frameBill = FindViewById<FrameLayout>(Resource.Id.frameBill);
            scrollViewReciep = FindViewById<ScrollView>(Resource.Id.scrollViewReciep);

            lnBack.Click += LnBack_Click;
            lnPrintCounter.Click += LnPrintCounter_Click;
            lnShowPrint.Click += LnPrint_Click;
            lnShowPDF.Click += LnPDF_Click;
            lnShowEmail.Click += LnEmail_Click;
            lnShowShar.Click += LnShare_Click;
            frameLayoutReciept.Click += FrameLayoutReciept_Click;
        }

        private void LnPrintCounter_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, GetString(Resource.String.receipt_activity_reprint) + txtPrintCounter.Text, ToastLength.Long).Show();
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            NewSale();
        }

        private async void LnShare_Click(object sender, EventArgs e)
        {
            try
            {
                lnShowShar.Enabled = false;
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
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
                string filePath = DataCashingAll.PathImageBill;
                string fullName = Path.Combine(filePath, fullpath);
                if (File.Exists(fullName))
                {
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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return Task.FromResult(false);
            }
        }

        private async void LnEmail_Click(object sender, EventArgs e)
        {
            try
            {
                lnShowEmail.Enabled = false;
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
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
                bitmap.Dispose();

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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }


        private async void LnPDF_Click(object sender, EventArgs e)
        {
            try
            {
                lnShowPDF.Enabled = false;
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
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

        public Task CreatePDF(string filename)
        {
            DialogLoading dialogLoading = new DialogLoading();
            dialogLoading.Cancelable = false;
            dialogLoading.Show(SupportFragmentManager, nameof(DialogLoading));
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
                    Toast.MakeText(this, "ไม่มีแอปพลิเคชันที่ใช้งานได้", ToastLength.Short).Show();
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OpenFile at Bill Detail");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return Task.FromResult(false);
            }
        }

        private void LnPrint_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
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
                    Toast.MakeText(this, GetString(Resource.String.settingprinter), ToastLength.Short).Show();
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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
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
            DeletePictureinFloder();
            tranWithDetails = null;
            tranWithDetails = await Utils.initialData();

            PosActivity.totlaItems = 0;
            PosActivity.SetTranDetail(tranWithDetails);

            if (CartActivity.cart != null)
            {
                CartActivity.cart.lnRemark.Visibility = Android.Views.ViewStates.Gone;
            }

            if (CartScanActivity.scan != null)
            {
                CartScanActivity.lnRemark.Visibility = Android.Views.ViewStates.Gone;
            }

            this.Finish();
        }

        public static void SetTranDeail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        public override void OnBackPressed()
        {
            NewSale();
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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

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
                    BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
                    ICollection<BluetoothDevice> bondedDevices = bluetoothAdapter.BondedDevices;
                    var printer = bondedDevices.Where(x => x.Name == DataCashingAll.setting.BLUETOOTH1 && x.Address == DataCashingAll.setting.IPADDRESS).FirstOrDefault();
                    if (printer == null)
                    {
                        Toast.MakeText(this, "Bluetooth DisConnected", ToastLength.Short).Show();
                    }
                    else
                    {
                        BluetoothDevice device = bluetoothAdapter.GetRemoteDevice(printer.Address);
                        Java.Util.UUID UDID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
                        BluetoothSocket socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));
                        socket.Connect();

                        if (DataCashingAll.setting.COMMAND != "Epson Command")
                        {
                            try
                            {
                                if (!socket.IsConnected)
                                {
                                    socket.Connect();
                                    DataCashingAll.addresssame = DataCashingAll.setting.IPADDRESS;
                                }
                            }
                            catch (Exception)
                            {
                                socket.Close();

                                socket = printer.CreateInsecureRfcommSocketToServiceRecord(UDID);
                                socket.Connect();

                            }
                        }
                        try
                        {

                            if (!socket.IsConnected)
                            {
                                socket.Connect();
                                DataCashingAll.addresssame = DataCashingAll.setting.IPADDRESS;
                            }

                            if (socket.IsConnected)
                            {
                                Stream outputStream = socket.OutputStream;
                                outputStream.Flush();

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

                                    await outputStream.WriteAsync(bytelist.ToArray());
                                    var imageforprinttext = await DrawString(tran);
                                    foreach (string txt in imageforprinttext)
                                    {
                                        var txt1 = txt;
                                        var x2 = ThaiLength(txt);
                                        var enc = System.Text.Encoding.GetEncoding("Windows-874");
                                        byte[] bytes = enc.GetBytes(txt1);
                                        var x = txt1.Length;
                                        await outputStream.WriteAsync(bytes);
                                        await outputStream.WriteAsync(new byte[] { (byte)10 });
                                    }
                                    byte[] byteArray = Encoding.ASCII.GetBytes("\n\n");
                                    await outputStream.WriteAsync(byteArray, 0, byteArray.Length);
                                }
                                else
                                {
                                    await outputStream.WriteAsync(bytesData, 0, bytesData.Length);
                                    byte[] byteArray = Encoding.ASCII.GetBytes("\n\n");
                                    await outputStream.WriteAsync(byteArray, 0, byteArray.Length);
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
                                if (DataCashingAll.setting.COMMAND == "Epson Command")
                                {
                                    socket.Close();
                                }
                                PostPrintcounter(tran);
                                await outputStream.FlushAsync();
                            }
                            else
                            {
                                Toast.MakeText(this, "Bluetooth DisConnected", ToastLength.Short).Show();
                            }
                        }
                        catch (IOException ee)
                        {
                            Toast.MakeText(this, ee.Message, ToastLength.Short).Show();
                        }
                        catch (Exception)
                        {
                            if (socket != null)
                            {
                                try
                                {
                                    socket.Close();
                                }
                                catch (IOException)
                                {

                                }
                                socket = null;
                            }
                        }
                        finally
                        {
                            if (socket != null)
                            {
                                socket.Dispose();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RunOnUiThread(() =>
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
            if (await GabanaAPI.CheckSpeedConnection())
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
                if (!string.IsNullOrEmpty(check))
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
                if (!string.IsNullOrEmpty(res2))
                {
                    itemName.Add("  " + res2);
                }
                if (!string.IsNullOrEmpty(res3))
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

        private async void PostPrintcounter(TranWithDetailsLocal tranWithDetails)
        {
            try
            {
                //บันทึกการพิมพ์ที่ Local 
                TransManage transManage = new TransManage();
                TranWithDetailsLocal tran = tranWithDetails;
                Tran getTran = await transManage.GetTran(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tran.tran.TranNo);
                if (getTran != null)
                {
                    localprintCounter = localprintCounter + 1;
                    getTran.PrintCounterLocal = localprintCounter;
                    var updatePrint = await transManage.UpdateTran(getTran);
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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
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

        bool deviceAsleep = false;
        bool openPage = false;
        public DateTime pauseDate = DateTime.Now;

        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
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

        public override void OnUserInteraction()
        {
            base.OnUserInteraction();
            if (deviceAsleep)
            {
                deviceAsleep = false;
                TimeSpan span = DateTime.Now.Subtract(pauseDate);

                long DISCONNECT_TIMEOUT = 5 * 60 * 1000; // 1 min = 1 * 60 * 1000 ms
                if ((span.Minutes * 60 * 1000) >= DISCONNECT_TIMEOUT)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(SplashActivity)));
                    this.Finish();
                    return;
                }
                else
                {
                    pauseDate = DateTime.Now;
                }
            }
            else
            {
                pauseDate = DateTime.Now;

            }
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                PinCodeActitvity.SetPincode("Pincode");
                openPage = true;
            }
        }

        public bool CheckPermission()
        {
            Permission permissionCamera = CheckSelfPermission(Manifest.Permission.Camera);
            Permission permissionRead = CheckSelfPermission(Manifest.Permission.ReadExternalStorage);
            Permission permissionWrite = CheckSelfPermission(Manifest.Permission.WriteExternalStorage);
            Permission permissionBluetooth = CheckSelfPermission(Manifest.Permission.Bluetooth);
            Permission permissionBluetoothC = CheckSelfPermission(Manifest.Permission.BluetoothConnect);

            string[] PERMISSIONS;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                PERMISSIONS = new string[]
                {
                    "android.permission.READ_EXTERNAL_STORAGE",
                    "android.permission.WRITE_EXTERNAL_STORAGE",
                    "android.permission.CAMERA",
                    "android.permission.BLUETOOTH",
                    "android.permission.BLUETOOTH_CONNECT"
                };
                if (permissionCamera != Permission.Granted
                    || permissionRead != Permission.Granted
                    || permissionWrite != Permission.Granted
                    || permissionBluetooth != Permission.Granted
                    || permissionBluetoothC != Permission.Granted)
                {
                    RequestPermissions(PERMISSIONS, 1);
                    return false;
                }
                return true;
            }
            else
            {
                PERMISSIONS = new string[]
               {
                    "android.permission.READ_EXTERNAL_STORAGE",
                    "android.permission.WRITE_EXTERNAL_STORAGE",
                    "android.permission.CAMERA",
                    "android.permission.BLUETOOTH"
                };
                if (permissionCamera != Permission.Granted || permissionRead != Permission.Granted || permissionWrite != Permission.Granted || permissionBluetooth != Permission.Granted)
                {
                    RequestPermissions(PERMISSIONS, 1);
                    return false;
                }
                return true;
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();

                bitmap?.Dispose();
                //imgProfileMerchant?.Dispose();
                recyclerviewReceipt?.Dispose();
                textNameMerchant?.Dispose();
                textTranNo?.Dispose();
                textDate?.Dispose();
                textCustomerName?.Dispose();
                textTotalItems?.Dispose();
                textUnit?.Dispose();
                textSubtotalAmount?.Dispose();
                textMemberDiscount?.Dispose();
                textMemberDiscountAmount?.Dispose();
                textVat?.Dispose();
                textVatAmount?.Dispose();
                textDiscount?.Dispose();
                textDiscountAmount?.Dispose();
                textTotalAmount?.Dispose();
                textCashAmount?.Dispose();
                textChangeAmount?.Dispose();
                textCashierName?.Dispose();
                frameLayoutReciept?.Dispose();
                frameRemark?.Dispose();
                textService?.Dispose();
                textServiceAmount?.Dispose();
                textRemarkDetail?.Dispose();
                lnReceiptImage?.Dispose();
                //imageReceiptImage?.Dispose();
                lnBack?.Dispose();
                lnPrintCounter?.Dispose();
                txtPrintCounter?.Dispose();
                lnService?.Dispose();
                lnVat?.Dispose();
                lnDiscountMember?.Dispose();
                lnDiscount?.Dispose();
                lnShowPrint?.Dispose();
                lnShowPDF?.Dispose();
                lnShowEmail?.Dispose();
                lnShowShar?.Dispose();
                frameBill?.Dispose();
                scrollViewReciep?.Dispose();

                GC.SuppressFinalize(this);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnDestroy at Receipt");
            }
        }
    }
}