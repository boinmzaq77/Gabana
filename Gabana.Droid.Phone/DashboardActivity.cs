using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Dashboard;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class DashboardActivity : AppCompatActivity
    {
        SwipeRefreshLayout refreshlayout;
        ChartView chartView1, chartView2, chartView3;
        internal static string branchID;
        TextView txtBranchName, txtDate2, txtSaleTotal, txtSaleCount, txtProfit, txtAvgSale, txtComparison;
        RecyclerView recyclerview_listBestSele, recyclerViewPayment;
        LinearLayout lnHaveDashboard, lnNoDashboard;
        List<Gabana.Model.PaymentType> listPayments = new List<Gabana.Model.PaymentType>
            {
                new Model.PaymentType(){ Type ="CH" ,Detail = "Cash", Logo = Resource.Mipmap.RPaymentCash , color = "#0095DA"},
                new Model.PaymentType(){ Type ="Cr" ,Detail = "Credit Card", Logo = Resource.Mipmap.RPaymentCredit, color = "#F8971D"},
                new Model.PaymentType() { Type = "Dr", Detail = "Debit Card", Logo = Resource.Mipmap.RPaymentDebit, color = "#E32D49" },
                new Model.PaymentType() { Type = "GV", Detail = "Gift Voucher", Logo = Resource.Mipmap.RPaymentGiftvoucher, color = "#37AA52" },
                new Model.PaymentType() { Type = "MYQR", Detail = "myQR", Logo = Resource.Mipmap.RPaymentMyQR, color = "#F75600" },
                new Model.PaymentType() { Type = "QRCH", Detail = "QR Cash", Logo = Resource.Mipmap.RPaymentQrCash, color = "#3F51B5" },
                new Model.PaymentType() { Type = "QRCR", Detail = "QR Credit", Logo = Resource.Mipmap.RPaymentQrCredit, color = "#00796B" },
                new Model.PaymentType() { Type = "WECHAT", Detail = "WECHAT", Logo = Resource.Mipmap.RPaymentWechat, color = "#8BC34A" }
            };

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.dashboard_activity_main);

                LinearLayout lnChooseBranch = FindViewById<LinearLayout>(Resource.Id.lnChooseBranch);
                lnChooseBranch.Click += LnChooseBranch_Click;
                lnHaveDashboard = FindViewById<LinearLayout>(Resource.Id.lnHaveDashboard);
                lnNoDashboard = FindViewById<LinearLayout>(Resource.Id.lnNoDashboard);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;

                txtBranchName = FindViewById<TextView>(Resource.Id.txtBranchName);
                branchID = Preferences.Get("Branch", "");
                if (string.IsNullOrEmpty(branchID))
                {
                    branchID = "0";
                }

                txtDate2 = FindViewById<TextView>(Resource.Id.txtDate2);
                txtSaleTotal = FindViewById<TextView>(Resource.Id.txtSaleTotal);
                txtSaleCount = FindViewById<TextView>(Resource.Id.txtSaleCount);
                txtProfit = FindViewById<TextView>(Resource.Id.txtProfit);
                txtAvgSale = FindViewById<TextView>(Resource.Id.txtAvgSale);
                txtComparison = FindViewById<TextView>(Resource.Id.txtComparison);

                refreshlayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
                refreshlayout.Refresh += (sender, e) =>
                {
                    OnResume();
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();

                };

                chartView1 = FindViewById<ChartView>(Resource.Id.chartView1);
                chartView2 = FindViewById<ChartView>(Resource.Id.chartView2);

                recyclerview_listBestSele = FindViewById<RecyclerView>(Resource.Id.recyclerview_listBestSele);
                recyclerViewPayment = FindViewById<RecyclerView>(Resource.Id.recyclerViewChart2);

                //DrawChart();

                CheckJwt();
                GetDataDashboard();
                GetNameBranch();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : DashboardActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Dashboard");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }


        private void LnChooseBranch_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(DashboardBranchActivity)));
        }

        async void GetDataDashboard()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }
                txtDate2.Text = DateTime.Now.ToString("dd MMM yyyy", new CultureInfo("en-US"));

                var result = await GabanaAPI.GetDataDashboard(Convert.ToInt32(branchID));
                if (result == null)
                {
                    lnNoDashboard.Visibility = ViewStates.Visible;
                    lnHaveDashboard.Visibility = ViewStates.Gone;
                    if (dialogLoading != null)
                    {
                        dialogLoading.DismissAllowingStateLoss();
                        dialogLoading.Dismiss();
                    }
                    return;

                }

                lnNoDashboard.Visibility = ViewStates.Gone;
                lnHaveDashboard.Visibility = ViewStates.Visible;

                txtSaleTotal.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(result.salesTotal);
                txtSaleCount.Text = result.salesCount.ToString();
                txtProfit.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(result.profit);
                txtAvgSale.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(result.averageSale);
                txtComparison.Text = result.comparison.ToString("##0.00") + "%";


                List<Model.ReportHourly> reportHourlies = new List<Model.ReportHourly>();
                int h = 0;
                for (h = 0; h < 24; h++)
                {
                    Model.ReportHourly reportHourly = new Model.ReportHourly()
                    {
                        IdHourly = h + 1,
                        Hourlyname = h.ToString("D2") + ":00",
                        value = 0
                    };

                    reportHourlies.Add(reportHourly);
                }

                foreach (var item in reportHourlies)
                {
                    item.value = result.salesByPeriods.Where(x => x.hourly == item.IdHourly).Sum(x => x.salesPeriodTotal);
                }

                List<ChartEntry> SalesByPeriods = new List<ChartEntry>();

                var firstIndex = reportHourlies.FindIndex(x => x.value > 0) - 1;
                var lastIndex = reportHourlies.FindLastIndex(x => x.value > 0) + 2;

                if (firstIndex < 0) firstIndex = 0;
                if (lastIndex > reportHourlies.Count) lastIndex = reportHourlies.Count;

                var dif = Math.Abs(lastIndex - firstIndex);
                if (dif > 8)
                {
                    firstIndex = reportHourlies.FindIndex(x => x.value > 0);
                    lastIndex = reportHourlies.FindLastIndex(x => x.value > 0);
                    if (firstIndex % 3 != 0) firstIndex = firstIndex - (firstIndex % 3);
                    if (lastIndex % 3 != 0) lastIndex = lastIndex + (3 - (lastIndex % 3));

                    foreach (var item in reportHourlies)
                    {
                        ChartEntry entry = new ChartEntry((float)item.value)
                        {
                            Label = item.Hourlyname.ToString(),
                            ValueLabel = item.value.ToString("#,###"),
                            Color = SKColor.Parse("#0095DA")
                        };

                        ChartEntry entry2 = new ChartEntry((float)item.value)
                        {
                            Label = "",
                            TextColor = SKColor.Parse("#fff"),
                            ValueLabel = item.value.ToString("#,###"),
                            Color = SKColor.Parse("#0095DA")
                        };
                        if (item.IdHourly == firstIndex || item.IdHourly == lastIndex)
                        {
                            SalesByPeriods.Add(entry);
                        }
                        else if (item.IdHourly % 3 == 0 && item.IdHourly > firstIndex && item.IdHourly < lastIndex)
                        {
                            SalesByPeriods.Add(entry);
                        }
                        else if (item.IdHourly > firstIndex && item.IdHourly < lastIndex)
                        {
                            SalesByPeriods.Add(entry2);
                        }
                    }
                }
                else
                {
                    for (int a = firstIndex; a < lastIndex; a++)
                    {
                        var item = reportHourlies[a];
                        ChartEntry entry = new ChartEntry((float)item.value)
                        {
                            Label = item.Hourlyname.ToString(),
                            ValueLabel = item.value.ToString("#,###.##"),
                            Color = SKColor.Parse("#0095DA")
                        };
                        SalesByPeriods.Add(entry);
                    }

                }
                //foreach (var item in reportHourlies)
                //{
                //    Entry entry = new Entry((float)item.value)
                //    {
                //        Label = item.IdHourly.ToString(),
                //        ValueLabel = item.value.ToString("#,##0.00"),
                //        Color = SKColor.Parse("#0095DA")
                //    };

                //    SalesByPeriods.Add(entry);
                //}


                var chart = new LineChart()
                {
                    Entries = SalesByPeriods,
                    LabelTextSize = 16f,
                    BackgroundColor = SKColor.Parse("#FFF"),
                    LineMode = LineMode.Straight,
                    YAxisPosition = Position.Left,
                    ValueLabelOption = ValueLabelOption.None,
                    ShowYAxisLines = true,
                    ShowYAxisText = true,
                    LabelOrientation = Microcharts.Orientation.Horizontal,
                    ValueLabelOrientation = Microcharts.Orientation.Vertical,
                    IsAnimated = false
                };
                chartView1.Chart = chart;


                List<ChartEntry> SalesByPayments = new List<ChartEntry>();
                foreach (var item in result.salesByPayments)
                {
                    var index = listPayments.FindIndex(x => x.Type.ToUpper().Contains(item.paymentType.ToUpper()));
                    if (index != -1)
                    {
                        var colorType = listPayments.Where(x => x.Type.ToUpper().Contains(item.paymentType.ToUpper())).Select(x => x.color).FirstOrDefault();
                        var defaulf = SKColor.Parse("#0095DA");
                        SKColor.TryParse(colorType, out defaulf);
                        ChartEntry entry = new ChartEntry((float)item.percentTotal)
                        {
                            Label = Utils.SetPaymentNameChart(item.paymentType.ToString()),
                            ValueLabel = item.percentTotal.ToString("##0.00") + "%",
                            Color = defaulf
                        };
                        SalesByPayments.Add(entry);
                    }
                    else
                    {
                        result.salesByPayments.Remove(item);
                    }

                }
                var chart2 = new DonutChart()
                {
                    Entries = SalesByPayments,
                    LabelTextSize = 20f,
                    HoleRadius = 0.7f,
                    LabelMode = LabelMode.None,
                    GraphPosition = GraphPosition.Center
                };
                chartView2.Chart = chart2;
                Dashboard_Adapter_ShowPayment dashboard_Adapter_ShowPayment = new Dashboard_Adapter_ShowPayment(result.salesByPayments, SalesByPayments);
                LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this);
                recyclerViewPayment.SetAdapter(dashboard_Adapter_ShowPayment);
                recyclerViewPayment.SetLayoutManager(linearLayoutManager);
                recyclerViewPayment.SetItemViewCacheSize(20);
                recyclerViewPayment.HasFixedSize = true;

                List<Item> items = new List<Item>();
                ItemManage itemManage = new ItemManage();
                items = await itemManage.GetAllItemType();



                ListBestSallItem listBestSallIteem = new ListBestSallItem(result.bestSellingItems);
                Dashboard_Adapter_Item dashboard_adapter_item = new Dashboard_Adapter_Item(items, listBestSallIteem);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                recyclerview_listBestSele.SetLayoutManager(gridLayoutItem);
                recyclerview_listBestSele.HasFixedSize = true;
                recyclerview_listBestSele.SetAdapter(dashboard_adapter_item);


                List<ChartEntry> BestEmp = new List<ChartEntry>();

                decimal sumAllAmount = 0;
                foreach (var item in result.bestEmployees)
                {
                    sumAllAmount += item.totalAmount;
                }

                List<BestEmployee> emp = new List<BestEmployee>();
                for (int j = 0; j < 5; j++)
                {
                    if (result.bestEmployees.Count > j)
                    {
                        BestEmployee data = new BestEmployee()
                        {
                            employeeName = result.bestEmployees[j].employeeName?.ToString(),
                            totalAmount = result.bestEmployees[j].totalAmount
                        };
                        emp.Add(data);
                    }
                    else
                    {
                        BestEmployee data = new BestEmployee()
                        {
                            employeeName = "",
                            totalAmount = 0
                        };
                        emp.Add(data);
                    }

                }
                foreach (var item in emp)
                {
                    if (item.totalAmount > 0 && !string.IsNullOrEmpty(item.employeeName))
                    {
                        ChartEntry entry = new ChartEntry((float)item.totalAmount)
                        {
                            Label = item.employeeName.ToString(),
                            ValueLabel = item.totalAmount.ToString("#,###"),
                            Color = SKColor.Parse("#0095DA")
                        };
                        BestEmp.Add(entry);
                    }
                    else
                    {
                        ChartEntry entry = new ChartEntry((float)item.totalAmount)
                        {
                            Label = item.employeeName.ToString(),
                            ValueLabel = item.totalAmount.ToString("#,###"),
                            Color = SKColor.Parse("#FFF"),
                            TextColor = SKColor.Parse("#FFF")

                        };
                        BestEmp.Add(entry);

                    }

                }


                //BarChart chart3 = new BarChart() { Entries = BestEmp, LabelTextSize = 18f };
                //chart3.MaxValue = (float)sumAllAmount;
                //chart3.MinValue = 0;

                ChartSerie chartSerie = new ChartSerie()
                {
                    Entries = BestEmp,
                    Color = SKColor.Parse("FFF"),
                    Name = "BestEmp"
                };

                //BarChart chart3 = new BarChart()
                //{
                //    BackgroundColor = SKColor.Parse("#FFF"),
                //    LabelTextSize = 15f,
                //    Entries = BestEmp,
                //    LabelOrientation = Microcharts.Orientation.Horizontal,
                //    ValueLabelOrientation = Microcharts.Orientation.Horizontal

                //};
                //chartView3.SetMinimumHeight(0);
                //chartView3.Chart = chart3;

                decimal maxvalue = emp.Sum(x => x.totalAmount);
                RecyclerView recyclerViewEmp = FindViewById<RecyclerView>(Resource.Id.recyclerViewEmp);
                var listEmp = emp.Where(x => !string.IsNullOrEmpty(x.employeeName)).OrderByDescending(x => x.totalAmount).ToList();
                Dashboard_Adapter_BestEmp dashboard_adapter_bestemp = new Dashboard_Adapter_BestEmp(listEmp, maxvalue);
                LinearLayoutManager layoutManager = new LinearLayoutManager(this);
                recyclerViewEmp.SetAdapter(dashboard_adapter_bestemp);
                recyclerViewEmp.SetLayoutManager(layoutManager);
                recyclerViewEmp.SetItemViewCacheSize(20);
                recyclerViewEmp.HasFixedSize = true;

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDataDashboard at DSashboard");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }

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

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                GetDataDashboard();
                GetNameBranch();
            }
            catch (Exception)
            {
                base.OnRestart();
            }

        }

        private async void GetNameBranch()
        {
            try
            {
                BranchManage branchManage = new BranchManage();
                List<Branch> lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
                Branch branchName = lstBranch.Where(x => x.BranchID == branchID).FirstOrDefault();
                txtBranchName.Text = branchName.BranchName?.ToString();
            }
            catch (Exception)
            {
                return;
            }
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

