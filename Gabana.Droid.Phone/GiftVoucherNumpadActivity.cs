using Android.Accounts;
using Android.App;
using Android.OS;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class GiftVoucherNumpadActivity : Activity
    {
        GiftVoucherNumpadActivity giftVoucherNumpadActivity;
        ImageButton imagebtnBack, btndeletenumber;
        public static TranWithDetailsLocal TranWithDetails;
        public static TranDetailItemNew itemNew;
        static TextView txtAmoount;
        public static double newPrice;
        Button btnpoint, btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9, btnnumber0, btnClear, btnSave;
        static string strValue = Utils.DisplayDecimal(0);
        int count = 0;
        List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
        Gabana.ORM.Master.MerchantConfig merchantConfig = new ORM.Master.MerchantConfig();
        MerchantConfigManage configManage = new MerchantConfigManage();
        string DECIMALPOINTDISPLAY;
        static string txtCurrency;
        decimal dec;
        bool hasPercent = false;
        Button btnCurrency, btnPercent;
        public static  char typeDiscount, oldtypeDiscount;
        string currency;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.giftvoucher_activity_numpad);
                giftVoucherNumpadActivity = this;
                imagebtnBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                btndeletenumber = FindViewById<ImageButton>(Resource.Id.btndeletenumber);
                txtAmoount = FindViewById<TextView>(Resource.Id.txtServiceCharge);
                btnpoint = FindViewById<Button>(Resource.Id.btnpoint);
                btnnumber0 = FindViewById<Button>(Resource.Id.btnnumber0);
                btnnumber1 = FindViewById<Button>(Resource.Id.btnnumber1);
                btnnumber2 = FindViewById<Button>(Resource.Id.btnnumber2);
                btnnumber3 = FindViewById<Button>(Resource.Id.btnnumber3);
                btnnumber4 = FindViewById<Button>(Resource.Id.btnnumber4);
                btnnumber5 = FindViewById<Button>(Resource.Id.btnnumber5);
                btnnumber6 = FindViewById<Button>(Resource.Id.btnnumber6);
                btnnumber7 = FindViewById<Button>(Resource.Id.btnnumber7);
                btnnumber8 = FindViewById<Button>(Resource.Id.btnnumber8);
                btnnumber9 = FindViewById<Button>(Resource.Id.btnnumber9);
                btnSave = FindViewById<Button>(Resource.Id.btnSave);
                btnClear = FindViewById<Button>(Resource.Id.btnClear);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += ImagebtnBack_Click;

                btnCurrency = FindViewById<Button>(Resource.Id.btnCurrency);
                btnPercent = FindViewById<Button>(Resource.Id.btnPercent);

                imagebtnBack.Click += ImagebtnBack_Click;
                btndeletenumber.Click += Btndeletenumber_Click;
                btnpoint.Click += Btnpoint_Click;
                btnnumber0.Click += Btnnumber0_Click;
                btnnumber1.Click += Btnnumber1_Click;
                btnnumber2.Click += Btnnumber2_Click;
                btnnumber3.Click += Btnnumber3_Click;
                btnnumber4.Click += Btnnumber4_Click;
                btnnumber5.Click += Btnnumber5_Click;
                btnnumber6.Click += Btnnumber6_Click;
                btnnumber7.Click += Btnnumber7_Click;
                btnnumber8.Click += Btnnumber8_Click;
                btnnumber9.Click += Btnnumber9_Click;

                CheckJwt();
                DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY; //ทศนิยม
                txtCurrency = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                if (!string.IsNullOrEmpty(DECIMALPOINTDISPLAY))
                {
                    if (DECIMALPOINTDISPLAY == "4")
                    {
                        dec = (decimal)0.0001;
                    }
                    else
                    {
                        dec = (decimal)0.01;
                    }
                }
                else
                {
                    dec = (decimal)0.01;
                }

                typeDiscount = 'C';
                btnPercent.Click += BtnPercent_Click;
                btnCurrency.Click += BtnCurrency_Click;
                currency = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                if (string.IsNullOrEmpty(currency))
                {
                    currency = "฿";
                }
                btnCurrency.Text = currency;

                txtAmoount.Text = strValue;
                btnClear.Click += BtnClear_Click;
                btnSave.Click += BtnSave_Click;
                SetBtnClear();
                SetBtnSave();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : GiftVoucherNumpadActivity");

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void SetTypeBtnDiscount()
        {
            if (typeDiscount == 'C')
            {
                strValue = strValue;
                btnPercent.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                btnPercent.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnCurrency.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnCurrency.SetBackgroundResource(Resource.Drawable.btnblue);
            }
            else
            {
                strValue = strValue + "%";
                btnPercent.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnPercent.SetBackgroundResource(Resource.Drawable.btnblue);
                btnCurrency.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                btnCurrency.SetBackgroundResource(Resource.Drawable.btnborderblue);
            }
        }
        private void BtnCurrency_Click(object sender, EventArgs e)
        {
            typeDiscount = 'C';
            SetTypeBtnDiscount();
            SetBtnSave();
        }
        private void BtnPercent_Click(object sender, EventArgs e)
        {
            try
            {
                typeDiscount = 'P';
                SetTypeBtnDiscount();
                decimal maxValue = 100;
                string data;
                if (string.IsNullOrEmpty(strValue))
                {
                    strValue = "0";
                }
                var check = strValue.IndexOf('%');
                if (check == -1)
                {
                    data = strValue;
                }
                else
                {
                    data = strValue.Replace("%", "");
                }
                if (maxValue < Convert.ToDecimal(data))
                {
                    var checklenght = Utils.CheckLenghtValue(strValue);
                    Toast.MakeText(this, GetString(Resource.String.maxdiscount) +
                    Utils.DisplayDouble(maxValue) + "%", ToastLength.Short).Show();
                }
                SetTypeBtnDiscount();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnPercent_Click at Add Discount");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SetTypeBtnDiscount();
                AddGiftvoucherActivity.SetAmount(strValue);
                this.Finish();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("btnSave_Click at gift");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        internal static void SetAmount(string amount)
        {
            try
            {
                if (string.IsNullOrEmpty(amount))
                {
                    strValue = Utils.DisplayDecimal(0);
                }
                else
                {
                    if (amount.Contains("%"))
                    {
                        strValue = Utils.DisplayDecimal(Convert.ToDecimal(amount.Replace("%", ""))) + "%";
                        typeDiscount = 'P';

                    }
                    else if (amount.Contains(txtCurrency))
                    {
                        if (!string.IsNullOrEmpty(txtCurrency))
                        {
                            strValue = Utils.DisplayDecimal(Convert.ToDecimal(strValue.Replace(txtCurrency, "")));
                        }
                        else
                        {
                            strValue = Utils.DisplayDecimal(Convert.ToDecimal(strValue));
                        }
                    }
                    else
                    {
                        strValue = Utils.DisplayDecimal(Convert.ToDecimal(amount));
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetAmount at ServiceCharge");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void SetBtnClear()
        {
            if (txtAmoount.Text != "")
            {
                btndeletenumber.Enabled = true;
                btnClear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            else
            {
                btndeletenumber.Enabled = false;
                btnClear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.colorrule, null));
            }
        }
        private void SetBtnSave()
        {
            try
            {
                if (strValue.Contains("%"))
                {
                    strValue = Utils.DisplayDecimal(Convert.ToDecimal(strValue.Replace("%", "")));
                    hasPercent = true;
                }
                else if (strValue.Contains(txtCurrency))
                {
                    if (!string.IsNullOrEmpty(txtCurrency))
                    {
                        strValue = Utils.DisplayDecimal(Convert.ToDecimal(strValue.Replace(txtCurrency, "")));
                    }
                    else
                    {
                        strValue = Utils.DisplayDecimal(Convert.ToDecimal(strValue));
                    }
                    hasPercent = false;
                }
                else
                {
                    strValue = Utils.DisplayDecimal(Convert.ToDecimal(strValue));
                    hasPercent = false;
                }

                if (Convert.ToDecimal(strValue) > 0)
                {
                    btnSave.Enabled = true;
                    btnSave.SetBackgroundResource(Resource.Drawable.btnblue);
                    btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                }
                else
                {
                    btnSave.Enabled = true;
                    btnSave.SetBackgroundResource(Resource.Drawable.btnborderblue);
                    btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                }

                if (hasPercent)
                {
                    strValue = strValue + "%";
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetBtnSave at ServiceCharge");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtAmoount.Text = "";
            strValue = "0";
            txtAmoount.Hint = Utils.DisplayDecimal((decimal)newPrice);
            SetBtnClear();
            SetBtnSave();
        }
        private void Btnnumber9_Click(object sender, EventArgs e)
        {
            strValue = txtAmoount.Text;
            SetValue(btnnumber9);
        }
        private void Btnnumber8_Click(object sender, EventArgs e)
        {
            strValue = txtAmoount.Text;
            SetValue(btnnumber8);
        }
        private void Btnnumber7_Click(object sender, EventArgs e)
        {
            strValue = txtAmoount.Text;
            SetValue(btnnumber7);
        }
        private void Btnnumber6_Click(object sender, EventArgs e)
        {
            strValue = txtAmoount.Text;
            SetValue(btnnumber6);
        }
        private void Btnnumber5_Click(object sender, EventArgs e)
        {
            strValue = txtAmoount.Text;
            SetValue(btnnumber5);
        }
        private void Btnnumber4_Click(object sender, EventArgs e)
        {
            strValue = txtAmoount.Text;
            SetValue(btnnumber4);
        }
        private void Btnnumber3_Click(object sender, EventArgs e)
        {
            strValue = txtAmoount.Text;
            SetValue(btnnumber3);
        }
        private void Btnnumber2_Click(object sender, EventArgs e)
        {
            strValue = txtAmoount.Text;
            SetValue(btnnumber2);
        }
        private void Btnnumber1_Click(object sender, EventArgs e)
        {
            strValue = txtAmoount.Text;
            SetValue(btnnumber1);
        }
        private void Btnnumber0_Click(object sender, EventArgs e)
        {
            strValue = txtAmoount.Text;
            SetValue(btnnumber0);
        }
        private void Btnpoint_Click(object sender, EventArgs e)
        {
            try
            {
                strValue = txtAmoount.Text;
                if (string.IsNullOrEmpty(strValue))
                {
                    strValue = "0";
                }

                if (!strValue.Contains("%"))
                {
                    SetValue(btnpoint);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btnpoint_Click at ServiceCharge");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private void Btndeletenumber_Click(object sender, EventArgs e)
        {
            try
            {
                int indexpoint = strValue.LastIndexOf(".");
                int indexclear = 0;
                decimal damount;
                string amount;

                if (strValue.Contains('%'))
                {
                    amount = strValue.Remove(strValue.Length - 1);
                    damount = Convert.ToDecimal(amount);
                    //txtAmoount.Text = Utils.DisplayDecimal(damount * (decimal)0.01);
                    txtAmoount.Text = Utils.DisplayDecimal(damount);
                    strValue = txtAmoount.Text;
                    txtAmoount.Focusable = true;
                    indexclear = txtAmoount.Text.LastIndexOf(".");
                    return;
                }

                if (strValue != string.Empty && strValue.Length > 1)
                {
                    amount = strValue.Remove(strValue.Length - 1);
                    damount = Convert.ToDecimal(amount);
                    txtAmoount.Text = Utils.DisplayDecimal(damount);
                    strValue = txtAmoount.Text;
                    txtAmoount.Focusable = true;
                    indexclear = txtAmoount.Text.LastIndexOf(".");
                }
                else
                {
                    strValue = "";
                    txtAmoount.Text = strValue;
                    txtAmoount.Focusable = true;
                }
                if (txtAmoount.Text != "")
                {
                    damount = Convert.ToDecimal(txtAmoount.Text);
                    if (DECIMALPOINTDISPLAY == "4")
                    {
                        amount = (damount * 1000).ToString();
                    }
                    else
                    {
                        amount = (damount * 10).ToString();
                    }
                }
                else
                {
                    amount = "0";
                }

                txtAmoount.Text = Utils.DisplayDecimal(Convert.ToDecimal(amount) * dec);
                strValue = txtAmoount.Text;

                if (indexpoint > indexclear)
                {
                    count = 0;
                }
                SetBtnClear();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private void ImagebtnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }
        public void SetValue(Button btn)
        {
            try
            {
                if (strValue == "0" && btn.Text != ".")
                {
                    strValue = string.Empty;
                    txtAmoount.Text = strValue;
                }
                string amount;
                if (txtAmoount.Text != "")
                {
                    if (txtAmoount.Text.Contains('%'))
                    {
                        var data = txtAmoount.Text.Replace("%", "");
                        var damount = Convert.ToDouble(data);
                        if (DECIMALPOINTDISPLAY == "4")
                        {
                            amount = (damount * 10000).ToString() + "%";
                        }
                        else
                        {
                            amount = (damount * 100).ToString() + "%";
                        }
                    }
                    else
                    {
                        var damount = Convert.ToDouble(txtAmoount.Text);
                        if (DECIMALPOINTDISPLAY == "4")
                        {
                            amount = (damount * 10000).ToString();
                        }
                        else
                        {
                            amount = (damount * 100).ToString();
                        }
                    }
                }
                else
                {
                    amount = "0";
                }
                var num = btn.Text.ToString();
                btn.RequestFocus();

                if (count == 0)
                {
                    switch (num)
                    {
                        case "0":
                            amount += num;
                            break;
                        case "1":
                            amount += num;
                            break;
                        case "2":
                            amount += num;
                            break;
                        case "3":
                            amount += num;
                            break;
                        case "4":
                            amount += num;
                            break;
                        case "5":
                            amount += num;
                            break;
                        case "6":
                            amount += num;
                            break;
                        case "7":
                            amount += num;
                            break;
                        case "8":
                            amount += num;
                            break;
                        case "9":
                            amount += num;
                            break;
                        default:
                            amount += num;
                            count++;
                            break;
                    }
                }
                else
                {
                    switch (num)
                    {
                        case "0":
                            amount += num;
                            break;
                        case "1":
                            amount += num;
                            break;
                        case "2":
                            amount += num;
                            break;
                        case "3":
                            amount += num;
                            break;
                        case "4":
                            amount += num;
                            break;
                        case "5":
                            amount += num;
                            break;
                        case "6":
                            amount += num;
                            break;
                        case "7":
                            amount += num;
                            break;
                        case "8":
                            amount += num;
                            break;
                        case "9":
                            amount += num;
                            break;
                        default:
                            amount += num;
                            break;
                    }
                }

                if (amount.Contains('%'))
                {
                    var data = amount.Replace("%", "");
                    txtAmoount.Text = Utils.DisplayDecimal(Convert.ToDecimal(data) * dec) + "%";
                    strValue = txtAmoount.Text;
                }
                else
                {
                    txtAmoount.Text = Utils.DisplayDecimal(Convert.ToDecimal(amount) * dec);
                    strValue = txtAmoount.Text;
                }

                SetBtnClear();
                SetBtnSave();

                //Percent
                if (strValue.Contains('%'))
                {
                    decimal maxValue = 100;
                    var data = strValue.Replace("%", "");
                    if (maxValue < Convert.ToDecimal(data))
                    {
                        var checklenght = Utils.CheckLenghtValue(strValue);
                        Toast.MakeText(this, GetString(Resource.String.maxdiscount) + Utils.DisplayDecimal(maxValue) + "%", ToastLength.Short).Show();
                        txtAmoount.Text = Utils.DisplayDecimal(maxValue) + "%";
                        strValue = txtAmoount.Text;
                        return;
                    }
                }
                else
                {
                    //Currency
                    // 6 หลัก 999,999.displayDecimal  
                    string maxdata;
                    if (DECIMALPOINTDISPLAY == "4")
                    {
                        maxdata = Utils.DisplayDecimal((decimal)999999.9999);
                    }
                    else
                    {
                        maxdata = Utils.DisplayDecimal((decimal)999999.99);
                    }

                    if (Convert.ToDecimal(maxdata) < Convert.ToDecimal(strValue))
                    {
                        Toast.MakeText(this, GetString(Resource.String.maxservicecharg) + maxdata, ToastLength.Short).Show();
                        txtAmoount.Text = maxdata;
                        strValue = txtAmoount.Text;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at ServiceCharge");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        bool deviceAsleep = false;
        bool openPage = false;
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

