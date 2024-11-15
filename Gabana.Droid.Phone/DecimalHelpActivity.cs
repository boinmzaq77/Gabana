using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class DecimalHelpActivity : Activity
    {
        string selectType, multiply;
        ImageView imgType0, imgType1, imgType2, imgType3, imgType4, imgType5;
        EditText textType4;
        LinearLayout lnType4;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.decimalhelp_activity);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;


                imgType0 = FindViewById<ImageView>(Resource.Id.imgType0);
                imgType1 = FindViewById<ImageView>(Resource.Id.imgType1);
                imgType2 = FindViewById<ImageView>(Resource.Id.imgType2);
                imgType3 = FindViewById<ImageView>(Resource.Id.imgType3);
                imgType4 = FindViewById<ImageView>(Resource.Id.imgType4);
                imgType5 = FindViewById<ImageView>(Resource.Id.imgType5);

                textType4 = FindViewById<EditText>(Resource.Id.textType4);

                TextView textHeader1 = FindViewById<TextView>(Resource.Id.textHeader1);
                TextView textHeader2 = FindViewById<TextView>(Resource.Id.textHeader2);
                TextView textHeader3 = FindViewById<TextView>(Resource.Id.textHeader3);
                TextView textHeader4 = FindViewById<TextView>(Resource.Id.textHeader4);
                TextView textHeader5 = FindViewById<TextView>(Resource.Id.textHeader5);
                TextView textHeader6 = FindViewById<TextView>(Resource.Id.textHeader6);

                TextView textView1 = FindViewById<TextView>(Resource.Id.textView1);
                TextView textView2 = FindViewById<TextView>(Resource.Id.textView2);
                TextView textView3 = FindViewById<TextView>(Resource.Id.textView3);
                TextView textView4 = FindViewById<TextView>(Resource.Id.textView4);
                TextView textView5 = FindViewById<TextView>(Resource.Id.textView5);
                TextView textView6 = FindViewById<TextView>(Resource.Id.textView6);
                TextView textView7 = FindViewById<TextView>(Resource.Id.textView7);
                TextView textView8 = FindViewById<TextView>(Resource.Id.textView8);
                TextView textView9 = FindViewById<TextView>(Resource.Id.textView9);
                TextView textView10 = FindViewById<TextView>(Resource.Id.textView10);
                TextView textView11 = FindViewById<TextView>(Resource.Id.textView11);
                TextView textView12 = FindViewById<TextView>(Resource.Id.textView12);
                TextView textView13 = FindViewById<TextView>(Resource.Id.textView13);
                TextView textView14 = FindViewById<TextView>(Resource.Id.textView14);
                TextView textView15 = FindViewById<TextView>(Resource.Id.textView15);
                TextView textView16 = FindViewById<TextView>(Resource.Id.textView16);
                TextView textView17 = FindViewById<TextView>(Resource.Id.textView17);
                TextView textView18 = FindViewById<TextView>(Resource.Id.textView18);
                TextView textView19 = FindViewById<TextView>(Resource.Id.textView19);
                TextView textView20 = FindViewById<TextView>(Resource.Id.textView20);
                TextView textView21 = FindViewById<TextView>(Resource.Id.textView21);
                if (DataCashing.Language == "th")
                {
                    textHeader1.Text = "ไม่ปัดเศษ";
                    textHeader2.Text = "การปัดเศษแบบที่ 1";
                    textHeader3.Text = "การปัดเศษแบบที่ 2";
                    textHeader4.Text = "การปัดเศษแบบที่ 3";
                    textHeader5.Text = "การปัดเศษแบบที่ 4";
                    textHeader6.Text = "การปัดเศษแบบที่ 5";

                    textView1.Text = "เช่น 3.15 ไม่ปัดเศษเป็น 3.25";

                    textView2.Text = "0.00 < X <= 0.25 = 0.25";
                    textView3.Text = "0.25 < X <= 0.50 = 0.50";
                    textView4.Text = "0.50 < X <= 0.75 = 0.75";
                    textView5.Text = "X > 0.75 = 1";

                    textView6.Text = "เช่น 3.15 ปัดเศษเป็น 3.25";
                    textView7.Text = "เช่น 3.35 ปัดเศษเป็น 3.50";
                    textView8.Text = "เช่น 3.55 ปัดเศษเป็น 3.75";
                    textView9.Text = "เช่น 3.85 ปัดเศษเป็น 4.00";

                    textView10.Text = "0 < X < 1 = 1";
                    textView11.Text = "เช่น 3.25 ปัดเศษเป็น 4.00";

                    textView12.Text = "0 < X < 0.5 = 0";
                    textView13.Text = "X >= 0.5 = 1";
                    textView14.Text = "เช่น 3.35 ปัดเศษเป็น 3.00";
                    textView15.Text = "เช่น 3.55 ปัดเศษเป็น 4.00";

                    textView16.Text = "0 < X < 0.5 = 1";
                    textView17.Text = "0.5 < X < 1 = 1";
                    textView18.Text = "เช่น ตัวคูณ 10, 4.00 ปัดเศษเป็น 0.00";
                    textView19.Text = "เช่น ตัวคูณ 10, 5.00 ปัดเศษเป็น 10.00";

                    textView20.Text = "X < 1.9 = 1";
                    textView21.Text = "เช่น 3.90 ปัดเศษเป็น 3.00";

                }
                else
                {
                    textHeader1.Text = "Not rounded";
                    textHeader2.Text = "Option 1";
                    textHeader3.Text = "Option 2";
                    textHeader4.Text = "Option 3";
                    textHeader5.Text = "Option 4";
                    textHeader6.Text = "Option 5";

                    textView1.Text = "For example 3.25 No rounded to 3.25";

                    textView2.Text = "0.00 < X <= 0.25 = 0.25";
                    textView3.Text = "0.25 < X <= 0.50 = 0.50";
                    textView4.Text = "0.50 < X <= 0.75 = 0.75";
                    textView5.Text = "X > 0.75 = 1";

                    textView6.Text = "For example 3.15 Round up to 3.25";
                    textView7.Text = "For example 3.35 Round up to 3.50";
                    textView8.Text = "For example 3.55 Round up to 3.75";
                    textView9.Text = "For example 3.85 Round up to 4.00";

                    textView10.Text = "0 < X < 1 = 1";
                    textView11.Text = "For example 3.25 Round up to 4.00";

                    textView12.Text = "0 < X < 0.5 = 0";
                    textView13.Text = "X >= 0.5 = 1";
                    textView14.Text = "For example 3.35  Round down to 3.00";
                    textView15.Text = "For example 3.55  Round up to 4.00";

                    textView16.Text = "0 < X < 0.5 = 1";
                    textView17.Text = "0.5 < X < 1 = 1";
                    textView18.Text = "For example Multiply 10, 4.00  Round down to 0.00";
                    textView19.Text = "For example Multiply 10, 5.00  Round up to 10.00";

                    textView20.Text = "X < 1.9 = 1";
                    textView21.Text = "For example 3.90 Round down to 3.00";

                }


                LinearLayout lnType0 = FindViewById<LinearLayout>(Resource.Id.lnType0);
                LinearLayout lnType1 = FindViewById<LinearLayout>(Resource.Id.lnType1);
                LinearLayout lnType2 = FindViewById<LinearLayout>(Resource.Id.lnType2);
                LinearLayout lnType3 = FindViewById<LinearLayout>(Resource.Id.lnType3);
                lnType4 = FindViewById<LinearLayout>(Resource.Id.lnType4);
                LinearLayout lnType5 = FindViewById<LinearLayout>(Resource.Id.lnType5);

                CheckJwt();

                lnType0.Click += LnType0_Click;
                lnType1.Click += LnType1_Click;
                lnType2.Click += LnType2_Click;
                lnType3.Click += LnType3_Click;
                lnType4.Click += LnType4_Click;
                lnType5.Click += LnType5_Click;
                selectType = DataCashingAll.setmerchantConfig.OPTION_ROUNDING_STRING;
                multiply = DataCashingAll.setmerchantConfig.OPTION_ROUNDING_INT;
                if (string.IsNullOrEmpty(multiply))
                {
                    textType4.Visibility = ViewStates.Invisible;
                }
                else
                {
                    textType4.Text = multiply;
                    Android.Views.InputMethods.InputMethodManager imm = (Android.Views.InputMethods.InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(textType4.WindowToken, Android.Views.InputMethods.HideSoftInputFlags.None);
                }
                SetShowSelect(selectType);

                Button btnApply = FindViewById<Button>(Resource.Id.btnApply);
                btnApply.Click += BtnApply_Click;

                _ = TinyInsights.TrackPageViewAsync("OnCreate : DecimalHelpActivity");

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Decimal");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }

        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            string text = "";
            if (selectType == "4")
            {
                text = textType4.Text;
            }
            DecimalActivity.SetDecimalType(selectType, text);
            this.Finish();
        }

        private void LnType5_Click(object sender, EventArgs e)
        {
            //DecimalActivity.SetDecimalType("6");
            //Finish();
            SetShowSelect("5");
        }

        private void LnType4_Click(object sender, EventArgs e)
        {
            //DecimalActivity.SetDecimalType("5");
            //Finish();         
            lnType4.Click += (sender, e) =>
            {
                textType4.PerformClick();
                textType4.RequestFocus();
                textType4.SetSelection(textType4.Text.Length);
                textType4.FocusableInTouchMode = true;
                textType4.Clickable = true;
                Openkeyboard();
            };
            SetShowSelect("4");

        }

        private void LnType3_Click(object sender, EventArgs e)
        {
            //DecimalActivity.SetDecimalType("4");
            //Finish();
            SetShowSelect("3");

        }

        private void LnType2_Click(object sender, EventArgs e)
        {
            //DecimalActivity.SetDecimalType("3");
            //Finish();
            SetShowSelect("2");

        }

        private void LnType1_Click(object sender, EventArgs e)
        {
            //DecimalActivity.SetDecimalType("2");
            //Finish();
            SetShowSelect("1");

        }

        private void LnType0_Click(object sender, EventArgs e)
        {
            //DecimalActivity.SetDecimalType("1");

            SetShowSelect("0");

        }

        private void SetShowSelect(string type)
        {
            imgType0.Visibility = ViewStates.Gone;
            imgType1.Visibility = ViewStates.Gone;
            imgType2.Visibility = ViewStates.Gone;
            imgType3.Visibility = ViewStates.Gone;
            imgType4.Visibility = ViewStates.Gone;
            imgType5.Visibility = ViewStates.Gone;
            textType4.Visibility = ViewStates.Invisible;
            switch (type)
            {
                case "0":
                    imgType0.Visibility = ViewStates.Visible;
                    break;
                case "1":
                    imgType1.Visibility = ViewStates.Visible;
                    break;
                case "2":
                    imgType2.Visibility = ViewStates.Visible;
                    break;
                case "3":
                    imgType3.Visibility = ViewStates.Visible;
                    break;
                case "4":
                    imgType4.Visibility = ViewStates.Visible;
                    textType4.Visibility = ViewStates.Visible;
                    break;
                case "5":
                    imgType5.Visibility = ViewStates.Visible;
                    break;
            }

            selectType = type;
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        void Openkeyboard()
        {
            Android.Views.InputMethods.InputMethodManager imm = (Android.Views.InputMethods.InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.ShowSoftInput(textType4, Android.Views.InputMethods.ShowFlags.Implicit);
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'DecimalHelpActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'DecimalHelpActivity.openPage' is assigned but its value is never used
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
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                PinCodeActitvity.SetPincode("Pincode");
                openPage = true;
            }

            
        }

    }
}

