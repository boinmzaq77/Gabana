using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AutoMapper;
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
//using iTextSharp.text;
//using iTextSharp.text.pdf;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]

    public class BillHistoryDetailActivity : AppCompatActivity
    {
        public BillHistoryDetailActivity billdetail;
        TranWithDetailsLocal tranWithDetailsLocal;
        public static TransHistoryNew tranhistory;
        TransManage transManage;
        TranDetailItemManage tranDetailItemManage;
        TranPaymentManage tranPaymentManage;
        TranTradDiscountManage tranTradDiscountManage;
        TranDetailItemToppingManage toppingManage;
        CultureInfo cultureUS = new CultureInfo("en-US");
        double total = 0.00;
        string CartDiscount = "";
        FrameLayout frameBill, frameRemark;
        public static AssetManager assets;
        RelativeLayout lnStatusVoid;
        LinearLayout lnShowButton, lnShowVoid, lnShowShar, lnShowEmail, lnShowPDF, lnShowPrint;
        TextView textNameMerchant, textTranNo, textDate, textCustomerName, textTotalItems, textUnit, textSubtotalAmount,
            textMemberDiscount, textMemberDiscountAmount, textVat, textVatAmount, textDiscount, textDiscountAmount,
            textTotalAmount, textCashAmount, textChangeAmount, textCashierName, txtTranNo, textService, textServiceAmount,
            textRemarkDetail;
        ImageView imgProfileMerchant;
        RecyclerView recyclerviewReceipt;
        static byte[] bytesData;
        Android.Graphics.Bitmap bitmap;
        FrameLayout lnVat, lnService, lnMemberDiscount, lnDiscount;
        string CURRENCYSYMBOLS, LoginType;
        LinearLayout lnReceiptImage;
        ImageView imageReceiptImage;
        TextView txtPrintCounter;
        int lengThName = 28, printCounter = 0;
        MainDialog dialog;
        static string branchID;
        double disDiscont = 0.0;
        Tran getTran = new Tran();
        bool CheckNet = false;
        ORM.MerchantDB.Branch BranchDetail = new ORM.MerchantDB.Branch();
        string SubTotal = "";
        string txtshowlocal = string.Empty;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.billhistory_activity_detail);

                billdetail = this;
                assets = this.Assets;

                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                var w = mainDisplayInfo.Width / 6;
                lnShowPrint = FindViewById<LinearLayout>(Resource.Id.lnShowPrint);
                lnShowPDF = FindViewById<LinearLayout>(Resource.Id.lnShowPDF);
                lnShowEmail = FindViewById<LinearLayout>(Resource.Id.lnShowEmail);
                lnShowShar = FindViewById<LinearLayout>(Resource.Id.lnShowShar);
                lnShowVoid = FindViewById<LinearLayout>(Resource.Id.lnShowVoid);

                lnReceiptImage = FindViewById<LinearLayout>(Resource.Id.lnReceiptImage);
                imageReceiptImage = FindViewById<ImageView>(Resource.Id.imageReceiptImage);
                lnShowPrint.LayoutParameters.Height = Convert.ToInt32(w);
                lnShowPrint.LayoutParameters.Width = Convert.ToInt32(w);
                lnShowPDF.LayoutParameters.Height = Convert.ToInt32(w);
                lnShowPDF.LayoutParameters.Width = Convert.ToInt32(w);
                lnShowEmail.LayoutParameters.Height = Convert.ToInt32(w);
                lnShowEmail.LayoutParameters.Width = Convert.ToInt32(w);
                lnShowShar.LayoutParameters.Height = Convert.ToInt32(w);
                lnShowShar.LayoutParameters.Width = Convert.ToInt32(w);
                lnShowVoid.LayoutParameters.Height = Convert.ToInt32(w);
                lnShowVoid.LayoutParameters.Width = Convert.ToInt32(w);

                lnVat = FindViewById<FrameLayout>(Resource.Id.lnVat);
                lnService = FindViewById<FrameLayout>(Resource.Id.lnService);
                lnMemberDiscount = FindViewById<FrameLayout>(Resource.Id.lnMemberDiscount);
                lnDiscount = FindViewById<FrameLayout>(Resource.Id.lnDiscount);

                imgProfileMerchant = FindViewById<ImageView>(Resource.Id.imgProfileMerchant);
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
                textService = FindViewById<TextView>(Resource.Id.textService);
                textServiceAmount = FindViewById<TextView>(Resource.Id.textServiceAmount);
                textTotalAmount = FindViewById<TextView>(Resource.Id.textTotalAmount);
                textCashAmount = FindViewById<TextView>(Resource.Id.textCashAmount);
                textChangeAmount = FindViewById<TextView>(Resource.Id.textChangeAmount);
                textCashierName = FindViewById<TextView>(Resource.Id.textCashierName);
                textRemarkDetail = FindViewById<TextView>(Resource.Id.textRemarkDetail);
                txtPrintCounter = FindViewById<TextView>(Resource.Id.txtPrintCounter);

                CheckJwt();

                LinearLayout lnPrintCounter = FindViewById<LinearLayout>(Resource.Id.lnPrintCounter);
                lnPrintCounter.Click += LnPrintCounter_Click;

                txtTranNo = FindViewById<TextView>(Resource.Id.txtTranNo);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
                frameRemark = FindViewById<FrameLayout>(Resource.Id.frameRemark);
                lnBack.Click += LnBack_Click;
                btnBack.Click += LnBack_Click;

                lnStatusVoid = FindViewById<RelativeLayout>(Resource.Id.lnStatusVoid);
                lnShowButton = FindViewById<LinearLayout>(Resource.Id.lnShowButton);
                frameBill = FindViewById<FrameLayout>(Resource.Id.frameBill);

                lnStatusVoid.Visibility = ViewStates.Gone;
                lnShowButton.Visibility = ViewStates.Visible;
                lnShowPrint.Click += LnPrint_Click;
                lnShowPDF.Click += LnPDF_Click;
                lnShowEmail.Click += LnEmail_Click;
                lnShowShar.Click += LnShare_Click;
                lnShowVoid.Click += LnVoid_Click;

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

                frameBill.SetBackgroundColor(Android.Graphics.Color.White);
                tranWithDetailsLocal = new TranWithDetailsLocal();

                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;
                if (CURRENCYSYMBOLS == null) CURRENCYSYMBOLS = "฿";
                txtshowlocal = Preferences.Get("Language", "");

                branchID = BillHistoryActivity.branchID;

                _ = TinyInsights.TrackPageViewAsync("OnCreate : BillHistoryDetailActivity");

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Bill Detail");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
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
                        if (dialog == null)
                        {
                            dialog = new MainDialog();
                        }
                        else
                        {
                            dialog.Dismiss();
                            dialog = new MainDialog();
                        }
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.void_dialog_main.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("Void", "Void");
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                    }
                    catch (Exception ex)
                    {
                        lnShowVoid.Enabled = true;
                        await TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("LnVoid_Click at Bill Detail : owner");
                        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    }
                }
                else
                {
                    #region Dialog not permission
                    //try
                    //{
                    //    if (dialog == null)
                    //    {
                    //        dialog = new MainDialog();
                    //    }
                    //    else
                    //    {
                    //        dialog.Dismiss();
                    //        dialog = new MainDialog();
                    //    }
                    //    Bundle bundle = new Bundle();
                    //    String myMessage = Resource.Layout.void_dialog_main.ToString();
                    //    bundle.PutString("message", myMessage);
                    //    bundle.PutString("Void", "VoidRole");
                    //    dialog.Arguments = bundle;
                    //    dialog.Show(SupportFragmentManager, myMessage);
                    //}
                    //catch (Exception ex)
                    //{
                    //    lnShowVoid.Enabled = true;
                    //    _ = TinyInsights.TrackErrorAsync(ex);
                    //    _ = TinyInsights.TrackPageViewAsync("LnVoid_Click at Bill Detail : อื่นๆ");
                    //    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    //} 
                    #endregion

                    Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                }
                lnShowVoid.Enabled = true;

            }
            catch (Exception ex)
            {
                lnShowVoid.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnVoid_Click at Bill Detail");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private async void LnShare_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                lnShowShar.Enabled = false;
               

                await CreateBitmap();
                toGrayscale();
                await SaveandShare("Receipt " + tranWithDetailsLocal.tran.TranNo, "Receipt " + tranWithDetailsLocal.tran.TranNo + " " + Utils.ShowDate(tranWithDetailsLocal.tran.TranDate), "Receipt " + tranWithDetailsLocal.tran.TranNo);

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                lnShowShar.Enabled = true;

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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return Task.FromResult(false);
            }
        }
        private async void LnEmail_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
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
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
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
                    Toast.MakeText(this, GetString(Resource.String.nonapplication), ToastLength.Short).Show();
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OpenFile at Bill Detail");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return Task.FromResult(false);
            }
        }
        private async void LnPrint_Click(object sender, EventArgs e)
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
                    Print(tranWithDetailsLocal);
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
        public static byte[] ImageToByteArray(Android.Graphics.Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 0, stream);
                return stream.ToArray();
            }
        }
        protected override async void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                CheckNet = await GabanaAPI.CheckSpeedConnection();

                if (tranhistory != null)
                {
                    ShowBillDetail();
                }
            }
            catch (Exception ex)
            { 
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at Bill Detaial");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                base.OnRestart();
            }
        }
        private async void ShowBillDetail()
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
               
                TranWithDetailsLocal tranWithDetails;
                if (CheckNet)
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
                    dialogLoading.Dismiss();
                    Toast.MakeText(this, GetString(Resource.String.notdata), ToastLength.Short).Show();
                    this.Finish();
                    return;
                }

                //Get Tran 
                //transManage = new TransManage();
                //getTran = await transManage.GetTran(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);
                //printCounter = (int)getTran.PrintCounter + (int)getTran.PrintCounterLocal;

                printCounter = (int)tranWithDetails.tran.PrintCounter + (int)tranWithDetails.tran.PrintCounterLocal;


                txtTranNo.Text = tranWithDetails.tran.TranNo;
                textNameMerchant.Text = DataCashingAll.MerchantLocal.Name;
                //offline Image     
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
                textDate.Text = TimeZoneInfo.ConvertTimeFromUtc(date, timezoneslocal).ToString("dd/MM/yyy HH:mm tt", new CultureInfo("en-US"));
                //textDate.Text = TimeZoneInfo.ConvertTimeFromUtc(date, timezoneslocal).ToString("dd/MM/yyy", new CultureInfo("en-US")) + " " + tranWithDetails.tran.TranDate.ToString("HH:mm tt");
                textCustomerName.Text = tranWithDetails.tran.CustomerName;

                Receipt_Adapter_Item receipt_adapter_item = new Receipt_Adapter_Item(tranWithDetails);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(this, 1, 1, false);
                recyclerviewReceipt.SetLayoutManager(gridLayoutManager);
                recyclerviewReceipt.HasFixedSize = true;
                recyclerviewReceipt.SetItemViewCacheSize(100);
                recyclerviewReceipt.SetAdapter(receipt_adapter_item);

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
                    if (DataCashing.SysCustomerID == 999)
                    {
                        lnMemberDiscount.Visibility = ViewStates.Gone;
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
                            lnMemberDiscount.Visibility = ViewStates.Visible;
                            disMember = Convert.ToDouble(tranTradDiscountMember.TradeDiscNoneVat + tranTradDiscountMember.TradeDiscHaveVat);
                            textMemberDiscountAmount.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disMember));
                        }
                        else
                        {
                            lnMemberDiscount.Visibility = ViewStates.Gone;
                        }
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

                    textVat.Text = GetString(Resource.String.vat) + " " + getvat + "%";

                    textVatAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);

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

                    Receipt_Adapter_Cash receipt_Adapter_Cash = new Receipt_Adapter_Cash(paymenttypeamout);
                    RecyclerView recyclerViewCash = FindViewById<RecyclerView>(Resource.Id.recyclerViewCash);
                    GridLayoutManager gridLayoutManager2 = new GridLayoutManager(this, 1, 1, false);
                    recyclerViewCash.SetLayoutManager(gridLayoutManager2);
                    recyclerViewCash.HasFixedSize = true;
                    recyclerViewCash.SetItemViewCacheSize(20);
                    recyclerViewCash.SetAdapter(receipt_Adapter_Cash);

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

                    if (string.IsNullOrEmpty(tranWithDetails.tran.Comments))
                    {
                        frameRemark.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        textRemarkDetail.Text = tranWithDetails.tran.Comments?.ToString();
                    }
                }

                if (tranhistory.fCancel == 1)
                {
                    lnStatusVoid.Visibility = ViewStates.Visible;
                    lnShowButton.Visibility = ViewStates.Gone;
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
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowBillDetail at Bill Detaial");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private void LnPrintCounter_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, GetString(Resource.String.receipt_activity_reprint) + txtPrintCounter.Text, ToastLength.Long).Show();
        }
        internal async Task Print(TranWithDetailsLocal tranWithDetails)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    lnShowPrint.Enabled = false;
                });
                TranWithDetailsLocal tran = tranWithDetails;

                #region Current Code
                if (DataCashingAll.setting.PRINTTYPE == "Image")
                {
                    if (DataCashingAll.setting?.TYPEPAGE.Substring(0, 2) == "80")
                    {
                        lengThName = 35;
                    }

                    List<List<KeyValuePair<string, string>>> items = new List<List<KeyValuePair<string, string>>>();
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

                    if (CheckNet)
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
                        new KeyValuePair<string, string>("@Total", tran.tran.Total.ToString("#,##0.00")),
                        new KeyValuePair<string, string>("@GrandTotal", tran.tran.GrandPayment.ToString("#,##0.00")),

                    },
                        Details = items,
                        Footer = items

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
                        BluetoothSocket socket = device.CreateRfcommSocketToServiceRecord(UDID);
                        socket.Connect();
                        try
                        {
                            Java.Lang.Thread.Sleep(3000);
                        }
                        catch (Exception)
                        {
                        }

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
                        catch (Exception ex)
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
                #endregion
                //PostPrintcounter(tran);

                RunOnUiThread(() =>
                {
                    lnShowPrint.Enabled = true;
                });

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
            if (CheckNet)
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
            data.Add(escPos.ReplaceSpacebar2(tran.tran.TranNo.ToString(), Utils.PrintDateTime(tran.tran.TranDate) ));
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


            double discount;
            var tranTradDiscount = tran.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
            if (tranTradDiscount != null)
            {
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
                    itemName.Add("  "+res2);
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
        private async void PostPrintcounter(TranWithDetailsLocal t)
        {
            try
            {
                //บันทึกการพิมพ์ที่ Local    
                TranWithDetailsLocal tranDetail = t;
                transManage = new TransManage();
                getTran = await transManage.GetTran(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranDetail.tran.TranNo);
                if (getTran != null)
                {
                    printCounter = printCounter + 1;
                    getTran.PrintCounterLocal = printCounter;
                    transManage = new TransManage();
                    var updatePrint = await transManage.UpdatePrintCounterTran(getTran);
                    txtPrintCounter.Text = (printCounter).ToString("#,##0");

                    //เพิ่มฟังก์ชันสำหรับนับการพิมพ์ PrintCounter
                    UtilsAll.PostPrintCounter(Convert.ToInt32(branchID), tranDetail.tran.TranNo, Utils.ChangeDateTime(tranDetail.tran.TranDate), 1, tranDetail.tran);
                }
                else
                {
                    //เพิ่มฟังก์ชันสำหรับนับการพิมพ์ PrintCounter
                    UtilsAll.PostPrintCounter(Convert.ToInt32(branchID), tranDetail.tran.TranNo, Utils.ChangeDateTime(tranDetail.tran.TranDate), 1, tranDetail.tran);
                    txtPrintCounter.Text = (tranDetail.tran.PrintCounter).ToString("#,##0");
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            tranhistory = null;
            DeletePictureinFolder();
            DataCashing.isModifyBillhistory = false;
        }
        //ลบรูปที่สร้างตอน share,Email ,PDF
        void DeletePictureinFolder()
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
                    string name = file.FullName;
                    if (name.Contains(".png"))
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DeletePictureinFolder at Bill Detaial");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
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
                tranWithDetailsLocal.tran = Imapper.Map<ORM.Period.Tran, Tran>(tranDetails.tran);
                tranWithDetailsLocal.tran.LocalDataStatus = 'U';

                //TranDetailItemWithTopping
                TranDetailItemNew tranDetailItem = new TranDetailItemNew();
                Model.TranDetailItemWithTopping tranDetailItemWithTopping = new Model.TranDetailItemWithTopping();
                List<Model.TranDetailItemWithTopping> lsttrandetailTopping = new List<Model.TranDetailItemWithTopping>();
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

                    tranDetailItemWithTopping = new Model.TranDetailItemWithTopping();
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
                    Utils.SetImage(imageReceiptImage, path);
                }
                else
                {
                    lnReceiptImage.Visibility = ViewStates.Gone;
                }
                return tranWithDetailsLocal;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetOnlineHistoryDetail at Bill Detaial");
                return new TranWithDetailsLocal();
            }
        }
        //Get TranWithDetails Offline
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
                Gabana.Model.TranDetailItemWithTopping detailItemWithTopping = new Model.TranDetailItemWithTopping();
                List<Model.TranDetailItemWithTopping> lstdetailItemWithTopping = new List<Model.TranDetailItemWithTopping>();

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
                    detailItemWithTopping = new Model.TranDetailItemWithTopping()
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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return new TranWithDetailsLocal();
            }
        }
        public static void SetTranHistory(TransHistoryNew t)
        {
            tranhistory = t;
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
        //private async Task<Android.Graphics.Bitmap> getBillDeatilView()
        //{
        //    try
        //    {

        //        TranWithDetailsLocal tran = tranWithDetailsLocal;
        //        List<List<KeyValuePair<string, string>>> items = new List<List<KeyValuePair<string, string>>>();
        //        for (int i = 0; i < tran.tranDetailItemWithToppings.Count; i++)
        //        {
        //            if (tran.tranDetailItemWithToppings[i].tranDetailItem.ItemName.Length > lengThName)
        //            {
        //                var itemNames = Utils.SplitItemName(lengThName, tran.tranDetailItemWithToppings[i].tranDetailItem.ItemName);

        //                items.Add(new List<KeyValuePair<string, string>>()
        //                {
        //                new KeyValuePair<string, string>("@QuantityItem",tran.tranDetailItemWithToppings[i].tranDetailItem.Quantity.ToString("#,##0") + "x " ),
        //                new KeyValuePair<string, string>("@ItemName",itemNames[0]),
        //                new KeyValuePair<string, string>("@ItemPrice",tran.tranDetailItemWithToppings[i].tranDetailItem.Amount.ToString("#,##0.00"))
        //                });
        //                items.Add(new List<KeyValuePair<string, string>>()
        //                {
        //                new KeyValuePair<string, string>("@QuantityItem"," " ),
        //                new KeyValuePair<string, string>("@ItemName",itemNames[1]),
        //                new KeyValuePair<string, string>("@ItemPrice"," ")
        //                });
        //            }
        //            else
        //            {
        //                items.Add(new List<KeyValuePair<string, string>>()
        //            {
        //            new KeyValuePair<string, string>("@QuantityItem",tran.tranDetailItemWithToppings[i].tranDetailItem.Quantity.ToString("#,##0") + "x " ),
        //            new KeyValuePair<string, string>("@ItemName",tran.tranDetailItemWithToppings[i].tranDetailItem.ItemName),
        //            new KeyValuePair<string, string>("@ItemPrice",tran.tranDetailItemWithToppings[i].tranDetailItem.Amount.ToString("#,##0.00"))
        //            });
        //            }

        //        }

        //        int quantity = 0;
        //        foreach (var item in tran.tranDetailItemWithToppings)
        //        {
        //            quantity += (int)item.tranDetailItem.Quantity;
        //        }
        //        BranchManage branchManage = new BranchManage();
        //        var lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);

        //        var branch = lstBranch.Where(x => x.SysBranchID == tran.tran.SysBranchID).FirstOrDefault();
        //        string branchaddress = "";
        //        branchaddress = await Utils.SetTextAddressAsync(branch);
        //        var address = Utils.SplitAddress(branchaddress);

        //        //string CusAddress1 = "";
        //        //string CusAddress2 = "";
        //        //List<string> CusAddress = new List<string>();
        //        //CustomerManage customerManage = new CustomerManage();
        //        //var dataCustomer = await customerManage.GetCustomer(DataCashingAll.MerchantId, (int)tran.tran.SysCustomerID);
        //        //if (dataCustomer != null)
        //        //{
        //        //    ORM.MerchantDB.Branch cusBranch = new ORM.MerchantDB.Branch()
        //        //    {
        //        //        Address = dataCustomer?.Address,
        //        //        DistrictsId = dataCustomer?.DistrictsId,
        //        //        AmphuresId = dataCustomer?.AmphuresId,
        //        //        ProvincesId = dataCustomer?.ProvincesId,
        //        //    };
        //        //    string StrCusaddress = "";
        //        //    StrCusaddress = await Utils.SetTextAddressAsync(cusBranch);
        //        //    CusAddress = Utils.SplitAddress(StrCusaddress);
        //        //    CusAddress1 = CusAddress[0];
        //        //    CusAddress2 = CusAddress[1];
        //        //}

        //        string merchantAddress1 = address[0];
        //        string merchantAddress2 = address[1];
        //        string merchantAddress3 = address[2];
        //        string vat1 = "";
        //        string vat2 = "";
        //        string vat3 = "";
        //        string service1 = "ค่าบริการ";
        //        string service2 = "Service";
        //        string service3 = "0.00";
        //        string Tel = "";
        //        string member1 = "สมาชิก";
        //        string member2 = "Member";
        //        string member3 = "0.00";
        //        string discountBill1 = "ส่วนลด";
        //        string discountBill2 = "Discount";
        //        string discountBill3 = "0.00";

        //        if (!string.IsNullOrEmpty(branch.Tel))
        //        {
        //            Tel = addTextTel(branch.Tel);
        //        }

        //        //Service charge
        //        if (!string.IsNullOrEmpty(tranWithDetailsLocal.tran.FmlServiceCharge))
        //        {
        //            var checkservice = tranWithDetailsLocal.tran.FmlServiceCharge.IndexOf('%');
        //            if (checkservice == -1)
        //            {
        //                var ServiceCharge = tranWithDetailsLocal.tran.FmlServiceCharge;
        //                if (Convert.ToDecimal(ServiceCharge) > 0)
        //                {
        //                    service1 = "ค่าบริการ " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge));
        //                    service2 = "Service ";
        //                    service3 = Utils.DisplayDecimal(Convert.ToDecimal(tranWithDetailsLocal.tran.ServiceCharge));
        //                }

        //            }
        //            else
        //            {
        //                string[] split = tranWithDetailsLocal.tran.FmlServiceCharge.Split('%');
        //                var ServiceCharge = split[0];
        //                if (Convert.ToDecimal(ServiceCharge) > 0)
        //                {
        //                    service1 = "ค่าบริการ " + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)) + "%";
        //                    service2 = "Service ";
        //                    service3 = Utils.DisplayDecimal(Convert.ToDecimal(tranWithDetailsLocal.tran.ServiceCharge));
        //                }
        //            }
        //        }

        //        //Discount From Member 
        //        double disMember = 0;
        //        var tranTradDiscountMember = tranWithDetailsLocal.tranTradDiscounts.Where(x => x.DiscountType == "PS").FirstOrDefault();
        //        if (tranTradDiscountMember != null)
        //        {
        //            var check = tranTradDiscountMember.FmlDiscount.IndexOf('%');
        //            if (check == -1)
        //            {
        //                member1 = "สมาชิก " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Replace("%", "")));
        //                member2 = "Member ";
        //                disMember = Convert.ToDouble(tranTradDiscountMember.TradeDiscNoneVat + tranTradDiscountMember.TradeDiscHaveVat);
        //                member3 = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disMember));
        //            }
        //            else
        //            {
        //                member1 = "สมาชิก " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Remove(check))) + "%";
        //                member2 = "Member ";
        //                disMember = Convert.ToDouble(tranTradDiscountMember.TradeDiscNoneVat + tranTradDiscountMember.TradeDiscHaveVat);
        //                member3 = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disMember));
        //            }
        //        }

        //        //Discount From Bill
        //        double discount;
        //        double disDiscont = 0.0;
        //        var tranTradDiscount = tranWithDetailsLocal.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
        //        if (tranTradDiscount != null)
        //        {
        //            var check = tranTradDiscount.FmlDiscount.IndexOf('%');
        //            if (check == -1)
        //            {
        //                var CartDiscount = tranTradDiscount.FmlDiscount;
        //                discount = Convert.ToDouble(CartDiscount);
        //                disDiscont = discount;
        //            }
        //            else
        //            {
        //                discount = Convert.ToDouble(tranTradDiscount.FmlDiscount.Remove(check));
        //                discount = discount / 100;
        //                disDiscont = total * discount;
        //            }
        //            discountBill1 = "ส่วนลด";
        //            discountBill2 = "Discount";
        //            discountBill3 = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disDiscont));
        //        }

        //        //Vat
        //        if (tran.tran.TotalVat > 0)
        //        {
        //            vat1 = "ภาษีมูลค่าเพิ่ม " + tran.tran.TaxRate?.ToString("#,##0") + " %";
        //            vat2 = "Vat " + tran.tran.TaxRate?.ToString("#,##0") + " %";
        //            vat3 = tran.tran.TotalVat.ToString("#,##0.00");
        //        }

        //        ParamSlip paramSlip = new ParamSlip()
        //        {
        //            Header = new List<KeyValuePair<string, string>>()
        //            {
        //                new KeyValuePair<string, string>("@VoucherName", "ใบเสร็จรับเงิน"),
        //                new KeyValuePair<string, string>("@merchantName",DataCashingAll.MerchantLocal.Name),
        //                new KeyValuePair<string, string>("@merchantAddress1", merchantAddress1),
        //                new KeyValuePair<string, string>("@merchantAddress2", merchantAddress2),
        //                new KeyValuePair<string, string>("@merchantAddress3", merchantAddress3),
        //                new KeyValuePair<string, string>("@merchantTel", Tel),
        //                new KeyValuePair<string, string>("@merchantTaxid",DataCashingAll.MerchantLocal.TaxID),

        //                new KeyValuePair<string, string>("@branchTaxId", ""),

        //                new KeyValuePair<string, string>("@Billno", tran.tran.TranNo),
        //                new KeyValuePair<string, string>("@Date",  Utils.PrintDateTime(tran.tran.TranDate)),
        //                new KeyValuePair<string, string>("@Time",  Utils.PrintDateTime(tran.tran.TranDate)),

        //                new KeyValuePair<string, string>("@Person", tran.tran.CustomerName),
        //                //new KeyValuePair<string, string>("@Address1",CusAddress1),
        //                //new KeyValuePair<string, string>("@Address2", CusAddress2),

        //                new KeyValuePair<string, string>("@CountDetail","จำนวนรายการ : " + tran.tranDetailItemWithToppings.Count.ToString("#,##0")),
        //                new KeyValuePair<string, string>("@SumQuantity","รวมจำนวนชิ้น : " +  quantity.ToString("#,##0")),

        //                new KeyValuePair<string, string>("@Cashier", "Cashier : " + tran.tran.SellerName),
        //                new KeyValuePair<string, string>("@Total1", "รวมเงิน"),
        //                new KeyValuePair<string, string>("@Total2", "Total"),
        //                new KeyValuePair<string, string>("@Total3", tran.tran.Total.ToString("#,##0.00")),
        //                new KeyValuePair<string, string>("@Vat1",vat1 ),
        //                new KeyValuePair<string, string>("@Vat2",vat2 ),
        //                new KeyValuePair<string, string>("@Vat3",vat3 ),
        //                 new KeyValuePair<string, string>("@Service1",service1 ),
        //                new KeyValuePair<string, string>("@Service2",service2 ),
        //                new KeyValuePair<string, string>("@Service3",service3 ),
        //                new KeyValuePair<string, string>("@Member1",member1 ),
        //                new KeyValuePair<string, string>("@Member2",member2 ),
        //                new KeyValuePair<string, string>("@Member3",member3 ),
        //                new KeyValuePair<string, string>("@Discount1",discountBill1 ),
        //                new KeyValuePair<string, string>("@Discount2",discountBill2 ),
        //                new KeyValuePair<string, string>("@Discount3",discountBill3) ,
        //                new KeyValuePair<string, string>("@GrandTotal1","รวมทั้งสิ้น" ),
        //                new KeyValuePair<string, string>("@GrandTotal2","Grand Total" ),
        //                new KeyValuePair<string, string>("@GrandTotal3", tran.tran.GrandPayment.ToString("#,##0.00")),
        //            },
        //            Details = items
        //        };

        //        //paramSlip.Header = paramSlip.Header.Where(x => !string.IsNullOrEmpty(x.Value)).Distinct().ToList();

        //        //paramSlip.Details = paramSlip.Details.Where(x => !x.Equals(string.Empty) || !x.Equals(null)).Distinct().ToList();

        //        Graphic graphic = new Graphic();
        //        AssetManager assets2 = assets;

        //        if (DataCashingAll.setting == null || string.IsNullOrEmpty(DataCashingAll.setting?.TYPEPAGE))
        //        {
        //            bitmap = graphic.ImageFromXml("_SlipTrans80mm.xml", paramSlip, assets);
        //        }

        //        if (DataCashingAll.setting?.TYPEPAGE == "80mm" || DataCashingAll.setting?.TYPEPAGE == "80 มม.")
        //        {
        //            bitmap = graphic.ImageFromXml("_SlipTrans80mm.xml", paramSlip, assets);
        //        }

        //        if (DataCashingAll.setting?.TYPEPAGE == "58mm" || DataCashingAll.setting?.TYPEPAGE == "58 มม.")
        //        {
        //            bitmap = graphic.ImageFromXml("SlipTrans58mm.xml", paramSlip, assets);
        //        }

        //        return bitmap;
        //    }
        //    catch (Exception ex)
        //    {
        //        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
        //        return null;
        //    }
        //}
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

        public void toGrayscale()
        {
            int width = frameBill.Width;
            int height = frameBill.Height;

            Android.Graphics.Bitmap bmpGrayscale = Android.Graphics.Bitmap.CreateBitmap(width, height, Android.Graphics.Bitmap.Config.Argb8888);

            Android.Graphics.Canvas canvas = new Android.Graphics.Canvas(bmpGrayscale);
            Android.Graphics.Paint paint = new Android.Graphics.Paint();

            ColorMatrix cm = new Android.Graphics.ColorMatrix();
            cm.SetSaturation(0);
            paint.SetColorFilter(new ColorMatrixColorFilter(cm));
            canvas.DrawBitmap(bitmap, width, height, paint);

            frameBill.Draw(canvas);



            //int width = frameBill.Width;
            //int height = frameBill.Height;

            //bitmap = Android.Graphics.Bitmap.CreateBitmap(width, height, Android.Graphics.Bitmap.Config.Argb8888);
            //Android.Graphics.Canvas canvas = new Android.Graphics.Canvas(bitmap);
            //canvas.DrawColor(Android.Graphics.Color.White);
            //if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            //{
            //    canvas.DrawBitmap(bitmap, width, height, null);
            //}
            //else
            //{
            //    canvas.SetViewport(width, height);
            //}
            //frameBill.Draw(canvas);
        }

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

    }
}