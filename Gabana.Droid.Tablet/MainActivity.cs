using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using AndroidX.AppCompat.App;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.More;
using Gabana.Droid.Tablet.Fragments.POS;
using System;
using Xamarin.Essentials;
using Android.Content.PM;
using Android;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using Gabana.Model;
using TinyInsightsLib;
using Gabana.ORM.MerchantDB;
using System.Collections.Generic;
using Gabana.Droid.Tablet.Fragments.Setting;
using Android.Content;
using Android.Provider;
using System.Linq;
using Android.Views.InputMethods;
using Newtonsoft.Json;
using Android.Util;
using System.Threading.Tasks;
using Gabana.Droid.Tablet.Fragments.Customers;
using Gabana.Droid.Tablet.Fragments.Dashboard;
using Gabana.Droid.Tablet.Fragments.Employee;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Items;
using Android.Content.Res;
using Android.Speech;
using Gabana.Droid.Tablet.Fragments.Bill;
using LinqToDB.Data;
using Gabana.Droid.Tablet.Fragments.Report;
using BellNotificationHub.Xamarin.Android;
using LinqToDB.Common;
using System.Diagnostics;
using static Android.Views.SurfaceControl;

namespace Gabana.Droid.Tablet
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, WindowSoftInputMode = SoftInput.AdjustPan)]
    public class MainActivity : AppCompatActivity
    {
        public static MainActivity main_activity;
        public AndroidX.Fragment.App.Fragment activeL, activeR;
        public static TranWithDetailsLocal tranWithDetails;
        string usernamelogin;
        public static string nameCategory = "", tabSelected;
        public static bool flagView = false; // falgView = false = row,flagView = true = grid
        public More_Fragment_Main more_fragment_main;
        POS_Fragment_Main pos_fragment_main;
        POS_Fragment_Cart pos_fragment_cart;

        Item_Fragment_Main item_fragment_main;
        Item_Fragment_AddItem item_fragment_additem;
        Item_Fragment_AddCategory item_fragment_addcategory;
        Item_Fragment_AddTopping item_fragment_addtopping;
        public POS_Dialog_AddItem pos_dialog_additem;
        public POS_Dialog_AddTopping pos_dialog_addtopping;

        Setting_Fragment_Main setting_fragment_main;
        Setting_Fragment_Merchant setting_fragment_merchant;
        Setting_Fragment_Branch setting_fragment_branch;
        Setting_Fragment_BranchDetail setting_fragment_branchdetail;
        Setting_Fragment_Empmanage setting_fragment_empmanage;
        Setting_Fragment_Package setting_fragment_package;
        Setting_Fragment_Note setting_fragment_note;
        Setting_Fragment_AddNote setting_fragment_addnote;
        Setting_Fragment_MemberType setting_fragment_membertype;
        Setting_Fragment_AddMemberType setting_fragment_addmembertype;
        Setting_Fragment_Vat setting_fragment_vat;
        Setting_Fragment_Currency setting_fragment_currency;
        Setting_Fragment_Decimal setting_fragment_decimal;
        Setting_Fragment_DecimalHelp setting_fragment_decimalhelp;
        Setting_Fragment_ServiceCharge setting_fragment_servicecharge;
        Setting_Fragment_CashGuild setting_fragment_cashguild;
        Setting_Fragment_AddCash setting_fragment_addcash;
        Setting_Fragment_GiftVoucher setting_fragment_giftvoucher;
        Setting_Fragment_AddGiftVoucher setting_fragment_addgiftvoucher;
        Setting_Fragment_GiftVoucherNumpad setting_fragment_giftvouchernumpad;
        Setting_Fragment_Device setting_fragment_device;
        Setting_Fragment_Printer setting_fragment_printer;
        Setting_Fragment_MyQR setting_fragment_myqr;
        Setting_Fragment_AddMyQR setting_fragment_addmyqr;
        Setting_Fragment_CashDrawer setting_fragment_cashdrawer;

        Customer_Fragment_Main customer_fragment_main;
        Customer_Fragment_AddCustomer customer_fragment_addcustomer;

        Employee_Fragment_Main employee_fragment_main;
        Employee_Fragment_CheckUser employee_fragment_checkuser;
        Employee_Fragment_AddEmployee employee_fragment_addemployee;


        Dashboard_Fragment_Main dashboard_fragment_main;

        Report_Fragment_Main report_fragment_main;
        Report_Fragment_ShowData report_fragment_showdata;

        BillHistory_Fragment_Main billhistory_fragment_main;
        BillHistory_Fragment_Detail billhistory_fragment_detail;

        FrameLayout frameLeft, frameRight;
        int framL, framR;
        LinearLayout lnMore, lnPos, lnItem, lnCustomer, lnEmployee, lnDashboard, lnReport, lnSetting, lnBillHistory;
        ImageView imgMore, imgPOS, imgItem, imgCustomer, imgEmployee, imgDashboard, imgReport, imgSetting, imgBillHistory;
        CategoryManage categorymanage = new CategoryManage();
        ItemManage itemManage = new ItemManage();
        string LoginType, CURRENCYSYMBOLS;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            Utils.ShowHi("Welcome");
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                base.OnCreate(savedInstanceState);

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.activity_main);

                main_activity = this;

                CombineUI();

                await InitializeNotifications();

                #region initial page
                framL = Resource.Id.content_frameL;
                framR = Resource.Id.content_frameR;
                more_fragment_main = More_Fragment_Main.NewInstance();
                pos_fragment_main = POS_Fragment_Main.NewInstance();
                pos_fragment_cart = POS_Fragment_Cart.NewInstance();

                var transaction = SupportFragmentManager.BeginTransaction();
                transaction.Add(framL, more_fragment_main, "more");
                transaction.Show(more_fragment_main).Commit();

                activeL = more_fragment_main;
                activeR = pos_fragment_cart;

                LoadFragmentMain(framL, "more", "default");
                #endregion

                LoginType = Preferences.Get("LoginType", "").ToLower();
                usernamelogin = Preferences.Get("User", "");

                DataCashing.CheckNet = await GabanaAPI.CheckNetWork();

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

                Task task1 = Task.Run(() => GetDataNull());
                Task task2 = Task.Run(async () => await GetAllData());

                // รอให้ทุก task ทำงานเสร็จสิ้น
                await Task.WhenAll(task1, task2);

                //this.RunOnUiThread(() =>
                //{
                //    SetFragment();
                //});

                Log.Debug("connectpass", "" + "OnCreate" + "Main");

                DataCashingAll.flagItemChange = true;
                DataCashingAll.flagCategoryChange = true;

                // ตรวจสอบว่า DialogLoading ยังไม่ปิดอยู่ก่อนที่จะแสดง Fragment
                if (dialogLoading.IsVisible)
                {
                    //SetFragment();
                    //this.RunOnUiThread(() =>
                    //{
                    //    SetFragment();
                    //});

                    SetFragment();
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }

                _ = TinyInsights.TrackPageViewAsync("OnCreate : MainActivity");
            }
            catch (Exception ex)
            {
                dialogLoading?.Dismiss();
                //_ = TinyInsights.TrackErrorAsync(ex);
                //_ = TinyInsights.TrackPageViewAsync("OnCreate at MainActivity");
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                });
            }
        }

        private async void GetDataNull()
        {
            try
            {
                //Case Missing กรณีผ่าน Splash?
                //Get BranchID
                string txtbranchID = Preferences.Get("Branch", "");
                int.TryParse(txtbranchID, out int branchID);
                DataCashingAll.SysBranchId = branchID;

                //Get DeviceNo
                DataCashingAll.DeviceNo = DataCashingAll.Merchant.Device.DeviceNo;
                await CheckOrderBeforeClose();
                await GetData();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDataNull at Main");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
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
                _ = TinyInsights.TrackPageViewAsync("CheckPermission at CheckPermissionAdriod12");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        private async Task InitializeNotifications()
        {
            try
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    if (!BellNotification.IsRegisted())
                    {
                        await BellNotificationHelper.RegisterBellNotification(this, GabanaAPI.gbnJWT, DataCashingAll.MerchantId, DataCashingAll.DeviceNo);
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        string SubscripttionType;
        string PackageDetail;
        public async Task GetGabanaInfo()
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
        private async void CheckJwt()
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

        public string SetDetailPackage()
        {
            try
            {
                PackageDetail = "";
                PackageManager PackageManager = this.PackageManager;
                Intent serviceIntent = new Intent("com.android.vending.billing.InAppBillingSevice.BIND");
                serviceIntent.SetPackage("com.android.vending");

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
                        //รายละเอียดจาก gabanaInfo
                        PackageDetail = detail[1] + " " + GetString(Resource.String.branch) + "/ "
                                            + detail[0] + " " + GetString(Resource.String.package_activity_user);
                        break;
                    default:
                        PackageDetail = "1 Branch/ 5 Users (Free)";
                        break;
                }
                return PackageDetail;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDetailPackageOnDB");
                return PackageDetail;
            }
        }

        private void SetFragment()
        {
            try
            {
                #region Old Code
                //framL = Resource.Id.content_frameL;
                //framR = Resource.Id.content_frameR;

                //more_fragment_main = More_Fragment_Main.NewInstance();
                //pos_fragment_main = POS_Fragment_Main.NewInstance();
                //pos_fragment_cart = POS_Fragment_Cart.NewInstance();

                //pos_dialog_additem = POS_Dialog_AddItem.NewInstance();
                //pos_dialog_addtopping = POS_Dialog_AddTopping.NewInstance();

                //item_fragment_main = Item_Fragment_Main.NewInstance();
                //item_fragment_additem = Item_Fragment_AddItem.NewInstance();
                //item_fragment_addcategory = Item_Fragment_AddCategory.NewInstance();
                //item_fragment_addtopping = Item_Fragment_AddTopping.NewInstance();
                //setting_fragment_main = Setting_Fragment_Main.NewInstance();
                //setting_fragment_merchant = Setting_Fragment_Merchant.NewInstance();
                //setting_fragment_branch = Setting_Fragment_Branch.NewInstance();
                //setting_fragment_branchdetail = Setting_Fragment_BranchDetail.NewInstance();
                //setting_fragment_empmanage = Setting_Fragment_Empmanage.NewInstance();
                //setting_fragment_package = Setting_Fragment_Package.NewInstance();
                //setting_fragment_note = Setting_Fragment_Note.NewInstance();
                //setting_fragment_addnote = Setting_Fragment_AddNote.NewInstance();
                //setting_fragment_membertype = Setting_Fragment_MemberType.NewInstance();
                //setting_fragment_addmembertype = Setting_Fragment_AddMemberType.NewInstance();
                //setting_fragment_vat = Setting_Fragment_Vat.NewInstance();
                //setting_fragment_currency = Setting_Fragment_Currency.NewInstance();
                //setting_fragment_decimal = Setting_Fragment_Decimal.NewInstance();
                //setting_fragment_decimalhelp = Setting_Fragment_DecimalHelp.NewInstance();
                //setting_fragment_servicecharge = Setting_Fragment_ServiceCharge.NewInstance();
                //setting_fragment_cashguild = Setting_Fragment_CashGuild.NewInstance();
                //setting_fragment_addcash = Setting_Fragment_AddCash.NewInstance();
                //setting_fragment_giftvoucher = Setting_Fragment_GiftVoucher.NewInstance();
                //setting_fragment_addgiftvoucher = Setting_Fragment_AddGiftVoucher.NewInstance();
                //setting_fragment_giftvouchernumpad = Setting_Fragment_GiftVoucherNumpad.NewInstance();
                //setting_fragment_device = Setting_Fragment_Device.NewInstance();
                //setting_fragment_myqr = Setting_Fragment_MyQR.NewInstance();
                //setting_fragment_addmyqr = Setting_Fragment_AddMyQR.NewInstance();
                //setting_fragment_printer = Setting_Fragment_Printer.NewInstance();

                //customer_fragment_main = Customer_Fragment_Main.NewInstance();
                //customer_fragment_addcustomer = Customer_Fragment_AddCustomer.NewInstance();

                //employee_fragment_main = Employee_Fragment_Main.NewInstance();
                //employee_fragment_addemployee = Employee_Fragment_AddEmployee.NewInstance();
                //employee_fragment_checkuser = Employee_Fragment_CheckUser.NewInstance();

                //dashboard_fragment_main = Dashboard_Fragment_Main.NewInstance();

                //report_fragment_main = Report_Fragment_Main.NewInstance();
                //report_fragment_showdata = Report_Fragment_ShowData.NewInstance();

                //billhistory_fragment_main = BillHistory_Fragment_Main.NewInstance();
                //billhistory_fragment_detail = BillHistory_Fragment_Detail.NewInstance();

                //main_activity.SupportFragmentManager.BeginTransaction().Add(framL, pos_fragment_main, "pos").Hide(pos_fragment_main).Commit();

                //main_activity.SupportFragmentManager.BeginTransaction().Add(framL, item_fragment_main, "item").Hide(item_fragment_main).Commit();

                //main_activity.SupportFragmentManager.BeginTransaction().Add(framL, customer_fragment_main, "customer").Hide(customer_fragment_main).Commit();

                //main_activity.SupportFragmentManager.BeginTransaction().Add(framL, employee_fragment_main, "employee").Hide(employee_fragment_main).Commit();

                //main_activity.SupportFragmentManager.BeginTransaction().Add(framL, dashboard_fragment_main, "dashboard").Hide(dashboard_fragment_main).Commit();

                //main_activity.SupportFragmentManager.BeginTransaction().Add(framL, report_fragment_main, "report").Hide(report_fragment_main).Commit();

                //main_activity.SupportFragmentManager.BeginTransaction().Add(framL, report_fragment_showdata, "showreport").Hide(report_fragment_showdata).Commit();

                //main_activity.SupportFragmentManager.BeginTransaction().Add(framL, setting_fragment_main, "setting").Hide(setting_fragment_main).Commit();

                //main_activity.SupportFragmentManager.BeginTransaction().Add(framL, billhistory_fragment_main, "bill").Hide(billhistory_fragment_main).Commit();


                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, billhistory_fragment_detail, "billdetail").Hide(main_activity.billhistory_fragment_detail).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, employee_fragment_checkuser, "checkuser").Hide(main_activity.employee_fragment_checkuser).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, employee_fragment_addemployee, "addemployee").Hide(main_activity.employee_fragment_addemployee).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, customer_fragment_addcustomer, "addcustomer").Hide(main_activity.customer_fragment_addcustomer).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, item_fragment_addcategory, "addcategory").Hide(main_activity.item_fragment_addcategory).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, item_fragment_addtopping, "addtopping").Hide(main_activity.item_fragment_addtopping).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, item_fragment_additem, "additem").Hide(main_activity.item_fragment_additem).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, pos_fragment_cart, "cart").Hide(main_activity.pos_fragment_cart).Commit();

                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_merchant, "merchant").Hide(main_activity.setting_fragment_merchant).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_branch, "branch").Hide(main_activity.setting_fragment_branch).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_branchdetail, "addbranch").Hide(main_activity.setting_fragment_branchdetail).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_empmanage, "empmanage").Hide(main_activity.setting_fragment_empmanage).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_package, "package").Hide(main_activity.setting_fragment_package).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_note, "note").Hide(main_activity.setting_fragment_note).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_addnote, "addnote").Hide(main_activity.setting_fragment_addnote).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_membertype, "membertype").Hide(main_activity.setting_fragment_membertype).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_addmembertype, "addmembertype").Hide(main_activity.setting_fragment_addmembertype).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_vat, "vat").Hide(main_activity.setting_fragment_vat).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_currency, "currency").Hide(main_activity.setting_fragment_currency).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_decimal, "decimal").Hide(main_activity.setting_fragment_decimal).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_decimalhelp, "decimalhelp").Hide(main_activity.setting_fragment_decimalhelp).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_servicecharge, "servicecharge").Hide(main_activity.setting_fragment_servicecharge).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_cashguild, "cashguild").Hide(main_activity.setting_fragment_cashguild).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_addcash, "addcash").Hide(main_activity.setting_fragment_addcash).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_giftvoucher, "giftvoucher").Hide(main_activity.setting_fragment_giftvoucher).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_addgiftvoucher, "addgiftvoucher").Hide(main_activity.setting_fragment_addgiftvoucher).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_giftvouchernumpad, "giftvouchernum").Hide(main_activity.setting_fragment_giftvouchernumpad).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_device, "device").Hide(main_activity.setting_fragment_device).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_printer, "printer").Hide(main_activity.setting_fragment_printer).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_myqr, "myqr").Hide(main_activity.setting_fragment_myqr).Commit();
                //main_activity.SupportFragmentManager.BeginTransaction().Add(framR, setting_fragment_addmyqr, "addmyqr").Hide(main_activity.setting_fragment_addmyqr).Commit();

                //main_activity.SupportFragmentManager.BeginTransaction().Add(framL, more_fragment_main, "more").Show(more_fragment_main).Commit();

                //activeL = main_activity.more_fragment_main;
                //activeR = main_activity.pos_fragment_cart;

                //LoadFragmentMain(framL, "more", "default");

                //tabSelected = "All";
                //if (Preferences.Get("ViewPos", "") != "Grid")
                //{
                //    flagView = false;
                //}
                //else
                //{
                //    flagView = true;
                //}
                //return; 
                #endregion


                #region Current Code
                framL = Resource.Id.content_frameL;
                framR = Resource.Id.content_frameR;
                more_fragment_main = More_Fragment_Main.NewInstance();

                pos_fragment_main = POS_Fragment_Main.NewInstance();
                pos_fragment_cart = POS_Fragment_Cart.NewInstance();
                pos_dialog_additem = POS_Dialog_AddItem.NewInstance();
                pos_dialog_addtopping = POS_Dialog_AddTopping.NewInstance();
                item_fragment_main = Item_Fragment_Main.NewInstance();
                item_fragment_additem = Item_Fragment_AddItem.NewInstance();
                item_fragment_addcategory = Item_Fragment_AddCategory.NewInstance();
                item_fragment_addtopping = Item_Fragment_AddTopping.NewInstance();
                setting_fragment_main = Setting_Fragment_Main.NewInstance();
                setting_fragment_merchant = Setting_Fragment_Merchant.NewInstance();
                setting_fragment_branch = Setting_Fragment_Branch.NewInstance();
                setting_fragment_branchdetail = Setting_Fragment_BranchDetail.NewInstance();
                setting_fragment_empmanage = Setting_Fragment_Empmanage.NewInstance();
                setting_fragment_package = Setting_Fragment_Package.NewInstance();
                setting_fragment_note = Setting_Fragment_Note.NewInstance();
                setting_fragment_addnote = Setting_Fragment_AddNote.NewInstance();
                setting_fragment_membertype = Setting_Fragment_MemberType.NewInstance();
                setting_fragment_addmembertype = Setting_Fragment_AddMemberType.NewInstance();
                setting_fragment_vat = Setting_Fragment_Vat.NewInstance();
                setting_fragment_currency = Setting_Fragment_Currency.NewInstance();
                setting_fragment_decimal = Setting_Fragment_Decimal.NewInstance();
                setting_fragment_decimalhelp = Setting_Fragment_DecimalHelp.NewInstance();
                setting_fragment_servicecharge = Setting_Fragment_ServiceCharge.NewInstance();
                setting_fragment_cashguild = Setting_Fragment_CashGuild.NewInstance();
                setting_fragment_addcash = Setting_Fragment_AddCash.NewInstance();
                setting_fragment_giftvoucher = Setting_Fragment_GiftVoucher.NewInstance();
                setting_fragment_addgiftvoucher = Setting_Fragment_AddGiftVoucher.NewInstance();
                setting_fragment_giftvouchernumpad = Setting_Fragment_GiftVoucherNumpad.NewInstance();
                setting_fragment_device = Setting_Fragment_Device.NewInstance();
                setting_fragment_myqr = Setting_Fragment_MyQR.NewInstance();
                setting_fragment_addmyqr = Setting_Fragment_AddMyQR.NewInstance();
                setting_fragment_printer = Setting_Fragment_Printer.NewInstance();
                setting_fragment_cashdrawer = Setting_Fragment_CashDrawer.NewInstance();
                customer_fragment_main = Customer_Fragment_Main.NewInstance();
                customer_fragment_addcustomer = Customer_Fragment_AddCustomer.NewInstance();
                employee_fragment_main = Employee_Fragment_Main.NewInstance();
                employee_fragment_addemployee = Employee_Fragment_AddEmployee.NewInstance();
                employee_fragment_checkuser = Employee_Fragment_CheckUser.NewInstance();
                dashboard_fragment_main = Dashboard_Fragment_Main.NewInstance();
                report_fragment_main = Report_Fragment_Main.NewInstance();
                report_fragment_showdata = Report_Fragment_ShowData.NewInstance();
                billhistory_fragment_main = BillHistory_Fragment_Main.NewInstance();
                billhistory_fragment_detail = BillHistory_Fragment_Detail.NewInstance();



                #region Current Code
                //var transaction = SupportFragmentManager.BeginTransaction();

                //transaction.Add(framL, pos_fragment_main, "pos").Hide(pos_fragment_main);
                //transaction.Add(framL, item_fragment_main, "item").Hide(item_fragment_main);
                //transaction.Add(framL, customer_fragment_main, "customer").Hide(customer_fragment_main);
                //transaction.Add(framL, employee_fragment_main, "employee").Hide(employee_fragment_main);
                //transaction.Add(framL, dashboard_fragment_main, "dashboard").Hide(dashboard_fragment_main);
                //transaction.Add(framL, report_fragment_main, "report").Hide(report_fragment_main);
                //transaction.Add(framL, report_fragment_showdata, "showreport").Hide(report_fragment_showdata);
                //transaction.Add(framL, setting_fragment_main, "setting").Hide(setting_fragment_main);
                //transaction.Add(framL, billhistory_fragment_main, "bill").Hide(billhistory_fragment_main);

                //transaction.Add(framR, billhistory_fragment_detail, "billdetail").Hide(main_activity.billhistory_fragment_detail);
                //transaction.Add(framR, employee_fragment_checkuser, "checkuser").Hide(main_activity.employee_fragment_checkuser);
                //transaction.Add(framR, employee_fragment_addemployee, "addemployee").Hide(main_activity.employee_fragment_addemployee);
                //transaction.Add(framR, customer_fragment_addcustomer, "addcustomer").Hide(main_activity.customer_fragment_addcustomer);
                //transaction.Add(framR, item_fragment_addcategory, "addcategory").Hide(main_activity.item_fragment_addcategory);
                //transaction.Add(framR, item_fragment_addtopping, "addtopping").Hide(main_activity.item_fragment_addtopping);
                //transaction.Add(framR, item_fragment_additem, "additem").Hide(main_activity.item_fragment_additem);
                //transaction.Add(framR, pos_fragment_cart, "cart").Hide(main_activity.pos_fragment_cart);

                //transaction.Add(framR, setting_fragment_merchant, "merchant").Hide(main_activity.setting_fragment_merchant);
                //transaction.Add(framR, setting_fragment_branch, "branch").Hide(main_activity.setting_fragment_branch);
                //transaction.Add(framR, setting_fragment_branchdetail, "addbranch").Hide(main_activity.setting_fragment_branchdetail);
                //transaction.Add(framR, setting_fragment_empmanage, "empmanage").Hide(main_activity.setting_fragment_empmanage);
                //transaction.Add(framR, setting_fragment_package, "package").Hide(main_activity.setting_fragment_package);
                //transaction.Add(framR, setting_fragment_note, "note").Hide(main_activity.setting_fragment_note);
                //transaction.Add(framR, setting_fragment_addnote, "addnote").Hide(main_activity.setting_fragment_addnote);
                //transaction.Add(framR, setting_fragment_membertype, "membertype").Hide(main_activity.setting_fragment_membertype);
                //transaction.Add(framR, setting_fragment_addmembertype, "addmembertype").Hide(main_activity.setting_fragment_addmembertype);
                //transaction.Add(framR, setting_fragment_vat, "vat").Hide(main_activity.setting_fragment_vat);
                //transaction.Add(framR, setting_fragment_currency, "currency").Hide(main_activity.setting_fragment_currency);
                //transaction.Add(framR, setting_fragment_decimal, "decimal").Hide(main_activity.setting_fragment_decimal);
                //transaction.Add(framR, setting_fragment_decimalhelp, "decimalhelp").Hide(main_activity.setting_fragment_decimalhelp);
                //transaction.Add(framR, setting_fragment_servicecharge, "servicecharge").Hide(main_activity.setting_fragment_servicecharge);
                //transaction.Add(framR, setting_fragment_cashguild, "cashguild").Hide(main_activity.setting_fragment_cashguild);
                //transaction.Add(framR, setting_fragment_addcash, "addcash").Hide(main_activity.setting_fragment_addcash);
                //transaction.Add(framR, setting_fragment_giftvoucher, "giftvoucher").Hide(main_activity.setting_fragment_giftvoucher);
                //transaction.Add(framR, setting_fragment_addgiftvoucher, "addgiftvoucher").Hide(main_activity.setting_fragment_addgiftvoucher);
                //transaction.Add(framR, setting_fragment_giftvouchernumpad, "giftvouchernum").Hide(main_activity.setting_fragment_giftvouchernumpad);
                //transaction.Add(framR, setting_fragment_device, "device").Hide(main_activity.setting_fragment_device);
                //transaction.Add(framR, setting_fragment_printer, "printer").Hide(main_activity.setting_fragment_printer);
                //transaction.Add(framR, setting_fragment_myqr, "myqr").Hide(main_activity.setting_fragment_myqr);
                //transaction.Add(framR, setting_fragment_addmyqr, "addmyqr").Hide(main_activity.setting_fragment_addmyqr);
                //transaction.Add(framL, more_fragment_main, "more").Show(more_fragment_main);

                //transaction.Commit();
                #endregion

                var transaction = SupportFragmentManager.BeginTransaction();

                // ฟังก์ชันเพิ่มและซ่อน Fragment ใน content_frameL
                void AddAndHideFragmentInContentFrameL(AndroidX.Fragment.App.Fragment fragment, string tag)
                {
                    transaction.Add(Resource.Id.content_frameL, fragment, tag).Hide(fragment);
                }

                // ฟังก์ชันเพิ่มและซ่อน Fragment ใน content_frameR
                void AddAndHideFragmentInContentFrameR(AndroidX.Fragment.App.Fragment fragment, string tag)
                {
                    transaction.Add(Resource.Id.content_frameR, fragment, tag).Hide(fragment);
                }

                // เพิ่มและซ่อน Fragment ที่ต้องการใน content_frameL
                AddAndHideFragmentInContentFrameL(pos_fragment_main, "pos");
                AddAndHideFragmentInContentFrameL(item_fragment_main, "item");
                AddAndHideFragmentInContentFrameL(customer_fragment_main, "customer");
                AddAndHideFragmentInContentFrameL(employee_fragment_main, "employee");
                AddAndHideFragmentInContentFrameL(dashboard_fragment_main, "dashboard");
                AddAndHideFragmentInContentFrameL(report_fragment_main, "report");
                AddAndHideFragmentInContentFrameL(report_fragment_showdata, "showreport");
                AddAndHideFragmentInContentFrameL(setting_fragment_main, "setting");
                AddAndHideFragmentInContentFrameL(billhistory_fragment_main, "bill");

                // เพิ่มและซ่อน Fragment ที่ต้องการใน content_frameR
                AddAndHideFragmentInContentFrameR(billhistory_fragment_detail, "billdetail");
                AddAndHideFragmentInContentFrameR(employee_fragment_checkuser, "checkuser");
                AddAndHideFragmentInContentFrameR(employee_fragment_addemployee, "addemployee");
                AddAndHideFragmentInContentFrameR(customer_fragment_addcustomer, "addcustomer");
                AddAndHideFragmentInContentFrameR(item_fragment_addcategory, "addcategory");
                AddAndHideFragmentInContentFrameR(item_fragment_addtopping, "addtopping");
                AddAndHideFragmentInContentFrameR(item_fragment_additem, "additem");
                AddAndHideFragmentInContentFrameR(pos_fragment_cart, "cart");

                // เพิ่มและซ่อน Fragment ที่ต้องการใน content_frameR
                AddAndHideFragmentInContentFrameR(setting_fragment_merchant, "merchant");
                AddAndHideFragmentInContentFrameR(setting_fragment_branch, "branch");
                AddAndHideFragmentInContentFrameR(setting_fragment_branchdetail, "addbranch");
                AddAndHideFragmentInContentFrameR(setting_fragment_empmanage, "empmanage");
                AddAndHideFragmentInContentFrameR(setting_fragment_package, "package");
                AddAndHideFragmentInContentFrameR(setting_fragment_note, "note");
                AddAndHideFragmentInContentFrameR(setting_fragment_addnote, "addnote");
                AddAndHideFragmentInContentFrameR(setting_fragment_membertype, "membertype");
                AddAndHideFragmentInContentFrameR(setting_fragment_addmembertype, "addmembertype");
                AddAndHideFragmentInContentFrameR(setting_fragment_vat, "vat");
                AddAndHideFragmentInContentFrameR(setting_fragment_currency, "currency");
                AddAndHideFragmentInContentFrameR(setting_fragment_decimal, "decimal");
                AddAndHideFragmentInContentFrameR(setting_fragment_decimalhelp, "decimalhelp");
                AddAndHideFragmentInContentFrameR(setting_fragment_servicecharge, "servicecharge");
                AddAndHideFragmentInContentFrameR(setting_fragment_cashguild, "cashguild");
                AddAndHideFragmentInContentFrameR(setting_fragment_addcash, "addcash");
                AddAndHideFragmentInContentFrameR(setting_fragment_giftvoucher, "giftvoucher");
                AddAndHideFragmentInContentFrameR(setting_fragment_addgiftvoucher, "addgiftvoucher");
                AddAndHideFragmentInContentFrameR(setting_fragment_giftvouchernumpad, "giftvouchernum");
                AddAndHideFragmentInContentFrameR(setting_fragment_device, "device");
                AddAndHideFragmentInContentFrameR(setting_fragment_printer, "printer");
                AddAndHideFragmentInContentFrameR(setting_fragment_myqr, "myqr");
                AddAndHideFragmentInContentFrameR(setting_fragment_addmyqr, "addmyqr");
                AddAndHideFragmentInContentFrameR(setting_fragment_cashdrawer, "cashdrawer");

                transaction.Add(framL, more_fragment_main, "more");

                activeL = more_fragment_main;
                activeR = pos_fragment_cart;

                LoadFragmentMain(framL, "more", "default");

                transaction.Commit();
                tabSelected = "All";
                if (Preferences.Get("ViewPos", "") != "Grid")
                {
                    flagView = false;
                }
                else
                {
                    flagView = true;
                }
                return;
                #endregion
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
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


        int CAMERA_REQUEST = 0;
        int GALLERY_PICTURE = 2;
        Android.Net.Uri cameraTakePictureUri;
        public void CameraTakePicture()
        {
            try
            {
                Intent CamIntent = new Intent(MediaStore.ActionImageCapture);
                CamIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);

                //ถ้ากำหนดชื่อชื่อไฟล์ มันจะ Save ลงที่ไฟล์นี้แล้วส่งไปให้ OnActivityResult

                string filePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                                                         Android.OS.Environment.DirectoryPictures,
                                                         "file_" + Guid.NewGuid().ToString() + ".jpg");

                Java.IO.File tempFile = new Java.IO.File(filePath);
                Android.Net.Uri tempURI;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    tempURI = Android.Support.V4.Content.FileProvider.GetUriForFile(Application.Context, Application.Context.ApplicationContext.PackageName + ".fileProvider", tempFile);

                    //this is the stuff that was missing - but only if you get the uri from FileProvider
                    CamIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);
                }
                else
                {
                    tempURI = Android.Net.Uri.FromFile(tempFile);
                }
                cameraTakePictureUri = tempURI;
                CamIntent.PutExtra(MediaStore.ExtraOutput, tempURI);
                CamIntent.PutExtra("return-data", false);
                CamIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);

                StartActivityForResult(CamIntent, CAMERA_REQUEST);
            }
            catch (ActivityNotFoundException ed)
            {
                Toast.MakeText(this, "Can not open CAMERA", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Tack Pic at add merchant");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        public void GalleryOpen()
        {
            try
            {
                string action;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    action = Intent.ActionOpenDocument;
                }
                else
                {
                    action = Intent.ActionPick;
                }
                Intent GalIntent = new Intent(action, MediaStore.Images.Media.ExternalContentUri);
                GalIntent.SetType("image/*");
                StartActivityForResult(Intent.CreateChooser(GalIntent, "Select image from gallery"), GALLERY_PICTURE);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GalleryOpen at Merchant");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        int RESULT_OK = -1;
        int CROP_REQUEST = 1;
        Android.Net.Uri keepCropedUri = null;
        Android.Graphics.Bitmap bitmap;
        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                if (requestCode == CAMERA_REQUEST && Convert.ToInt32(resultCode) == RESULT_OK)
                {
                    // Solution 1 : เอาชื่อไฟล์ที่ได้ส่งไป crop
                    CropImage(cameraTakePictureUri);

                    // Solution 2 : เอา Data ที่เป็น Bitmap Save ลง Temp โรสำ แล้ว ชื่อไฟล์ที่ได้ส่งไป crop
                    //            : แบบนี้ ภาพไม่ชัด
                    //Bundle bundle = data.Extras;
                    //Bitmap bitmap = (Bitmap)bundle.GetParcelable("data");
                }
                else if (requestCode == GALLERY_PICTURE && Convert.ToInt32(resultCode) == RESULT_OK)
                {
                    // มาจาก User เลื่อกรุปจาก Gallory : (case นี้จะมี uri)
                    if (data != null)
                    {
                        Android.Net.Uri selectPictureUri = data.Data;
                        CropImage(selectPictureUri);
                    }
                    else
                    {
                        Toast.MakeText(this, "error : GALLERY_PICTURE data is null", ToastLength.Short).Show();
                        return;
                    }
                }
                else if (requestCode == CROP_REQUEST && Convert.ToInt32(resultCode) == RESULT_OK)
                {
                    // มาจาก User เลื่อกถ่ายรูป หรือ เลื่อกรุปจาก Gallory แล้วผ่าน function CropImage();
                    if (data != null)
                    {

                        Bundle bundle = data.Extras;

                        // Solution 1 : เอาค่า BitMap มาจัดการเลย (ok) แต่ใช้กับ Android 10 ไม่ได้ครับ
                        //Bitmap bitmap = (Bitmap)bundle.GetParcelable("data");

                        // Solution 2 : อ่าน BitMap จากไฟล์ (ok)
                        Android.Net.Uri cropImageURI = keepCropedUri;
                        bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);

                        // Solution 3 : อ่าน BitMap จากไฟล์ โดยเอาค่าไฟล์จาก bundle.GetParcelable(MediaStore.ExtraOutput) : จะ error กับ Andord ที่ต่ำกว่า Android.N
                        //Android.Net.Uri cropImageURI = (Android.Net.Uri)bundle.GetParcelable(MediaStore.ExtraOutput); // ใช้กับ Andord ที่ต่ำกว่า Android.N ไม่ได้ เพราะจะมีค่าเป็น Null
                        //Bitmap bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);

                        DataCashing.flagChooseMedia = true;

                        if (activeR == main_activity.setting_fragment_merchant)
                        {
                            Setting_Fragment_Merchant.imgProfile.SetImageBitmap(bitmap);
                            Setting_Fragment_Merchant.keepCropedUri = keepCropedUri;
                            Setting_Fragment_Merchant.fragment_main.CheckDataChange();
                        }

                        if (activeR == main_activity.setting_fragment_addmyqr)
                        {
                            Setting_Fragment_AddMyQR.imgQRCode.SetImageBitmap(bitmap);
                            Setting_Fragment_AddMyQR.keepCropedUri = keepCropedUri;
                            Setting_Fragment_AddMyQR.fragment_addmyqr.CheckDataChange();
                        }

                        if (activeR == main_activity.customer_fragment_addcustomer)
                        {
                            Customer_Fragment_AddCustomer.imgProfile.SetImageBitmap(bitmap);
                            Customer_Fragment_AddCustomer.keepCropedUri = keepCropedUri;
                            Customer_Fragment_AddCustomer.fragment_main.CheckDataChange();
                        }

                        if (activeR == main_activity.item_fragment_additem)
                        {
                            Item_Fragment_AddItem.imageViewItem.SetImageBitmap(bitmap);
                            Item_Fragment_AddItem.keepCropedUri = keepCropedUri;
                            Item_Fragment_AddItem.fragment_additem.CheckDataChange();
                        }

                        if (activeR == main_activity.item_fragment_addtopping)
                        {
                            Item_Fragment_AddTopping.imageViewItem.SetImageBitmap(bitmap);
                            Item_Fragment_AddTopping.keepCropedUri = keepCropedUri;
                            Item_Fragment_AddTopping.fragment_addtopping.CheckDataChange();
                        }

                        if (POS_Dialog_AddItem.dialog_additem != null)
                        {
                            if (POS_Dialog_AddItem.dialog_additem.Dialog.IsShowing)
                            {
                                POS_Dialog_AddItem.imageViewItem.SetImageBitmap(bitmap);
                                POS_Dialog_AddItem.keepCropedUri = keepCropedUri;
                                POS_Dialog_AddItem.dialog_additem.CheckDataChange();
                            }
                        }

                        if (POS_Dialog_AddTopping.dialog_addtopping != null)
                        {
                            if (POS_Dialog_AddTopping.dialog_addtopping.Dialog.IsShowing)
                            {
                                POS_Dialog_AddTopping.imageViewItem.SetImageBitmap(bitmap);
                                POS_Dialog_AddTopping.keepCropedUri = keepCropedUri;
                                POS_Dialog_AddTopping.dialog_addtopping.CheckDataChange();
                            }
                        }

                        if (Cart_Dailog_AddCustomer.cart_dailog_adCustomer != null)
                        {
                            if (Cart_Dailog_AddCustomer.cart_dailog_adCustomer.Dialog.IsShowing)
                            {
                                Cart_Dailog_AddCustomer.imgProfile.SetImageBitmap(bitmap);
                                Cart_Dailog_AddCustomer.keepCropedUri = keepCropedUri;
                                Cart_Dailog_AddCustomer.cart_dailog_adCustomer.CheckDataChange();
                            }
                        }

                    }
                    else
                    {
                        Toast.MakeText(this, "error : CROP_REQUEST data is null", ToastLength.Short).Show();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnActivityResult at Merchant");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        public async void CropImage(Android.Net.Uri imageUri)
        {
            try
            {

                Intent CropIntent = new Intent("com.android.camera.action.CROP");
                CropIntent.SetDataAndType(imageUri, "image/*");
                CropIntent.AddFlags(ActivityFlags.GrantReadUriPermission); // **** ต้อง อนุญาติให้อ่าน ได้ด้วยนะครับ สำคัญ มันจะสามารถอ่านไฟล์ที่ได้จากการ CaptureImage ได้ ****

                CropIntent.PutExtra("crop", "true");
                CropIntent.PutExtra("outputX", 600);
                CropIntent.PutExtra("outputY", 600);
                CropIntent.PutExtra("aspectX", 1);
                CropIntent.PutExtra("aspectY", 1);
                CropIntent.PutExtra("scaleUpIfNeeded", true);
                // do not use return data for big images
                CropIntent.PutExtra("return-data", false);


                //string cropedFilePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                //                                         Android.OS.Environment.DirectoryPictures,
                //                                         "file_" + Guid.NewGuid().ToString() + ".jpg");

                //Java.IO.File cropedFile = new Java.IO.File(cropedFilePath);


                string filePath = DataCashingAll.PathImageBill;
                string fullName = filePath + "file_" + Guid.NewGuid().ToString() + ".jpg";

                Java.IO.File cropedFile = new Java.IO.File(fullName);


                // create new file handle to get full resolution crop
                Android.Net.Uri cropedUri;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    cropedUri = Android.Support.V4.Content.FileProvider.GetUriForFile(Application.Context, Application.Context.ApplicationContext.PackageName + ".fileProvider", cropedFile);

                    //this is the stuff that was missing - but only if you get the uri from FileProvider
                    CropIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);

                    //กำหนดสิทธิให้ Inten อื่นสามารถ เขียน Uri ได้
                    List<ResolveInfo> resolvedIntentActivities = Application.Context.PackageManager.QueryIntentActivities(CropIntent, PackageInfoFlags.MatchDefaultOnly).ToList();
                    foreach (ResolveInfo resolvedIntentInfo in resolvedIntentActivities)
                    {
                        String packageName = resolvedIntentInfo.ActivityInfo.PackageName;
                        Application.Context.GrantUriPermission(packageName, cropedUri, ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
                    }
                }
                else
                {
                    cropedUri = Android.Net.Uri.FromFile(cropedFile);
                }
                keepCropedUri = cropedUri;  // เก็บเอาไว้ใช้งานที่ OnActionResult เพราะ Android ที่ต่ำกว่า Android.N จะ Get เอาจาก ค่าที่ส่งไปใน Functio ไม่ได้

                CropIntent.PutExtra(MediaStore.ExtraOutput, cropedUri);
                StartActivityForResult(CropIntent, CROP_REQUEST);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("CropImage at add merchant");
                Toast.MakeText(this, "error : " + ex.Message, ToastLength.Short).Show(); return;
            }
        }

        private async Task InitializeLanguage()
        {
            try
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
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task InitializeUserAccountInfo()
        {
            try
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
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void SetUsePinCode(ORM.MerchantDB.UserAccountInfo usePinCode)
        {
            try
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
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void SetLocale(Android.Content.Res.Configuration conf, string language)
        {
            try
            {
                conf.SetLocale(new Java.Util.Locale(language));
                Resources.UpdateConfiguration(conf, Resources.DisplayMetrics);
                Preferences.Set("Language", language);
                DataCashing.Language = language;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                DataCashing.CheckNet = await GabanaAPI.CheckNetWork();
                LoginType = Preferences.Get("LoginType", "").ToLower();
                usernamelogin = Preferences.Get("User", "");

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

                RunOnUiThread(() =>
                {
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
            try
            {
                var fragment = new Login_Dialog_UpdateApp();
                fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Login_Dialog_UpdateApp));
                return;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
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
            }
        }

        private void CombineUI()
        {
            try
            {
                //button menu
                imgMore = FindViewById<ImageView>(Resource.Id.imgMore);
                imgPOS = FindViewById<ImageView>(Resource.Id.imgPOS);
                imgItem = FindViewById<ImageView>(Resource.Id.imgItem);
                imgCustomer = FindViewById<ImageView>(Resource.Id.imgCustomer);
                imgEmployee = FindViewById<ImageView>(Resource.Id.imgEmployee);
                imgDashboard = FindViewById<ImageView>(Resource.Id.imgDashboard);
                imgReport = FindViewById<ImageView>(Resource.Id.imgReport);
                imgSetting = FindViewById<ImageView>(Resource.Id.imgSetting);
                imgBillHistory = FindViewById<ImageView>(Resource.Id.imgBillHistory);

                lnMore = FindViewById<LinearLayout>(Resource.Id.lnMore);
                lnPos = FindViewById<LinearLayout>(Resource.Id.lnPos);
                lnItem = FindViewById<LinearLayout>(Resource.Id.lnItem);
                lnCustomer = FindViewById<LinearLayout>(Resource.Id.lnCustomer);
                lnEmployee = FindViewById<LinearLayout>(Resource.Id.lnEmployee);
                lnDashboard = FindViewById<LinearLayout>(Resource.Id.lnDashboard);
                lnReport = FindViewById<LinearLayout>(Resource.Id.lnReport);
                lnSetting = FindViewById<LinearLayout>(Resource.Id.lnSetting);
                lnBillHistory = FindViewById<LinearLayout>(Resource.Id.lnBillHistory);

                lnMore.Click += LnMore_Click;
                lnPos.Click += LnPos_Click;
                lnItem.Click += LnItem_Click;
                lnCustomer.Click += LnCustomer_Click;
                lnEmployee.Click += LnEmployee_Click;
                lnDashboard.Click += LnDashboard_Click;
                lnReport.Click += LnReport_Click;
                lnSetting.Click += LnSetting_Click;
                lnBillHistory.Click += LnBillHistory_Click;

                //fragment
                frameLeft = FindViewById<FrameLayout>(Resource.Id.content_frameL);
                frameRight = FindViewById<FrameLayout>(Resource.Id.content_frameR);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CombineUI at Main");
            }


        }
        private void CheckForgetPincode()
        {
            try
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
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckForgetPincode at Main");
            }
        }
        private async void LnBillHistory_Click(object sender, EventArgs e)
        {
            LoadFragmentMain(Resource.Id.lnBillHistory, "billhistory", "default");
        }
        private async void LnSetting_Click(object sender, EventArgs e)
        {
            LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
        }
        private async void LnReport_Click(object sender, EventArgs e)
        {
            LoadFragmentMain(Resource.Id.lnReport, "report", "default");
        }
        private async void LnDashboard_Click(object sender, EventArgs e)
        {
            LoadFragmentMain(Resource.Id.lnDashboard, "dashboard", "default");
        }
        private async void LnEmployee_Click(object sender, EventArgs e)
        {
            LoadFragmentMain(Resource.Id.lnEmployee, "employee", "default");
        }
        private async void LnCustomer_Click(object sender, EventArgs e)
        {
            LoadFragmentMain(Resource.Id.lnCustomer, "customer", "default");
        }
        private async void LnItem_Click(object sender, EventArgs e)
        {
            LoadFragmentMain(Resource.Id.lnItem, "item", "default");
        }
        private async void LnMore_Click(object sender, EventArgs e)
        {
            LoadFragmentMain(Resource.Id.lnMore, "more", "default");
        }
        private async void LnPos_Click(object sender, EventArgs e)
        {
            LoadFragmentMain(Resource.Id.lnPos, "pos", "default");
        }

        public async void LoadFragmentMain(int id, string menu, string subpage)
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                AndroidX.Fragment.App.Fragment fragmentLeft = null;
                AndroidX.Fragment.App.Fragment fragmentRight = null;

                #region Old Code
                //lnMore.SetBackgroundColor(Android.Graphics.Color.White);
                //lnPos.SetBackgroundColor(Android.Graphics.Color.White);
                //lnItem.SetBackgroundColor(Android.Graphics.Color.White);
                //lnCustomer.SetBackgroundColor(Android.Graphics.Color.White);
                //lnEmployee.SetBackgroundColor(Android.Graphics.Color.White);
                //lnDashboard.SetBackgroundColor(Android.Graphics.Color.White);
                //lnReport.SetBackgroundColor(Android.Graphics.Color.White);
                //lnSetting.SetBackgroundColor(Android.Graphics.Color.White);
                //lnBillHistory.SetBackgroundColor(Android.Graphics.Color.White);

                //imgMore.SetImageResource(Resource.Mipmap.MenuMoreN);
                //imgPOS.SetImageResource(Resource.Mipmap.MenuPOSN);
                //imgItem.SetImageResource(Resource.Mipmap.MenuItemN);
                //imgCustomer.SetImageResource(Resource.Mipmap.MenuCustomerN);
                //imgEmployee.SetImageResource(Resource.Mipmap.MenuEmployeeN);
                //imgDashboard.SetImageResource(Resource.Mipmap.MenuDashboardN);
                //imgReport.SetImageResource(Resource.Mipmap.MenuReportN);
                //imgSetting.SetImageResource(Resource.Mipmap.MenuSettingN);
                //imgBillHistory.SetImageResource(Resource.Mipmap.MenuBillHistoryN);
                //frameLeft.Visibility = ViewStates.Visible;

                //if (subpage == "default")
                //{
                //    View view = main_activity.CurrentFocus;
                //    if (view != null)
                //    {
                //        main_activity.CloseKeyboard(view);
                //    }
                //}

                //switch (menu)
                //{
                //    case "more":
                //        frameRight.Visibility = ViewStates.Gone;
                //        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeL).Show(main_activity.more_fragment_main).Commit();
                //        main_activity.activeL = main_activity.more_fragment_main;
                //        lnMore.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                //        imgMore.SetImageResource(Resource.Mipmap.MenuMore);
                //        break;
                //    case "pos":
                //        frameRight.Visibility = ViewStates.Visible;
                //        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeL).Show(main_activity.pos_fragment_main).Commit();
                //        main_activity.activeL = main_activity.pos_fragment_main;
                //        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.pos_fragment_cart).Commit();
                //        main_activity.activeR = main_activity.pos_fragment_cart;
                //        lnPos.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                //        imgPOS.SetImageResource(Resource.Mipmap.MenuPOS);
                //        pos_fragment_cart.OnResume();
                //        pos_fragment_main.OnResume();
                //        break;
                //    case "item":
                //        frameRight.Visibility = ViewStates.Visible;
                //        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeL).Show(main_activity.item_fragment_main).Commit();
                //        main_activity.activeL = main_activity.item_fragment_main;
                //        lnItem.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                //        imgItem.SetImageResource(Resource.Mipmap.MenuItem);

                //        switch (subpage)
                //        {
                //            case "default":
                //                frameRight.Visibility = ViewStates.Invisible;
                //                break;
                //            case "additem":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.item_fragment_additem).Commit();
                //                main_activity.activeR = main_activity.item_fragment_additem;
                //                item_fragment_additem.OnResume();
                //                break;
                //            case "addtopping":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.item_fragment_addtopping).Commit();
                //                main_activity.activeR = main_activity.item_fragment_addtopping;
                //                item_fragment_addtopping.OnResume();
                //                break;
                //            case "addcategory":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.item_fragment_addcategory).Commit();
                //                main_activity.activeR = main_activity.item_fragment_addcategory;
                //                item_fragment_addcategory.OnResume();
                //                break;
                //            default:
                //                break;
                //        }
                //        break;
                //    case "customer":
                //        frameRight.Visibility = ViewStates.Visible;
                //        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeL).Show(main_activity.customer_fragment_main).Commit();
                //        main_activity.activeL = main_activity.customer_fragment_main;
                //        lnCustomer.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                //        imgCustomer.SetImageResource(Resource.Mipmap.MenuCustomer);
                //        switch (subpage)
                //        {
                //            case "default":
                //                frameRight.Visibility = ViewStates.Invisible;
                //                CustomerManage customerManage = new CustomerManage();
                //                var listCustomer = await customerManage.GetAllCustomer();
                //                if (listCustomer.Count == 0)
                //                {
                //                    frameRight.Visibility = ViewStates.Gone;
                //                }
                //                customer_fragment_main.OnResume();
                //                break;
                //            case "addcustomer":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.customer_fragment_addcustomer).Commit();
                //                main_activity.activeR = main_activity.customer_fragment_addcustomer;
                //                customer_fragment_addcustomer.OnResume();
                //                break;
                //            default:
                //                break;
                //        }
                //        break;
                //    case "employee":
                //        frameRight.Visibility = ViewStates.Visible;
                //        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeL).Show(main_activity.employee_fragment_main).Commit();
                //        main_activity.activeL = main_activity.employee_fragment_main;
                //        lnEmployee.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                //        imgEmployee.SetImageResource(Resource.Mipmap.MenuEmployee);
                //        switch (subpage)
                //        {
                //            case "default":
                //                frameRight.Visibility = ViewStates.Invisible;
                //                break;
                //            case "checkuser":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.employee_fragment_checkuser).Commit();
                //                main_activity.activeR = main_activity.employee_fragment_checkuser;
                //                employee_fragment_checkuser.OnResume();
                //                break;
                //            case "addemployee":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.employee_fragment_addemployee).Commit();
                //                main_activity.activeR = main_activity.employee_fragment_addemployee;
                //                employee_fragment_addemployee.OnResume();
                //                break;
                //            default:
                //                break;
                //        }
                //        break;
                //    case "dashboard":
                //        frameRight.Visibility = ViewStates.Gone;
                //        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeL).Show(main_activity.dashboard_fragment_main).Commit();
                //        main_activity.activeL = main_activity.dashboard_fragment_main;
                //        lnDashboard.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                //        imgDashboard.SetImageResource(Resource.Mipmap.MenuDashboard);
                //        dashboard_fragment_main.OnResume();
                //        break;
                //    case "report":
                //        frameRight.Visibility = ViewStates.Gone;
                //        var fmReport = main_activity.SupportFragmentManager.FindFragmentByTag("report");
                //        if (fmReport != null)
                //        {
                //            main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeL).Show(main_activity.report_fragment_main).Commit();
                //            dashboard_fragment_main.OnResume();
                //        }
                //        main_activity.activeL = main_activity.report_fragment_main;

                //        lnReport.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                //        imgReport.SetImageResource(Resource.Mipmap.MenuReport);
                //        break;
                //    case "showreport":
                //        frameRight.Visibility = ViewStates.Gone;
                //        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeL).Show(main_activity.report_fragment_showdata).Commit();
                //        lnReport.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                //        imgReport.SetImageResource(Resource.Mipmap.MenuReport);
                //        main_activity.activeL = main_activity.report_fragment_showdata;
                //        report_fragment_showdata.OnResume();
                //        break;
                //    case "setting":
                //        frameRight.Visibility = ViewStates.Visible;
                //        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeL).Show(main_activity.setting_fragment_main).Commit();
                //        main_activity.activeL = main_activity.setting_fragment_main;
                //        lnSetting.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                //        imgSetting.SetImageResource(Resource.Mipmap.MenuSetting);
                //        switch (subpage)
                //        {
                //            case "default":
                //                frameRight.Visibility = ViewStates.Invisible;
                //                break;
                //            case "merchant":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_merchant).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_merchant;
                //                setting_fragment_merchant.OnResume();
                //                break;
                //            case "branch":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_branch).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_branch;
                //                setting_fragment_branch.OnResume();
                //                break;
                //            case "addbranch":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_branchdetail).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_branchdetail;
                //                setting_fragment_branchdetail.OnResume();
                //                break;
                //            case "empmanage":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_empmanage).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_empmanage;
                //                setting_fragment_empmanage.OnResume();
                //                break;
                //            case "package":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_package).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_package;
                //                setting_fragment_package.OnResume();
                //                break;
                //            case "note":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_note).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_note;
                //                setting_fragment_note.OnResume();
                //                break;
                //            case "addnote":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_addnote).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_addnote;
                //                setting_fragment_addnote.OnResume();
                //                break;
                //            case "membertype":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_membertype).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_membertype;
                //                setting_fragment_membertype.OnResume();
                //                break;
                //            case "addmembertype":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_addmembertype).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_addmembertype;
                //                setting_fragment_addmembertype.OnResume();
                //                break;
                //            case "vat":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_vat).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_vat;
                //                setting_fragment_vat.OnResume();
                //                break;
                //            case "currency":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_currency).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_currency;
                //                setting_fragment_currency.OnResume();
                //                break;
                //            case "decimal":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_decimal).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_decimal;
                //                setting_fragment_currency.OnResume();
                //                break;
                //            case "decimalhelp":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_decimalhelp).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_decimalhelp;
                //                setting_fragment_decimalhelp.OnResume();
                //                break;
                //            case "servicecharge":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_servicecharge).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_servicecharge;
                //                setting_fragment_cashguild.OnResume();
                //                break;
                //            case "cashguild":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_cashguild).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_cashguild;
                //                setting_fragment_cashguild.OnResume();
                //                break;
                //            case "addcash":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_addcash).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_addcash;
                //                setting_fragment_addcash.OnResume();
                //                break;
                //            case "giftvoucher":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_giftvoucher).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_giftvoucher;
                //                setting_fragment_giftvoucher.OnResume();
                //                break;
                //            case "addgiftvoucher":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_addgiftvoucher).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_addgiftvoucher;
                //                setting_fragment_addgiftvoucher.OnResume();
                //                break;
                //            case "giftvouchernum":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_giftvouchernumpad).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_giftvouchernumpad;
                //                setting_fragment_giftvouchernumpad.OnResume();
                //                break;
                //            case "device":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_device).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_device;
                //                setting_fragment_device.OnResume();
                //                break;
                //            case "printer":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_printer).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_printer;
                //                setting_fragment_printer.OnResume();
                //                break;
                //            case "myqr":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_myqr).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_myqr;
                //                setting_fragment_myqr.OnResume();
                //                break;
                //            case "addmyqr":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_addmyqr).Commit();
                //                main_activity.activeR = main_activity.setting_fragment_addmyqr;
                //                setting_fragment_addmyqr.OnResume();
                //                break;
                //            default:
                //                break;
                //        }
                //        break;
                //    case "billhistory":
                //        frameRight.Visibility = ViewStates.Visible;
                //        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeL).Show(main_activity.billhistory_fragment_main).Commit();
                //        main_activity.activeL = main_activity.billhistory_fragment_main;
                //        lnBillHistory.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                //        imgBillHistory.SetImageResource(Resource.Mipmap.MenuBillHistory);
                //        switch (subpage)
                //        {
                //            case "default":
                //                frameRight.Visibility = ViewStates.Invisible;
                //                break;
                //            case "billdetail":
                //                frameRight.Visibility = ViewStates.Visible;
                //                main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.billhistory_fragment_detail).Commit();
                //                main_activity.activeR = main_activity.billhistory_fragment_detail;
                //                billhistory_fragment_detail.OnResume();
                //                break;

                //            default:
                //                break;
                //        }
                //        break;
                //    default:
                //        fragmentLeft = main_activity.more_fragment_main;
                //        lnMore.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                //        imgMore.SetImageResource(Resource.Mipmap.MenuMore);
                //        break;

                //} 
                #endregion

                ResetMenuUI();

                HandleDefaultSubpage(subpage);

                switch (menu)
                {
                    case "more":
                        ShowMoreFragment(more_fragment_main);
                        break;
                    case "pos":
                        ShowPosFragments();
                        break;
                    case "item":
                        ShowItemFragments(subpage);
                        break;
                    case "customer":
                        ShowCustomerFragments(subpage);
                        break;
                    case "employee":
                        ShowEmployeeFragments(subpage);
                        break;
                    case "dashboard":
                        ShowDashboardFragment();
                        break;
                    case "report":
                        ShowReportFragments(subpage);
                        break;
                    case "showreport":
                        ShowShowReportFragments();
                        break;
                    case "setting":
                        ShowSettingFragments(subpage);
                        break;
                    case "billhistory":
                        ShowBillHistoryFragments(subpage);
                        break;
                    default:
                        // Show default fragment
                        ShowMoreFragment(more_fragment_main);
                        break;
                }

                // Dismiss the loading dialog
                dialogLoading?.DismissAllowingStateLoss();
                dialogLoading?.Dismiss();
            }
            catch (Exception ex)
            {
                dialogLoading?.DismissAllowingStateLoss();
                dialogLoading?.Dismiss();
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void ShowPosFragments()
        {
            try
            {
                frameLeft.Visibility = ViewStates.Visible;
                frameRight.Visibility = ViewStates.Visible;
                main_activity.SupportFragmentManager.BeginTransaction()
                    .Hide(main_activity.activeL)
                    .Show(main_activity.pos_fragment_main)
                    .Commit();
                main_activity.activeL = main_activity.pos_fragment_main;
                main_activity.SupportFragmentManager.BeginTransaction()
                    .Hide(main_activity.activeR)
                    .Show(main_activity.pos_fragment_cart)
                    .Commit();
                main_activity.activeR = main_activity.pos_fragment_cart;
                SetMenuUIActive(main_activity.pos_fragment_main);

                //test
                pos_fragment_cart.OnResume();
                pos_fragment_main.OnResume();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void ShowItemFragments(string subpage)
        {
            try
            {
                frameLeft.Visibility = ViewStates.Visible;
                frameRight.Visibility = ViewStates.Visible;
                main_activity.SupportFragmentManager.BeginTransaction()
                    .Hide(main_activity.activeL)
                    .Show(main_activity.item_fragment_main)
                    .Commit();
                main_activity.activeL = main_activity.item_fragment_main;
                SetMenuUIActive(main_activity.item_fragment_main);
                switch (subpage)
                {
                    case "default":
                        frameRight.Visibility = ViewStates.Invisible;
                        //item_fragment_main.OnResume();
                        break;
                    case "additem":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.item_fragment_additem).Commit();
                        main_activity.activeR = main_activity.item_fragment_additem;
                        item_fragment_additem.OnResume();
                        break;
                    case "addtopping":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.item_fragment_addtopping).Commit();
                        main_activity.activeR = main_activity.item_fragment_addtopping;
                        item_fragment_addtopping.OnResume();
                        break;
                    case "addcategory":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.item_fragment_addcategory).Commit();
                        main_activity.activeR = main_activity.item_fragment_addcategory;
                        item_fragment_addcategory.OnResume();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void ShowCustomerFragments(string subpage)
        {
            try
            {
                frameLeft.Visibility = ViewStates.Visible;
                frameRight.Visibility = ViewStates.Visible;
                main_activity.SupportFragmentManager.BeginTransaction()
                    .Hide(main_activity.activeL)
                    .Show(main_activity.customer_fragment_main)
                    .Commit();
                main_activity.activeL = main_activity.customer_fragment_main;
                SetMenuUIActive(main_activity.customer_fragment_main);
                switch (subpage)
                {
                    case "default":
                        frameRight.Visibility = ViewStates.Invisible;
                        CustomerManage customerManage = new CustomerManage();
                        var listCustomer = await customerManage.GetAllCustomer();
                        if (listCustomer.Count == 0)
                        {
                            frameRight.Visibility = ViewStates.Gone;
                        }
                        customer_fragment_main.OnResume();
                        break;
                    case "addcustomer":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.customer_fragment_addcustomer).Commit();
                        main_activity.activeR = main_activity.customer_fragment_addcustomer;
                        customer_fragment_addcustomer.OnResume();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void ShowEmployeeFragments(string subpage)
        {
            try
            {
                frameLeft.Visibility = ViewStates.Visible;
                frameRight.Visibility = ViewStates.Visible;
                main_activity.SupportFragmentManager.BeginTransaction()
                    .Hide(main_activity.activeL)
                    .Show(main_activity.employee_fragment_main)
                    .Commit();
                main_activity.activeL = main_activity.employee_fragment_main;
                SetMenuUIActive(main_activity.employee_fragment_main);
                switch (subpage)
                {
                    case "default":
                        frameRight.Visibility = ViewStates.Invisible;
                        break;
                    case "checkuser":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.employee_fragment_checkuser).Commit();
                        main_activity.activeR = main_activity.employee_fragment_checkuser;
                        employee_fragment_checkuser.OnResume();
                        break;
                    case "addemployee":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.employee_fragment_addemployee).Commit();
                        main_activity.activeR = main_activity.employee_fragment_addemployee;
                        employee_fragment_addemployee.OnResume();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void ShowDashboardFragment()
        {
            try
            {
                frameLeft.Visibility = ViewStates.Visible;
                frameRight.Visibility = ViewStates.Gone;
                main_activity.SupportFragmentManager.BeginTransaction()
                    .Hide(main_activity.activeL)
                    .Show(main_activity.dashboard_fragment_main)
                    .Commit();
                main_activity.activeL = main_activity.dashboard_fragment_main;
                SetMenuUIActive(main_activity.dashboard_fragment_main);
                dashboard_fragment_main.OnResume();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void ShowReportFragments(string subpage)
        {
            try
            {
                frameLeft.Visibility = ViewStates.Visible;
                frameRight.Visibility = ViewStates.Gone;

                var fmReport = main_activity.SupportFragmentManager.FindFragmentByTag("report");
                if (fmReport != null)
                {
                    main_activity.SupportFragmentManager.BeginTransaction()
                    .Hide(main_activity.activeL)
                    .Show(main_activity.report_fragment_main)
                    .Commit();
                    //report_fragment_main.OnResume();
                }
                main_activity.activeL = main_activity.report_fragment_main;
                SetMenuUIActive(main_activity.report_fragment_main);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void ShowShowReportFragments()
        {
            try
            {
                frameLeft.Visibility = ViewStates.Visible;
                frameRight.Visibility = ViewStates.Gone;
                main_activity.SupportFragmentManager.BeginTransaction()
                    .Hide(main_activity.activeL)
                    .Show(main_activity.report_fragment_showdata)
                    .Commit();
                main_activity.activeL = main_activity.report_fragment_showdata;
                //report_fragment_main.OnResume();
                report_fragment_showdata.OnResume();
                SetMenuUIActive(main_activity.report_fragment_showdata);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void ShowSettingFragments(string subpage)
        {
            try
            {
                frameLeft.Visibility = ViewStates.Visible;
                frameRight.Visibility = ViewStates.Visible;
                main_activity.SupportFragmentManager.BeginTransaction()
                    .Hide(main_activity.activeL)
                    .Show(main_activity.setting_fragment_main)
                    .Commit();
                main_activity.activeL = main_activity.setting_fragment_main;
                SetMenuUIActive(main_activity.setting_fragment_main);
                switch (subpage)
                {
                    case "default":
                        frameRight.Visibility = ViewStates.Invisible;
                        break;
                    case "merchant":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_merchant).Commit();
                        main_activity.activeR = main_activity.setting_fragment_merchant;
                        setting_fragment_merchant.OnResume();
                        break;
                    case "branch":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_branch).Commit();
                        main_activity.activeR = main_activity.setting_fragment_branch;
                        setting_fragment_branch.OnResume();
                        break;
                    case "addbranch":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_branchdetail).Commit();
                        main_activity.activeR = main_activity.setting_fragment_branchdetail;
                        setting_fragment_branchdetail.OnResume();
                        break;
                    case "empmanage":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_empmanage).Commit();
                        main_activity.activeR = main_activity.setting_fragment_empmanage;
                        setting_fragment_empmanage.OnResume();
                        break;
                    case "package":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_package).Commit();
                        main_activity.activeR = main_activity.setting_fragment_package;
                        setting_fragment_package.OnResume();
                        break;
                    case "note":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_note).Commit();
                        main_activity.activeR = main_activity.setting_fragment_note;
                        setting_fragment_note.OnResume();
                        break;
                    case "addnote":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_addnote).Commit();
                        main_activity.activeR = main_activity.setting_fragment_addnote;
                        setting_fragment_addnote.OnResume();
                        break;
                    case "membertype":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_membertype).Commit();
                        main_activity.activeR = main_activity.setting_fragment_membertype;
                        setting_fragment_membertype.OnResume();
                        break;
                    case "addmembertype":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_addmembertype).Commit();
                        main_activity.activeR = main_activity.setting_fragment_addmembertype;
                        setting_fragment_addmembertype.OnResume();
                        break;
                    case "vat":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_vat).Commit();
                        main_activity.activeR = main_activity.setting_fragment_vat;
                        setting_fragment_vat.OnResume();
                        break;
                    case "currency":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_currency).Commit();
                        main_activity.activeR = main_activity.setting_fragment_currency;
                        setting_fragment_currency.OnResume();
                        break;
                    case "decimal":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_decimal).Commit();
                        main_activity.activeR = main_activity.setting_fragment_decimal;
                        setting_fragment_currency.OnResume();
                        break;
                    case "decimalhelp":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_decimalhelp).Commit();
                        main_activity.activeR = main_activity.setting_fragment_decimalhelp;
                        setting_fragment_decimalhelp.OnResume();
                        break;
                    case "servicecharge":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_servicecharge).Commit();
                        main_activity.activeR = main_activity.setting_fragment_servicecharge;
                        setting_fragment_cashguild.OnResume();
                        break;
                    case "cashguild":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_cashguild).Commit();
                        main_activity.activeR = main_activity.setting_fragment_cashguild;
                        setting_fragment_cashguild.OnResume();
                        break;
                    case "addcash":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_addcash).Commit();
                        main_activity.activeR = main_activity.setting_fragment_addcash;
                        setting_fragment_addcash.OnResume();
                        break;
                    case "giftvoucher":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_giftvoucher).Commit();
                        main_activity.activeR = main_activity.setting_fragment_giftvoucher;
                        setting_fragment_giftvoucher.OnResume();
                        break;
                    case "addgiftvoucher":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_addgiftvoucher).Commit();
                        main_activity.activeR = main_activity.setting_fragment_addgiftvoucher;
                        setting_fragment_addgiftvoucher.OnResume();
                        break;
                    case "giftvouchernum":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_giftvouchernumpad).Commit();
                        main_activity.activeR = main_activity.setting_fragment_giftvouchernumpad;
                        setting_fragment_giftvouchernumpad.OnResume();
                        break;
                    case "device":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_device).Commit();
                        main_activity.activeR = main_activity.setting_fragment_device;
                        setting_fragment_device.OnResume();
                        break;
                    case "printer":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_printer).Commit();
                        main_activity.activeR = main_activity.setting_fragment_printer;
                        setting_fragment_printer.OnResume();
                        break;
                    case "myqr":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_myqr).Commit();
                        main_activity.activeR = main_activity.setting_fragment_myqr;
                        setting_fragment_myqr.OnResume();
                        break;
                    case "addmyqr":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_addmyqr).Commit();
                        main_activity.activeR = main_activity.setting_fragment_addmyqr;
                        setting_fragment_addmyqr.OnResume();
                        break;
                    case "cashdrawer":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.setting_fragment_cashdrawer).Commit();
                        main_activity.activeR = main_activity.setting_fragment_cashdrawer;
                        setting_fragment_cashdrawer.OnResume();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void ShowBillHistoryFragments(string subpage)
        {
            try
            {
                frameLeft.Visibility = ViewStates.Visible;
                frameRight.Visibility = ViewStates.Visible;
                main_activity.SupportFragmentManager.BeginTransaction()
                    .Hide(main_activity.activeL)
                    .Show(main_activity.billhistory_fragment_main)
                    .Commit();
                main_activity.activeL = main_activity.billhistory_fragment_main;
                SetMenuUIActive(main_activity.billhistory_fragment_main);
                switch (subpage)
                {
                    case "default":
                        frameRight.Visibility = ViewStates.Invisible;
                        break;
                    case "billdetail":
                        frameRight.Visibility = ViewStates.Visible;
                        main_activity.SupportFragmentManager.BeginTransaction().Hide(main_activity.activeR).Show(main_activity.billhistory_fragment_detail).Commit();
                        main_activity.activeR = main_activity.billhistory_fragment_detail;
                        billhistory_fragment_detail.OnResume();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void SetMenuUIActive(AndroidX.Fragment.App.Fragment fragment)
        {
            try
            {
                if (fragment == more_fragment_main)
                {
                    lnMore.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                    imgMore.SetImageResource(Resource.Mipmap.MenuMore);
                }
                else if (fragment == main_activity.pos_fragment_main)
                {
                    lnPos.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                    imgPOS.SetImageResource(Resource.Mipmap.MenuPOS);
                }
                else if (fragment == main_activity.item_fragment_main)
                {
                    lnItem.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                    imgItem.SetImageResource(Resource.Mipmap.MenuItem);
                }
                else if (fragment == main_activity.customer_fragment_main)
                {
                    lnCustomer.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                    imgCustomer.SetImageResource(Resource.Mipmap.MenuCustomer);
                }
                else if (fragment == main_activity.employee_fragment_main)
                {
                    lnEmployee.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                    imgEmployee.SetImageResource(Resource.Mipmap.MenuEmployee);
                }
                else if (fragment == main_activity.dashboard_fragment_main)
                {
                    lnDashboard.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                    imgDashboard.SetImageResource(Resource.Mipmap.MenuDashboard);
                }
                else if (fragment == main_activity.report_fragment_main)
                {
                    lnReport.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                    imgReport.SetImageResource(Resource.Mipmap.MenuReport);
                }
                else if (fragment == main_activity.report_fragment_showdata)
                {
                    lnReport.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                    imgReport.SetImageResource(Resource.Mipmap.MenuReport);
                }
                else if (fragment == main_activity.setting_fragment_main)
                {
                    lnSetting.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                    imgSetting.SetImageResource(Resource.Mipmap.MenuSetting);
                }
                else if (fragment == main_activity.billhistory_fragment_main)
                {
                    lnBillHistory.SetBackgroundColor(Color.ParseColor("#E8E8E8"));
                    imgBillHistory.SetImageResource(Resource.Mipmap.MenuBillHistory);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void ShowMoreFragment(AndroidX.Fragment.App.Fragment fragment)
        {
            try
            {
                frameLeft.Visibility = ViewStates.Visible;
                frameRight.Visibility = ViewStates.Gone;
                main_activity.SupportFragmentManager.BeginTransaction()
                    .Hide(main_activity.activeL)
                    .Show(fragment)
                    .Commit();
                main_activity.activeL = fragment;
                SetMenuUIActive(fragment);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void ResetMenuUI()
        {
            try
            {
                // Reset background colors
                lnMore.SetBackgroundColor(Android.Graphics.Color.White);
                lnPos.SetBackgroundColor(Android.Graphics.Color.White);
                lnItem.SetBackgroundColor(Android.Graphics.Color.White);
                lnCustomer.SetBackgroundColor(Android.Graphics.Color.White);
                lnEmployee.SetBackgroundColor(Android.Graphics.Color.White);
                lnDashboard.SetBackgroundColor(Android.Graphics.Color.White);
                lnReport.SetBackgroundColor(Android.Graphics.Color.White);
                lnSetting.SetBackgroundColor(Android.Graphics.Color.White);
                lnBillHistory.SetBackgroundColor(Android.Graphics.Color.White);

                // Reset image resources
                imgMore.SetImageResource(Resource.Mipmap.MenuMoreN);
                imgPOS.SetImageResource(Resource.Mipmap.MenuPOSN);
                imgItem.SetImageResource(Resource.Mipmap.MenuItemN);
                imgCustomer.SetImageResource(Resource.Mipmap.MenuCustomerN);
                imgEmployee.SetImageResource(Resource.Mipmap.MenuEmployeeN);
                imgDashboard.SetImageResource(Resource.Mipmap.MenuDashboardN);
                imgReport.SetImageResource(Resource.Mipmap.MenuReportN);
                imgSetting.SetImageResource(Resource.Mipmap.MenuSettingN);
                imgBillHistory.SetImageResource(Resource.Mipmap.MenuBillHistoryN);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void HandleDefaultSubpage(string subpage)
        {
            try
            {
                if (subpage == "default")
                {
                    View view = main_activity.CurrentFocus;
                    if (view != null)
                    {
                        main_activity.CloseKeyboard(view);
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public void CloseKeyboard(View view)
        {
            try
            {
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
        public void OpenKeyboard(View view)
        {
            try
            {
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(view.WindowToken, HideSoftInputFlags.ImplicitOnly);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private async Task CheckOrderBeforeClose()
        {
            try
            {
                //Check order ว่าได้เลือกก่อนปิดแอปหรือไม่
                TransManage transManage = new TransManage();
                var checkOder = await transManage.GetTranOrderBeforeClose(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                if (checkOder == null)
                {
                    if (tranWithDetails == null || tranWithDetails?.tran == null || tranWithDetails?.tran.TranType == 'O' || tranWithDetails?.tran.TranType == '\0')
                    {
                        DataCashing.SysCustomerID = 999;
                        tranWithDetails = null;
                        tranWithDetails = await Utils.initialData();
                    }
                }
                else
                {
                    if (!DataCashing.isCurrentOrder)
                    {
                        getTranOrderDetail(checkOder);
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckOrderBeforeClose at POS");
            }
        }
        TranDetailItemManage tranDetailItemManage = new TranDetailItemManage();
        TranDetailItemToppingManage toppingManage = new TranDetailItemToppingManage();
        TranPaymentManage tranPaymentManage = new TranPaymentManage();
        TranTradDiscountManage discountManage = new TranTradDiscountManage();

        async void getTranOrderDetail(Tran tran)
        {
            try
            {
                var lsttranDetailItemWithToppings = new List<TranDetailItemWithTopping>();
                TranDetailItemWithTopping detailItemWithTopping = new TranDetailItemWithTopping();

                tranWithDetails = new TranWithDetailsLocal();
                tranWithDetails.tran = tran;
                tranWithDetails.tran.TranDate = Utils.GetTranDate(tranWithDetails.tran.TranDate);
                tranWithDetails.tran.LastDateModified = Utils.GetTranDate(tranWithDetails.tran.LastDateModified);
                tranWithDetails.tran.WaitSendingTime = Utils.GetTranDate(tranWithDetails.tran.WaitSendingTime);

                //List<Detail Item >             
                var tranDetail = await tranDetailItemManage.GetTranDetailItem(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);

                foreach (var item in tranDetail)
                {
                    //Detail Item
                    TranDetailItemNew DetailItem = new TranDetailItemNew()
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
                        ItemPrice = item.ItemPrice,
                    };
                    List<TranDetailItemTopping> lstitemDetail = new List<TranDetailItemTopping>();
                    //Detail ItemTopping        
                    var tranTopping = await toppingManage.GetTranDetailItemTopping(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo, (int)item.DetailNo);
                    foreach (var itemtopping in tranTopping)
                    {
                        TranDetailItemTopping itemDetail = new TranDetailItemTopping()
                        {
                            MerchantID = itemtopping.MerchantID,
                            SysBranchID = itemtopping.SysBranchID,
                            TranNo = itemtopping.TranNo,
                            DetailNo = itemtopping.DetailNo,
                            ToppingNo = itemtopping.ToppingNo,
                            ItemName = itemtopping.ItemName,//toppping
                            SysItemID = itemtopping.SysItemID,
                            UnitName = itemtopping.UnitName,
                            RegularSizeName = itemtopping.RegularSizeName,
                            Quantity = itemtopping.Quantity,
                            ToppingPrice = itemtopping.ToppingPrice,
                            EstimateCost = itemtopping.EstimateCost,
                            Comments = itemtopping.Comments
                        };
                        lstitemDetail.Add(itemDetail);
                    }

                    detailItemWithTopping = new TranDetailItemWithTopping();
                    detailItemWithTopping.tranDetailItem = DetailItem;
                    detailItemWithTopping.tranDetailItemToppings = lstitemDetail;
                    lsttranDetailItemWithToppings.Add(detailItemWithTopping);

                    lstitemDetail = new List<TranDetailItemTopping>();
                }

                tranWithDetails.tranDetailItemWithToppings.AddRange(lsttranDetailItemWithToppings);

                //Tran Payment
                var tranPayment = await tranPaymentManage.GetTranPayment(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);
                foreach (var item in tranPayment)
                {
                    tranWithDetails.tranPayments.Add(item);
                }

                //Tran Discount
                var tranDiscount = await discountManage.GetTranTradDiscount(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);
                foreach (var itemDiscount in tranDiscount)
                {
                    tranWithDetails.tranTradDiscounts.Add(itemDiscount);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("getTranOrderDetail at POS");
            }
        }
        DateTime LastTimeCheckNet;
        public async override void OnUserInteraction()
        {
            base.OnUserInteraction();
            TimeSpan timeSpan = DateTime.Now - LastTimeCheckNet;
            if (timeSpan.TotalMinutes > 0.5)
            {
                DataCashing.CheckNet = await GabanaAPI.CheckNetWork();
                Log.Debug("CheckNet", LastTimeCheckNet + "internet" + DataCashing.CheckNet.ToString());
                LastTimeCheckNet = DateTime.Now;
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

        private async Task GetData()
        {
            try
            {
                if (DataCashing.CheckNet)
                {
                    if (DataCashingAll.Merchant?.UserAccountInfo == null)
                    {
                        DataCashingAll.Merchant = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetData at main");
            }
        }

        public static bool CheckNet = false;
        public async Task<bool> RecheckNet()
        {
            try
            {
                CheckNet = await GabanaAPI.CheckNetWork();
                return CheckNet;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("RecheckNet at main");
                return CheckNet;
            }
        }

        public static async void OpenDialogImage(Item item)
        {
            try
            {
                string path = "";

                if (!string.IsNullOrEmpty(item.PicturePath))
                {
                    if (item.PicturePath.Contains("http"))
                    {
                        path = item.PicturePath;
                    }
                    else
                    {
                        Java.IO.File imgFile = new Java.IO.File(item.PictureLocalPath);
                        if (imgFile != null)
                        {
                            path = imgFile.AbsolutePath;
                        }
                    }
                }
                Item_Dialog_ShowImage dialog = new Item_Dialog_ShowImage();
                Item_Dialog_ShowImage.SetPath(path);
                var fragment = new Item_Dialog_ShowImage();
                fragment.Show(main_activity.SupportFragmentManager, nameof(Item_Dialog_ShowImage));
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OpenDialogImage at item");
                return;
            }
        }


        public static DefaultAllData allData = new DefaultAllData();
        public async Task GetAllData()
        {
            try
            {
                #region old code
                //if (DataCashingAll.flagItemChange == false && DataCashingAll.flagCategoryChange == false)
                //{
                //    return;
                //}


                //List<Item> DefaultDataItem = new List<Item>();
                //List<Item> DefaultDataTopping = new List<Item>();
                //List<Item> DefaultDataItemonBranch = new List<Item>();
                //List<Item> DefaultAllItem = new List<Item>();
                //List<Category> DefaultDataCategory = new List<Category>();
                //List<Item> AllItemStatusD = new List<Item>();
                //List<Item> Data = new List<Item>();

                //DefaultDataItem = await GetListItem();
                //DefaultDataTopping = await GetListTopping();
                //DefaultDataCategory = await GetListCategory();
                //Data = await GetAll();

                //DefaultAllItem.AddRange(DefaultDataItem);
                //DefaultAllItem.AddRange(DefaultDataTopping);
                //DefaultDataItemonBranch = DefaultAllItem.Where(x => x.FTrackStock == 1).OrderBy(x => x.ItemName).ToList();

                //AllItemStatusD = Data.Where(x => x.DataStatus == 'D').ToList();


                //allData = new DefaultAllData()
                //{
                //    DefaultDataItem = DefaultDataItem,
                //    DefaultDataTopping = DefaultDataTopping,
                //    DefaultDataCategory = DefaultDataCategory,
                //    AllItemStatusD = AllItemStatusD,
                //    DefaultDataItemonBranch = DefaultDataItemonBranch,
                //}; 
                #endregion

                //if (!DataCashingAll.flagItemChange && !DataCashingAll.flagCategoryChange)
                //{
                //    return;
                //}

                List<Item> DefaultDataItem = new List<Item>();
                List<Item> DefaultDataTopping = new List<Item>();
                List<Item> DefaultDataItemonBranch = new List<Item>();
                List<Item> DefaultAllItem = new List<Item>();
                List<Category> DefaultDataCategory = new List<Category>();
                List<Item> AllItemStatusD = new List<Item>();
                List<Item> Data = new List<Item>();

                // รวมการเรียก API ที่เกี่ยวข้องและรอให้ทุกอย่างเสร็จสมบูรณ์ก่อนที่จะดำเนินการต่อไป
                await Task.WhenAll(
                    Task.Run(async () => DefaultDataItem = await GetListItem()),
                    Task.Run(async () => DefaultDataTopping = await GetListTopping()),
                    Task.Run(async () => DefaultDataCategory = await GetListCategory()),
                    Task.Run(async () => Data = await GetAll())
                );

                DefaultAllItem.AddRange(DefaultDataItem);
                DefaultAllItem.AddRange(DefaultDataTopping);

                // ลดการใช้งาน LINQ และเพิ่มประสิทธิภาพของการกรองข้อมูลโดยใช้วงเล็บเปิดและปิดเพื่อป้องกันการคัดลอกข้อมูลของรายการ DefaultAllItem
                DefaultDataItemonBranch = DefaultAllItem.FindAll(x => x.FTrackStock == 1);
                DefaultDataItemonBranch.Sort((x, y) => string.Compare(x.ItemName, y.ItemName));

                foreach (var item in Data)
                {
                    if (item.DataStatus == 'D')
                    {
                        AllItemStatusD.Add(item);
                    }
                }

                allData = new DefaultAllData()
                {
                    DefaultDataItem = DefaultDataItem,
                    DefaultDataTopping = DefaultDataTopping,
                    DefaultDataCategory = DefaultDataCategory,
                    AllItemStatusD = AllItemStatusD,
                    DefaultDataItemonBranch = DefaultDataItemonBranch,
                };
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetAllData at Main");
            }
        }

        private async Task<List<Category>> GetListCategory()
        {
            try
            {
                var lstCategory = await categorymanage.GetAllCategory();
                if (lstCategory == null)
                {
                    lstCategory = new List<Category>();
                }
                return lstCategory;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListCategory at Main");
                return new List<Category>();
            }
        }

        private async Task<List<Item>> GetListTopping()
        {
            try
            {
                var listTopping = await itemManage.GetToppingItem();
                if (listTopping == null)
                {
                    listTopping = new List<Item>();
                }
                return listTopping;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListTopping at Main");
                return new List<Item>();
            }
        }

        private async Task<List<Item>> GetListItem()
        {

            try
            {
                var listItem = await itemManage.GetAllItem();
                if (listItem == null)
                {
                    listItem = new List<Item>();
                }
                return listItem;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListItem at Main");
                return new List<Item>();
            }
        }

        private async Task<List<Item>> GetAll()
        {
            try
            {
                var AllItem = await itemManage.GetAll(DataCashingAll.MerchantId);
                if (AllItem == null)
                {
                    AllItem = new List<Item>();
                }
                return AllItem;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetAll at Main");
                return new List<Item>();
            }
        }

        List<SystemRevisionNo> lstsystemRevisions = new List<SystemRevisionNo>();
        List<SystemRevisionNo> listRivision = new List<SystemRevisionNo>();
        SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
        private List<ORM.MerchantDB.ItemOnBranch> itemOnBranch;
        ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
        int maxItemRevision = 0;
        ItemExSizeManage itemExSizeManage = new ItemExSizeManage();
        int maxCategoryRevision = 0;
        Category category = new Category();

        public async Task GetOnlineDataCategory()
        {
            try
            {
                ItemManage itemManage = new ItemManage();
                SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
                List<SystemRevisionNo> listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = listRivision.FirstOrDefault(x => x.SystemID == 20);
                if (revisionNo == null)
                {
                    return;
                }

                int maxCategoryRevision = 0;
                int maxCategory = 0;

                #region Category
                try
                {
                    //Get Category API
                    var allcategory = await GabanaAPI.GetDataCategory((int)revisionNo.LastRevisionNo);

                    if (allcategory == null || (allcategory.Categories.Count == 0 && allcategory.CategoryBins.Count == 0))
                    {
                        return;
                    }

                    CategoryManage CategoryManage = new CategoryManage();
                    allcategory.Categories.ToList().OrderBy(x => x.RevisionNo);
                    maxCategory = allcategory.Categories.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();

                    //check ว่ามีไหม

                    List<Category> GetallCate = await CategoryManage.GetAllCategory();
                    List<ORM.Master.Category> UpdateCategory = new List<ORM.Master.Category>();
                    List<ORM.Master.Category> InsertCategory = new List<ORM.Master.Category>();
                    UpdateCategory.AddRange(allcategory.Categories.Where(x => GetallCate.Select(y => (long)y.SysCategoryID).ToList().Contains(x.SysCategoryID)).ToList());
                    InsertCategory.AddRange(allcategory.Categories.Where(x => !(GetallCate.Select(y => (long)y.SysCategoryID).ToList().Contains(x.SysCategoryID))).ToList());

                    //Insert Category
                    if (InsertCategory.Count > 0)
                    {
                        List<Category> BulkCategory = new List<Category>();
                        foreach (var category in InsertCategory)
                        {
                            BulkCategory.Add(new Category()
                            {
                                MerchantID = category.MerchantID,
                                SysCategoryID = category.SysCategoryID,
                                Ordinary = category.Ordinary,
                                Name = category.Name,
                                DateCreated = category.DateCreated,
                                DateModified = category.DateModified,
                                DataStatus = 'I',
                                FWaitSending = 0,
                                WaitSendingTime = DateTime.UtcNow,
                                LinkProMaxxID = category.LinkProMaxxID
                            });
                            maxCategoryRevision = category.RevisionNo;
                        }

                        using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                        {
                            try
                            {
                                await db.BulkCopyAsync(BulkCategory);
                            }
                            catch (Exception ex)
                            {
                                var errorRevison = InsertCategory.Select(x => x.RevisionNo).Min();
                                maxCategory = errorRevison;
                                Log.Error("connecterror", "BulkCategory :" + ex.Message);
                                throw ex;
                            }
                        }
                    }

                    //Update Category
                    if (UpdateCategory.Count > 0)
                    {
                        foreach (var item in UpdateCategory)
                        {
                            //insertorreplace
                            category = new Category()
                            {
                                MerchantID = item.MerchantID,
                                SysCategoryID = item.SysCategoryID,
                                Ordinary = item.Ordinary,
                                Name = item.Name,
                                DateCreated = item.DateCreated,
                                DateModified = item.DateModified,
                                DataStatus = 'I',
                                FWaitSending = 0,
                                WaitSendingTime = DateTime.UtcNow,
                                LinkProMaxxID = item.LinkProMaxxID
                            };
                            var insertOrreplace = await CategoryManage.InsertOrReplaceCategory(category);
                            maxCategoryRevision = item.RevisionNo;
                        }
                    }

                    allcategory.CategoryBins.ToList().OrderBy(x => x.RevisionNo);
                    maxCategory = allcategory.CategoryBins.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();
                    foreach (var item in allcategory.CategoryBins)
                    {
                        //UpdateItem
                        List<Item> UpdateItemCate = new List<Item>();
                        UpdateItemCate = await itemManage.GetItembyCategory(item.MerchantID, item.SysCategoryID);
                        if (UpdateItemCate != null)
                        {
                            foreach (var update in UpdateItemCate)
                            {
                                update.SysCategoryID = null;
                                var resultUpdate = await itemManage.UpdateItem(update);
                            }
                        }
                        //delete
                        var delete = await CategoryManage.DeleteCategory(item.MerchantID, item.SysCategoryID);
                        if (!delete)
                        {
                            var data = await CategoryManage.GetCategory(item.MerchantID, item.SysCategoryID);
                            if (data != null)
                            {
                                data.DataStatus = 'D';
                                data.FWaitSending = 0;
                                await CategoryManage.UpdateCategory(category);
                            }
                        }
                        maxCategoryRevision = item.RevisionNo;
                    }

                    await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxCategory);
                    Log.Debug("connectpass", "listRivisionCategory");
                    DataCashingAll.flagCategoryChange = true;
                }
                catch (Exception ex)
                {
                    Log.Debug("connecterror", "listRivisionCategory : " + ex.Message);
                    await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxCategoryRevision);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetOnlineDataCategory at item");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public async Task GetOnlineDataItemonBranch()
        {
            try
            {
                int maxItemOnBranchRevision = 0;
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = new SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 31).FirstOrDefault();
                if (revisionNo != null)
                {
                    #region ItemOnBranch 
                    try
                    {
                        var allItemOnBranch = await GabanaAPI.GetDataItemOnBranchV2((int)revisionNo.LastRevisionNo, 0);

                        if (allItemOnBranch == null)
                        {
                            return;
                        }

                        if (allItemOnBranch.totalItemOnBranch == 0)
                        {
                            return;
                        }

                        if (allItemOnBranch.totalItemOnBranch > 0)
                        {
                            int round = 0, addrount = 0;
                            round = allItemOnBranch.totalItemOnBranch / 100;
                            addrount = round + 1;

                            for (int j = 0; j < addrount; j++)
                            {
                                allItemOnBranch = await GabanaAPI.GetDataItemOnBranchV2((int)revisionNo.LastRevisionNo, j);

                                if (allItemOnBranch == null)
                                {
                                    break;
                                }

                                if (allItemOnBranch.totalItemOnBranch == 0)
                                {
                                    break;
                                }

                                allItemOnBranch.ItemOnBranches.OrderBy(x => x.RevisionNo);
                                var maxItemOnBranch = allItemOnBranch.ItemOnBranches.Max(x => x.RevisionNo);

                                //check ว่ามีไหม
                                List<ORM.Master.ItemOnBranch> UpdateItemOnBranch = new List<ORM.Master.ItemOnBranch>();
                                List<ORM.Master.ItemOnBranch> InsertItemOnBranch = new List<ORM.Master.ItemOnBranch>();
                                List<ItemOnBranch> GetallItemonBranch = await onBranchManage.GetAllItemOnBranchOnlyMerchantID(DataCashingAll.MerchantId);
                                UpdateItemOnBranch.AddRange(allItemOnBranch.ItemOnBranches.Where(x => GetallItemonBranch.Select(y => (long)y.SysItemID).ToList().Contains(x.SysItemID)).ToList());
                                InsertItemOnBranch.AddRange(allItemOnBranch.ItemOnBranches.Where(x => !(GetallItemonBranch.Select(y => (long)y.SysItemID).ToList().Contains(x.SysItemID))).ToList());

                                //Insert ItemonBranch
                                if (InsertItemOnBranch.Count > 0)
                                {
                                    List<ItemOnBranch> BulkItemOnBranch = new List<ItemOnBranch>();

                                    foreach (var itemOnBranch in InsertItemOnBranch)
                                    {
                                        BulkItemOnBranch.Add(new ItemOnBranch()
                                        {
                                            MerchantID = itemOnBranch.MerchantID,
                                            SysBranchID = itemOnBranch.SysBranchID,
                                            SysItemID = itemOnBranch.SysItemID,
                                            BalanceStock = itemOnBranch.BalanceStock,
                                            MinimumStock = itemOnBranch.MinimumStock,
                                            LastDateBalanceStock = itemOnBranch.LastDateBalanceStock,
                                        });
                                    }

                                    using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                                    {
                                        try
                                        {
                                            await db.BulkCopyAsync(BulkItemOnBranch);
                                        }
                                        catch (Exception ex)
                                        {
                                            var errorRevison = InsertItemOnBranch.Select(x => x.RevisionNo).Min();
                                            maxItemOnBranchRevision = errorRevison;
                                            Log.Error("connecterror", "BulkItemOnBranch :" + ex.Message);
                                            throw ex;
                                        }
                                    }
                                }

                                //Update ItemonBranch
                                if (UpdateItemOnBranch.Count > 0)
                                {
                                    foreach (var item in UpdateItemOnBranch)
                                    {
                                        ItemOnBranch stock = new ItemOnBranch()
                                        {
                                            MerchantID = item.MerchantID,
                                            SysBranchID = item.SysBranchID,
                                            SysItemID = item.SysItemID,
                                            BalanceStock = item.BalanceStock,
                                            MinimumStock = item.MinimumStock,
                                            LastDateBalanceStock = item.LastDateBalanceStock,
                                        };
                                        var insertStock = await onBranchManage.InsertorReplaceItemOnBranch(stock);
                                        maxItemOnBranchRevision = item.RevisionNo;
                                    }
                                }

                                await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxItemOnBranch);
                            }
                            Log.Debug("connectpass", "listRivisionItemOnBranch");
                            //DataCashingAll.flagItemOnBranchChange = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("connecterror", "listRivisionItemOnBranch : " + ex.Message);
                        await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxItemOnBranchRevision);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetOnlineDataTemonBranch at item");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public async Task GetOnlineDataitem()
        {
            try
            {
                ItemManage itemManage = new ItemManage();
                SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
                List<SystemRevisionNo> listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = listRivision.FirstOrDefault(x => x.SystemID == 30);
                if (revisionNo == null)
                {
                    return;
                }

                int maxItemRevision = 0;

                #region Item                   
                List<Item> GetAllitem = new List<Item>();
                List<Gabana3.JAM.Items.ItemWithItemExSizes> UpdateItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                List<Gabana3.JAM.Items.ItemWithItemExSizes> InsertItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                try
                {
                    var allItem = await GabanaAPI.GetDataItem((int)revisionNo.LastRevisionNo, 0);

                    if (allItem == null || (allItem?.ItemsWithItemExSizes.Count == 0))
                    {
                        return;
                    }

                    int round = 0, addrount = 0;
                    round = allItem.totalItems / 100;
                    addrount = round + 1;
                    double increaseProgress = 0;
                    increaseProgress = 25 / addrount;

                    for (int j = 0; j < addrount; j++)
                    {
                        allItem = await GabanaAPI.GetDataItem((int)revisionNo.LastRevisionNo, j);

                        if (allItem == null || (allItem.totalItems == 0))
                        {
                            break;
                        }

                        allItem.ItemsWithItemExSizes.ToList().OrderBy(x => x.ItemStatus.item.RevisionNo);
                        var maxItem = allItem.ItemsWithItemExSizes.ToList().Max(x => x.ItemStatus.item.RevisionNo);// OrderByDescending(x => x.item.RevisionNo).First();                            

                        GetAllitem = await itemManage.GetAllItemType();
                        UpdateItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                        InsertItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();

                        UpdateItem.AddRange(allItem.ItemsWithItemExSizes.Where(x => GetAllitem.Select(y => (long)y.SysItemID).ToList().Contains(x.ItemStatus.item.SysItemID)).ToList());
                        InsertItem.AddRange(allItem.ItemsWithItemExSizes.Where(x => !(GetAllitem.Select(y => (long)y.SysItemID).ToList().Contains(x.ItemStatus.item.SysItemID)) && x.ItemStatus.DataStatus != 'D').ToList());

                        List<Item> Bulkitem = new List<Item>();
                        List<ItemExSize> BulkitemExsize = new List<ItemExSize>();

                        //Insert Item
                        if (InsertItem.Count > 0)
                        {
                            foreach (var item in InsertItem)
                            {
                                string thumnailPath = string.Empty;
                                try
                                {
                                    if (!string.IsNullOrEmpty(item.ItemStatus.item.PicturePath))
                                    {
                                        string pathImage = await Utils.InsertLocalPictureItemMaster(item.ItemStatus.item.PicturePath);
                                        thumnailPath = pathImage ?? "";
                                    }
                                    else
                                    {
                                        thumnailPath = "";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //Update RevisionNo ที่ผิดพลาด เพื่อเรียกข้อมูลใหม่
                                    var errorRevison = InsertItem.Select(x => x.ItemStatus.item.RevisionNo).Min();
                                    maxItemRevision = (errorRevison == 0) ? 0 : errorRevison + 1;
                                    Log.Error("connecterror", "Bulkitem - Image : " + ex.Message);
                                    thumnailPath = "";
                                }

                                Bulkitem.Add(new Item()
                                {
                                    MerchantID = item.ItemStatus.item.MerchantID,
                                    SysItemID = item.ItemStatus.item.SysItemID,
                                    ItemName = item.ItemStatus.item.ItemName,
                                    Ordinary = item.ItemStatus.item.Ordinary,
                                    SysCategoryID = item.ItemStatus.item.SysCategoryID,
                                    ItemCode = item.ItemStatus.item.ItemCode,
                                    ShortName = item.ItemStatus.item.ShortName,
                                    PicturePath = item.ItemStatus.item.PicturePath,
                                    ThumbnailPath = item.ItemStatus.item.ThumbnailPath,
                                    PictureLocalPath = "",
                                    ThumbnailLocalPath = thumnailPath,
                                    Colors = item.ItemStatus.item.Colors,
                                    FavoriteNo = item.ItemStatus.item.FavoriteNo,
                                    UnitName = item.ItemStatus.item.UnitName,
                                    RegularSizeName = item.ItemStatus.item.RegularSizeName,
                                    EstimateCost = item.ItemStatus.item.EstimateCost,
                                    Price = item.ItemStatus.item.Price,
                                    OptSalePrice = item.ItemStatus.item.OptSalePrice,
                                    TaxType = item.ItemStatus.item.TaxType,
                                    SellBy = item.ItemStatus.item.SellBy,
                                    FTrackStock = item.ItemStatus.item.FTrackStock,
                                    TrackStockDateTime = item.ItemStatus.item.TrackStockDateTime,
                                    SaleItemType = item.ItemStatus.item.SaleItemType,
                                    Comments = item.ItemStatus.item.Comments,
                                    LastDateModified = item.ItemStatus.item.LastDateModified,
                                    UserLastModified = item.ItemStatus.item.UserLastModified,
                                    DataStatus = item.ItemStatus.DataStatus,
                                    FWaitSending = 0,
                                    WaitSendingTime = DateTime.UtcNow,
                                    LinkProMaxxItemID = item.ItemStatus.item.LinkProMaxxItemID,
                                    LinkProMaxxItemUnit = item.ItemStatus.item.LinkProMaxxItemUnit,
                                    FDisplayOption = item.ItemStatus.item.FDisplayOption
                                });

                                var itemSizes = allItem.ItemsWithItemExSizes.Where(x => x.ItemStatus.item.SysItemID == item.ItemStatus.item.SysItemID).Select(x => x.itemExSizes).FirstOrDefault() ?? new List<ORM.Master.ItemExSize>();

                                itemSizes.ForEach(itemSize => BulkitemExsize.Add(new ItemExSize()
                                {
                                    MerchantID = itemSize.MerchantID,
                                    SysItemID = itemSize.SysItemID,
                                    EstimateCost = itemSize.EstimateCost,
                                    ExSizeName = itemSize.ExSizeName,
                                    ExSizeNo = itemSize.ExSizeNo,
                                    Price = itemSize.Price,
                                    Comments = itemSize.Comments
                                }));
                            }

                            using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                            {
                                await db.BeginTransactionAsync();
                                try
                                {
                                    await db.BulkCopyAsync(Bulkitem);
                                    await db.BulkCopyAsync(BulkitemExsize);
                                    await db.CommitTransactionAsync();
                                }
                                catch (Exception ex)
                                {
                                    await db.RollbackTransactionAsync();
                                    //Update RevisionNo ที่ผิดพลาด เพื่อเรียกข้อมูลใหม่
                                    var errorRevison = InsertItem.Select(x => x.ItemStatus.item.RevisionNo).Min();
                                    maxItemRevision = errorRevison;
                                    Utils.DeletePictureItemMaster(Bulkitem.Select(x => x.ThumbnailLocalPath).ToList());
                                    Log.Error("connecterror", "Bulkitem,BulkitemExsize : " + ex.Message);
                                    throw ex;
                                }
                            }
                        }

                        //Update Item
                        if (UpdateItem.Count > 0)
                        {
                            UpdateItem.ForEach(async item =>
                            {
                                char itemStatus = item.ItemStatus.DataStatus;
                                List<ORM.Master.ItemOnBranch> itemOnBranch = allItem.ItemsWithItemExSizes.Where(x => x.ItemStatus.item.SysItemID == item.ItemStatus.item.SysItemID).Select(x => x.itemOnBranchs).FirstOrDefault() ?? new List<ORM.Master.ItemOnBranch>();
                                var data = await itemManage.GetItem((int)item.ItemStatus.item.MerchantID, (int)item.ItemStatus.item.SysItemID);

                                if (itemStatus == 'D')
                                {
                                    await Utils.DeleteItem(data, itemOnBranch);
                                }
                                else
                                {
                                    string thumnailLocalPath = string.Empty;
                                    if (!string.IsNullOrEmpty(item.ItemStatus.item.PicturePath))
                                    {
                                        if (item.ItemStatus.item.PicturePath != data.PicturePath)
                                        {
                                            //delete รูป
                                            if (!string.IsNullOrEmpty(data?.ThumbnailLocalPath))
                                            {
                                                Java.IO.File imgTempFile = new Java.IO.File(data?.ThumbnailLocalPath);

                                                if (System.IO.File.Exists(imgTempFile.AbsolutePath))
                                                {
                                                    System.IO.File.Delete(imgTempFile.AbsolutePath);
                                                }
                                            }

                                            string pathImage = await Utils.InsertLocalPictureItemMaster(item.ItemStatus.item.PicturePath);
                                            thumnailLocalPath = pathImage ?? "";
                                        }
                                        else
                                        {
                                            thumnailLocalPath = data?.ThumbnailLocalPath;
                                        }
                                    }

                                    Item updateItem = new Item()
                                    {
                                        MerchantID = item.ItemStatus.item.MerchantID,
                                        SysItemID = item.ItemStatus.item.SysItemID,
                                        ItemName = item.ItemStatus.item.ItemName,
                                        Ordinary = item.ItemStatus.item.Ordinary,
                                        SysCategoryID = item.ItemStatus.item.SysCategoryID,
                                        ItemCode = item.ItemStatus.item.ItemCode,
                                        ShortName = item.ItemStatus.item.ShortName,
                                        PicturePath = item.ItemStatus.item.PicturePath,
                                        ThumbnailPath = item.ItemStatus.item.ThumbnailPath,
                                        PictureLocalPath = "",
                                        ThumbnailLocalPath = thumnailLocalPath,
                                        Colors = item.ItemStatus.item.Colors,
                                        FavoriteNo = item.ItemStatus.item.FavoriteNo,
                                        UnitName = item.ItemStatus.item.UnitName,
                                        RegularSizeName = item.ItemStatus.item.RegularSizeName,
                                        EstimateCost = item.ItemStatus.item.EstimateCost,
                                        Price = item.ItemStatus.item.Price,
                                        OptSalePrice = item.ItemStatus.item.OptSalePrice,
                                        TaxType = item.ItemStatus.item.TaxType,
                                        SellBy = item.ItemStatus.item.SellBy,
                                        FTrackStock = item.ItemStatus.item.FTrackStock,
                                        TrackStockDateTime = item.ItemStatus.item.TrackStockDateTime,
                                        SaleItemType = item.ItemStatus.item.SaleItemType,
                                        Comments = item.ItemStatus.item.Comments,
                                        LastDateModified = item.ItemStatus.item.LastDateModified,
                                        UserLastModified = item.ItemStatus.item.UserLastModified,
                                        DataStatus = item.ItemStatus.DataStatus,
                                        FWaitSending = 0,
                                        WaitSendingTime = DateTime.UtcNow,
                                        LinkProMaxxItemID = item.ItemStatus.item.LinkProMaxxItemID,
                                        LinkProMaxxItemUnit = item.ItemStatus.item.LinkProMaxxItemUnit,
                                        FDisplayOption = item.ItemStatus.item.FDisplayOption
                                    };

                                    var insertOrreplace = await itemManage.UpdateItem(updateItem);

                                    var itemSizes = allItem.ItemsWithItemExSizes.Where(x => x.ItemStatus.item.SysItemID == item.ItemStatus.item.SysItemID).Select(x => x.itemExSizes).FirstOrDefault() ?? new List<ORM.Master.ItemExSize>();

                                    ItemExSizeManage itemExSizeManage = new ItemExSizeManage();
                                    itemSizes.ForEach(async itemSize =>
                                    {
                                        await itemExSizeManage.InsertOrReplaceItemSize(new ItemExSize()
                                        {
                                            MerchantID = itemSize.MerchantID,
                                            SysItemID = itemSize.SysItemID,
                                            EstimateCost = itemSize.EstimateCost,
                                            ExSizeName = itemSize.ExSizeName,
                                            ExSizeNo = itemSize.ExSizeNo,
                                            Price = itemSize.Price,
                                            Comments = itemSize.Comments
                                        });
                                    });
                                }
                                maxItemRevision = item.ItemStatus.item.RevisionNo;
                            });
                        }

                        await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxItem);
                    }
                    Log.Debug("connectpass", "listRivisionItem");
                    DataCashingAll.flagItemChange = true;
                }
                catch (Exception ex)
                {
                    Log.Debug("connecterror", "listRivisionItem : " + ex.Message);
                    await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxItemRevision);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetOnlineDataitem at ite m");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

    }
}

