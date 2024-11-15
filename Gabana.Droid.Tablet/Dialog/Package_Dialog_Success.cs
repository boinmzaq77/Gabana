using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.Setting;
using Gabana.Model;
using Gabana.ShareSource;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Package_Dialog_Success : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Package_Dialog_Success NewInstance()
        {
            var frag = new Package_Dialog_Success { Arguments = new Bundle() };
            return frag;
        }
        Button btnOK;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.package_dialog_success, container, false);
            try
            {
                btnOK = view.FindViewById<Button>(Resource.Id.btnOK);
                btnOK.Click += BtnOK_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }
        private async void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (RenewPackageActivity.activity == null)
                {
                    //ปกติ settingPackage -> setting
                    if (PackageActivity.activity != null)
                    {
                        PackageActivity.activity.Finish();
                    }

                    if (Setting_Fragment_Package.fragment_package != null)
                    {
                        Setting_Fragment_Package.fragment_package.OnResume();
                    }
                    this.Dialog.Dismiss();
                }
                else
                {
                    #region New Code เท่ากับของมือถือ
                    btnOK.Enabled = false;
                    //27/01/66 เพิ่มจังหวะหน่วงเวลา 5 วินาที
                    Thread.Sleep(5000);

                    VerifyOTP verifyOTP = new VerifyOTP();
                    var Getverifyotp = Preferences.Get("VerifyOTP", "");
                    if (Getverifyotp == "")
                    {
                        btnOK.Enabled = true;
                        StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                        this.Dialog.Dismiss();
                        //Case Renew ออกไปที่ Login
                        if (RenewPackageActivity.activity != null)
                        {
                            //RenewPackageActivity.activity.Finish();
                            RenewPackageActivity.activity = null;
                        }
                        return;
                    }

                    verifyOTP = JsonConvert.DeserializeObject<VerifyOTP>(Getverifyotp);
                    ResultAPI resultAPI = await GabanaAPI.LoginGabana(verifyOTP);
                    if (!resultAPI.Status)
                    {
                        StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                        this.Dialog.Dismiss();
                        if (RenewPackageActivity.activity != null)
                        {
                            //RenewPackageActivity.activity.Finish();
                            RenewPackageActivity.activity = null;
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
                    TokenResult restoken = await TokenServiceBase.GetToken();
                    if (!restoken.status)
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

                    btnOK.Enabled = true;
                    StartActivity(new Intent(Application.Context, typeof(SplashActivity)));
                    this.Dialog.Dismiss();
                    //Case Renew ออกไปที่ Splash
                    if (RenewPackageActivity.activity != null)
                    {
                        //RenewPackageActivity.activity.Finish();
                        RenewPackageActivity.activity = null;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Subscriped_Dialog BtnOK_Click");
            }
        }
    }
}