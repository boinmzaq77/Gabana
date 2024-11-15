using Android.Accounts;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Trans;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_GiftVoucherNumpad : AndroidX.Fragment.App.Fragment
    {
        public static Setting_Fragment_GiftVoucherNumpad fragment_giftvouchernumpad;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static Setting_Fragment_GiftVoucherNumpad NewInstance()
        {
            Setting_Fragment_GiftVoucherNumpad frag = new Setting_Fragment_GiftVoucherNumpad();
            return frag;
        }

        View view;
        string DECIMALPOINTDISPLAY;
        static string typebtn = string.Empty;
        static string txtCurrency;        

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_giftvouchernumpad, container, false);
            try
            {
                fragment_giftvouchernumpad = this;
                CheckJwt();
                ComBineUI();
                SetEventUI();
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Merchant");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }
        }

        private void SetEventUI()
        {
            lnBack.Click += ImagebtnBack_Click;
            imagebtnBack.Click += ImagebtnBack_Click;
            btndeletenumber.Click += Btndeletenumber_Click;
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
            btnPoint.Click += BtnPoint_Click;
            btnClear.Click += BtnClear_Click;
            btnSave.Click += BtnSave_Click;
            btnPercent.Click += BtnPercent_Click;
            btnCurrency.Click += BtnCurrency_Click;
        }

        private void SetDefaultGiftVoucher()
        {
            DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY; //ทศนิยม
            txtCurrency = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            if (string.IsNullOrEmpty(txtCurrency))
            {
                txtCurrency = "฿";
            }
            btnCurrency.Text = txtCurrency;
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
            typebtn = txtCurrency;
            SetTypeBtn();
        }

        ImageButton imagebtnBack, btndeletenumber;
        static TextView txtServiceCharge;
        Button btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7,
            btnnumber8, btnnumber9, btnnumber0, btnClear, btnSave, btnPoint;
        static Button btnCurrency , btnPercent;
        LinearLayout lnBack;
        int DECIMALPOINT = 2;
        static string strValue = string.Empty;


        private void ComBineUI()
        {
            imagebtnBack = view.FindViewById<ImageButton>(Resource.Id.imagebtnBack);
            btndeletenumber = view.FindViewById<ImageButton>(Resource.Id.btndeletenumber);
            txtServiceCharge = view.FindViewById<TextView>(Resource.Id.txtServiceCharge);
            btnPercent = view.FindViewById<Button>(Resource.Id.btnPercent);
            btnnumber0 = view.FindViewById<Button>(Resource.Id.btnnumber0);
            btnnumber1 = view.FindViewById<Button>(Resource.Id.btnnumber1);
            btnnumber2 = view.FindViewById<Button>(Resource.Id.btnnumber2);
            btnnumber3 = view.FindViewById<Button>(Resource.Id.btnnumber3);
            btnnumber4 = view.FindViewById<Button>(Resource.Id.btnnumber4);
            btnnumber5 = view.FindViewById<Button>(Resource.Id.btnnumber5);
            btnnumber6 = view.FindViewById<Button>(Resource.Id.btnnumber6);
            btnnumber7 = view.FindViewById<Button>(Resource.Id.btnnumber7);
            btnnumber8 = view.FindViewById<Button>(Resource.Id.btnnumber8);
            btnnumber9 = view.FindViewById<Button>(Resource.Id.btnnumber9);
            btnPoint = view.FindViewById<Button>(Resource.Id.btnPoint);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
            btnClear = view.FindViewById<Button>(Resource.Id.btnClear);
            btnCurrency = view.FindViewById<Button>(Resource.Id.btnCurrency);
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
        }

        bool clickPoint = false;
        private void BtnPoint_Click(object sender, EventArgs e)
        {
            try
            {
                clickPoint = true;
                SetValue(btnPoint);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnPoint_Click at GiftVoucherNumpad");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void BtnCurrency_Click(object sender, EventArgs e)
        {
            try
            {                
                typebtn = txtCurrency;
                SetTypeBtn();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnCurrency_Click at ServiceCharge");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private static void SetTypeBtn()
        {
            if (typebtn == txtCurrency) // Currency
            {
                btnCurrency.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnCurrency.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnPercent.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                btnPercent.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
            }
            else
            {
                btnPercent.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnPercent.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnCurrency.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                btnCurrency.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtServiceCharge.Text = "";
            txtServiceCharge.Hint = Utils.DisplayDecimal(0);
            strValue = "0";           
            SetBtnClear();
            SetBtnSave();
        }
        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {               
                DataCashing.flagAmountGiftVoucher = true;
                Setting_Fragment_AddGiftVoucher.fragment_giftvoucher.SetAddAmount(strValue,typebtn);                
                SetBtnClear();
                SetBtnSave();
                strValue = "0";
                Setting_Fragment_AddGiftVoucher.fragment_giftvoucher.CheckDataChange();
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addgiftvoucher");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("btnSave_Click at gift");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
        Gabana.ORM.Master.MerchantConfig merchantConfig = new ORM.Master.MerchantConfig();
        MerchantConfigManage configManage = new MerchantConfigManage();

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

        private async void ImagebtnBack_Click(object sender, EventArgs e)
        {
            DataCashing.flagAmountGiftVoucher = true;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addgiftvoucher");
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
                    txtServiceCharge.Text = Utils.DisplayComma(str);
                }
                else
                {
                    strValue = "0";
                    txtServiceCharge.Text = "";
                    txtServiceCharge.Hint = "0.00";
                    txtServiceCharge.Focusable = true;
                }
                SetTypeBtn();
                SetBtnClear();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private void SetBtnClear()
        {
            if (txtServiceCharge.Text != "")
            {
                btnClear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else
            {
                btnClear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.colorrule, null));
            }
        }
        bool hasPercent = false;
        private void SetBtnSave()
        {
            try
            {
                if (string.IsNullOrEmpty(strValue))
                {
                    strValue = "0";
                }

                if (strValue.Contains("%"))
                {
                    strValue = strValue.Replace("%", "");
                    hasPercent = true;
                }
                else if (strValue.Contains(txtCurrency))
                {
                    if (!string.IsNullOrEmpty(txtCurrency))
                    {
                        strValue = strValue.Replace(txtCurrency, "");
                    }                   
                    hasPercent = false; 
                }
                else
                {
                    if (Convert.ToDecimal(strValue) == 0)
                    {
                        strValue = "0";
                    }
                    hasPercent = false;
                }

                if (Convert.ToDecimal(strValue) > 0)
                {
                    btnSave.Enabled = true;
                    btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                    btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                }
                else
                {
                    btnSave.Enabled = true;
                    btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                    btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
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

        private void BtnPercent_Click(object sender, EventArgs e)
        {
            try
            {
                typebtn = "%";
                SetTypeBtn();                
                SetBtnSave();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnPercent_Click at ServiceCharge");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
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
                    txtServiceCharge.Text = strValue;
                }

                if (strValue.Contains("%") || strValue.Contains(txtCurrency))
                {
                    return;
                }

                if (clickPoint)
                {
                    strValue += ".";
                    var data = Utils.AllIndexOf(strValue, ".", StringComparison.Ordinal);
                    if (data.Count > 1)
                    {
                        strValue = strValue.Substring(0, strValue.Length - 1);
                        clickPoint = false;
                        return;
                    }
                    str = strValue;
                    clickPoint = false;
                }
                else
                {
                    decimal result = 0;
                    decimal.TryParse(txtServiceCharge.Hint, out result);
                    if (result != 0)
                    {
                        strValue = string.Empty;
                        txtServiceCharge.Hint = "0.00";
                    }

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
                txtServiceCharge.Text = Utils.DisplayComma(str);

                //Percent
                if (strValue.Contains('%'))
                {
                    decimal maxValue = 100;
                    var data = strValue.Replace("%", "");
                    if (maxValue < Convert.ToDecimal(data))
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.maxservicecharg) + " " + Utils.DisplayDecimal(maxValue) + "%", ToastLength.Short).Show();
                        txtServiceCharge.Text = Utils.DisplayDecimal(maxValue) + "%";
                        strValue = txtServiceCharge.Text;
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
                        Toast.MakeText(this.Activity, GetString(Resource.String.maxservicecharg) + " " + maxdata, ToastLength.Short).Show();
                        txtServiceCharge.Text = maxdata;
                        strValue = txtServiceCharge.Text;
                        return;
                    }
                }

                SetBtnClear();
                SetTypeBtn();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at ServiceCharge");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }        

        private void UINewGiftVoucherNumpad()
        {
            txtServiceCharge.Text = string.Empty;
        }

        public override void OnResume()
        {
            base.OnResume();

            //if (!IsVisible)
            //{
            //    return;
            //}

            CheckJwt();
            UINewGiftVoucherNumpad();
            SetDefaultGiftVoucher();            
            if (string.IsNullOrEmpty(strValue))
            {
                txtServiceCharge.Hint = Utils.DisplayDecimal(0);
            }
            else
            {
                txtServiceCharge.Hint = strValue;                
            }
            SetBtnClear();
            SetBtnSave();
        }       

        private async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
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

        internal static void SetAmount(string amount)
        {
            try
            {
                if (string.IsNullOrEmpty(amount))
                {
                    strValue = string.Empty;
                }
                else
                {
                    if (amount.Contains("%"))
                    {
                        strValue = Utils.DisplayDecimal(Convert.ToDecimal(amount.Replace("%", "")));
                        typebtn = "%";
                        SetTypeBtn();
                    }
                    else if (amount.Contains(txtCurrency))
                    {
                        if (!string.IsNullOrEmpty(txtCurrency))
                        {
                            strValue = Utils.DisplayDecimal(Convert.ToDecimal(amount.Replace(txtCurrency, "")));
                        }
                        else
                        {
                            strValue = Utils.DisplayDecimal(Convert.ToDecimal(amount));
                        }
                        typebtn = txtCurrency;
                        SetTypeBtn();
                    }
                    else
                    {
                        strValue = Utils.DisplayDecimal(Convert.ToDecimal(amount));
                        typebtn = txtCurrency;
                        SetTypeBtn();
                    }

                    if (string.IsNullOrEmpty(strValue))
                    {
                        txtServiceCharge.Hint = Utils.DisplayDecimal(0);
                    }
                    else
                    {
                        txtServiceCharge.Hint = strValue;
                    }
                    txtServiceCharge.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetAddAmount at giftvouchernumpad");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

    }
}