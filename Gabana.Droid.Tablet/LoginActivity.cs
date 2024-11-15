using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Login;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Interfaces;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Merchant;
using Newtonsoft.Json;
using Org.Apache.Commons.Logging;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;
using static Xamarin.Google.Android.DataTransport.Runtime.Scheduling.JobScheduling.SchedulerConfig;
using Xamarin.Forms.Shapes;

namespace Gabana.Droid.Tablet
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class LoginActivity : AppCompatActivity
    {
        public static LoginActivity login_activity;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.login_activity_main);
            login_activity = this;
            if (await GabanaAPI.CheckNetWork())
            {
                GabanaAPI.ccJWT = await GetToken.Get_ccJWT();
            }
            string local = Resources.Configuration.Locale.Language.ToString();
            if (string.IsNullOrEmpty(Preferences.Get("Language", "")))
            {
                DataCashing.Language = local;
                Preferences.Set("Language", local);
            }
            else
            {
                DataCashing.Language = Preferences.Get("Language", "");
            }
            tabSelected = "Owner";
            CombineUI();

        }


        LinearLayout lnBack, lnMain, lnLogin, lnRegister;
        FrameLayout content_frame;
        RecyclerView rcvTypeLogin;
        int fram;
        string tabSelected;
        RelativeLayout tab_Owner, tab_Employee;
        Button btnRegistSignUp, btnOwnerLogin, btnEmpLogin;
        EditText txtRegistel, txtOwnermobilenumber, txtMerchantID, txtEmpUsername, txtEmpPassword;
        string refstring, SettingPrinter;
        public int clickCount;
        bool CheckPermiss = false;
        private void CombineUI()
        {
            content_frame = FindViewById<FrameLayout>(Resource.Id.content_frame);
            lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Visibility = ViewStates.Invisible;
            lnBack.Click += LnBack_Click;
            fram = Resource.Id.content_frame;

            CheckPermission();
            ChecKVersionApp();
            TextView textVersion = FindViewById<TextView>(Resource.Id.textVersion);
            var vername = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName?.ToString();
            long vernumber = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionCode;
            textVersion.Text = "Version " + vername + "." + vernumber;

            //ui linear Register
            lnRegister = FindViewById<LinearLayout>(Resource.Id.lnRegister);
            txtRegistel = FindViewById<EditText>(Resource.Id.txtRegistel);
            btnRegistSignUp = FindViewById<Button>(Resource.Id.btnRegistSignUp);
            btnRegistSignUp.Click += BtnRegistSignUp_Click;

            //ui linear Main
            lnMain = FindViewById<LinearLayout>(Resource.Id.lnMain);
            Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            Button btnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);
            btnLogin.Click += BtnLogin_Click;
            btnSignUp.Click += BtnSignUp_Click;

            //uilinear Login
            lnLogin = FindViewById<LinearLayout>(Resource.Id.lnLogin);
            rcvTypeLogin = FindViewById<RecyclerView>(Resource.Id.rcvTypeLogin);
            tab_Owner = FindViewById<RelativeLayout>(Resource.Id.tab_Owner);
            tab_Employee = FindViewById<RelativeLayout>(Resource.Id.tab_Employee);

            //login Owner 
            txtOwnermobilenumber = FindViewById<EditText>(Resource.Id.txtOwnermobilenumber);
            btnOwnerLogin = FindViewById<Button>(Resource.Id.btnOwnerLogin);
            btnOwnerLogin.Click += BtnOwnerLogin_Click;

            //login Employee
            txtMerchantID = FindViewById<EditText>(Resource.Id.txtMerchantID);
            txtEmpUsername = FindViewById<EditText>(Resource.Id.txtEmpUsername);
            txtEmpPassword = FindViewById<EditText>(Resource.Id.txtEmpPassword);
            btnEmpLogin = FindViewById<Button>(Resource.Id.btnEmpLogin);
            btnEmpLogin.Click += BtnEmpLogin_Click;

            //ui tab Typelogin
            SettingPrinter = Preferences.Get("Setting", "");
            tabSelected = "Owner";
            CreateTabLoginType();

            LoadUI("main");
        }
        LoginEmp loginEmp = new LoginEmp();
        Merchants merchants = new Merchants();
        private async void BtnEmpLogin_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                CheckPermiss = CheckPermission();
                if (!CheckPermiss)
                {
                    btnEmpLogin.Enabled = true;
                    RequestPermissions(PERMISSIONS, 1);
                }

                btnEmpLogin.Enabled = false;
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                loginEmp = new LoginEmp() { MerchantID = RemoveLastWhiteSpace(txtMerchantID.Text), Username = RemoveLastWhiteSpace(txtEmpUsername.Text), Password = RemoveLastWhiteSpace(txtEmpPassword.Text) };

                #region Old Code เคสเคยล็อกอินไม่ต้องล็อกอินใหม่
                var merchantid = Preferences.Get("MerchantID", 0);
                if (txtEmpUsername.Text == Preferences.Get("User", "") && Preferences.Get("User", "") != "" && txtMerchantID.Text == merchantid.ToString() && Preferences.Get("MerchantID", 0) != 0)
                {
                    try
                    {
                        GabanaAPI.gbnJWT = await GetToken.GetgbnJWTForEmp(loginEmp);
                        if (GabanaAPI.gbnJWT == null)
                        {
                            Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Long).Show();
                            btnEmpLogin.Enabled = true;
                            dialogLoading.Dismiss();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        string ShowError = UtilsAll.CheckErrorGetToken(ex.Message);
                        if (ShowError == "invalid_grant: invalid_username_or_password" || ShowError == "UserPassIncorrect")
                        {
                            Toast.MakeText(this, GetString(Resource.String.dataincorrect), ToastLength.Long).Show();
                        }
                        else if (ShowError == "invalid_grant: User Can't Access CloudProduct" || ShowError == "UserCanNotAccessCloudProduct")
                        {
                            Toast.MakeText(this, GetString(Resource.String.permissionincorrect), ToastLength.Long).Show();
                        }
                        else if (ShowError == "CloudProductExpired" || ShowError == "invalid_grant: Cloud Product Expired")
                        {
                            // แสดง dialog expire ของ employee  -> ไปหน้า login
                            Bundle bundle = new Bundle();
                            var fragementExpireEmp = new Login_Dialog_ExpiryEmp();
                            fragementExpireEmp.Cancelable = false;
                            fragementExpireEmp.Arguments = bundle;
                            fragementExpireEmp.Show(this.SupportFragmentManager, nameof(Login_Dialog_ExpiryEmp));
                        }
                        else
                        {
                            Toast.MakeText(this, ShowError, ToastLength.Long).Show();
                        }
                        btnEmpLogin.Enabled = true;
                        dialogLoading.Dismiss();
                        return;
                    }

                    Preferences.Set("AppState", "login");
                    TokenResult res = await TokenServiceBase.GetToken();
                    if (res != null && res.status)
                    {
                        dialogLoading.Dismiss();
                        SetAppStateLogon();
                        //DataCashingAll.UserActive = DateTime.Now; //stamp เวลาในการเข้าใช้งาน
                        Preferences.Set("UserActive", DateTime.Now.ToString());
                        StartActivity(typeof(SplashActivity));
                        this.Finish();
                        return;
                    }
                }
                #endregion

                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    btnEmpLogin.Enabled = true;
                    dialogLoading.Dismiss();
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    return;
                }

                try
                {
                    if (txtMerchantID.Text.Length != 8)
                    {
                        txtMerchantID.Hint = "Merchant Id";
                        txtMerchantID.RequestFocus();
                        Toast.MakeText(this, GetString(Resource.String.merchantidnotcomplete), ToastLength.Short).Show();
                        btnEmpLogin.Enabled = true;
                        dialogLoading.Dismiss();
                        return;
                    }
                    if (string.IsNullOrEmpty(txtEmpUsername.Text))
                    {
                        txtEmpUsername.RequestFocus();
                        Toast.MakeText(this, GetString(Resource.String.userincorrect), ToastLength.Long).Show();
                        btnEmpLogin.Enabled = true;
                        dialogLoading.Dismiss();
                        return;
                    }
                    if (string.IsNullOrEmpty(txtEmpPassword.Text))
                    {
                        txtEmpPassword.RequestFocus();
                        Toast.MakeText(this, GetString(Resource.String.passwordincorrect), ToastLength.Long).Show();
                        btnEmpLogin.Enabled = true;
                        dialogLoading.Dismiss();
                        return;
                    }

                    btnEmpLogin.Enabled = true;
                    // startactivity otp
                    //loginEmp = new LoginEmp() { MerchantID = RemoveLastWhiteSpace(txtMerchantID.Text), Username = RemoveLastWhiteSpace(txtEmpUsername.Text), Password = RemoveLastWhiteSpace(txtEmpPassword.Text) };

                    //refstring คือ RefOTP
                    if (loginEmp == null)
                    {
                        btnEmpLogin.Enabled = true;
                        dialogLoading.Dismiss();
                        return;
                    }

                    DataCashing.flagProgress = false;
                    DataCashingAll.UserAccountInfo = null;

                    try
                    {
                        //GetTokenEmployee
                        GabanaAPI.gbnJWT = await GetToken.GetgbnJWTForEmp(loginEmp);
                        if (GabanaAPI.gbnJWT == null)
                        {
                            Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                            btnEmpLogin.Enabled = true;
                            dialogLoading.Dismiss();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        string ShowError = UtilsAll.CheckErrorGetToken(ex.Message);
                        if (ShowError == "invalid_grant: invalid_username_or_password" || ShowError == "UserPassIncorrect")
                        {
                            Toast.MakeText(this, GetString(Resource.String.dataincorrect), ToastLength.Long).Show();
                        }
                        else if (ShowError == "invalid_grant: User Can't Access CloudProduct" || ShowError == "UserCanNotAccessCloudProduct")
                        {
                            Toast.MakeText(this, GetString(Resource.String.permissionincorrect), ToastLength.Long).Show();
                        }
                        else if (ShowError == "CloudProductExpired" || ShowError == "invalid_grant: Cloud Product Expired")
                        {
                            // แสดง dialog expire ของ employee  -> ไปหน้า login
                            Bundle bundle = new Bundle();
                            var fragementExpireEmp = new Login_Dialog_ExpiryEmp();
                            fragementExpireEmp.Cancelable = false;
                            fragementExpireEmp.Arguments = bundle;
                            fragementExpireEmp.Show(this.SupportFragmentManager, nameof(Login_Dialog_ExpiryEmp));
                        }
                        else
                        {
                            Toast.MakeText(this, ShowError, ToastLength.Long).Show();
                        }
                        btnEmpLogin.Enabled = true;
                        dialogLoading.Dismiss();
                        return;
                    }

                    //Get Merchant Detail from API
                    merchants = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
                    if (merchants == null)
                    {
                        btnEmpLogin.Enabled = true;
                        dialogLoading.Dismiss();
                        return;
                    }

                    if (DataCashingAll.UserAccountInfo == null || DataCashingAll.UserAccountInfo?.Count == 0)
                    {
                        DataCashingAll.UserAccountInfo = new List<Model.UserAccountInfo>();
                        DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
                    }

                    string LoginType = "employee";
                    string username = string.Empty;
                    var data = DataCashingAll.UserAccountInfo.Where(x => x.UserName.ToLower() == loginEmp.Username.ToLower()).FirstOrDefault();
                    if (data != null)
                    {
                        LoginType = data.MainRoles;
                        username = data.UserName;
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.contactadmin), ToastLength.Short).Show();
                        dialogLoading.Dismiss();
                        return;
                    }

                    List<BranchPolicy> lstbranchPolicies = new List<BranchPolicy>();
                    List<ORM.Master.BranchPolicy> getlstbranchPolicy = new List<ORM.Master.BranchPolicy>();
                    int merchentID = 0;
                    merchentID = Convert.ToInt32(loginEmp.MerchantID);

                    //Check listBranchPolicy จาก GabanaAPI เนื่องจาก ยังไม่มีการสร้างฐานข้อมูลต้องนำข้อมูลมาจากเซิฟเวอร์
                    //เช็กเงื่อนไขเพิ่มเติม เกิดบัค เพราะ ยังไม่มีการสร้างฐานข้อมูล

                    if (string.IsNullOrEmpty(DataCashingAll.Pathdb))
                    {
                        getlstbranchPolicy = await GabanaAPI.GetDataBranchPolicy();
                        if (getlstbranchPolicy == null)
                        {
                            Toast.MakeText(this, GetString(Resource.String.contactadmin), ToastLength.Short).Show();
                            dialogLoading.Dismiss();
                            return;
                        }
                        getlstbranchPolicy = getlstbranchPolicy.Where(x => x.MerchantID == merchentID && x.UserName.ToLower() == username.ToLower()).ToList();

                        if (LoginType.ToLower() != "admin" && getlstbranchPolicy.Count == 0)
                        {
                            Toast.MakeText(this, GetString(Resource.String.login_fragment_nonpermission), ToastLength.Short).Show();
                            Preferences.Set("AppState", "logout");
                            dialogLoading.Dismiss();
                            return;
                        }
                    }
                    else
                    {
                        string strSource = DataCashingAll.Pathdb, strStart = "999", strEnd = DataCashing.sqliteMerchantDB;
                        string check = "", strSub = "", pattern = "/", replacement = "";

                        if (strSource.Contains(strStart) && strSource.Contains(strEnd))
                        {
                            int Start, End;
                            Start = strSource.IndexOf(strStart, 0) + strStart.Length - 3;
                            End = strSource.IndexOf(strEnd, Start);
                            strSub = strSource.Substring(Start, End - (Start));

                            System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(pattern);
                            check = System.Text.RegularExpressions.Regex.Replace(regEx.Replace(strSub, replacement), @"\s+", "");
                        }

                        if (check.Equals(txtMerchantID.Text))
                        {
                            BranchPolicyManage branchPolicyManage = new BranchPolicyManage();
                            lstbranchPolicies = await branchPolicyManage.GetlstBranchPolicy(merchentID, username);
                            if (lstbranchPolicies == null)
                            {
                                Toast.MakeText(this, GetString(Resource.String.contactadmin), ToastLength.Short).Show();
                                dialogLoading.Dismiss();
                                return;
                            }

                            if (LoginType.ToLower() != "admin" && lstbranchPolicies.Count == 0)
                            {
                                Toast.MakeText(this, GetString(Resource.String.login_fragment_nonpermission), ToastLength.Short).Show();
                                Preferences.Set("AppState", "logout");
                                dialogLoading.Dismiss();
                                return;
                            }
                        }
                        else
                        {
                            getlstbranchPolicy = await GabanaAPI.GetDataBranchPolicy();
                            if (getlstbranchPolicy == null)
                            {
                                Toast.MakeText(this, GetString(Resource.String.contactadmin), ToastLength.Short).Show();
                                dialogLoading.Dismiss();
                                return;
                            }
                            getlstbranchPolicy = getlstbranchPolicy.Where(x => x.MerchantID == merchentID && x.UserName.ToLower() == username.ToLower()).ToList();

                            if (LoginType.ToLower() != "admin" && getlstbranchPolicy.Count == 0)
                            {
                                Toast.MakeText(this, GetString(Resource.String.login_fragment_nonpermission), ToastLength.Short).Show();
                                Preferences.Set("AppState", "logout");
                                dialogLoading.Dismiss();
                                return;
                            }
                        }
                    }

                    //DataCashingAll.UserActive = DateTime.Now; //stamp เวลาในการเข้าใช้งาน
                    Preferences.Set("UserActive", DateTime.Now.ToString());
                    Preferences.Set("ViewPos", "Grid");
                    Preferences.Set("AppState", "login");
                    Preferences.Set("LoginType", LoginType);
                    Preferences.Set("User", username.ToLower());
                    //Preferences.Set("CreateDB", CreateDB);
                    if (DataCashingAll.MerchantId.ToString() == txtMerchantID.Text)
                    {
                        Preferences.Set("Setting", SettingPrinter);
                    }
                    StartActivity(typeof(SplashActivity));
                    dialogLoading.Dismiss();
                    this.Finish();
                }
                catch (Exception ex)
                {
                    string ShowError = UtilsAll.CheckErrorGetToken(ex.Message);
                    if (ShowError == "invalid_grant: invalid_username_or_password" || ShowError == "UserPassIncorrect")
                    {
                        Toast.MakeText(this, GetString(Resource.String.dataincorrect), ToastLength.Long).Show();
                    }
                    else if (ShowError == "invalid_grant: User Can't Access CloudProduct" || ShowError == "UserCanNotAccessCloudProduct")
                    {
                        Toast.MakeText(this, GetString(Resource.String.permissionincorrect), ToastLength.Long).Show();
                    }
                    else if (ShowError == "CloudProductExpired" || ShowError == "invalid_grant: Cloud Product Expired")
                    {
                        // แสดง dialog expire ของ employee  -> ไปหน้า login
                        Bundle bundle = new Bundle();
                        var fragementExpireEmp = new Login_Dialog_ExpiryEmp();
                        fragementExpireEmp.Cancelable = false;
                        fragementExpireEmp.Arguments = bundle;
                        fragementExpireEmp.Show(this.SupportFragmentManager, nameof(Login_Dialog_ExpiryEmp));
                    }
                    else
                    {
                        Toast.MakeText(this, ShowError, ToastLength.Long).Show();
                    }
                    btnEmpLogin.Enabled = true;
                    dialogLoading.Dismiss();
                    return;
                }

                if (dialogLoading != null)
                {
                    btnEmpLogin.Enabled = true;
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                btnEmpLogin.Enabled = true;
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        public static string RemoveLastWhiteSpace(string str)
        {
            if (!(char.IsLetter(str[str.Length - 1])) && (!(char.IsNumber(str[str.Length - 1]))) && ((str[str.Length - 1] != '@')) && ((str[str.Length - 1] != '.')) && ((str[str.Length - 1] != '_')))
            {
                str = str.Trim().Trim(new[] { '\uFEFF', '\u200B' });
            }
            return str;
        }
        private async void BtnOwnerLogin_Click(object sender, EventArgs e)
        {
            var fragment = new DialogLoading();
            try
            {
                CheckPermiss = CheckPermission();
                if (!CheckPermiss)
                {
                    btnOwnerLogin.Enabled = true;
                    RequestPermissions(PERMISSIONS, 1);
                }

                fragment.Cancelable = false;
                fragment.Show(SupportFragmentManager, nameof(DialogLoading));
                btnOwnerLogin.Enabled = false;
                try
                {
                    #region Old Code เคสเคยล็อกอินไม่ต้องล็อกอินใหม่
                    if (txtOwnermobilenumber.Text.Trim() == Preferences.Get("PhoneNumber", "") && Preferences.Get("PhoneNumber", "") != "")
                    {
                        fragment.Dismiss();
                        Preferences.Set("AppState", "login");
                        TokenResult res = await TokenServiceBase.GetToken();
                        if (res != null && res.status)
                        {
                            SetAppStateLogon();
                            //DataCashingAll.UserActive = DateTime.Now; //stamp เวลาในการเข้าใช้งาน
                            Preferences.Set("UserActive", DateTime.Now.ToString());
                            StartActivity(typeof(SplashActivity));
                            this.Finish();
                            return;
                        }
                    }
                    #endregion

                    if (!await GabanaAPI.CheckSpeedConnection())
                    {
                        btnOwnerLogin.Enabled = true;
                        fragment.Dismiss();
                        Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                        return;
                    }

                    if (txtOwnermobilenumber.Text.Trim().Length != 10)
                    {
                        txtOwnermobilenumber.Hint = "input phone number";
                        txtOwnermobilenumber.RequestFocus();
                        Toast.MakeText(this, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
                        btnOwnerLogin.Enabled = true;
                        fragment.Dismiss();
                        return;
                    }

                    //SeAuth2API CheckSingUpOrLoginGabana เช็คว่ามี merchantId หรือไม่?
                    var statusLogin = await GabanaAPI.CheckSingUpOrLoginGabana(txtOwnermobilenumber.Text.Trim());
                    if (!statusLogin.Status)
                    {
                        btnOwnerLogin.Enabled = true;
                        Toast.MakeText(this, GetString(Resource.String.pleaseregister), ToastLength.Long).Show();
                        fragment.Dismiss();
                        return;
                    }
                    else
                    {
                        int.TryParse(statusLogin.Message, out int result);
                        DataCashingAll.MerchantId = Convert.ToInt32(result);
                    }

                    //InitialGabana
                    VerifyOTP verify = await Sentotp(txtOwnermobilenumber.Text.Trim());

                    //refstring คือ RefOTP
                    if (verify.OwnerID == null)
                    {
                        string exception = verify.RefOTP;
                        //string exception = "invalid_grant: Cloud Product Expired"; //ทดสอบหมดอายุ
                        switch (exception)
                        {
                            case "invalid_grant: Cloud Product Expired":
                                // แสดง dialog expire ของ Owner  -> ไปหน้า Renew 
                                Bundle bundle = new Bundle();
                                var fragement = new Login_Dialog_Expiry();
                                fragement.Cancelable = false;
                                fragement.Arguments = bundle;
                                fragement.Show(SupportFragmentManager, nameof(Login_Dialog_Expiry));
                                break;
                            default:
                                Toast.MakeText(this, exception, ToastLength.Long).Show();
                                break;
                        }
                        btnOwnerLogin.Enabled = true;
                        fragment.Dismiss();
                        return;
                    }

                    refstring = verify.RefOTP.ToString();
                    var Intentotp = new Intent(this, typeof(OtpActivity));
                    Intentotp.PutExtra("refstring", JsonConvert.SerializeObject(verify));
                    Intentotp.PutExtra("Layout", "LoginOwner");
                    StartActivity(Intentotp);
                    fragment.Dismiss();
                    this.Finish();
                }
                catch (Exception ex)
                {
                    fragment.Dismiss();
                    _ = TinyInsights.TrackErrorAsync(ex);
                    _ = TinyInsights.TrackPageViewAsync("btnOwnerLogin.Click at Login");
                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    return;
                }
            }
            catch (Exception ex)
            {
                fragment.Dismiss();
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        public async Task<VerifyOTP> Sentotp(string mobilephone)
        {
            try
            {
                var verify = new SendOTP() { OwnerID = mobilephone, UDID = DataCashingAll.DeviceUDID };
                var refstring = await GabanaAPI.urlInitialLoginGabana(verify);
                VerifyOTP verifyOTP = new VerifyOTP() { OwnerID = mobilephone, RefOTP = refstring.Message };
                return verifyOTP;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("Sentotp at login");
                VerifyOTP verify = new VerifyOTP();
                verify.RefOTP = ex.Message.ToString();
                return verify;
            }
        }
        private async void BtnRegistSignUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (!await GabanaAPI.CheckNetWork())
                {
                    Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    return;
                }

                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    return;
                }

                var fragment = new DialogLoading() { Cancelable = false };
                fragment.Show(SupportFragmentManager, nameof(DialogLoading));

                btnRegistSignUp.Enabled = false;
                if (txtRegistel.Text == Preferences.Get("User", "") && Preferences.Get("User", "") != "")
                {
                    SetAppStateLogon();
                    btnRegistSignUp.Enabled = true;
                }
                else
                {
                    try
                    {
                        if (txtRegistel.Text.Length != 10)
                        {
                            txtRegistel.Hint = "input phone number";
                            txtRegistel.RequestFocus();
                            Toast.MakeText(this, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
                            btnRegistSignUp.Enabled = true;
                            fragment.Dismiss();
                            return;
                        }

                        btnRegistSignUp.Enabled = true;
                        // startactivity otp
                        //SeAuth2API CheckSingUpOrLoginGabana เช็คว่ามี merchantId หรือไม่?
                        var haveJWT = await GabanaAPI.CheckSingUpOrLoginGabana(txtRegistel.Text);
                        if (haveJWT.Status)
                        {
                            Toast.MakeText(this, GetString(Resource.String.pleaselogin), ToastLength.Long).Show();
                            txtRegistel.Text = "";
                            fragment.Dismiss();
                            return;
                        }

                        //InitialGabana
                        VerifyOTP verify = await SentRegisterotp(txtRegistel.Text);

                        //refstring คือ RefOTP
                        if (verify == null)
                        {
                            fragment.Dismiss();
                            return;
                        }

                        refstring = verify.RefOTP.ToString();
                        var Intentotp = new Intent(this, typeof(OtpActivity));
                        Intentotp.PutExtra("refstring", JsonConvert.SerializeObject(verify));
                        Intentotp.PutExtra("Layout", "Create Account");
                        StartActivity(Intentotp);
                        fragment.Dismiss();
                    }
                    catch (Exception ex)
                    {
                        fragment.Dismiss();
                        Console.WriteLine(ex.Message);
                        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        fragment.Dismiss();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        public async Task<VerifyOTP> SentRegisterotp(string mobilephone)
        {
            try
            {
                var verify = new SendOTP() { OwnerID = mobilephone, UDID = DataCashingAll.DeviceUDID };
                var refstring = await GabanaAPI.urlInitialCreateGabana(verify);
                VerifyOTP verifyOTP = new VerifyOTP() { OwnerID = mobilephone, RefOTP = refstring.Message };
                return verifyOTP;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SentRegisterotp at login");
                VerifyOTP verify = new VerifyOTP();
                verify.RefOTP = ex.Message.ToString();
                return verify;
            }
        }
        public void SetAppStateLogon()
        {
            Preferences.Set("AppState", "login");
            Preferences.Set("ViewPos", "Grid");
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            LoadUI("main");
        }
        List<Model.MenuTab> MenuTab { get; set; }
        private void CreateTabLoginType()
        {
            MenuTab = new List<Model.MenuTab>
            {
                new Model.MenuTab() { NameMenuEn = "Owner" , NameMenuTh = "เจ้าของ" },
                new Model.MenuTab() { NameMenuEn = "Employee" , NameMenuTh = "พนักงาน" }
            };
            SetTabShowMenu();
        }
        private void SetTabShowMenu()
        {
            GridLayoutManager menuLayoutManager = new GridLayoutManager(this, 2, 1, false);
            rcvTypeLogin.HasFixedSize = true;
            rcvTypeLogin.SetLayoutManager(menuLayoutManager);
            Login_Adapter_TypeLogin adapter_typelogin = new Login_Adapter_TypeLogin(MenuTab, tabSelected);
            rcvTypeLogin.SetAdapter(adapter_typelogin);
            adapter_typelogin.ItemClick += Adapter_typelogin_ItemClick; ;

            SetTabLogin();
        }
        private void SetTabLogin()
        {
            tab_Owner.Visibility = ViewStates.Gone;
            tab_Employee.Visibility = ViewStates.Gone;
            switch (tabSelected)
            {
                case "Owner":
                    tab_Owner.Visibility = ViewStates.Visible;
                    break;
                case "Employee":
                    tab_Employee.Visibility = ViewStates.Visible;
                    break;
                default:
                    tab_Owner.Visibility = ViewStates.Visible;
                    break;
            }
        }
        private void Adapter_typelogin_ItemClick(object sender, int e)
        {
            tabSelected = MenuTab[e].NameMenuEn;
            SetTabShowMenu();
            SetTabLogin();
        }
        private void BtnSignUp_Click(object sender, EventArgs e)
        {
            LoadUI("register");
        }
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            LoadUI("login");
        }
        private void LoadUI(string menu)
        {
            lnBack.Visibility = ViewStates.Invisible;
            lnMain.Visibility = ViewStates.Invisible;
            lnLogin.Visibility = ViewStates.Invisible;
            lnRegister.Visibility = ViewStates.Invisible;

            switch (menu)
            {
                case "main":
                    lnMain.Visibility = ViewStates.Visible;
                    break;
                case "login":
                    lnLogin.Visibility = ViewStates.Visible;
                    lnBack.Visibility = ViewStates.Visible;
                    break;
                case "register":
                    lnRegister.Visibility = ViewStates.Visible;
                    lnBack.Visibility = ViewStates.Visible;
                    break;
                default:
                    break;
            }

        }

            
        string[] PERMISSIONS;
        public bool CheckPermission()
        {
            Permission permissionCamera = CheckSelfPermission(Manifest.Permission.Camera);
            Permission permissionRead = CheckSelfPermission(Manifest.Permission.ReadExternalStorage);
            Permission permissionWrite = CheckSelfPermission(Manifest.Permission.WriteExternalStorage);
            Permission permissionBluetooth = CheckSelfPermission(Manifest.Permission.Bluetooth);
            Permission permissionBluetoothC = CheckSelfPermission(Manifest.Permission.BluetoothConnect);
            Permission ddd = CheckSelfPermission(Manifest.Permission.MountUnmountFilesystems);

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

        private async void ChecKVersionApp()
        {
            try
            {
                var vername = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
                var versionApp = await GabanaAPI.GetDataAppConfig("AndroidVersionMinimum");
                if (Convert.ToDecimal(vername) < Convert.ToDecimal(versionApp?.CfgString))
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.login_dialog_updateapp.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ChecKVersionApp at Login");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}