using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Newtonsoft.Json;
using System;
using System.Threading;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    public class Subscriped_Dialog : Android.Support.V4.App.DialogFragment
    {
        Button btn_save;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Subscriped_Dialog NewInstance()
        {
            var frag = new Subscriped_Dialog { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.offline_dialog_main, container, false);
            try
            {
                TextView textUsepoint = view.FindViewById<TextView>(Resource.Id.textUsepoint);
                var txt1 = Application.Context.GetString(Resource.String.package_activity_success);
                textUsepoint.Text = txt1;
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
                btn_save.Click += BtnOK_Click;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private async void BtnOK_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(this.Activity.SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                if (RenewPackageActivity.activity == null)
                {
                    //ปกติ settingPackage -> setting
                    if (PackageActivity.activity != null)
                    {
                        PackageActivity.activity.Finish();
                    }

                    if (SettingPackageActivity.setting != null)
                    {
                        SettingPackageActivity.setting.Finish();
                    }

                    StartActivity(new Intent(Application.Context, typeof(SettingPackageActivity)));
                    MainDialog.CloseDialog();
                }
                else
                {
                    btn_save.Enabled = false;
                    //27/01/66 เพิ่มจังหวะหน่วงเวลา 5 วินาที
                    Thread.Sleep(5000);

                    VerifyOTP verifyOTP = new VerifyOTP();
                    var Getverifyotp = Preferences.Get("VerifyOTP", "");
                    if (Getverifyotp == "")
                    {
                        btn_save.Enabled = true;
                        StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                        MainDialog.CloseDialog();
                        //Case Renew ออกไปที่ Login
                        if (RenewPackageActivity.activity != null)
                        {
                            RenewPackageActivity.activity.Finish();
                        }
                        return;
                    }

                    verifyOTP = JsonConvert.DeserializeObject<VerifyOTP>(Getverifyotp);
                    ResultAPI resultAPI = await GabanaAPI.LoginGabana(verifyOTP);
                    if (!resultAPI.Status) 
                    {
                        StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                        MainDialog.CloseDialog();
                        if (RenewPackageActivity.activity != null)
                        {
                            RenewPackageActivity.activity.Finish();
                        }
                        return;
                    }

                    #region Check Token ก่อนจะได้ไม่ต้องไป login ใหม่
                    //expiredate has Token
                    if (!string.IsNullOrEmpty(GabanaAPI.gbnJWT))
                    {
                        if (!await GabanaAPI.isValidToken(GabanaAPI.gbnJWT))
                        {
                            char LoginType = UtilsAll.GetLoginTypeToRefreshToken();
                            GabanaAPI.gbnJWT = await Gabana.ShareSource.GetToken.RefreshToken(Preferences.Get("RefreshToken", ""), LoginType);
                        }
                    }

                    //Login Token ว่าง
                    TokenResult res = await TokenServiceBase.GetToken();
                    if (!res.status)
                    {
                        GabanaAPI.gbnJWT = await GetToken.GetgbnJWTForOwner(verifyOTP);
                    }

                    //DataCashingAll.UserActive = DateTime.Now; //stamp เวลาในการเข้าใช้งาน
                    Preferences.Set("UserActive", DateTime.Now.ToString());
                    Preferences.Set("ViewPos", "Grid");
                    Preferences.Set("AppState", "login");
                    Preferences.Set("LoginType", "owner");
                    Preferences.Set("User", "Owner");
                    #endregion                                       

                    btn_save.Enabled = true;
                    StartActivity(new Intent(Application.Context, typeof(SplashActivity)));
                    MainDialog.CloseDialog();
                    //Case Renew ออกไปที่ Splash
                    if (RenewPackageActivity.activity != null)
                    {
                        RenewPackageActivity.activity.Finish();
                    }
                }

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception)
            {

                btn_save.Enabled = true;
                StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                MainDialog.CloseDialog();
                //Case Renew ออกไปที่ Splash
                if (RenewPackageActivity.activity != null)
                {
                    RenewPackageActivity.activity.Finish();
                }

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
        }
    }
}