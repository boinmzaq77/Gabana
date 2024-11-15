using Acr.Logging;
using CoreFoundation;
using Foundation;
using Gabana.ios;
using Gabana.ORM.MerchantDB;
using Gabana.ORM.PoolDB;
using Gabana.POS;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Items;
using Gabana3.JAM.Merchant;
using LinqToDB.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using UserNotifications;
using Xamarin.Essentials;
using DataBaseInfo = Gabana.ORM.MerchantDB.DataBaseInfo;

namespace Gabana.iOS
{
   
    public class SplashLoadingController : UIViewController
    {
        UILabel lblGiftory;
        UIImageView GiftImg, GifttxtImg, Reloadimg, seniorsoftimg;
        UIProgressView loading;
        SystemInfoManage SystemInfoManage;
        SystemRevisionNoManage systemRevisionNoManage;
        SystemRevisionNo SystemRevisionNo;
        DeviceSystemSeqNoManage systemSeqNoManage;
        DeviceSystemSeqNo deviceSystemSeq;
        List<SystemRevisionNo> listRivision;
        List<SystemInfo> lstSystemInfo;
        CategoryManage categoryManage;
        Category category;
        DeviceTranRunningNoManage deviceTranRunningNoManage;
        DeviceTranRunningNo deviceTranRunning;
        CustomerManage customerManage = new CustomerManage();
        Customer customer;
        ItemManage itemManage;
        ItemExSizeManage itemExSizeManage;
        UIWindow uIWindowRoot;
        DiscountTemplateManage discountTemplateManage;
        DiscountTemplate discount = new DiscountTemplate();
        MemberTypeManage membertypeManage;
        //CustomerManage customerManage = new CustomerManage();
        private int counterror = 0;
        MerchantConfigManage merchantconfigManage;
        public static Gabana3.JAM.Merchant.Merchants merchants = new Gabana3.JAM.Merchant.Merchants();
        public static List<Province> provinces;
        public static List<Amphure> amphure;
        public static List<District> district;
        public static List<Geography> geography;
        PoolManage poolManager = new PoolManage();
        Task[] tasksitem = new Task[2];
        Task[] taskscus = new Task[2];
        ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
        String TAG = "Splash";


