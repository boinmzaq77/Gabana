using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Merchant;
using LinqToDB.SqlQuery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Theme = "@style/AppTheme.Main", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@mipmap/GabanaLogIn", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginActivity : AppCompatActivity
    {
        public static LoginActivity login_Activity;
        LinearLayout linearmain, linearowner, linearemp, linearregis, lineartabOwner, lineartabEmy;
        EditText txtOwnermobilenumber, txtEmpTel, txtEmpUsername, txtEmpPassword;
        EditText txtRegistel;
        Button btnOwnerLogin, ButtonLogin, btnSignUp;
        Button btnEmpLogin, btnRegisSign;
        ImageButton btnBack;
        //TabHost tablogin;
        public ImageView test;
        LinearLayout framMLogo;
        LinearLayout framLLogo;
        string refstring, SettingPrinter;
        public int clickCount;
        private string tabSelected;
        RecyclerView recyclerHeader;
        Gabana3.JAM.Merchant.Merchants merchants = new Merchants();
        LoginEmp loginEmp = new LoginEmp();
        List<Model.MenuTab> MenuTab { get; set; }
        int merchentID = 0;
        bool CheckPermiss = false;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.login_activity);
                login_Activity = this;
                                
                if (string.IsNullOrEmpty(GabanaAPI.ccJWT))
                {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        GabanaAPI.ccJWT = await GetToken.Get_ccJWT();
                    }
                }

                //------------------------------all linear-----------------------------------
                linearmain = login_Activity.FindViewById<LinearLayout>(Resource.Id.linearmain);
                linearowner = login_Activity.FindViewById<LinearLayout>(Resource.Id.linearowner);
                linearemp = login_Activity.FindViewById<LinearLayout>(Resource.Id.linearemp);
                linearregis = login_Activity.FindViewById<LinearLayout>(Resource.Id.linearregis);
                lineartabOwner = login_Activity.FindViewById<LinearLayout>(Resource.Id.tab_Owner);
                lineartabEmy = login_Activity.FindViewById<LinearLayout>(Resource.Id.tab_Employee);
                //lineartabLogin = login_Activity.FindViewById<LinearLayout>(Resource.Id.lntabLogin);
                //tablogin = login_Activity.FindViewById(Resource.Id.tabhost) as TabHost;

                recyclerHeader = FindViewById<RecyclerView>(Resource.Id.recyclerHeader);

                btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
                btnBack.Click += BtnBack_Click;
                //lineartabLogin.Visibility = ViewStates.Gone;
                framLLogo = FindViewById<LinearLayout>(Resource.Id.framLLogo);
                framMLogo = FindViewById<LinearLayout>(Resource.Id.framMLogo);

                framLLogo.Visibility = ViewStates.Visible;
                framMLogo.Visibility = ViewStates.Gone;
#pragma warning disable CS0618 // Type or member is obsolete
                string local = Resources.Configuration.Locale.Language.ToString();
