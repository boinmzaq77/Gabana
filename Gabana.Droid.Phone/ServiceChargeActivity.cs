using Android.App;
using Android.OS;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class ServiceChargeActivity : Activity
    {
        ServiceChargeActivity serviceChargeActivity;
        ImageButton imagebtnBack, btndeletenumber;
        public static TranWithDetailsLocal TranWithDetails;
        public static TranDetailItemNew itemNew;
        TextView txtServiceCharge;
        public static double newPrice;
        Button btnpoint, btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9, btnnumber0, btnClear, btnSave;
        Button btnAfterDiscount, btnBeforeDiscount;
        string strValue = Utils.DisplayDecimal(0);
        int count = 0;
        string typeServiceCharge;
        List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
        Gabana.ORM.Master.MerchantConfig merchantConfig = new ORM.Master.MerchantConfig();
        MerchantConfigManage configManage = new MerchantConfigManage();
        string SERVICECHARGERATE, DECIMALPOINTDISPLAY, SERVICECHARGETYPE;
        decimal dec;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.servicecharge_activity);
                serviceChargeActivity = this;
                imagebtnBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                btndeletenumber = FindViewById<ImageButton>(Resource.Id.btndeletenumber);
                txtServiceCharge = FindViewById<TextView>(Resource.Id.txtServiceCharge);
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
                if (string.IsNullOrEmpty(DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE))
                {
                    SERVICECHARGERATE = "";
                }
                else
                {
                    SERVICECHARGERATE = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE; //อัตราการคิด service charge
                }

                SERVICECHARGETYPE = DataCashingAll.setmerchantConfig.SERVICECHARGE_TYPE; //รูปแบบการคิด service charge A,B               
                DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY; //ทศนิยม
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

                txtServiceCharge.Hint = Utils.DisplayDecimal(0) + "%";

                decimal servicecharge = 0;
                if (string.IsNullOrEmpty(SERVICECHARGERATE))
                {
                    servicecharge = 0;
                }
                else
                {
                    var check = SERVICECHARGERATE.IndexOf('%');
                    if (check == -1)
                    {
                        servicecharge = Convert.ToDecimal(SERVICECHARGERATE);
                    }
                    else
                    {
                        servicecharge = Convert.ToDecimal(SERVICECHARGERATE.Remove(check)); //% ที่ SERVICECHARGE_RATE
                    }
                }

                if (SERVICECHARGERATE.Contains('%'))
                {
                    txtServiceCharge.Text = servicecharge == 0 ? Utils.DisplayDecimal(0) : Utils.DisplayDecimal(servicecharge) + "%";
                }
                else
                {
                    txtServiceCharge.Text = servicecharge == 0 ? Utils.DisplayDecimal(0) : Utils.DisplayDecimal(servicecharge);
                }

                typeServiceCharge = SERVICECHARGETYPE;
                strValue = txtServiceCharge.Text;

                btnAfterDiscount = FindViewById<Button>(Resource.Id.btnAfter);
                btnBeforeDiscount = FindViewById<Button>(Resource.Id.btnBefore);
                btnBeforeDiscount.Click += BtnBefore_Click;
                btnAfterDiscount.Click += BtnAfter_Click;

                btnClear.Click += BtnClear_Click;
                btnSave.Click += btnSave_Click;
                SetBtnClear();
                SetBtnSave();
                SetTypeBtnService();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }

        }

        private void BtnAfter_Click(object sender, EventArgs e)
        {
            typeServiceCharge = "A";
            SetTypeBtnService();
        }

        private void BtnBefore_Click(object sender, EventArgs e)
        {
            typeServiceCharge = "B";
            SetTypeBtnService();
        }

        private void SetTypeBtnService()
        {
            if (typeServiceCharge == "A")
            {
                btnAfterDiscount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnAfterDiscount.SetBackgroundResource(Resource.Drawable.btnblue);
                btnBeforeDiscount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                btnBeforeDiscount.SetBackgroundResource(Resource.Drawable.btnborderblue);
            }
            else
            {
                btnBeforeDiscount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnBeforeDiscount.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAfterDiscount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                btnAfterDiscount.SetBackgroundResource(Resource.Drawable.btnborderblue);
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
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

                UpdateMerchantConfig();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("btnSave_Click at ServiceCharge");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        async void UpdateMerchantConfig()
        {
            try
            {
                lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "SERVICECHARGE_TYPE",
                    CfgString = typeServiceCharge
                };
                lstmerchantConfig.Add(merchantConfig);

                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "SERVICECHARGE_RATE",
                    CfgString = strValue
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
                    CfgKey = "SERVICECHARGE_TYPE",
                    CfgString = typeServiceCharge
                };
                lstlocal.Add(localConfig);

                MerchantConfig localConfig2 = new MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "SERVICECHARGE_RATE",
                    CfgString = strValue
                };
                lstlocal.Add(localConfig2);

                var localVAT = await configManage.InsertorReplaceListMerchantConfig(lstlocal);
                if (localVAT)
                {
                    SERVICECHARGERATE = strValue;
                    SERVICECHARGETYPE = typeServiceCharge.ToString();

                    DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE = SERVICECHARGERATE;
                    DataCashingAll.setmerchantConfig.SERVICECHARGE_TYPE = SERVICECHARGETYPE;
                    this.Finish();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateMerchantConfig at ServiceCharge");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void SetBtnClear()
        {
            if (txtServiceCharge.Text != "")
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
                var index = strValue.IndexOf('%');
                string service = txtServiceCharge.Text;
                if (index != -1)
                {
                    service = strValue.Remove(index);
                }

                var index2 = SERVICECHARGERATE.IndexOf('%');
                string service2 = SERVICECHARGERATE;
                if (index2 != -1)
                {
                    service2 = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE.Remove(index2);
                }

                decimal servicechargeNew = Convert.ToDecimal(string.IsNullOrEmpty(service) ? "0" : service);
                decimal servicechargeOld = Convert.ToDecimal(string.IsNullOrEmpty(service2) ? "0" : service2);

                if (servicechargeNew != servicechargeOld)
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
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateMerchantConfig at ServiceCharge");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }


        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtServiceCharge.Text = "";
            strValue = "0";
            txtServiceCharge.Hint = Utils.DisplayDecimal((decimal)newPrice);
            SetBtnClear();
            SetBtnSave();
        }

        private void Btnnumber9_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber9);
        }

        private void Btnnumber8_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber8);
        }

        private void Btnnumber7_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber7);
        }

        private void Btnnumber6_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber6);
        }

        private void Btnnumber5_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber5);
        }

        private void Btnnumber4_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber4);
        }

        private void Btnnumber3_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber3);
        }

        private void Btnnumber2_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber2);
        }

        private void Btnnumber1_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber1);
        }

        private void Btnnumber0_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber0);
        }

        //%
        private void Btnpoint_Click(object sender, EventArgs e)
        {
            try
            {
                strValue = txtServiceCharge.Text;
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
                    //txtServiceCharge.Text = Utils.DisplayDecimal(damount * (decimal)0.01);
                    txtServiceCharge.Text = Utils.DisplayDecimal(damount);
                    strValue = txtServiceCharge.Text;
                    txtServiceCharge.Focusable = true;
                    indexclear = txtServiceCharge.Text.LastIndexOf(".");
                    return;
                }

                if (strValue != string.Empty && strValue.Length > 1)
                {
                    amount = strValue.Remove(strValue.Length - 1);
                    damount = Convert.ToDecimal(amount);
                    txtServiceCharge.Text = Utils.DisplayDecimal(damount);
                    strValue = txtServiceCharge.Text;
                    txtServiceCharge.Focusable = true;
                    indexclear = txtServiceCharge.Text.LastIndexOf(".");
                }
                else
                {
                    strValue = "";
                    txtServiceCharge.Text = strValue;
                    txtServiceCharge.Focusable = true;
                }

                if (txtServiceCharge.Text != "")
                {
                    damount = Convert.ToDecimal(txtServiceCharge.Text);
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

                txtServiceCharge.Text = Utils.DisplayDecimal(Convert.ToDecimal(amount) * dec);
                strValue = txtServiceCharge.Text;

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
                    txtServiceCharge.Text = strValue;
                }
                string amount;
                if (txtServiceCharge.Text != "")
                {
                    if (txtServiceCharge.Text.Contains('%'))
                    {
                        var data = txtServiceCharge.Text.Replace("%", "");
                        var damount = Convert.ToDouble(data);
                        amount = (damount * 10000).ToString("#,###.####") + "%";
                    }
                    else
                    {
                        var damount = Convert.ToDouble(txtServiceCharge.Text);
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
                    txtServiceCharge.Text = Utils.DisplayDecimal(Convert.ToDecimal(data) * dec) + "%";
                    strValue = txtServiceCharge.Text;
                }
                else
                {
                    txtServiceCharge.Text = Utils.DisplayDecimal(Convert.ToDecimal(amount) * dec);
                    strValue = txtServiceCharge.Text;
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
                        Toast.MakeText(this, GetString(Resource.String.maxservicecharg) + " " + Utils.DisplayDecimal(maxValue) + "%", ToastLength.Short).Show();
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
                        Toast.MakeText(this, GetString(Resource.String.maxservicecharg) + " " + maxdata, ToastLength.Short).Show();
                        txtServiceCharge.Text = maxdata;
                        strValue = txtServiceCharge.Text;
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

        public static void SetSysItem(TranDetailItemNew item, TranWithDetailsLocal tranWithDetails)
        {
            itemNew = item;
            TranWithDetails = tranWithDetails;
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'ServiceChargeActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'ServiceChargeActivity.openPage' is assigned but its value is never used
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

