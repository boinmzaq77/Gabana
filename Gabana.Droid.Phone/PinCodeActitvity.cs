using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class PinCodeActitvity : AppCompatActivity
    {
        FrameLayout btnDelete;
        ImageView imgPin1, imgPin2, imgPin3, imgPin4, imgPin5, imgPin6;
        Button btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9, btnnumber0;
        TextView txtIncorrect, txtForget, textTitle1, textTitle2;
        public PinCodeActitvity pinCodeActitvity;
        string Pincode = string.Empty, usernamelogin, PincodeCorrect;
        UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
        string pin1 = null, pin2 = null;
        TextView textTypePincode;
        public static string typePage;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.pincode_activity);
                btnDelete = FindViewById<FrameLayout>(Resource.Id.btnDelete);
                btnDelete.Click += BtnDelete_Click;
                imgPin1 = FindViewById<ImageView>(Resource.Id.imgPin1);
                imgPin2 = FindViewById<ImageView>(Resource.Id.imgPin2);
                imgPin3 = FindViewById<ImageView>(Resource.Id.imgPin3);
                imgPin4 = FindViewById<ImageView>(Resource.Id.imgPin4);
                imgPin5 = FindViewById<ImageView>(Resource.Id.imgPin5);
                imgPin6 = FindViewById<ImageView>(Resource.Id.imgPin6);
                btnnumber0 = FindViewById<Button>(Resource.Id.btnnumber0);
                btnnumber1 = FindViewById<Button>(Resource.Id.btnnumber1);
                btnnumber2 = FindViewById<Button>(Resource.Id.btnnumber2);
                btnnumber3 = FindViewById<Button>(Resource.Id.btnnumber3);
                btnnumber4 = FindViewById<Button>(Resource.Id.btnnumber4);
                btnnumber5 = FindViewById<Button>(Resource.Id.btnnumber5);
                btnnumber6 = FindViewById<Button>(Resource.Id.btnnumber6);
                btnnumber7 = FindViewById<Button>(Resource.Id.btnnumber7);
                btnnumber8 = FindViewById<Button>(Resource.Id.btnnumber8);
                btnnumber9 = FindViewById<Button>(Resource.Id.btnnumber9);
                txtIncorrect = FindViewById<TextView>(Resource.Id.txtIncorrect);
                txtIncorrect.Visibility = ViewStates.Gone;

                txtForget = FindViewById<TextView>(Resource.Id.txtForget);
                txtForget.Click += TxtForget_Click;

                textTitle1 = FindViewById<TextView>(Resource.Id.textTitle1);
                textTitle2 = FindViewById<TextView>(Resource.Id.textTitle2);

                textTypePincode = FindViewById<TextView>(Resource.Id.textTypePincode);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                LinearLayout linearback = FindViewById<LinearLayout>(Resource.Id.linearback);
                lnBack.Click += LnBack_Click;
                btnnumber0.Click += Btnnumber0_Click;
                btnnumber1.Click += Btnnumber1_Click;
                btnnumber2.Click += Btnnumber2_Click;
                btnnumber3.Click += Btnnumber3_Click;
                btnnumber4.Click += Btnnumber4_Click;
                btnnumber5.Click += Btnnumber5_Click;
                btnnumber6.Click += Btnnumber6_Click;
                btnnumber7.Click += Btnnumber7_Click;
                btnnumber8.Click += Btnnumber8_Click;
                btnnumber9.Click += Btnnumber9_Click;

                usernamelogin = Preferences.Get("User", "");
                txtForget.Visibility = ViewStates.Gone;
                txtIncorrect.Visibility = ViewStates.Gone;
                textTitle1.Visibility = ViewStates.Gone;
                textTitle2.Visibility = ViewStates.Gone;
                lnBack.Visibility = ViewStates.Visible;
                linearback.Visibility = ViewStates.Visible;
                textTypePincode.Text = Application.Context.GetString(Resource.String.pincode_activity_enterpincode);

                CheckJwt();
                //await GetUserAccount();

                //set ui 
                switch (typePage)
                {
                    case "NewPincode": /// SetPincode From ChangePassword Activity
                        textTitle2.Visibility = ViewStates.Visible;
                        textTitle2.Text = Application.Context.GetString(Resource.String.pincode_activity_settingpincode);
                        textTypePincode.Text = Application.Context.GetString(Resource.String.pincode_activity_newpincode);
                        break;
                    case "ChangePincode": /// SetPincode From ChangePassword Activity
                        textTitle2.Visibility = ViewStates.Visible;
                        textTitle2.Text = Application.Context.GetString(Resource.String.pincode_activity_changepincode);
                        textTypePincode.Text = Application.Context.GetString(Resource.String.pincode_activity_newpincode);
                        break;
                    case "Pincode":
                        lnBack.Visibility = ViewStates.Gone;
                        linearback.Visibility = ViewStates.Gone;
                        textTitle1.Visibility = ViewStates.Visible;
                        txtForget.Visibility = ViewStates.Visible;
                        break;
                    case "OffPincode":
                        textTitle1.Visibility = ViewStates.Visible;
                        txtForget.Visibility = ViewStates.Visible;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Pincode");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        async Task GetUserAccount()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

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
                        await accountInfoManage.InsertorReplaceUserAccount(UpdatelocalUser);
                    }
                }
                
                var Data = await accountInfoManage.GetUserAccount(DataCashingAll.MerchantId, usernamelogin.ToLower());
                if (Data != null)
                {
                    PincodeCorrect = Data.PinCode;
                    var u = Data.FUsePincode;
                }

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetUserAccount at Pincode");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        internal static void SetPincode(string v)
        {
            typePage = v;
        }
        private async void TxtForget_Click(object sender, EventArgs e)
        {
            try
            {
                var userforget = Preferences.Get("User", "");
                Preferences.Set("AppState", "logout");
                Preferences.Set("UserForgetPincode", userforget);
                Intent intent = new Intent(Application.Context, typeof(SplashActivity));
                StartActivity(intent);
                this.Finish();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TxtForget_Click at Pincode");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }
        private void Btnnumber9_Click(object sender, EventArgs e)
        {
            SetPinCode(btnnumber9);
        }
        private void Btnnumber8_Click(object sender, EventArgs e)
        {
            SetPinCode(btnnumber8);
        }
        private void Btnnumber7_Click(object sender, EventArgs e)
        {
            SetPinCode(btnnumber7);
        }
        private void Btnnumber6_Click(object sender, EventArgs e)
        {
            SetPinCode(btnnumber6);
        }
        private void Btnnumber5_Click(object sender, EventArgs e)
        {
            SetPinCode(btnnumber5);
        }
        private void Btnnumber4_Click(object sender, EventArgs e)
        {
            SetPinCode(btnnumber4);
        }
        private void Btnnumber3_Click(object sender, EventArgs e)
        {
            SetPinCode(btnnumber3);
        }
        private void Btnnumber2_Click(object sender, EventArgs e)
        {
            SetPinCode(btnnumber2);
        }
        private void Btnnumber1_Click(object sender, EventArgs e)
        {
            SetPinCode(btnnumber1);
        }
        private void Btnnumber0_Click(object sender, EventArgs e)
        {
            SetPinCode(btnnumber0);
        }
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (Pincode.Length == 0)
                {
                    txtIncorrect.Visibility = ViewStates.Gone;
                }
                if (Pincode.Length > 0)
                {
                    Pincode = Pincode.Remove(Pincode.Length - 1);
                    SetViewPinCode();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnDelete_Click at Pincode");
                return;
            }
        }
        public async void SetPinCode(Button btn)
        {
            try
            {
                Pincode += btn.Text;
                SetViewPinCode();

                if (Pincode.Length == 6)
                {
                    if (string.IsNullOrEmpty(PincodeCorrect))
                    {
                        await GetUserAccount();
                    }
                    ChechKPincode();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetPinCode at Pincode");
                return;
            }
        }

        private void ChechKPincode()
        {
            switch (typePage)
            {
                case "NewPincode": /// SetPincode From ChangePassword Activity
                    if (pin1 == null)
                    {
                        pin1 = Pincode;
                        Pincode = string.Empty;
                        textTypePincode.Text = Application.Context.GetString(Resource.String.pincode_activity_confirmnewpincode);
                        SetViewPinCode();
                        return;
                    }
                    else
                    {
                        pin2 = Pincode;
                    }

                    if (pin1 != pin2)
                    {
                        txtIncorrect.Visibility = ViewStates.Visible;
                        Pincode = string.Empty;
                        textTypePincode.Text = Application.Context.GetString(Resource.String.pincode_activity_confirmnewpincode);
                        SetViewPinCode();
                    }
                    else
                    {
                        ChangePasswordActivity.SetPinCode(pin1, 1);
                        this.Finish();
                    }

                    break;
                case "ChangePincode":                    
                    if (pin1 == null)
                    {
                        pin1 = Pincode;
                        Pincode = string.Empty;
                        textTypePincode.Text = Application.Context.GetString(Resource.String.pincode_activity_confirmnewpincode);
                        SetViewPinCode();
                        return;
                    }
                    else
                    {
                        pin2 = Pincode;
                    }

                    if (pin1 != pin2)
                    {
                        txtIncorrect.Visibility = ViewStates.Visible;
                        Pincode = string.Empty;
                        textTypePincode.Text = Application.Context.GetString(Resource.String.pincode_activity_confirmnewpincode);
                        SetViewPinCode();
                    }
                    else
                    {
                        ChangePasswordActivity.SetPinCode(pin1, 1);
                        ChangePasswordActivity.typepagepincode = typePage;
                        this.Finish();
                    }

                    break;
                case "OffPincode":
                    pin1 = Pincode;
                    if (PincodeCorrect == pin1)
                    {
                        ChangePasswordActivity.SetPinCode(null, 0);
                        this.Finish();
                    }
                    else
                    {
                        txtIncorrect.Visibility = ViewStates.Visible;
                        Pincode = string.Empty;
                        textTypePincode.Text = Application.Context.GetString(Resource.String.pincode_activity_enterpincode);
                        SetViewPinCode();
                    }
                    break;
                case "Pincode":
                    if (Pincode == PincodeCorrect)
                    {
                        //DataCashingAll.UserActive = DateTime.Now;
                        Preferences.Set("UserActive", DateTime.Now.ToString());
                        ChangePasswordActivity.typepagepincode = typePage;
                        this.Finish();
                    }
                    else
                    {
                        txtIncorrect.Visibility = ViewStates.Visible;
                        Pincode = string.Empty;
                        textTypePincode.Text = Application.Context.GetString(Resource.String.pincode_activity_enterpincode);
                        ChangePasswordActivity.typepagepincode = typePage;
                        SetViewPinCode();
                    }
                    break;
                default:
                    break;
            }
        }
        void SetViewPinCode()
        {
            imgPin1.SetImageResource(Resource.Mipmap.UserInactive);
            imgPin2.SetImageResource(Resource.Mipmap.UserInactive);
            imgPin3.SetImageResource(Resource.Mipmap.UserInactive);
            imgPin4.SetImageResource(Resource.Mipmap.UserInactive);
            imgPin5.SetImageResource(Resource.Mipmap.UserInactive);
            imgPin6.SetImageResource(Resource.Mipmap.UserInactive);

            switch (Pincode.Length)
            {
                case 1:
                    imgPin1.SetImageResource(Resource.Mipmap.UserActive);
                    txtIncorrect.Visibility = ViewStates.Gone;
                    break;
                case 2:
                    imgPin1.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin2.SetImageResource(Resource.Mipmap.UserActive);
                    break;
                case 3:
                    imgPin1.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin2.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin3.SetImageResource(Resource.Mipmap.UserActive);
                    break;
                case 4:
                    imgPin1.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin2.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin3.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin4.SetImageResource(Resource.Mipmap.UserActive);
                    break;
                case 5:
                    imgPin1.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin2.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin3.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin4.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin5.SetImageResource(Resource.Mipmap.UserActive);
                    break;
                case 6:
                    imgPin1.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin2.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin3.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin4.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin5.SetImageResource(Resource.Mipmap.UserActive);
                    imgPin6.SetImageResource(Resource.Mipmap.UserActive);
                    break;
                default:
                    //imgPin1.SetImageResource(Resource.Mipmap.UserInactive);
                    break;
            }
        }

        public override void OnBackPressed()
        {
            switch (typePage)
            {
                case "NewPincode": /// SetPincode From ChangePassword Activity
                    ChangePasswordActivity.SetPinCode(null, 0);
                    this.Finish();
                    break;
                case "ChangePincode": /// SetPincode From ChangePassword Activity
                    ChangePasswordActivity.SetPinCode(PincodeCorrect, 1);
                    this.Finish();
                    break;
                case "Pincode":

                    break;
                case "OffPincode":
                    ChangePasswordActivity.SetPinCode(PincodeCorrect, 1);
                    this.Finish();
                    break;
                default:
                    break;
            }
        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                usernamelogin = Preferences.Get("User", "");
                await GetUserAccount();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this,ex.Message,ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at Pincode");
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
        }
    }
}