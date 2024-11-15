using Android;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Com.Karumi.Dexter;
using Com.Karumi.Dexter.Listener;
using Com.Karumi.Dexter.Listener.Single;
using EDMTDev.ZXingXamarinAndroid;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class ItemCodeScanActivity : AppCompatActivity, IPermissionListener
    {
        public static ItemCodeScanActivity scan;
        public ItemCodeScanActivity itemCodeScanActivity;
        public static ZXingScannerView ScannerView;
        public TranDetailItemWithTopping tranDetail;
        static string itemCodeResult;
        public static List<Item> itemList;
        DialogLoading dialogLoading = new DialogLoading();

        protected override void OnCreate(Bundle savedInstanceState)

        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.itemscan_activity_main);
                scan = this;

                CheckJwt();
                //setview
                ScannerView = FindViewById<ZXingScannerView>(Resource.Id.zxscan);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                ImageButton imgBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                lnBack.Click += ImgBack_Click;
                imgBack.Click += ImgBack_Click;

                //Request Permission
                Dexter.WithActivity(this)
                    .WithPermission(Manifest.Permission.Camera)
                    .WithListener(this)
                    .Check();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : ItemCodeScanActivity");

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long);
            }
        }

        async void GetItemList()
        {
            try
            {
                itemList = new List<ORM.MerchantDB.Item>();
                ItemManage itemManage = new ItemManage();
                itemList = await itemManage.GetAllItem();
                if (itemList == null)
                {
                    Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetItemList at ItemScan");
                Console.WriteLine(ex.Message);
            }
        }

        private void ImgBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        protected override void OnDestroy()
        {
            ScannerView.StopCamera();
            base.OnDestroy();
        }

        protected  override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();

                Dexter.WithActivity(this)
                   .WithPermission(Manifest.Permission.Camera)
                   .WithListener(this)
                   .Check();

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                GetItemList();

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                    dialogLoading = new DialogLoading();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at itemScan");
            }
        }

        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
            Toast.MakeText(this, "You Must Enable Permission", ToastLength.Long).Show();

        }

        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
            ScannerView.SetResultHandler(new MyResultHandler(this));
            ScannerView.SetAutoFocus(true);
            ScannerView.SetLaserColor(Color.LightBlue);
            ScannerView.SetBorderColor(Color.Transparent);
            ScannerView.StartCamera();
        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {

        }

        private class MyResultHandler : IResultHandler
        {
            private ItemCodeScanActivity itemScanActivity;

            public MyResultHandler(ItemCodeScanActivity itemScanActivity)
            {
                this.itemScanActivity = itemScanActivity;
            }

            public async void HandleResult(ZXing.Result rawResult)
            {
                try
                {
                    itemCodeResult = rawResult.Text;
                    AddItemActivity.SetItemCode(itemCodeResult);
                    this.itemScanActivity.Finish();
                }
                catch (Exception ex)
                {
                    await TinyInsights.TrackErrorAsync(ex);
                    await TinyInsights.TrackPageViewAsync("MyResultHandler at itemScan");
                }
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
    }
}