using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.Report
{
    public class Report_Fragment_Main : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Report_Fragment_Main NewInstance()
        {
            Report_Fragment_Main frag = new Report_Fragment_Main();
            return frag;
        }
        internal static string branchID;
        View view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
             
            view = inflater.Inflate(Resource.Layout.report_fragment_main, container, false);
            try
            {
                branchID = Preferences.Get("Branch", "");

                ComBineUI();
                SetUIEvent();
                return view;
            }
            catch (Exception)
            {
                return view;
            }
        }

        private void SetUIEvent()
        {
            btnSalesReport.Click += BtnSalesReport_Click;
            btnSalesReportByBranch.Click += BtnSalesReportByBranch_Click;
            btnProfitReport.Click += BtnProfitReport_Click;
            btnCategoryReport.Click += BtnCategoryReport_Click;
            btnDailyCustomer.Click += BtnDailyCustomer_Click;
            btnDailyEmployee.Click += BtnDailyEmployee_Click;
            btnDailyPayment.Click += BtnDailyPayment_Click;
            btnBestSellItem.Click += BtnBestSellItem_Click;
        }

        private void BtnBestSellItem_Click(object sender, EventArgs e)
        {
            Report_Dialog_Custom.SetTypeReport("ReportBestSale");
            var fragment = new Report_Dialog_Custom();
            Report_Dialog_Custom dialog = new Report_Dialog_Custom();
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Custom));
        }
        private void BtnDailyPayment_Click(object sender, EventArgs e)
        {
            Report_Dialog_Custom.SetTypeReport("PaymentReport");
            var fragment = new Report_Dialog_Custom();
            Report_Dialog_Custom dialog = new Report_Dialog_Custom();
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Custom));
        }
        private async void BtnDailyEmployee_Click(object sender, EventArgs e)
        {
            DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
            DataCashingAll.BranchPolicy = await GabanaAPI.GetDataBranchPolicy();
            Report_Dialog_Custom.SetTypeReport("EmployeeReport");
            var fragment = new Report_Dialog_Custom();
            Report_Dialog_Custom dialog = new Report_Dialog_Custom();
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Custom));
        }
        private void BtnDailyCustomer_Click(object sender, EventArgs e)
        {
            Report_Dialog_Custom.SetTypeReport("CustomerReport");
            var fragment = new Report_Dialog_Custom();
            Report_Dialog_Custom dialog = new Report_Dialog_Custom();
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Custom));
        }
        private void BtnCategoryReport_Click(object sender, EventArgs e)
        {
            Report_Dialog_Custom.SetTypeReport("CategoryReport");
            var fragment = new Report_Dialog_Custom();
            Report_Dialog_Custom dialog = new Report_Dialog_Custom();
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Custom));
        }
        private void BtnProfitReport_Click(object sender, EventArgs e)
        {
            Report_Dialog_Custom.SetTypeReport("ProfitReport");
            var fragment = new Report_Dialog_Custom();
            Report_Dialog_Custom dialog = new Report_Dialog_Custom();
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Custom));
        }

        private void BtnSalesReportByBranch_Click(object sender, EventArgs e)
        {
            Report_Dialog_Custom.SetTypeReport("SalesReportByBranch");
            var fragment = new Report_Dialog_Custom();
            Report_Dialog_Custom dialog = new Report_Dialog_Custom();
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Custom));
        }
        private void BtnSalesReport_Click(object sender, EventArgs e)
        {
            Report_Dialog_Custom.SetTypeReport("SalesReport");
            var fragment = new Report_Dialog_Custom();
            Report_Dialog_Custom dialog = new Report_Dialog_Custom();
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Custom));
        }

        LinearLayout btnSalesReport, btnSalesReportByBranch, btnProfitReport, btnCategoryReport;
        LinearLayout btnDailyCustomer, btnDailyEmployee, btnDailyPayment, btnBestSellItem;
        private void ComBineUI()
        {

            btnSalesReport = view.FindViewById<LinearLayout>(Resource.Id.btnSalesReport);
            btnSalesReportByBranch = view.FindViewById<LinearLayout>(Resource.Id.btnSalesReportByBranch);
            btnProfitReport = view.FindViewById<LinearLayout>(Resource.Id.btnProfitReport);
            btnCategoryReport = view.FindViewById<LinearLayout>(Resource.Id.btnCategoryReport);
            btnDailyCustomer = view.FindViewById<LinearLayout>(Resource.Id.btnDailyCustomer);
            btnDailyEmployee = view.FindViewById<LinearLayout>(Resource.Id.btnDailyEmployee);
            btnDailyPayment = view.FindViewById<LinearLayout>(Resource.Id.btnDailyPayment);
            btnBestSellItem = view.FindViewById<LinearLayout>(Resource.Id.btnBestSellItem);

        }

    }
}