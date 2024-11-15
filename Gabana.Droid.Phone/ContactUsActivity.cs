using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ContactUsActivity : Activity
    {
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.contactus_activity);
                CheckJwt();
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                LinearLayout lnTel = FindViewById<LinearLayout>(Resource.Id.lnTel);
                lnTel.Click += (sender, e) =>
                {
                    var uri = Android.Net.Uri.Parse("tel:026925899");
                    var intent = new Intent(Intent.ActionView, uri);
                    StartActivity(intent);
                };
                LinearLayout lnEmail = FindViewById<LinearLayout>(Resource.Id.lnEmail);
                lnEmail.Click += (sender, e) =>
                {
                    var uri = Android.Net.Uri.Parse("https://mail.google.com/mail/u/0/?view=cm&fs=1&tf=1&source=mailto&to=Info@SeniorSoft.co.th");
                    var intent = new Intent(Intent.ActionView, uri);
                    StartActivity(intent);
                };
                LinearLayout lnLine = FindViewById<LinearLayout>(Resource.Id.lnLine);
                lnLine.Click += (sender, e) =>
                {
                    var uri = Android.Net.Uri.Parse("https://line.me/R/ti/p/%40698ipxjn");
                    var intent = new Intent(Intent.ActionView, uri);
                    StartActivity(intent);
                };
                LinearLayout lnFacebook = FindViewById<LinearLayout>(Resource.Id.lnFacebook);
                lnFacebook.Click += (sender, e) =>
                {
                    var uri = Android.Net.Uri.Parse("https://th-th.facebook.com/seniorsoft");
                    var intent = new Intent(Intent.ActionView, uri);
                    StartActivity(intent);
                };
                LinearLayout lnWebsite = FindViewById<LinearLayout>(Resource.Id.lnWebsite);
                lnWebsite.Click += (sender, e) =>
                {
                    var uri = Android.Net.Uri.Parse("https://www.seniorsoft.co.th/th/");
                    var intent = new Intent(Intent.ActionView, uri);
                    StartActivity(intent);
                };
                LinearLayout lnAddress = FindViewById<LinearLayout>(Resource.Id.lnAddress);
                lnAddress.Click += async (sender, e) =>
                {
                    try
                    {
                        //var uri = Android.Net.Uri.Parse("geo:" + imemberCard.Lat?.Trim() + imemberCard.Long?.Trim());
                        var uri = Android.Net.Uri.Parse("http://maps.google.com/maps?q=loc:" + 13.7743597 + "," + 100.632165); 
                        var intent = new Intent(Intent.ActionView, uri);
                        StartActivity(intent);

                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, "Error  : " + ex.Message, ToastLength.Short).Show();
                        await TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("contactus");
                        return;
                    }
                };
                _ = TinyInsights.TrackPageViewAsync("OnCreate : ContactUsActivity");


            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("contactus");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }

        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
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
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                PinCodeActitvity.SetPincode("Pincode");
                openPage = true;
            }

            
        }

    }
}