        public SplashLoadingController()
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            // 
        }
        public override void ViewDidLoad()
        {
            
            base.ViewDidLoad();
            try
            {

            
                View.BackgroundColor = UIColor.White; 
                CreateView();
                setupView();
                string x = "6.00%";
                var y = x.Remove(4);
                CultureInfo ci = new CultureInfo("th-TH");
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
            }
            // Perform any additional setup after loading the view
        }
        bool isFirstTime = true;
        private UILabel lblversion;
        private List<Item> GetAllitem;
        private List<ItemWithItemExSizes> UpdateItem;
        private List<ItemWithItemExSizes> InsertItem;

        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            try
            {

            
                //progressview.Progress = 0;
                DataCashingAll.DeviceUDID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
                DataCashingAll.DevicePlatform = "APNS";

                // ถ้าเป็น FirstTime คือครั้งแรกที่ เปิด App เข้ามาถึงจะ check UNUserNotificationCenter
                // ส่วนครั้งต่อไปที่เข้าที่หน้านี้ก็จะไปเรียก Reconnect ได้เลย
                if (isFirstTime)
                {
                    isFirstTime = false;

                    UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                        (granted, error) =>
                        {
                            if (granted)
                            {
                                InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                            
                            }

                            // ต้องเอามาไว้ตอนที่ User ว่าจะ Allow หรือ Not Allow เสร็จเรียบร้อย
                            // ถึงจะเรียก Reconnect() ได้ เพราะใน Reconnect() มีการเรียกใช้งาน await httpclient
                            // ถ้าไม่รอให้ user ตอบให้จบก่อนมันจะเกิด System.Threading.Tasks.TaskCanceledException
                            InvokeOnMainThread(async ()  =>
                            {
                                if (!await GabanaAPI.CheckNetWork())
                                {
                                    Reconnectoffine();
                                    DataCashingAll.CheckConnectInternet = false;
                                }
                                else
                                {
                                    Reconnect();
                                    DataCashingAll.CheckConnectInternet = true;
                                }
                               // Reconnect();
                            });
                        });
                }
                else
                {

                    if (!await GabanaAPI.CheckNetWork())
                    {
                        Reconnectoffine();
                    }
                    else
                    {
                        Reconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        private async void Reconnectoffine()
        {
            DataCaching.LoginNavigation = new LoginNavigationController();
            try
            {
                var Jwt = Preferences.Get("gbnJWT", "");
                string Username = Preferences.Get("User", "");
                GabanaAPI.gbnJWT = Jwt;

                LocalDBTransaction localDB = new LocalDBTransaction();
                DataCashingAll.MerchantId = Preferences.Get("MerchantID", 0);
                if (DataCashingAll.MerchantId==0)
                {
                    DataCaching.LoginNavigation.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewControllerAsync(DataCaching.LoginNavigation, true);
                    return;
                }
                
                localDB.CreateLocalBase(DataCashingAll.MerchantId);
                localDB.CreatePoolDB(DataCashingAll.MerchantId);
                //Preferences.Set("MerchantID", (int)merchants.Merchant.MerchantID);
                

                //Local Data
                DataCashingAll.Pathdb = Preferences.Get("PathMerchantDB", "");
                DataCashingAll.Pathdbpool = Preferences.Get("PathPoolDB", "");

                loading.Progress += 0.1f;

                //Merchant
                MerchantManage merchantManage = new MerchantManage();
                var merchantid = DataCashingAll.MerchantId;
                DataCashingAll.MerchantId = merchantid;
                var GETmerchantlocal = await merchantManage.GetMerchant(merchantid);
                DataCashingAll.MerchantLocal = GETmerchantlocal;

                loading.Progress += 0.2f;

                //Merchant Config                
                if (DataCashingAll.setmerchantConfig == null)
                {
                    var setmerchantConfig = Preferences.Get("SetmerchantConfig", "");
                    var Config = JsonConvert.DeserializeObject<SetMerchantConfig>(setmerchantConfig);
                    DataCashingAll.setmerchantConfig = Config;
                }

                loading.Progress += 0.2f;
                if (DataCashingAll.GetGabanaInfo == null)
                {
                    var GabanaInfo = Preferences.Get("GabanaInfo", "");
                    if (GabanaInfo != "")
                    {
                        var info = JsonConvert.DeserializeObject<Gabana.Model.GabanaInfo>(GabanaInfo);
                        DataCashingAll.GetGabanaInfo = info;
                    }
                }
                //Useraccount                
                if (DataCashingAll.UserAccountInfo == null)
                {
                    var Employee = Preferences.Get("Employeeoffline", "");
                    var lstEmployee = JsonConvert.DeserializeObject<List<Model.UserAccountInfo>>(Employee);
                    DataCashingAll.UserAccountInfo = lstEmployee;
                }

                loading.Progress += 0.1f;

                //Device                
                if (DataCashingAll.Device == null)
                {
                    var setDevice = Preferences.Get("Device", "");
                    var Config = JsonConvert.DeserializeObject<Device>(setDevice);
                    DataCashingAll.Device = Config;
                    DataCashingAll.DeviceNo = Config.DeviceNo;
                }

                if (string.IsNullOrEmpty(DataCashingAll.setting?.TYPE))
                {
                    var settingPrinter = Preferences.Get("Setting", "");
                    var Config = JsonConvert.DeserializeObject<SettingPrinter>(settingPrinter);
                    if (Config != null)
                    {
                        DataCashingAll.setting = Config;
                    }
                    else
                    {
                        DataCashingAll.setting = new SettingPrinter()
                        {
                            TYPE = "Wifi",
                            PORTNUMBER = "9100",
                            IPADDRESS = "192.168.200.182",
                            USE = "Wifi",
                            TYPEPAGE = "58mm",
                            TYPESPEED = "Image"
                        };
                    }
                }
                if (string.IsNullOrEmpty(DataCashingAll.PrintType))
                {
                    var printType = Preferences.Get("PrintType", "");
                    if (string.IsNullOrEmpty(printType))
                    {
                        DataCashingAll.PrintType = "Image";
                        Preferences.Set("PrintType", "Image");
                    }
                    else
                    {
                        DataCashingAll.PrintType = printType;
                    }
                }

                loading.Progress += 0.1f;

                //Branch
                //string branchID = Preferences.Get("Branch", "");
                //DataCashingAll.SysBranchId = Convert.ToInt32(branchID);
                //BranchManage branchManage = new BranchManage();
                //var branchData = await branchManage.GetBranch(merchantid, DataCashingAll.SysBranchId);
                //if (branchData != null)
                //{
                //    DataCaching.branchDeatail = branchData;
                //}
                //loading.Progress += 0.3f;

                if (Preferences.Get("AppState", "") == "logout")
                {
                    DataCaching.LoginNavigation.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewControllerAsync(DataCaching.LoginNavigation, true);
                }
                else
                {
                    //ใช้ข้อมูลจาก localbase  
                    MainController.merchant = merchants;
                    string branch = Preferences.Get("Branch", "");
                    if (string.IsNullOrEmpty(branch))
                    {
                        DataCaching.LoginNavigation.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewControllerAsync(DataCaching.LoginNavigation, true);
                        return;
                    }
                    BranchManage branchManage = new BranchManage();
                    var branchData = await branchManage.GetBranch(merchantid, DataCashingAll.SysBranchId);
                    if (branchData != null)
                    {
                        DataCaching.branchDeatail = branchData;
                    }
                    DataCashingAll.SysBranchId = Int32.Parse(branch);
                    DataCaching.MainNavigation = new MainNavigationController("main", merchants);
                    uIWindowRoot = UIApplication.SharedApplication.Windows.First();
                    uIWindowRoot.RootViewController = DataCaching.MainNavigation;
                    uIWindowRoot.MakeKeyAndVisible();
                    return;
                    //Toast.MakeText(this, "ขณะ Offline จะไม่สามารถบางฟังก์ชันได้ ", ToastLength.Short).Show();
                    //StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                    //this.Finish();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Connectoffine");
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }


        }
        private async void Reconnect()
        {
            try
            {
                
                Log.Info(TAG, "1 reconnect:" + DateTime.Now);
                //Console.WriteLine("1 reconnect:"+DateTime.Now);
                 DataCaching.LoginNavigation = new LoginNavigationController();
                //provinces = null;
                //var x1 = provinces.Count();
                //progressview.Progress = (float)0.2;
               
                await ChecKVersionApp();

                var res = await TokenServiceBase.GetToken();
                if (res.status)
                {
                    var listmerchantConfig = await GabanaAPI.GetDataMerchantConfig();
                    Log.Info(TAG, "2 gettoken:" + DateTime.Now);
                    
                    GabanaAPI.gbnJWT = res.gbnJWT;
                    string Id = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
                    merchants = await GabanaAPI.GetMerchantDetail("APNS", Id);
                    // update merchant 
                    if (merchants == null)//.Status)
                    {
                        UpdateNavigationController update = new UpdateNavigationController();
                        update.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewControllerAsync(update, true);
                        return;
                    }

                    var jsonmerchants = JsonConvert.SerializeObject(merchants);
                    Preferences.Set("Merchant", jsonmerchants);
                    DataCashingAll.Merchant = merchants;
                    DataCashingAll.DeviceNo = merchants.Device.DeviceNo;
                    _ = TinyInsights.TrackEventAsync(merchants.Merchant.MerchantID.ToString());
                    _ = TinyInsights.TrackEventAsync(merchants.Device.DeviceNo.ToString());

                    if (merchants.Merchant.FPrivacyPolicy != 'Y' || merchants.Merchant.FTermConditions != 'Y')
                    {
                        MainNavigationController mainNavigation = new MainNavigationController("term", merchants);
                        mainNavigation.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewControllerAsync(mainNavigation, true);
                        return;
                    }
                    
                    var perSetting = Preferences.Get("Setting", "");
                    if (perSetting != "")
                    {
                        var settingPrinter = JsonConvert.DeserializeObject<SettingPrinter>(perSetting);
                        DataCashingAll.setting = settingPrinter;
                    }
                    else
                    {
                        DataCashingAll.setting = new SettingPrinter()
                        {
                            TYPE = "Wifi",
                            PORTNUMBER = "9100",
                            IPADDRESS = "192.168.200.182",
                            USE = "Wifi",
                            TYPEPAGE = "58mm",
                            TYPESPEED = "Image"
                        };
                    }
                    if (string.IsNullOrEmpty(DataCashingAll.PrintType))
                    {
                        var printType = Preferences.Get("PrintType", "");
                        if (string.IsNullOrEmpty(printType))
                        {
                            DataCashingAll.PrintType = "Image";
                            Preferences.Set("PrintType", "Image");
                        }
                        else
                        {
                            DataCashingAll.PrintType = printType;
                        }
                    }
                    //--------------------------------------
                    //Notification
                    //--------------------------------------
                    var tokenbell = Preferences.Get("NotificationDeviceToken","");
                    if (!string.IsNullOrEmpty(tokenbell))
                    {
                        BellNotificationHelper.RegisterBellNotification(GabanaAPI.gbnJWT, merchants.Merchant.MerchantID, merchants.Device.DeviceNo);
                    }

                    DataCashingAll.MerchantId = merchants.Merchant.MerchantID;

                    CreateNewMerchant newMerchant = new CreateNewMerchant();
                    var create = Preferences.Get("CreateDB", "");
                    if (create != "")
                    {
                        newMerchant = JsonConvert.DeserializeObject<CreateNewMerchant>(create);
                    }
                    if (newMerchant.createNew || string.IsNullOrEmpty(create))
                    {
                        //create Folder Merchant.DB
                        LocalDBTransaction localDB = new LocalDBTransaction();

                        localDB.CreateLocalBase(merchants.Merchant.MerchantID);
                        localDB.CreatePoolDB(merchants.Merchant.MerchantID);
                        Preferences.Set("MerchantID", (int)merchants.Merchant.MerchantID) ;
                        DataCashingAll.MerchantId = Preferences.Get("MerchantID", 0);

                        CreateNewMerchant createNewMerchant = new CreateNewMerchant()
                        {
                            createNew = false,
                            MerchantID = DataCashingAll.MerchantId,
                        };
                        var createNewDB = JsonConvert.SerializeObject(createNewMerchant);
                        Preferences.Set("CreateDB", createNewDB);
                    }
                    Console.WriteLine("time createbase" + DateTime.Now);
                    DataBaseInfoManage dataBaseInfoManage = new DataBaseInfoManage();
                    var datainfo = await dataBaseInfoManage.GetDatabaseInfo();
                    if (datainfo == null)
                    {
                        //create Folder Merchant.DB
                        LocalDBTransaction localDB = new LocalDBTransaction();

                        localDB.CreateLocalBase(merchants.Merchant.MerchantID);
                        localDB.CreatePoolDB(merchants.Merchant.MerchantID);
                        Preferences.Set("MerchantID", (int)merchants.Merchant.MerchantID);
                        DataCashingAll.MerchantId = Preferences.Get("MerchantID", 0);
                        CreateNewMerchant createNewMerchant = new CreateNewMerchant()
                        {
                            createNew = false,
                            MerchantID = DataCashingAll.MerchantId,
                        };
                        var createNewDB = JsonConvert.SerializeObject(createNewMerchant);
                        Preferences.Set("CreateDB", createNewDB);
                    }
                    var DataVersion = await GabanaAPI.GetDataVersion(datainfo?.DataDBInfo);
                    if (DataVersion != null)
                    {
                        //foreach (var item in DataVersion)
                        //{
                        //    DataBaseInfo dataBaseInfo = new DataBaseInfo()
                        //    {
                        //        KeyDBInfo = "GabanaErVersion",
                        //        DataDBInfo = item.ErVersionDBInfo
                        //    };
                            
                        //}
                        await dataBaseInfoManage.AlterDatabaseInfo(datainfo, DataVersion);
                    }
                    Console.WriteLine("time createbaseend" + DateTime.Now);

                    PoolManage pool = new PoolManage();
                    var province = await pool.GetProvinces();
                    var procount  = province.Count;
                    var result = false;
                    string pathClound = merchants.Merchant.LogoPath;
                    string imagePath = Utils.SplitCloundPath(pathClound);
                    Merchant merchant = new Merchant()
                    {
                        MerchantID = merchants.Merchant.MerchantID,
                        Name = merchants.Merchant.Name,
                        FMasterMerchant = merchants.Merchant.FMasterMerchant, // ถ้า FMasterMerchant = 1 ค่าของ RefMasterMerchantID จะเป็นเลขเดียวกับ MerchantID 
                        RefMasterMerchantID = merchants.Merchant.MerchantID,//รอแก้ไขเรื่อง ถ้า FMasterMerchant = 0 ค่าของ RefMasterMerchantID จะเป็นค่าของ MerchantID ที่เป็น Master ที่ Merchant นี้เป็น Franchise อยู่
                        Status = merchants.Merchant.Status,
                        DateOpenMerchant = merchants.Merchant.DateOpenMerchant,
                        RefPackageID = merchants.Merchant.RefPackageID, //Reference ID ของ Package ไปยังระบบ MerchantLicence
                        DayOfPeriod = merchants.Merchant.DayOfPeriod,//วันที่ของรอบการคิดเงิน ของ Package เช่นทุกวันที่ 10 ของทุกเดือน
                        DueDate = merchants.Merchant.DueDate,
                        LanguageCountryCode = merchants.Merchant.LanguageCountryCode,//Default 'th-TH'
                        TimeZoneName = merchants.Merchant.TimeZoneName,
                        TimeZoneUTCOffset = merchants.Merchant.TimeZoneUTCOffset,
                        LogoPath = merchants.Merchant.LogoPath,
                        DateCreated = merchants.Merchant.DateCreated,
                        DateModified = merchants.Merchant.DateModified,
                        UserNameModified = merchants.Merchant.UserNameModified,
                        DateCloseMerchant = merchants.Merchant.DateCloseMerchant,
                        FPrivacyPolicy = merchants.Merchant.FPrivacyPolicy,
                        FTermConditions = merchants.Merchant.FTermConditions,
                        
                        RegMark = merchants.Merchant.RegMark,
                        TaxID = merchants.Merchant.TaxID
                    };
                    Log.Info(TAG, "3 create bass:" + DateTime.Now);
                    Console.WriteLine("time merchantestart" + DateTime.Now);

                    ShareSource.Manage.MerchantManage merchantManage = new ShareSource.Manage.MerchantManage();
                    var merchantlocal = await merchantManage.GetMerchant(merchants.Merchant.MerchantID);

                    if (merchantlocal == null)
                    {
                        result = await merchantManage.InsertMerchant(merchant);

                    }
                    else
                    {
                        merchant.LogoLocalPath = merchantlocal.LogoLocalPath; 
                        result = await merchantManage.UpdateMerchant(merchant);
                    }
                    
                    var GETmerchantlocal = await merchantManage.GetMerchant(merchants.Merchant.MerchantID);
                    if (Utils.SplitPath(GETmerchantlocal.LogoLocalPath) != imagePath)
                    {
                        GETmerchantlocal.LogoPath = merchants.Merchant.LogoPath;
                        await merchantManage.UpdateMerchant(GETmerchantlocal);
                        await Utils.InsertLocalPictureMerchant(GETmerchantlocal);
                    }
                    DataCashingAll.MerchantLocal = GETmerchantlocal;
                    Preferences.Set("MerchantID", (int)GETmerchantlocal.MerchantID);
                    DataCashingAll.MerchantId = Preferences.Get("MerchantID", 0);

                    Console.WriteLine("time merchanteend" + DateTime.Now);
                    Console.WriteLine("time branchstart" + DateTime.Now);
                    await InsertBranchtoLocal();
                    Console.WriteLine("time branchend" + DateTime.Now);
                    if (newMerchant.createNew)
                    {
                        Log.Info(TAG, "4 create new:" + DateTime.Now);
                        
                        //insert list Branch 
                        //await InsertBranchtoLocal();
                        loading.Progress += 0.1f;//progressBar 25

                        //InsertorReplace MerchantConfig
                        await GetmerchantConfig();
                        loading.Progress += 0.1f;//progressBar 30
                        Console.WriteLine("time marchantend" + DateTime.Now);

                        //GetDevice
                        await GetDeviceData();
                        loading.Progress += 0.1f;
                        Console.WriteLine("time deviceend" + DateTime.Now);
                        //Insert UserAccount
                        await InsertUserAccount();
                        loading.Progress += 0.1f;//progressBar 100
                        Console.WriteLine("time useraccountend" + DateTime.Now);

                        //owner                            
                        if (merchants.Branch.Count == 1)
                        {
                            //owner
                            string BranchID = merchants.Branch[0].SysBranchID.ToString();
                            Preferences.Set("Branch", BranchID);
                            DataCashingAll.SysBranchId = (int)merchants.Branch[0].SysBranchID;
                            await SetDataDeviceTranRunningNo();
                            MainController.merchant = merchants;
                            DataCaching.MainNavigation = new MainNavigationController("main", merchants);
                            uIWindowRoot = UIApplication.SharedApplication.Windows.First();
                            uIWindowRoot.RootViewController = DataCaching.MainNavigation;
                            uIWindowRoot.MakeKeyAndVisible();
                            return;
                        }
                    }
                    BranchManage branchManage2 = new BranchManage();

                    string branch = Preferences.Get("Branch", "");
                    if (string.IsNullOrEmpty(branch))
                    {
                        
                        //await InsertBranchtoLocal();
                        Log.Info(TAG, "time InsertBranchtoLocal" + DateTime.Now);
                        Console.WriteLine("InsertBranchtoLocal:" + DateTime.Now);
                        loading.Progress += 0.1f;//progressBar 25

                        //InsertorReplace MerchantConfig
                        await GetmerchantConfig();
                        Log.Info(TAG, "time GetmerchantConfig" + DateTime.Now);
                        Console.WriteLine("GetmerchantConfig:" + DateTime.Now);
                        loading.Progress += 0.1f;//progressBar 30

                        //GetDevice
                        await GetDeviceData();
                        Log.Info(TAG, "time GetDeviceData" + DateTime.Now);
                        Console.WriteLine("GetDeviceData:" + DateTime.Now);
                        loading.Progress += 0.1f;
                        //Insert UserAccount
                        await InsertUserAccount();
                        loading.Progress += 0.1f;//progressBar 100
                        Log.Info(TAG, "time InsertUserAccount" + DateTime.Now);
                        Console.WriteLine("InsertUserAccount:" + DateTime.Now);

                        string LoginType = Preferences.Get("LoginType", "");
                        string Username = Preferences.Get("User", "");
                         
                        List<BranchPolicy> lstuserBranch = new List<BranchPolicy>();
                        List<ORM.MerchantDB.Branch> lstBranch = new List<ORM.MerchantDB.Branch>();
                        if (DataCashingAll.UserAccountInfo != null)
                        {
                            //if (LoginType.ToLower() == "employee")
                            //{
                            //    string mainrole = "";
                            //    var data = DataCashingAll.UserAccountInfo.Where(x => x.UserName.ToLower() == Username.ToLower()).FirstOrDefault();
                            //    if (data != null)
                            //    {
                            //        mainrole = data.MainRoles;
                            //        Preferences.Set("LoginType", mainrole);
                            //        LoginType = Preferences.Get("LoginType", "");

                            //        //ข้อมูลสาขาของพนักงานที่เข้าใช้งาน
                            //        BranchPolicyManage branchManage = new BranchPolicyManage();
                            //        lstuserBranch = await branchManage.GetlstBranchPolicy(DataCashingAll.MerchantId, Username);
                            //        Preferences.Set("Branch", lstuserBranch[0].SysBranchID);
                            //    }
                            //    else
                            //    {

                            //    }
                            //}

                            if (LoginType.ToLower() == "owner" | LoginType.ToLower() == "admin")
                            {
                                //owner & admin
                                BranchManage branchManage = new BranchManage();
                                lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
                                if (lstBranch.Count == 1)
                                {
                                    //owner
                                    string BranchID = lstBranch[0].SysBranchID.ToString();
                                    Preferences.Set("Branch", BranchID);
                                    DataCashingAll.SysBranchId = (int)lstBranch[0].SysBranchID;
                                }
                                else
                                {
                                    MainNavigationController mainNavigation = new MainNavigationController("choosebranch", merchants);
                                    mainNavigation.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                    PresentViewControllerAsync(mainNavigation, true);
                                    return;
                                }
                                
                            }
                            else
                            {
                                BranchPolicyManage branchManage = new BranchPolicyManage();
                                lstuserBranch = await branchManage.GetlstBranchPolicy(DataCashingAll.MerchantId, Username);
                                if (lstuserBranch.Count == 1)
                                {
                                    //ตามพนักงาน
                                    string BranchID = lstuserBranch[0].SysBranchID.ToString();
                                    Preferences.Set("Branch", BranchID);
                                    DataCashingAll.SysBranchId = (int)lstuserBranch[0].SysBranchID;
                                }
                                else  if (lstuserBranch.Count > 1)
                                {
                                    MainNavigationController mainNavigation = new MainNavigationController("choosebranch", merchants);
                                    mainNavigation.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                    PresentViewControllerAsync(mainNavigation, true);
                                    return;
                                }
                                else
                                {
                                    Utils.ShowAlert(this, NSBundle.MainBundle.GetLocalizedString("fail", "Fail !"), "คุณไม่มีสิทธิเข้าใช้งาน");
                                    return;
                                }
                                
                            }
                        }
                        else
                        {
                            await InsertUserAccount();
                        }
                    }
                    else
                    {
                        DataCashingAll.SysBranchId = Int32.Parse(branch);
                    }
                    

                    await GetmerchantConfig();
                    Log.Info(TAG, "GetmerchantConfig" + DateTime.Now);
                    Console.WriteLine("time GetmerchantConfig:" + DateTime.Now);
                    loading.Progress += 0.1f;
                    await InsertUserAccount();
                    await GetDeviceData();
                    //GetMemberType
                    await GetMemberTypeData();
                    Log.Info(TAG, "GetMemberTypeData" + DateTime.Now);
                    Console.WriteLine("time GetDeviceData:" + DateTime.Now);
                    //progressBar 40
                    loading.Progress += 0.1f;
                    //Insert GiftVoucher
                    await GetGiftVoucher();
                    Log.Info(TAG, "GetGiftVoucher" + DateTime.Now);
                    Console.WriteLine("time GetGiftVoucher:" + DateTime.Now);
                    //progressBar 45
                    loading.Progress += 0.1f;
                    //Insert MYQRcode
                    await GetMyQRCode();
                    Log.Info(TAG, "GetMyQRCode" + DateTime.Now);
                    Console.WriteLine("time GetMyQRCode:" + DateTime.Now);
                    //progressBar 50
                    loading.Progress += 0.1f;
                    await GetCashTemplate();
                    Log.Info(TAG, "GetCashTemplate" + DateTime.Now);
                    Console.WriteLine("time GetCashTemplate:" + DateTime.Now);
                    //Insert InitialSystemInfo

                    Console.WriteLine("start:InitialSystemInfo" + DateTime.Now);
                    await InitialSystemInfo();//progressBar 100
                    Console.WriteLine("end:InitialSystemInfo" + DateTime.Now);

                    //InvokeOnMainThread(() => Utils.CheckImageLoadnotComplete());
                    if (tasksitem[0] == null) tasksitem[0] = Task.FromResult(0);
                    if (tasksitem[1] == null) tasksitem[1] = Task.FromResult(0);
                    if (taskscus[0] == null) taskscus[0] = Task.FromResult(0);
                    if (taskscus[1] == null) taskscus[1] = Task.FromResult(0);
                    Task.Factory.ContinueWhenAll(tasksitem, completedTasks => Utils.CheckImageLoaditemnotComplete());
                    Task.Factory.ContinueWhenAll(taskscus, completedTasks => Utils.CheckImageLoadcustomernotComplete());

                    //if (tasksitem[0] == null && tasksitem[1] == null)
                    //{
                    //    Task.Factory.StartNew(() => Utils.CheckImageLoaditemnotComplete());

                    //}
                    //else
                    //{
                    //    Task.Factory.ContinueWhenAll(tasksitem, completedTasks => Utils.CheckImageLoaditemnotComplete());

                    //}
                    //if (taskscus[0] == null && taskscus[1] == null )
                    //{

                    //    Task.Factory.StartNew(() => Utils.CheckImageLoadcustomernotComplete());
                    //}
                    //else
                    //{
                    //    Task.Factory.ContinueWhenAll(taskscus, completedTasks => Utils.CheckImageLoadcustomernotComplete());

                    //}
                    //Task.Factory. ContinueWhenAll(takssitem , completedTasks => Utils.CheckImageLoaditemnotComplete());

                    Log.Info(TAG, "InitialSystemInfo" + DateTime.Now);
                    Console.WriteLine("InitialSystemInfo:" + DateTime.Now);


                    loading.Progress = 1f;
                    try
                    {
                        
                        branch = Preferences.Get("Branch", "");
                        if (!string.IsNullOrEmpty(branch))
                        {
                            MainController.merchant = merchants;
                            DataCashingAll.SysBranchId = Int32.Parse(branch);
                            DataCaching.MainNavigation = new MainNavigationController("main", merchants);
                            uIWindowRoot = UIApplication.SharedApplication.Windows.First();
                            uIWindowRoot.RootViewController = DataCaching.MainNavigation;
                            uIWindowRoot.MakeKeyAndVisible();
                            return;
                        }
                        else
                        {

                            MainNavigationController mainNavigation = new MainNavigationController("choosebranch", merchants);
                            mainNavigation.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            PresentViewControllerAsync(mainNavigation, true);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        _ = TinyInsights.TrackErrorAsync(ex);
                    }
                    loading.Progress += 0.1f;

                    

                    // load 
                    //MainController.merchant = merchants; 
                    //DataCaching.MainNavigation =  new MainNavigationController("main",merchants);
                    //uIWindowRoot = UIApplication.SharedApplication.Windows.First();
                    //uIWindowRoot.RootViewController = DataCaching.MainNavigation;
                    //uIWindowRoot.MakeKeyAndVisible();
                }
                else
                {

                    DataCaching.LoginNavigation.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewControllerAsync(DataCaching.LoginNavigation, true);
                }
            }
            catch (Exception ex)
            {
                if (counterror == 2)
                {
                    Preferences.Set("AppState", "logout");
                    Preferences.Set("Branch", "");
                    POSController.tranWithDetails = null; 
                    await BellNotificationHelper.UnRegisterBellNotification(GabanaAPI.gbnJWT);
                    DataCaching.LoginNavigation.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewControllerAsync(DataCaching.LoginNavigation, true);
                    counterror = 0;
                    Preferences.Clear();
                    _ = TinyInsightsLib.TinyInsights.TrackErrorAsync(ex, null);
                }
                counterror++;
                Utils.ShowAlert(this, NSBundle.MainBundle.GetLocalizedString("fail", "Fail !"), "ไม่สามารถโหลดข้อมูลได้กรุณาติดต่อ Admin");
                Reloadimg.Hidden = false;
                loading.Hidden = true;
                Utils.ShowAlert(this, ex.Message, "");
                _ = TinyInsights.TrackErrorAsync(ex);
            }

        }

        [Export("btnReconnect:")]
        public async void btnReconnect(UIGestureRecognizer sender)
        {
            Reconnect();
        }

        async Task SetDataDeviceTranRunningNo()
        {
            try
            {
                deviceTranRunningNoManage = new DeviceTranRunningNoManage();
                var TranRunnning = await deviceTranRunningNoManage.GetAllDeviceTranRunningNo(DataCashingAll.MerchantId);
                List<DeviceTranRunningNo> TranRunnningConvert = new List<DeviceTranRunningNo>();
                if (TranRunnning.Count == 0)
                {
                    //Get TranRunning จาก API
                    TranRunnningConvert = await GabanaAPI.GetDataDeviceTranRunningNo(DataCashingAll.DeviceNo);
                    if (TranRunnningConvert.Count != 0)
                    {
                        foreach (var item in TranRunnningConvert)
                        {
                            deviceTranRunning = new DeviceTranRunningNo()
                            {
                                MerchantID = item.MerchantID,
                                SysBranchID = item.SysBranchID,
                                DeviceNo = item.DeviceNo,
                                TranLastRunningNo = item.TranLastRunningNo
                            };
                            var updatetResult = await deviceTranRunningNoManage.InsertOrReplaceDeviceTranRunningNo(deviceTranRunning);
                        }
                    }
                }

                if (DataCashingAll.SysBranchId != 0)
                {
                    TranRunnning = await deviceTranRunningNoManage.GetAllDeviceTranRunningNo(DataCashingAll.MerchantId);
                    TranRunnning = TranRunnning.Where(x => x.MerchantID == DataCashingAll.MerchantId & x.SysBranchID == DataCashingAll.SysBranchId).ToList();
                    if (TranRunnning.Count == 0)
                    {
                        deviceTranRunning = new DeviceTranRunningNo()
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            SysBranchID = DataCashingAll.SysBranchId,
                            DeviceNo = DataCashingAll.DeviceNo,
                            TranLastRunningNo = 0
                        };
                        var updatetResult2 = await deviceTranRunningNoManage.InsertOrReplaceDeviceTranRunningNo(deviceTranRunning);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, ex.Message, "");
                //RunOnUiThread(() =>
                //{
                //    btnReconnect.Visibility = ViewStates.Visible;
                //});

                //numreconnect = numreconnect + 1;
                //if (numreconnect > 2)
                //{
                //    progressBar.Progress = 100;
                //    StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                //    numreconnect = 0;
                //    this.Finish();
                //}

                //_ = TinyInsights.TrackErrorAsync(ex);
                //_ = TinyInsights.TrackPageViewAsync("SetDataDeviceTranRunningNo at Splash");
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        public async Task InsertBranchtoLocal() 
        {
            try
            {
                MerchantManage merchantManage = new MerchantManage();
                var getMerchant = await merchantManage.GetMerchant(merchants.Merchant.MerchantID);

                BranchManage branchManage = new BranchManage();

                ORM.MerchantDB.Branch insertBranch = new ORM.MerchantDB.Branch();
                for (int i = 0; i < merchants.Branch.Count; i++)
                {
                    insertBranch.MerchantID = merchants.Merchant.MerchantID;
                    insertBranch.SysBranchID = Convert.ToInt64(merchants.Branch[i].BranchID);
                    insertBranch.Ordinary = merchants.Branch[i].Ordinary;
                    insertBranch.BranchName = merchants.Branch[i].BranchName;
                    insertBranch.BranchID = merchants.Branch[i].BranchID;
                    insertBranch.Address = merchants.Branch[i].Address;
                    insertBranch.ProvincesId = merchants.Branch[i].ProvincesId;
                    insertBranch.AmphuresId = merchants.Branch[i].AmphuresId;
                    insertBranch.DistrictsId = merchants.Branch[i].DistrictsId;
                    insertBranch.Status = merchants.Branch[i].Status;
                    insertBranch.DisplayLanguage = merchants.Branch[i].DisplayLanguage;
                    insertBranch.Lat = merchants.Branch[i].Lat;
                    insertBranch.Lng = merchants.Branch[i].Lng;
                    insertBranch.Email = merchants.Branch[i].Email;
                    insertBranch.Tel = merchants.Branch[i].Tel;
                    insertBranch.Line = merchants.Branch[i].Line;
                    insertBranch.Facebook = merchants.Branch[i].Facebook;
                    insertBranch.Instagram = merchants.Branch[i].Instagram;
                    insertBranch.TaxBranchName = merchants.Branch[i].TaxBranchName;
                    insertBranch.TaxBranchID = merchants.Branch[i].TaxBranchID;
                    insertBranch.LinkProMaxxID = merchants.Branch[i].LinkProMaxxID;
                    insertBranch.Comments = merchants.Branch[i].Comments;

                    var insert = await branchManage.InsertorReplacrBranch(insertBranch);

                    if (merchants.Branch.Count == 1 | merchants.Branch[i].SysBranchID == 1)
                    {
                        DataCaching.branchDeatail = insertBranch;
                    }

                    if (!insert)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log.Debug("insertBranch", ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }

        public async Task InitialSystemInfo() //SystemRivisionNo
        {
            try
            {
                systemSeqNoManage = new DeviceSystemSeqNoManage();
                deviceSystemSeq = new DeviceSystemSeqNo();
                systemRevisionNoManage = new SystemRevisionNoManage();
                SystemInfoManage = new SystemInfoManage();
                listRivision = new List<SystemRevisionNo>();
                //-------------------------------------------------------------
                 lstSystemInfo = new List<SystemInfo>();
                categoryManage = new CategoryManage();
                category = new Category();
                itemManage = new ItemManage();
                itemExSizeManage = new ItemExSizeManage();
                var getItem = new Item();
                var getitemSize = new ItemExSize();
                var deviceSystemSeqNo = new DeviceSystemSeqNo();
                var deviceTranRunningNoManage = new DeviceTranRunningNoManage();
                var deviceTranRunning = new DeviceTranRunningNo();
                customerManage = new CustomerManage();
                customer = new Customer();
                discountTemplateManage = new DiscountTemplateManage();
                discount = new DiscountTemplate();
                var noteManage = new NoteManage();
                var note = new Note();
                var noteCategoryManage = new NoteCategoryManage();
                var noteCategory = new NoteCategory();
                CultureInfo.CurrentCulture = new CultureInfo("en-US");

                //---------------------------------------------------------
                provinces = new List<Province>();
                amphure = new List<Amphure>();
                district = new List<District>();
                geography = new List<Geography>();

                string ProvinceQuery;
                //---------------------------------------------------------

                if (merchants == null)
                {
                    return;
                }

                lstSystemInfo = await SystemInfoManage.GetSystemInfo();
                if (lstSystemInfo.Count == 0)
                {
                    List<SystemInfo> systemInfo = new List<SystemInfo>()
                    {
                        new SystemInfo(){ Name = "Branch" , SystemID = 10},
                        new SystemInfo(){ Name = "Category" , SystemID = 20},
                        new SystemInfo(){ Name = "Item" , SystemID = 30},
                        new SystemInfo(){ Name = "ItemOnBranch" , SystemID = 31},
                        new SystemInfo(){ Name = "Discount" , SystemID = 40},
                        new SystemInfo(){ Name = "Customer" , SystemID = 50},
                        new SystemInfo(){ Name = "NoteCategory" , SystemID = 60},
                        new SystemInfo(){ Name = "Note" , SystemID = 70}
                    };
                    await SystemInfoManage.InsertSystemInfo(systemInfo);
                    lstSystemInfo = await SystemInfoManage.GetSystemInfo();
                }
                
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                if (listRivision.Count == 0)
                {
                    for (int i = 0; i < lstSystemInfo.Count; i++)
                    {
                        SystemRevisionNo = new SystemRevisionNo()
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            SystemID = lstSystemInfo[i].SystemID,
                            LastRevisionNo = 0
                        };
                        var result = await systemRevisionNoManage.InsertSystemRevisionno(SystemRevisionNo);

                        deviceSystemSeq = new DeviceSystemSeqNo
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            DeviceNo = merchants.Device.DeviceNo,
                            SystemID = lstSystemInfo[i].SystemID,
                            LastSysSeqNo = 0
                        };
                        var resultSeqNo = await systemSeqNoManage.InsertDeviceSystemSeqNo(deviceSystemSeq);

                    }
                }
                //--------------------------------------------------------------------------
                provinces = await poolManager.GetProvinces();
                if (provinces == null || provinces.Count == 0)
                {
                    //   System.IO.StreamReader file = new System.IO.StreamReader("c:\\test.txt");
                    string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ProvinceGB.txt");
                    using (var reader = new System.IO.StreamReader(fileName))
                    {
                        ProvinceQuery = reader.ReadToEnd();
                    }
                    var x = ProvinceQuery;
                }
                

                var TranRunnning = await deviceTranRunningNoManage.GetAllDeviceTranRunningNo(DataCashingAll.MerchantId);
                List<DeviceTranRunningNo> TranRunnningConvert = new List<DeviceTranRunningNo>();
                if (TranRunnning.Count == 0)
                {
                    //Get TranRunning จาก API
                    TranRunnningConvert = await GabanaAPI.GetDataDeviceTranRunningNo(DataCashingAll.DeviceNo);
                    if (TranRunnningConvert.Count != 0)
                    {
                        foreach (var item in TranRunnningConvert)
                        {
                            deviceTranRunning = new DeviceTranRunningNo()
                            {
                                MerchantID = item.MerchantID,
                                SysBranchID = item.SysBranchID,
                                DeviceNo = item.DeviceNo,
                                TranLastRunningNo = item.TranLastRunningNo
                            };
                            var updatetResult = await deviceTranRunningNoManage.InsertOrReplaceDeviceTranRunningNo(deviceTranRunning);
                        }
                    }
                }

                if (DataCashingAll.SysBranchId != 0)
                {
                    TranRunnning = await deviceTranRunningNoManage.GetAllDeviceTranRunningNo(DataCashingAll.MerchantId);
                    TranRunnning = TranRunnning.Where(x => x.MerchantID == DataCashingAll.MerchantId & x.SysBranchID == DataCashingAll.SysBranchId).ToList();
                    if (TranRunnning.Count == 0)
                    {
                        deviceTranRunning = new DeviceTranRunningNo()
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            SysBranchID = DataCashingAll.SysBranchId,
                            DeviceNo = DataCashingAll.DeviceNo,
                            TranLastRunningNo = 0
                        };
                        var updatetResult2 = await deviceTranRunningNoManage.InsertOrReplaceDeviceTranRunningNo(deviceTranRunning);
                    }
                }


                //GetSeqNoApi
                //get api
                List<ORM.Master.DeviceSystemSeqNo> systemSeqNos = new List<ORM.Master.DeviceSystemSeqNo>();

                systemSeqNos = await GabanaAPI.GetDataDeviceSeqNo(merchants.Device.DeviceNo);
                if (systemSeqNos == null)
                {
                    foreach (var item in listRivision)
                    {
                        deviceSystemSeqNo = new DeviceSystemSeqNo()
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            SystemID = item.SystemID,
                            DeviceNo = DataCashingAll.DeviceNo,
                            LastSysSeqNo = 0
                        };
                        var result = await systemSeqNoManage.InsertOrReplaceDeviceSystemSeqNo(deviceSystemSeqNo);
                    }
                }

                foreach (var item in systemSeqNos)
                {
                    var resultseq = await systemSeqNoManage.GetDeviceSystemSeqNo(merchants.Merchant.MerchantID, merchants.Device.DeviceNo, (int)item.SystemID);
                    if (item.LastSysSeqNo > resultseq)
                    {
                        deviceSystemSeqNo = new DeviceSystemSeqNo()
                        {
                            MerchantID = item.MerchantID,
                            SystemID = item.SystemID,
                            DeviceNo = item.DeviceNo,
                            LastSysSeqNo = item.LastSysSeqNo
                        };
                        var result = await systemSeqNoManage.InsertOrReplaceDeviceSystemSeqNo(deviceSystemSeqNo);
                    }
                }

                //-----------------------------------------------
                //Check local ว่า SystemREvisionNo ว่างหรือไม่
                //Get Branch 10
                //Get Category (int revisionNo) 20 
                //Get Item (int revisionNo,int offset) 30
                //Get Discount 40
                //Get Customer 50
                //Get Note 60
                //Get NoteCategory 70
                //-----------------------------------------------
                Console.WriteLine("InitialSystemInfo: setting " + DateTime.Now);
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                Console.WriteLine("InitialSystemInfo:GetAllSystemRevisionNo" + DateTime.Now);
                int maxItemRevision = 0;
                int maxCategoryRevision = 0;
                
                int maxItemOnBranchRevision = 0;
                int maxDiscountRevision = 0;
                int maxCustomerRevision = 0;
                int maxNoteCategoryRevision = 0;
                int maxNoteRevision = 0;

                for (int i = 0; i < listRivision.Count; i++)
                {
                    switch (listRivision[i].SystemID)
                    {
                        case 10:
                            //Get Branch API
                            //var branchResult = "";

                            break;
                        case 20:
                            Console.WriteLine("time categorystart" + DateTime.Now);
                            #region Category
                            //Get Category API
                            var allcategory = await GabanaAPI.GetDataCategory((int)listRivision[i].LastRevisionNo);

                            if (allcategory == null)
                            {
                                return;
                            }

                            if (allcategory.Categories.Count == 0)
                            {
                                break;
                            }
                            var maxCategory = allcategory.Categories.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();
                            maxCategoryRevision = maxCategory;

                            //insert to Local DB
                            SystemRevisionNo = new SystemRevisionNo()
                            {
                                MerchantID = DataCashingAll.MerchantId,
                                SystemID = 20,
                                LastRevisionNo = maxCategoryRevision
                            };

                            var result = await systemRevisionNoManage.UpdateSystemReviosion(SystemRevisionNo);

                            foreach (var item in allcategory.Categories)
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
                                    FWaitSending = 1,
                                    WaitSendingTime = DateTime.UtcNow,
                                    LinkProMaxxID = item.LinkProMaxxID
                                };
                                var insertOrreplace = await categoryManage.InsertOrReplaceCategory(category);
                            }
                            foreach (var item in allcategory.CategoryBins)
                            {
                                //UpdateItem
                                var UpdateItem = await itemManage.GetItembyCategory(item.MerchantID, item.SysCategoryID);
                                if (UpdateItem != null)
                                {
                                    foreach (var update in UpdateItem)
                                    {
                                        update.SysCategoryID = null;
                                        var resultUpdate = await itemManage.UpdateItem(update);
                                    }
                                }
                                //delete
                                var delete = await categoryManage.DeleteCategory(item.MerchantID, item.SysCategoryID);
                            }
                            #endregion
                            Console.WriteLine("time categoryend" + DateTime.Now);
                            break;
                        case 30:
                            Console.WriteLine("time itemstart" + DateTime.Now);
                            #region Item
                            //------------------------------------------------
                            //Get Item API
                            //offset = index สำหรับเรียกข้อมูล ครั้งละ 100 ตัว เริ่มที่ 0
                            //total >= 100 item = 0 - 99     รอบที่ 1  offset = 0
                            //             item = 100 - 199  รอบที่ 2  offset = 1
                            //total > 100  => totalitem/100 = จำนวนรอบที่เรียก 
                            //------------------------------------------------
                            List<Item> lstInsertItemImage = new List<Item>();
                            List<Gabana3.JAM.Items.ItemWithItemExSizes> UpdateItemlist = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                            List<Gabana3.JAM.Items.ItemWithItemExSizes> InsertItemlist = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                            try
                            {

                                var allItem = await GabanaAPI.GetDataItem((int)listRivision[i].LastRevisionNo, 0);


                                if (allItem == null)
                                {
                                    break;
                                }
                                else if (allItem?.ItemsWithItemExSizes.Count == 0)
                                {
                                    break;
                                }
                                else
                                {
                                    int round30 = allItem.totalItems / 100;
                                    int addrount30 = round30 + 1;
                                    for (int j = 0; j < addrount30; j++)
                                    {
                                        Console.WriteLine("Getstartitem row " + j + 1 + "/time" + DateTime.Now);
                                        
                                        allItem = await GabanaAPI.GetDataItem((int)listRivision[i].LastRevisionNo, j);
                                        Console.WriteLine("GetEnditem row " + j + 1 + "/time" + DateTime.Now);
                                        Console.WriteLine("Makedatastartitem row " + j + 1 + "/time" + DateTime.Now);
                                        
                                        if (allItem == null)
                                        {
                                            break;
                                        }

                                        if (allItem.totalItems == 0)
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

                                        if (InsertItem.Count > 0)
                                        {
                                            foreach (var item in InsertItem)
                                            {
                                                string thumnailPath = string.Empty;
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
                                                    var errorRevison = InsertItem.Select(x => x.ItemStatus.item.RevisionNo).Min();
                                                    maxItemRevision = errorRevison;
                                                    Log.Error("connecterror", "Bulkitem,BulkitemExsize : " + ex.Message);
                                                    _ = TinyInsights.TrackErrorAsync(ex);
                                                    throw ex;
                                                }
                                            }
                                            lstInsertItemImage.AddRange(Bulkitem);
                                        }

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

                                        await UtilsAll.updateRevisionNo((int)listRivision[i].SystemID, maxItem);
                                        Console.WriteLine("MakedataEnditem row " + j + 1 + "/time" + DateTime.Now);
                                    }

                                    
                                    tasksitem[0] = Task.Factory.StartNew(() => Utils.InsertPictureLocalItem(lstInsertItemImage));
                                    tasksitem[1] = Task.Factory.StartNew(() => Utils.UpdateImageItem(UpdateItemlist));

                                }
                            }
                            catch (Exception ex)
                            {
                                _ = TinyInsights.TrackErrorAsync(ex);
                                Console.WriteLine(ex.Message);
                                await UtilsAll.ErrorRevisionNo((int)listRivision[i].SystemID, maxItemRevision);
                            }
                            #endregion
                            Console.WriteLine("time itemend" + DateTime.Now);
                            break;
                        case 31:
                            Console.WriteLine("time itemonbranchstart" + DateTime.Now);
                            #region ItemOnBranch                            
                            try
                            {

                                //var allItemOnBranch = await GabanaAPI.GetDataItemOnBranch((int)listRivision[i].LastRevisionNo, 0);
                                //if (allItemOnBranch == null)
                                //{
                                //    break;
                                //}


                                //if (allItemOnBranch.Count > 0) 
                                //{
                                //    int round31 = allItemOnBranch.Count / 100;
                                //    int addrount31 =  1;
                                //    for (int j = 0; j <= addrount31; j++)
                                //    {
                                //        allItemOnBranch = await GabanaAPI.GetDataItemOnBranch((int)listRivision[i].LastRevisionNo, j);
                                //        if (allItemOnBranch.Count == 100)
                                //        {
                                //            addrount31++;
                                //        }

                                //        if (allItemOnBranch == null)
                                //        {
                                //            break;
                                //        }

                                //        if (allItemOnBranch.Count == 0)
                                //        {
                                //            break;
                                //        }

                                //        allItemOnBranch.OrderBy(x => x.RevisionNo);
                                //        var maxItemOnBranch = allItemOnBranch.Max(x => x.RevisionNo);

                                //        foreach (var item in allItemOnBranch)
                                //        {
                                //            ItemOnBranch stock = new ItemOnBranch()
                                //            {
                                //                MerchantID = item.MerchantID,
                                //                SysBranchID = item.SysBranchID,
                                //                SysItemID = item.SysItemID,
                                //                BalanceStock = item.BalanceStock,
                                //                MinimumStock = item.MinimumStock,
                                //                LastDateBalanceStock = item.LastDateBalanceStock,
                                //            };

                                //            var insertStock = await onBranchManage.InsertorReplaceItemOnBranch(stock);
                                //            maxItemRevision = item.RevisionNo;
                                //        }
                                //        await UtilsAll.updateRevisionNo((int)listRivision[i].SystemID, allItemOnBranch.Max(x=>x.RevisionNo));
                                //    }
                                //}

                                var allItemOnBranch = await GabanaAPI.GetDataItemOnBranchV2((int)listRivision[i].LastRevisionNo, 0);
                                if (allItemOnBranch == null)
                                {
                                    
                                    break;
                                }

                                if (allItemOnBranch.totalItemOnBranch == 0)
                                {
                                    
                                    break;
                                }

                                if (allItemOnBranch.totalItemOnBranch > 0)
                                {
                                    //RunOnUiThread(async ()  =>
                                    //{
                                    int percent = 0;
                                    double round = 0, addrount = 0;
                                    round = allItemOnBranch.totalItemOnBranch / 100;
                                    addrount = round + 1;
                                    double percentage = 0, temp = 0;
                                    percentage = (20 / addrount);
                                    temp = percentage;
                                    percentage = 0;

                                    for (int j = 0; j < addrount; j++)
                                    {
                                        allItemOnBranch = await GabanaAPI.GetDataItemOnBranchV2((int)listRivision[i].LastRevisionNo, j);

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

                                        await UtilsAll.updateRevisionNo((int)listRivision[i].SystemID, maxItemOnBranch);

                                    }
                                    Log.Debug("connectpass", "listRivisionItemOnBranch");
                                    //});
                                }
                            }
                            catch (Exception ex)
                            {
                                _ = TinyInsights.TrackErrorAsync(ex);
                                Console.WriteLine(ex.Message);
                                await UtilsAll.ErrorRevisionNo((int)listRivision[i].SystemID, maxItemRevision);
                            }
                            #endregion                            
                            Console.WriteLine("time itemonbranchend" + DateTime.Now);
                            break;
                        case 40:
                            Console.WriteLine("time discountstart" + DateTime.Now);
                            #region Discount
                            //Get Discount API
                            var alldiscount = await GabanaAPI.GetDataDiscountTemplate((int)listRivision[i].LastRevisionNo, 0);

                            if (alldiscount == null)
                            {
                                break;
                            }

                            if (alldiscount.total == 0)
                            {
                                break;
                            }
                            var maxDiscount = alldiscount.DiscountTemplateStatus.ToList().Max(x => x.DiscountTemplates.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();
                            maxDiscountRevision = maxDiscount;

                            //insert to Local DB
                            SystemRevisionNo = new SystemRevisionNo()
                            {
                                MerchantID = DataCashingAll.MerchantId,
                                SystemID = 40,
                                LastRevisionNo = maxDiscountRevision
                            };

                            var resultDiscount = await systemRevisionNoManage.UpdateSystemReviosion(SystemRevisionNo);

                            foreach (var item in alldiscount.DiscountTemplateStatus)
                            {
                                if (item.DataStatus == 'D')
                                {
                                    //delete
                                    var delete = await discountTemplateManage.DeleteDiscountTemplate(item.DiscountTemplates.MerchantID, item.DiscountTemplates.SysDiscountTemplate);
                                }
                                else
                                {
                                    //insertorreplace
                                    discount = new DiscountTemplate()
                                    {
                                        MerchantID = item.DiscountTemplates.MerchantID,
                                        SysDiscountTemplate = item.DiscountTemplates.SysDiscountTemplate,
                                        TemplateName = item.DiscountTemplates.TemplateName,
                                        TemplateType = item.DiscountTemplates.TemplateType,
                                        FmlDiscount = item.DiscountTemplates.FmlDiscount,
                                        DateCreated = item.DiscountTemplates.DateCreated,
                                        DataStatus = 'I',
                                        DateModified = item.DiscountTemplates.DateModified,
                                        FWaitSending = 1,
                                        WaitSendingTime = DateTime.UtcNow

                                    };
                                    var insertOrreplace = await discountTemplateManage.InsertOrReplaceDiscountTemplate(discount);
                                }
                            }
                            #endregion
                            Console.WriteLine("time discountend" + DateTime.Now);
                            break;
                        case 50:
                            Console.WriteLine("time customerstart" + DateTime.Now);
                            #region Customer
                            //Get Customer API
                            try
                            {
                                var allcustomer = await GabanaAPI.GetDataCustomer((int)listRivision[i].LastRevisionNo, 0);
                               
                                if (allcustomer == null)
                                {
                                    break;
                                }

                                if (allcustomer.totalCustomer == 0)
                                {
                                    break;
                                }
                                List<Gabana3.JAM.Customer.CustomerStatus> UpdateCustomer = new List<Gabana3.JAM.Customer.CustomerStatus>();
                                List<Gabana3.JAM.Customer.CustomerStatus> InsertCustomer = new List<Gabana3.JAM.Customer.CustomerStatus>();
                                List<Customer> lstCustomerImage = new List<Customer>();
                                int round = 0, addrount = 0;
                                round = allcustomer.totalCustomer / 100;
                                addrount = round + 1;
                                for (int j = 0; j < addrount; j++)
                                {
                                    allcustomer = await GabanaAPI.GetDataCustomer((int)listRivision[i].LastRevisionNo, j);
                                    if (allcustomer == null)
                                    {
                                        break;
                                    }

                                    if (allcustomer.totalCustomer == 0)
                                    {
                                        break;
                                    }

                                    allcustomer.CustomerStatus.ToList().OrderBy(x => x.Customers.RevisionNo);
                                    var maxCustomer = allcustomer.CustomerStatus.ToList().Max(x => x.Customers.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();

                                    List<Customer> GetallCustomer = new List<Customer>();
                                    GetallCustomer = await customerManage.GetAllCustomer();
                                    UpdateCustomer.AddRange(allcustomer.CustomerStatus.Where(x => GetallCustomer.Select(y => (long)y.SysCustomerID).ToList().Contains(x.Customers.SysCustomerID)).ToList());
                                    InsertCustomer.AddRange(allcustomer.CustomerStatus.Where(x => !(GetallCustomer.Select(y => (long)y.SysCustomerID).ToList().Contains(x.Customers.SysCustomerID)) && x.DataStatus != 'D').ToList());


                                    if (InsertCustomer.Count > 0)
                                    {
                                        List<Customer> BulkCustomer = new List<Customer>();
                                        foreach (var customer in InsertCustomer)
                                        {
                                            string thumnailPath = string.Empty;
                                           

                                            BulkCustomer.Add(new Customer()
                                            {
                                                MerchantID = customer.Customers.MerchantID,
                                                SysCustomerID = customer.Customers.SysCustomerID,
                                                CustomerName = customer.Customers.CustomerName,
                                                Ordinary = customer.Customers.Ordinary,
                                                ShortName = customer.Customers.ShortName,
                                                PictureLocalPath = "",
                                                ThumbnailLocalPath = thumnailPath,
                                                EMail = customer.Customers.EMail,
                                                Mobile = customer.Customers.Mobile,
                                                Gender = customer.Customers.Gender,
                                                BirthDate = customer.Customers.BirthDate,
                                                Address = customer.Customers.Address,
                                                ProvincesId = customer.Customers.ProvincesId,
                                                AmphuresId = customer.Customers.AmphuresId,
                                                DistrictsId = customer.Customers.DistrictsId,
                                                PicturePath = customer.Customers.PicturePath, //Clound Image
                                                IDCard = customer.Customers.IDCard,
                                                Comments = customer.Customers.Comments,
                                                LastDateModified = customer.Customers.LastDateModified,
                                                UserLastModified = customer.Customers.UserLastModified,
                                                DataStatus = customer.DataStatus,
                                                FWaitSending = 0,
                                                WaitSendingTime = DateTime.UtcNow,
                                                LinkProMaxxID = customer.Customers.LinkProMaxxID,
                                                MemberTypeNo = customer.Customers.MemberTypeNo,
                                                CustomerID = customer.Customers.CustomerID,
                                                LineID = customer.Customers.LineID,
                                                ThumbnailPath = customer.Customers.ThumbnailPath, //Clound Image
                                            });
                                            maxCustomerRevision = customer.Customers.RevisionNo;
                                        }

                                        using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                                        {
                                            try
                                            {
                                                await db.BulkCopyAsync(BulkCustomer);
                                            }
                                            catch (Exception ex)
                                            {
                                                var errorRevison = InsertCustomer.Select(x => x.Customers.RevisionNo).Min();
                                                maxCustomerRevision = errorRevison;
                                                Log.Error("connecterror", "BulkCustomer :" + ex.Message);
                                                _ = TinyInsights.TrackErrorAsync(ex);
                                                throw ex;
                                            }
                                        }

                                        lstCustomerImage.AddRange(BulkCustomer);
                                    }

                                    //Update Customer
                                    if (UpdateCustomer.Count > 0)
                                    {
                                        foreach (var customer in UpdateCustomer)
                                        {
                                            var data = await customerManage.GetCustomer(customer.Customers.MerchantID, customer.Customers.SysCustomerID);

                                            if (customer.DataStatus == 'D')
                                            {
                                                //delete รูป
                                                if (!string.IsNullOrEmpty(data?.ThumbnailLocalPath))
                                                {
                                                    //Java.IO.File imgTempFile = new Java.IO.File(data?.ThumbnailLocalPath);

                                                    //if (System.IO.File.Exists(imgTempFile.AbsolutePath))
                                                    //{
                                                    //    System.IO.File.Delete(imgTempFile.AbsolutePath);
                                                    //}
                                                }
                                                //delete
                                                var delete = await customerManage.DeleteCustomer(customer.Customers.MerchantID, customer.Customers.SysCustomerID);
                                                if (!delete)
                                                {
                                                    if (data != null)
                                                    {
                                                        data.DataStatus = 'D';
                                                        data.FWaitSending = 0;
                                                        await customerManage.UpdateCustomer(data);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                string thumnailLocalPath = string.Empty;

                                                //insertorreplace
                                                Customer _customer = new Customer()
                                                {
                                                    MerchantID = customer.Customers.MerchantID,
                                                    SysCustomerID = customer.Customers.SysCustomerID,
                                                    CustomerName = customer.Customers.CustomerName,
                                                    Ordinary = customer.Customers.Ordinary,
                                                    ShortName = customer.Customers.ShortName,
                                                    PictureLocalPath = "",
                                                    ThumbnailLocalPath = thumnailLocalPath,
                                                    EMail = customer.Customers.EMail,
                                                    Mobile = customer.Customers.Mobile,
                                                    Gender = customer.Customers.Gender,
                                                    BirthDate = customer.Customers.BirthDate,
                                                    Address = customer.Customers.Address,
                                                    ProvincesId = customer.Customers.ProvincesId,
                                                    AmphuresId = customer.Customers.AmphuresId,
                                                    DistrictsId = customer.Customers.DistrictsId,
                                                    PicturePath = customer.Customers.PicturePath, //Clound Image
                                                    IDCard = customer.Customers.IDCard,
                                                    Comments = customer.Customers.Comments,
                                                    LastDateModified = customer.Customers.LastDateModified,
                                                    UserLastModified = customer.Customers.UserLastModified,
                                                    DataStatus = customer.DataStatus,
                                                    FWaitSending = 0,
                                                    WaitSendingTime = DateTime.UtcNow,
                                                    LinkProMaxxID = customer.Customers.LinkProMaxxID,
                                                    MemberTypeNo = customer.Customers.MemberTypeNo,
                                                    CustomerID = customer.Customers.CustomerID,
                                                    LineID = customer.Customers.LineID,
                                                    ThumbnailPath = customer.Customers.ThumbnailPath, //Clound Image

                                                };
                                                var insertOrreplace = await customerManage.InsertOrReplaceCustomer(_customer);
                                            }

                                            maxCustomerRevision = customer.Customers.RevisionNo;
                                        }
                                    }
                                    await UtilsAll.updateRevisionNo((int)listRivision[i].SystemID, maxCustomer);
                                }
                                Log.Debug("connectpass", "InsertPictureLocalCustomer(lstCustomerImage) lstCustomerImage : " + lstCustomerImage.Count);
                                taskscus[0] = Task.Factory.StartNew(() => Utils.InsertPictureLocalCustomer(lstCustomerImage));
                                
                                Log.Debug("connectpass", "UpdateImageCustomer(UpdateCustomer) UpdateCustomer : " + UpdateCustomer.Count);
                                taskscus[1] = Task.Factory.StartNew(() => Utils.UpdateImageCustomer(UpdateCustomer));
                                
                                Log.Debug("connectpass", "listRivisionCustomer");
                                
                            }
                            catch (Exception ex)
                            {
                                Log.Debug("connecterror", "listRivisionCustomer : " + ex.Message);
                                await UtilsAll.ErrorRevisionNo((int)listRivision[i].SystemID, maxCustomerRevision);
                                _ = TinyInsights.TrackErrorAsync(ex);
                            }
                            #endregion
                            Console.WriteLine("time customerend" + DateTime.Now);
                            break;
                        case 60:
                            Console.WriteLine("time notecatstart" + DateTime.Now);
                            #region NoteCategory
                            try
                            {
                                var allNoteCategory = await GabanaAPI.GetDataNoteCategory((int)listRivision[i].LastRevisionNo);

                                if (allNoteCategory == null)
                                {
                                    break;
                                }

                                int maxNoteCategory = 0;
                                if (allNoteCategory.NoteCategory.Count > 0)
                                {
                                    allNoteCategory.NoteCategory.ToList().OrderBy(x => x.RevisionNo);
                                    maxNoteCategory = allNoteCategory.NoteCategory.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();  

                                    //check ว่ามีไหม
                                    List<ORM.Master.NoteCategory> UpdateNoteCategory = new List<ORM.Master.NoteCategory>();
                                    List<ORM.Master.NoteCategory> InsertNoteCategory = new List<ORM.Master.NoteCategory>();
                                    List<NoteCategory> GetallNoteCategory = await noteCategoryManage.GetAllNoteCategory();
                                    UpdateNoteCategory.AddRange(allNoteCategory.NoteCategory.Where(x => GetallNoteCategory.Select(y => (long)y.SysNoteCategoryID).ToList().Contains(x.SysNoteCategoryID)).ToList());
                                    InsertNoteCategory.AddRange(allNoteCategory.NoteCategory.Where(x => !(GetallNoteCategory.Select(y => (long)y.SysNoteCategoryID).ToList().Contains(x.SysNoteCategoryID))).ToList());

                                    //Insert NoteCategory
                                    if (InsertNoteCategory.Count > 0)
                                    {
                                        List<NoteCategory> BulkNoteCategory = new List<NoteCategory>();

                                        foreach (var NoteCategory in InsertNoteCategory)
                                        {
                                            BulkNoteCategory.Add(new NoteCategory()
                                            {
                                                MerchantID = NoteCategory.MerchantID,
                                                SysNoteCategoryID = NoteCategory.SysNoteCategoryID,
                                                Ordinary = NoteCategory.Ordinary,
                                                Name = NoteCategory.Name,
                                                DateCreated = NoteCategory.DateCreated,
                                                DateModified = NoteCategory.DateModified,
                                                DataStatus = 'I',
                                                FWaitSending = 1,
                                                WaitSendingTime = DateTime.UtcNow
                                            });
                                            maxNoteCategoryRevision = NoteCategory.RevisionNo;
                                        }

                                        using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                                        {
                                            try
                                            {
                                                await db.BulkCopyAsync(BulkNoteCategory);
                                            }
                                            catch (Exception ex)
                                            {
                                                _ = TinyInsights.TrackErrorAsync(ex);
                                                var errorRevison = InsertNoteCategory.Select(x => x.RevisionNo).Min();
                                                maxNoteCategoryRevision = errorRevison;
                                                Log.Error("connecterror", "BulkNoteCategory :" + ex.Message);
                                                throw ex;
                                            }
                                        }
                                    }

                                    //Update NoteCategory
                                    if (UpdateNoteCategory.Count > 0)
                                    {
                                        foreach (var item in UpdateNoteCategory)
                                        {
                                            noteCategory = new NoteCategory()
                                            {
                                                MerchantID = item.MerchantID,
                                                SysNoteCategoryID = item.SysNoteCategoryID,
                                                Ordinary = item.Ordinary,
                                                Name = item.Name,
                                                DateCreated = item.DateCreated,
                                                DateModified = item.DateModified,
                                                DataStatus = 'I',
                                                FWaitSending = 1,
                                                WaitSendingTime = DateTime.UtcNow
                                            };
                                            var insertOrreplace = await noteCategoryManage.InsertOrReplaceCategory(noteCategory);

                                            maxNoteCategoryRevision = item.RevisionNo;
                                        }
                                    }
                                }

                                if (allNoteCategory.NoteCategoryBin.Count > 0)
                                {
                                    allNoteCategory.NoteCategoryBin.ToList().OrderBy(x => x.RevisionNo);
                                    maxNoteCategory = allNoteCategory.NoteCategoryBin.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();  
                                    //delete
                                    foreach (var itemDelete in allNoteCategory.NoteCategoryBin)
                                    {
                                        //UpdateItem
                                        var UpdateNote = await noteManage.GetNoteBYCategory(itemDelete.MerchantID, itemDelete.SysNoteCategoryID);
                                        if (UpdateNote != null)
                                        {
                                            foreach (var update in UpdateNote)
                                            {
                                                update.SysNoteCategoryID = null;
                                                var resultUpdate = await noteManage.UpdateNote(update);
                                            }
                                        }
                                        var delete = await noteCategoryManage.DeleteNoteCategory(itemDelete.MerchantID, itemDelete.SysNoteCategoryID);
                                        if (!delete)
                                        {
                                            var data = await noteCategoryManage.GetNoteCategory(itemDelete.MerchantID, itemDelete.SysNoteCategoryID);
                                            if (data != null)
                                            {
                                                data.DataStatus = 'D';
                                                data.FWaitSending = 0;
                                                await noteCategoryManage.UpdateNoteCategory(data);
                                            }
                                        }
                                        maxNoteCategoryRevision = itemDelete.RevisionNo;
                                    }
                                }

                                //progressBar.Progress += 5;//progressBar 95
                                Log.Debug("connectpass", "listRivisionNoteCategory");
                                await UtilsAll.updateRevisionNo((int)listRivision[i].SystemID, maxNoteCategory);
                            }
                            catch (Exception ex)
                            {
                                _ = TinyInsights.TrackErrorAsync(ex);
                                Log.Debug("connecterror", "listRivisionNoteCategory : " + ex.Message);
                                await UtilsAll.ErrorRevisionNo((int)listRivision[i].SystemID, maxNoteCategoryRevision);
                            }
                            #endregion
                            Console.WriteLine("time notecatend" + DateTime.Now);
                            break;
                        case 70:
                            Console.WriteLine("time notestart" + DateTime.Now);
                            #region Note
                            try
                            {
                                var allNote = await GabanaAPI.GetDataNotes((int)listRivision[i].LastRevisionNo, 0);

                                if (allNote == null)
                                {
                                    break;
                                }

                                if (allNote.totalNotes == 0)
                                {
                                    break;
                                }
                                int round = 0, addrount = 0;
                                round = allNote.totalNotes / 100;
                                addrount = round + 1;
                                for (int j = 0; j < addrount; j++)
                                {
                                    allNote = await GabanaAPI.GetDataNotes((int)listRivision[i].LastRevisionNo, j);

                                    if (allNote == null)
                                    {
                                        break;
                                    }

                                    if (allNote.totalNotes == 0)
                                    {
                                        break;
                                    }

                                    allNote.noteWithNoteStatuses.ToList().OrderBy(x => x.note.RevisionNo);
                                    var maxNote = allNote.noteWithNoteStatuses.ToList().Max(x => x.note.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();                                                             

                                    //check ว่ามีไหม
                                    List<Gabana3.JAM.Notes.NoteWithNoteStatus> UpdateNote = new List<Gabana3.JAM.Notes.NoteWithNoteStatus>();
                                    List<Gabana3.JAM.Notes.NoteWithNoteStatus> InsertNote = new List<Gabana3.JAM.Notes.NoteWithNoteStatus>();
                                    List<Note> GetallNote = await noteManage.GetAllNote(DataCashingAll.MerchantId);
                                    UpdateNote.AddRange(allNote.noteWithNoteStatuses.Where(x => GetallNote.Select(y => (long)y.SysNoteID).ToList().Contains(x.note.SysNoteID)).ToList());
                                    InsertNote.AddRange(allNote.noteWithNoteStatuses.Where(x => !(GetallNote.Select(y => (long)y.SysNoteID).ToList().Contains(x.note.SysNoteID)) && x.DataStatus != 'D').ToList()); ;

                                    //Insert Note
                                    if (InsertNote.Count > 0)
                                    {
                                        List<Note> BulkNote = new List<Note>();
                                        foreach (var Note in InsertNote)
                                        {
                                            BulkNote.Add(new Note()
                                            {
                                                MerchantID = Note.note.MerchantID,
                                                SysNoteID = Note.note.SysNoteID,
                                                Ordinary = Note.note.Ordinary,
                                                Message = Note.note.Message,
                                                SysNoteCategoryID = Note.note.SysNoteCategoryID,
                                                LastDateModified = Note.note.LastDateModified,
                                                UserLastModified = Note.note.UserLastModified,
                                                DataStatus = 'I',
                                                FWaitSending = 1,
                                                WaitSendingTime = DateTime.UtcNow
                                            });
                                            maxNoteCategoryRevision = Note.note.RevisionNo;
                                        }

                                        using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                                        {
                                            try
                                            {
                                                await db.BulkCopyAsync(BulkNote);
                                            }
                                            catch (Exception ex)
                                            {
                                                _ = TinyInsights.TrackErrorAsync(ex);
                                                var errorRevison = InsertNote.Select(x => x.note.RevisionNo).Min();
                                                maxNoteRevision = errorRevison;
                                                Log.Error("connecterror", "BulkNote :" + ex.Message);
                                                throw ex;
                                            }
                                        }
                                    }

                                    //Update Note
                                    if (UpdateNote.Count > 0)
                                    {
                                        foreach (var Note in UpdateNote)
                                        {
                                            if (Note.DataStatus == 'D')
                                            {
                                                //delete
                                                var delete = await noteManage.DeleteNote(Note.note.MerchantID, Note.note.SysNoteID);
                                                if (!delete)
                                                {
                                                    var data = await noteManage.GetNote(Note.note.MerchantID, Note.note.SysNoteID);
                                                    if (data != null)
                                                    {
                                                        data.DataStatus = 'D';
                                                        data.FWaitSending = 0;
                                                        await noteManage.UpdateNote(data);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                note = new Note()
                                                {
                                                    MerchantID = Note.note.MerchantID,
                                                    SysNoteID = Note.note.SysNoteID,
                                                    Ordinary = Note.note.Ordinary,
                                                    Message = Note.note.Message,
                                                    SysNoteCategoryID = Note.note.SysNoteCategoryID,
                                                    LastDateModified = Note.note.LastDateModified,
                                                    UserLastModified = Note.note.UserLastModified,
                                                    DataStatus = 'I',
                                                    FWaitSending = 1,
                                                    WaitSendingTime = DateTime.UtcNow
                                                };
                                                var insertOrreplace = await noteManage.InsertOrReplaceNote(note);
                                            }
                                            maxNoteRevision = Note.note.RevisionNo;
                                        }
                                    }

                                    await UtilsAll.updateRevisionNo((int)listRivision[i].SystemID, maxNote);
                                }
                            }
                            catch (Exception ex)
                            {
                                _ = TinyInsights.TrackErrorAsync(ex);
                                Log.Debug("connecterror", "listRivisionNote : " + ex.Message);
                                await UtilsAll.ErrorRevisionNo((int)listRivision[i].SystemID, maxNoteRevision);
                            }
                            Log.Debug("connectpass", "listRivisionNote");

                            #endregion
                            Console.WriteLine("time noteend" + DateTime.Now);
                            break;
                        default:
                            break;
                    }
                    Console.WriteLine("InitialSystemInfo:switch ("+listRivision[i].SystemID.ToString()+")" + DateTime.Now);
                }

                //-----------------------------------------
                //Get tranRunning API
                //------------------------------------------

                #region Tran API
                //TranWithDetailsLocal tranWithDetails = new TranWithDetailsLocal();
                //List<TranWithDetailsLocal> lsttranWithDetails = new List<TranWithDetailsLocal>();
                //TransManage transManage = new TransManage();

                //lsttranWithDetails = new List<TranWithDetailsLocal>();
                //var date = DateTime.UtcNow.ToString("yyyy-MM-dd", null);
                //var tranDetails = await GabanaAPI.GetDataTran("2021-03-02", date, DataCashingAll.SysBranchId, 0);
                //var tranConvert = JsonConvert.DeserializeObject<Gabana3.JAM.Trans.TransResult>(tranDetails.Message);

                //if (tranConvert.totalTrans == 0)
                //{
                //    return;
                //}

                //var config = new MapperConfiguration(cfg =>
                //{
                //    //struct ของ table
                //    cfg.CreateMap<Gabana3.JAM.Trans.TranWithDetails, Model.TranWithDetailsLocal>();
                //    cfg.CreateMap<ORM.Period.Tran, Tran>();
                //    cfg.CreateMap<ORM.Period.TranDetailItem, TranDetailItem>();
                //    cfg.CreateMap<ORM.Period.TranPayment, TranPayment>();
                //});

                //// TranWithDetailsLocal
                //var Imapper = config.CreateMapper();
                //foreach (var i in tranConvert.transWithDetails)
                //{
                //    tranWithDetails = Imapper.Map<Gabana3.JAM.Trans.TranWithDetails, Model.TranWithDetailsLocal>(i);
                //    var InsertToLocal = await transManage.InsertOrReplaceTran(tranWithDetails);
                //} 
                #endregion


            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                //TinyInsights.TrackErrorAsync(ex);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private void CreateView()
        {
            GiftImg = new UIImageView();
            GiftImg.Image = UIImage.FromBundle("GabanaMain.png");
            //GiftImg.Frame = new CGRect(new CGPoint(0, 0), new CGSize(120, 120));
            GiftImg.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(GiftImg);

            GifttxtImg = new UIImageView();
            GifttxtImg.Image = UIImage.FromBundle("GabanaTxt.png");
            GifttxtImg.ContentMode = UIViewContentMode.ScaleAspectFit;
            //GiftImg.Frame = new CGRect(new CGPoint(0, 0), new CGSize(120, 120));
            GifttxtImg.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(GifttxtImg);

            loading = new UIProgressView();
            loading.TranslatesAutoresizingMaskIntoConstraints = false;
            loading.ProgressTintColor = UIColor.FromRGB(0, 149, 218);
            View.AddSubview(loading);

            Reloadimg = new UIImageView();
            Reloadimg.Hidden = true;
            Reloadimg.Image = UIImage.FromBundle("Reconnect");
            //GiftImg.Frame = new CGRect(new CGPoint(0, 0), new CGSize(120, 120));
            Reloadimg.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(Reloadimg);
            Reloadimg.UserInteractionEnabled = true;
            var tapGesture4 = new UITapGestureRecognizer(this,
                   new ObjCRuntime.Selector("btnReconnect:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Reloadimg.AddGestureRecognizer(tapGesture4);

            seniorsoftimg = new UIImageView();
            seniorsoftimg.Image = UIImage.FromBundle("PoweredBySNS.png");
            //GiftImg.Frame = new CGRect(new CGPoint(0, 0), new CGSize(120, 120));
            seniorsoftimg.ContentMode = UIViewContentMode.ScaleAspectFit;
            seniorsoftimg.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(seniorsoftimg);



            lblversion = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblversion.TextColor = UIColor.FromRGB(64, 64, 64);
            lblversion.Font = lblversion.Font.WithSize(12);
            lblversion.TextAlignment = UITextAlignment.Center;
            View.AddSubview(lblversion);
            var version = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
            var version2 = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();
            lblversion.Text = "Version "+version + "." + version2;
        }
        async Task GetDeviceData()
        {
            try
            {
                var Device = await GabanaAPI.GetDataDevice(DataCashingAll.DeviceUDID, DataCashingAll.DevicePlatform);
                var DeviceDetail = JsonConvert.SerializeObject(Device);
                Preferences.Set("Device", DeviceDetail);
                var setDevice = Preferences.Get("Device", "");
                if (setDevice != "")
                {
                    var Config = JsonConvert.DeserializeObject<Device>(setDevice);
                    DataCashingAll.Device = Config;
                }
            }
            catch (Exception ex)
            {
                string text = "GetDeviceData";
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(text);
                Utils.ShowMessage(ex.Message);
                
            }
        }

        async Task GetGiftVoucher()
        {
            try
            {
                GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
                var giftVouchers = await GabanaAPI.GetDataGiftVoucher();
                if (giftVouchers.Count > 0)
                {
                    foreach (var item in giftVouchers)
                    {
                        ORM.MerchantDB.GiftVoucher giftVoucher = new GiftVoucher()
                        {
                            DateCreated = item.DateCreated,
                            DateModified = item.DateModified,
                            FmlAmount = item.FmlAmount,
                            GiftVoucherCode = item.GiftVoucherCode,
                            GiftVoucherName = item.GiftVoucherName,
                            MerchantID = item.MerchantID,
                            Ordinary = item.Ordinary,
                            UserNameModified = item.UserNameModified
                        };
                        var insert = await giftVoucherManage.InsertOrReplaceGiftVoucher(giftVoucher);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
                
            }
        }

        async Task GetMyQRCode()
        {
            try
            {
                MyQrCodeManage QrCodeManage = new MyQrCodeManage();
                List<ORM.Master.MyQrCode> myqrcodes = await GabanaAPI.GetDataMyQrCode();
                if (myqrcodes != null)
                {
                    var lst = myqrcodes.OrderBy(x => x.MyQrCodeNo).ToList();
                    foreach (var item in lst)
                    {
                        ORM.MerchantDB.MyQrCode myQrCode = new ORM.MerchantDB.MyQrCode()
                        {
                            DateCreated = item.DateCreated,
                            DateModified = item.DateModified,
                            MerchantID = item.MerchantID,
                            Ordinary = item.Ordinary,
                            UserNameModified = item.UserNameModified,
                            Comments = item.Comments,
                            FMyQrAllBranch = item.FMyQrAllBranch,
                            MyQrCodeName = item.MyQrCodeName,
                            MyQrCodeNo = item.MyQrCodeNo,
                            PicturePath = item.PicturePath,
                            SysBranchID = item.SysBranchID  // FMyQrAllBranch = 'A' : null,FMyQrAllBranch = 'B' : DataCashingAll.SysBranchId 
                        };
                        await QrCodeManage.InsertOrReplaceMyQrCode(myQrCode);
                        await Utils.InsertLocalPictureQrcode(myQrCode);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
            }
        }
        private async Task ChecKVersionApp() 
        {
            try
            {
                var version = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
                var versionApp = await GabanaAPI.GetDataAppConfig("iOSVersionMinimum");
                float fversion = float.Parse(version);
                float fversionMin = float.Parse(versionApp?.CfgString);
                if (fversion < fversionMin)
                {
                    var storeUrl = "itms-apps://itunes.apple.com/app/id1494882513";
                    UIApplication.SharedApplication.OpenUrl(new NSUrl(storeUrl));
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ChecKVersionApp at Login");
                Utils.ShowMessage(ex.Message);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        async Task GetCashTemplate()
        {
            try
            {
                CashTemplateManage templateManage = new CashTemplateManage();
                if (await GabanaAPI.CheckNetWork())
                {
                    List<ORM.Master.CashTemplate> listcashTemplate = new List<ORM.Master.CashTemplate>();
                    listcashTemplate = await GabanaAPI.GetDataCashTemplate();
                    if (listcashTemplate.Count > 0)
                    {
                        //ลบข้อมูลทั้งหมด
                        var delete = await templateManage.DeleteAllCashTemplatee(DataCashingAll.MerchantId);

                        var lst = new List<CashTemplate>();
                        foreach (var item in listcashTemplate)
                        {
                            CashTemplate cashTemplate = new CashTemplate()
                            {
                                Amount = item.Amount,
                                CashTemplateNo = item.CashTemplateNo,
                                DateModified = item.DateModified,
                                MerchantID = item.MerchantID,
                            };
                            var InsertorReplace = await templateManage.InsertorReplaceCashTemplate(cashTemplate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string text = "GetDeviceData";
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(text);
                Utils.ShowMessage(ex.Message);
            }
        }
        async Task GetmerchantConfig()
        {
            try
            {
                List<MerchantConfig> lstconfig = new List<MerchantConfig>();
                SetMerchantConfig setconfig = new SetMerchantConfig();
                merchantconfigManage = new MerchantConfigManage();
                //Merchant Config
                var listmerchantConfig = await GabanaAPI.GetDataMerchantConfig();
                if (listmerchantConfig != null)
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
                        setconfig.TAXRATE = TAXRATE.CfgFloat.ToString();
                    }

                    var CURRENCY_SYMBOLS = lstconfig.Where(x => x.CfgKey == "CURRENCY_SYMBOLS").FirstOrDefault();
                    if (CURRENCY_SYMBOLS != null)
                    {
                        if (CURRENCY_SYMBOLS.CfgString == "")
                        {
                            CURRENCY_SYMBOLS.CfgString = " ";
                        }
                        setconfig.CURRENCY_SYMBOLS = CURRENCY_SYMBOLS.CfgString;
                    }

                    var DECIMAL_POINT_CALC = lstconfig.Where(x => x.CfgKey == "DECIMAL_POINT_CALC").FirstOrDefault();
                    if (DECIMAL_POINT_CALC != null)
                    {
                        setconfig.DECIMAL_POINT_CALC = DECIMAL_POINT_CALC.CfgInteger.ToString();
                    }

                    var DECIMAL_POINT_DISPLAY = lstconfig.Where(x => x.CfgKey == "DECIMAL_POINT_DISPLAY").FirstOrDefault();
                    if (DECIMAL_POINT_DISPLAY != null)
                    {
                        setconfig.DECIMAL_POINT_DISPLAY = DECIMAL_POINT_DISPLAY.CfgInteger.ToString();
                    }

                    var OPTION_ROUNDING = lstconfig.Where(x => x.CfgKey == "OPTION_ROUNDING").FirstOrDefault();
                    if (OPTION_ROUNDING != null)
                    {
                        setconfig.OPTION_ROUNDING_STRING = OPTION_ROUNDING.CfgString;
                        setconfig.OPTION_ROUNDING_INT = OPTION_ROUNDING.CfgInteger.ToString();
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
                string text = "GetmerchantConfig";
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(text);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        async Task GetMemberTypeData()
        {
            try
            {
                List<MemberType> lstmembertypr = new List<MemberType>();
                membertypeManage = new MemberTypeManage();
                //MemberType
                var listmembertype = await GabanaAPI.GetDataMemberType();
                if (listmembertype != null && listmembertype.Count > 0 )
                {
                    var Allmember = await membertypeManage.DeleteAllMemberType(DataCashingAll.MerchantId);
                    foreach (var item in listmembertype)
                    {
                        MemberType memberType = new MemberType()
                        {
                            DateModified = item.DateModified,
                            LinkProMaxxID = item.LinkProMaxxID,
                            MemberTypeName = item.MemberTypeName,
                            MemberTypeNo = item.MemberTypeNo,
                            MerchantID = item.MerchantID,
                            PercentDiscount = item.PercentDiscount
                        };
                        var InsertorReplace = await membertypeManage.InsertorReplacrMemberType(memberType);
                    }
                }
            }
            catch (Exception ex)
            {
                string text = "GetMemberTypeData";
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(text);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private void setupView()
        {
            try
            {

            
            GiftImg.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor, 0).Active = true;
            GiftImg.HeightAnchor.ConstraintEqualTo(120).Active = true;
            GiftImg.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 120).Active = true;
            GiftImg.WidthAnchor.ConstraintEqualTo(120).Active = true;

            GifttxtImg.TopAnchor.ConstraintEqualTo(GiftImg.SafeAreaLayoutGuide.BottomAnchor, 50).Active = true;
            GifttxtImg.HeightAnchor.ConstraintEqualTo(57).Active = true;
            GifttxtImg.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor, 0).Active = true;
            GifttxtImg.WidthAnchor.ConstraintEqualTo(184).Active = true;

            loading.TopAnchor.ConstraintEqualTo(GifttxtImg.SafeAreaLayoutGuide.BottomAnchor, 20).Active = true;
            loading.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 50).Active = true;
            loading.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -50).Active = true;
            loading.HeightAnchor.ConstraintEqualTo(5).Active = true;

            seniorsoftimg.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -20).Active = true;
            seniorsoftimg.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor, 0).Active = true;
            seniorsoftimg.HeightAnchor.ConstraintEqualTo(29).Active = true;
            seniorsoftimg.WidthAnchor.ConstraintEqualTo(240).Active = true;

            lblversion.TopAnchor.ConstraintEqualTo(seniorsoftimg.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            lblversion.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor, 0).Active = true;
            lblversion.HeightAnchor.ConstraintEqualTo(10).Active = true;
            lblversion.WidthAnchor.ConstraintEqualTo(240).Active = true;

            Reloadimg.BottomAnchor.ConstraintEqualTo(seniorsoftimg.SafeAreaLayoutGuide.TopAnchor, -30).Active = true;
            Reloadimg.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor, 0).Active = true;
            Reloadimg.HeightAnchor.ConstraintEqualTo(30).Active = true;
            Reloadimg.WidthAnchor.ConstraintEqualTo(30).Active = true;

            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
            }

        }
        async Task InsertUserAccount()
        {
            try
            {
                //InsertUserAccount
                List<Gabana.ORM.Master.BranchPolicy> getlstbranchPolicy;
                UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
                DataCashingAll.UserAccountInfo = new List<Model.UserAccountInfo>();
                DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
                getlstbranchPolicy = await GabanaAPI.GetDataBranchPolicy();


                //insert Seauth to local
                foreach (var UserAccountInfo in DataCashingAll.UserAccountInfo)
                {
                    ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                    {
                        MerchantID = UserAccountInfo.MerchantID,
                        UserName = UserAccountInfo.UserName,
                        FUsePincode = merchants.UserAccountInfo.Where(x => x.UserName == UserAccountInfo.UserName).Select(x => (int?)x.FUsePincode).FirstOrDefault() ?? 0,
                        PinCode = merchants.UserAccountInfo.Where(x => x.UserName == UserAccountInfo.UserName).Select(x => x.PinCode).FirstOrDefault(),
                        Comments = merchants.UserAccountInfo.Where(x => x.UserName == UserAccountInfo.UserName).Select(x => x.Comments).FirstOrDefault(),
                    };
                    var insertlocal = await accountInfoManage.InsertorReplaceUserAccount(localUser);

                    //Insert BranchPolicy
                    if (insertlocal)
                    {
                        if (getlstbranchPolicy != null & UserAccountInfo.MainRoles.ToLower() != "owner" & UserAccountInfo.MainRoles.ToLower() != "admin")
                        {
                            List<ORM.Master.BranchPolicy> result = new List<ORM.Master.BranchPolicy>();
                            result = getlstbranchPolicy.Where(x => x.UserName.ToLower() == UserAccountInfo.UserName?.ToLower()).ToList();
                            if (result == null)
                            {
                                return;
                            }
                            foreach (var itembranch in result)
                            {
                                ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                                {
                                    MerchantID = itembranch.MerchantID,
                                    SysBranchID = (int)itembranch.SysBranchID,
                                    UserName = itembranch.UserName?.ToLower(),
                                };
                                BranchPolicyManage policyManage = new BranchPolicyManage();
                                var insertlocalbranchPolicy = await policyManage.InsertorReplacrBranchPolicy(branchPolicy);
                            }
                        }
                        
                    }
                }
                var listEmployees = await accountInfoManage.GetAllUserAccount();

                var Employeeoffline = JsonConvert.SerializeObject(DataCashingAll.UserAccountInfo);
                Preferences.Set("Employeeoffline", Employeeoffline);
                var Employee = Preferences.Get("Employeeoffline", "");
                if (Employee != "")
                {
                    var lstEmployee = JsonConvert.DeserializeObject<List<Model.UserAccountInfo>>(Employee);
                    DataCashingAll.UserAccountInfo = lstEmployee;
                }

                //Local == GabanaAPI เพื่อลบข้อมูล useraccount 
                var getUseraccount = await accountInfoManage.GetAllUserAccount();
                var lstGabanaAPI = merchants.UserAccountInfo;

                HashSet<string> sentIDs = new HashSet<string>(getUseraccount.Select(s => s.UserName));
                var results = lstGabanaAPI.Where(m => !sentIDs.Contains(m.UserName)).ToList();
                if (results.Count > 0)
                {
                    foreach (var item in results)
                    {
                        if (item.UserName.ToLower() == "owner")
                        {
                            break;
                        }
                        //branchPolicy
                        BranchPolicyManage policyManage = new BranchPolicyManage();
                        var getBranchPolicy = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, item.UserName);
                        foreach (var branchPolicy in getBranchPolicy)
                        {
                            var deleteBranchPolicy = await policyManage.DeleteBranch(DataCashingAll.MerchantId, item.UserName);
                        }

                        //Useraccount
                        var deleteUseraccount = await accountInfoManage.DeleteUserAccount(DataCashingAll.MerchantId, item.UserName);
                    }
                }

                ////Local == GabanaAPI เพื่อลบข้อมูล useraccount 
                //var getUseraccount = await accountInfoManage.GetAllUserAccount();
                //var lstGabanaAPI = merchants.UserAccountInfo;

                //HashSet<string> sentIDs = new HashSet<string>(getUseraccount.Select(s => s.UserName));
                //var results = lstGabanaAPI.Where(m => !sentIDs.Contains(m.UserName)).ToList();
                //if (results.Count > 0)
                //{
                //    foreach (var item in results)
                //    {
                //        //branchPolicy
                //        BranchPolicyManage policyManage = new BranchPolicyManage();
                //        var getBranchPolicy = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, item.UserName);
                //        foreach (var branchPolicy in getBranchPolicy)
                //        {
                //            var deleteBranchPolicy = await policyManage.DeleteBranch(DataCashingAll.MerchantId, item.UserName);
                //        }

                //        //Useraccount
                //        var deleteUseraccount = await accountInfoManage.DeleteUserAccount(DataCashingAll.MerchantId, item.UserName);
                //    }
                //}

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
    }
}