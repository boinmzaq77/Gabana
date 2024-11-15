using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class LanguageSettingActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.languagesetting_activity);
            CombinerUI();
        }

        ImageView imgCheckThai, imgCheckEng;
        string selectLang;
        Context context = Android.App.Application.Context;
        Button btnSave;
        private void CombinerUI()
        {
            LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Click += LnBack_Click;

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

#pragma warning disable CS0612 // 'LanguageSettingActivity.BtnSave_Click(object, EventArgs)' is obsolete
            btnSave.Click += BtnSave_Click; ;
#pragma warning restore CS0612 // 'LanguageSettingActivity.BtnSave_Click(object, EventArgs)' is obsolete
        }

        private void SetBtnSave()
        {
            if (Preferences.Get("Language", "") != selectLang)
            {
                btnSave.Enabled = true;
                btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnSave.Enabled = true;
                btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));

            }
        }

        [Obsolete]
        private void BtnSave_Click(object sender, EventArgs e)
        {
            Android.Content.Res.Configuration configuration = Resources.Configuration;
            configuration.SetLayoutDirection(new Java.Util.Locale(selectLang));
            Resources.UpdateConfiguration(configuration, Resources.DisplayMetrics);
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
    }
}