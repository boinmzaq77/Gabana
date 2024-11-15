using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Droid.Helper;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class GiftvoucherActivity : AppCompatActivity
    {
        public static GiftvoucherActivity activity;
        SwipeRefreshLayout refreshlayout;
        ImageButton addGiftvoucher;
        RecyclerView recyclerview_listGiftvoucher;
        GiftVoucher_Adapter_Main giftVoucher_Adapter;
        public static List<GiftVoucher> lstvouchers;
        ListGiftVoucher listgiftvoucher;
        GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
        LinearLayout lnNoGiftvoucher;
        List<ORM.Master.GiftVoucher> giftVouchers = new List<ORM.Master.GiftVoucher>();
        bool checkManinRole;
        private string LoginType;
        public static GiftVoucher FocusGiftVoucher;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.giftvoucher_activity);
                activity = this;
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnNoGiftvoucher = FindViewById<LinearLayout>(Resource.Id.lnNoGiftvoucher);
                CheckJwt();

                lnBack.Click += LnBack_Click;
                addGiftvoucher = FindViewById<ImageButton>(Resource.Id.addGiftvoucher);
                addGiftvoucher.Click += AddGiftvoucher_Click;
                refreshlayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
                refreshlayout.Refresh += async (sender, e) =>
                {
                    if (!await GabanaAPI.CheckNetWork())
                    {
                        Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    }
                    else if (!await GabanaAPI.CheckSpeedConnection())
                    {
                        Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    }
                    else
                    {
                        DataCashingAll.flagGiftVoucherChange = true;
                        OnResume();
                    }
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };
                DataCashingAll.flagGiftVoucherChange = true;

                recyclerview_listGiftvoucher = FindViewById<RecyclerView>(Resource.Id.recyclerview_listGiftvoucher);
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerview_listGiftvoucher.HasFixedSize = true;
                recyclerview_listGiftvoucher.SetLayoutManager(mLayoutManager);

                LoginType = Preferences.Get("LoginType", "");
                checkManinRole = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "giftvoucher");
                UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "giftvoucher");

                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                var w = mainDisplayInfo.Width;
                var Width = w / 5;
                MySwipeHelper mySwipe = new MyImplementSwipeHelper(this, recyclerview_listGiftvoucher, (int)Width);

                _ = TinyInsights.TrackPageViewAsync("OnCreate : GiftvoucherActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Giftvoucher");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
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
        private async Task GetGiftvoucherData()
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                lstvouchers = new List<GiftVoucher>();

                if (await GabanaAPI.CheckSpeedConnection())
                {
                    List<GiftVoucher> gifts = new List<GiftVoucher>();
                    giftVouchers = await GabanaAPI.GetDataGiftVoucher();
                    if (giftVouchers == null)
                    {
                        dialogLoading.Dismiss();
                        return;
                    }
                    if (giftVouchers.Count == 0)
                    {
                        lstvouchers = new List<GiftVoucher>();
                    }
                    if (giftVouchers.Count > 0)
                    {
                        //ลบข้อมูลทั้งหมด
                        var Allgifts = await giftVoucherManage.DeleteAllGiftVoucher(DataCashingAll.MerchantId);

                        var lst = giftVouchers.OrderBy(x => x.FmlAmount).ToList();
                        foreach (var item in lst)
                        {
                            ORM.MerchantDB.GiftVoucher giftVoucher = new GiftVoucher()
                            {
                                DateCreated = item.DateCreated,
                                DateModified = item.DateModified,
                                FmlAmount = item.FmlAmount,
                                GiftVoucherCode = item.GiftVoucherCode,
                                GiftVoucherName = item.GiftVoucherName,
                                MerchantID = item.MerchantID,
                                Ordinary = item.Ordinary,
                                UserNameModified = item.UserNameModified
                            };
                            await giftVoucherManage.InsertOrReplaceGiftVoucher(giftVoucher);
                            gifts.Add(giftVoucher);
                        }
                        lstvouchers = new List<GiftVoucher>();
                        lstvouchers.AddRange(gifts);
                    }
                }
                else
                {
                    lstvouchers = await giftVoucherManage.GetAllGiftVoucher();
                    if (lstvouchers == null)
                    {
                        Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                        lstvouchers = new List<GiftVoucher>();
                    }
                }

                SetGiftvoucherData();

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                    dialogLoading = new DialogLoading();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetGiftvoucherData at Giftvoucher");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void SetGiftvoucherData()
        {
            try
            {
                if (lstvouchers.Count > 0)
                {
                    lstvouchers = lstvouchers.OrderBy(x => x.GiftVoucherCode.ToLower()).ToList();
                }
                listgiftvoucher = new ListGiftVoucher(lstvouchers);
                giftVoucher_Adapter = new GiftVoucher_Adapter_Main(listgiftvoucher);
                recyclerview_listGiftvoucher.SetItemViewCacheSize(50);
                recyclerview_listGiftvoucher.SetAdapter(giftVoucher_Adapter);
                giftVoucher_Adapter.ItemClick += GiftVoucher_Adapter_ItemClick;

                if (giftVoucher_Adapter.ItemCount == 0)
                {
                    lnNoGiftvoucher.Visibility = ViewStates.Visible;
                }
                else
                {
                    lnNoGiftvoucher.Visibility = ViewStates.Gone;
                }

                if (!checkManinRole)
                {
                    addGiftvoucher.SetBackgroundResource(Resource.Mipmap.AddMax);
                    addGiftvoucher.Enabled = false;
                }
                else
                {
                    addGiftvoucher.SetBackgroundResource(Resource.Mipmap.Add);
                    addGiftvoucher.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                addGiftvoucher.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetGiftvoucherData at Giftvoucher");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void GiftVoucher_Adapter_ItemClick(object sender, int e)
        {
            try
            {
                if (checkManinRole)
                {
                    var voucher = lstvouchers[e];
                    StartActivity(new Intent(Application.Context, typeof(AddGiftvoucherActivity)));
                    AddGiftvoucherActivity.SetVoucherDetail(voucher);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                    return;
                }               
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GiftVoucher_Adapter_ItemClick at giftvoucher");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void AddGiftvoucher_Click(object sender, EventArgs e)
        {
            addGiftvoucher.Enabled = false;
            if (!checkManinRole)
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                addGiftvoucher.Enabled =true;
                return;
            }
            StartActivity(new Intent(Application.Context, typeof(AddGiftvoucherActivity)));
            addGiftvoucher.Enabled = true;
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            FocusGiftVoucher = null;
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }
        protected async override void OnResume()
        {
            base.OnResume();
            CheckJwt();
            if (DataCashingAll.flagGiftVoucherChange)
            {
                await GetGiftvoucherData();
                DataCashingAll.flagGiftVoucherChange = false;
            }
            GiftVoucherFocus();
        }
        public void Resume()
        {
            OnResume();
        }
        private void GiftVoucherFocus()
        {
            try
            {
                if (FocusGiftVoucher != null)
                {
                    if (lstvouchers.Count == 0)
                    {
                        lstvouchers.Add(FocusGiftVoucher);
                        SetGiftvoucherData();
                        FocusGiftVoucher = null;
                        return;
                    }
                    int index = lstvouchers.FindIndex(x => x.GiftVoucherCode == FocusGiftVoucher.GiftVoucherCode);
                    if (index != -1)
                    {
                        lstvouchers.RemoveAt(0);
                    }
                    lstvouchers.Insert(0, FocusGiftVoucher);
                    giftVoucher_Adapter.NotifyDataSetChanged();
                    FocusGiftVoucher = null;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GiftVoucherFocus at GiftVoucher");
            }
        }
        internal static void SetFocusGiftVoucher(GiftVoucher voucher)
        {
            try
            {
                FocusGiftVoucher = voucher;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFocusGiftVoucher at GiftVoucher");
            }
        }
        private class MyImplementSwipeHelper : MySwipeHelper
        {
            Context context;
            RecyclerView recyclerView;
            int buttonWidth;
            public MyImplementSwipeHelper(Context context, RecyclerView recyclerView, int buttonWidth) : base(context, recyclerView, buttonWidth)
            {
                this.context = context;
                this.recyclerView = recyclerView;
                this.buttonWidth = buttonWidth;
            }

            public override void InstantiateMybutton(RecyclerView.ViewHolder viewHolder, List<MyButton> buffer)
            {
                buffer.Add(new MyButton(context,
                    "Delete",
                    0,
                    Resource.Mipmap.DeleteBt,
                    "#33AAE1",
                    new MyDeleteButtonClick(this)));
            }

            private class MyDeleteButtonClick : MyButtonClickListener
            {
                private MyImplementSwipeHelper myImplementSwipeHelper;

                public MyDeleteButtonClick(MyImplementSwipeHelper myImplementSwipeHelper)
                {
                    this.myImplementSwipeHelper = myImplementSwipeHelper;
                }
                public void OnClick(int position)
                {
                    try
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("deleteType", "giftvoucher");
                        bundle.PutString("giftVoucherCode", lstvouchers[position].GiftVoucherCode);
                        bundle.PutString("fromPage", "main");
                        dialog.Arguments = bundle;
                        dialog.Show(activity.SupportFragmentManager, myMessage);
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("MyImplementSwipeHelper at giftvoucher");
                        Toast.MakeText(myImplementSwipeHelper.context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }
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

