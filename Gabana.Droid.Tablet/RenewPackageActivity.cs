using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Plugin.InAppBilling;
using AndroidX.SwipeRefreshLayout.Widget;
using TinyInsightsLib;
using System.Threading;
using Gabana.Droid.Tablet.Adapter.Setting;
using Gabana.Droid.Tablet.Dialog;

namespace Gabana.Droid.Tablet
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class RenewPackageActivity : AppCompatActivity
    {
        public static RenewPackageActivity activity;
        ImageButton btnClose;
        RecyclerView recyclerPackage;
        List<PackageProduce> packages = new List<PackageProduce>();
        public static int PackageIdSelected;
        PackageProduce Produce;
        RelativeLayout lnPromotionCode;
        public static string HistoryPurchase;
        MerchantConfigManage configManage = new MerchantConfigManage();
        InAppBillingPurchase purchases;
        SwipeRefreshLayout refreshlayout;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.package_activity_renew);

                activity = this;

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(this.SupportFragmentManager, nameof(DialogLoading));
                }

                lnPromotionCode = FindViewById<RelativeLayout>(Resource.Id.lnPromotionCode);
                lnPromotionCode.Click += LnPromotionCode_Click;
                btnClose = FindViewById<ImageButton>(Resource.Id.txnClose);
                btnClose.Click += BtnClose_Click;

                recyclerPackage = FindViewById<RecyclerView>(Resource.Id.recyclerPackage);
                Intent serviceIntent = new Intent("com.android.vending.billing.InAppBillingSevice.BIND");
                serviceIntent.SetPackage("com.android.vending");

                PackageIdSelected = 1;
                await GetListPackage();

                if (packages.Count > 0)
                {
                    Produce = packages[0];
                }

                refreshlayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
                refreshlayout.Refresh += async (sender, e) =>
                {
                    if (!await GabanaAPI.CheckNetWork())
                    {
                        Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    }
                    else if (!await GabanaAPI.CheckSpeedConnection())
                    {
                        Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    }
                    else
                    {
                        OnResume();
                        await GetListPackage();
                    }
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };
                _ = TinyInsights.TrackPageViewAsync("OnCreate : RenewPackage");

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                    dialogLoading = new DialogLoading();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreate at RenewPackage");
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            try
            {
                Bundle bundle = new Bundle();
                var fragement = new Logout_Dialog_Main();
                fragement.Arguments = bundle;
                fragement.Show(this.SupportFragmentManager, nameof(Logout_Dialog_Main));
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnClose_Click at RenewPackage");
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
                    packages.Add(package);
                }
                packages = packages.OrderBy(x => x.id).ToList();

                Package_Adapter_Package package_adapter_main = new Package_Adapter_Package(packages);
                GridLayoutManager gridLayout = new GridLayoutManager(this, 2, 1, false);
                recyclerPackage.HasFixedSize = true;
                recyclerPackage.SetLayoutManager(gridLayout);
                recyclerPackage.SetItemViewCacheSize(4);
                recyclerPackage.SetAdapter(package_adapter_main);
                _ = TinyInsights.TrackPageViewAsync("OnCreate : RenewPackageActivity");

            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                Toast.MakeText(this, purchaseEx.Message, ToastLength.Short).Show();               
                return;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListPackage at Package");
            }
        }

        public async void SelectPackage(PackageProduce packages)
        {
            try
            {

                if (Produce != null) PackageIdSelected = packages.id;
                Produce = packages;
                {
                    var connect = await CrossInAppBilling.Current.ConnectAsync();

                    //------- ทำการซื้อแพ็กเกจแล้วส่งขึ้นไปบันทึกที่ Cloud --------
                    purchases = await CrossInAppBilling.Current.PurchaseAsync(Produce.ProductId, ItemType.Subscription, DataCashingAll.MerchantId.ToString());
                    var AcknowledgePurchase = await CrossInAppBilling.Current.AcknowledgePurchaseAsync(purchases.PurchaseToken);
                    if (purchases.State == Plugin.InAppBilling.PurchaseState.Purchased)
                    {
                        #region OldCode
                        //var state = ConvertPurchaseState(purchases.State);
                        //if (state == 0)
                        //{
                        //    var ConsumptionState = ConvertPurchaseConsumptionState(purchases.ConsumptionState);
                        //    RenewModel renewModel = new RenewModel
                        //    {
                        //        Id = purchases.Id,
                        //        TransactionDateUtc = purchases.TransactionDateUtc,
                        //        ProductId = purchases.ProductId,
                        //        Quantity = purchases.Quantity,
                        //        ProductIds = purchases.ProductIds.ToList(),
                        //        AutoRenewing = purchases.AutoRenewing,
                        //        PurchaseToken = purchases.PurchaseToken,
                        //        State = state,
                        //        ConsumptionState = ConsumptionState,
                        //        IsAcknowledged = purchases.IsAcknowledged,
                        //        ObfuscatedAccountId = purchases.ObfuscatedAccountId,
                        //        ObfuscatedProfileId = purchases.ObfuscatedProfileId,
                        //        Payload = purchases.Payload,
                        //        OriginalJson = purchases.OriginalJson,
                        //        Signature = purchases.Signature,
                        //    };

                        //    var PurchasePackage = await GabanaAPI.PutDataRenewPackage(renewModel);
                        //    if (PurchasePackage.Status)
                        //    {
                        //        PutRenewData();
                        //    }
                        //    else
                        //    {
                        //        //กรณีซื้อไม่สำเร็จ
                        //        //ส่งใหม่ dialog refresh เพื่อส่งข้อมูลใหม่
                        //        MainDialog dialog = new MainDialog();
                        //        Bundle bundle = new Bundle();
                        //        String myMessage = Resource.Layout.package_dialog_refresh.ToString();
                        //        bundle.PutString("message", myMessage);
                        //        dialog.Arguments = bundle;
                        //        dialog.Show(SupportFragmentManager, myMessage);
                        //    }
                        //} 
                        #endregion

                        var verify = await CrossInAppBilling.Current.FinishTransaction(purchases);

                        //Dialog
                        //เปลี่ยนแพ็กเกจสำเร็จ
                        Bundle bundle = new Bundle();
                        var fragement = new Package_Dialog_Success();
                        fragement.Cancelable = false;
                        fragement.Arguments = bundle;
                        fragement.Show(this.SupportFragmentManager, nameof(Package_Dialog_Success));

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
            try
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
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("MakePurchase at RenewPackage");
                return false;
            }
        }

        public override void OnBackPressed()
        {
            try
            {
                Bundle bundle = new Bundle();
                var fragement = new Logout_Dialog_Main();
                fragement.Arguments = bundle;
                fragement.Show(this.SupportFragmentManager, nameof(Logout_Dialog_Main));
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnClose_Click at RenewPackage");
            }
        }
    }
}