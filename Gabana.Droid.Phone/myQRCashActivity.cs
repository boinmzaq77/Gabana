
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

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class myQRCashActivity : AppCompatActivity
    {
        public static myQRCashActivity myqrActivity;
        LinearLayout lnBack;
        TransManage transManage = new TransManage();
        public static TranWithDetailsLocal tranWithDetails;
        public static TranPayment tranPayment = new TranPayment();       

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
        List<ORM.Master.MyQrCode> myqrcodes = new List<ORM.Master.MyQrCode>();
        public static bool checkNet = false;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                SetContentView(Resource.Layout.myqrcash_activity);
                myqrActivity = this;

                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                Button btnSave = FindViewById<Button>(Resource.Id.btnSave);

                lnNomyQR = FindViewById<LinearLayout>(Resource.Id.lnNomyQR);
                lnMyqr = FindViewById<LinearLayout>(Resource.Id.lnMyqr);
                lnindicator = FindViewById<LinearLayout>(Resource.Id.lnindicator);
                viewPagerImgQRCode = FindViewById<ViewPager>(Resource.Id.viewPagerImgQRCode);
                viewPagerImgQRCode.PageSelected += ViewPagerImgQRCode_PageSelected;
                lnBack.Click += LnBack_Click; ;
                btnSave.Click += BtnSave_Click;

                CheckJwt();
                checkNet = await GabanaAPI.CheckSpeedConnection();

                await SetDatamyQR();
                AddDotsIndicator(0);
                _ = TinyInsights.TrackPageViewAsync("OnCreate : myQRCashActivity");

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Oncreate at myQRCash");
            }
        }

        async Task SetDatamyQR()
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                lstQrCodes = new List<MyQrCode>();
                if (checkNet)
                {
                    List<MyQrCode> qrcodes = new List<MyQrCode>();
                    myqrcodes = await GabanaAPI.GetDataMyQrCode();
                    if (myqrcodes == null)
                    {
                        if (dialogLoading != null)
                        {
                            dialogLoading.DismissAllowingStateLoss();
                            dialogLoading.Dismiss();
                        }
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

                        var lst = myqrcodes.OrderBy(x => x.MyQrCodeNo).ToList();
                        foreach (var item in lst)
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

                lisQRAllBranch = lstQrCodes.Where(x => x.FMyQrAllBranch == 'A').ToList();
                lisQRAllBranch = lisQRAllBranch.OrderBy(x => x.MyQrCodeNo).ToList();
                lisQRThisBranch = lstQrCodes.Where(x => x.SysBranchID == DataCashingAll.SysBranchId).ToList();
                lisQRThisBranch = lisQRThisBranch.OrderBy(x => x.MyQrCodeNo).ToList();

                lstQrCodes = new List<MyQrCode>();
                lstQrCodes.AddRange(lisQRThisBranch);
                lstQrCodes.AddRange(lisQRAllBranch);


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
                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetDatamyQR at myQRCash");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();

                lstQrCodes = new List<MyQrCode>();
                ListMyQRCode = new ListMyQRCode(lstQrCodes);
                return;
            }
        }

        public void AddDotsIndicator(int pos)
        {
            try
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
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("AddDotsIndicator at myQRCash");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private void ViewPagerImgQRCode_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            AddDotsIndicator(e.Position);
            positionNow = e.Position;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(myQRReceiptActivity)));
            myQRReceiptActivity.SetTranDetail(tranWithDetails);
            this.Finish();
        }

        private void BtnCustomer_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(SelectCustomerActivity)));
        }


        private void LnBack_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(PaymentActivity)));
            PaymentActivity.SetTranDetail(tranWithDetails);
            this.Finish();
        }
        public override void OnBackPressed()
        {
            //base.OnBackPressed();
            lnBack.PerformClick();
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'myQRCashActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'myQRCashActivity.openPage' is assigned but its value is never used
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