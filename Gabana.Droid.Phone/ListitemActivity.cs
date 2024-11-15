using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;

namespace Gabana.Droid
{
    [Activity(Theme = "@style/AppTheme", MainLauncher = false, LaunchMode = Android.Content.PM.LaunchMode.SingleTop, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ListitemActivity : AppCompatActivity
    {
        RecyclerView mRecycleView;
        Button btnRetive, btnAdd1, btnUpdate1 ;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.listitem_activity_main);
            mRecycleView = FindViewById<RecyclerView>(Resource.Id.recyclerViewListitem);
            btnRetive = FindViewById<Button>(Resource.Id.btnRetive);
            btnAdd1 = FindViewById<Button>(Resource.Id.btnAdd1);
            btnUpdate1 = FindViewById<Button>(Resource.Id.btnUpdate1);
            btnUpdate1.Click += BtnUpdate1_Click;
        }

        private void BtnUpdate1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Gabana.Model.ResultAPI resultAPI = Utils.CheckNullValue();
            if (resultAPI.Status)
            {
                if (resultAPI.Message == "login")
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                }
                else
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(SplashActivity)));
                }
                this.Finish(); return;
            }
        }
    }
}