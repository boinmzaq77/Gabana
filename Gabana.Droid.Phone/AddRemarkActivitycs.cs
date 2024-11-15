using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
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
    public class AddRemarkActivitycs : AppCompatActivity
    {
        AddRemarkActivitycs addremark;

        ImageButton btnBack;
        LinearLayout lnNoCustomer, lnHaveCustomer;
        TextView txtNameCustomer;
        Customer selectCus;
        public static TranWithDetailsLocal tranWithDetails;
        EditText txtRemark;
        Button btnSave;
        ImageButton btnCustomer;
        public static string AddRemark;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.addremark_activity_main);

            addremark = this;

            btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
            btnCustomer = FindViewById<ImageButton>(Resource.Id.btnCustomer);
            lnNoCustomer = FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
            lnHaveCustomer = FindViewById<LinearLayout>(Resource.Id.lnHaveCustomer);
            txtNameCustomer = FindViewById<TextView>(Resource.Id.txtNameCustomer);
            txtRemark = FindViewById<EditText>(Resource.Id.txtRemark);
            btnSave = FindViewById<Button>(Resource.Id.btnSave);

            txtRemark.TextChanged += TxtRemark_TextChanged;
            btnCustomer.Click += BtnCustomer_Click;
            lnNoCustomer.Click += BtnCustomer_Click;
            lnHaveCustomer.Click += BtnCustomer_Click;
            btnBack.Click += BtnBack_Click;
            btnSave.Click += BtnSave_Click;

            CheckJwt();
            SetCustomer();

            if (!string.IsNullOrEmpty(tranWithDetails.tran.Comments))
            {
                txtRemark.Text = tranWithDetails.tran.Comments;
                txtRemark.SetSelection(txtRemark.Text.Length);
            }
            _ = TinyInsights.TrackPageViewAsync("OnCreate : AddRemarkActivitycs");

        }

        private void TxtRemark_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            AddRemark = txtRemark.Text;
            btnSave.SetBackgroundResource(Resource.Drawable.btnblue);
            btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
        }

        private void BtnCustomer_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(SelectCustomerActivity)));
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (tranWithDetails == null)
            {
                return;
            }
            DataCashing.ModifyTranOrder = true;
            tranWithDetails.tran.Comments = AddRemark;
            CartActivity.SetTranDetail(tranWithDetails);
            AddRemark = string.Empty;
            this.Finish();
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        public async void SetCustomer()
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
                        DataCashing.ModifyTranOrder = true;
                        tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    }
                    txtNameCustomer.Text = tranWithDetails.tran.CustomerName?.ToString();
                }


            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected async override void OnResume()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                base.OnResume();
                CheckJwt();
                SetCustomer();
            }
            catch (Exception)
            {
                base.OnRestart();
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