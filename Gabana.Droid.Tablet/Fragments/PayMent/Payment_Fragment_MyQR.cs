using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.ViewPager.Widget;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using Gabana3.JAM.Trans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gabana.ORM.MerchantDB;
using Gabana.Droid.Tablet.Fragments.Setting;
using TinyInsightsLib;
using Gabana.Droid.Tablet.Adapter.Payment;
using Android.Graphics;
using Com.Bumptech.Glide.Load.Engine;
using ZXing;

namespace Gabana.Droid.Tablet.Fragments.PayMent
{
    public class Payment_Fragment_MyQR : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Payment_Fragment_MyQR NewInstance()
        {
            Payment_Fragment_MyQR frag = new Payment_Fragment_MyQR();
            return frag;
        }
        Payment_Fragment_MyQR fragment_myqr;
        View view;
        public static TranWithDetailsLocal tranWithDetails;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.payment_fragment_myqr, container, false);
            try
            {
                fragment_myqr = this;
                tranWithDetails = PaymentActivity.tranWithDetails;
                ComBineUI();                
                SetUIEvent();
                //OnResume();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
            return view;
        }
        public override async void OnResume() 
        {
            try
            {
                base.OnResume();

                if (!IsAdded)
                {
                    return;
                }

                //if (!IsVisible)
                //{
                //    return;
                //}

                CheckJwt();
                await SetDatamyQR();
                AddDotsIndicator(0);
                
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            Utils.ShowHi("Welcome");
        }

        List<MyQrCode> lstQrCodes;
        List<ORM.Master.MyQrCode> myqrcodes = new List<ORM.Master.MyQrCode>();
        MyQrCodeManage QrCodeManage = new MyQrCodeManage();
        Payment_Adapter_MyQR payment_adapter_myqr;
        async Task SetDatamyQR()
        {
            DialogLoading dialogLoading = new DialogLoading();
            dialogLoading.Cancelable = false;
            try
            {
                dialogLoading?.Show(PaymentActivity.payment_main.SupportFragmentManager, nameof(DialogLoading));
                lstQrCodes = new List<MyQrCode>();
                if (await GabanaAPI.CheckNetWork())
                {
                    List<MyQrCode> qrcodes = new List<MyQrCode>();
                    myqrcodes = await GabanaAPI.GetDataMyQrCode();
                    if (myqrcodes == null)
                    {
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

                ListMyQRCode = new ListMyQRCode(lstQrCodes);
                payment_adapter_myqr = new Payment_Adapter_MyQR(ListMyQRCode, this.Activity, MainActivity.CheckNet);
                viewPagerImgQRCode.Adapter = payment_adapter_myqr;

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
                _ = TinyInsights.TrackPageViewAsync("SetDatamyQR at MyQRActivity");
                Toast.MakeText(this.Activity, "error SetDatamyQR" + ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetUIEvent()
        {
            viewPagerImgQRCode.PageSelected += ViewPagerImgQRCode_PageSelected;
            lnBack.Click += LnBack_Click;
            btnSave.Click += BtnSave_Click;
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            MainActivity.tranWithDetails = tranWithDetails;
            PaymentActivity.payment_main.LoadFragment(Resource.Id.lnMyQr, "payment", "myqrreceipt");
        }

        int positionNow = 0;

        private void ViewPagerImgQRCode_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            AddDotsIndicator(e.Position);
            showqr(lstQrCodes, e.Position);
            
            positionNow = e.Position; 
        }
        public void showqr(List<MyQrCode> lstQrCodes , int position) 
        {
            if (string.IsNullOrEmpty(lstQrCodes[position].PictureLocalPath)) return;
            Bitmap bitmap = null;
            bitmap = BitmapFactory.DecodeFile(lstQrCodes[position].PictureLocalPath);
            var reader = new BarcodeReader();
            var result2 = reader.Decode(bitmap);
            if (result2 != null)
            {
                string qrstring = result2.Text;
                //test qrstring to bitmap
                Utils.Getqrcodetodisplay2(qrstring);
                //vh.bitmapimgrQRCode.Visibility = ViewStates.Gone; ไม่แสดงที่แอป
            }
            else
            {
                Utils.ShowHi("Welcome");
            }
        } 
        private void LnBack_Click(object sender, EventArgs e)
        {
            MainActivity.tranWithDetails = tranWithDetails;
            PaymentActivity.payment_main.LoadFragment(Resource.Id.lnMyQr, "payment", "default");
        }
        public static TextView[] _dots { get; set; }
        ListMyQRCode ListMyQRCode;
        public void AddDotsIndicator(int pos)
        {
            _dots = new TextView[ListMyQRCode.Count];
            lnindicator.RemoveAllViews();
            for (int i = 0; i < _dots.Length; i++)
            {
                _dots[i] = new TextView(this.Activity);
                _dots[i].Text = ".";
                _dots[i].TextSize = 50;
                lnindicator.AddView(_dots[i]);
            }
            if (_dots.Length > 0)
                _dots[pos].SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null)); //change indicator color on selected page
        }

        LinearLayout lnBack;
        ViewPager viewPagerImgQRCode;
        LinearLayout lnindicator;
        LinearLayout lnNomyQR, lnMyqr;
        Button btnSave;
        private void ComBineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            viewPagerImgQRCode = view.FindViewById<ViewPager>(Resource.Id.viewPagerImgQRCode);
            lnindicator = view.FindViewById<LinearLayout>(Resource.Id.lnindicator);
            lnNomyQR = view.FindViewById<LinearLayout>(Resource.Id.lnNomyQR);
            lnMyqr = view.FindViewById<LinearLayout>(Resource.Id.lnMyqr);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);

        }

        async void CheckJwt()
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

        internal async void showqrclick()
        {
            await SetDatamyQR();
             showqr(lstQrCodes, 0);
        }
    }
}