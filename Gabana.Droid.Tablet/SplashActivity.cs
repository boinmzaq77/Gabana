using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using BellNotificationHub.Xamarin.Android;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB;
using LinqToDB.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using TinyInsightsLib.ApplicationInsights;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Launcher", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class SplashActivity : AppCompatActivity
    {
        SplashActivity main;
        ProgressBar progressBar;
        ImageButton btnReconnect;
        int numreconnect = 0;
        string error;
        public Java.IO.File FileDir;
        public const string TAG = "SplashActivity";
        internal static readonly string CHANNEL_ID = "GabanaChannel";
        public List<Gabana3.JAM.Items.ItemWithItemExSizes> listItemAPI;
        public List<Item> listItemLocal;
        SystemInfoManage SystemInfoManage;
        SystemRevisionNoManage systemRevisionNoManage;
        SystemRevisionNo SystemRevisionNo;
        DeviceSystemSeqNoManage systemSeqNoManage;
        DeviceSystemSeqNo deviceSystemSeq;
        List<SystemRevisionNo> listRivision;
        List<SystemInfo> lstSystemInfo;
        CategoryManage categoryManage;
        Category category;
        CustomerManage customerManage;
        Customer customer;
        ItemManage itemManage;
        ItemExSizeManage itemExSizeManage;
        Item getItem;
        ItemExSize getitemSize;
        DiscountTemplateManage discountTemplateManage;
        DiscountTemplate discount;
        DeviceSystemSeqNo deviceSystemSeqNo;
        DeviceTranRunningNoManage deviceTranRunningNoManage;
        DeviceTranRunningNo deviceTranRunning;
        Note note;
        NoteManage noteManage;
        NoteCategory noteCategory;
        NoteCategoryManage noteCategoryManage;
        MerchantConfigManage merchantconfigManage;
        MemberTypeManage membertypeManage;
        ItemOnBranchManage onBranchManage;
        ORM.Master.SystemRevisionNo revisionNo;
        public static Gabana3.JAM.Merchant.Merchants merchants = new Gabana3.JAM.Merchant.Merchants();
        bool deviceAsleep = false;
        bool openPage = false,  CheckResult = false;
        public DateTime pauseDate = DateTime.Now;
        Task[] tasksItem = new Task[2];
        Task[] tasksCustomer = new Task[2];

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                SetTheme(Resource.Style.AppTheme_Main);
                base.OnCreate(savedInstanceState);

                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.splash_screen);
                main = this;

                IsPlayServicesAvailable();
                CreateNotificationChannel();

                //Set Flag ไม่ให้ notification ทำงาน ตอนที่โหลดข้อมูลที่หน้า Splash
                DataCashingAll.flagNotificationEnble = true;
#if (DEBUG)
                var appInsightsProvider = new ApplicationInsightsProvider("30fdd00a-0dcb-4dd5-be28-062f8765466b", "GabanaTabletAppLab");
#else
                var appInsightsProvider = new ApplicationInsightsProvider("30fdd00a-0dcb-4dd5-be28-062f8765466b", "GabanaTabletApp");
