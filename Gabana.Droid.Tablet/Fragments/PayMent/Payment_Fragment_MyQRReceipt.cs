using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide.Load.Engine;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Fragments.PayMent
{
    public class Payment_Fragment_MyQRReceipt : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Payment_Fragment_MyQRReceipt NewInstance()
        {
            Payment_Fragment_MyQRReceipt frag = new Payment_Fragment_MyQRReceipt();
            return frag;
        }
        public static Payment_Fragment_MyQRReceipt fragment_myqrreceipt;
        View view;
        internal static Android.Net.Uri keepCropedUri = null;

        public static TranWithDetailsLocal tranWithDetails;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.payment_fragment_myqrreceipt, container, false);
            try
            {
                fragment_myqrreceipt = this;
                tranWithDetails = PaymentActivity.tranWithDetails;
                pathFolderPicture = DataCashingAll.PathFolderImage;
                ComBineUI();
                SetUIEvent();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
            return view;
        }
        LinearLayout lnBack;
        LinearLayout lnAddimage;
        EditText txtReceiptName, txtComment;
        TextView textNamePic;
        public static  ImageView imageViewShowReciept;
        Button btnSave;
        private void ComBineUI()
        {
            try
            {
                lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnAddimage = view.FindViewById<LinearLayout>(Resource.Id.lnAddimage);
                txtReceiptName = view.FindViewById<EditText>(Resource.Id.txtReceiptName);
                txtComment = view.FindViewById<EditText>(Resource.Id.txtComment);
                textNamePic = view.FindViewById<TextView>(Resource.Id.textNamePic);
                imageViewShowReciept = view.FindViewById<ImageView>(Resource.Id.imageViewShowReciept);
                btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }


        private void SetUIEvent()
        {
            lnBack.Click += LnBack_Click;
            lnAddimage.Click += LnAddimage_Click; 
            btnSave.Click += BtnSave_Click;

            txtReceiptName.Text = "Receipt-" + DateTime.Now.ToString("HH:mm");
            txtComment.Text = "จ่ายแล้ว";

        }
        public static TranPayment tranPayment = new TranPayment();
        int PaymentNo;
        private void initialData()
        {
            if (tranWithDetails == null)
            {
                return;
            }

            tranPayment = new TranPayment()
            {
                MerchantID = DataCashingAll.MerchantId,
                SysBranchID = DataCashingAll.SysBranchId,
                TranNo = tranWithDetails.tran.TranNo,
                PaymentNo = PaymentNo,
                PaymentType = DataCashing.PaymentType,
                PaymentAmount = (decimal)0, //เงินที่ต้องจ่าย
                CreditCardType = null,
                CardNo = null,
                ExprieDateYYYYMM = null,
                ApproveCode = null,
                TotalRedeemPoint = null,
                //EPaymentType = "QR",
                RequestNum = null,
                RequestDateTime = null,
                FEPaymentCancel = 0,
                ReferenceNo1 = null,
                ReferenceNo2 = null,
                ReferenceNo3 = null,
                ReferenceNo4 = null,
                Comments = null,
            };
        }
        string CURRENCYSYMBOLS, path, pathFolderPicture;
        Android.Graphics.Bitmap bitmap;

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave.Enabled = false;
                initialData();
                int PaymentNo = tranWithDetails.tranPayments.Count();
                PaymentNo++;

                decimal paymentAmount = 0;
                foreach (var item in tranWithDetails.tranPayments)
                {
                    paymentAmount += item.PaymentAmount;
                }

                decimal amount = tranWithDetails.tran.GrandTotal - paymentAmount;

                tranPayment.PaymentNo = PaymentNo;
                tranPayment.PaymentAmount = amount; //เงินที่จ่าย
                tranPayment.Comments = txtReceiptName.Text;

                if (keepCropedUri != null)
                {
                    path = Utils.SplitPath(keepCropedUri.ToString());
                    var checkResultPicture = await Utils.InsertImageToPicture(path, bitmap);
                    tranPayment.PicturePath = pathFolderPicture + path;
                }
                else
                {
                    tranPayment.PicturePath = null;
                }

                tranWithDetails.tranPayments.Add(tranPayment);
                PaymentActivity.payment_main.LoadFragment(Resource.Id.btnChange, "balance", "receipt");
                DataCashing.flagChooseMedia = false;
            }
            catch (Exception ex)
            {
                btnSave.Enabled = true;
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
                return;
            }
        }

        private void LnAddimage_Click(object sender, EventArgs e)
        {
            try
            {
                DataCashing.flagChooseMedia = true;
                var fragment = new MyQRReceipt_Dialog_ChooseMedia();
                fragment.Show(this.Activity.SupportFragmentManager, nameof(MyQRReceipt_Dialog_ChooseMedia));
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            PaymentActivity.payment_main.LoadFragment(Resource.Id.lnMyQr, "payment", "myqr");
        }

        private void SetImgPayment()
        {
            try
            {
                if (keepCropedUri != null)
                {
                    //Clear รูปภาพก่อนทำอะไรใหม่
                    string setpathnull = string.Empty;
                    Android.Net.Uri urisetpathnull = Android.Net.Uri.Parse(setpathnull);
                    imageViewShowReciept.SetImageURI(urisetpathnull);

                    Android.Net.Uri cropImageURI = keepCropedUri;
                    bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);
                    imageViewShowReciept.SetImageBitmap(bitmap);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public override void OnResume()
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
                pathFolderPicture = DataCashingAll.PathFolderImage;
                //เพิ่ม flag สำหรับตรวจจับว่ามีการกดเลือกรูปหรือไม่ เพราะ ตอนนี้จะเข้า Onresume ตลอดทำให้ข้อมูลที่เคยกรอกไว้หายไป
                if (DataCashing.flagChooseMedia)
                {
                    SetImgPayment();
                    return;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
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