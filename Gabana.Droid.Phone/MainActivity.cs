using Android;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Usb;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Text;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Telephony;
using Android.Util;
using Android.Views;
using Android.Widget;
using BellNotificationHub.Xamarin.Android;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using Plugin.InAppBilling;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TinyInsightsLib;
using Xamarin.Essentials;
using static Android.BillingClient.Api.BillingClient;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        public static MainActivity activity;
        TextView txtUsername, txtCompany, txtBranch;
        ImageView imgPos, imgItem, imgCustomer, imgEmployee, imgDashboard, imgReport, imgSetting, imgHistory, imgQR;
        FrameLayout framPos, framItem, framCustomer, framEmployee, framDashboard, framReport, framSetting, framHistory, framQR;
        ImageButton imgMore;
        Android.Support.V4.Widget.DrawerLayout drawerLayout;
        bool flagDrawer = false;
        Utils utils;
        NavigationView navigationView;
        string LoginType, CURRENCYSYMBOLS, usernamelogin;
        AppCompatImageView imgProfile;
        bool deviceAsleep = false;
        bool openPage = false;
        public DateTime pauseDate = DateTime.Now;
        TextView textPackage;
        public static InAppBillingPurchase inAppBillingPurchase = new InAppBillingPurchase();
        string SubscripttionType;
        public const string TAG = "MainActivity";

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.main_activity);

                activity = this;

                InitialUIElement();

                await InitializeNotifications();
                
                Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

                SetupTimer();

                SetupClearRam();

                SetupLayout();

                LoginType = Preferences.Get("LoginType", "");

                await Task.Run(async () =>
                {
                    await InitializeLanguage();
                });


                if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                {
                    CheckPermissionAdriod12();
                }
                else
                {
                    CheckPermission();
                }

                CheckInAppBillingService();

                PrintDeviceToken();
                
                Log.Debug("connectpass", "" + "OnCreate at Main");

                await Task.Run(async () =>
                {
                    await TrackPageView();
                });
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void SetupClearRam()
        {
            timerclearRAM = new System.Threading.Timer(ClearRAM, null, 0, 600000); //600000 ms , 10  นาที
        }

        private void ClearRAM(object state)
        {
            try
            {
                Utils.ClearRam();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error handling memory issues: " + ex.Message);
            }
        }

        private void HandleException(Exception ex)
        {
            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            Log.Debug("connectpass", "Error OnCreate");
            _ = TinyInsights.TrackErrorAsync(ex);
            _ = TinyInsights.TrackPageViewAsync("OnCreate at main");
        }

        private async Task TrackPageView()
        {
            if (await GabanaAPI.CheckNetWork())
            {
                await TinyInsights.TrackPageViewAsync("OnCreate : MainActivity");
            }
        }

        private void PrintDeviceToken()
        {
            var DeviceToken = Preferences.Get("NotificationDeviceToken", "");
            if (!string.IsNullOrEmpty(DeviceToken))
            {
                Console.WriteLine("DeviceToken : " + DeviceToken);
            }
            else
            {
                Console.WriteLine(" No DeviceToken ");
            }
        }

        private void CheckInAppBillingService()
        {
            PackageManager PackageManager = this.PackageManager;
            Intent serviceIntent = new Intent("com.android.vending.billing.InAppBillingSevice.BIND");
            serviceIntent.SetPackage("com.android.vending");
            var list = PackageManager.QueryIntentServices(serviceIntent, 0);
        }

        private async Task InitializeLanguage()
        {
            Android.Content.Res.Configuration conf = Resources.Configuration;
            if (string.IsNullOrEmpty(Preferences.Get("Language", "")))
            {
                SetLocale(conf, Resources.Configuration.Locale.Language.ToString());
            }
            else
            {
                SetLocale(conf, Preferences.Get("Language", ""));
            }

            await InitializeUserAccountInfo();
        }

        private void SetLocale(Android.Content.Res.Configuration conf, string language)
        {
            conf.SetLocale(new Java.Util.Locale(language));
            Resources.UpdateConfiguration(conf, Resources.DisplayMetrics);
            Preferences.Set("Language", language);
            DataCashing.Language = language;
        }

        private async Task InitializeUserAccountInfo()
        {
            UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
            var usePinCode = await accountInfoManage.GetUserAccount(DataCashingAll.MerchantId, usernamelogin);
            if (!string.IsNullOrEmpty(Preferences.Get("UserForgetPincode", "")))
            {
                CheckForgetPincode();
            }
            else
            {
                SetUsePinCode(usePinCode);
            }
        }

        private void SetUsePinCode(ORM.MerchantDB.UserAccountInfo usePinCode)
        {
            if (usePinCode != null)
            {
                if (usePinCode.FUsePincode == 1)
                {
                    DataCashingAll.UsePinCode = true;
                    Preferences.Set("UsePincode", 1);
                    DataCashingAll.intUsePinCode = "1";
                }
                else
                {
                    DataCashingAll.UsePinCode = false;
                    Preferences.Set("UsePincode", 0);
                    DataCashingAll.intUsePinCode = string.Empty;
                }
            }
        }

        private void SetupLayout()
        {
            LinearLayout lnMenu = FindViewById<LinearLayout>(Resource.Id.lnMenu);
            FrameLayout framProfile = FindViewById<FrameLayout>(Resource.Id.framProfile);
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            lnMenu.LayoutParameters.Height = Convert.ToInt32(mainDisplayInfo.Width);
            var sizeLogo = Convert.ToInt32(((mainDisplayInfo.Width - 40) / 3) * 0.5);
            var sizeBorder = Convert.ToInt32((mainDisplayInfo.Width - 80) / 3);
            framProfile.LayoutParameters.Height = Convert.ToInt32((mainDisplayInfo.Height - 140) / 3);
            SetlayoutMenu(sizeLogo);

            if (flagDrawer)
            {
                drawerLayout.CloseDrawers();
            }
        }

        private System.Threading.Timer timer,timerclearRAM;

        private void SetupTimer()
        {
            timer = new System.Threading.Timer(TimerCallback, null, 0, 600000); //6000000 s
        }

        // สร้างเมท็อด TimerCallback สำหรับใช้เป็น callback ใน Timer
        private void TimerCallback(object state)
        {
            // ทำงานที่ต้องการในแต่ละรอบของ Timer
            TimerResentData();
        }

        private async Task InitializeNotifications()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (!BellNotification.IsRegisted())
                {
                    await BellNotificationHelper.RegisterBellNotification(this, GabanaAPI.gbnJWT, DataCashingAll.MerchantId, DataCashingAll.DeviceNo);
                }
            }
        }

        private void InitialUIElement()
        {
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            toolbar.SetNavigationIcon(Resource.Color.primary);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            View headerView = navigationView.GetHeaderView(0);
            textPackage = headerView.FindViewById<TextView>(Resource.Id.textPackage);


            LinearLayout lnPackage = FindViewById<LinearLayout>(Resource.Id.lnPackage);
            lnPackage.Click += LnPackage_Click;
            txtUsername = FindViewById<TextView>(Resource.Id.text_Username);
            txtCompany = FindViewById<TextView>(Resource.Id.text_Company);
            txtBranch = FindViewById<TextView>(Resource.Id.text_Branch);
            imgProfile = FindViewById<AppCompatImageView>(Resource.Id.imgProfile);

            framPos = FindViewById<FrameLayout>(Resource.Id.framPos);
            imgPos = FindViewById<ImageView>(Resource.Id.imgPos);
            framPos.Click += Pos_Click;
            imgPos.Click += Pos_Click;
            framItem = FindViewById<FrameLayout>(Resource.Id.framItem);
            imgItem = FindViewById<ImageView>(Resource.Id.imgItem);
            framItem.Click += Item_Click; ;
            imgItem.Click += Item_Click; ;
            framCustomer = FindViewById<FrameLayout>(Resource.Id.framCustomer);
            imgCustomer = FindViewById<ImageView>(Resource.Id.imgCustomer);
            framCustomer.Click += Customer_Click;
            imgCustomer.Click += Customer_Click;
            framEmployee = FindViewById<FrameLayout>(Resource.Id.framEmployee);
            imgEmployee = FindViewById<ImageView>(Resource.Id.imgEmployee);
            framEmployee.Click += Employee_Click;
            imgEmployee.Click += Employee_Click;
            framDashboard = FindViewById<FrameLayout>(Resource.Id.framDashboard);
            imgDashboard = FindViewById<ImageView>(Resource.Id.imgDashboard);
            framDashboard.Click += Dashboard_Click;
            imgDashboard.Click += Dashboard_Click;
            framReport = FindViewById<FrameLayout>(Resource.Id.framReport);
            imgReport = FindViewById<ImageView>(Resource.Id.imgReport);
            framReport.Click += Report_Click; ;
            imgReport.Click += Report_Click;
            framSetting = FindViewById<FrameLayout>(Resource.Id.framSetting);
            imgSetting = FindViewById<ImageView>(Resource.Id.imgSetting);
            framSetting.Click += Setting_Click; ;
            imgSetting.Click += Setting_Click;
            framHistory = FindViewById<FrameLayout>(Resource.Id.framHistory);
            imgHistory = FindViewById<ImageView>(Resource.Id.imgHistory);
            framHistory.Click += History_Click;
            imgHistory.Click += History_Click;
            framQR = FindViewById<FrameLayout>(Resource.Id.framQR);
            imgQR = FindViewById<ImageView>(Resource.Id.imgQR);
            framQR.Click += myQR_Click;
            imgQR.Click += myQR_Click;
        }

        private void LnPackage_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(PackageActivity)));
            openPage = true;
        }

        public bool CheckPermissionAdriod12()
        {
            try
            {
                Permission permissionCamera = CheckSelfPermission(Manifest.Permission.Camera);
                Permission permissionRead = CheckSelfPermission(Manifest.Permission.ReadExternalStorage);
                Permission permissionWrite = CheckSelfPermission(Manifest.Permission.WriteExternalStorage);
                Permission permissionBluetooth = CheckSelfPermission(Manifest.Permission.Bluetooth);
                Permission permissionBluetoothC = CheckSelfPermission(Manifest.Permission.BluetoothConnect);


                string[] PERMISSIONS =
                    {
                    "android.permission.READ_EXTERNAL_STORAGE",
                    "android.permission.WRITE_EXTERNAL_STORAGE",
                    "android.permission.CAMERA",
                    "android.permission.BLUETOOTH",
                    "android.permission.BLUETOOTH_CONNECT"

                };
                if (
                    permissionCamera != Permission.Granted ||
                    permissionRead != Permission.Granted ||
                    permissionWrite != Permission.Granted ||
                    permissionBluetooth != Permission.Granted ||
                    permissionBluetoothC != Permission.Granted
                    )
                {
                    RequestPermissions(PERMISSIONS, 1);

                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckPermission at Printer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        private async void CheckVersionApp()
        {
            try
            {
                var packageInfo = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0);
                decimal.TryParse(packageInfo.VersionName, out decimal currentVersion);

                var appConfig = await GabanaAPI.GetDataAppConfig("AndroidVersionMinimum");
                decimal.TryParse(appConfig?.CfgString, out decimal minimumVersion);
                if (currentVersion < minimumVersion)
                {
                    ShowUpdateDialog();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "CheckVersionApp at Main");
            }
        }

        private void HandleException(Exception ex, string pageView)
        {
            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            Log.Debug("connectpass", $"Error in {pageView}");
            _ = TinyInsights.TrackErrorAsync(ex);
            _ = TinyInsights.TrackPageViewAsync(pageView);
        }

        private void ShowUpdateDialog()
        {
            MainDialog dialog = new MainDialog() { Cancelable = false };
            Bundle bundle = new Bundle();
            String myMessage = Resource.Layout.updateapp_dialog_main.ToString();
            bundle.PutString("message", myMessage);
            dialog.Arguments = bundle;
            dialog.Show(SupportFragmentManager, myMessage);
        }

        public async Task TimerResentData()
        {
            try
            {
                RunOnUiThread( async () =>
                {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        if (!BellNotification.IsRegisted())
                        {
                            await BellNotificationHelper.RegisterBellNotification(this, GabanaAPI.gbnJWT, DataCashingAll.MerchantId, DataCashingAll.DeviceNo);
                        }

                        
                        ResendData();
                    }
                });
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("TimerResentData at Main");
                Log.Debug("connectpass", "" + "error TimerResentData");
                activity.RunOnUiThread(() => Toast.MakeText(this, ex.Message, ToastLength.Short).Show());
            }
        }

        private void CheckForgetPincode()
        {
            if (Preferences.Get("UserForgetPincode", "") == Preferences.Get("User", ""))
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(ChangePasswordActivity)));
                StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                PinCodeActitvity.SetPincode("ChangePincode");
                Preferences.Set("UserForgetPincode", "");
                this.Finish();
                return;

            }
        }

        public override async void OnUserInteraction()
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


            await CheckJwt();

        }
        private void myQR_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(myQRActivity)));
            openPage = true;
        }

        private void ImgNavigation_Click(object sender, EventArgs e)
        {
            navigationView.PerformClick();
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            try
            {
                if (e.NetworkAccess == NetworkAccess.Internet)
                {
                    var access = e.NetworkAccess;
                    var profiles = e.ConnectionProfiles;
                    //-----------------------------------------------------------
                    //Resend Fwaiting = 2
                    //-----------------------------------------------------------
                    RunOnUiThread(() =>
                    {
                        Task.Delay(5000);
                        ResendData();
                    });
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task ResendData()
        {
            try
            {
                utils = new Utils();

                //Item
                utils.ResentItem();

                //Category
                utils.ResentCategory();

                //Tran
                utils.ResentTran();

                //Customer
                utils.ResentCustomer();

                //NoteCategory
                utils.ResentNoteCategory();

                //Note
                utils.ResentNote();

                //Trant PrinCounter
                utils.ResentPrintCounter();

                //Utils.ResentTranFwaitingOnetwo();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ResendData at Main");
                //activity.RunOnUiThread(() => Toast.MakeText(this, ex.Message, ToastLength.Short).Show());
                Log.Debug("connectpass", ex.Message + " error ResendData");
            }
        }

        private void DrawerLayout_DrawerClosed(object sender, Android.Support.V4.Widget.DrawerLayout.DrawerClosedEventArgs e)
        {
            drawerLayout.CloseDrawers();
        }

        private void ImgMore_Click(object sender, EventArgs e)
        {
            flagDrawer = true;
            drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
        }

        private void SetlayoutMenu(int size)
        {
            imgPos.LayoutParameters.Width = size;
            imgPos.LayoutParameters.Height = size;
            imgItem.LayoutParameters.Width = size;
            imgItem.LayoutParameters.Height = size;
            imgCustomer.LayoutParameters.Width = size;
            imgCustomer.LayoutParameters.Height = size;
            imgEmployee.LayoutParameters.Width = size;
            imgEmployee.LayoutParameters.Height = size;
            imgDashboard.LayoutParameters.Width = size;
            imgDashboard.LayoutParameters.Height = size;
            imgReport.LayoutParameters.Width = size;
            imgReport.LayoutParameters.Height = size;
            imgSetting.LayoutParameters.Width = size;
            imgSetting.LayoutParameters.Height = size;
            imgHistory.LayoutParameters.Width = size;
            imgHistory.LayoutParameters.Height = size;
            imgQR.LayoutParameters.Width = size;
            imgQR.LayoutParameters.Height = size;
        }

        private void History_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(BillHistoryActivity)));
            openPage = true;
        }

        private void Setting_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(SettingActivity)));
            openPage = true;
        }

        private async void Report_Click(object sender, EventArgs e)
        {
            if (await GabanaAPI.CheckNetWork())
            {
                if (await GabanaAPI.CheckSpeedConnection())
                {
                    StartActivity(new Intent(Application.Context, typeof(ReportActivity)));
                    openPage = true;
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                }
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
            }
        }

        private async void Dashboard_Click(object sender, EventArgs e)
        {
            if (await GabanaAPI.CheckNetWork())
            {
                if (await GabanaAPI.CheckSpeedConnection())
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(DashboardActivity)));
                    openPage = true;
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                }
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
            }
        }

        private void Employee_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(EmployeeActivity)));
            openPage = true;
        }

        private void Customer_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(CustomerActivity)));
            openPage = true;

        }

        private void Item_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ItemActivity)));
            openPage = true;
        }
        private void Pos_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(PosActivity)));
            DataCashing.SysCustomerID = 999;
            openPage = true;

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


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public override void OnBackPressed()
        {
            try
            {
                DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                if (drawer.IsDrawerOpen(GravityCompat.Start))
                {
                    drawer.CloseDrawer(GravityCompat.Start);
                }
                else
                {
                    //base.OnBackPressed();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.changepassword)
            {
                LoginType = Preferences.Get("LoginType", "");
                StartActivity(new Intent(Application.Context, typeof(ChangePasswordActivity)));
                openPage = true;
            }
            else if (id == Resource.Id.changebranch)
            {
                StartActivity(new Intent(Application.Context, typeof(ChangeBranchActivity)));
                openPage = true;

            }
            else if (id == Resource.Id.languagesetting)
            {
                StartActivity(new Intent(Application.Context, typeof(LanguageActivity)));
                openPage = true;
            }
            else if (id == Resource.Id.contactus)
            {
                StartActivity(new Intent(Application.Context, typeof(ContactUsActivity)));
                openPage = true;
            }
            else if (id == Resource.Id.termconditions)
            {
                StartActivity(new Intent(Application.Context, typeof(TermActivity)));
                TermActivity.Setpage("Main");
                openPage = true;

            }
            else if (id == Resource.Id.version)
            {

            }
            else if (id == Resource.Id.logout)
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.logout_dialog_main.ToString();
                bundle.PutString("message", myMessage);
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            else if (id == Resource.Id.guildes)
            {
                //https://shorturl.asia/csQfV
                var uri = Android.Net.Uri.Parse("https://shorturl.asia/csQfV");
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        async Task ShowDetail()
        {
            try
            {
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;

                if (CURRENCYSYMBOLS == null) CURRENCYSYMBOLS = "฿";


                if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                {
                    //รอหาวิธีเช็คเพิ่มเติมสำหรับ 13 เป็นต้นไป
                }
                else
                {
                    if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
                    {
                        Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new System.String[] { Manifest.Permission.WriteExternalStorage }, 0);
                    }
                }

                MerchantManage merchantManage = new MerchantManage();
                var GETmerchantlocal = await merchantManage.GetMerchant(DataCashingAll.MerchantId);
                if (GETmerchantlocal == null)
                {
                    Log.Debug("connectpass", "" + "GETmerchantlocal is null");
                    return;
                }

                DataCashingAll.MerchantLocal = GETmerchantlocal;
                var cloudpath = GETmerchantlocal.LogoPath ?? string.Empty;
                var localpath = GETmerchantlocal.LogoLocalPath ?? string.Empty;

                if (await GabanaAPI.CheckNetWork())
                {
                    if (string.IsNullOrEmpty(localpath))
                    {
                        Utils.SetImage(imgProfile, string.IsNullOrEmpty(cloudpath) ? null : cloudpath);
                    }
                    else
                    {
                        Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                        imgProfile.SetImageURI(uri);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(localpath))
                    {
                        Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                        imgProfile.SetImageURI(uri);
                    }
                    else
                    {
                        imgProfile.SetBackgroundResource(Resource.Mipmap.LogoDefault);
                    }
                }

                string branchID = Preferences.Get("Branch", "");
                int.TryParse(branchID, out int result);
                DataCashingAll.SysBranchId = result;
                if (result == 0)
                {
                    DataCashingAll.SysBranchId = 1;
                }
                else
                {
                    DataCashingAll.SysBranchId = result;
                    BranchManage branchManage = new BranchManage();
                    var Getresult = await branchManage.GetBranch(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                    if (Getresult != null)
                    {
                        txtBranch.Text = " " + Getresult.BranchName?.ToString();
                    }
                }

                if (DataCashingAll.MerchantLocal == null)
                {
                    DataCashingAll.MerchantLocal = GETmerchantlocal;
                }
                txtCompany.Text = DataCashingAll.MerchantLocal.MerchantID + ", " + DataCashingAll.MerchantLocal.Name;

                string Username = Preferences.Get("User", "");
                if (Username.ToLower() == "owner")
                {
                    var fullnameOwner = DataCashingAll.UserAccountInfo.Where(x => x.UserName.ToLower() == "owner").FirstOrDefault();
                    if (!string.IsNullOrEmpty(fullnameOwner?.FullName?.ToString()))
                    {
                        txtUsername.Text = fullnameOwner.FullName?.ToString();
                    }
                    else
                    {
                        txtUsername.Text = Username.ToLower();
                    }
                }
                else
                {
                    txtUsername.Text = Username.ToLower();
                }

                await Task.Run(async () =>
                {
                    await GetUserAccount();
                });
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowDetail at main");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                Log.Debug("connectpass", "" + "error ShowDetail");
            }
        }

        UserAccountInfoManage UserAccountInfoManage = new UserAccountInfoManage();
        async Task GetUserAccount()
        {
            try
            {
                if (await GabanaAPI.CheckNetWork())
                {
                    //เพิ่มการดึงข้อมูลของผู้ใช้งานกรณีไม่ได้ผ่านหน้า Splash
                    //Load Data ใหม่ เนื่องจาก ถ้ามีการแก้ไข จะไม่ใช่ข้อมูลล่าสุด จาก Seauth
                    DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
                    var useraccountData = DataCashingAll.UserAccountInfo.Where(x => x.UserName.ToLower() == usernamelogin.ToLower()).FirstOrDefault();

                    //Load Data ใหม่ เนื่องจาก ถ้ามีการแก้ไข จะไม่ใช่ข้อมูลล่าสุด จาก GabanaAPI 
                    var UserAccocunt = await GabanaAPI.GetDataUserAccount(useraccountData.UserName);
                    if (UserAccocunt != null)
                    {
                        ORM.MerchantDB.UserAccountInfo UpdatelocalUser = new ORM.MerchantDB.UserAccountInfo()
                        {
                            MerchantID = UserAccocunt.userAccountInfo.MerchantID,
                            UserName = UserAccocunt.userAccountInfo.UserName.ToLower(),
                            FUsePincode = UserAccocunt?.userAccountInfo.FUsePincode ?? 0,
                            PinCode = UserAccocunt?.userAccountInfo.PinCode ?? null,
                            Comments = UserAccocunt?.userAccountInfo.Comments,
                        };
                        await UserAccountInfoManage.InsertorReplaceUserAccount(UpdatelocalUser);
                    }
                }

                var Data = await UserAccountInfoManage.GetUserAccount(DataCashingAll.MerchantId, usernamelogin.ToLower());
                if (Data != null)
                {
                    if (Data.FUsePincode == 1)
                    {
                        DataCashingAll.UsePinCode = true;
                        Preferences.Set("UsePincode", 1);
                        DataCashingAll.intUsePinCode = "1";
                    }
                    else
                    {
                        DataCashingAll.UsePinCode = false;
                        Preferences.Set("UsePincode", 0);
                        DataCashingAll.intUsePinCode = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetUserAccount");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }


        protected override async void OnResume()
        {
            try
            {
                base.OnResume();
                if (openPage) openPage = false;
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

                await CheckJwt();
                usernamelogin = Preferences.Get("User", "");
                await ShowDetail();

                bool connectionValid = await CheckConnectionAndRetrieveGabanaInfoAsync();
                if (!connectionValid)
                {
                    return;
                }

                if (await HandleExpiry())
                {
                    return;
                }

                // โค้ดที่ไม่ได้เกี่ยวข้องกับ UI ทำงานใน background thread
                Task.Run(async () =>
                {
                    CheckVersionApp();
                    await GetmerchantConfig();

                    MyFirebaseMessagingService._ReloadNoticication();
                });

                RunOnUiThread(() => { 
                    SetDetailPackage();
                });

                if (!string.IsNullOrEmpty(Preferences.Get("UserForgetPincode", "")))
                {
                    CheckForgetPincode();
                }
                else if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                    PinCodeActitvity.SetPincode("Pincode");
                    openPage = true;
                }
                Log.Debug("connectpass", "" + "OnResume" + "Main");
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("error OnResume at main");
            }
        }

        private async Task<bool> CheckExpireDate()
        {
            try
            {
                GabanaInfo gabanaInfo = new GabanaInfo();
                gabanaInfo = await GabanaAPI.GetDataGabanaInfo();
                DataCashingAll.GetGabanaInfo = gabanaInfo;
                var GabanaInfo = JsonConvert.SerializeObject(gabanaInfo);
                Preferences.Set("GabanaInfo", GabanaInfo);

                //ActiveUntilDate = null
                var expire = DataCashingAll.GetGabanaInfo?.ActiveUntilDate;
                if ((expire != null) && (DateTime.Now.Date > expire.Value.Date)) //datenow > expire 19/10 > 07/11
                {
                    return true; //หมดอายุแล้ว
                }
                else
                {
                    return false; //ยังไม่หมดอยุ
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckExpireDate at main");
                return true;
            }
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
                _ = TinyInsights.TrackPageViewAsync("GetmerchantConfig at Main");
                throw;
            }
        }

        private void SetDetailPackage()
        {
            try
            {
                SubscripttionType = DataCashingAll.setmerchantConfig?.SUBSCRIPTION_TYPE;
                int PackageIDCurrent = 1;
                PackageIDCurrent = Utils.SetPackageID(DataCashingAll.GetGabanaInfo.TotalBranch, DataCashingAll.GetGabanaInfo.TotalUser);
                List<string> detail = Utils.SetDetailPackage(PackageIDCurrent.ToString());
                switch (SubscripttionType)
                {
                    case "P":
                    case "A":
                    case "F":
                    case "U":
                    case "B":
                        textPackage.Text = GetString(Resource.String.settingpackage_activity_title) + " : "
                                            + detail[1] + " " + GetString(Resource.String.branch) + " "
                                            + detail[0] + " " + GetString(Resource.String.package_activity_user);
                        break;
                    default:
                        textPackage.Text = "Package : 1Branch/5Users (Free)";
                        break;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDetailPackageOnDB");
            }
        }

        async Task CheckJwt()
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

        async Task CheckRevisionNoTimer()
        {
            try
            {
                //เพิ่มฟังก์ชันสำหรับตรวจสอบ RevisionNo ถ้าน้อยกว่าเรียกข้อมูลใหม่
                MyFirebaseMessagingService myFirebaseMessaging = new MyFirebaseMessagingService();
                List<SystemRevisionNo> lstsystemRevisions = new List<SystemRevisionNo>();
                List<SystemRevisionNo> listRivision = new List<SystemRevisionNo>();
                SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
                SystemRevisionNo revisionNo = new SystemRevisionNo();

                //Get Cloud 
                lstsystemRevisions = await GabanaAPI.GetDataSystemRevisionNo();
                //Get Local 
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();

                if (lstsystemRevisions.Count > 0)
                {
                    foreach (var item in lstsystemRevisions)
                    {
                        revisionNo = new SystemRevisionNo();
                        revisionNo = listRivision.Where(x => x.SystemID == item.SystemID).FirstOrDefault();

                        if (item.LastRevisionNo > revisionNo.LastRevisionNo)
                        {
                            myFirebaseMessaging.ReloadRevision((int)item.SystemID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckRevisionNoTimer at Main");
            }
        }

        private async Task<bool> CheckConnectionAndRetrieveGabanaInfoAsync()
        {
            try
            {
                if (!await GabanaAPI.CheckNetWork())
                {
                    Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    string gabanaInfo = Preferences.Get("GabanaInfo", "");
                    GabanaInfo GabanaInfo = JsonConvert.DeserializeObject<GabanaInfo>(gabanaInfo);
                    DataCashingAll.GetGabanaInfo = GabanaInfo;
                    return false;
                }
                else if (!await GabanaAPI.CheckSpeedConnection())
                {
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    string gabanaInfo = Preferences.Get("GabanaInfo", "");
                    GabanaInfo GabanaInfo = JsonConvert.DeserializeObject<GabanaInfo>(gabanaInfo);
                    DataCashingAll.GetGabanaInfo = GabanaInfo;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckConnectionAndRetrieveGabanaInfoAsync at Main");
                return false;
            }
        }

        private async Task<bool> HandleExpiry()
        {
            try
            {
                bool isExpiry = await CheckExpireDate();
                if (isExpiry)
                {
                    ShowExpiryDialog();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("HandleExpiry at Main");
                return false;
            }
        }

        private void ShowExpiryDialog()
        {
            try
            {
                if (LoginType.ToLower() == "owner")
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.login_dialog_expiry.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                }
                else
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.login_dialog_expiryemp.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowExpiryDialog at Main");
            }
        }
    }
}

