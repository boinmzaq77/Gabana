using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Linq;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class CreditReceiptActivity : AppCompatActivity
    {
        TextView TextViewCardType, TextViewCardNo;
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

                TextViewCardNo = FindViewById<TextView>(Resource.Id.CardNo);
                TextViewCardType = FindViewById<TextView>(Resource.Id.CardType);
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
                CheckJwt();
                SetShowdetail();
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;

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
                _ = TinyInsights.TrackPageViewAsync("OnCreate : CreditReceiptActivity");

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

            if (CardType == "Credit Card" || CardType == "บัตรเครดิต")
            {
                TextViewCardType.Text = GetString(Resource.String.creditdebit_activity_cardtype);
                TextViewCardNo.Text = GetString(Resource.String.creditdebit_activity_cardno);
            }
            if (CardType == "Debit Card" || CardType == "บัตรเดบิต")
            {
                TextViewCardType.Text = GetString(Resource.String.debit_activity_cardtype);
                TextViewCardNo.Text = GetString(Resource.String.debit_activity_cardno);
            }
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
                btnSave.Enabled = false;
                if (CardType == "Credit Card" || CardType == "บัตรเครดิต")
                {
                    DataCashing.PaymentType = "CR";
                }
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
                        Toast.MakeText(this, GetString(Resource.String.cardnonotcomplete), ToastLength.Short).Show();
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
                StartActivity(new Intent(Application.Context, typeof(BalanceActivity)));
                BalanceActivity.SetTranDetail(tranWithDetails);
                btnSave.Enabled = true;
                this.Finish();
            }
            catch (Exception ex)
            {
                btnSave.Enabled = true;
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

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'CreditReceiptActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'CreditReceiptActivity.openPage' is assigned but its value is never used
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

