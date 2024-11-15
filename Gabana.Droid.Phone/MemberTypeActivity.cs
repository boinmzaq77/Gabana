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
using LinqToDB.SqlQuery;
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
    public class MemberTypeActivity : AppCompatActivity
    {
        public static MemberTypeActivity membertype;
        LinearLayout lnBack;
        EditText txtTypeName1, txtDiscount1, txtTypeName2, txtDiscount2, txtTypeName3, txtDiscount3;
        Button btnSave;
        static List<MemberType> lstmemberTypes = new List<MemberType>();
        CultureInfo culture = new CultureInfo("en-US");
        MemberTypeManage memberTypeManage = new MemberTypeManage();
        RecyclerView recyclerview_listMembertype;
        ImageButton addMembertype;
        SwipeRefreshLayout refreshlayout;
        LinearLayout lnNoMembertype;
        FrameLayout lnMembertype;
        List<ORM.Master.MemberType> listMemberType = new List<ORM.Master.MemberType>();
        DialogLoading dialogLoading = new DialogLoading();
        public static bool flagData = false , checkManinRole;
        private string LoginType;
        public static MemberType FocusMember;
        Membertype_Adapter_Main membertype_adapter;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.membertype_activity);
                membertype = this;

                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                txtTypeName1 = FindViewById<EditText>(Resource.Id.txtTypeName1);
                txtTypeName2 = FindViewById<EditText>(Resource.Id.txtTypeName2);
                txtTypeName3 = FindViewById<EditText>(Resource.Id.txtTypeName3);
                txtDiscount1 = FindViewById<EditText>(Resource.Id.txtDiscount1);
                txtDiscount2 = FindViewById<EditText>(Resource.Id.txtDiscount2);
                txtDiscount3 = FindViewById<EditText>(Resource.Id.txtDiscount3);
                btnSave = FindViewById<Button>(Resource.Id.btnSave);

                CheckJwt();
                lnBack.Click += LnBack_Click;

                lnNoMembertype = FindViewById<LinearLayout>(Resource.Id.lnNoMembertype);
                lnMembertype = FindViewById<FrameLayout>(Resource.Id.lnMembertype);

                addMembertype = FindViewById<ImageButton>(Resource.Id.addMembertype);
                addMembertype.Click += AddMembertype_Click;
                recyclerview_listMembertype = FindViewById<RecyclerView>(Resource.Id.recyclerview_listMembertype);  
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
                        flagData = true;
                        OnResume();
                    }
                    
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };

                LoginType = Preferences.Get("LoginType", "");
                checkManinRole = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "membertype");
                UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "membertype");
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                var w = mainDisplayInfo.Width;
                var Width = w / 5;
                MySwipeHelper mySwipe = new MyImplementSwipeHelper(this, recyclerview_listMembertype, (int)Width);
                flagData = true;

                _ = TinyInsights.TrackPageViewAsync("OnCreate : MemberTypeActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Membertype");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void AddMembertype_Click(object sender, EventArgs e)
        {
            addMembertype.Enabled = false;
            if (checkManinRole && lstmemberTypes.Count < 3)
            {
                StartActivity(new Intent(Application.Context, typeof(AddMembertypeActivity)));
                AddMembertypeActivity.SetMembertype(null);
                addMembertype.Enabled = true;
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                addMembertype.Enabled = true;
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

        async Task GetMemberType()
        {
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                if (await GabanaAPI.CheckSpeedConnection())
                {
                    listMemberType = await GabanaAPI.GetDataMemberType();
                    if (listMemberType == null)
                    {
                        dialogLoading.Dismiss();
                        return;
                    }
                    if (listMemberType.Count == 0)
                    {
                        lstmemberTypes = new List<MemberType>();
                    }
                    if (listMemberType.Count > 0)
                    {
                        //ลบข้อมูลทั้งหมดก่อน
                        var Allmember = await memberTypeManage.DeleteAllMemberType(DataCashingAll.MerchantId);

                        var lstmember = new List<MemberType>();
                        foreach (var item in listMemberType)
                        {
                            MemberType memberType = new MemberType()
                            {
                                DateModified = item.DateModified,
                                LinkProMaxxID = item.LinkProMaxxID,
                                MemberTypeName = item.MemberTypeName,
                                MemberTypeNo = item.MemberTypeNo,
                                MerchantID = item.MerchantID,
                                PercentDiscount = item.PercentDiscount
                            };
                            var InsertorReplace = await memberTypeManage.InsertorReplacrMemberType(memberType);
                            lstmember.Add(memberType);
                        }
                        lstmemberTypes = new List<MemberType>();
                        lstmemberTypes.AddRange(lstmember);
                        lstmemberTypes = lstmemberTypes.OrderBy(x => x.MemberTypeNo).ToList();
                    }
                }
                else
                {
                    lstmemberTypes = new List<MemberType>();
                    lstmemberTypes = await memberTypeManage.GetAllMemberType(DataCashingAll.MerchantId);
                }

                SetMembertype();

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

                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowMemberType at Membertype");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void SetMembertype()
        {
            try
            {
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerview_listMembertype.HasFixedSize = false;
                recyclerview_listMembertype.SetLayoutManager(mLayoutManager);
                membertype_adapter = new Membertype_Adapter_Main(lstmemberTypes);
                recyclerview_listMembertype.SetItemViewCacheSize(50);
                recyclerview_listMembertype.SetAdapter(membertype_adapter);
                membertype_adapter.ItemClick += Membertype_adapter_ItemClick;

                if (membertype_adapter.ItemCount == 0)
                {
                    lnNoMembertype.Visibility = ViewStates.Visible;
                    recyclerview_listMembertype.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoMembertype.Visibility = ViewStates.Gone;
                    recyclerview_listMembertype.Visibility = ViewStates.Visible;
                }
                if (membertype_adapter.ItemCount == 3 || !checkManinRole)
                {
                    addMembertype.SetBackgroundResource(Resource.Mipmap.AddMax);
                    addMembertype.Enabled = false;
                }
                else
                {
                    addMembertype.SetBackgroundResource(Resource.Mipmap.Add);
                    addMembertype.Enabled = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Membertype_adapter_ItemClick(object sender, int e)
        {
            if (checkManinRole)
            {
                var memberType = lstmemberTypes[e];
                StartActivity(new Intent(Application.Context, typeof(AddMembertypeActivity)));
                AddMembertypeActivity.SetMembertype(memberType);
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
            FocusMember = null;
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                if (flagData)
                {
                    await GetMemberType();
                    flagData = false;
                }
                MemberTypeFocus();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at Membertype");
            }
        }

        internal static void SetFocusMembertype(MemberType member)
        {
            try
            {
                FocusMember = member;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFocusMembertype at Membertype");
            }
        }

        private void MemberTypeFocus()
        {
            try
            {
                if (FocusMember != null)
                {
                    int index = -1;
                    if (lstmemberTypes != null)
                    {
                        if (lstmemberTypes.Count == 0)
                        {
                            lstmemberTypes.Add(FocusMember);
                            SetMembertype();
                            FocusMember = null;
                            return;
                        }
                        index = lstmemberTypes.FindIndex(x=>x.MemberTypeNo == FocusMember.MemberTypeNo);
                        if (index != -1)
                        {
                            lstmemberTypes.RemoveAt(index);
                        }
                        lstmemberTypes.Insert(0,FocusMember);
                    }
                    membertype_adapter.NotifyDataSetChanged();
                    FocusMember = null;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("MemberTypeFocus at Item");
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
                        var lstmemberType = new List<ORM.Master.MemberType>();
                        var MastermemberType = new ORM.Master.MemberType()
                        {
                            DateModified = lstmemberTypes[position].DateModified,
                            LinkProMaxxID = lstmemberTypes[position].LinkProMaxxID,
                            MemberTypeName = lstmemberTypes[position].MemberTypeName,
                            MemberTypeNo = (int)lstmemberTypes[position].MemberTypeNo,
                            MerchantID = (int)lstmemberTypes[position].MerchantID,
                            PercentDiscount = lstmemberTypes[position].PercentDiscount
                        };
                        lstmemberType.Add(MastermemberType);
                        var json = JsonConvert.SerializeObject(lstmemberType);

                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("membertype", "membertype");
                        bundle.PutString("membertypedata", json);
                        bundle.PutString("fromPage", "main");
                        dialog.Arguments = bundle;
                        dialog.Show(membertype.SupportFragmentManager, myMessage);
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("MyImplementSwipeHelper at Membertype");
                        Toast.MakeText(myImplementSwipeHelper.context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }
            }
        }
    }
}