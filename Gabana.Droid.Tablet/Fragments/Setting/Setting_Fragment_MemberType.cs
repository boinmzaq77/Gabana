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
using Gabana.Droid.Tablet.Helper;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
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

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_MemberType : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Setting_Fragment_MemberType NewInstance()
        {
            Setting_Fragment_MemberType frag = new Setting_Fragment_MemberType();
            return frag;
        }

        View view;
        public static Setting_Fragment_MemberType fragment_main;
        string LoginType;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_membertype, container, false);
            try
            {
                fragment_main = this;
                ComBineUI();
                var Width = 130;
                MySwipeHelper mySwipe = new MyImplementSwipeHelper(this.Activity, rcvMemberType, (int)Width);
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Branch");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }
        }
        TextView textTitle;
        LinearLayout lnBack, lnNoMembertype;
        FrameLayout lnMembertype;
        SwipeRefreshLayout refreshlayout;
        ImageButton addMembertype;
        RecyclerView rcvMemberType;
        private void ComBineUI()
        {
            textTitle = view.FindViewById<TextView>(Resource.Id.textTitle);
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            refreshlayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
            lnMembertype = view.FindViewById<FrameLayout>(Resource.Id.lnMembertype);
            lnNoMembertype = view.FindViewById<LinearLayout>(Resource.Id.lnNoMembertype);
            addMembertype = view.FindViewById<ImageButton>(Resource.Id.addMembertype);
            rcvMemberType = view.FindViewById<RecyclerView>(Resource.Id.rcvMemberType);
            lnBack.Click += LnBack_Click;

            addMembertype.Click += AddMembertype_Click;
            refreshlayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
            refreshlayout.Refresh += (sender, e) =>
            {
                OnResume();
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();
            };
        }

        private async void LnBack_Click(object sender, EventArgs e)
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
        }

        private async void AddMembertype_Click(object sender, EventArgs e)
        {
            DataCashing.EditMemberType = null;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addmembertype");
        }

        List<ORM.Master.MemberType> listMemberType = new List<ORM.Master.MemberType>();
        static List<MemberType> lstmemberTypes = new List<MemberType>();
        MemberTypeManage memberTypeManage = new MemberTypeManage();

        async Task ShowMemberType()
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

                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                rcvMemberType.HasFixedSize = true;
                rcvMemberType.SetLayoutManager(mLayoutManager);

                Setting_Adapter_MemberType membertype_adapter = new Setting_Adapter_MemberType(lstmemberTypes);
                rcvMemberType.SetItemViewCacheSize(50);
                rcvMemberType.SetAdapter(membertype_adapter);
                membertype_adapter.ItemClick += Membertype_adapter_ItemClick;

                if (membertype_adapter.ItemCount == 0)
                {
                    lnNoMembertype.Visibility = ViewStates.Visible;
                    rcvMemberType.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoMembertype.Visibility = ViewStates.Gone;
                    rcvMemberType.Visibility = ViewStates.Visible;
                }
                if (membertype_adapter.ItemCount == 3)
                {
                    addMembertype.SetBackgroundResource(Resource.Mipmap.AddMax);
                    addMembertype.Enabled = false;
                }
                else
                {
                    addMembertype.SetBackgroundResource(Resource.Mipmap.Add);
                    addMembertype.Enabled = true;
                }
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private async void Membertype_adapter_ItemClick(object sender, int e)
        {
            var memberType = lstmemberTypes[e];
            DataCashing.EditMemberType = memberType;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addmembertype");

        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
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

                _ = ShowMemberType();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity,ex.Message,ToastLength.Short).Show();
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
                        DataCashing.DeleteMemberType = lstmemberType;
                        var fragment = new MemberType_Dialog_Delete();
                        fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(MemberType_Dialog_Delete));
                        MemberType_Dialog_Delete.SetMembertypeDetail(lstmemberType);
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