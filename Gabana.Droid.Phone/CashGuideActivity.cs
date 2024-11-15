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
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class CashGuideActivity : AppCompatActivity
    {
        public static CashGuideActivity cashGuideActivity;
        LinearLayout lnBack;
        Button btnSave;
        static List<CashTemplate> lstcashTemplates;
        CultureInfo culture = new CultureInfo("en-US");
        CashTemplateManage CashTemplateManage = new CashTemplateManage();
        public static RecyclerView recyclerview_listCash;
        ImageButton addCash;
        public static bool flagLoadData = false;
        private string LoginType;
        SwipeRefreshLayout refreshlayout;
        LinearLayout lnNoCash;
        FrameLayout lnCash;
        List<ORM.Master.CashTemplate> listcashTemplate = new List<ORM.Master.CashTemplate>();
        public static bool checkManinRole;
        public static CashTemplate FocusNewCashGuide;
        CashTemplate_Adapter_Main cashTemplate_Adapter;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.cashguide_activity);
                cashGuideActivity = this;
                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                btnSave = FindViewById<Button>(Resource.Id.btnSave);
                lnBack.Click += LnBack_Click;
                lnNoCash = FindViewById<LinearLayout>(Resource.Id.lnNoCash);
                lnCash = FindViewById<FrameLayout>(Resource.Id.lnCash);
                addCash = FindViewById<ImageButton>(Resource.Id.addCash);
                addCash.Click += AddCash_Click;
                recyclerview_listCash = FindViewById<RecyclerView>(Resource.Id.recyclerview_listCash);

                CheckJwt();

                refreshlayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
                LoginType = Preferences.Get("LoginType", "");
                checkManinRole = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "cash");
                UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "cash");
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
                        flagLoadData = true;
                        OnResume();
                    }                    
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                var w = mainDisplayInfo.Width;
                var Width = w / 5;

                
                MySwipeHelper mySwipe = new MyImplementSwipeHelper(this, recyclerview_listCash, (int)Width);
                flagLoadData = true;

                _ = TinyInsights.TrackPageViewAsync("OnCreate : CashGuideActivity");
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Cash");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }        

        private void AddCash_Click(object sender, EventArgs e)
        {
            if (!checkManinRole)
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                return;
            }

            StartActivity(new Intent(Application.Context, typeof(AddCashGuideActivity)));
            AddCashGuideActivity.SetCashTemplate(null);
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }
        async Task GetCashGuideData()
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                if (await GabanaAPI.CheckSpeedConnection())
                {
                    listcashTemplate = await GabanaAPI.GetDataCashTemplate();
                    if (listcashTemplate == null)
                    {
                        dialogLoading.Dismiss();
                        return;
                    }
                    //ลบข้อมูลทั้งหมด
                    var delete = await CashTemplateManage.DeleteAllCashTemplatee(DataCashingAll.MerchantId);

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
                        var InsertorReplace = await CashTemplateManage.InsertorReplaceCashTemplate(cashTemplate);
                        lst.Add(cashTemplate);
                    }
                    lstcashTemplates = new List<CashTemplate>();
                    lstcashTemplates.AddRange(lst);
                    lstcashTemplates = lstcashTemplates.OrderBy(x => x.CashTemplateNo).ToList();
                }
                else
                {
                    lstcashTemplates = new List<CashTemplate>();
                    lstcashTemplates = await CashTemplateManage.GetAllCashTemplate(DataCashingAll.MerchantId);
                }

                SetCashGuideData();

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
                _ = TinyInsights.TrackPageViewAsync("GetCashGuideData at CashGuide");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private void SetCashGuideData()
        {
            try
            {
                lstcashTemplates = lstcashTemplates.OrderBy(x => x.Amount).ToList();
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerview_listCash.HasFixedSize = false;
                recyclerview_listCash.SetLayoutManager(mLayoutManager);
                cashTemplate_Adapter = new CashTemplate_Adapter_Main(lstcashTemplates);
                recyclerview_listCash.SetItemViewCacheSize(50);
                recyclerview_listCash.SetAdapter(cashTemplate_Adapter);
                cashTemplate_Adapter.ItemClick += CashTemplate_Adapter_ItemClick;

                if (cashTemplate_Adapter.ItemCount == 0)
                {
                    lnNoCash.Visibility = ViewStates.Visible;
                    recyclerview_listCash.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoCash.Visibility = ViewStates.Gone;
                    recyclerview_listCash.Visibility = ViewStates.Visible;
                }
                if (cashTemplate_Adapter.ItemCount == 8 || !checkManinRole)
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
            catch (Exception)
            {
                throw;
            }
        }
        private void CashTemplate_Adapter_ItemClick(object sender, int e)
        {
            if (checkManinRole)
            {
                var cash = lstcashTemplates[e];
                StartActivity(new Intent(this, typeof(AddCashGuideActivity)));
                AddCashGuideActivity.SetCashTemplate(cash);
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                return;
            }
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        protected override async void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                if (flagLoadData)
                {
                    await GetCashGuideData();
                    flagLoadData = false;
                }
                CashGuideFocus();
            }
            catch (Exception)
            {
                base.OnRestart();
            }
        }
        private void CashGuideFocus()
        {
            try
            {
                if (FocusNewCashGuide != null)
                {
                    int index = -1;
                    if (lstcashTemplates != null)
                    {
                        if (lstcashTemplates.Count == 0)
                        {
                            lstcashTemplates.Add(FocusNewCashGuide);
                            SetCashGuideData();
                            FocusNewCashGuide = null;
                            return;
                        }

                        index = lstcashTemplates.FindIndex(x => x.CashTemplateNo == FocusNewCashGuide.CashTemplateNo);
                        if (index != -1)
                        {
                            lstcashTemplates.RemoveAt(index);
                        }
                        lstcashTemplates.Insert(0, FocusNewCashGuide);
                    }
                    cashTemplate_Adapter.NotifyDataSetChanged();
                    FocusNewCashGuide = null;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ItemFocus at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        internal static void SetFocusCashGuide(CashTemplate template)
        {
            try
            {
                FocusNewCashGuide = template;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFocusCashGuide at CashTemplate");
            }
        }
        public void Resume()
        {
            OnResume();
        }
        public string CheckLenghtValue(string strValue)
        {
            try
            {
                string pattern = "[.]";
                string replacement = "";

                System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(pattern);
                var check = System.Text.RegularExpressions.Regex.Replace(regEx.Replace(strValue, replacement), @"\s+", "");
                return check;
            }
            catch (Exception)
            {
                return strValue;
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
                    //delete
                    try
                    {
                        var lstCashTemplate = new List<CashTemplate>();
                        var MasterCashTemplate = new CashTemplate()
                        {
                            Amount = lstcashTemplates[position].Amount,
                            CashTemplateNo = lstcashTemplates[position].CashTemplateNo,
                            MerchantID = lstcashTemplates[position].MerchantID,
                            DateModified = lstcashTemplates[position].DateModified,
                        };
                        lstCashTemplate.Add(MasterCashTemplate);
                        var json = JsonConvert.SerializeObject(lstCashTemplate);

                        if (CashGuideActivity.checkManinRole)
                        {
                            MainDialog dialog = new MainDialog();
                            Bundle bundle = new Bundle();
                            String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                            bundle.PutString("message", myMessage);
                            bundle.PutString("CashGuid", "CashGuid");
                            bundle.PutString("CashGuidData", json);
                            bundle.PutString("fromPage", "main");
                            dialog.Arguments = bundle;
                            dialog.Show(cashGuideActivity.SupportFragmentManager, myMessage);
                        }
                        else
                        {
                            Toast.MakeText(cashGuideActivity, cashGuideActivity.GetString(Resource.String.notperm), ToastLength.Short).Show();
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("MyImplementSwipeHelper at add Category");
                        Toast.MakeText(myImplementSwipeHelper.context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }
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