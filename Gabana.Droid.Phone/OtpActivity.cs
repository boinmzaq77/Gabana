using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Newtonsoft.Json;
using System;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(MainLauncher = false, LaunchMode = Android.Content.PM.LaunchMode.SingleTop, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class OtpActivity : AppCompatActivity
    {
        public static OtpActivity OTP_Activity;
        public LoginActivity login;
        static TextView txtAleart, txtRef, txtNumber;
        static String otp;
        ImageButton imgBack;
        TextView edit1, edit2, edit3, edit4, edit5, edit6;
        VerifyOTP verify;
        string getlayout, SettingPrinter, PhoneNumber, PhoneNumberOld;
        EditText textOTP;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.otp_activity);
                OTP_Activity = this;

                edit1 = FindViewById<TextView>(Resource.Id.editText1);
                edit2 = FindViewById<TextView>(Resource.Id.editText2);
                edit3 = FindViewById<TextView>(Resource.Id.editText3);
                edit4 = FindViewById<TextView>(Resource.Id.editText4);
                edit5 = FindViewById<TextView>(Resource.Id.editText5);
                edit6 = FindViewById<TextView>(Resource.Id.editText6);

                textOTP = FindViewById<EditText>(Resource.Id.textOTP);
                textOTP.TextChanged += TextOTP_TextChanged;

                txtRef = FindViewById<TextView>(Resource.Id.txtref);
                txtNumber = FindViewById<TextView>(Resource.Id.txtnumber);

                imgBack = FindViewById<ImageButton>(Resource.Id.imgBack);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += BtnBack_Click;
                imgBack.Click += BtnBack_Click;
                txtAleart = FindViewById<TextView>(Resource.Id.txtAleart);
                txtAleart.Text = "OTP ของคุณไม่ถูกต้อง";
                txtAleart.Visibility = ViewStates.Gone;

                var data = JsonConvert.DeserializeObject<VerifyOTP>(Intent.GetStringExtra("refstring"));
                getlayout = Intent.GetStringExtra("Layout");
                if (data != null)
                {
                    txtNumber.Text = data.OwnerID;
                    PhoneNumber = data.OwnerID;
                    if (data.RefOTP.Length > 5)
                    {
                        txtRef.Visibility = ViewStates.Invisible;
                        txtAleart.Visibility = ViewStates.Visible;
                        txtAleart.Text = "Error Code : " + data.RefOTP;
                    }
                    else
                    {
                        txtRef.Text = "Ref = " + data.RefOTP;

                    }
                    verify = data;
                }
                else
                {
                    txtNumber.Text = string.Empty;
                    txtRef.Text = "Ref = " + string.Empty;
                    verify = data;
                }

                SettingPrinter = Preferences.Get("Setting", "");
                PhoneNumberOld = Preferences.Get("PhoneNumber", "");

                _ = TinyInsights.TrackPageViewAsync("OnCreate : OTP");

            }
            catch (Exception ex)
            {
                textOTP.Text = "";
                Log.Debug("error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            textOTP.Text = "";
            //base.OnBackPressed();
            MainDialog dialog = new MainDialog();
            Bundle bundle = new Bundle();
            String myMessage = Resource.Layout.logout_dialog_main.ToString();
            bundle.PutString("message", myMessage);
            dialog.Arguments = bundle;
            dialog.Show(SupportFragmentManager, myMessage);

        }
        public override void OnBackPressed()
        {
            textOTP.Text = "";
            //base.OnBackPressed();
            MainDialog dialog = new MainDialog();
            Bundle bundle = new Bundle();
            String myMessage = Resource.Layout.logout_dialog_main.ToString();
            bundle.PutString("message", myMessage);
            dialog.Arguments = bundle;
            dialog.Show(SupportFragmentManager, myMessage);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        private void Textotp()
        {
            otp = textOTP.Text.Trim();
            switch (otp.Length)
            {
                case 0:
                    edit1.Text = "";
                    edit2.Text = "";
                    edit3.Text = "";
                    edit4.Text = "";
                    edit5.Text = "";
                    edit6.Text = "";
                    break;
                case 1:
                    edit1.Text = otp.Substring(0, 1);
                    edit2.Text = "";
                    edit3.Text = "";
                    edit4.Text = "";
                    edit5.Text = "";
                    edit6.Text = "";
                    break;
                case 2:
                    edit1.Text = otp.Substring(0, 1);
                    edit2.Text = otp.Substring(1, 1);
                    edit3.Text = "";
                    edit4.Text = "";
                    edit5.Text = "";
                    edit6.Text = "";
                    break;
                case 3:
                    edit1.Text = otp.Substring(0, 1);
                    edit2.Text = otp.Substring(1, 1);
                    edit3.Text = otp.Substring(2, 1);
                    edit4.Text = "";
                    edit5.Text = "";
                    edit6.Text = "";
                    break;
                case 4:
                    edit1.Text = otp.Substring(0, 1);
                    edit2.Text = otp.Substring(1, 1);
                    edit3.Text = otp.Substring(2, 1);
                    edit4.Text = otp.Substring(3, 1);
                    edit5.Text = "";
                    edit6.Text = "";
                    break;
                case 5:
                    edit1.Text = otp.Substring(0, 1);
                    edit2.Text = otp.Substring(1, 1);
                    edit3.Text = otp.Substring(2, 1);
                    edit4.Text = otp.Substring(3, 1);
                    edit5.Text = otp.Substring(4, 1);
                    edit6.Text = "";
                    break;
                case 6:
                    edit1.Text = otp.Substring(0, 1);
                    edit2.Text = otp.Substring(1, 1);
                    edit3.Text = otp.Substring(2, 1);
                    edit4.Text = otp.Substring(3, 1);
                    edit5.Text = otp.Substring(4, 1);
                    edit6.Text = otp.Substring(5, 1);
                    AutocheckOtp();
                    break;
                default:
                    break;
            }

        }
        private void AutocheckOtp()
        {
            otp = textOTP.Text.Trim();

            if (otp.Length == 6)
            {
                //ตรวจสอบรหัส OTP     
                verify.OTP = otp;
                switch (getlayout)
                {
                    //Login Owner
                    case "LoginOwner":
                        GetTokenOwner();
                        break;
                    //Create Account
                    default:
                        UpdateMerchant();
                        break;
                }
            }
            else
            {
                return;
            }
        }
         
        private void TextOTP_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            Textotp();
        }

        public async void GetTokenOwner()
        {
            try
            {
                LoginActivity login = new LoginActivity();
                //ตรวจสอบ OTP 
                ResultAPI resultAPI = await GabanaAPI.LoginGabana(verify);
                if (!resultAPI.Status)
                {
                    var result = resultAPI.Message;
                    string ShowError = UtilsAll.CheckErrorGetToken(result);
                    if (ShowError == "CloudProductExpired" || ShowError == "invalid_grant: Cloud Product Expired")
                    {
                        var verifyotp = JsonConvert.SerializeObject(verify);
                        Preferences.Set("VerifyOTP", verifyotp);

                        // ไปหน้า Renew  
                        // แสดง dialog expire ของ Owner  -> ไปหน้า Renew 
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.login_dialog_expiry.ToString();
                        bundle.PutString("message", myMessage);
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                    }
                    else
                    {
                        Toast.MakeText(this, resultAPI.Message, ToastLength.Short).Show();
                    }                   
                    return;
                }

                //get CreateDB มาเก็บไว้ก่อนจะเคลียร์
                DataCashing.flagProgress = false;
                DataCashingAll.UserAccountInfo = null;
                //string CreateDB = Preferences.Get("CreateDB", "");
                var DeviceToken = Preferences.Get("NotificationDeviceToken", "");
                Preferences.Clear();
                Preferences.Set("NotificationDeviceToken", DeviceToken);               

                //GetTokenOwner
                GabanaAPI.gbnJWT = await GetToken.GetgbnJWTForOwner(verify);
                if (GabanaAPI.gbnJWT == null)
                {
                    Toast.MakeText(this, "ไม่สามารถขอ JWT ได้", ToastLength.Short).Show();
                    return;
                }

                //DataCashingAll.UserActive = DateTime.Now; //stamp เวลาในการเข้าใช้งาน
                Preferences.Set("UserActive", DateTime.Now.ToString());
                Preferences.Set("ViewPos", "Grid");
                Preferences.Set("AppState", "login");
                Preferences.Set("LoginType", "owner");
                //Preferences.Set("CreateDB", CreateDB);
                Preferences.Set("User", "Owner");
                Preferences.Set("PhoneNumber", PhoneNumber);
                Preferences.Set("Branch", string.Empty);
                string vername = "";
                vername = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName?.ToString();
                Preferences.Set("VersionName", vername);
                if (PhoneNumberOld == PhoneNumber)
                {
                    Preferences.Set("Setting", SettingPrinter);
                }

                StartActivity(typeof(SplashActivity));
                this.Finish();
            }
            catch (Exception ex)
            {
                string ShowError = UtilsAll.CheckErrorGetToken(ex.Message);
                if (ShowError == "invalid_grant: User or Password or Code are incorrect" || ShowError == "UserPassIncorrect")
                {
                    Toast.MakeText(this, GetString(Resource.String.dataincorrect), ToastLength.Long).Show();
                }
                else if (ShowError == "CloudProductExpired"  || ShowError == "invalid_grant: Cloud Product Expired")
                {
                    // ไปหน้า Renew  
                    // แสดง dialog expire ของ Owner  -> ไปหน้า Renew 
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.login_dialog_expiry.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                }
                else
                {
                    Toast.MakeText(this, ShowError, ToastLength.Long).Show();
                    _ = TinyInsights.TrackErrorAsync(ex);
                    _ = TinyInsights.TrackPageViewAsync("GetTokenOwner at OTP");
                }
                textOTP.Text = "";
            }
        }

        public async void UpdateMerchant()
        {
            try
            {
                //Detail Merchant 
                ResultAPI resultAPI = await GabanaAPI.CreateGabana(verify);

                if (!resultAPI.Status)
                {
                    Toast.MakeText(this, "ไม่สามารถแสดงข้อมูล Merchant ได้", ToastLength.Short).Show();
                    return;
                }

                DataCashing.flagProgress = false;
                DataCashingAll.UserAccountInfo = null;
                //string CreateDB = Preferences.Get("CreateDB", "");
                var DeviceToken = Preferences.Get("NotificationDeviceToken", "");
                Preferences.Clear();
                Preferences.Set("NotificationDeviceToken", DeviceToken);

                //JWT2
                var accessToken = await GetToken.GetgbnJWTForOwner(verify);
                if (accessToken == null)
                {
                    Toast.MakeText(this, "ไม่สามารถขอ JWT ได้", ToastLength.Short).Show();
                    return;
                }
                //DataCashingAll.UserActive = DateTime.Now; //stamp เวลาในการเข้าใช้งาน
                Preferences.Set("UserActive", DateTime.Now.ToString());
                Preferences.Set("ViewPos", "Grid");
                Preferences.Set("AppState", "login");
                Preferences.Set("LoginType", "owner");
                Preferences.Set("User", "Owner");
                //Preferences.Set("CreateDB", CreateDB);
                StartActivity(typeof(SplashActivity));
                this.Finish();
            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateMerchant at OTP");
                Log.Debug("error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        bool deviceAsleep = false;
        bool openPage = false;
        public DateTime pauseDate = DateTime.Now;
        

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

