using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class ContactUsActivity : AppCompatActivity
    {
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                // Create your application here
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.contactus_activity);
                CombineUI();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("contactus");
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void CombineUI()
        {
            LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Click += LnBack_Click;
            LinearLayout lnTel = FindViewById<LinearLayout>(Resource.Id.lnTel);
            lnTel.Click += async (sender, e) =>
            {
                try
                {
                    var uri = Android.Net.Uri.Parse("tel:026925899");
                    var intent = new Intent(Intent.ActionView, uri);
                    StartActivity(intent);
                }
                catch (Exception ex)
                {
                    await TinyInsights.TrackErrorAsync(ex);
                }
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
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }
    }
}