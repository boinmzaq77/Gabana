using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Droid.ListData;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid
{
        [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait ,Exported = false)]
    public class DiscountActivity : Activity
    {
        List<DiscountTemplate> lstdiscountTemplates;
        ListDiscount listDiscount;
        RecyclerView mRecycleViewDiscount;
        Item_Adapter_Discount Item_Adapter_Discount;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.discount_activity);
                // Create your application here
                mRecycleViewDiscount = FindViewById<RecyclerView>(Resource.Id.recyclerview_listDiscount);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
                Button addDiscount = FindViewById<Button>(Resource.Id.addDiscount);
                lnBack.Click += LnBack_Click;
                btnBack.Click += LnBack_Click;
                addDiscount.Click += AddDiscount_Click;

                SetDiscountData();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Dicsount");
                return;
            }
        }

        private void AddDiscount_Click(object sender, EventArgs e)
        {
            DataCashing.EditDiscount = "";
            StartActivity(new Intent(Application.Context, typeof(AddDiscountActivity)));
        }

        protected override void OnResume()
        {
            base.OnResume();
            SetDiscountData();
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        private async void SetDiscountData()
        {
            try
            {
                DiscountTemplateManage discountTemplateManage = new DiscountTemplateManage();
                lstdiscountTemplates = await discountTemplateManage.GetAllDiscountTemplate();
                listDiscount = new ListDiscount(lstdiscountTemplates);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
                mRecycleViewDiscount.HasFixedSize = true;
                mRecycleViewDiscount.SetLayoutManager(mLayoutManager);
                Item_Adapter_Discount = new Item_Adapter_Discount(listDiscount);
                mRecycleViewDiscount.SetAdapter(Item_Adapter_Discount);
                Item_Adapter_Discount.ItemClick += Item_Adapter_Discount_ItemClick;

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetDiscountData at Dicsount");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Item_Adapter_Discount_ItemClick(object sender, int e)
        {
            try
            {
                StartActivity(new Intent(Application.Context, typeof(AddDiscountActivity)));
                DataCashing.EditDiscount = "EditDiscount";
                var Discount = listDiscount[e];
                DataCashing.EditSysDiscountTemplate = Discount.SysDiscountTemplate;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
    }
}