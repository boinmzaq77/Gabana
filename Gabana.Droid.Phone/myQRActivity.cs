using Android.App;
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

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class myQRActivity : AppCompatActivity
    {
        public static myQRActivity myqrActivity;
        LinearLayout lnNomyQR, lnMyqr;
        public static LinearLayout lnindicator;
        ViewPager viewPagerImgQRCode;
        public static string branchID;
        ListMyQRCode ListMyQRCode;
        List<MyQrCode> lstQrCodes;
        MyQrCodeManage QrCodeManage = new MyQrCodeManage();
        MyQrCode_Adapter_Main myQrCode_Adapter_Main;
        public static TextView[] _dots { get; set; }
        int positionNow = 0;
        DialogLoading dialogLoading = new DialogLoading();
        List<ORM.Master.MyQrCode> myqrcodes = new List<ORM.Master.MyQrCode>();
        public static bool checkNet = false;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.myqr_activity_main);
                myqrActivity = this;
                CheckJwt();
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                lnNomyQR = FindViewById<LinearLayout>(Resource.Id.lnNomyQR);
                lnMyqr = FindViewById<LinearLayout>(Resource.Id.lnMyqr);
                lnindicator = FindViewById<LinearLayout>(Resource.Id.lnindicator);
                viewPagerImgQRCode = FindViewById<ViewPager>(Resource.Id.viewPagerImgQRCode);
                viewPagerImgQRCode.PageSelected += ViewPagerImgQRCode_PageSelected;
                lnBack.Click += LnBack_Click;
                btnBack.Click += LnBack_Click;

                _ = TinyInsights.TrackPageViewAsync("OnCreate : myQRActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at myQR");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void ViewPagerImgQRCode_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            AddDotsIndicator(e.Position);
            positionNow = e.Position;
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
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

                await SetDatamyQR();
                AddDotsIndicator(0);

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
                _ = TinyInsights.TrackPageViewAsync("OnResume at MyQR");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                dialogLoading.Dismiss();
                base.OnRestart();
            }
        }

        public void Resume()
        {
            OnResume();
        }

        async Task SetDatamyQR()
        {
            try
            {
                //if (dialogLoading.Cancelable != false)
                //{
                //    dialogLoading.Cancelable = false;
                //    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                //}

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
                    }
                }

                #region lstsort
                //var lstsort = new List<MyQrCode>();
                //lstsort = lstQrCodes.Where(x => (x.SysBranchID == DataCashingAll.SysBranchId && x.FMyQrAllBranch == 'B') || x.FMyQrAllBranch == 'A').ToList().OrderBy(x => {
                //    var xx = 0;
                //    switch (x.FMyQrAllBranch)
                //    {
                //        case 'A':
                //            xx = 2;
                //            break;
                //        case 'B':
                //            xx = 1;
                //            break;
                //        default:
                //            xx = 0;
                //            break;
                //    }
                //    return xx;
                //}).ToList();
                //lstQrCodes = lstsort; 
                #endregion

                #region lstsort1 sort myqy ตามสาขาที่เข้าใช้งาน -> All Branch 
                //List<MyQrCode> lisQRAllBranch = new List<MyQrCode>();
                //List<MyQrCode> lisQRThisBranch = new List<MyQrCode>();

                //lisQRAllBranch = lstQrCodes.Where(x => x.FMyQrAllBranch == 'A').ToList();
                //lisQRAllBranch = lisQRAllBranch.OrderBy(x => x.MyQrCodeNo).ToList();
                //lisQRThisBranch = lstQrCodes.Where(x => x.SysBranchID == DataCashingAll.SysBranchId).ToList();
                //lisQRThisBranch = lisQRThisBranch.OrderBy(x => x.MyQrCodeNo).ToList();

                //lstQrCodes = new List<MyQrCode>();
                //lstQrCodes.AddRange(lisQRThisBranch);
                //lstQrCodes.AddRange(lisQRAllBranch); 
                #endregion 

                //sort myqy ตามสาขาที่เข้าใช้งาน -> All Branch -> สาขาอื่นๆ

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

                ListMyQRCode = new ListMyQRCode(lstQrCodes);
                myQrCode_Adapter_Main = new MyQrCode_Adapter_Main(ListMyQRCode, myqrActivity, checkNet);
                viewPagerImgQRCode.Adapter = myQrCode_Adapter_Main;

                //if (dialogLoading != null)
                //{
                //    dialogLoading.DismissAllowingStateLoss();
                //    dialogLoading.Dismiss();
                //}
            }
            catch (Exception ex)
            {
                //dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetDatamyQR at MyQRActivity");
                Toast.MakeText(this, "error SetDatamyQR" + ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public void AddDotsIndicator(int pos)
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
        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'myQRActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'myQRActivity.openPage' is assigned but its value is never used
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