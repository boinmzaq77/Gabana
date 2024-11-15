using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TinyInsightsLib;

namespace Gabana.Droid
{
        [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait ,Exported = false)]
    public class CreditRecieptActivity : AppCompatActivity
    {

        DialogLoading dialogLoading = new DialogLoading();
        LinearLayout lnBack;
        TransManage transManage = new TransManage();
        private static TranWithDetailsLocal tranWithDetails;
        public static TranPayment tranPayment = new TranPayment();
        Button btnSave;
        private int PaymentNo;
        EditText txtCreditCardNo;
        string CardNo = string.Empty;
        FrameLayout lnShowDetail;
        private bool showdetail;
        LinearLayout lnDetails;
        ImageButton btnShowDetail;
        Spinner spinnerCardType, spinnerCreditType;
        private string CreditType;
        private string CardType;
        private string CURRENCYSYMBOLS;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.creditdebitreceipt_activity);

                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                btnSave = FindViewById<Button>(Resource.Id.btnSave);
                txtCreditCardNo = FindViewById<EditText>(Resource.Id.txtCreditCardNo);
                lnBack.Click += LnBack_Click;
                btnSave.Click += BtnSave_Click;
                txtCreditCardNo.TextChanged += TxtCreditCardNo_TextChanged;
                btnShowDetail = FindViewById<ImageButton>(Resource.Id.btnShowDetail);
                lnDetails = FindViewById<LinearLayout>(Resource.Id.lnDetails);
                lnShowDetail = FindViewById<FrameLayout>(Resource.Id.lnShowDetail);
                lnShowDetail.Click += LnShowDetail_Click;
                showdetail = false;
                SetShowdetail();

                spinnerCardType = FindViewById<Spinner>(Resource.Id.spinnerCardType);
                spinnerCreditType = FindViewById<Spinner>(Resource.Id.spinnerCreditType);

                spinnerCardType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinnerCardType_ItemSelected);
                var AdapterCardType = ArrayAdapter.CreateFromResource(this, Resource.Array.spinCardType, Resource.Layout.spinner_item);
                AdapterCardType.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerCardType.Adapter = AdapterCardType;


                spinnerCreditType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinnerCreditType_ItemSelected);
                var AdapterCreditType = ArrayAdapter.CreateFromResource(this, Resource.Array.spinCreditType, Resource.Layout.spinner_item);
                AdapterCreditType.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerCreditType.Adapter = AdapterCreditType;

                SetBtnPay();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackPageViewAsync("PackageActivity");
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();               
                return;
            }           
        }

        private void SetBtnPay()
        {
            if (tranWithDetails == null)
            {
                return;
            }
            decimal paymentAmount = 0;
            foreach (var item in tranWithDetails.tranPayments)
            {
                paymentAmount += item.PaymentAmount;
            }
            decimal amount = tranWithDetails.tran.GrandTotal - paymentAmount;
            btnSave.Text = GetString(Resource.String.btnCharge) + " " + CURRENCYSYMBOLS + Utils.DisplayDecimal(Convert.ToDecimal(amount));
            btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            btnSave.SetBackgroundResource(Resource.Drawable.btnblue);
        }
        private void SpinnerCreditType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            CreditType = spinnerCreditType.SelectedItem?.ToString();
        }

        private void SpinnerCardType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            CardType = spinnerCardType.SelectedItem?.ToString();
        } 

        private void LnShowDetail_Click(object sender, EventArgs e)
        {
            if (showdetail)
            {
                showdetail = false;
            }
            else
            {
                showdetail = true;
            }
            SetShowdetail();
        }
        private void SetShowdetail()
        {
            if (showdetail)
            {
                lnDetails.Visibility = ViewStates.Visible;
                btnShowDetail.SetBackgroundResource(Resource.Mipmap.DetailShow);
            }
            else
            {
                lnDetails.Visibility = ViewStates.Gone;
                btnShowDetail.SetBackgroundResource(Resource.Mipmap.DetailNotShow);
            }
        }
        private void TxtCreditCardNo_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtCreditCardNo.Text))
                {
                    return;
                }

                CardNo = txtCreditCardNo.Text;
                int textlength = txtCreditCardNo.Text.Length;
                if (textlength == 5)
                {
                    var index = txtCreditCardNo.Text.LastIndexOf("-");
                    if (textlength == 5 & index == 4)
                    {
                        CardNo.Remove(4, 1);
                    }
                    else
                    {
                        txtCreditCardNo.Text = CardNo.Insert(CardNo.Length - 1, "-").ToString();
                        txtCreditCardNo.SetSelection(txtCreditCardNo.Text.Length);
                    }
                }
                else if (textlength == 10)
                {
                    var index = txtCreditCardNo.Text.LastIndexOf("-");
                    if (textlength == 9 & index == 8)
                    {
                        CardNo.Remove(9, 1);
                    }
                    else
                    {
                        txtCreditCardNo.Text = CardNo.Insert(CardNo.Length - 1, "-").ToString();
                        txtCreditCardNo.SetSelection(txtCreditCardNo.Text.Length);
                    }
                }
                else if (textlength == 15)
                {
                    var index = txtCreditCardNo.Text.LastIndexOf("-");
                    if (textlength == 14 & index == 12)
                    {
                        CardNo.Remove(14, 1);
                    }
                    else
                    {
                        txtCreditCardNo.Text = CardNo.Insert(CardNo.Length - 1, "-").ToString();
                        txtCreditCardNo.SetSelection(txtCreditCardNo.Text.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TxtCreditCardNo_TextChanged");
                return;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (CardType == "Credit Card")
                {
                    DataCashing.PaymentType = "CR";
                }
                if (CardType == "Debit Card")
                {
                    DataCashing.PaymentType = "DR";
                }

                initialData();
                string cardno = "";
                if (!string.IsNullOrEmpty(txtCreditCardNo.Text))
                {
                    cardno = txtCreditCardNo.Text.Replace("-", "");
                    if (cardno.Length< 16)
                    {
                        Toast.MakeText(this, GetString(Resource.String.cardnonotcomplete), ToastLength.Short).Show();
                        return;
                    }
                }
                PaymentNo = DataCashing.PaymentNo;
                PaymentNo++;
                decimal paymentAmount = 0;
                foreach (var item in tranWithDetails.tranPayments)
                {
                    paymentAmount += item.PaymentAmount;
                }
                decimal amount = tranWithDetails.tran.GrandTotal - paymentAmount;
                tranPayment.PaymentNo = PaymentNo;
                tranPayment.PaymentAmount = amount; //เงินที่จ่าย
                tranPayment.CardNo = cardno;
                tranPayment.CreditCardType = CreditType;
                tranPayment.PaymentType = DataCashing.PaymentType;
                tranWithDetails.tranPayments.Add(tranPayment);
                DataCashing.PaymentNo = PaymentNo;
                StartActivity(new Intent(Application.Context, typeof(BalanceActivity)));
                BalanceActivity.SetTranDetail(tranWithDetails);
                this.Finish();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                return;
            }
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
                //EPaymentType = null,
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
    }
}

