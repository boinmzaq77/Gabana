using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Gabana.Droid.Helper;
using Gabana.Droid.Tablet.Adapter.Setting;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Customers;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Droid.Tablet.Helper;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_GiftVoucher : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Setting_Fragment_GiftVoucher NewInstance()
        {
            Setting_Fragment_GiftVoucher frag = new Setting_Fragment_GiftVoucher();
            return frag;
        }

        View view;
        public static Setting_Fragment_GiftVoucher fragment_giftvoucher;

        public override  View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_giftvoucher, container, false);
            try
            {
                fragment_giftvoucher = this;
                CheckJwt();
                ComBineUI();

                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                var w = mainDisplayInfo.Width;
                var Width = 130;
                MySwipeHelper mySwipe = new MyImplementSwipeHelper(this.Activity, rcvGiftvoucher, (int)Width);

                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("OnCreate at GiftVoucher");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }
        }
        LinearLayout lnBack, lnNoGiftvoucher;
        SwipeRefreshLayout refreshlayout;
        FrameLayout lnGiftvoucher;
        RecyclerView rcvGiftvoucher;
        ImageButton addGiftvoucher;

        private void ComBineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnNoGiftvoucher = view.FindViewById<LinearLayout>(Resource.Id.lnNoGiftvoucher);
            lnGiftvoucher = view.FindViewById<FrameLayout>(Resource.Id.lnGiftvoucher);
            rcvGiftvoucher = view.FindViewById<RecyclerView>(Resource.Id.rcvGiftvoucher);
            addGiftvoucher = view.FindViewById<ImageButton>(Resource.Id.addGiftvoucher);
            refreshlayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);            
            refreshlayout.Refresh += async (sender, e) =>
            {
                if (!DataCashing.CheckNet)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                }
                else
                {
                    DataCashingAll.flagGiftVoucherChange = true;
                    OnResume();
                }
                DataCashingAll.flagGiftVoucherChange = true;
                await GetGiftvoucherData();
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();
            };
            lnBack.Click += LnBack_Click;
            addGiftvoucher.Click += AddGiftvoucher_Click;
        }
        public static List<GiftVoucher> lstvouchers = new List<GiftVoucher>();
        List<ORM.Master.GiftVoucher> giftVouchers = new List<ORM.Master.GiftVoucher>();
        GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
        ListGiftVoucher listgiftvoucher;
        Setting_Adapter_GiftVoucher giftVoucher_Adapter;
        
        private async Task GetGiftvoucherData()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {                
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }
                lstvouchers = new List<GiftVoucher>();
                if (DataCashing.CheckNet)
                {
                    List<GiftVoucher> gifts = new List<GiftVoucher>();
                    giftVouchers = await GabanaAPI.GetDataGiftVoucher();
                    if (giftVouchers == null)
                    {
                        lstvouchers = new List<GiftVoucher>();
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
                        Toast.MakeText(this.Activity, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
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
                dialogLoading = new DialogLoading();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetGiftvoucherData at Giftvoucher");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                giftVoucher_Adapter = new Setting_Adapter_GiftVoucher(listgiftvoucher);
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                rcvGiftvoucher.HasFixedSize = true;
                rcvGiftvoucher.SetLayoutManager(mLayoutManager);
                rcvGiftvoucher.SetItemViewCacheSize(50);
                rcvGiftvoucher.SetAdapter(giftVoucher_Adapter);
                giftVoucher_Adapter.ItemClick += GiftVoucher_Adapter_ItemClick;

                if (lstvouchers.Count == 0)
                {
                    lnNoGiftvoucher.Visibility = ViewStates.Visible;
                    rcvGiftvoucher.Visibility = ViewStates.Gone;

                }
                else
                {
                    lnNoGiftvoucher.Visibility = ViewStates.Gone;
                    rcvGiftvoucher.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetGiftvoucherData at Giftvoucher");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void GiftVoucher_Adapter_ItemClick(object sender, int e)
        {
            try
            {
                DataCashing.EditGiftVoucher = lstvouchers[e];
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addgiftvoucher");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GiftVoucher_Adapter_ItemClick at giftvoucher");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void AddGiftvoucher_Click(object sender, EventArgs e)
        {
            DataCashing.EditGiftVoucher = null;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addgiftvoucher");
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }
        private async void LnBack_Click(object sender, EventArgs e)
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
        }

        public async void ReloadGiftVoucher(GiftVoucher NewGiftVoucher)
        {
            try
            {
                int index = 0;
                index = lstvouchers.FindIndex(x => x.GiftVoucherCode == NewGiftVoucher.GiftVoucherCode);
                if (index > -1)
                {
                    lstvouchers[index] = NewGiftVoucher;
                    giftVoucher_Adapter.NotifyItemChanged(index);
                    return;
                }

                await GetGiftvoucherData();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ReloadCustomer at Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public void DeleteGiftVoucher(GiftVoucher _DeleteGiftVoucher)
        {
            try
            {
                int index = 0;
                index = lstvouchers.FindIndex(x => x.GiftVoucherCode == _DeleteGiftVoucher.GiftVoucherCode);
                if (index == -1)
                {
                    return;
                }
                lstvouchers.RemoveAt(index);
                giftVoucher_Adapter.NotifyItemRemoved(index);
                Setting_Fragment_AddGiftVoucher.fragment_giftvoucher.UINewGiftVoucher();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DeleteCustomer at Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public override async void OnResume()
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                CheckJwt();                
                await GetGiftvoucherData();                
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                Log.Debug("Token", "Token" + " " + res.gbnJWT);
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
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
                    Resource.Mipmap.DeleteBt2,
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
                        DataCashing.EditGiftVoucher =  lstvouchers[position];
                        var fragment = new Giftvoucher_Dialog_Delete();
                        fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Giftvoucher_Dialog_Delete));
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

    }

    public class ListGiftVoucher
    {
        public List<GiftVoucher> vouchers;
        static List<GiftVoucher> builitem;
        public ListGiftVoucher(List<GiftVoucher> vouchers)
        {
            builitem = vouchers;
            this.vouchers = builitem;
        }
        public int Count
        {
            get
            {
                return vouchers == null ? 0 : vouchers.Count;
            }
        }
        public GiftVoucher this[int i]
        {
            get { return vouchers == null ? null : vouchers[i]; }
        }
    }



}
