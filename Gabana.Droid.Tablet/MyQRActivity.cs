using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.AppCompat.App;
using Gabana.ORM.MerchantDB;
using Gabana.Droid.Tablet.Fragments.Setting;
using TinyInsightsLib;
using System.Threading.Tasks;
using Gabana.Droid.Tablet.Adapter.More;
using AndroidX.RecyclerView.Widget;

namespace Gabana.Droid.Tablet
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class myQRActivity : AppCompatActivity
    {
        public static myQRActivity myqrActivity;
        LinearLayout lnNomyQR, lnMyqr;
        public static string branchID;
        ListMyQRCode ListMyQRCode;
        List<MyQrCode> lstQrCodes;
        MyQrCodeManage QrCodeManage = new MyQrCodeManage();
        MyQR_Adapter_Main myqr_adapter_main;
        int positionNow = 0;
        DialogLoading dialogLoading = new DialogLoading();
        List<ORM.Master.MyQrCode> myqrcodes = new List<ORM.Master.MyQrCode>();
        public static bool checkNet = false;
        RecyclerView rcvMyQR;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.myqr_activity_main);
                myqrActivity = this;
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                lnNomyQR = FindViewById<LinearLayout>(Resource.Id.lnNomyQR);
                lnMyqr = FindViewById<LinearLayout>(Resource.Id.lnMyqr);
                rcvMyQR = FindViewById<RecyclerView>(Resource.Id.rcvMyQR);
                lnBack.Click += LnBack_Click;
                btnBack.Click += LnBack_Click;

                _ = TinyInsights.TrackPageViewAsync("OnCreate : myQRActivity");

            }
            catch (Exception ex)
            {
                _ =  TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at myQR");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
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
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
                    return;
                }
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                checkNet = await GabanaAPI.CheckNetWork();

                await SetDatamyQR();

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
                myqr_adapter_main = new MyQR_Adapter_Main(ListMyQRCode, checkNet);
                LinearLayoutManager layoutManager = new LinearLayoutManager(this, 0 ,false);
                rcvMyQR.SetLayoutManager(layoutManager);
                rcvMyQR.SetAdapter(myqr_adapter_main);
                rcvMyQR.HasFixedSize = true;

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


        public override void OnUserInteraction()
        {
            base.OnUserInteraction();
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                PinCodeActitvity.SetPincode("Pincode");
            }
        }
    }

}