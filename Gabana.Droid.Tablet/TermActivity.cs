using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Gabana.Droid.Tablet.Dialog;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class TermActivity : AppCompatActivity
    {
        public static TermActivity activity;
        public static bool term, policy;
        private static string ipage;
        public static Gabana3.JAM.Merchant.Merchants MerchantDetail;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.term_activity);

                ComebineUI();
               
                activity = this;
                TextView textTitle = FindViewById<TextView>(Resource.Id.textTitle);
                textTitle.Text = "Terms of Service & Privacy Policy";
                string local = Resources.Configuration.Locale.Language.ToString();
                //เก็บไว้ SetText Title นะคะ มี & set จากไฟลล์ String ไม่ได้
                if (local == "en" || local == "es")
                {

                }
                else
                {

                }

                term = false;
                policy = false;
                
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

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Term");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        FrameLayout frameAccept, lnTerm, lnPolicy;
        ImageView imgTerm, imgPolicy;
        Button btnSave;
        private void ComebineUI()
        {
            LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Click += LnBack_Click;
            frameAccept = FindViewById<FrameLayout>(Resource.Id.frameAccept);
            lnTerm = FindViewById<FrameLayout>(Resource.Id.lnTerm);
            lnPolicy = FindViewById<FrameLayout>(Resource.Id.lnPolicy);
            imgTerm = FindViewById<ImageView>(Resource.Id.imgTerm);
            imgPolicy = FindViewById<ImageView>(Resource.Id.imgPolicy);
            btnSave = FindViewById<Button>(Resource.Id.btnSave);

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
                btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
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
                        Toast.MakeText(this, "บันทึกข้อมูลสำเร็จ", ToastLength.Short).Show();
                        DataCashingAll.Merchant.Merchant = MerchantDetail.Merchant;

                    }
                    else
                    {
                        Toast.MakeText(this, "ไม่สามารถบันทึกข้อมูลได้", ToastLength.Short).Show();
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
                        Toast.MakeText(this, "บันทึกข้อมูลสำเร็จ", ToastLength.Short).Show();
                        DataCashingAll.Merchant.Merchant = MerchantDetail.Merchant;

                    }
                    else
                    {
                        Toast.MakeText(this, "ไม่สามารถบันทึกข้อมูลได้", ToastLength.Short).Show();
                        return;
                    }
                }
            }
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            if (ipage == "Splash" || !term || !policy)
            {
                Bundle bundle = new Bundle();
                var fragement = new Logout_Dialog_Main();
                fragement.Arguments = bundle;
                fragement.Show(this.SupportFragmentManager, nameof(Logout_Dialog_Main));
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
                        Toast.MakeText(this, "บันทึกข้อมูลสำเร็จ", ToastLength.Short).Show();
                        DataCashingAll.Merchant.Merchant = MerchantDetail.Merchant;
                        this.Finish();

                    }
                    else
                    {
                        Toast.MakeText(this, "ไม่สามารถบันทึกข้อมูลได้", ToastLength.Short).Show();
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
    }
}