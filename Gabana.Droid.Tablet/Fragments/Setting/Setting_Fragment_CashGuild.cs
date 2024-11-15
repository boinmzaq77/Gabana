
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Gabana.Droid.Tablet.Adapter;
using Gabana.Droid.Tablet.Dialog;
using Gabana.ORM.MerchantDB;
using Gabana.Model;
using Gabana.ORM.PoolDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;
using Gabana.Droid.Tablet.Adapter.Setting;
using Gabana.Droid.Tablet.Fragments.Customers;
using SkiaSharp;
using Gabana.Droid.Helper;
using System.Diagnostics;
using Gabana.Droid.Tablet.Helper;
using Gabana3.JAM.Trans;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_CashGuild : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Setting_Fragment_CashGuild NewInstance()
        {
            Setting_Fragment_CashGuild frag = new Setting_Fragment_CashGuild();
            return frag;
        }

        View view;
        public static Setting_Fragment_CashGuild fragment_main;
        string LoginType, UserLogin;
        internal bool flagLoadData = false;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_cashguild, container, false);
            try
            {
                fragment_main = this;
                LoginType = Preferences.Get("LoginType", "");
                UserLogin = Preferences.Get("User", "");
                ComBineUI();
                flagLoadData = true;
                checkManinRole = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "cash");

                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                var w = mainDisplayInfo.Width;
                var Width = 130;
                MySwipeHelper mySwipe = new MyImplementSwipeHelper(this.Activity, recyclerview_listCash, (int)Width);

                refreshlayout.Refresh += (sender, e) =>
                {
                    flagLoadData = true;
                    OnResume();
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };

                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Branch");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }
        }
        public async override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();

                //if (!IsVisible)
                //{
                //    return;
                //}

                if (flagLoadData)
                {
                    await ShowCash();
                    flagLoadData = false;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Onresume at CashGuide");
            }
        }

        CashTemplateManage cashTemplateManage = new CashTemplateManage();
        List<ORM.Master.CashTemplate> listcashTemplate = new List<ORM.Master.CashTemplate>();
        static List<CashTemplate> lstcashTemplates;
        Setting_Adapter_Cash setting_adapter_cash;
        async Task ShowCash()
        {
            DialogLoading dialogLoading = new DialogLoading();            
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }

                if (DataCashing.CheckNet)
                {
                    listcashTemplate = await GabanaAPI.GetDataCashTemplate();
                    if (listcashTemplate == null)
                    {
                        dialogLoading.Dismiss();
                        return;
                    }
                    //ลบข้อมูลทั้งหมด
                    var delete = await cashTemplateManage.DeleteAllCashTemplatee(DataCashingAll.MerchantId);

                    List<CashTemplate> lst = new List<CashTemplate>();
                    foreach (var item in listcashTemplate)
                    {
                        CashTemplate cashTemplate = new CashTemplate()
                        {
                            Amount = item.Amount,
                            CashTemplateNo = item.CashTemplateNo,
                            DateModified = item.DateModified,
                            MerchantID = item.MerchantID,
                        };
                        var InsertorReplace = await cashTemplateManage.InsertorReplaceCashTemplate(cashTemplate);
                        lst.Add(cashTemplate);
                    }
                    lstcashTemplates = new List<CashTemplate>();
                    lstcashTemplates.AddRange(lst);
                    lstcashTemplates = lstcashTemplates.OrderBy(x => x.CashTemplateNo).ToList();
                }
                else
                {
                    lstcashTemplates = new List<CashTemplate>();
                    lstcashTemplates = await cashTemplateManage.GetAllCashTemplate(DataCashingAll.MerchantId);
                }

                lstcashTemplates = lstcashTemplates.OrderBy(x => x.Amount).ToList();
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                recyclerview_listCash.HasFixedSize = true;
                recyclerview_listCash.SetLayoutManager(mLayoutManager);

                setting_adapter_cash = new Setting_Adapter_Cash(lstcashTemplates);
                recyclerview_listCash.SetItemViewCacheSize(50);
                recyclerview_listCash.SetAdapter(setting_adapter_cash);
                setting_adapter_cash.ItemClick += Setting_adapter_cash_ItemClick; 

                if (setting_adapter_cash.ItemCount == 0)
                {
                    lnNoCash.Visibility = ViewStates.Visible;
                    recyclerview_listCash.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoCash.Visibility = ViewStates.Gone;
                    recyclerview_listCash.Visibility = ViewStates.Visible;
                }
                if (setting_adapter_cash.ItemCount == 8 || !checkManinRole)
                {
                    addCash.SetBackgroundResource(Resource.Mipmap.AddMax);
                    addCash.Enabled = false;
                }
                else
                {
                    addCash.SetBackgroundResource(Resource.Mipmap.Add);
                    addCash.Enabled = true;
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
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowCash at CashGuide");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void Setting_adapter_cash_ItemClick(object sender, int e)
        {
            if (!checkManinRole)
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                return;
            }

            var cash = lstcashTemplates[e];
            DataCashing.EditCashGuide = cash;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addcash");
        }

        ImageButton addCash;
        LinearLayout lnBack;
        SwipeRefreshLayout refreshlayout;
        FrameLayout lnCash;
        LinearLayout lnNoCash;
        RecyclerView recyclerview_listCash;
        public static bool checkManinRole;
        private void ComBineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            refreshlayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
            lnCash = view.FindViewById<FrameLayout>(Resource.Id.lnCash);
            lnNoCash = view.FindViewById<LinearLayout>(Resource.Id.lnNoCash);
            recyclerview_listCash = view.FindViewById<RecyclerView>(Resource.Id.recyclerview_listCash);
            addCash = view.FindViewById<ImageButton>(Resource.Id.addCash);
            lnBack.Click += LnBack_Click;
            addCash.Click += AddCash_Click;
        }

        private void AddCash_Click(object sender, EventArgs e)
        {
            if (!checkManinRole) 
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                return;
            }

            DataCashing.EditCashGuide = null;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addcash");
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
        }
      
        private void LnBack_Click(object sender, EventArgs e)
        {
            Setting_Fragment_Main.SetEnableBtnBranch();
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
        }

        public void ReloadCashGuide(CashTemplate NewcashGuide)
        {
            try
            {                
                int index = 0;
                index = lstcashTemplates.FindIndex(x => x.CashTemplateNo == NewcashGuide.CashTemplateNo);
                if (index > -1)
                {
                    lstcashTemplates[index] = NewcashGuide;
                    setting_adapter_cash.NotifyItemChanged(index);
                    return;
                }

                lstcashTemplates.Insert(0, NewcashGuide);
                recyclerview_listCash.SmoothScrollToPosition(0);
                setting_adapter_cash.NotifyItemInserted(0);

                if (lstcashTemplates.Count == 8)
                {
                    addCash.SetBackgroundResource(Resource.Mipmap.AddMax);
                    addCash.Enabled = false;
                }
                else
                {
                    addCash.SetBackgroundResource(Resource.Mipmap.Add);
                    addCash.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ReloadCustomer at Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }        

        public void DeleteCashGuid(CashTemplate DeleteCashGuide)
        {
            try
            {
                int index = 0;
                index = lstcashTemplates.FindIndex(x => x.CashTemplateNo == DeleteCashGuide.CashTemplateNo);
                if (index == -1)
                {
                    return;
                }

                lstcashTemplates.RemoveAt(index);
                setting_adapter_cash.NotifyItemRemoved(index);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DeleteCustomer at Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
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
                        var MasterCashTemplate = new ORM.Master.CashTemplate()
                        {
                            Amount = lstcashTemplates[position].Amount,
                            CashTemplateNo = (int)lstcashTemplates[position].CashTemplateNo,
                            MerchantID = (int)lstcashTemplates[position].MerchantID,
                            DateModified = lstcashTemplates[position].DateModified,
                        };

                        var fragment = new Cashguide_Dialog_Delete();
                        fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Cashguide_Dialog_Delete));
                        Cashguide_Dialog_Delete.SetCashguild(MasterCashTemplate);
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
}
