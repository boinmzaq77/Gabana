using Android.App;
using Android.Content;
using Android.OS;
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
    public class LanguageActivity : Activity
    {
        ImageView imgCheckThai, imgCheckEng;
        Button btnSave;
        string selectLang;
        Context context = Android.App.Application.Context;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.language_activity);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;

                CheckJwt();

                int lang = 0;
                imgCheckThai = FindViewById<ImageView>(Resource.Id.imgCheck1);
                imgCheckEng = FindViewById<ImageView>(Resource.Id.imgCheck2);
                Button btnThai = FindViewById<Button>(Resource.Id.btnThai);
                Button btnEnglish = FindViewById<Button>(Resource.Id.btnEnglish);
                btnSave = FindViewById<Button>(Resource.Id.btnSave);

                //string local = Resources.Configuration.Locale.Language.ToString();
                string local = Preferences.Get("Language", "");
                btnThai.Enabled = false;

                if (local == "en" || local == "es")
                {
                    lang = 0;
                    selectLang = "en";
                    btnThai.Enabled = true;
                    Setcheck(lang);
                }
                else
                {
                    lang = 1;
                    selectLang = "th";
                    btnEnglish.Enabled = true;
                    Setcheck(lang);
                }

                btnThai.Click += (sender, e) =>
                {
                    lang = 1;
                    Setcheck(lang);
                    selectLang = "th";
                    btnThai.Enabled = false;
                    btnEnglish.Enabled = true;
                    SetBtnSave();
                };
                btnEnglish.Click += (sender, e) =>
                {
                    lang = 0;
                    Setcheck(lang);
                    selectLang = "en";
                    btnEnglish.Enabled = false;
                    btnThai.Enabled = true;
                    SetBtnSave();
                };

                btnSave.Click += BtnSave_Click; ;
                SetBtnSave();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : LanguageActivity");

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
            if (Preferences.Get("Language", "") != selectLang)
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
            Android.Content.Res.Configuration configuration = Resources.Configuration;
            configuration.SetLayoutDirection(new Java.Util.Locale(selectLang));
#pragma warning disable CS0618 // Type or member is obsolete
            Resources.UpdateConfiguration(configuration, Resources.DisplayMetrics);
#pragma warning restore CS0618 // Type or member is obsolete
            Preferences.Set("Language", selectLang);
            DataCashing.Language = selectLang;

            // Relaunch MainActivity
            Intent intent = new Intent(context, new SplashActivity().Class);
            intent.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);

        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }
        private void Setcheck(int lang)
        {
            if (lang == 1)
            {
                imgCheckThai.Visibility = ViewStates.Visible;
                imgCheckEng.Visibility = ViewStates.Invisible;
            }
            else
            {
                imgCheckThai.Visibility = ViewStates.Invisible;
                imgCheckEng.Visibility = ViewStates.Visible;
            }
        }

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
    }
}

