using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class ChangeActivity : Activity
    {
#pragma warning disable CS0649 // Field 'ChangeActivity.Change' is never assigned to, and will always have its default value 0
#pragma warning disable CS0649 // Field 'ChangeActivity.Cash' is never assigned to, and will always have its default value 0
        decimal Change, Cash;
#pragma warning restore CS0649 // Field 'ChangeActivity.Cash' is never assigned to, and will always have its default value 0
#pragma warning restore CS0649 // Field 'ChangeActivity.Change' is never assigned to, and will always have its default value 0
        public static TranWithDetailsLocal tranWithDetails;
        public static ChangeActivity changeActivity;
        ImageButton btnBack;
        static LinearLayout lnNoCustomer, lnHaveCustomer;
        static ImageButton btnCustomer;
        static TextView txtNameCustomer;
        private static Customer selectCus;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                changeActivity = this;
                SetContentView(Resource.Layout.change_activity_main);
                TextView textreceive = FindViewById<TextView>(Resource.Id.textreceive);
                TextView textChange = FindViewById<TextView>(Resource.Id.textChange);
                lnNoCustomer = FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
                lnHaveCustomer = FindViewById<LinearLayout>(Resource.Id.lnHaveCustomer);
                btnCustomer = FindViewById<ImageButton>(Resource.Id.btnCustomer);
                txtNameCustomer = FindViewById<TextView>(Resource.Id.txtNameCustomer);


                textreceive.Text = Utils.DisplayDecimal(Cash);
                textChange.Text = Utils.DisplayDecimal(Change);
                textChange.Text = Utils.DisplayDecimal(tranWithDetails.tran.Change);
                Button btnViewREceipt = FindViewById<Button>(Resource.Id.btnViewREceipt);
                btnViewREceipt.Click += BtnViewREceipt_Click;
                btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                btnBack.Click += BtnBack_Click;
                lnBack.Click += BtnBack_Click;
                //btnCustomer.Click += BtnCustomer_Click;

                SetCustomer();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : ChangeActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("change");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public static async void SetCustomer()
        {
            try
            {
                if (DataCashing.SysCustomerID == 999)
                {
                    if (tranWithDetails.tran.SysCustomerID != 999)
                    {
                        tranWithDetails = await BLTrans.RemovePerson(tranWithDetails);
                    }

                    lnHaveCustomer.Visibility = ViewStates.Gone;
                    lnNoCustomer.Visibility = ViewStates.Visible;

                }
                else
                {
                    lnHaveCustomer.Visibility = ViewStates.Visible;
                    lnNoCustomer.Visibility = ViewStates.Gone;

                    CustomerManage customerManage = new CustomerManage();
                    var listCustomer = new List<Customer>();
                    listCustomer = await customerManage.GetAllCustomer();
                    selectCus = listCustomer.Where(x => x.SysCustomerID == DataCashing.SysCustomerID).FirstOrDefault();
                    if (selectCus == null) return;
                    if (tranWithDetails.tran.SysCustomerID != DataCashing.SysCustomerID)
                    {
                        tranWithDetails.tran.SysCustomerID = selectCus.SysCustomerID;
                        tranWithDetails.tran.CustomerName = selectCus.CustomerName;

                        tranWithDetails = await BLTrans.ChoosePerson(tranWithDetails, selectCus);
                        tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    }
                    txtNameCustomer.Text = tranWithDetails.tran.CustomerName?.ToString();
                }


            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        private async void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                //StartActivity(new Intent(Application.Context, typeof(PosActivity)));
                tranWithDetails = new TranWithDetailsLocal { tran = new Tran { TaxRate = 0 }, tranDetailItemWithToppings = new List<TranDetailItemWithTopping>(), tranPayments = new List<TranPayment>(), tranTradDiscounts = new List<TranTradDiscount>() };
                PosActivity.SetTranDetail(tranWithDetails);
                this.Finish();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnBack_Click at change");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
                    return;
                }
                SetCustomer();
            }
            catch (Exception)
            {
                base.OnRestart();
            }
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
            btnBack.PerformClick();
        }

        private void BtnViewREceipt_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Application.Context, typeof(ReceiptActivity)));
                ReceiptActivity.SetTranDeail(tranWithDetails);
                this.Finish();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnViewREceipt_Click at Branch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public static void SetTranDeail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }
    }
}