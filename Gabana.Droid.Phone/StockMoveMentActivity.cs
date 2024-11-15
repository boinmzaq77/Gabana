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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class StockMoveMentActivity : AppCompatActivity
    {
        public static StockMoveMentActivity activity;
        SwipeRefreshLayout refreshlayout;
        RecyclerView recyclerview_listStock;
        public static List<ItemMovement> lstItemMovement;
        ListStockMoveMent listStock;
        private static Item item;
        int Last, offset, position;
        bool islast;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                islast = true;
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.stock_activity);
                activity = this;
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                ImageView imageViewItem = FindViewById<ImageView>(Resource.Id.imageViewItem);
                TextView textViewItemName = FindViewById<TextView>(Resource.Id.textViewItemName);
                TextView txtNameItem = FindViewById<TextView>(Resource.Id.txtNameItem);
                TextView txtMinimaumStock = FindViewById<TextView>(Resource.Id.txtMinimaumStock);
                TextView txtStockBalance = FindViewById<TextView>(Resource.Id.txtStockBalance);
                recyclerview_listStock = FindViewById<RecyclerView>(Resource.Id.recyclerview_listStock);

                refreshlayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
                refreshlayout.Refresh += (sender, e) =>
                {
                    OnResume();
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };

                lstItemMovement = new List<ItemMovement>();

                if (item == null)
                {
                    return;
                }
                var paths = item.ThumbnailLocalPath;
                if (!string.IsNullOrEmpty(paths))
                {
                    Android.Net.Uri uri = Android.Net.Uri.Parse(paths);
                    imageViewItem.SetImageURI(uri);
                    textViewItemName.Visibility = ViewStates.Gone;
                }
                else
                {
                    string conColor = Utils.SetBackground(Convert.ToInt32(item.Colors));
                    var color = Android.Graphics.Color.ParseColor(conColor);
                    imageViewItem.SetBackgroundColor(color);
                    textViewItemName.Text = item.ItemName?.ToString();
                }
                txtNameItem.Text = item.ItemName?.ToString();

                CheckJwt();
                var stock = await GabanaAPI.GetDataStock((int)DataCashingAll.SysBranchId, (int)item.SysItemID);
                if (stock != null)
                {
                    txtMinimaumStock.Text = stock.MinimumStock.ToString("#,###");
                    txtStockBalance.Text = stock.BalanceStock.ToString("#,###");
                }
                else
                {
                    txtMinimaumStock.Text = "0";
                    txtStockBalance.Text = "0";
                }
                offset = 0;
                position = 0;

                _ = TinyInsights.TrackPageViewAsync("OnCreate : StockMoveMentActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Stock");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        async void GetDataStock()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                lstItemMovement = await GabanaAPI.GetDataStockItemMovement(DataCashingAll.SysBranchId, (int)item.SysItemID, offset);
                if (lstItemMovement == null)
                {
                    lstItemMovement = new List<ItemMovement>();
                }

                Last = lstItemMovement.Count;
                listStock = new ListStockMoveMent(lstItemMovement);
                Stock_Adapter_Main stock_Adapter_Main = new Stock_Adapter_Main(listStock);
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerview_listStock.SetLayoutManager(mLayoutManager);
                recyclerview_listStock.HasFixedSize = true;
                recyclerview_listStock.SetItemViewCacheSize(100);
                if (Last == 100)
                {
                    stock_Adapter_Main.OnCardCellbtnIndex += Stock_Adapter_Main_OnCardCellbtnIndex;
                }
                recyclerview_listStock.SetAdapter(stock_Adapter_Main);
                position = stock_Adapter_Main.ItemCount;

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
                _ = TinyInsights.TrackPageViewAsync("GetDataStock at Stock");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void Stock_Adapter_Main_OnCardCellbtnIndex()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                offset++;
                if (islast)
                {
                    position = lstItemMovement.Count - 1;
                    dialogLoading.Show(SupportFragmentManager, nameof(DialogLoading));

                    List<ItemMovement> lst = new List<ItemMovement>();
                    lst = await GabanaAPI.GetDataStockItemMovement(DataCashingAll.SysBranchId, (int)item.SysItemID, offset);
                    lstItemMovement.AddRange(lst);

                    if (Last == lstItemMovement.Count)
                    {
                        islast = false;
                    }
                    Last = lstItemMovement.Count;
                    listStock = new ListStockMoveMent(lstItemMovement);
                    var ad = recyclerview_listStock.GetAdapter() as Stock_Adapter_Main;
                    ad.refresh(listStock);
                }

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
                _ = TinyInsights.TrackPageViewAsync("Bill_Adapter_Main_OnCardCellbtnIndex5 at ItemMovement");
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
                offset = 0;
                islast = true;
                GetDataStock();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at ItemMovement");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                base.OnRestart();
            }
        }

        internal static void SetPageView(Item i)
        {
            item = i;
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'StockMoveMentActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'StockMoveMentActivity.openPage' is assigned but its value is never used
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

