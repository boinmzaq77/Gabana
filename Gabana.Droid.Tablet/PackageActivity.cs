using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidX.RecyclerView.Widget;
using Plugin.InAppBilling;
using AndroidX.SwipeRefreshLayout.Widget;
using TinyInsightsLib;
using System.Threading;
using Gabana.Droid.Tablet.Adapter.Setting;
using Xamarin.Essentials;
using Gabana.ORM.MerchantDB;
using Gabana.Droid.Tablet.Dialog;

namespace Gabana.Droid.Tablet
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class PackageActivity : AppCompatActivity
    {
        public static PackageActivity activity;
        //DialogLoading dialogLoading = new DialogLoading();
        RecyclerView recyclerPackage;
        List<PackageProduce> packages = new List<PackageProduce>();
        public static int PackageIdSelected;

        PackageProduce Produce;
        RelativeLayout lnPromotionCode;
        public static string HistoryPurchase;
        MerchantConfigManage configManage = new MerchantConfigManage();
        InAppBillingPurchase purchases;
        public static string TextError = "";
        BranchManage branchManage = new BranchManage();
        List<ORM.MerchantDB.Branch> lstBranch = new List<ORM.MerchantDB.Branch>();
        string SubscripttionType;
        SwipeRefreshLayout refreshlayout;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.package_activity);
                activity = this;

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += ImagebtnBack_Click;
                lnPromotionCode = FindViewById<RelativeLayout>(Resource.Id.lnPromotionCode);
                lnPromotionCode.Click += LnPromotionCode_Click;

                //btnContact
                TextView textContact = FindViewById<TextView>(Resource.Id.textContact);
                textContact.Click += (sender, e) =>
                {
                    try
                    {
                        var uri = Android.Net.Uri.Parse("tel:026925899");
                        var intent = new Intent(Intent.ActionView, uri);
                        StartActivity(intent);
                    }
                    catch (Exception)
                    {
                    }
                };                
                
                recyclerPackage = FindViewById<RecyclerView>(Resource.Id.recyclerPackage);                              

                Intent serviceIntent = new Intent("com.android.vending.billing.InAppBillingSevice.BIND");
                serviceIntent.SetPackage("com.android.vending");
                                

                refreshlayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
                refreshlayout.Refresh += async (sender, e) =>
                {
                    OnResume();
                    await GetListPackage();
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };

                _ = TinyInsights.TrackPageViewAsync("OnCreate : PackageActivity");

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                    dialogLoading = new DialogLoading();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("PackageActivity");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        public async void SelectPackage(PackageProduce packages)
        {
            try
            {

                if (SubscripttionType == "A")
                {
                    //case sub_type คนละตัวกับระบบปฏิบัติการ แจ้งเตือน dialog ให้ unsub ก่อน return; 
                    TextError = "";
                    Bundle bundle = new Bundle();
                    var fragement = new Package_Dialog_Error();
                    fragement.Cancelable = false;
                    fragement.Arguments = bundle;
                    fragement.Show(this.SupportFragmentManager, nameof(Package_Dialog_Error));
                    return;
                }
                else if (SubscripttionType == "B")
                {
                    //เปิด webview สำหรับต่อที่หลังบ้าน
                    return;
                }
                else //เคส P,U,F
                {
                    if (Produce != null) PackageIdSelected = packages.id;
                    Produce = packages;
                    {
                        var connect = await CrossInAppBilling.Current.ConnectAsync();
                        var purchaseHistory = await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.Subscription);

                        //สมัครแพ็กเกจครั้งแรก ไม่มีประวัติการสมัครโดยใช้อีเมลที่เครื่องไหน หรือร้านค้าไหน
                        if (purchaseHistory == null || purchaseHistory.FirstOrDefault() == null)
                        {
                            //เคส P
                            if (SubscripttionType == "P")
                            {
                                TextError = GetString(Resource.String.package_activity_changemail);
                                Bundle bundle = new Bundle();
                                var fragement = new Package_Dialog_Error();
                                fragement.Cancelable = false;
                                fragement.Arguments = bundle;
                                fragement.Show(this.SupportFragmentManager, nameof(Package_Dialog_Error));
                                return;
                            }
                            else //เคส F,U
                            {
                                purchases = await CrossInAppBilling.Current.PurchaseAsync(Produce.ProductId, ItemType.Subscription, DataCashingAll.MerchantId.ToString());
                                var AcknowledgePurchase = await CrossInAppBilling.Current.AcknowledgePurchaseAsync(purchases.PurchaseToken);
                                if (purchases.State == Plugin.InAppBilling.PurchaseState.Purchased)
                                {
                                    var verify = await CrossInAppBilling.Current.FinishTransaction(purchases);

                                    //Dialog
                                    //เปลี่ยนแพ็กเกจสำเร็จ
                                    Bundle bundle = new Bundle();
                                    var fragement = new Package_Dialog_Success();
                                    fragement.Cancelable = false;
                                    fragement.Arguments = bundle;
                                    fragement.Show(this.SupportFragmentManager, nameof(Package_Dialog_Success));
                                }
                                return;
                            }
                        }
                        else //มีประวัติการสมัครสมาชิก
                        {
                            // merchantID ตรงกับร้านค้าปัจจุบัน
                            if (purchaseHistory.FirstOrDefault().ObfuscatedAccountId == DataCashingAll.MerchantId.ToString())
                            {
                                //มีการ Subscription อยู่ Auto Renew == true
                                if (purchaseHistory.FirstOrDefault().AutoRenewing == true)
                                {
                                    bool hasCurrentPackage = await CheckCurrentPackage(purchaseHistory.OrderByDescending(x => x.TransactionDateUtc).FirstOrDefault(), Produce);
                                    if (hasCurrentPackage)
                                    {
                                        return;
                                    }
                                }
                                else  //ไม่มีการ SubscriptionAuto Renew == false
                                {
                                    purchases = await CrossInAppBilling.Current.PurchaseAsync(Produce.ProductId, ItemType.Subscription, DataCashingAll.MerchantId.ToString());
                                    var AcknowledgePurchase = await CrossInAppBilling.Current.AcknowledgePurchaseAsync(purchases.PurchaseToken);
                                    if (purchases.State == Plugin.InAppBilling.PurchaseState.Purchased)
                                    {
                                        var verify = await CrossInAppBilling.Current.FinishTransaction(purchases);                                        

                                        //Dialog
                                        //เปลี่ยนแพ็กเกจสำเร็จ
                                        Bundle bundle = new Bundle();
                                        var fragement = new Package_Dialog_Success();
                                        fragement.Cancelable = false;
                                        fragement.Arguments = bundle;
                                        fragement.Show(this.SupportFragmentManager, nameof(Package_Dialog_Success));
                                    }
                                    return;
                                }
                            }
                            else // merchantID ไม่ตรงกับร้านค้าปัจจุบัน
                            {
                                //มีการ Subscription อยู่ Auto Renew == true
                                if (purchaseHistory.FirstOrDefault().AutoRenewing == true)
                                {
                                    TextError = GetString(Resource.String.package_activity_sub1) + " " + purchaseHistory.FirstOrDefault().ObfuscatedAccountId + " " +
                                   GetString(Resource.String.package_activity_sub2) + " " + DataCashingAll.MerchantId
                                   + " " + GetString(Resource.String.package_activity_sub3);
                                    Bundle bundle = new Bundle();
                                    var fragement = new Package_Dialog_Error();
                                    fragement.Cancelable = false;
                                    fragement.Arguments = bundle;
                                    fragement.Show(this.SupportFragmentManager, nameof(Package_Dialog_Error));
                                    return;
                                }
                                else //ไม่มีการ SubscriptionAuto Renew == false
                                {
                                    //เคส P
                                    if (SubscripttionType == "P")
                                    {
                                        TextError = GetString(Resource.String.package_activity_changemail);
                                        Bundle bundle = new Bundle();
                                        var fragement = new Package_Dialog_Error();
                                        fragement.Cancelable = false;
                                        fragement.Arguments = bundle;
                                        fragement.Show(this.SupportFragmentManager, nameof(Package_Dialog_Error));
                                        return;
                                    }
                                    else //เคส F,U
                                    {
                                        purchases = await CrossInAppBilling.Current.PurchaseAsync(Produce.ProductId, ItemType.Subscription, DataCashingAll.MerchantId.ToString());
                                        var AcknowledgePurchase = await CrossInAppBilling.Current.AcknowledgePurchaseAsync(purchases.PurchaseToken);
                                        if (purchases.State == Plugin.InAppBilling.PurchaseState.Purchased)
                                        {
                                            var verify = await CrossInAppBilling.Current.FinishTransaction(purchases);
                                            
                                            //Dialog
                                            //เปลี่ยนแพ็กเกจสำเร็จ
                                            MainDialog dialog = new MainDialog();
                                            Bundle bundle = new Bundle();
                                            String myMessage = Resource.Layout.package_dialog_success.ToString();
                                            bundle.PutString("message", myMessage);
                                            bundle.PutString("SubscriptSuccess", "SubscriptSuccess");
                                            dialog.Arguments = bundle;
                                            dialog.Show(SupportFragmentManager, myMessage);
                                        }
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                Toast.MakeText(this, purchaseEx.Message, ToastLength.Short).Show();
                return;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void LnPromotionCode_Click(object sender, EventArgs e)
        {
            Bundle bundle = new Bundle();
            var fragement = new Package_Dialog_PromotionRef();
            fragement.Cancelable = false;
            fragement.Arguments = bundle;
            fragement.Show(this.SupportFragmentManager, nameof(Package_Dialog_PromotionRef));
        }

        public async Task<bool> MakePurchase()
        {
            if (!CrossInAppBilling.IsSupported)
            {
                return false;
            }
            using (var billing = CrossInAppBilling.Current)
            {
                try
                {
                    var connected = await billing.ConnectAsync();
                    if (!connected)
                        return false;
                    else
                    {
                        return true;
                    }
                }
                finally
                {
                    await billing.DisconnectAsync();
                }
            }
        }

        private async Task GetListPackage()
        {            
            try
            {
               
                if (!await MakePurchase())
                {

                    return;
                }
                var ProductId = new string[]
                {
                "1",
                "2",
                "3",
                "4"
                };
                var connect = await CrossInAppBilling.Current.ConnectAsync();
                if (!connect)
                {
                    return;
                }
                var GetProductInfo = await CrossInAppBilling.Current.GetProductInfoAsync(ItemType.Subscription, ProductId);
                //var GetProductInfo = await CrossInAppBilling.Current.GetProductInfoAsync(ItemType.Subscription, null);
                packages = new List<PackageProduce>();
                foreach (var item in GetProductInfo)
                {
                    var detail = Utils.SetDetailPackage(item.ProductId);
                    PackageProduce package = new PackageProduce();
                    package.id = Convert.ToInt32(item.Name.Replace("GabanaPackage", "").Substring(0, 1));
                    package.ProductId = item.ProductId;
                    package.PackageName = item.Name.Replace("Gabana", "").Replace("()", "");
                    package.MaxBranch = detail[1] + " " + GetString(Resource.String.branch);
                    package.MaxUser = detail[0] + " " + GetString(Resource.String.package_activity_user);
                    package.Price = item.OriginalPrice;
                    if (HistoryPurchase == item.ProductId)
                    {
                        PackageIdSelected = package.id;
                    }
                    packages.Add(package);
                }
                packages = packages.OrderBy(x => x.id).ToList();
                Package_Adapter_Package setting_adapter_package = new Package_Adapter_Package(packages);
                GridLayoutManager gridLayout = new GridLayoutManager(this, 4, 1, false);
                recyclerPackage.HasFixedSize = true;
                recyclerPackage.SetLayoutManager(gridLayout);
                recyclerPackage.SetItemViewCacheSize(4);
                recyclerPackage.SetAdapter(setting_adapter_package);

                
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                Toast.MakeText(this, purchaseEx.Message, ToastLength.Short).Show();
                //dialogLoading.Dismiss();
                //dialogLoading = new DialogLoading();
                return;
            }
            catch (Exception ex)
            {
                //dialogLoading.Dismiss();
                //dialogLoading = new DialogLoading();
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListPackage at Package");
            }
        }

        public void CallPutData()
        {
            PutData();
        }
        private async void PutData()
        {
            try
            {
                List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
                var merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "SUBSCRIPTION_TYPE",
                    CfgString = "P"
                };
                lstmerchantConfig.Add(merchantConfig);

                var update = await GabanaAPI.PutDataMerchantConfig(lstmerchantConfig, DataCashingAll.DeviceNo);
                if (update.Status)
                {
                    //Insert to Local DB
                    MerchantConfig localConfig = new MerchantConfig()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        CfgKey = "SUBSCRIPTION_TYPE",
                        CfgString = "P"
                    };

                    var localVAT = await configManage.InsertorReplacrMerchantConfig(localConfig);
                    if (localVAT)
                    {
                        DataCashingAll.setmerchantConfig.SUBSCRIPTION_TYPE = "P";
                    }
                }
                else
                {
                    Toast.MakeText(this, update.Message, ToastLength.Short).Show();
                }

                var verify = await CrossInAppBilling.Current.FinishTransaction(purchases);
                //Dialog
                //เปลี่ยนแพ็กเกจสำเร็จ

                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.package_dialog_offline.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("SubscriptSuccess", "SubscriptSuccess");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async Task<bool> CheckCurrentPackage(InAppBillingPurchase purchaseCurrent, PackageProduce packageProduce)
        {
            try
            {
                var _purchaseCurrent = purchaseCurrent;
                if (_purchaseCurrent != null)
                {
                    InAppBillingPurchase UpgradePurchasedSub = new InAppBillingPurchase();

                    //เช็คจากสาขา ถ้าสาขาปัจจุบันมีสาขามากกว่า                     
                    await GetGabanaInfo();

                    //get package ID
                    //id ปัจจุบัน , id ตัวที่เลือก
                    //เงื่อนไข มากว่า น้อยกว่า

                    int PackageIDCurrent = Utils.SetPackageID(DataCashingAll.GetGabanaInfo.TotalBranch, DataCashingAll.GetGabanaInfo.TotalUser);
                    int PackageSelect = Convert.ToInt32(packageProduce.ProductId);

                    if (PackageIDCurrent == -1)
                    {
                        //กรณี add on ที่มีการเพิ่มจากหลังบ้าน เนื่องจาก ไม่มี packageid = -1
                        return true;
                    }

                    if (PackageSelect < PackageIDCurrent)
                    {
                        //การ downgrade จะตรวจสอบก่อนว่า user และ branch มีมากกว่า package ที่ต้องการ downgrade หรือไม่ 
                        //หากมีจะไม่สามารถทำการ downgrade ได้ และมีข้อความแจ้งให้ไปทำการลบข้อมูลก่อน

                        //เช็คจำนวนสาขาปัจจุบัน ว่าน้อยกว่าหรือเท่ากับสาขาที่เลือกหรือไม่
                        List<string> detail = Utils.SetDetailPackage(packageProduce.ProductId);

                        if ((lstBranch.Count > Convert.ToInt32(detail[1])) && (DataCashingAll.UserAccountInfo.Count > Convert.ToInt32(detail[0])))
                        {
                            TextError = Resources.GetString(Resource.String.package_activity_downgrade);
                            Bundle bundle = new Bundle();
                            var fragement = new Package_Dialog_Error();
                            fragement.Cancelable = false;
                            fragement.Arguments = bundle;
                            fragement.Show(this.SupportFragmentManager, nameof(Package_Dialog_Error));
                            return true;
                        }

                        UpgradePurchasedSub = await CrossInAppBilling.Current.UpgradePurchasedSubscriptionAsync(packageProduce.ProductId, _purchaseCurrent.PurchaseToken, SubscriptionProrationMode.ImmediateWithoutProration);//Downgrade
                    }
                    else if (PackageIDCurrent == PackageSelect)
                    {
                        return true;
                    }
                    else
                    {
                        UpgradePurchasedSub = await CrossInAppBilling.Current.UpgradePurchasedSubscriptionAsync(packageProduce.ProductId, _purchaseCurrent.PurchaseToken, SubscriptionProrationMode.ImmediateAndChargeProratedPrice); //Upgrade
                    }

                    var AcknowledgePurchase = await CrossInAppBilling.Current.AcknowledgePurchaseAsync(UpgradePurchasedSub.PurchaseToken);
                    if (UpgradePurchasedSub.State == Plugin.InAppBilling.PurchaseState.Purchased)
                    {
                        var verify = await CrossInAppBilling.Current.FinishTransaction(purchases);

                        DialogLoading dialogLoading = new DialogLoading();
                        if (dialogLoading.Cancelable != false)
                        {
                            dialogLoading.Cancelable = false;
                            dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                        }

                        //Dialog
                        //เปลี่ยนแพ็กเกจสำเร็จ
                        Bundle bundle = new Bundle();
                        var fragement = new Package_Dialog_Success();
                        fragement.Cancelable = false;
                        fragement.Arguments = bundle;
                        fragement.Show(this.SupportFragmentManager, nameof(Package_Dialog_Success));

                        if (dialogLoading != null)
                        {
                            dialogLoading.DismissAllowingStateLoss();
                            dialogLoading.Dismiss();
                            dialogLoading = new DialogLoading();
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return true;
            }
        }

        private int ConvertPurchaseConsumptionState(ConsumptionState consumptionState)
        {
            try
            {
                switch (consumptionState)
                {
                    case ConsumptionState.NoYetConsumed:
                        return 0;
                    case ConsumptionState.Consumed:
                        return 1;
                    default:
                        return 0;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return 0;
            }
        }

        private int ConvertPurchaseState(Plugin.InAppBilling.PurchaseState state)
        {
            try
            {
                switch (state)
                {
                    case Plugin.InAppBilling.PurchaseState.Purchased:
                        return 0;
                    case Plugin.InAppBilling.PurchaseState.Canceled:
                        return 1;
                    case Plugin.InAppBilling.PurchaseState.Refunded:
                        return 2;
                    case Plugin.InAppBilling.PurchaseState.Purchasing:
                        return 3;
                    case Plugin.InAppBilling.PurchaseState.Failed:
                        return 4;
                    case Plugin.InAppBilling.PurchaseState.Restored:
                        return 5;
                    case Plugin.InAppBilling.PurchaseState.Deferred:
                        return 6;
                    case Plugin.InAppBilling.PurchaseState.FreeTrial:
                        return 7;
                    case Plugin.InAppBilling.PurchaseState.PaymentPending:
                        return 8;
                    case Plugin.InAppBilling.PurchaseState.Pending:
                        return 9;
                    case Plugin.InAppBilling.PurchaseState.Unknown:
                        return 10;
                    default:
                        return 0;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return 0;
            }
        }

        private void ImagebtnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        async Task GetmerchantConfig()
        {
            try
            {
                List<MerchantConfig> lstconfig = new List<MerchantConfig>();
                SetMerchantConfig setconfig = new SetMerchantConfig();
                MerchantConfigManage merchantconfigManage = new MerchantConfigManage();

                List<ORM.Master.MerchantConfig> listmerchantConfig = new List<ORM.Master.MerchantConfig>();
                listmerchantConfig = await GabanaAPI.GetDataMerchantConfig();
                if (listmerchantConfig == null)
                {
                    return;
                }
                if (listmerchantConfig.Count > 0)
                {
                    foreach (var item in listmerchantConfig)
                    {
                        MerchantConfig config = new MerchantConfig()
                        {
                            MerchantID = item.MerchantID,
                            CfgKey = item.CfgKey,
                            CfgInteger = item.CfgInteger,
                            CfgFloat = item.CfgFloat,
                            CfgString = item.CfgString,
                            CfgDate = item.CfgDate
                        };
                        var InsertorReplace = await merchantconfigManage.InsertorReplacrMerchantConfig(config);
                        if (InsertorReplace)
                        {
                            lstconfig.Add(config);
                        }
                    }

                    #region merchantConfig
                    var TAXTYPE = lstconfig.Where(x => x.CfgKey == "TAXTYPE").FirstOrDefault();
                    if (TAXTYPE != null)
                    {
                        setconfig.TAXTYPE = TAXTYPE.CfgString;
                    }

                    var TAXRATE = lstconfig.Where(x => x.CfgKey == "TAXRATE").FirstOrDefault();
                    if (TAXRATE != null)
                    {
                        if (TAXRATE.CfgFloat != null)
                        {
                            setconfig.TAXRATE = TAXRATE.CfgFloat.ToString();
                        }
                    }

                    var CURRENCY_SYMBOLS = lstconfig.Where(x => x.CfgKey == "CURRENCY_SYMBOLS").FirstOrDefault();
                    if (CURRENCY_SYMBOLS != null)
                    {
                        setconfig.CURRENCY_SYMBOLS = CURRENCY_SYMBOLS.CfgString;
                    }

                    var DECIMAL_POINT_CALC = lstconfig.Where(x => x.CfgKey == "DECIMAL_POINT_CALC").FirstOrDefault();
                    if (DECIMAL_POINT_CALC != null)
                    {
                        if (DECIMAL_POINT_CALC.CfgInteger != null)
                        {
                            setconfig.DECIMAL_POINT_CALC = DECIMAL_POINT_CALC.CfgInteger.ToString();
                        }
                    }

                    var DECIMAL_POINT_DISPLAY = lstconfig.Where(x => x.CfgKey == "DECIMAL_POINT_DISPLAY").FirstOrDefault();
                    if (DECIMAL_POINT_DISPLAY != null)
                    {
                        if (DECIMAL_POINT_DISPLAY.CfgInteger != null)
                        {
                            setconfig.DECIMAL_POINT_DISPLAY = DECIMAL_POINT_DISPLAY.CfgInteger.ToString();
                        }
                    }

                    var OPTION_ROUNDING = lstconfig.Where(x => x.CfgKey == "OPTION_ROUNDING").FirstOrDefault();
                    if (OPTION_ROUNDING != null)
                    {
                        setconfig.OPTION_ROUNDING_STRING = OPTION_ROUNDING.CfgString;
                        if (OPTION_ROUNDING.CfgInteger != null)
                        {
                            setconfig.OPTION_ROUNDING_INT = OPTION_ROUNDING.CfgInteger.ToString();
                        }
                    }

                    var SERVICECHARGE_TYPE = lstconfig.Where(x => x.CfgKey == "SERVICECHARGE_TYPE").FirstOrDefault();
                    if (SERVICECHARGE_TYPE != null)
                    {
                        setconfig.SERVICECHARGE_TYPE = SERVICECHARGE_TYPE.CfgString;
                    }

                    var SERVICECHARGE_RATE = lstconfig.Where(x => x.CfgKey == "SERVICECHARGE_RATE").FirstOrDefault();
                    if (SERVICECHARGE_RATE != null)
                    {
                        setconfig.SERVICECHARGE_RATE = SERVICECHARGE_RATE.CfgString;
                    }

                    var PRINTER_DEFAULT = lstconfig.Where(x => x.CfgKey == "PRINTER_DEFAULT").FirstOrDefault();
                    if (PRINTER_DEFAULT != null)
                    {
                        setconfig.PRINTER_DEFAULT = PRINTER_DEFAULT.CfgString;
                    }

                    var SUBSCRIPTION_TYPE = lstconfig.Where(x => x.CfgKey == "SUBSCRIPTION_TYPE").FirstOrDefault();
                    if (SUBSCRIPTION_TYPE != null)
                    {
                        setconfig.SUBSCRIPTION_TYPE = SUBSCRIPTION_TYPE.CfgString;
                    }

                    var CASHDRAWER = lstconfig.Where(x => x.CfgKey == "CASHDRAWER").FirstOrDefault();
                    if (CASHDRAWER != null)
                    {
                        setconfig.CASHDRAWER = CASHDRAWER.CfgInteger.ToString();
                    }
                    else
                    {
                        setconfig.CASHDRAWER = "0";
                    }
                    #endregion

                    var merchantConfig = JsonConvert.SerializeObject(setconfig);
                    Preferences.Set("SetmerchantConfig", merchantConfig);
                    var setmerchantConfig = Preferences.Get("SetmerchantConfig", "");
                    if (setmerchantConfig != "")
                    {
                        var Config = JsonConvert.DeserializeObject<SetMerchantConfig>(setmerchantConfig);
                        DataCashingAll.setmerchantConfig = Config;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetmerchantConfig");
                Log.Error("connecterror", "GetmerchantConfig : " + ex.Message);
                throw;
            }
        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                if (await GabanaAPI.CheckNetWork())
                {
                    await GetGabanaInfo();
                    await GetmerchantConfig();
                }

                //เปลี่ยนจากการเรียกข้อมูลจาก goolge เป็นเรียกจาก gabanainfo
                int PackageIDCurrent = 1;
                PackageIDCurrent = Utils.SetPackageID(DataCashingAll.GetGabanaInfo.TotalBranch, DataCashingAll.GetGabanaInfo.TotalUser);
                if (PackageIDCurrent == -1)
                {
                    PackageIdSelected = 1;
                }
                else
                {
                    HistoryPurchase = PackageIDCurrent.ToString();
                }

                await GetListPackage();
                if (packages.Count > 0)
                {
                    Produce = packages[0];
                }
                SubscripttionType = DataCashingAll.setmerchantConfig?.SUBSCRIPTION_TYPE;
                lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("error OnResume at Package");
            }
        }

        private async Task GetGabanaInfo()
        {
            try
            {
                GabanaInfo gabanaInfo = new GabanaInfo();
                gabanaInfo = await GabanaAPI.GetDataGabanaInfo();
                DataCashingAll.GetGabanaInfo = gabanaInfo;
                var GabanaInfo = JsonConvert.SerializeObject(gabanaInfo);
                Preferences.Set("GabanaInfo", GabanaInfo);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetCloudProductLicence at Package");
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