#pragma warning restore CS0618 // Type or member is obsolete
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
                CreateTab();
                setDisplay();

                //---------------------------main page button---------------------------------
                ButtonLogin = login_Activity.FindViewById<Button>(Resource.Id.btnLogin);
                btnSignUp = login_Activity.FindViewById<Button>(Resource.Id.btnSignUp);
                btnBack.Visibility = ViewStates.Gone;

                ButtonLogin.Click += (sender, e) =>
                {
                    // show lay employee
                    btnBack.Visibility = ViewStates.Visible;
                    linearmain.Visibility = ViewStates.Gone;
                    linearemp.Visibility = ViewStates.Visible;
                    linearowner.Visibility = ViewStates.Visible;
                    linearregis.Visibility = ViewStates.Gone;
                    //lineartabLogin.Visibility = ViewStates.Visible; 
                    //tablogin.Visibility = ViewStates.Visible;
                    framLLogo.Visibility = ViewStates.Gone;
                    framMLogo.Visibility = ViewStates.Visible;
                    setDisplay();

                    //Check Percmission 

                };

                btnSignUp.Click += (sender, e) =>
                {
                    // show lay employee
                    linearmain.Visibility = ViewStates.Gone;
                    linearemp.Visibility = ViewStates.Gone;
                    linearowner.Visibility = ViewStates.Gone;
                    linearregis.Visibility = ViewStates.Visible;
                    //lineartabLogin.Visibility = ViewStates.Gone;
                    btnBack.Visibility = ViewStates.Visible;
                    framLLogo.Visibility = ViewStates.Gone;
                    framMLogo.Visibility = ViewStates.Visible;
                    setDisplay();

                };

                SettingPrinter = Preferences.Get("Setting", "");


                //---------------------------owner page button---------------------------------
                #region Owner Login
                txtOwnermobilenumber = login_Activity.FindViewById<EditText>(Resource.Id.txtOwnermobilenumber);
                btnOwnerLogin = login_Activity.FindViewById<Button>(Resource.Id.btnOwnerLogin);

                btnOwnerLogin.Click += async (sender, e) =>
                {
                    CheckPermiss = CheckPermission();                    
                    if (!CheckPermiss)
                    {
                        btnOwnerLogin.Enabled = true;
                        RequestPermissions(PERMISSIONS, 1);
                    }                    

                    var fragment = new DialogLoading();
                    fragment.Cancelable = false;
                    fragment.Show(SupportFragmentManager, nameof(DialogLoading));
                    btnOwnerLogin.Enabled = false;
                    try
                    {
                        #region Old Code เคสเคยล็อกอินไม่ต้องล็อกอินใหม่

                        var LoginType = Preferences.Get("LoginType", "");
                        if (txtOwnermobilenumber.Text.Trim() == Preferences.Get("PhoneNumber", "") && Preferences.Get("PhoneNumber", "") != "" && LoginType?.ToLower() == "owner")
                        {
                            Preferences.Set("AppState", "login");
                            TokenResult res = await TokenServiceBase.GetToken();
                            if (res != null && res.status)
                            {
                                fragment.Dismiss();
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
                            switch (exception)
                            {
                                case "invalid_grant: Cloud Product Expired":
                                    // แสดง dialog expire ของ Owner  -> ไปหน้า Renew 
                                    MainDialog dialog = new MainDialog();
                                    Bundle bundle = new Bundle();
                                    String myMessage = Resource.Layout.login_dialog_expiry.ToString();
                                    bundle.PutString("message", myMessage);
                                    dialog.Arguments = bundle;
                                    dialog.Show(SupportFragmentManager, myMessage);
                                    fragment.Dismiss();
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

                };
                #endregion

                //---------------------------employee page button---------------------------------
                #region Employee Login
                txtEmpTel = login_Activity.FindViewById<EditText>(Resource.Id.txtEmpTel);
                txtEmpUsername = login_Activity.FindViewById<EditText>(Resource.Id.txtEmpUsername);
                txtEmpPassword = login_Activity.FindViewById<EditText>(Resource.Id.txtEmpPassword);
                btnEmpLogin = login_Activity.FindViewById<Button>(Resource.Id.btnEmpLogin);

                btnEmpLogin.Click += async (sender, e) =>
                {
                    CheckPermiss = CheckPermission();   
                    if (!CheckPermiss)
                    {
                        btnEmpLogin.Enabled = true;
                        RequestPermissions(PERMISSIONS, 1);
                    }

                    DialogLoading dialogLoading = new DialogLoading();
                    btnEmpLogin.Enabled = false;
                    if (dialogLoading.Cancelable != false)
                    {
                        dialogLoading.Cancelable = false;
                        dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                    }

                    loginEmp = new LoginEmp() { MerchantID = RemoveLastWhiteSpace(txtEmpTel.Text), Username = RemoveLastWhiteSpace(txtEmpUsername.Text), Password = RemoveLastWhiteSpace(txtEmpPassword.Text) };

                    #region Old Code เคสเคยล็อกอินไม่ต้องล็อกอินใหม่
                    var merchantid = Preferences.Get("MerchantID", 0);
                    if (txtEmpUsername.Text == Preferences.Get("User", "") && Preferences.Get("User", "") != "" && txtEmpTel.Text == merchantid.ToString() && Preferences.Get("MerchantID", 0) != 0)
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
                                MainDialog dialog = new MainDialog();
                                Bundle bundle = new Bundle();
                                String myMessage = Resource.Layout.login_dialog_expiryemp.ToString();
                                bundle.PutString("message", myMessage);
                                dialog.Arguments = bundle;
                                dialog.Show(SupportFragmentManager, myMessage);
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
                        if (txtEmpTel.Text.Length != 8)
                        {
                            txtEmpTel.Hint = "Merchant Id";
                            txtEmpTel.RequestFocus();
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
                        
                        //refstring คือ RefOTP
                        if (loginEmp == null)
                        {
                            btnEmpLogin.Enabled = true;
                            dialogLoading.Dismiss();
                            return;
                        }                        

                        DataCashing.flagProgress = false;
                        DataCashingAll.UserAccountInfo = null;

                        //GetTokenEmployee
                        GabanaAPI.gbnJWT = await GetToken.GetgbnJWTForEmp(loginEmp);
                        if (GabanaAPI.gbnJWT == null)
                        {
                            Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
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

                            if (check.Equals(txtEmpTel.Text))
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
                        Preferences.Set("User", username);
                        Preferences.Set("Branch", string.Empty);
                        if (DataCashingAll.MerchantId.ToString() == txtEmpTel.Text)
                        {
                            Preferences.Set("Setting", SettingPrinter);
                        }
                        string vername = "";
                        vername = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName?.ToString();
                        Preferences.Set("VersionName", vername);
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
                            MainDialog dialog = new MainDialog();
                            Bundle bundle = new Bundle();
                            String myMessage = Resource.Layout.login_dialog_expiryemp.ToString();
                            bundle.PutString("message", myMessage);
                            dialog.Arguments = bundle;
                            dialog.Show(SupportFragmentManager, myMessage);
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
                        dialogLoading.DismissAllowingStateLoss();
                        dialogLoading.Dismiss();
                    }

                };
                #endregion

                //---------------------------regis page button---------------------------------
                #region Register
                txtRegistel = login_Activity.FindViewById<EditText>(Resource.Id.txtRegistel);
                btnRegisSign = login_Activity.FindViewById<Button>(Resource.Id.btnRegistSignUp);

                btnRegisSign.Click += async (sender, e) =>
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

                    btnRegisSign.Enabled = false;
                    if (txtRegistel.Text == Preferences.Get("User", "") && Preferences.Get("User", "") != "")
                    {
                        SetAppStateLogon();
                        btnRegisSign.Enabled = true;
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
                                btnRegisSign.Enabled = true;
                                fragment.Dismiss();
                                return;
                            }

                            btnRegisSign.Enabled = true;
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

                };
                #endregion

                CheckPermission();
                ChecKVersionApp();
                TextView textVersion = FindViewById<TextView>(Resource.Id.textVersion);
                var vername = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName?.ToString();
                long vernumber = 0;
                vernumber = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionCode;
                textVersion.Text = GetString(Resource.String.versions) + " " + vername + "." + vernumber;

                _ = TinyInsights.TrackPageViewAsync("OnCreate : LoginActivity");

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Login");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void setDisplay()
        {
            //linearLayout1 = FindViewById<LinearLayout>(Resource.Id.linearLayout1);
            //linearLayout2 = FindViewById<LinearLayout>(Resource.Id.linearLayout2);
            //linearLayout3 = FindViewById<FrameLayout>(Resource.Id.linearLayout3);

            //var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            //var width = mainDisplayInfo.Width;
            //var height3 = linearLayout3.LayoutParameters.Height;
            //var h = mainDisplayInfo.Height - height3;
            //var heigth1 = (h / 5) * 2;
            //linearLayout1.LayoutParameters.Height = Convert.ToInt32(heigth1);
            //linearLayout2.LayoutParameters.Height = Convert.ToInt32(h - heigth1);
            //linearLayout3.LayoutParameters.Height = 80;
        }
        private async void ChecKVersionApp()
        {
            try
            {
                var vername = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
                decimal.TryParse(vername, out decimal vessionOnApp);
                var versionApp = await GabanaAPI.GetDataAppConfig("AndroidVersionMinimum");
                decimal.TryParse(versionApp?.CfgString, out decimal versionOnCloud);
                if (vessionOnApp < versionOnCloud)
                {
                    MainDialog dialog = new MainDialog() { Cancelable = false };
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.updateapp_dialog_main.ToString();
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
                return;
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


        private void BtnBack_Click(object sender, EventArgs e)
        {
            linearmain.Visibility = ViewStates.Visible;
            linearemp.Visibility = ViewStates.Gone;
            linearowner.Visibility = ViewStates.Gone;
            linearregis.Visibility = ViewStates.Gone;
            btnBack.Visibility = ViewStates.Gone;
            framLLogo.Visibility = ViewStates.Visible;
            framMLogo.Visibility = ViewStates.Gone;
            setDisplay();

        }
        public static void IsEmpty(EditText etText)
        {
            if (etText.Text.Trim() == null)
            {
                etText.RequestFocus();
                return;
            }
        }
        public static string RemoveLastWhiteSpace(string str)
        {
            try
            {
                if (!(char.IsLetter(str[str.Length - 1])) && (!(char.IsNumber(str[str.Length - 1]))) && ((str[str.Length - 1] != '@')) && ((str[str.Length - 1] != '.')) && ((str[str.Length - 1] != '_')))
                {
                    str = str.Trim().Trim(new[] { '\uFEFF', '\u200B' });
                }
                return str;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("RemoveLastWhiteSpace at Login");
                return str;
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public void CreateTab()
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
            recyclerHeader.HasFixedSize = true;
            recyclerHeader.SetLayoutManager(menuLayoutManager);
            Report_Adapter_Header report_adapter_header = new Report_Adapter_Header(MenuTab, tabSelected);
            recyclerHeader.SetAdapter(report_adapter_header);
            report_adapter_header.ItemClick += Report_adapter_header_ItemClick;
            SetTabLogin();

        }
        private void Report_adapter_header_ItemClick(object sender, int e)
        {
            tabSelected = MenuTab[e].NameMenuEn;
            SetTabShowMenu();
            SetTabLogin();
        }
        private void SetTabLogin()
        {
            lineartabOwner.Visibility = ViewStates.Gone;
            lineartabEmy.Visibility = ViewStates.Gone;
            switch (tabSelected)
            {
                case "Owner":
                    lineartabOwner.Visibility = ViewStates.Visible;
                    break;
                case "Employee":
                    lineartabEmy.Visibility = ViewStates.Visible;
                    break;
                default:
                    break;
            }

        }
        public async Task<VerifyOTP> Sentotp(string mobilephone)
        {
            try
            {
                if (string.IsNullOrEmpty(DataCashingAll.DeviceUDID))
                {
                    DataCashingAll.DeviceUDID = Preferences.Get("DeviceUDID","");
                }
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
        public override void OnBackPressed()
        {
            try
            {
                btnBack.PerformClick();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        protected override async void OnResume()
        {
            try
            {
                base.OnResume();
                setDisplay();
                if (string.IsNullOrEmpty(GabanaAPI.ccJWT))
                {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        GabanaAPI.ccJWT = await GetToken.Get_ccJWT();
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("OnResume at login");
            }
        }
    }
}

