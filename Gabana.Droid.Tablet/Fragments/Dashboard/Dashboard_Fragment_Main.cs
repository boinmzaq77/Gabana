using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using Gabana3.JAM.Dashboard;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AndroidX.RecyclerView.Widget;
using TinyInsightsLib;
using Gabana.Droid.Tablet.Adapter.Dashboard;
using Gabana.ORM.MerchantDB;
using Gabana.Model;
using Xamarin.Essentials;
using Android.Support.V4.Widget;
using System.ComponentModel;
using System.Threading;
using iTextSharp.text.pdf;
using Gabana.ORM.Master;

namespace Gabana.Droid.Tablet.Fragments.Dashboard
{
    public class Dashboard_Fragment_Main : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Dashboard_Fragment_Main NewInstance()
        {
            Dashboard_Fragment_Main frag = new Dashboard_Fragment_Main();
            return frag;
        }
        string SearchName, LoginType;
        View view;
        ChartView chartView1, chartView2, chartView3;
        internal static string branchID;
        LinearLayout lnHaveDashboard, lnNoDashboard, lnChooseBranch;
        TextView txtBranchName, txtDate2, txtSaleTotal, txtSaleCount, txtProfit, txtAvgSale, txtComparison;
        RecyclerView rcvBestSale, rcvPayment;
        SwipeRefreshLayout lnSwipeRefresh;
        ImageView imgProfile;
        List<Gabana.Model.PaymentType> listPayments = new List<Gabana.Model.PaymentType>
            {
                new Model.PaymentType(){ Type ="CH" ,Detail = "Cash", Logo = Resource.Mipmap.DbColorCash , color = "#0095DA"},
                new Model.PaymentType(){ Type ="Cr" ,Detail = "Credit Card", Logo = Resource.Mipmap.DbColorCredit, color = "#F8971D"},
                new Model.PaymentType() { Type = "Dr", Detail = "Debit Card", Logo = Resource.Mipmap.DbColorDebit, color = "#E32D49" },
                new Model.PaymentType() { Type = "GV", Detail = "Gift Voucher", Logo = Resource.Mipmap.DbColorGiftVoucher, color = "#37AA52" },
                new Model.PaymentType() { Type = "MYQR", Detail = "myQR", Logo = Resource.Mipmap.DbColormyQR, color = "#F75600" },
                new Model.PaymentType() { Type = "QRCH", Detail = "QR Cash", Logo = Resource.Mipmap.DbColorQRCash, color = "#3F51B5" },
                new Model.PaymentType() { Type = "QRCR", Detail = "QR Credit", Logo = Resource.Mipmap.DbColorQRCredit, color = "#00796B" },
                new Model.PaymentType() { Type = "WECHAT", Detail = "WECHAT", Logo = Resource.Mipmap.DbColorQRWechat, color = "#8BC34A" }
            };
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.dashboard_fragment_main, container, false);
            try
            {
                CombineUI();

                branchID = Preferences.Get("Branch", "");
                if (string.IsNullOrEmpty(branchID))
                {
                    branchID = "0";
                }

                CheckJwt();

                return view;
            }
            catch (Exception)
            {
                return view;

            }
        }

        private async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
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
        private void LnChooseBranch_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(DashboardBranchActivity)));
        }

        private async void GetNameBranch()
        {
            try
            {
                BranchManage branchManage = new BranchManage();
                List<ORM.MerchantDB.Branch> lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
                ORM.MerchantDB.Branch branchName = lstBranch.Where(x => x.BranchID == branchID).FirstOrDefault();
                txtBranchName.Text = branchName.BranchName?.ToString();
            }
            catch (Exception)
            {
                return;
            }
        }

        private void CombineUI()
        {

            chartView1 = view.FindViewById<ChartView>(Resource.Id.chartView1);
            chartView2 = view.FindViewById<ChartView>(Resource.Id.chartView2);
            txtDate2 = view.FindViewById<TextView>(Resource.Id.txtDate2);
            lnHaveDashboard = view.FindViewById<LinearLayout>(Resource.Id.lnHaveDashboard);
            lnNoDashboard = view.FindViewById<LinearLayout>(Resource.Id.lnNoDashboard);
            txtSaleTotal = view.FindViewById<TextView>(Resource.Id.txtSaleTotal);
            txtSaleCount = view.FindViewById<TextView>(Resource.Id.txtSaleCount);
            txtProfit = view.FindViewById<TextView>(Resource.Id.txtProfit);
            txtAvgSale = view.FindViewById<TextView>(Resource.Id.txtAvgSale);
            txtComparison = view.FindViewById<TextView>(Resource.Id.txtComparison);

            rcvBestSale = view.FindViewById<RecyclerView>(Resource.Id.rcvBestSale);
            rcvPayment = view.FindViewById<RecyclerView>(Resource.Id.rcvPayment);

            txtBranchName = view.FindViewById<TextView>(Resource.Id.txtBranchName);

            lnSwipeRefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.lnSwipeRefresh);
            lnChooseBranch = view.FindViewById<LinearLayout>(Resource.Id.lnChooseBranch);
            lnChooseBranch.Click += LnChooseBranch_Click;

            imgProfile = view.FindViewById<ImageView>(Resource.Id.imgProfile);

            lnSwipeRefresh.Refresh += (sender, e) =>
            {
                OnResume();
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();

            };
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lnSwipeRefresh.Refreshing = false;
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }
        public override void OnResume()
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                GetDataDashboard();
                GetNameBranch();
            }
            catch (Exception)
            {
                return;
            }
        }
        async void GetDataDashboard()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
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

                var imgMerchantlogo = DataCashingAll.MerchantLocal;
                var cloudpath = imgMerchantlogo.LogoPath == null ? string.Empty : imgMerchantlogo.LogoPath;
                var localpath = imgMerchantlogo.LogoLocalPath == null ? string.Empty : imgMerchantlogo.LogoLocalPath;

                if (string.IsNullOrEmpty(localpath))
                {
                    if (string.IsNullOrEmpty(cloudpath))
                    {
                        //defalut
                        imgProfile.SetBackgroundResource(Resource.Mipmap.defaultcust);
                    }
                    else
                    {
                        //cloud
                        Utils.SetImage(imgProfile, cloudpath);
                    }
                }
                else
                {
                    //local
                    Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                    imgProfile.SetImageURI(uri);
                }


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
                GridLayoutManager gridlayoutManage = new GridLayoutManager(this.Activity, 2 , 1,false);
                rcvPayment.SetAdapter(dashboard_Adapter_ShowPayment);
                rcvPayment.SetLayoutManager(gridlayoutManage);
                rcvPayment.SetItemViewCacheSize(20);
                rcvPayment.HasFixedSize = true;

                List<ORM.MerchantDB.Item> items = new List<ORM.MerchantDB.Item>();
                ItemManage itemManage = new ItemManage();
                items = await itemManage.GetAllItemType();



                ListBestSallItem listBestSallIteem = new  ListBestSallItem(result.bestSellingItems);
                Dashboard_Adapter_Item dashboard_adapter_item = new Dashboard_Adapter_Item(items, listBestSallIteem);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvBestSale.SetLayoutManager(gridLayoutItem);
                rcvBestSale.HasFixedSize = true;
                rcvBestSale.SetAdapter(dashboard_adapter_item);


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
                RecyclerView rcvEmp = view.FindViewById<RecyclerView>(Resource.Id.rcvEmp);
                var listEmp = emp.Where(x => !string.IsNullOrEmpty(x.employeeName)).OrderByDescending(x => x.totalAmount).ToList();
                Dashboard_Adapter_BestEmp dashboard_adapter_bestemp = new Dashboard_Adapter_BestEmp(listEmp, maxvalue);
                LinearLayoutManager layoutManager = new LinearLayoutManager(this.Activity);
                rcvEmp.SetAdapter(dashboard_adapter_bestemp);
                rcvEmp.SetLayoutManager(layoutManager);
                rcvEmp.SetItemViewCacheSize(20);
                rcvEmp.HasFixedSize = true;

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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }

        }


    }

    public class ListBestSallItem
    {
        public List<Gabana3.JAM.Dashboard.BestSellingItem> bestSellings;
        static List<Gabana3.JAM.Dashboard.BestSellingItem> builitem;
        public ListBestSallItem(List<Gabana3.JAM.Dashboard.BestSellingItem> bestSellings)
        {
            builitem = bestSellings.OrderByDescending(x => x.totalAmount).ToList();
            this.bestSellings = builitem;
        }
        public int Count
        {

            get
            {
                if (bestSellings.Count <= 5)
                {
                    return bestSellings == null ? 0 : bestSellings.Count;
                }
                else
                {
                    return bestSellings == null ? 0 : 5;
                }
            }
        }
        public Gabana3.JAM.Dashboard.BestSellingItem this[int i]
        {
            get { return bestSellings == null ? null : bestSellings[i]; }
        }
        
    }

}