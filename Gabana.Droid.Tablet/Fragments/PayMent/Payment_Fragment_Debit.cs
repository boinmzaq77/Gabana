using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Fragments.PayMent
{
    public class Payment_Fragment_Debit : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Payment_Fragment_Debit NewInstance()
        {
            Payment_Fragment_Debit frag = new Payment_Fragment_Debit();
            return frag;
        }
        View view;
        public static TranWithDetailsLocal tranWithDetails;
        public static Payment_Fragment_Debit fragment_debit;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.payment_fragment_debit, container, false);
            try
            {
                fragment_debit = this;
                tranWithDetails = MainActivity.tranWithDetails;
                ComBineUI();
                SetUIEvent();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }

            return view;
        }
        private string CURRENCYSYMBOLS;

        private void SetUIEvent()
        {
            btnSave.Click += BtnSave_Click;
            txtCreditCardNo.TextChanged += TxtCreditCardNo_TextChanged;
            lnShowDetail.Click += LnShowDetail_Click;
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
        string CardNo = string.Empty;

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
                btnSave.Enabled = false;
                if (CardType == "Debit Card" || CardType == "บัตรเดบิต")
                {
                    DataCashing.PaymentType = "DR";
                }

                if (CreditType == "Master Card")
                {
                    CreditType = "MASTER";
                }

                initialData();
                string cardno = "";
                if (!string.IsNullOrEmpty(txtCreditCardNo.Text))
                {
                    cardno = txtCreditCardNo.Text.Replace("-", "");
                    if (cardno.Length < 16)
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.cardnonotcomplete), ToastLength.Short).Show();
                        btnSave.Enabled = true;
                        return;
                    }
                }
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
                tranPayment.CardNo = cardno;
                tranPayment.CreditCardType = CreditType;
                tranPayment.PaymentType = DataCashing.PaymentType;
                tranWithDetails.tranPayments.Add(tranPayment);

                PaymentActivity.tranWithDetails = tranWithDetails;
                PaymentActivity.payment_main.LoadFragment(Resource.Id.btnChange, "balance", "receipt");
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                btnSave.Enabled = true;
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
                return;
            }
        }
        private int PaymentNo;

        public static TranPayment tranPayment = new TranPayment();

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

        private string CreditType;
        private string CardType;

        private void SetBtnPay()
        {
            try
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
                btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        private void spnCreditType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            CreditType = spnCreditType.SelectedItem?.ToString();
        }
        ImageButton btnShowDetail;
        FrameLayout lnShowDetail;
        LinearLayout lnDetails;
        Spinner spnCreditType;
        ImageButton btnCreditType;
        EditText txtCreditCardNo;
        Button btnSave;

        private void ComBineUI()
        {
            //spnCardType = view.FindViewById<Spinner>(Resource.Id.spnCardType);
            //btnCardType = view.FindViewById<ImageButton>(Resource.Id.btnCardType);
            btnShowDetail = view.FindViewById<ImageButton>(Resource.Id.btnShowDetail);
            lnShowDetail = view.FindViewById<FrameLayout>(Resource.Id.lnShowDetail);
            lnDetails = view.FindViewById<LinearLayout>(Resource.Id.lnDetails);
            spnCreditType = view.FindViewById<Spinner>(Resource.Id.spnCreditType);
            btnCreditType = view.FindViewById<ImageButton>(Resource.Id.btnCreditType);
            txtCreditCardNo = view.FindViewById<EditText>(Resource.Id.txtCreditCardNo);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
        }
        private bool showdetail;
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
                showdetail = false;
                SetShowdetail();
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;

                spnCreditType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spnCreditType_ItemSelected);
                var AdapterCreditType = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.spinCreditType, Resource.Layout.spinner_item);
                AdapterCreditType.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnCreditType.Adapter = AdapterCreditType;

                SetBtnPay();
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