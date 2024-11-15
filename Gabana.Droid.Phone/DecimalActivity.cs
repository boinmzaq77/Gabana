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
    public class DecimalActivity : Activity
    {
        Spinner spinnerDecimalCal, spinnerDecimalType;
        Button btnSave;
        List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
        Gabana.ORM.Master.MerchantConfig merchantConfig = new ORM.Master.MerchantConfig();
        MerchantConfigManage configManage = new MerchantConfigManage();
        List<MerchantConfig> lstlocal = new List<MerchantConfig>();
        string DECIMALPOINTDISPLAY;
        public static string OPTIONROUNDING_string, OPTIONROUNDING_int;
        string DECIMALPOINTCALC;

        string selectedDecimalType;
        TextView textDecimalType, textOPTIONROUNDING;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.decimal_activity);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                spinnerDecimalCal = FindViewById<Spinner>(Resource.Id.spinnerDecimalShow); //คำนวณ
                spinnerDecimalType = FindViewById<Spinner>(Resource.Id.spinnerCustomerType);//รูปแบบ

                textOPTIONROUNDING = FindViewById<TextView>(Resource.Id.textOPTIONROUNDING);

                spinnerDecimalCal.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerDecimalCal_ItemSelected);
                var adapter1 = ArrayAdapter.CreateFromResource(this, Resource.Array.spinDecimal, Resource.Layout.spinner_item);
                adapter1.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerDecimalCal.Adapter = adapter1;

                spinnerDecimalType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinnerDecimalType_ItemSelected);
                var adapter2 = ArrayAdapter.CreateFromResource(this, Resource.Array.spinDecimal, Resource.Layout.spinner_item);
                adapter2.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerDecimalType.Adapter = adapter2;

                CheckJwt();

                btnSave = FindViewById<Button>(Resource.Id.btnSave);
                btnSave.Click += BtnSave_Click;
                LinearLayout lnDecimalHelp = FindViewById<LinearLayout>(Resource.Id.lnDecimalHelp);
                lnDecimalHelp.Click += LnDecimalHelp_Click; ;
                textDecimalType = FindViewById<TextView>(Resource.Id.textDecimalType);

                DECIMALPOINTCALC = DataCashingAll.setmerchantConfig.DECIMAL_POINT_CALC;//รูปแบบการคำนวณทศนิยม
                DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY; // รูปแบบการแสดงทศนิยม
                OPTIONROUNDING_string = DataCashingAll.setmerchantConfig.OPTION_ROUNDING_STRING; //การปัดเศษทศนิยม
                OPTIONROUNDING_int = DataCashingAll.setmerchantConfig.OPTION_ROUNDING_INT; //การปัดเศษทศนิยม

                int selectedDecimal = Convert.ToInt32(DECIMALPOINTCALC);
                if (selectedDecimal == 2)
                {
                    spinnerDecimalCal.SetSelection(0);
                }
                else
                {
                    spinnerDecimalCal.SetSelection(1);
                }

                int selectedDecimalDisplay = Convert.ToInt32(DECIMALPOINTDISPLAY);
                if (selectedDecimalDisplay == 2)
                {
                    spinnerDecimalType.SetSelection(0);
                }
                else
                {
                    spinnerDecimalType.SetSelection(1);
                }

                selectedDecimalType = OPTIONROUNDING_string;

                FrameLayout lnSpinDecimalDisplay = FindViewById<FrameLayout>(Resource.Id.lnSpinDecimalDisplay);
                FrameLayout lnSpinDecimal = FindViewById<FrameLayout>(Resource.Id.lnSpinDecimal);
                FrameLayout lnSpinDecimalType = FindViewById<FrameLayout>(Resource.Id.lnSpinDecimalType);
                lnSpinDecimal.Click += LnSpinDecimal_Click;
                lnSpinDecimalType.Click += LnSpinDecimalType_Click;
                lnSpinDecimalDisplay.Click += LnSpinDecimalDisplay_Click;
                SetBtnSave();
                SetDecimalDisplay();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : DecimalActivity");


            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" at Decimal");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SpinnerDecimalType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            string select = spinnerDecimalType.SelectedItem.ToString();
            var lowerdata = "4 Decimal digits";
            if (select == "4 หลัก" || select.ToString().ToLower() == lowerdata.ToLower())
            {
                DECIMALPOINTDISPLAY = "4";
            }
            else
            {
                DECIMALPOINTDISPLAY = "2";
            }
            SetBtnSave();
        }

        private void LnSpinDecimalDisplay_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(DecimalHelpActivity)));
        }

        private void SetBtnSave()
        {
            int.TryParse(DataCashingAll.setmerchantConfig.OPTION_ROUNDING_INT, out int result);
            int.TryParse(OPTIONROUNDING_int, out int input);
            if (DECIMALPOINTDISPLAY != DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY ||
               OPTIONROUNDING_string != DataCashingAll.setmerchantConfig.OPTION_ROUNDING_STRING ||
               DECIMALPOINTCALC != DataCashingAll.setmerchantConfig.DECIMAL_POINT_CALC ||
                result != input)
            {
                btnSave.Enabled = true;
                btnSave.SetBackgroundResource(Resource.Drawable.btnblue);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnSave.Enabled = false;
                btnSave.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
        }

        private void LnSpinDecimalType_Click(object sender, EventArgs e)
        {
            spinnerDecimalType.PerformClick();
        }

        protected  override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                ShowdecimalType();
                SetBtnSave();
                SetDecimalDisplay();
            }
            catch (Exception)
            {
                base.OnRestart();
            }
        }

        private void SetDecimalDisplay()
        {
            if (DataCashing.Language == "th")
            {
                switch (OPTIONROUNDING_string)
                {
                    case "0":
                        textOPTIONROUNDING.Text = "ไม่ปัดเศษ";
                        break;
                    case "1":
                        textOPTIONROUNDING.Text = "การปัดเศษแบบที่ 1";
                        break;
                    case "2":
                        textOPTIONROUNDING.Text = "การปัดเศษแบบที่ 2";
                        break;
                    case "3":
                        textOPTIONROUNDING.Text = "การปัดเศษแบบที่ 3";
                        break;
                    case "4":
                        textOPTIONROUNDING.Text = "การปัดเศษแบบที่ 4";
                        break;
                    case "5":
                        textOPTIONROUNDING.Text = "การปัดเศษแบบที่ 5";
                        break;
                }
            }
            else
            {
                switch (OPTIONROUNDING_string)
                {
                    case "0":
                        textOPTIONROUNDING.Text = "Not rounded";
                        break;
                    case "1":
                        textOPTIONROUNDING.Text = "Option 1";
                        break;
                    case "2":
                        textOPTIONROUNDING.Text = "Option 2";
                        break;
                    case "3":
                        textOPTIONROUNDING.Text = "Option 3";
                        break;
                    case "4":
                        textOPTIONROUNDING.Text = "Option 4";
                        break;
                    case "5":
                        textOPTIONROUNDING.Text = "Option 5";
                        break;
                }
            }

        }

        internal static void SetDecimalType(string v, string t)
        {
            OPTIONROUNDING_string = v;
            OPTIONROUNDING_int = t;
        }

        private void LnSpinDecimal_Click(object sender, EventArgs e)
        {
            spinnerDecimalCal.PerformClick();
        }

        private async void ShowdecimalType()
        {
            try
            {
                if (OPTIONROUNDING_string != null)
                {
                    selectedDecimalType = OPTIONROUNDING_string;
                    switch (selectedDecimalType)
                    {
                        case "0":
                            textDecimalType.Text = "ไม่ปัดเศษ";
                            break;
                        case "1":
                            textDecimalType.Text = "แบบที่ 1";
                            break;
                        case "2":
                            textDecimalType.Text = "แบบที่ 2";
                            break;
                        case "3":
                            textDecimalType.Text = "แบบที่ 3";
                            break;
                        case "4":
                            textDecimalType.Text = "แบบที่ 4";
                            break;
                        case "5":
                            textDecimalType.Text = "แบบที่ 5";
                            break;
                        default:
                            textDecimalType.Text = "เลือกรูปแบบการปัดเศษ";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowdecimalType at Decimal");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (await GabanaAPI.CheckNetWork())
            {
                if (await GabanaAPI.CheckSpeedConnection())
                {
                    UpdateMerchantConfig();
                    this.Finish();
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                }                
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
            }
        }

        private async void UpdateMerchantConfig()
        {
            try
            {
                lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();

                //DecimalCal 2,4
                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "DECIMAL_POINT_DISPLAY",
                    CfgInteger = Convert.ToInt32(DECIMALPOINTDISPLAY),
                };
                lstmerchantConfig.Add(merchantConfig);

                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "DECIMAL_POINT_CALC",
                    CfgInteger = Convert.ToInt32(DECIMALPOINTCALC),
                };
                lstmerchantConfig.Add(merchantConfig);


                //DecimalType 1,2,3,4,5 การปัดเศษ
                if (OPTIONROUNDING_string == "4")
                {
                    merchantConfig = new ORM.Master.MerchantConfig()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        CfgKey = "OPTION_ROUNDING",
                        CfgInteger = Convert.ToInt32(OPTIONROUNDING_int),
                        CfgString = OPTIONROUNDING_string
                    };
                    lstmerchantConfig.Add(merchantConfig);
                }
                else
                {
                    merchantConfig = new ORM.Master.MerchantConfig()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        CfgKey = "OPTION_ROUNDING",
                        CfgString = OPTIONROUNDING_string
                    };
                    lstmerchantConfig.Add(merchantConfig);
                }

                var update = await GabanaAPI.PutDataMerchantConfig(lstmerchantConfig, DataCashingAll.DeviceNo);
                if (update.Status)
                {
                    //Insert to Local DB
                    foreach (var item in lstmerchantConfig)
                    {
                        MerchantConfig localConfig = new MerchantConfig()
                        {
                            MerchantID = item.MerchantID,
                            CfgKey = item.CfgKey,
                            CfgInteger = item.CfgInteger,
                            CfgString = item.CfgString
                        };
                        var local = await configManage.InsertorReplacrMerchantConfig(localConfig);
                    }

                    DataCashingAll.setmerchantConfig.DECIMAL_POINT_CALC = DECIMALPOINTCALC;
                    DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY = DECIMALPOINTDISPLAY;
                    DataCashingAll.setmerchantConfig.OPTION_ROUNDING_STRING = OPTIONROUNDING_string;
                    DataCashingAll.setmerchantConfig.OPTION_ROUNDING_INT = OPTIONROUNDING_int;

                    Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                    this.Finish();
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateMerchantConfig at Decimal");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void spinnerDecimalCal_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            string select = spinnerDecimalCal.SelectedItem.ToString();
            if (select == "4 หลัก")
            {
                DECIMALPOINTCALC = "4";
            }
            else
            {
                DECIMALPOINTCALC = "2";
            }
            SetBtnSave();

        }

        private void LnDecimalHelp_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(DecimalHelpActivity)));
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'DecimalActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'DecimalActivity.openPage' is assigned but its value is never used
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

