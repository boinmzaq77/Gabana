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
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class FilterActivity : Activity
    {
        ImageView imgSelectTime, imgSelectDes, imgSelectAsc, imgSelectAz, imgSelectZa;
        Button btnSave;
        public static int selectFilter;
        Context context = Android.App.Application.Context;
        FilterActivity filterActivity;
        protected async override void OnCreate(Bundle savedInstanceState)
        {

            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.filter_activity_main);
                filterActivity = this;
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;

                imgSelectTime = FindViewById<ImageView>(Resource.Id.imgSelectTime);
                imgSelectDes = FindViewById<ImageView>(Resource.Id.imgSelectDes);
                imgSelectAsc = FindViewById<ImageView>(Resource.Id.imgSelectAsc);
                imgSelectAz = FindViewById<ImageView>(Resource.Id.imgSelectAz);
                imgSelectZa = FindViewById<ImageView>(Resource.Id.imgSelectZa);

                CheckJwt();

                Button btnTime = FindViewById<Button>(Resource.Id.btnTime);
                Button btnDes = FindViewById<Button>(Resource.Id.btnDes);
                Button btnAsc = FindViewById<Button>(Resource.Id.btnAsc);
                Button btnAz = FindViewById<Button>(Resource.Id.btnAz);
                Button btnZa = FindViewById<Button>(Resource.Id.btnZa);

                FrameLayout lnSortTime = FindViewById<FrameLayout>(Resource.Id.lnSortTime);
                FrameLayout lnSortAZ = FindViewById<FrameLayout>(Resource.Id.lnSortAZ);
                FrameLayout lnSortZA = FindViewById<FrameLayout>(Resource.Id.lnSortZA);
                if (ShowReportDailySaleActivity.TypeReport.Contains("SalesReport") || ShowReportDailySaleActivity.TypeReport.Contains("ProfitReport"))
                {
                    lnSortAZ.Visibility = ViewStates.Gone;
                    lnSortZA.Visibility = ViewStates.Gone;
                    lnSortTime.Visibility = ViewStates.Visible;
                }
                else
                {
                    lnSortAZ.Visibility = ViewStates.Visible;
                    lnSortZA.Visibility = ViewStates.Visible;
                    lnSortTime.Visibility = ViewStates.Gone;
                }
                btnSave = FindViewById<Button>(Resource.Id.btnSave);
                btnTime.Click += (sender, e) =>
                {
                    selectFilter = 0;
                    SetFilterButton(selectFilter);
                };
                btnDes.Click += (sender, e) =>
                {
                    selectFilter = 1;
                    SetFilterButton(selectFilter);
                };
                btnAsc.Click += (sender, e) =>
                {
                    selectFilter = 2;
                    SetFilterButton(selectFilter);
                };
                btnAz.Click += (sender, e) =>
                {
                    selectFilter = 3;
                    SetFilterButton(selectFilter);
                };
                btnZa.Click += (sender, e) =>
                {
                    selectFilter = 4;
                    SetFilterButton(selectFilter);
                };

                btnSave.Click += BtnSave_Click;
                SetFilterButton(selectFilter);

                _ = TinyInsights.TrackPageViewAsync("OnCreate : FilterActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Language");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }

        }

        private void SetBtnSave()
        {
            if (selectFilter != ShowReportDailySaleActivity.filterReport)
            {
                btnSave.Enabled = true;
                btnSave.SetBackgroundResource(Resource.Drawable.btnblue);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnSave.Enabled = true;
                btnSave.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            ShowReportDailySaleActivity.filterReport = selectFilter;
            this.Finish();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }
        private void SetFilterButton(int filter)
        {
            imgSelectTime.Visibility = ViewStates.Invisible;
            imgSelectDes.Visibility = ViewStates.Invisible;
            imgSelectAsc.Visibility = ViewStates.Invisible;
            imgSelectAz.Visibility = ViewStates.Invisible;
            imgSelectZa.Visibility = ViewStates.Invisible;

            switch (filter)
            {
                case 0:
                    imgSelectTime.Visibility = ViewStates.Visible;
                    break;
                case 1:
                    imgSelectDes.Visibility = ViewStates.Visible;
                    break;
                case 2:
                    imgSelectAsc.Visibility = ViewStates.Visible;
                    break;
                case 3:
                    imgSelectAz.Visibility = ViewStates.Visible;
                    break;
                case 4:
                    imgSelectZa.Visibility = ViewStates.Visible;
                    break;
                default:
                    break;
            }

            SetBtnSave();
        }

        internal static void SetFilter(int filterReport)
        {
            selectFilter = filterReport;
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'FilterActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'FilterActivity.openPage' is assigned but its value is never used
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

