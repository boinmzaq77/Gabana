using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.ViewPager.Widget;
using Gabana.Droid.Tablet.Adapter.Setting;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using static Android.Provider.Telephony.Mms;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_MyQR : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static Setting_Fragment_MyQR NewInstance()
        {
            Setting_Fragment_MyQR frag = new Setting_Fragment_MyQR();
            return frag;
        }

        View view;
        public static Setting_Fragment_MyQR fragment_myqr;
        int positionNow = 0;
        public static TextView[] _dots { get; set; }
        List<ORM.Master.MyQrCode> myqrcodes = new List<ORM.Master.MyQrCode>();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_myqr, container, false);
            try
            {
                fragment_myqr = this;
                CheckJwt();
                CombineUI();
                return view;
            }
            catch (Exception)
            {
                return view;
            }
        }
        LinearLayout lnBack, lnindicator, lnNomyQR, lnMyqr;
        ViewPager viewPagerImgQRCode;
        MyQrCodeManage QrCodeManage = new MyQrCodeManage();
        List<MyQrCode> lstQrCodes = new List<MyQrCode>();
        ImageButton addQR;
        Setting_Adapter_MyQR setting_adapter_myqr;

        async Task SetDatamyQR()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {               
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }
                lstQrCodes = new List<MyQrCode>();
                if (DataCashing.CheckNet)
                {
                    List<MyQrCode> qrcodes = new List<MyQrCode>();
                    myqrcodes = await GabanaAPI.GetDataMyQrCode();
                    if (myqrcodes == null)
                    {
                        lstQrCodes = new List<MyQrCode>();
                    }
                    if (myqrcodes.Count == 0)
                    {
                        lstQrCodes = new List<MyQrCode>();
                    }
                    if (myqrcodes.Count > 0)
                    {
                        //ลบข้อมูลทังหมดก่อน
                        var data = await QrCodeManage.GetAllMyQrCode(DataCashingAll.MerchantId);
                        if (data != null)
                        {
                            foreach (var item in data)
                            {
                                if (System.IO.File.Exists(item.PictureLocalPath))
                                {
                                    System.IO.File.Delete(item.PictureLocalPath);
                                }
                            }
                        }

                        var AllQR = await QrCodeManage.DeleteAllMyQrCode(DataCashingAll.MerchantId);
                        foreach (var item in myqrcodes)
                        {
                            ORM.MerchantDB.MyQrCode myQrCode = new ORM.MerchantDB.MyQrCode()
                            {
                                DateCreated = item.DateCreated,
                                DateModified = item.DateModified,
                                MerchantID = item.MerchantID,
                                Ordinary = item.Ordinary,
                                UserNameModified = item.UserNameModified,
                                Comments = item.Comments,
                                FMyQrAllBranch = item.FMyQrAllBranch,
                                MyQrCodeName = item.MyQrCodeName,
                                MyQrCodeNo = item.MyQrCodeNo,
                                PicturePath = item.PicturePath,
                                PictureLocalPath = item.PicturePath,
                                SysBranchID = item.SysBranchID  // FMyQrAllBranch = 'A' : null,FMyQrAllBranch = 'B' : DataCashingAll.SysBranchId 
                            };
                            await QrCodeManage.InsertOrReplaceMyQrCode(myQrCode);
                            await Utils.InsertLocalPictureMyQRCode(myQrCode);
                            qrcodes.Add(myQrCode);
                        }
                        lstQrCodes = new List<MyQrCode>();
                        lstQrCodes.AddRange(qrcodes);
                    }
                }
                else
                {
                    lstQrCodes = await QrCodeManage.GetAllMyQrCode(DataCashingAll.MerchantId);
                    if (lstQrCodes == null)
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                        lstQrCodes = new List<MyQrCode>();
                    }
                }

                List<MyQrCode> lisQRAllBranch = new List<MyQrCode>();
                List<MyQrCode> lisQRThisBranch = new List<MyQrCode>();
                List<MyQrCode> lisQRAnotherBranch = new List<MyQrCode>();

                lisQRAllBranch = lstQrCodes.Where(x => x.FMyQrAllBranch == 'A').ToList();
                lisQRAllBranch = lisQRAllBranch.OrderBy(x => x.MyQrCodeNo).ToList();
                lisQRThisBranch = lstQrCodes.Where(x => x.SysBranchID == DataCashingAll.SysBranchId).ToList();
                lisQRThisBranch = lisQRThisBranch.OrderBy(x => x.MyQrCodeNo).ToList();
                lisQRAnotherBranch = lstQrCodes.Where(x => x.SysBranchID != DataCashingAll.SysBranchId && x.FMyQrAllBranch != 'A').ToList();
                lisQRAnotherBranch = lisQRAnotherBranch.OrderBy(x => x.MyQrCodeNo).ToList();

                lstQrCodes = new List<MyQrCode>();
                lstQrCodes.AddRange(lisQRThisBranch);
                lstQrCodes.AddRange(lisQRAllBranch);
                lstQrCodes.AddRange(lisQRAnotherBranch);

                if (lstQrCodes.Count == 0)
                {
                    lnNomyQR.Visibility = ViewStates.Visible;
                    lnMyqr.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNomyQR.Visibility = ViewStates.Gone;
                    lnMyqr.Visibility = ViewStates.Visible;
                }

                if (lstQrCodes.Count == 0)
                {
                    lnNomyQR.Visibility = ViewStates.Visible;
                    lnMyqr.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNomyQR.Visibility = ViewStates.Gone;
                    lnMyqr.Visibility = ViewStates.Visible;
                }

                ListMyQRCode = new ListMyQRCode(lstQrCodes);

                setting_adapter_myqr = new Setting_Adapter_MyQR(ListMyQRCode, MainActivity.main_activity, DataCashing.CheckNet);
                viewPagerImgQRCode.Adapter = setting_adapter_myqr;
                setting_adapter_myqr.ItemClick += Setting_adapter_myqr_ItemClick;

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetDatamyQR at SettingQR");
                Toast.MakeText(this.Activity, "error SetDatamyQR" + ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void Setting_adapter_myqr_ItemClick(object sender, int e)
        {
            try
            {
                if (lstQrCodes.Count > 0)
                {
                    MyQrCode qrCode = new MyQrCode();
                    qrCode = lstQrCodes[positionNow];
                    DataCashing.EditMyQR = qrCode;
                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addmyqr");
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("MyQrCode_Adapter_Main_ItemClick at SettingQR");                
                return;
            }
        }

        private async void AddQR_Click(object sender, EventArgs e)
        {
            DataCashing.EditMyQR = null;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addmyqr");
        }

        private void CombineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnindicator = view.FindViewById<LinearLayout>(Resource.Id.lnindicator);
            lnNomyQR = view.FindViewById<LinearLayout>(Resource.Id.lnNomyQR);
            lnMyqr = view.FindViewById<LinearLayout>(Resource.Id.lnMyqr);
            viewPagerImgQRCode = view.FindViewById<ViewPager>(Resource.Id.viewPagerImgQRCode);
            addQR = view.FindViewById<ImageButton>(Resource.Id.addQR);
            viewPagerImgQRCode.PageSelected += ViewPagerImgQRCode_PageSelected;
            lnBack.Click += LnBack_Click;
            addQR.Click += AddQR_Click;
        }        

        private void LnBack_Click(object sender, EventArgs e)
        {
            Focusitem = 0;
            positionNow = 0;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
        }

        private void ViewPagerImgQRCode_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            if (e.Position != -1)
            {
                AddDotsIndicator(e.Position);
                positionNow = e.Position;
            }
        }
        ListMyQRCode ListMyQRCode;
        private void AddDotsIndicator(int pos)
        {
            try
            {
                if (ListMyQRCode != null && ListMyQRCode.Count > 0)
                {
                    _dots = new TextView[ListMyQRCode.Count];
                    lnindicator.RemoveAllViews();
                    for (int i = 0; i < _dots.Length; i++)
                    {
                        _dots[i] = new TextView(this.Activity);
                        _dots[i].Text = ".";
                        _dots[i].TextSize = 80;
                        lnindicator.AddView(_dots[i]);
                    }
                    if (_dots.Length > 0)
                        _dots[pos].SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null)); //change indicator color on selected page
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("AddDotsIndicator at SettingQR");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public static long Focusitem;
        internal static void SetFocusQR(long id)
        {
            Focusitem = id;
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
                await SetDatamyQR();
                
                if (Focusitem == 0)
                {
                    positionNow = 0;
                    AddDotsIndicator(positionNow);
                }
                else
                {
                    if (!DataCashingAll.flagMyQrCodeChange)
                    {
                        return;
                    }

                    var index = lstQrCodes.FindIndex(x => x.MyQrCodeNo == (int)Focusitem);
                    if (index == -1)
                    {
                        positionNow = 0;
                        AddDotsIndicator(0);
                    }
                    else
                    {
                        positionNow = index;
                        AddDotsIndicator(index);
                        viewPagerImgQRCode.SetCurrentItem(index, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
    }
    public class ListMyQRCode
    {
        public List<MyQrCode> myQrCodes;
        static List<MyQrCode> builitem;
        public ListMyQRCode(List<MyQrCode> myQrCodes)
        {
            builitem = myQrCodes;
            this.myQrCodes = builitem;
        }
        public int Count
        {
            get
            {
                return myQrCodes == null ? 0 : myQrCodes.Count;
            }
        }
        public MyQrCode this[int i]
        {
            get { return myQrCodes == null ? null : myQrCodes[i]; }
        }
    }


}