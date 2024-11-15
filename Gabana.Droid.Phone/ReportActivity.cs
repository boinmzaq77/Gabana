using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Microcharts.Droid;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class ReportActivity : AppCompatActivity
    {
        SwipeRefreshLayout refreshlayout;
        ChartView chartView1, chartView2;
        internal static string branchID;
        TextView txtBranchName;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.report_activity_main);


                CheckJwt();
                LinearLayout lnChooseBranch = FindViewById<LinearLayout>(Resource.Id.lnChooseBranch);
                lnChooseBranch.Click += LnChooseBranch_Click;

                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;

                txtBranchName = FindViewById<TextView>(Resource.Id.txtBranchName);
                branchID = Preferences.Get("Branch", "");

                chartView1 = FindViewById<ChartView>(Resource.Id.chartView1);
                chartView2 = FindViewById<ChartView>(Resource.Id.chartView2);


                LinearLayout btnSalesReport = FindViewById<LinearLayout>(Resource.Id.btnSalesReport);
                btnSalesReport.Click += BtnSalesReport_Click;
                LinearLayout btnSalesReportByBranch = FindViewById<LinearLayout>(Resource.Id.btnSalesReportByBranch);
                btnSalesReportByBranch.Click += BtnSalesReportByBranch_Click; ;
                LinearLayout btnProfitReport = FindViewById<LinearLayout>(Resource.Id.btnProfitReport);
                btnProfitReport.Click += BtnProfitReport_Click;
                LinearLayout btnCategoryReport = FindViewById<LinearLayout>(Resource.Id.btnCategoryReport);
                btnCategoryReport.Click += BtnCategoryReport_Click;
                LinearLayout btnDailyCustomer = FindViewById<LinearLayout>(Resource.Id.btnDailyCustomer);
                btnDailyCustomer.Click += BtnDailyCustomer_Click;
                LinearLayout btnDailyEmployee = FindViewById<LinearLayout>(Resource.Id.btnDailyEmployee);
                btnDailyEmployee.Click += BtnDailyEmployee_Click;
                LinearLayout btnDailyPayment = FindViewById<LinearLayout>(Resource.Id.btnDailyPayment);
                btnDailyPayment.Click += BtnDailyPayment_Click;
                LinearLayout btnBestSellItem = FindViewById<LinearLayout>(Resource.Id.btnBestSellItem);
                btnBestSellItem.Click += BtnBestSellItem_Click;
                LinearLayout btnItemBalance = FindViewById<LinearLayout>(Resource.Id.btnItemBalance);
                btnItemBalance.Click += BtnItemBalance_Click;
                GetNameBranch();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : ReportActivity");


            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Report");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void BtnSalesReportByBranch_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportDailySaleActivity)));
            ReportDailySaleActivity.SetTypeReport("SalesReportByBranch");
        }

        private void BtnItemBalance_Click(object sender, EventArgs e)
        {
            //StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportBalanceActivity)));
        }

        private void BtnBestSellItem_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportDailySaleActivity)));
            ReportDailySaleActivity.SetTypeReport("ReportBestSale");
        }

        private void BtnDailyPayment_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportDailySaleActivity)));
            ReportDailySaleActivity.SetTypeReport("PaymentReport");
        }

        private async void BtnDailyEmployee_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportDailySaleActivity)));
            ReportDailySaleActivity.SetTypeReport("EmployeeReport");
            DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
            DataCashingAll.BranchPolicy = await GabanaAPI.GetDataBranchPolicy();
        }

        private void BtnDailyCustomer_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportDailySaleActivity)));
            ReportDailySaleActivity.SetTypeReport("CustomerReport");
        }

        private void BtnCategoryReport_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportDailySaleActivity)));
            ReportDailySaleActivity.SetTypeReport("CategoryReport");
        }

        private void BtnProfitReport_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportDailySaleActivity)));
            ReportDailySaleActivity.SetTypeReport("ProfitReport");
        }

        private void BtnSalesReport_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportDailySaleActivity)));
            ReportDailySaleActivity.SetTypeReport("SalesReport");
        }

        private void LnChooseBranch_Click(object sender, EventArgs e)
        {
            //StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportBranchActivity)));
            //ReportBranchActivity.SetBranch(branchID);
        }


        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        protected  override void OnResume()
        {
            base.OnResume();
            CheckJwt();
            GetNameBranch();
        }

        private async void GetNameBranch()
        {
            BranchManage branchManage = new BranchManage();
            var lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
            var branchName = lstBranch.Where(x => x.BranchID == branchID).FirstOrDefault();
            txtBranchName.Text = branchName.BranchName?.ToString();
        }
        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'ReportActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'ReportActivity.openPage' is assigned but its value is never used
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

