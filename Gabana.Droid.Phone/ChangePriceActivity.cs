using Android.App;
using Android.OS;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class ChangePriceActivity : Activity
    {
        ChangePriceActivity changePriceActivity;
        ImageButton imagebtnBack, btndeletenumber;
        public static TranWithDetailsLocal TranWithDetails;
        public static TranDetailItemNew itemNew;
        TextView txtNewPrice;
        Button btnpoint, btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9, btnnumber0, btnClear, btnDone;
        string strValue, DECIMALPOINTDISPLAY;
        int DECIMALPOINT = 2;
        decimal dec;
        bool deviceAsleep = false;
        bool openPage = false;
        public DateTime pauseDate = DateTime.Now;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.changeprice_activity_main);
                changePriceActivity = this;
                imagebtnBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                btndeletenumber = FindViewById<ImageButton>(Resource.Id.btndeletenumber);
                txtNewPrice = FindViewById<TextView>(Resource.Id.txtDisCount);
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
                btnDone = FindViewById<Button>(Resource.Id.btnDone);
                btnClear = FindViewById<Button>(Resource.Id.btnClear);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += ImagebtnBack_Click;
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

                txtNewPrice.Hint = Utils.DisplayDecimal(0);
                txtNewPrice.Text = strValue;
                btnClear.Click += BtnClear_Click;
                btnDone.Click += BtnDone_Click; 

                DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY; //ทศนิยม
                if (!string.IsNullOrEmpty(DECIMALPOINTDISPLAY))
                {
                    if (DECIMALPOINTDISPLAY == "4")
                    {
                        DECIMALPOINT = 4;
                    }
                    else
                    {
                        DECIMALPOINT = 2;
                    }
                }
                else
                {
                    DECIMALPOINT = 2;
                }

                SetBtnClear();
                SetBtnSave();
                _ = TinyInsights.TrackPageViewAsync("OnCreate : ChangePriceActivity");

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void BtnDone_Click(object sender, EventArgs e)
        {
            try
            {
                decimal price = 0;
                if (string.IsNullOrEmpty(txtNewPrice.Text))
                {
                    price = Convert.ToDecimal("0");
                }
                else
                {
                    price = Convert.ToDecimal(txtNewPrice.Text);
                }
                TranWithDetails = BLTrans.ChangePrice(TranWithDetails, itemNew, price);
                DataCashing.ModifyTranOrder = true;
                TranWithDetails = BLTrans.Caltran(TranWithDetails);
                if (CartActivity.CurrentActivity)
                {
                    CartActivity.SetTranDetail(TranWithDetails);
                    this.Finish();
                }
                else
                {
                    CartScanActivity.SetTranDetail(TranWithDetails);
                    this.Finish();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnDone_Click at changePass");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetBtnClear()
        {
            if (txtNewPrice.Text != "")
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
            if (txtNewPrice.Text != itemNew.TotalPrice.ToString() && txtNewPrice.Text != "")
            {
                btnDone.Enabled = true;
                btnDone.SetBackgroundResource(Resource.Drawable.btnblue);
                btnDone.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnDone.Enabled = false;
                btnDone.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnDone.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
        }


        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtNewPrice.Text = "";
            txtNewPrice.Hint = Utils.DisplayDecimal(0);
            strValue = "0";
            SetBtnClear();
            SetBtnSave();
        }

        private void Btnnumber9_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber9);
        }

        private void Btnnumber8_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber8);
        }

        private void Btnnumber7_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber7);
        }

        private void Btnnumber6_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber6);
        }

        private void Btnnumber5_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber5);
        }

        private void Btnnumber4_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber4);
        }

        private void Btnnumber3_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber3);
        }

        private void Btnnumber2_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber2);
        }

        private void Btnnumber1_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber1);
        }

        private void Btnnumber0_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber0);
        }

        bool clickPoint = false;
        private void Btnpoint_Click(object sender, EventArgs e)
        {
            clickPoint = true;
            SetValue(btnpoint);
        }

        private void Btndeletenumber_Click(object sender, EventArgs e)
        {
            try
            {
                if (strValue != string.Empty && strValue.Length > 1)
                {
                    strValue = strValue.Remove(strValue.Length - 1);
                    string str = "";
                    str = strValue;
                    txtNewPrice.Text = Utils.DisplayComma(str);
                }
                else
                {
                    strValue = "0";
                    txtNewPrice.Text = "";
                    txtNewPrice.Hint = "0.00";
                    txtNewPrice.Focusable = true;
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
                string str = "";
                if (string.IsNullOrEmpty(strValue))
                {
                    strValue = "0";
                }
                if (strValue == "0" && btn.Text != ".")
                {
                    strValue = string.Empty;
                    txtNewPrice.Text = strValue;
                }

                if (clickPoint)
                {
                    #region กดตัวเลขแสดงทศนิยมหลังค่าที่กรอก .00
                    //if (strValue.Contains(".00"))
                    //{
                    //    var val = strValue.Split(".00");
                    //    strValue = val[0];
                    //} 
                    #endregion
                    strValue += ".";
                    var data = Utils.AllIndexOf(strValue, ".", StringComparison.Ordinal);
                    if (data.Count > 1)
                    {
                        strValue = strValue.Substring(0,strValue.Length - 1);
                        clickPoint = false;
                        return;
                    }
                    str = strValue;
                    clickPoint = false;
                }
                else
                {
                    if (strValue.Contains("."))
                    {
                        var val = strValue.Split(".");
                        var sp = val[1];
                        if (sp.Length == DECIMALPOINT)
                        {
                            return;
                        }                        
                    }
                    str = strValue + btn.Text.ToString();
                }
                strValue = str;

                #region กดตัวเลขแสดงทศนิยมหลังค่าที่กรอก .00
                //txtVat.Text = (Convert.ToDouble(str)).ToString("#,##0.00");
                //txtVat.Text = Utils.DisplayComma(Convert.ToDecimal(str)); 
                #endregion

                txtNewPrice.Text = Utils.DisplayComma(str);

                string maxdata;
                if (DECIMALPOINTDISPLAY == "4")
                {
                    maxdata = Utils.DisplayDecimal((decimal)9999999999.9999);
                    //maxdata = Utils.DisplayDecimal((decimal)9999999999999.9999);
                }
                else
                {
                    maxdata = Utils.DisplayDecimal((decimal)9999999999.99);
                    //maxdata = Utils.DisplayDecimal((decimal)9999999999999.99);
                }

                if (Convert.ToDecimal(maxdata) < Convert.ToDecimal(strValue))
                {
                    Toast.MakeText(this, GetString(Resource.String.maxprice) + maxdata, ToastLength.Short).Show();
                    txtNewPrice.Text = maxdata;
                    strValue = txtNewPrice.Text;
                    return;
                }

                SetBtnClear();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at ChangePride");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public static void SetSysItem(TranDetailItemNew item, TranWithDetailsLocal tranWithDetails)
        {
            itemNew = item;
            TranWithDetails = tranWithDetails;
        }

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

