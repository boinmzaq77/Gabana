using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class SettingmyQRActivity : AppCompatActivity
    {
        public static SettingmyQRActivity myqrActivity;
        LinearLayout lnNomyQR, lnMyqr;
        //RecyclerView recyclerview_listmyqr;
        ViewPager viewPagerImgQRCode;
        public static LinearLayout lnindicator;
        ListMyQRCode ListMyQRCode;
        List<MyQrCode> lstQrCodes;
        MyQrCodeManage QrCodeManage = new MyQrCodeManage();
        public static TextView[] _dots { get; set; }
        MyQrCode_Adapter_Main myQrCode_Adapter_Main;
        int positionNow = 0;
        public static long Focusitem;
        List<ORM.Master.MyQrCode> myqrcodes = new List<ORM.Master.MyQrCode>();
        DialogLoading dialogLoading = new DialogLoading();
        ImageButton addQR;
        public static bool checkNet = false;
        bool checkManinRole;
        private string LoginType;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.settingmyqr_activity_main);
                myqrActivity = this;
                CheckJwt();
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                ImageButton imagebtnBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                addQR = FindViewById<ImageButton>(Resource.Id.addGiftvoucher);
                lnNomyQR = FindViewById<LinearLayout>(Resource.Id.lnNomyQR);
                lnMyqr = FindViewById<LinearLayout>(Resource.Id.lnMyqr);
                lnindicator = FindViewById<LinearLayout>(Resource.Id.lnindicator);
                viewPagerImgQRCode = FindViewById<ViewPager>(Resource.Id.viewPagerImgQRCode);
                viewPagerImgQRCode.PageSelected += ViewPagerImgQRCode_PageSelected;
                addQR = FindViewById<ImageButton>(Resource.Id.addQR);
                lnBack.Click += LnBack_Click;
                imagebtnBack.Click += LnBack_Click;
                addQR.Click += AddQR_Click;
                DataCashingAll.flagMyQrCodeChange = true;
                LoginType = Preferences.Get("LoginType", "");
                checkManinRole = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "myqr");
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at SettingQR");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void ViewPagerImgQRCode_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            if (e.Position != -1)
            {
                AddDotsIndicator(e.Position);
                positionNow = e.Position;
            }
        }

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
                        _dots[i] = new TextView(this);
                        _dots[i].Text = ".";
                        _dots[i].TextSize = 50;
                        lnindicator.AddView(_dots[i]);
                    }
                    if (_dots.Length > 0)
                        _dots[pos].SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null)); //change indicator color on selected page
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("AddDotsIndicator at SettingQR");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void AddQR_Click(object sender, EventArgs e)
        {
            addQR.Enabled = false;
            if (checkManinRole)
            {
                StartActivity(new Intent(Application.Context, typeof(AddmyQRActivity)));
                AddmyQRActivity.SetMyQrCodeDetail(null);
                addQR.Enabled = true;
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                addQR.Enabled = true;
                return;
            }            
        }

        async Task SetDatamyQR()
        {
            try
            {               
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }
                lstQrCodes = new List<MyQrCode>();
                if (checkNet)
                {
                    List<MyQrCode> qrcodes = new List<MyQrCode>();
                    myqrcodes = await GabanaAPI.GetDataMyQrCode();
                    if (myqrcodes == null)
                    {
                        dialogLoading.Dismiss();
                        return;
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
                        Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                        lstQrCodes = new List<MyQrCode>();
                    }
                }
                List<MyQrCode> lisQRAllBranch = new List<MyQrCode>();
                List<MyQrCode> lisQRThisBranch = new List<MyQrCode>();
                List<MyQrCode> lisQRAnotherBranch = new List<MyQrCode>();

                lisQRAllBranch = lstQrCodes.Where(x => x.FMyQrAllBranch == 'A').OrderBy(x => x.MyQrCodeNo).ToList();
                lisQRThisBranch = lstQrCodes.Where(x => x.SysBranchID == DataCashingAll.SysBranchId).OrderBy(x => x.MyQrCodeNo).ToList();
                lisQRAnotherBranch = lstQrCodes.Where(x => x.SysBranchID != DataCashingAll.SysBranchId && x.FMyQrAllBranch != 'A')
                                     .OrderBy(x => x.MyQrCodeNo).ToList();

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

                ListMyQRCode = new ListMyQRCode(lstQrCodes);
                myQrCode_Adapter_Main = new MyQrCode_Adapter_Main(ListMyQRCode, myqrActivity, checkNet);
                viewPagerImgQRCode.Adapter = myQrCode_Adapter_Main;
                myQrCode_Adapter_Main.ItemClick += MyQrCode_Adapter_Main_ItemClick;

                if (!checkManinRole)
                {
                    addQR.SetBackgroundResource(Resource.Mipmap.AddMax);
                    addQR.Enabled = false;
                }
                else
                {
                    addQR.SetBackgroundResource(Resource.Mipmap.Add);
                    addQR.Enabled = true;
                }

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                addQR.Enabled = true;
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetDatamyQR at SettingQR");
                Toast.MakeText(this, "error SetDatamyQR" + ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void MyQrCode_Adapter_Main_ItemClick(object sender, int e)
        {
            try
            {
                if (checkManinRole)
                {
                    if (lstQrCodes.Count > 0)
                    {
                        MyQrCode qrCode = new MyQrCode();
                        qrCode = lstQrCodes[positionNow];
                        StartActivity(new Intent(Application.Context, typeof(AddmyQRActivity)));
                        AddmyQRActivity.SetMyQrCodeDetail(qrCode);
                    }
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
                _ = TinyInsights.TrackPageViewAsync("MyQrCode_Adapter_Main_ItemClick at SettingQR");
                return;
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Focusitem = 0;
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
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }
                
                checkNet = await GabanaAPI.CheckSpeedConnection();

                if (DataCashingAll.flagMyQrCodeChange)
                {
                    await SetDatamyQR();
                    DataCashingAll.flagMyQrCodeChange = false;
                }

                if (Focusitem == 0)
                {
                    positionNow = 0;
                    AddDotsIndicator(positionNow);
                }
                else
                {
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

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at SettingQR");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                dialogLoading.Dismiss();
            }
        }

        public void Resume()
        {
            OnResume();
        }

        internal static void SetFocusQR(long id)
        {
            Focusitem = id;
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'SettingmyQRActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'SettingmyQRActivity.openPage' is assigned but its value is never used
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

