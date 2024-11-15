using Android.Accounts;
using Android.App;
using Android.OS;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class VatActivity : Activity
    {
        ImageButton imagebtnBack, btndeletenumber;
        TextView txtVat;
        Button btnpoint, btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9, btnnumber0, btnClear, btnSave;
        VatActivity vatActivity;

        public TransManage transManage = new TransManage();
        public static TranWithDetailsLocal tranWithDetails;
        public static TranPayment tranPayment = new TranPayment();
        string strValue; //strValue คือ จำนวนเงินที่จะจ่าย
        int count = 0;
        public static double Change, Cash;//เงินทอน
        List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
        Gabana.ORM.Master.MerchantConfig merchantConfig = new ORM.Master.MerchantConfig();
        MerchantConfigManage configManage = new MerchantConfigManage();
        private char taxType;
        Button btnInclude, btnExclude;
        string Vat;
        string VatType;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.vat_activity);
                vatActivity = this;

                imagebtnBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                btndeletenumber = FindViewById<ImageButton>(Resource.Id.btndeletenumber);
                txtVat = FindViewById<TextView>(Resource.Id.txtVat);
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

                btnInclude = FindViewById<Button>(Resource.Id.btnInclude);
                btnExclude = FindViewById<Button>(Resource.Id.btnExclude);
                btnInclude.Click += BtnInclude_Click;
                btnExclude.Click += BtnExclude_Click;
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
                VatType = DataCashingAll.setmerchantConfig.TAXTYPE;
                taxType = char.Parse(VatType);

                decimal Vat = 0;
                if (string.IsNullOrEmpty(DataCashingAll.setmerchantConfig.TAXRATE))
                {
                    Vat = 0;
                }
                else
                {
                    Vat = Convert.ToDecimal(DataCashingAll.setmerchantConfig.TAXRATE);                    
                }

                if (Vat == 0)
                {
                    txtVat.Text = Vat.ToString("#,##0.00");
                    strValue = "0";
                }
                else
                {
                    txtVat.Text = Vat.ToString("#,##0.00");
                    strValue = txtVat.Text;
                }
                btnClear.Click += BtnClear_Click;
                btnSave.Click += BtnSave_Click;
                SetBtnClear();
                SetBtnSave();                
                SetBtnTaxType();
                _ = TinyInsights.TrackPageViewAsync("OnCreate : VatActivity");

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnExclude_Click(object sender, EventArgs e)
        {
            taxType = 'E';
            SetBtnTaxType();
        }

        private void BtnInclude_Click(object sender, EventArgs e)
        {
            taxType = 'I';
            SetBtnTaxType();
        }

        private void SetBtnTaxType()
        {
            if (taxType == 'E')
            {
                btnExclude.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnExclude.SetBackgroundResource(Resource.Drawable.btnblue);
                btnInclude.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                btnInclude.SetBackgroundResource(Resource.Drawable.btnborderblue);
            }
            else
            {
                btnInclude.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnInclude.SetBackgroundResource(Resource.Drawable.btnblue);
                btnExclude.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                btnExclude.SetBackgroundResource(Resource.Drawable.btnborderblue);
            }
        }

        private void SetBtnClear()
        {
            if (txtVat.Text != "")
            {
                btnClear.Enabled = true;
                btnClear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            else
            {
                btnClear.Enabled = false;
                btnClear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.colorrule, null));
            }            
        }

        private void SetBtnSave()
        {
            decimal.TryParse(DataCashingAll.setmerchantConfig.TAXRATE, out decimal taxrate);
            decimal.TryParse(txtVat.Text, out decimal taxinput);
            if (taxinput != taxrate || taxType.ToString() != DataCashingAll.setmerchantConfig.TAXTYPE)
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
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (!await GabanaAPI.CheckNetWork())
            {
                Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                return;
            }

            if (!await GabanaAPI.CheckSpeedConnection())
            {
                Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                return;
            }

            await UpdateMerchantConfig();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtVat.Text = "";
            txtVat.Hint = "0.00";            
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
                    #region กดตัวเลขแสดงทศนิยมหลังค่าที่กรอก .00
                    //txtVat.Text = Utils.DisplayDecimal(Convert.ToDecimal(str));
                    //txtVat.Text = Utils.DisplayComma(Convert.ToDecimal(strValue)); 
                    #endregion
                    txtVat.Text = Utils.DisplayComma(str);
                }
                else
                {
                    strValue = "0";
                    #region กดตัวเลขแสดงทศนิยมหลังค่าที่กรอก .00
                    //txtVat.Text = (Convert.ToDecimal(strValue)).ToString("#,##0.00"); //ใส่เพื่อแสดงจุดทศนิยมและศูนย์ศูนย์ .00 ต่อท้ายตัวเลขที่กรอก
                    //txtVat.Text = Utils.DisplayComma(Convert.ToDecimal(strValue));  //ใส่เพื่อแสดงจุดทศนิยมและศูนย์ศูนย์ .00 ต่อท้ายตัวเลขที่กรอก 
                    #endregion
                    txtVat.Text = "";
                    txtVat.Hint = "0.00";
                    txtVat.Focusable = true;
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
                if (strValue == "0" && btn.Text != ".")
                {
                    strValue = string.Empty;
                    txtVat.Text = strValue;
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
                        if (sp.Length == 2)
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

                txtVat.Text = Utils.DisplayComma(str);

                SetBtnClear();
                SetBtnSave();

                //// ค่าสูงสุด 99.99 %
                double maxValue = 99.99;
                if (maxValue < Convert.ToDouble(strValue))
                {
                    var checklenght = Utils.CheckLenghtValue(strValue);
                    Toast.MakeText(this, GetString(Resource.String.maxvat) + " 99.99", ToastLength.Short).Show();
                    txtVat.Text = (9999 * 0.01).ToString("#,##0.00");
                    strValue = txtVat.Text;
                    return;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at Vat");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        async Task UpdateMerchantConfig()
        {
            try
            {
                string vat = "";
                if (string.IsNullOrEmpty(txtVat.Text))
                {
                    vat = Utils.DisplayDecimal(0);
                }
                else
                {
                    vat = txtVat.Text;
                }
                decimal TAXRATE = Convert.ToDecimal(vat);
                lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "TAXRATE",
                    CfgFloat = TAXRATE
                };
                lstmerchantConfig.Add(merchantConfig);

                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "TAXTYPE",
                    CfgString = taxType.ToString()
                };
                lstmerchantConfig.Add(merchantConfig);

                var update = await GabanaAPI.PutDataMerchantConfig(lstmerchantConfig, DataCashingAll.DeviceNo);
                if (!update.Status) 
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }

                Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();

                //Insert to Local DB
                List<MerchantConfig> lstlocal = new List<MerchantConfig>();
                MerchantConfig localConfig = new MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "TAXRATE",
                    CfgFloat = TAXRATE
                };
                lstlocal.Add(localConfig);

                MerchantConfig localConfig2 = new MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "TAXTYPE",
                    CfgString = taxType.ToString()
                };
                lstlocal.Add(localConfig2);

                var localVAT = await configManage.InsertorReplaceListMerchantConfig(lstlocal);
                if (localVAT)
                {
                    Vat = TAXRATE.ToString();
                    VatType = taxType.ToString();

                    DataCashingAll.setmerchantConfig.TAXRATE = Vat;
                    DataCashingAll.setmerchantConfig.TAXTYPE = VatType;
                    this.Finish();
                }
            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);
                _= TinyInsights.TrackPageViewAsync("UpdateMerchantConfig at Vat");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'VatActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'VatActivity.openPage' is assigned but its value is never used
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