#endif
                appInsightsProvider.LogLevel = TinyInsightsLib.TinyLogLevel.Infomation;
                TinyInsightsLib.TinyInsights.Configure(appInsightsProvider);

                FirebaseDeviceToken firebaseDeviceToken = new FirebaseDeviceToken();
                firebaseDeviceToken.KeepDeviceToken();

                progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
                btnReconnect = FindViewById<ImageButton>(Resource.Id.btnReconnect);
                btnReconnect.Visibility = ViewStates.Invisible;
                if (error != null)
                {
                    error = null;
                    btnReconnect.Visibility = ViewStates.Visible;
                }
                btnReconnect.Click += BtnReconnect_Click;

                JobQueue.Default = new JobQueue(this);

                //Get UDID Set Platform
                DataCashingAll.DeviceUDID = Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                DataCashingAll.DevicePlatform = "FCM";
                Preferences.Set("DeviceUDID", DataCashingAll.DeviceUDID);

                Log.Debug("TAGSPLASH", $"CreatSpash");

                TextView textVersion = FindViewById<TextView>(Resource.Id.textVersion);
                var vername = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName?.ToString();
                long vernumber = 0;
                vernumber = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionCode;
                textVersion.Text = "Version " + vername + "." + vernumber;

                //check version ปัจจุบัน กับ pre vername
                string PreverName = Preferences.Get("VersionName", vername);

                decimal.TryParse(vername, out decimal verNow);
                decimal.TryParse(PreverName, out decimal verpreference);
                if (verNow > verpreference)
                {
                    StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
                    return;
                }
                _ = TinyInsights.TrackPageViewAsync("OnCreate : SplashActivity");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Splash");
                Toast.MakeText(this, "OnCreate at Splash :" + ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnReconnect_Click(object sender, EventArgs e)
        {
            progressBar.Progress = 0;

            btnReconnect.Visibility = ViewStates.Invisible;

            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }
        protected async override void OnResume()
        {
            try
            {
                base.OnResume();
                if (!main.IsFinishing)
                {
                    Task startupWork = new Task(() => { SimulateStartup(); });
                    startupWork.Start();
                    Android.Util.Log.Debug("stateCycle", "OnResume at splash");
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at Splash");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                base.OnRestart();
            }
        } 
        private void SimulateStartup()
        {
            try
            {
                RunOnUiThread(() =>
                {
                    progressBar.Max = 100;
                    progressBar.Progress = 0;
                    progressBar.Visibility = ViewStates.Visible;
                    btnReconnect.Visibility = ViewStates.Invisible;
                });

                //set langueage 
                Android.Content.Res.Configuration conf = Resources.Configuration;

                string currentLanguage = Preferences.Get("Language", "");

                if (string.IsNullOrEmpty(currentLanguage))
                {
                    if (Resources.Configuration.Locale.Language.ToString() == "th")
                    {
                        conf.SetLocale(new Java.Util.Locale("th"));
                        conf.SetLayoutDirection(new Java.Util.Locale("en"));
                        Resources.UpdateConfiguration(conf, Resources.DisplayMetrics);
                        Preferences.Set("Language", "th");
                        DataCashing.Language = "th";

                    }
                    else
                    {
                        conf.SetLocale(new Java.Util.Locale(Resources.Configuration.Locale.Language.ToString()));
                        conf.SetLayoutDirection(new Java.Util.Locale(Resources.Configuration.Locale.Language.ToString()));
                        Resources.UpdateConfiguration(conf, Resources.DisplayMetrics);
                        Preferences.Set("Language", "en");
                        DataCashing.Language = "en";
                    }
                }
                else
                {

                    if (Preferences.Get("Language", "") == "en")
                    {
                        conf.SetLocale(new Java.Util.Locale("en"));
                        conf.SetLayoutDirection(new Java.Util.Locale("en"));
                        Resources.UpdateConfiguration(conf, Resources.DisplayMetrics);
                        DataCashing.Language = "en";
                    }
                    else
                    {
                        conf.SetLocale(new Java.Util.Locale("th"));
                        conf.SetLayoutDirection(new Java.Util.Locale("en"));
                        Resources.UpdateConfiguration(conf, Resources.DisplayMetrics);
                        DataCashing.Language = "th";
                    }
                }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    //เข้าขั้นแรก , เน้ตช้า , ไม่มีเน็ต
                    //if (!await GabanaAPI.CheckSpeedConnection())
                    //{
                    //    TokenResult res = await TokenServiceBase.GetToken();
                    //    if (res != null && res.status)
                    //    {
                    //        Connectoffine();
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                    //        this.Finish();
                    //        return;
                    //    }
                    //}
                    //else
                    //{
                    //    TokenResult res = await TokenServiceBase.GetToken();
                    //    if (res != null && res.status)
                    //    {
                    //        await ConnectOnline();
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                    //        this.Finish();
                    //        return;
                    //    }
                    //}

                    bool hasInternetConnection = await GabanaAPI.CheckSpeedConnection();
                    TokenResult res = await TokenServiceBase.GetToken();

                    if (res != null && res.status)
                    {
                        if (!hasInternetConnection)
                        {
                            Connectoffine();
                        }
                        else
                        {
                            await ConnectOnline();
                        }
                    }
                    else
                    {
                        StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                        Finish();
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                RunOnUiThread(() =>
                {
                    btnReconnect.Visibility = ViewStates.Visible;
                });
                numreconnect++;
            }
            if (numreconnect > 2)
            {
                progressBar.Progress = 100;
                StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                numreconnect = 0;
                this.Finish();
            }
        }
        async Task InsertUserAccount()
        {
            try
            {
                //InsertUserAccount
                List<Gabana.ORM.Master.BranchPolicy> getlstbranchPolicy = new List<ORM.Master.BranchPolicy>(); ;
                UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
                DataCashingAll.UserAccountInfo = new List<Model.UserAccountInfo>();
                DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
                getlstbranchPolicy = await GabanaAPI.GetDataBranchPolicy();

                if (DataCashingAll.UserAccountInfo.Count > 0 & getlstbranchPolicy.Count > 0)
                {
                    //insert Seauth to local
                    foreach (var UserAccountInfo in DataCashingAll.UserAccountInfo)
                    {
                        ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                        {
                            MerchantID = UserAccountInfo.MerchantID,
                            UserName = UserAccountInfo.UserName?.ToLower(),
                            FUsePincode = merchants.UserAccountInfo.Where(x => x.UserName.ToLower() == UserAccountInfo.UserName?.ToLower()).Select(x => (int?)x.FUsePincode).FirstOrDefault() ?? 0,
                            PinCode = merchants.UserAccountInfo.Where(x => x.UserName.ToLower() == UserAccountInfo.UserName?.ToLower()).Select(x => x.PinCode).FirstOrDefault(),
                            Comments = merchants.UserAccountInfo.Where(x => x.UserName.ToLower() == UserAccountInfo.UserName?.ToLower()).Select(x => x.Comments).FirstOrDefault(),
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
                                    break;
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

                        #region Insert Useraccount Seauth to GabanaAPI เพื่อให้ข้อมูลตรงกันก่อน
                        // ข้อมูล Seauth เช็คกับ GabaAPI
                        //บันทึกลง gabana //branch policy null ไม่ได้ = สำนักงานใหญ่


                        //Insert To GabanaAPI
                        //List<ORM.Master.BranchPolicy> lstbranchPolicies = new List<ORM.Master.BranchPolicy>();
                        //List<ORM.Master.Branch> lstMerchantBranch = new List<ORM.Master.Branch>();
                        //BranchManage branchManage = new BranchManage();

                        ////MainRole Admin Owner
                        //var getBranch = await branchManage.GetAllBranch(UserAccountInfo.MerchantID);
                        //if (getBranch.Count > 0)
                        //{
                        //    foreach (var item in getBranch)
                        //    {
                        //        ORM.Master.BranchPolicy branchPolicy = new ORM.Master.BranchPolicy()
                        //        {
                        //            MerchantID = UserAccountInfo.MerchantID,
                        //            UserName = UserAccountInfo.UserName,
                        //            SysBranchID = (int)item.SysBranchID
                        //        };
                        //        lstbranchPolicies.Add(branchPolicy);
                        //    }

                        //    ORM.Master.UserAccountInfo gbnAPIUser = new ORM.Master.UserAccountInfo()
                        //    {
                        //        MerchantID = UserAccountInfo.MerchantID,
                        //        UserName = UserAccountInfo.UserName,
                        //        FUsePincode = 0,
                        //        PinCode = null,
                        //        Comments = null,
                        //    };

                        //    Gabana3.JAM.UserAccount.UserAccountResult userAccountResult = new Gabana3.JAM.UserAccount.UserAccountResult()
                        //    {
                        //        branchPolicy = lstbranchPolicies,
                        //        userAccountInfo = gbnAPIUser
                        //    };

                        //    var postgbnAPIUser = await GabanaAPI.PostDataUserAccount(userAccountResult);
                        //}
                        #endregion
                    }

                    var Employeeoffline = JsonConvert.SerializeObject(DataCashingAll.UserAccountInfo);
                    Preferences.Set("Employeeoffline", Employeeoffline);
                    var Employee = Preferences.Get("Employeeoffline", "");
                    if (Employee != "")
                    {
                        var lstEmployee = JsonConvert.DeserializeObject<List<Model.UserAccountInfo>>(Employee);
                        DataCashingAll.UserAccountInfo = lstEmployee;
                    }

                    //Local == GabanaAPI เพื่อลบข้อมูล useraccount 
                    List<ORM.MerchantDB.UserAccountInfo> getUseraccount = new List<ORM.MerchantDB.UserAccountInfo>();
                    List<ORM.Master.UserAccountInfo> lstGabanaAPI = new List<ORM.Master.UserAccountInfo>();
                    getUseraccount = await accountInfoManage.GetAllUserAccount();
                    lstGabanaAPI = merchants.UserAccountInfo;

                    if (getUseraccount[0].MerchantID == 0)
                    {
                        _ = TinyInsights.TrackPageViewAsync("InsertUserAccount :" + getUseraccount[0].Comments);
                        return;
                    }

                    HashSet<string> sentIDs = new HashSet<string>(getUseraccount.Select(s => s.UserName.ToLower()));
                    var results = lstGabanaAPI.Where(m => !sentIDs.Contains(m.UserName.ToLower())).ToList();
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
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertUserAccount");
                Log.Error("connecterror", "InsertUserAccount : " + ex.Message);
                throw;
            }
        }

        private async Task ConnectOnline()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (res == null)
                {
                    StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
                }

                if (res.status)
                {
                    Log.Debug("connectpass", "res.status");
                    progressBar.Progress += 5;
                    GabanaAPI.gbnJWT = res.gbnJWT;
                    Log.Debug("connectpass", "GabanaAPI.gbnJWT");

                    merchants = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
                    Log.Debug("connectpass", "merchants");
                    //Update Merchant
                    if (merchants == null)
                    {
                        //เพิ่มหลอด progrees ตามจังหวะการโหลดหรือเช็คข้อมูล 
                        progressBar.Progress += 95;
                        //ส่ง Merchant Detail ไปที่หน้า Update Merchant
                        var IntentUpdateMerchant = new Intent(this, typeof(UpdateProfileActivity));
                        DataCashingAll.Merchant = merchants;
                        StartActivity(IntentUpdateMerchant);
                    }
                    else
                    {
                        //--------------------------------------
                        //Notification
                        //--------------------------------------

                        if (merchants.Merchant != null)
                        {
                            Log.Debug("connectpass", "merchants.Merchant != null");
                            if (merchants.Merchant.MerchantID == 0 || merchants.Device.DeviceNo == 0)
                            {
                                _ = TinyInsights.TrackEventAsync("merchants.Merchant.MerchantID || merchants.Device.DeviceNo = 0 :");
                                StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                                this.Finish();
                                return;
                            }
                            Log.Debug("connectpass", "merchants.Merchant.MerchantID == 0 || merchants.Device.DeviceNo == 0");

                            BellNotificationHelper.RegisterBellNotification(this, GabanaAPI.gbnJWT, merchants.Merchant.MerchantID, merchants.Device.DeviceNo);
                            
                            _ = TinyInsights.TrackEventAsync("RegisterBellNotification Success :");
                            Log.Debug("TAGSPLASH2", $"RegisNotisplash");
                            Log.Debug("connectpass", "BellNotificationHelper");
                        }

                        if (!DataCashing.flagProgress)
                        {
                            DataCashingAll.Merchant = merchants;
                            var jsonmerchants = JsonConvert.SerializeObject(merchants);
                            Preferences.Set("Merchant", jsonmerchants);
                            _ = TinyInsights.TrackEventAsync("Preferences Merchant Success : " );
                            Log.Debug("connectpass", "Preferences Merchant Success");
                            DataCashingAll.MerchantId = Preferences.Get("MerchantID", 0);
                            var device = merchants.Device.DeviceNo;
                            Preferences.Set("DeviceNo", device);
                            DataCashingAll.DeviceNo = device;
                            Log.Debug("connectpass", "DataCashingAll.DeviceNo");

                            if (merchants.Merchant.FPrivacyPolicy != 'Y' || merchants.Merchant.FTermConditions != 'Y')
                            {
                                DataCashing.flagProgress = false;
                                StartActivity(new Intent(Application.Context, typeof(TermActivity)));
                                TermActivity.Setpage("Splash");
                                return;
                            }

                            FileDir = this.FilesDir;
                            DataCashingAll.MerchantId = merchants.Merchant.MerchantID;

                            //เช็คเผื่อไม่ต้องเข้าซ้ำ เนื่องจาก ไม่ต้องสร้างใหม่
                            CreateNewMerchant newMerchant = new CreateNewMerchant();
                            var create = Preferences.Get("CreateDB", "");
                            if (!string.IsNullOrEmpty(create))
                            {
                                newMerchant = JsonConvert.DeserializeObject<CreateNewMerchant>(create);
                                _ = TinyInsights.TrackEventAsync("CreateDB Success : ");
                            }

                            bool CheckResult;
                            if (newMerchant.createNew || string.IsNullOrEmpty(create))
                            {
                                //create Folder Merchant.DB
                                CheckResult = await Utils.CreateMerchnatDB(merchants.Merchant.MerchantID, FileDir);
                                _ = TinyInsights.TrackEventAsync("CreateMerchnatDB Success : ");
                                if (!CheckResult)
                                {
                                    await Utils.DeleteMerchnatDB(merchants.Merchant.MerchantID, FileDir);
                                    ReloadReConnect();
                                    return;
                                }
                                CheckResult = await Utils.CreatePoolDB(merchants.Merchant.MerchantID, FileDir);
                                _ = TinyInsights.TrackEventAsync("CreatePoolDB Success : "  );
                                if (!CheckResult)
                                {
                                    await Utils.DeletePoolDB(merchants.Merchant.MerchantID, FileDir);
                                    ReloadReConnect();
                                    return;
                                }
                                //create Folder image
                                CheckResult = await Utils.CreateLocalImage(merchants.Merchant.MerchantID);
                                _ = TinyInsights.TrackEventAsync("CreateLocalImage Success : ");
                                if (!CheckResult)
                                {
                                    ReloadReConnect();
                                    return;
                                }
                                CheckResult = await Utils.CreateThumbnailLocalImage(merchants.Merchant.MerchantID);
                                _ = TinyInsights.TrackEventAsync("CreateThumbnailLocalImage Success : ");
                                if (!CheckResult)
                                {
                                    ReloadReConnect();
                                    return;
                                }
                                CheckResult = await Utils.CreateDownloadBill(merchants.Merchant.MerchantID);
                                _ = TinyInsights.TrackEventAsync("CreateDownloadBill Success : ");
                                if (!CheckResult)
                                {
                                    ReloadReConnect();
                                    return;
                                }
                                CreateNewMerchant createNewMerchant = new CreateNewMerchant()
                                {
                                    createNew = false,
                                    MerchantID = DataCashingAll.MerchantId,
                                };
                                var createNewDB = JsonConvert.SerializeObject(createNewMerchant);
                                Preferences.Set("CreateDB", createNewDB);
                                _ = TinyInsights.TrackEventAsync("createNewDB Success : ");
                                progressBar.Progress += 5;//progressBar 10  
                            }
                            else
                            {
                                progressBar.Progress += 5;//progressBar 10                 
                            }

                            #region Check Update Database
                            //Check Update Database 
                            DataBaseInfoManage dataBaseInfoManage = new DataBaseInfoManage();
                            DataBaseInfo datainfo = new DataBaseInfo();
                            datainfo = await dataBaseInfoManage.GetDatabaseInfo();
                            if (datainfo == null)
                            {
                                //create Folder Merchant.DB
                                await Utils.CreateMerchnatDB(merchants.Merchant.MerchantID, FileDir);
                                await Utils.CreatePoolDB(merchants.Merchant.MerchantID, FileDir);
                                //create Folder image
                                await Utils.CreateLocalImage(merchants.Merchant.MerchantID);
                                await Utils.CreateThumbnailLocalImage(merchants.Merchant.MerchantID);
                                await Utils.CreateDownloadBill(merchants.Merchant.MerchantID);
                                CreateNewMerchant createNewMerchant = new CreateNewMerchant()
                                {
                                    createNew = false,
                                    MerchantID = DataCashingAll.MerchantId,
                                };
                                var createNewDB = JsonConvert.SerializeObject(createNewMerchant);
                                Preferences.Set("CreateDB", createNewDB);
                            }
                            else
                            {
                                //DatabaseInfo
                                //Get DatabaseInfo/Version จะ response List<ScriptAlterMerchantDB> เรียงลำดับตามเลข er revision และ detailno
                                //ให้วน alter จนครบและเอา ErVersionDBInfo ของรายการสุดท้ายมาอัพเดตที่ MerchantDB
                                string erVersion = datainfo.DataDBInfo;

                                List<Gabana.ORM.Master.ScriptAlterMerchantDB> scriptAlterMerchants = new List<ORM.Master.ScriptAlterMerchantDB>();
                                scriptAlterMerchants = await GabanaAPI.GetDataVersion(erVersion);
                                if (scriptAlterMerchants != null)
                                {
                                    bool AlterDB = await dataBaseInfoManage.AlterDatabaseInfo(datainfo, scriptAlterMerchants);
                                }
                                //กรณีถัดไป ถ้า Alter สำเร็จ?                                
                            }
                            #endregion

                            Log.Debug("connectpass", "Database");
                            _ = TinyInsights.TrackEventAsync("UpdateDatabase Success");
                            //Insert Merchant to LocalDB
                            var result = false;
                            string pathClound = merchants.Merchant.LogoPath;

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

                            MerchantManage merchantManage = new MerchantManage();
                            var merchantlocal = await merchantManage.GetMerchant(merchants.Merchant.MerchantID);
                            if (merchantlocal == null)
                            {
                                string path = await Utils.InsertLocalPictureMerchantMaster(pathClound);
                                merchant.LogoLocalPath = path;
                                result = await merchantManage.InsertMerchant(merchant);
                            }
                            else
                            {
                                string logoPath = string.Empty;
                                logoPath = Utils.SplitCloundPath(merchantlocal?.LogoPath);

                                if ((logoPath?.ToString() != Utils.SplitCloundPath(merchants?.Merchant.LogoPath)) || (string.IsNullOrEmpty(merchantlocal.LogoLocalPath)))
                                {
                                    string path = await Utils.InsertLocalPictureMerchantMaster(pathClound);
                                    merchant.LogoLocalPath = path;
                                }
                                else
                                {
                                    merchant.LogoLocalPath = merchantlocal.LogoLocalPath;
                                }
                                result = await merchantManage.UpdateMerchant(merchant);
                            }

                            if (!result)
                            {
                                RunOnUiThread(() =>
                                {
                                    btnReconnect.Visibility = ViewStates.Visible;
                                });

                                numreconnect++;
                            }

                            var GETmerchantlocal = await merchantManage.GetMerchant(merchants.Merchant.MerchantID);
                            if (GETmerchantlocal.LogoPath != pathClound)
                            {
                                await Utils.InsertLocalPictureMerchant(GETmerchantlocal);
                            }
                            Log.Debug("connectpass", "Insert/Update Merchant");
                            _ = TinyInsights.TrackEventAsync("Manage Merchant Success");

                            DataCashingAll.MerchantLocal = GETmerchantlocal;
                            Preferences.Set("MerchantID", (int)GETmerchantlocal.MerchantID);
                            DataCashingAll.MerchantId = Preferences.Get("MerchantID", 0);

                            _ = TinyInsights.TrackEventAsync( "DeviceUDID :" + DataCashingAll.DeviceUDID?.ToString() + " MerchantId :" + DataCashingAll.MerchantId);

                            //Register
                            if (newMerchant.createNew)
                            {
                                _ = TinyInsights.TrackEventAsync("Register funtion");
                                //insert list Branch 
                                await InsertBranchtoLocal();
                                _ = TinyInsights.TrackEventAsync("InsertBranchtoLocal Success");

                                //InsertorReplace MerchantConfig
                                await GetmerchantConfig();                               
                                _ = TinyInsights.TrackEventAsync("GetmerchantConfig Success");

                                //GetDevice
                                await GetDeviceData();
                                _ = TinyInsights.TrackEventAsync("GetDeviceData Success");

                                //Insert UserAccount
                                await InsertUserAccount();
                                _ = TinyInsights.TrackEventAsync("InsertUserAccount Success");

                                progressBar.Progress += 5;//progressBar 15 
                                await RegisterInitialSystemInfo();
                                _ = TinyInsights.TrackEventAsync("RegisterInitialSystemInfo Success");

                                progressBar.Progress += 35;//progressBar 50    

                                //owner                            
                                if (merchants.Branch.Count == 1)
                                {
                                    //owner
                                    string BranchID = merchants.Branch[0].SysBranchID.ToString();
                                    Preferences.Set("Branch", BranchID);
                                    DataCashingAll.SysBranchId = (int)merchants.Branch[0].SysBranchID;
                                    await SetDataDeviceTranRunningNo();
                                    StartActivity(typeof(MainActivity));
                                    DataCashingAll.flagNotificationEnble = false;
                                    progressBar.Progress += 50;
                                    this.Finish();
                                    return;
                                }
                            }

                            //Login
                            //ลบลงใหม่ และเข้าใหม่
                            BranchManage branchManage = new BranchManage();

                            string branch = Preferences.Get("Branch", "");
                            if (string.IsNullOrEmpty(branch))
                            {
                                _ = TinyInsights.TrackEventAsync("Login Clean App funtion");

                                //insert list Branch 
                                await InsertBranchtoLocal();
                                _ = TinyInsights.TrackEventAsync("InsertBranchtoLocal Success");

                                //InsertorReplace MerchantConfig
                                await GetmerchantConfig();
                                _ = TinyInsights.TrackEventAsync("GetmerchantConfig Success");

                                //GetDevice
                                await GetDeviceData();
                                _ = TinyInsights.TrackEventAsync("GetDeviceData Success");

                                //Insert UserAccount
                                await InsertUserAccount();
                                _ = TinyInsights.TrackEventAsync("InsertUserAccount Success branch is empty");

                                progressBar.Progress += 5;//progressBar 15  

                                string LoginType = Preferences.Get("LoginType", "");
                                string Username = Preferences.Get("User", "");

                                if (string.IsNullOrEmpty(LoginType) || string.IsNullOrEmpty(Username))
                                {
                                    StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                                    DataCashingAll.flagNotificationEnble = false;
                                    this.Finish();
                                    return;
                                }

                                List<BranchPolicy> lstuserBranch = new List<BranchPolicy>();
                                List<ORM.MerchantDB.Branch> lstBranch = new List<ORM.MerchantDB.Branch>();

                                if (DataCashingAll.UserAccountInfo != null)
                                {
                                    string Login = LoginType.ToLower();
                                    if (Login == "owner" | Login == "admin")
                                    {
                                        //owner & admin                                        
                                        lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
                                        //มีสาขา = 1
                                        if (lstBranch.Count == 1)
                                        {
                                            //owner

                                            string BranchID = lstBranch[0].SysBranchID.ToString();
                                            Preferences.Set("Branch", BranchID);
                                            DataCashingAll.SysBranchId = (int)lstBranch[0].SysBranchID;
                                        }
                                        else
                                        {
                                            //มีสาขามากกว่า 1
                                            StartActivity(typeof(SelectBranchActivity));
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        BranchPolicyManage branchPolicyManage = new BranchPolicyManage();
                                        lstuserBranch = await branchPolicyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, Username);
                                        //มีสาขา = 1
                                        if (lstuserBranch.Count == 1)
                                        {
                                            //ตามพนักงาน
                                            string BranchID = lstuserBranch[0].SysBranchID.ToString();
                                            Preferences.Set("Branch", BranchID);
                                            DataCashingAll.SysBranchId = (int)lstuserBranch[0].SysBranchID;
                                        }
                                        else
                                        {
                                            //มีสาขามากกว่า 1
                                            StartActivity(typeof(SelectBranchActivity));
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    await InsertUserAccount();
                                    _ = TinyInsights.TrackEventAsync("InsertUserAccount Success");
                                }
                            }
                            DataCashing.flagProgress = true;
                        }

                        progressBar.Progress = 15;

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
                                    PRINTTYPE = "Image",
                                    COMMAND = "Epson Command",
                                };
                            }
                        }

                        await GetmerchantConfig();
                        _ = TinyInsights.TrackEventAsync("GetmerchantConfig Success");
                        await InsertUserAccount();
                        _ = TinyInsights.TrackEventAsync("InsertUserAccount Success");
                        await GetMemberTypeData();
                        _ = TinyInsights.TrackEventAsync("GetMemberTypeData Success");
                        await GetDeviceData();
                        _ = TinyInsights.TrackEventAsync("GetDeviceData Success");
                        await GetGiftVoucher();
                        _ = TinyInsights.TrackEventAsync("GetGiftVoucher Success");                        
                        await GetMyQRCode();
                        _ = TinyInsights.TrackEventAsync("GetMyQRCode Success");
                        await GetCashTemplate();
                        _ = TinyInsights.TrackEventAsync("GetCashTemplate Success");
                        progressBar.Progress += 5;//progressBar 20
                        await InitialSystemInfo();//progressBar 100
                        _ = TinyInsights.TrackEventAsync("InitialSystemInfo Success");

                        if (tasksItem[0] == null) tasksItem[0] = Task.FromResult(0);
                        if (tasksItem[1] == null) tasksItem[1] = Task.FromResult(0);
                        if (tasksCustomer[0] == null) tasksCustomer[0] = Task.FromResult(0);
                        if (tasksCustomer[1] == null) tasksCustomer[1] = Task.FromResult(0);

                        Task.Factory.ContinueWhenAll(tasksItem, completedTasks => Utils.CheckImageLoadnotCompleteItem());
                        Task.Factory.ContinueWhenAll(tasksCustomer, completedTasks => Utils.CheckImageLoadnotCompleteCustomer());

                        Log.Debug("connectpass", "InsertLocalPictureMerchant End " + "Time : "  );

                        StartActivity(typeof(MainActivity));
                        DataCashingAll.flagNotificationEnble = false;
                        this.Finish();
                        return;
                    }
                }
                else
                {
                    if (Preferences.Get("AppState", "") != "login") //logout
                    {
                        progressBar.Progress += 100;
                        StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                        numreconnect = 0;
                        DataCashingAll.flagNotificationEnble = false;
                        this.Finish();
                    }
                    RunOnUiThread(() =>
                    {
                        btnReconnect.Visibility = ViewStates.Visible;
                    });
                    numreconnect++;
                    if (numreconnect > 2)
                    {
                        progressBar.Progress = 100;
                        StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                        numreconnect = 0;
                        DataCashingAll.flagNotificationEnble = false;
                        this.Finish();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ConnectOnline at Splash");

                RunOnUiThread(() =>
                {
                    btnReconnect.Visibility = ViewStates.Visible;
                });

                numreconnect++;
            }

            if (numreconnect > 2)
            {
                progressBar.Progress = 100;
                Preferences.Set("AppState", "logout");
                StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                numreconnect = 0;
                DataCashingAll.flagNotificationEnble = false;
                this.Finish();
            }
        }
        private async void Connectoffine()
        {
            try
            {
                var Jwt = Preferences.Get("gbnJWT", "");
                if (string.IsNullOrEmpty(Jwt))
                {
                    StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                    numreconnect = 0;
                    DataCashingAll.flagNotificationEnble = false;
                    this.Finish();
                    return;
                }
                GabanaAPI.gbnJWT = Jwt;

                //Local Data
                DataCashingAll.Pathdb = Preferences.Get("PathMerchantDB", "");
                DataCashingAll.Pathdbpool = Preferences.Get("PathPoolDB", "");
                DataCashingAll.PathFolderImage = Preferences.Get("PathFolderImage", "");
                DataCashingAll.PathThumnailFolderImage = Preferences.Get("PathThumnailFolderImage", "");
                DataCashingAll.PathImageBill = Preferences.Get("PathImageBill", "");

                progressBar.Progress += 10;

                //Merchant
                if (DataCashingAll.Merchant == null)
                {
                    var jsonmerchants = Preferences.Get("Merchant", "");
                    var Merchant = JsonConvert.DeserializeObject<Gabana3.JAM.Merchant.Merchants>(jsonmerchants);
                    DataCashingAll.Merchant = Merchant;
                }
                MerchantManage merchantManage = new MerchantManage();
                var merchantid = Preferences.Get("MerchantID", 0);
                DataCashingAll.MerchantId = merchantid;
                var GETmerchantlocal = await merchantManage.GetMerchant(merchantid);
                DataCashingAll.MerchantLocal = GETmerchantlocal;

                progressBar.Progress += 20;

                //Merchant Config                
                if (DataCashingAll.setmerchantConfig == null)
                {
                    var setmerchantConfig = Preferences.Get("SetmerchantConfig", "");
                    var Config = JsonConvert.DeserializeObject<SetMerchantConfig>(setmerchantConfig);
                    DataCashingAll.setmerchantConfig = Config;
                }

                progressBar.Progress += 20;

                //Useraccount                
                if (DataCashingAll.UserAccountInfo == null)
                {
                    var Employee = Preferences.Get("Employeeoffline", "");
                    var lstEmployee = JsonConvert.DeserializeObject<List<Model.UserAccountInfo>>(Employee);
                    DataCashingAll.UserAccountInfo = lstEmployee;
                }

                progressBar.Progress += 10;

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
                            PRINTTYPE = "Image",
                            COMMAND = "Epson Command",
                        };
                    }
                }

                progressBar.Progress += 10;

                //Branch
                string branchID = Preferences.Get("Branch", "");
                int.TryParse(branchID, out int result);
                if (result == 0)
                {
                    DataCashingAll.SysBranchId = 1;
                }
                else
                {
                    DataCashingAll.SysBranchId = result;
                }
                BranchManage branchManage = new BranchManage();
                var branchData = await branchManage.GetBranch(merchantid, DataCashingAll.SysBranchId);
                if (branchData != null)
                {
                    DataCashing.branchDeatail = branchData;
                }
                progressBar.Progress += 30;

                if (Preferences.Get("AppState", "") != "login")
                {
                    StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                    numreconnect = 0;
                    DataCashingAll.flagNotificationEnble = false;
                    this.Finish();
                }
                else
                {
                    //ใช้ข้อมูลจาก localbase  
                    Toast.MakeText(main, "ขณะ Offline จะไม่สามารถบางฟังก์ชันได้ ", ToastLength.Short).Show();
                    StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                    DataCashingAll.flagNotificationEnble = false;
                    this.Finish();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Connectoffine");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public async Task InsertBranchtoLocal()
        {
            try
            {
                MerchantManage merchantManage = new MerchantManage();
                BranchManage branchManage = new BranchManage();

                var getMerchant = await merchantManage.GetMerchant(merchants.Merchant.MerchantID);
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
                        DataCashing.branchDeatail = insertBranch;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("insertBranch");
                Log.Error("connecterror", "InsertBranchtoLocal : " + ex.Message);
                throw ex;
            }
        }
        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    Log.Debug(TAG, GoogleApiAvailability.Instance.GetErrorString(resultCode));
                else
                {
                    Log.Debug(TAG, "This device is not supported");
                    Finish();
                }
                return false;
            }
            Log.Debug(TAG, "Google Play Services is available.");
            return true;
        }
        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }
            var channelName = CHANNEL_ID;
            var channelDescription = string.Empty;
            var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.Default)
            {
                Description = channelDescription,
            };

            channel.SetShowBadge(false);

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
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
                lstSystemInfo = new List<SystemInfo>();
                categoryManage = new CategoryManage();
                category = new Category();
                itemManage = new ItemManage();
                itemExSizeManage = new ItemExSizeManage();
                getItem = new Item();
                getitemSize = new ItemExSize();
                deviceSystemSeqNo = new DeviceSystemSeqNo();
                deviceTranRunningNoManage = new DeviceTranRunningNoManage();
                deviceTranRunning = new DeviceTranRunningNo();
                customerManage = new CustomerManage();
                customer = new Customer();
                discountTemplateManage = new DiscountTemplateManage();
                discount = new DiscountTemplate();
                noteManage = new NoteManage();
                note = new Note();
                noteCategoryManage = new NoteCategoryManage();
                noteCategory = new NoteCategory();
                onBranchManage = new ItemOnBranchManage();

                if (merchants == null)
                {
                    return;
                }

                lstSystemInfo = await SystemInfoManage.GetSystemInfo();
                if (lstSystemInfo == null)
                {
                    ReloadReConnect();
                    return;
                }
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                if (listRivision == null)
                {
                    ReloadReConnect();
                    return;
                }
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
                        if (!result)
                        {
                            ReloadReConnect();
                            return;
                        }

                        deviceSystemSeq = new DeviceSystemSeqNo
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            DeviceNo = merchants.Device.DeviceNo,
                            SystemID = lstSystemInfo[i].SystemID,
                            LastSysSeqNo = 0
                        };
                        var resultSeqNo = await systemSeqNoManage.InsertDeviceSystemSeqNo(deviceSystemSeq);
                        if (!resultSeqNo)
                        {
                            ReloadReConnect();
                            return;
                        }
                    }
                }
                Log.Debug("connectpass", "GetlistRivision");
                //--------------------------------------------------------------------------
                //DeviceTranRunningNo set initial
                //--------------------------------------------------------------------------               
                await SetDataDeviceTranRunningNo();
                Log.Debug("connectpass", "SetDataDeviceTranRunningNo");

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
                progressBar.Progress += 5;//progressBar 35
                Log.Debug("connectpass", "systemSeqNos");

                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);


                //-----------------------------------------------
                //Check local ว่า SystemREvisionNo ว่างหรือไม่
                //Get Branch 10
                //Get Category (int revisionNo) 20 
                //Get Item (int revisionNo,int offset) 30
                //Get Discount 40
                //Get Customer 50
                //Get NoteCategory 60
                //Get Note 70
                //-----------------------------------------------
                int maxCategoryRevision = 0;
                int maxItemRevision = 0;
                int maxItemOnBranchRevision = 0;
                int maxDiscountRevision = 0;
                int maxCustomerRevision = 0;
                int maxNoteCategoryRevision = 0;
                int maxNoteRevision = 0;

                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();

                foreach (var dataRivision in listRivision)
                {
                    switch (dataRivision.SystemID)
                    {
                        case 10:
                            break;
                        case 20:
                            //progressBar.Progress += 5;//progressBar 40
                            #region Category
                            try
                            {
                                //Get Category API
                                var allcategory = await GabanaAPI.GetDataCategory((int)dataRivision.LastRevisionNo);

                                if (allcategory == null)
                                {
                                    break;
                                }

                                if (allcategory.Categories.Count == 0 && allcategory.CategoryBins.Count == 0)
                                {
                                    progressBar.Progress += 5;//progressBar 40
                                    break;
                                }

                                int maxCategory = 0;
                                if (allcategory.Categories.Count > 0)
                                {
                                    allcategory.Categories.ToList().OrderBy(x => x.RevisionNo);
                                    maxCategory = allcategory.Categories.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();

                                    //check ว่ามีไหม
                                    List<Category> GetallCate = await categoryManage.GetAllCategory();
                                    List<ORM.Master.Category> UpdateCategory = new List<ORM.Master.Category>();
                                    List<ORM.Master.Category> InsertCategory = new List<ORM.Master.Category>();
                                    UpdateCategory.AddRange(allcategory.Categories.Where(x => GetallCate.Select(y => (long)y.SysCategoryID).ToList().Contains(x.SysCategoryID)).ToList());
                                    InsertCategory.AddRange(allcategory.Categories.Where(x => !(GetallCate.Select(y => (long)y.SysCategoryID).ToList().Contains(x.SysCategoryID))).ToList());

                                    //CultureInfo.CurrentCulture = new CultureInfo("en-US", false);

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
                                                //CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
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
                                            var insertOrreplace = await categoryManage.InsertOrReplaceCategory(category);
                                            maxCategoryRevision = item.RevisionNo;
                                        }
                                    }
                                }

                                if (allcategory.CategoryBins.Count > 0)
                                {
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
                                        var delete = await categoryManage.DeleteCategory(item.MerchantID, item.SysCategoryID);
                                        if (!delete)
                                        {
                                            var data = await categoryManage.GetCategory(item.MerchantID, item.SysCategoryID);
                                            if (data != null)
                                            {
                                                data.DataStatus = 'D';
                                                data.FWaitSending = 0;
                                                await categoryManage.UpdateCategory(category);
                                            }
                                        }
                                        maxCategoryRevision = item.RevisionNo;
                                    }
                                }

                                progressBar.Progress += 5;//progressBar 40
                                await UtilsAll.updateRevisionNo((int)dataRivision.SystemID, maxCategory);
                                Log.Debug("connectpass", "listRivisionCategory");
                            }
                            catch (Exception ex)
                            {
                                Log.Debug("connecterror", "listRivisionCategory : " + ex.Message);
                                await UtilsAll.ErrorRevisionNo((int)dataRivision.SystemID, maxCategoryRevision);
                            }
                            #endregion                           
                            break;
                        case 30:
                            //progressBar 65
                            #region Item
                            //------------------------------------------------
                            //Get Item API
                            //offset = index สำหรับเรียกข้อมูล ครั้งละ 100 ตัว เริ่มที่ 0
                            //total >= 100 item = 0 - 99     รอบที่ 1  offset = 0
                            //             item = 100 - 199  รอบที่ 2  offset = 1
                            //total > 100  => totalitem/100 = จำนวนรอบที่เรียก 
                            //------------------------------------------------

                            List<Item> lstInsertItemImage = new List<Item>();
                            List<Item> GetAllitem = new List<Item>();
                            List<Gabana3.JAM.Items.ItemWithItemExSizes> UpdateItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                            List<Gabana3.JAM.Items.ItemWithItemExSizes> InsertItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                            try
                            {
                                var allItem = await GabanaAPI.GetDataItem((int)dataRivision.LastRevisionNo, 0);

                                if (allItem == null)
                                {
                                    progressBar.Progress += 40;
                                    break;
                                }
                                else if (allItem?.ItemsWithItemExSizes.Count == 0)
                                {
                                    progressBar.Progress += 40;
                                    break;
                                }
                                else
                                {
                                    int percent = 0;
                                    double round = 0, addrount = 0;
                                    round = allItem.totalItems / 100;
                                    addrount = round + 1;
                                    double percentage = 0, temp = 0;
                                    percentage = (25 / addrount);
                                    temp = percentage;
                                    percentage = 0;

                                    for (int j = 0; j < addrount; j++)
                                    {
                                        allItem = await GabanaAPI.GetDataItem((int)dataRivision.LastRevisionNo, j);

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

                                        //Insert Item
                                        if (InsertItem.Count > 0)
                                        {
                                            foreach (var item in InsertItem)
                                            {
                                                string thumnailPath = string.Empty;
                                                //try
                                                //{
                                                //    if (!string.IsNullOrEmpty(item.ItemStatus.item.PicturePath))
                                                //    {
                                                //        string pathImage = await Utils.InsertLocalPictureItemMaster(item.ItemStatus.item.PicturePath);
                                                //        thumnailPath = pathImage ?? "";
                                                //    }
                                                //    else
                                                //    {
                                                //        thumnailPath = "";
                                                //    }
                                                //}
                                                //catch (Exception ex)
                                                //{
                                                //    //Update RevisionNo ที่ผิดพลาด เพื่อเรียกข้อมูลใหม่
                                                //    var errorRevison = InsertItem.Select(x => x.ItemStatus.item.RevisionNo).Min();
                                                //    maxItemRevision = (errorRevison == 0) ? 0 : errorRevison + 1;
                                                //    Log.Error("connecterror", "Bulkitem - Image : " + ex.Message);
                                                //    thumnailPath = "";
                                                //}

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

                                        await UtilsAll.updateRevisionNo((int)dataRivision.SystemID, maxItem);

                                        if (temp < 1)
                                        {
                                            percentage += temp;
                                        }
                                        else
                                        {
                                            percentage = temp;
                                        }

                                        if (percentage >= 1)
                                        {
                                            if (percentage.ToString().Contains('.'))
                                            {
                                                var split = percentage.ToString().Split('.');
                                                percent = Convert.ToInt32(split[0].ToString());
                                            }
                                            else
                                            {
                                                percent = (int)percentage;
                                            }
                                            percentage = 0;
                                        }
                                        progressBar.Progress += percent;
                                        percent = 0;

                                        if (j == round)
                                        {
                                            progressBar.Progress = 65;
                                        }
                                    }

                                    Log.Debug("connectpass", "Item End Model " + "Time : "  );

                                    //insert Image to Local เมื่อเพิ่มข้อมูลทั้งหมดสำเร็จ ทั้งเคสเพิ่มและเคสอัปเดต
                                    Log.Debug("connectpass", "InsertPictureLocal(lstInsertItemImage)" + "lstInsertItemImage " + lstInsertItemImage.Count);
                                    tasksItem[0] = Task.Factory.StartNew(() => Utils.InsertPictureLocalItem(lstInsertItemImage));
                                    //Function Check Update Image 
                                    Log.Debug("connectpass", "UpdateImageItem(UpdateItem)" + "UpdateItem " + UpdateItem.Count);
                                    tasksItem[1] = Task.Factory.StartNew(() => Utils.UpdateImageItem(UpdateItem));
                                    Log.Debug("connectpass", "InsertPictureLocal(lstInsertItemImage)" + "lstInsertItemImage " + lstInsertItemImage.Count);
                                    Log.Debug("connectpass", "listRivisionItem");
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Debug("connecterror", "listRivisionItem : " + ex.Message);
                                await UtilsAll.ErrorRevisionNo((int)dataRivision.SystemID, maxItemRevision);
                            }
                            #endregion                            
                            break;
                        case 31:
                            //progressBar 85
                            #region ItemOnBranch                            
                            #region OldCode
                            //try
                            //{
                            //    var allItemOnBranch = await GabanaAPI.GetDataItemOnBranch((int)listRivision[i].LastRevisionNo, 0);

                            //    if (allItemOnBranch == null)
                            //    {
                            //        break;
                            //    }

                            //    if (allItemOnBranch.Count > 0)
                            //    {
                            //        int round = allItemOnBranch.Count / 100;
                            //        int addrount = round + 1;
                            //        for (int j = 0; j < addrount; j++)
                            //        {
                            //            allItemOnBranch = await GabanaAPI.GetDataItemOnBranch((int)listRivision[i].LastRevisionNo, j);

                            //            if (allItemOnBranch == null)
                            //            {
                            //                break;
                            //            }

                            //            if (allItemOnBranch.Count == 0)
                            //            {
                            //                break;
                            //            }

                            //            allItemOnBranch.OrderBy(x => x.RevisionNo);
                            //            var maxItemOnBranch = allItemOnBranch.Max(x => x.RevisionNo);

                            //            foreach (var item in allItemOnBranch)
                            //            {
                            //                ItemOnBranch stock = new ItemOnBranch()
                            //                {
                            //                    MerchantID = item.MerchantID,
                            //                    SysBranchID = item.SysBranchID,
                            //                    SysItemID = item.SysItemID,
                            //                    BalanceStock = item.BalanceStock,
                            //                    MinimumStock = item.MinimumStock,
                            //                    LastDateBalanceStock = item.LastDateBalanceStock,
                            //                };
                            //                var insertStock = await onBranchManage.InsertorReplaceItemOnBranch(stock);
                            //                maxItemRevision = item.RevisionNo;
                            //                await UtilsAll.updateRevisionNo((int)listRivision[i].SystemID, maxItemRevision);
                            //            }
                            //        }
                            //    }
                            //}
                            //catch (Exception ex)
                            //{
                            //    Console.WriteLine(ex.Message);
                            //    await UtilsAll.ErrorRevisionNo((int)listRivision[i].SystemID, maxItemRevision);
                            //} 
                            #endregion

                            try
                            {
                                var allItemOnBranch = await GabanaAPI.GetDataItemOnBranchV2((int)dataRivision.LastRevisionNo, 0);

                                if (allItemOnBranch == null)
                                {
                                    progressBar.Progress += 20;
                                    break;
                                }

                                if (allItemOnBranch.totalItemOnBranch == 0)
                                {
                                    progressBar.Progress += 20;
                                    break;
                                }

                                if (allItemOnBranch.totalItemOnBranch > 0)
                                {
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
                                        allItemOnBranch = await GabanaAPI.GetDataItemOnBranchV2((int)dataRivision.LastRevisionNo, j);

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

                                        await UtilsAll.updateRevisionNo((int)dataRivision.SystemID, maxItemOnBranch);

                                        if (temp < 1)
                                        {
                                            percentage += temp;
                                        }
                                        else
                                        {
                                            percentage = temp;
                                        }

                                        if (percentage >= 1)
                                        {
                                            if (percentage.ToString().Contains('.'))
                                            {
                                                var split = percentage.ToString().Split('.');
                                                percent = Convert.ToInt32(split[0].ToString());
                                            }
                                            else
                                            {
                                                percent = (int)percentage;
                                            }
                                            percentage = 0;
                                        }
                                        progressBar.Progress += percent;
                                        percent = 0;

                                        if (j == round)
                                        {
                                            progressBar.Progress = 85;
                                        }

                                    }
                                    Log.Debug("connectpass", "listRivisionItemOnBranch");
                                    //});
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Debug("connecterror", "listRivisionItemOnBranch : " + ex.Message);
                                await UtilsAll.ErrorRevisionNo((int)dataRivision.SystemID, maxItemOnBranchRevision);
                            }
                            #endregion                            
                            break;
                        case 40:
                            #region Discount
                            try
                            {
                                //Get Discount API
                                var alldiscount = await GabanaAPI.GetDataDiscountTemplate((int)dataRivision.LastRevisionNo, 0);

                                if (alldiscount == null)
                                {
                                    break;
                                }

                                if (alldiscount.total == 0)
                                {
                                    break;
                                }
                                alldiscount.DiscountTemplateStatus.ToList().OrderBy(x => x.DiscountTemplates.RevisionNo);
                                var maxDiscount = alldiscount.DiscountTemplateStatus.ToList().Max(x => x.DiscountTemplates.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();

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
                                            FWaitSending = 0,
                                            WaitSendingTime = DateTime.UtcNow

                                        };
                                        var insertOrreplace = await discountTemplateManage.InsertOrReplaceDiscountTemplate(discount);
                                    }
                                    maxDiscountRevision = item.DiscountTemplates.RevisionNo;
                                }

                                await UtilsAll.updateRevisionNo((int)dataRivision.SystemID, maxDiscount);
                            }
                            catch (Exception)
                            {
                                await UtilsAll.ErrorRevisionNo((int)dataRivision.SystemID, maxDiscountRevision);
                            }
                            #endregion
                            break;
                        case 50:
                            #region Customer
                            try
                            {
                                //Get Customer API
                                var allcustomer = await GabanaAPI.GetDataCustomer((int)dataRivision.LastRevisionNo, 0);

                                if (allcustomer == null)
                                {
                                    progressBar.Progress += 5;
                                    break;
                                }

                                if (allcustomer.totalCustomer == 0)
                                {
                                    progressBar.Progress += 5;
                                    break;
                                }

                                //check ว่ามีไหม
                                List<Gabana3.JAM.Customer.CustomerStatus> UpdateCustomer = new List<Gabana3.JAM.Customer.CustomerStatus>();
                                List<Gabana3.JAM.Customer.CustomerStatus> InsertCustomer = new List<Gabana3.JAM.Customer.CustomerStatus>();
                                List<Customer> lstCustomerImage = new List<Customer>();
                                int round = 0, addrount = 0;
                                round = allcustomer.totalCustomer / 100;
                                addrount = round + 1;
                                for (int j = 0; j < addrount; j++)
                                {
                                    allcustomer = await GabanaAPI.GetDataCustomer((int)dataRivision.LastRevisionNo, j);

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

                                    //Insert Customer
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
                                                    Java.IO.File imgTempFile = new Java.IO.File(data?.ThumbnailLocalPath);

                                                    if (System.IO.File.Exists(imgTempFile.AbsolutePath))
                                                    {
                                                        System.IO.File.Delete(imgTempFile.AbsolutePath);
                                                    }
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
                                    await UtilsAll.updateRevisionNo((int)dataRivision.SystemID, maxCustomer);
                                }
                                Log.Debug("connectpass", "listRivisionCustomer");
                                progressBar.Progress += 5;
                                //insert Image to Local เมื่อเพิ่มข้อมูลทั้งหมดสำเร็จ ทั้งเคสเพิ่มและเคสอัปเดต
                                Log.Debug("connectpass", "InsertPictureLocalCustomer(lstCustomerImage) lstCustomerImage : " + lstCustomerImage.Count);
                                tasksCustomer[0] = Task.Factory.StartNew(() => Utils.InsertPictureLocalCustomer(lstCustomerImage));
                                Log.Debug("connectpass", "UpdateImageCustomer(UpdateCustomer) UpdateCustomer : " + UpdateCustomer.Count);
                                tasksCustomer[1] = Task.Factory.StartNew(() => Utils.UpdateImageCustomer(UpdateCustomer));
                                Log.Debug("connectpass", "listRivisionCustomer");
                            }
                            catch (Exception ex)
                            {
                                Log.Debug("connecterror", "listRivisionCustomer : " + ex.Message);
                                await UtilsAll.ErrorRevisionNo((int)dataRivision.SystemID, maxCustomerRevision);
                            }
                            #endregion   
                            break;
                        case 60:
                            #region NoteCategory
                            try
                            {
                                var allNoteCategory = await GabanaAPI.GetDataNoteCategory((int)dataRivision.LastRevisionNo);

                                if (allNoteCategory == null)
                                {
                                    progressBar.Progress += 5;//progressBar 95
                                    break;
                                }

                                if (allNoteCategory.NoteCategory.Count == 0 && allNoteCategory.NoteCategoryBin.Count == 0)
                                {
                                    progressBar.Progress += 5;//progressBar 95
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
                                                FWaitSending = 0,
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
                                                FWaitSending = 0,
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

                                progressBar.Progress += 5;//progressBar 95
                                Log.Debug("connectpass", "listRivisionNoteCategory");
                                await UtilsAll.updateRevisionNo((int)dataRivision.SystemID, maxNoteCategory);
                            }
                            catch (Exception ex)
                            {
                                Log.Debug("connecterror", "listRivisionNoteCategory : " + ex.Message);
                                await UtilsAll.ErrorRevisionNo((int)dataRivision.SystemID, maxNoteCategoryRevision);
                            }
                            #endregion
                            break;
                        case 70:
                            #region Note                            
                            try
                            {
                                //Get NoteCategory API
                                var allNote = await GabanaAPI.GetDataNotes((int)dataRivision.LastRevisionNo, 0);

                                if (allNote == null)
                                {
                                    progressBar.Progress += 5;//progressBar 100
                                    break;
                                }

                                if (allNote.totalNotes == 0)
                                {
                                    progressBar.Progress += 5;//progressBar 100
                                    break;
                                }

                                int round = 0, addrount = 0;
                                round = allNote.totalNotes / 100;
                                addrount = round + 1;
                                for (int j = 0; j < addrount; j++)
                                {
                                    allNote = await GabanaAPI.GetDataNotes((int)dataRivision.LastRevisionNo, j);

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
                                                FWaitSending = 0,
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
                                                    FWaitSending = 0,
                                                    WaitSendingTime = DateTime.UtcNow
                                                };
                                                var insertOrreplace = await noteManage.InsertOrReplaceNote(note);
                                            }
                                            maxNoteRevision = Note.note.RevisionNo;
                                        }
                                    }

                                    await UtilsAll.updateRevisionNo((int)dataRivision.SystemID, maxNote);
                                }
                                Log.Debug("connectpass", "listRivisionNote");
                            }
                            catch (Exception ex)
                            {
                                Log.Debug("connecterror", "listRivisionNote : " + ex.Message);
                                await UtilsAll.ErrorRevisionNo((int)dataRivision.SystemID, maxNoteRevision);
                            }
                            #endregion
                            progressBar.Progress += 5;//progressBar 100
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                //_ = TinyInsights.TrackPageViewAsync("InitialSystemInfo");
                Log.Error("connecterror", "InitialSystemInfo : " + ex.Message);
                throw;
            }
        }

        async Task GetmerchantConfig()
        {
            try
            {
                List<MerchantConfig> lstconfig = new List<MerchantConfig>();
                SetMerchantConfig setconfig = new SetMerchantConfig();
                merchantconfigManage = new MerchantConfigManage();
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
                        setconfig.TAXRATE = TAXRATE.CfgFloat.ToString();
                    }

                    var CURRENCY_SYMBOLS = lstconfig.Where(x => x.CfgKey == "CURRENCY_SYMBOLS").FirstOrDefault();
                    if (CURRENCY_SYMBOLS != null)
                    {
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
                return;
            }
            catch (Exception ex)
            {
                string text = "GetmerchantConfig";
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(text);
                Log.Error("connecterror", "GetmerchantConfig : " + ex.Message);
                throw;
            }
        }
        async Task GetMemberTypeData()
        {
            try
            {
                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    ReloadReConnect();
                    return;
                }

                List<MemberType> lstmembertypr = new List<MemberType>();
                membertypeManage = new MemberTypeManage();                
                if (await GabanaAPI.CheckNetWork())
                {
                    List<ORM.Master.MemberType> listmembertype = new List<ORM.Master.MemberType>();
                    listmembertype = await GabanaAPI.GetDataMemberType();
                    if (listmembertype.Count > 0)
                    {
                        //ลบข้อมูลทั้งหมดก่อน
                        var Allmember = await membertypeManage.DeleteAllMemberType(DataCashingAll.MerchantId);

                        var lstmember = new List<MemberType>();
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
                            lstmember.Add(memberType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetMemberTypeData");
                Log.Error("connecterror", "GetMemberTypeData : " + ex.Message);
                throw;
            }
        }
        async Task GetDeviceData()
        {
            try
            {
                ORM.Master.Device Device = new ORM.Master.Device();
                Device = await GabanaAPI.GetDataDevice(DataCashingAll.DeviceUDID, DataCashingAll.DevicePlatform);
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
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDeviceData");
                Log.Error("connecterror", "GetDeviceData : " + ex.Message);
                throw;
            }
        }

        async Task GetGiftVoucher()
        {
            try
            {
                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    ReloadReConnect();
                    return;
                }

                GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
                if (await GabanaAPI.CheckNetWork())
                {
                    List<ORM.Master.GiftVoucher> listgiftVouchers = new List<ORM.Master.GiftVoucher>();
                    listgiftVouchers = await GabanaAPI.GetDataGiftVoucher();
                    if (listgiftVouchers == null)
                    {
                        return;
                    }
                    if (listgiftVouchers.Count > 0)
                    {
                        //ลบข้อมูลทั้งหมด
                        var Allgifts = await giftVoucherManage.DeleteAllGiftVoucher(DataCashingAll.MerchantId);

                        var lst = listgiftVouchers.OrderBy(x => x.FmlAmount).ToList();
                        foreach (var item in lst)
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
                            await giftVoucherManage.InsertOrReplaceGiftVoucher(giftVoucher);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetGiftVoucher");
                Log.Error("connecterror", "GetGiftVoucher : " + ex.Message);
                throw;
            }
        }

        async Task GetMyQRCode()
        {
            try
            {
                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    ReloadReConnect();
                    return;
                }

                MyQrCodeManage QrCodeManage = new MyQrCodeManage();
                List<ORM.Master.MyQrCode> myqrcodes = new List<ORM.Master.MyQrCode>();
                List<MyQrCode> lstmyqr = new List<MyQrCode>();
                Log.Debug("connectpass", "GetMyQRCode Start API " + "Time : ");
                myqrcodes = await GabanaAPI.GetDataMyQrCode();
                Log.Debug("connectpass", "GetMyQRCode End API " + "Time : "  );
                Log.Debug("connectpass", "GetMyQRCode Start Model " + "Time : "  );
                if (myqrcodes == null)
                {
                    return;
                }
                if (myqrcodes.Count > 0)
                {
                    //ลบข้อมูลทังหมดก่อน                        
                    var data = await QrCodeManage.GetAllMyQrCode(DataCashingAll.MerchantId);
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            if (System.IO.File.Exists(item.PictureLocalPath))
                            {
                                System.IO.File.Delete(item.PictureLocalPath);
                            }
                        }
                    }
                    var AllQR = await QrCodeManage.DeleteAllMyQrCode(DataCashingAll.MerchantId);

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
                            PictureLocalPath = item.PicturePath,
                            SysBranchID = item.SysBranchID  // FMyQrAllBranch = 'A' : null,FMyQrAllBranch = 'B' : DataCashingAll.SysBranchId 
                        };
                        await QrCodeManage.InsertOrReplaceMyQrCode(myQrCode);
                        lstmyqr.Add(myQrCode);
                    }

                    Task.Factory.StartNew(() => Utils.InsertPictureLocalMyQR(lstmyqr));
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetMyQRCode at Splash");
                Log.Error("connecterror", "GetMyQRCode : " + ex.Message);
                throw;
            }
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
                _ = TinyInsights.TrackPageViewAsync("SetDataDeviceTranRunningNo at Splash");
                Log.Error("connecterror", "SetDataDeviceTranRunningNo : " + ex.Message);
                throw;
            }
        }

        async Task GetCashTemplate()
        {
            try
            {
                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    ReloadReConnect();
                    return;
                }

                CashTemplateManage templateManage = new CashTemplateManage();
                List<ORM.Master.CashTemplate> listcashTemplate = new List<ORM.Master.CashTemplate>();
                Log.Debug("connectpass", "GetCashTemplate Start Model " + "Time : "  );
                listcashTemplate = await GabanaAPI.GetDataCashTemplate();
                Log.Debug("connectpass", "GetCashTemplate End Model " + "Time : "  );
                Log.Debug("connectpass", "GetCashTemplate Start Model " + "Time : "  );
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
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetCashTemplate at Splash");
                Log.Error("connecterror", "GetCashTemplate : " + ex.Message);
                throw;
            }
        }

        private void ReloadReConnect()
        {
            try
            {
                RunOnUiThread(() =>
                {
                    btnReconnect.Visibility = ViewStates.Visible;
                });

                numreconnect++;

                if (numreconnect > 2)
                {
                    progressBar.Progress = 100;
                    var DeviceToken = Preferences.Get("NotificationDeviceToken", "");
                    Preferences.Clear();
                    Preferences.Set("NotificationDeviceToken", DeviceToken);
                    StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                    numreconnect = 0;
                    DataCashingAll.flagNotificationEnble = false;
                    this.Finish();
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async Task RegisterInitialSystemInfo()
        {
            try
            {
                systemSeqNoManage = new DeviceSystemSeqNoManage();
                deviceSystemSeq = new DeviceSystemSeqNo();
                systemRevisionNoManage = new SystemRevisionNoManage();
                SystemInfoManage = new SystemInfoManage();
                listRivision = new List<SystemRevisionNo>();
                lstSystemInfo = new List<SystemInfo>();

                if (merchants == null)
                {
                    return;
                }

                lstSystemInfo = await SystemInfoManage.GetSystemInfo();
                if (lstSystemInfo == null)
                {
                    ReloadReConnect();
                    return;
                }
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                if (listRivision == null)
                {
                    ReloadReConnect();
                    return;
                }
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
                        if (!result)
                        {
                            ReloadReConnect();
                            return;
                        }

                        deviceSystemSeq = new DeviceSystemSeqNo
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            DeviceNo = merchants.Device.DeviceNo,
                            SystemID = lstSystemInfo[i].SystemID,
                            LastSysSeqNo = 0
                        };
                        var resultSeqNo = await systemSeqNoManage.InsertDeviceSystemSeqNo(deviceSystemSeq);
                        if (!resultSeqNo)
                        {
                            ReloadReConnect();
                            return;
                        }
                    }
                }
                Log.Debug("connectpass", "GetlistRivision");
                //--------------------------------------------------------------------------
                //DeviceTranRunningNo set initial
                //--------------------------------------------------------------------------               
                await SetDataDeviceTranRunningNo();
                Log.Debug("connectpass", "SetDataDeviceTranRunningNo");

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
                Log.Debug("connectpass", "RegisterInitialSystemInfo");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("RegisterInitialSystemInfo at Splash");
                Log.Error("connecterror", "RegisterInitialSystemInfo : " + ex.Message);
                throw;
            }
        }

    }

}