using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class TermActivity : AppCompatActivity
    {

        Button btnSave;
        Context context = Android.App.Application.Context;
        ImageView imgTerm, imgPolicy;
        public static bool term, policy;
        public static TermActivity activity;
        public static Gabana3.JAM.Merchant.Merchants MerchantDetail;
        private static string ipage;
        FrameLayout frameAccept;

        protected async override void OnCreate(Bundle savedInstanceState)
        {

            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.term_activity);

                CheckJwt();
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                activity = this;
                TextView textTitle = FindViewById<TextView>(Resource.Id.textTitle);
                textTitle.Text = "Terms of Service & Privacy Policy";
                term = false;
                policy = false;

                frameAccept = FindViewById<FrameLayout>(Resource.Id.frameAccept);
                FrameLayout lnTerm = FindViewById<FrameLayout>(Resource.Id.lnTerm);
                FrameLayout lnPolicy = FindViewById<FrameLayout>(Resource.Id.lnPolicy);
                imgTerm = FindViewById<ImageView>(Resource.Id.imgTerm);
                imgPolicy = FindViewById<ImageView>(Resource.Id.imgPolicy);
                btnSave = FindViewById<Button>(Resource.Id.btnSave);

                if (ipage == "Main")
                {
                    frameAccept.Visibility = ViewStates.Gone;
                }
                else
                {
                    frameAccept.Visibility = ViewStates.Visible;
                }
                LinearLayout linearLayout1 = FindViewById<LinearLayout>(Resource.Id.linearLayout1);
                if (Preferences.Get("LoginType", "").ToLower() == "owner")
                {
                    linearLayout1.Visibility = ViewStates.Visible;
                }
                else
                {
                    linearLayout1.Visibility = ViewStates.Gone;
                }

                lnTerm.Click += LnTerm_Click;
                lnPolicy.Click += LnPolicy_Click;

                btnSave.Click += BtnSave_Click;

                if (DataCashingAll.Merchant != null)
                {
                    MerchantDetail = DataCashingAll.Merchant;
                    if (MerchantDetail.Merchant.FTermConditions == 'Y')
                    {
                        term = true;
                    }
                    else
                    {
                        term = false;
                    }
                    if (MerchantDetail.Merchant.FPrivacyPolicy == 'Y')
                    {
                        policy = true;
                    }
                    else
                    {
                        policy = false;
                    }
                }
                SetImageTermPolicy();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : TermActivity");


            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Term");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        protected override void OnResume()
        {
            try
            {
                base.OnResume();
            }
            catch (Exception)
            {
                base.OnRestart();
            }
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            UpdateMerchant();
        }
        public void SetImageTermPolicy()
        {

            //setimageTerm
            if (term)
            {
                imgTerm.SetBackgroundResource(Resource.Mipmap.OptionCheck);
            }
            else
            {
                imgTerm.SetBackgroundResource(Resource.Mipmap.OptionBlank);
            }

            if (policy)
            {
                imgPolicy.SetBackgroundResource(Resource.Mipmap.OptionCheck);
            }
            else
            {
                imgPolicy.SetBackgroundResource(Resource.Mipmap.OptionBlank);
            }


            if (term && policy)
            {
                btnSave.SetBackgroundResource(Resource.Drawable.btnblue);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnSave.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primary, null));
            }
        }
        private async void LnPolicy_Click(object sender, EventArgs e)
        {
            if (policy)
            {
                policy = false;
            }
            else
            {
                policy = true;
            }
            SetImageTermPolicy();
            if (ipage == "Main")
            {
                if (!term)
                {
                    MainDialog dialog = new MainDialog() { Cancelable = false };
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.term_dialog_confirm.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    Termpolicy_Dialog_Confirm.SetAction("policy");
                }
                else
                {
                    var updatemerchant = await GabanaAPI.PutMerchant(MerchantDetail, null);
                    if (updatemerchant.Status)
                    {
                        Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                        DataCashingAll.Merchant.Merchant = MerchantDetail.Merchant;

                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                        return;
                    }
                }
            }
        }
        private async void LnTerm_Click(object sender, EventArgs e)
        {

            if (term)
            {
                term = false;
            }
            else
            {
                term = true;
            }
            SetImageTermPolicy();

            if (ipage == "Main")
            {
                if (!term)
                {
                    MainDialog dialog = new MainDialog() { Cancelable = false };
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.term_dialog_confirm.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    Termpolicy_Dialog_Confirm.SetAction("term");
                }
                else
                {
                    var updatemerchant = await GabanaAPI.PutMerchant(MerchantDetail, null);
                    if (updatemerchant.Status)
                    {
                        Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                        DataCashingAll.Merchant.Merchant = MerchantDetail.Merchant;

                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                        return;
                    }
                }
            }
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            if (ipage == "Splash" || !term || !policy)
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.logout_dialog_main.ToString();
                bundle.PutString("message", myMessage);
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }

            if (ipage == "Main" && term && policy)
            {
                base.OnBackPressed();
            }


        }
        async void UpdateMerchant()
        {
            try
            {
                string usernamelogin = Preferences.Get("User", "");

                if (term)
                {
                    MerchantDetail.Merchant.FTermConditions = 'Y';
                }
                else
                {
                    MerchantDetail.Merchant.FTermConditions = 'N';

                }
                if (policy)
                {
                    MerchantDetail.Merchant.FPrivacyPolicy = 'Y';
                }
                else
                {
                    MerchantDetail.Merchant.FPrivacyPolicy = 'N';
                }

                MerchantDetail.Merchant.UserNameModified = usernamelogin;

                if (!term || !policy)
                {
                    MainDialog dialog = new MainDialog() { Cancelable = false };
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.term_dialog_confirm.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);

                }
                else
                {
                    var updatemerchant = await GabanaAPI.PutMerchant(MerchantDetail, null);
                    if (updatemerchant.Status)
                    {
                        Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                        DataCashingAll.Merchant.Merchant = MerchantDetail.Merchant;
                        this.Finish();

                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                        return;
                    }
                }


            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateMerchant at Term");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        internal static void Setpage(string v)
        {
            ipage = v;
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'TermActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'TermActivity.openPage' is assigned but its value is never used
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
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                PinCodeActitvity.SetPincode("Pincode");
                openPage = true;
            }
        }
    }
}

