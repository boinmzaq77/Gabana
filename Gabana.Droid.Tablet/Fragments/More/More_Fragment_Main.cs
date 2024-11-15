using Android.App;
using Android.Content;
using Android.Gms.Common.Util;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.CardView.Widget;
using BellNotificationHub.Xamarin.Android;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.More
{
    public class More_Fragment_Main : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static More_Fragment_Main NewInstance()
        {
            More_Fragment_Main frag = new More_Fragment_Main();
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.more_fragment_main, container, false);
            try
            {
                ComBineUI();

                CheckJwt();

                SetupTimer();

                SetupClearRam();

                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    if (!BellNotification.IsRegisted())
                    {
                        BellNotificationHelper.RegisterBellNotification(this.Activity, GabanaAPI.gbnJWT, DataCashingAll.MerchantId, DataCashingAll.DeviceNo);
                    }
                }
                Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            }
            catch (Exception)
            {

            }

            return view;
        }

        public async Task TimerResentData()
        {
            try
            {
                if (Activity != null)
                {
                    Activity.RunOnUiThread(async () =>
                    {
                        if (await GabanaAPI.CheckNetWork())
                        {
                            if (!BellNotification.IsRegisted())
                            {
                                await BellNotificationHelper.RegisterBellNotification(this.Activity, GabanaAPI.gbnJWT, DataCashingAll.MerchantId, DataCashingAll.DeviceNo);
                            }

                            ResendData();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("TimerResentData at Main");
                Log.Debug("connectpass", "" + "error TimerResentData");
            }
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            try
            {
                if (e.NetworkAccess == NetworkAccess.Internet)
                {
                    var access = e.NetworkAccess;
                    var profiles = e.ConnectionProfiles;
                    //-----------------------------------------------------------
                    //Resend Fwaiting = 1
                    //-----------------------------------------------------------
                    this.Activity.RunOnUiThread(() =>
                    {
                        Task.Delay(5000);
                        ResendData();
                    });
                }
                else
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private System.Threading.Timer timer, timerclearRAM;
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


        Utils utils;
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
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ResendData at Main");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short);
                Log.Debug("connectpass", "" + "error ResendData");
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
                    this.Activity.Finish();
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

        public override async void OnResume()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}


                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }

                await ShowDetail();

                dialogLoading?.DismissAllowingStateLoss();
                dialogLoading?.Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        View view;
        ImageView imgProfile;
        TextView txtBranchName, txtMerchantName, txtEmployeeName, txtPackageName;
        CardView cardViewImage;
        CardView cardViewColor;
        private void ComBineUI()
        {
            try
            {
                cardViewImage = view.FindViewById<CardView>(Resource.Id.cardViewImage);
                imgProfile = view.FindViewById<ImageView>(Resource.Id.imgProfile);

                LinearLayout lnHead = view.FindViewById<LinearLayout>(Resource.Id.lnHead);
                FrameLayout framProfile = view.FindViewById<FrameLayout>(Resource.Id.framProfile);

                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                lnHead.LayoutParameters.Height = Convert.ToInt32(mainDisplayInfo.Width);
                var sizeLogo = Convert.ToInt32(((mainDisplayInfo.Width - 40) / 3) * 0.5);
                var sizeBorder = Convert.ToInt32((mainDisplayInfo.Width - 80) / 3);
                framProfile.LayoutParameters.Height = 110;
                framProfile.LayoutParameters.Width = 110;
                cardViewColor = view.FindViewById<CardView>(Resource.Id.cardViewColor);
                cardViewColor.Radius = 55;
                cardViewImage.Radius = (float)52.50;

                TextView texthi = view.FindViewById<TextView>(Resource.Id.texthi);
                texthi.TextSize = 25;
                txtEmployeeName = view.FindViewById<TextView>(Resource.Id.txtEmployeeName);
                txtEmployeeName.TextSize = 25;
                TextView txtMerchantID = view.FindViewById<TextView>(Resource.Id.txtMerchantID);
                txtMerchantID.TextSize = 20;
                txtMerchantName = view.FindViewById<TextView>(Resource.Id.txtMerchantName);
                txtMerchantName.TextSize = 20;
                TextView txtBranch = view.FindViewById<TextView>(Resource.Id.txtBranch);
                txtBranch.TextSize = 20;
                txtBranchName = view.FindViewById<TextView>(Resource.Id.txtBranchName);
                txtBranchName.TextSize = 20;
                TextView txtPackage = view.FindViewById<TextView>(Resource.Id.txtPackage);
                txtPackage.TextSize = 20;
                txtPackageName = view.FindViewById<TextView>(Resource.Id.txtPackageName);
                txtPackageName.TextSize = 20;
                TextView txtVersion = view.FindViewById<TextView>(Resource.Id.txtVersion);
                var vername = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName?.ToString();
                long vernumber = 0;

                vernumber = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionCode;
                txtVersion.Text = " " + vername + "." + vernumber;
                LinearLayout btnLogout = view.FindViewById<LinearLayout>(Resource.Id.btnLogout);
                ImageView imgLogout = view.FindViewById<ImageView>(Resource.Id.imgLogout);
                btnLogout.LayoutParameters.Width = 60;
                btnLogout.LayoutParameters.Height = 60;
                imgLogout.LayoutParameters.Width = 28;
                imgLogout.LayoutParameters.Height = 28;
                btnLogout.Click += BtnLogout_Click;

                LinearLayout lnChangePassword = view.FindViewById<LinearLayout>(Resource.Id.lnChangePassword);
                lnChangePassword.Click += LnChangePassword_Click;
                LinearLayout lnChangeBranch = view.FindViewById<LinearLayout>(Resource.Id.lnChangeBranch);
                lnChangeBranch.Click += LnChangeBranch_Click;
                LinearLayout lnChangeLanguage = view.FindViewById<LinearLayout>(Resource.Id.lnChangeLanguage);
                lnChangeLanguage.Click += LnChangeLanguage_Click;
                LinearLayout lnContactUs = view.FindViewById<LinearLayout>(Resource.Id.lnContactUs);
                lnContactUs.Click += LnContactUs_Click;
                LinearLayout lnTerm = view.FindViewById<LinearLayout>(Resource.Id.lnTerm);
                lnTerm.Click += LnTerm_Click;
                LinearLayout lnPackage = view.FindViewById<LinearLayout>(Resource.Id.lnPackage);
                lnPackage.Click += LnPackage_Click;
                LinearLayout lnMyQr = view.FindViewById<LinearLayout>(Resource.Id.lnMyQr);
                lnMyQr.Click += LnMyQr_Click;
                LinearLayout lnGuides = view.FindViewById<LinearLayout>(Resource.Id.lnGuides);
                lnGuides.Click += LnGuides_Click;

                ImageView imgmenupassword = view.FindViewById<ImageView>(Resource.Id.imgmenupassword);
                imgmenupassword.LayoutParameters.Width = 100;
                imgmenupassword.LayoutParameters.Height = 100;
                TextView txtchangepassword = view.FindViewById<TextView>(Resource.Id.txtchangepassword);
                txtchangepassword.TextSize = 20;
                ImageView imgmenubranch = view.FindViewById<ImageView>(Resource.Id.imgmenubranch);
                imgmenubranch.LayoutParameters.Width = 100;
                imgmenubranch.LayoutParameters.Height = 100;
                TextView imgchangebranch = view.FindViewById<TextView>(Resource.Id.imgchangebranch);
                imgchangebranch.TextSize = 20;
                ImageView imgmenulanguage = view.FindViewById<ImageView>(Resource.Id.imgmenulanguage);
                imgmenulanguage.LayoutParameters.Width = 100;
                imgmenulanguage.LayoutParameters.Height = 100;
                TextView imglanguagesetting = view.FindViewById<TextView>(Resource.Id.imglanguagesetting);
                imglanguagesetting.TextSize = 20;
                ImageView imgmenucontact = view.FindViewById<ImageView>(Resource.Id.imgmenucontact);
                imgmenucontact.LayoutParameters.Width = 100;
                imgmenucontact.LayoutParameters.Height = 100;
                TextView imgcontactus = view.FindViewById<TextView>(Resource.Id.imgcontactus);
                imgcontactus.TextSize = 20;
                ImageView imgmenupolicy = view.FindViewById<ImageView>(Resource.Id.imgmenupolicy);
                imgmenupolicy.LayoutParameters.Width = 100;
                imgmenupolicy.LayoutParameters.Height = 100;
                TextView imgtermconditions = view.FindViewById<TextView>(Resource.Id.imgtermconditions);
                imgtermconditions.TextSize = 20;
                ImageView imgmenuguide = view.FindViewById<ImageView>(Resource.Id.imgmenuguide);
                imgmenuguide.LayoutParameters.Width = 100;
                imgmenuguide.LayoutParameters.Height = 100;
                TextView imgguide = view.FindViewById<TextView>(Resource.Id.imgguide);
                imgguide.TextSize = 20;
                ImageView imgmenupackage = view.FindViewById<ImageView>(Resource.Id.imgmenupackage);
                imgmenupackage.LayoutParameters.Width = 100;
                imgmenupackage.LayoutParameters.Height = 100;
                TextView textPack1 = view.FindViewById<TextView>(Resource.Id.textPack1);
                textPack1.TextSize = 20;
                TextView textPack2 = view.FindViewById<TextView>(Resource.Id.textPack2);
                textPack2.TextSize = 20;
                ImageView imgmenumyqr = view.FindViewById<ImageView>(Resource.Id.imgmenumyqr);
                imgmenumyqr.LayoutParameters.Width = 100;
                imgmenumyqr.LayoutParameters.Height = 100;
                TextView txtMyqr = view.FindViewById<TextView>(Resource.Id.txtMyqr);
                txtMyqr.TextSize = 20;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void LnGuides_Click(object sender, EventArgs e)
        {
            try
            {
                var uri = Android.Net.Uri.Parse("https://shorturl.asia/csQfV");
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity,ex.Message,ToastLength.Short).Show();
            }
        }

        private void LnMyQr_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(myQRActivity)));
        }

        private void LnPackage_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(PackageActivity)));
        }

        private void LnTerm_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(TermActivity)));
            TermActivity.Setpage("Main");
        }

        private void LnContactUs_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(ContactUsActivity)));
        }

        private void LnChangeLanguage_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(LanguageSettingActivity)));
        }

        private void LnChangeBranch_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(ChangeBranchActivity)));
        }

        private void LnChangePassword_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(ChangePasswordActivity)));
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            Bundle bundle = new Bundle();
            var fragement = new Logout_Dialog_Main();
            fragement.Arguments = bundle;
            fragement.Show(this.Activity.SupportFragmentManager, nameof(Logout_Dialog_Main));
        }

        string CURRENCYSYMBOLS;
        async Task ShowDetail()
        {
            try
            {
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;

                if (CURRENCYSYMBOLS == null) CURRENCYSYMBOLS = "฿";


                MerchantManage merchantManage = new MerchantManage();
                var GETmerchantlocal = await merchantManage.GetMerchant(DataCashingAll.MerchantId);
                if (GETmerchantlocal == null)
                {
                    Log.Debug("connectpass", "" + "GETmerchantlocal is null");
                    return;
                }

                DataCashingAll.MerchantLocal = GETmerchantlocal;
                var cloudpath = GETmerchantlocal.LogoPath == null ? string.Empty : GETmerchantlocal.LogoPath;
                var localpath = GETmerchantlocal.LogoLocalPath == null ? string.Empty : GETmerchantlocal.LogoLocalPath;

                
                if (await MainActivity.main_activity.RecheckNet())
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

                string Username = Preferences.Get("User", "");
                string branchID = Preferences.Get("Branch", "");
                int.TryParse(branchID, out int result);
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
                        txtBranchName.Text = " " + Getresult.BranchName?.ToString();
                    }
                }

                if (DataCashingAll.MerchantLocal == null)
                {
                    DataCashingAll.MerchantLocal = GETmerchantlocal;
                }
                txtMerchantName.Text = DataCashingAll.MerchantLocal.MerchantID + ", " + DataCashingAll.MerchantLocal.Name;


                if (Username.ToLower() == "owner")
                {
                    var fullnameOwner = DataCashingAll.UserAccountInfo.Where(x => x.UserName.ToLower() == "owner").FirstOrDefault().FullName?.ToString();
                    if (string.IsNullOrEmpty(fullnameOwner))
                    {
                        txtEmployeeName.Text = Username.ToLower();
                    }
                    else
                    {
                        txtEmployeeName.Text = fullnameOwner;
                    }
                }
                else
                {
                    txtEmployeeName.Text = Username.ToLower();
                }

                await MainActivity.main_activity.GetGabanaInfo();
                txtPackageName.Text = MainActivity.main_activity.SetDetailPackage();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowDetail at main");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                Log.Debug("connectpass", "" + "error ShowDetail");
            }
        }
        
    }
}