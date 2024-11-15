using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_Decimal : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Setting_Fragment_Decimal NewInstance()
        {
            Setting_Fragment_Decimal frag = new Setting_Fragment_Decimal();
            return frag;
        }
        View view;
        public static Setting_Fragment_Decimal fragment_currency;
        string DECIMALPOINTCALC;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_decimal, container, false);
            try
            {
                CombineUI();
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Merchant");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            //if (!IsVisible)
            //{
            //    return;
            //}

            spinnerDecimalCal.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerDecimalCal_ItemSelected);
            var adapter1 = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.spinDecimal, Resource.Layout.spinner_item);
            adapter1.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerDecimalCal.Adapter = adapter1;

            spinnerDecimalType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinnerDecimalType_ItemSelected);
            var adapter2 = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.spinDecimal, Resource.Layout.spinner_item);
            adapter2.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerDecimalType.Adapter = adapter2;

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

            ShowdecimalType();
            SetBtnSave();
            SetDecimalDisplay();
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
        LinearLayout lnBack;
        Spinner spinnerDecimalCal, spinnerDecimalType;
        FrameLayout lnSpinDecimal, lnSpinDecimalType, lnSpinDecimalDisplay;
        TextView textDecimalType, textOPTIONROUNDING;
        Button btnSave;
        private void CombineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            spinnerDecimalCal = view.FindViewById<Spinner>(Resource.Id.spinnerDecimalCal);
            lnSpinDecimal = view.FindViewById<FrameLayout>(Resource.Id.lnSpinDecimal);
            textDecimalType = view.FindViewById<TextView>(Resource.Id.textDecimalType);
            spinnerDecimalType = view.FindViewById<Spinner>(Resource.Id.spinnerDecimalType);
            lnSpinDecimalType = view.FindViewById<FrameLayout>(Resource.Id.lnSpinDecimalType);
            textOPTIONROUNDING = view.FindViewById<TextView>(Resource.Id.textOPTIONROUNDING);

            lnSpinDecimalDisplay = view.FindViewById<FrameLayout>(Resource.Id.lnSpinDecimalDisplay);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
            btnSave.Click += BtnSave_Click;
            lnSpinDecimalDisplay.Click += LnSpinDecimalDisplay_Click;
            lnSpinDecimalType.Click += LnSpinDecimalType_Click;
            lnSpinDecimal.Click += LnSpinDecimal_Click;

        }

        private void LnSpinDecimal_Click(object sender, EventArgs e)
        {
            spinnerDecimalCal.PerformClick();
        }

        private void LnSpinDecimalType_Click(object sender, EventArgs e)
        {
            spinnerDecimalType.PerformClick();
        }
        internal static void SetDecimalType(string v, string t)
        {
            OPTIONROUNDING_string = v;
            OPTIONROUNDING_int = t;
        }
        private async void LnSpinDecimalDisplay_Click(object sender, EventArgs e)
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting,"setting", "decimalhelp");
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            var checkNet = await GabanaAPI.CheckNetWork();
            if (checkNet)
            {
                UpdateMerchantConfig();
            }
            else
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
            }
        }

        List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
        Gabana.ORM.Master.MerchantConfig merchantConfig = new ORM.Master.MerchantConfig();
        MerchantConfigManage configManage = new MerchantConfigManage();
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

                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
                }
                else
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateMerchantConfig at Decimal");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }

        }

        public static string OPTIONROUNDING_string, OPTIONROUNDING_int;
        string selectedDecimalType;
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        string DECIMALPOINTDISPLAY;
        private void SetBtnSave()
        {
            int.TryParse(DataCashingAll.setmerchantConfig.OPTION_ROUNDING_INT, out int result);
            int.TryParse(OPTIONROUNDING_int, out int input);

            if (DECIMALPOINTDISPLAY != DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY || 
                OPTIONROUNDING_string != DataCashingAll.setmerchantConfig.OPTION_ROUNDING_STRING || 
                DECIMALPOINTCALC !=  DataCashingAll.setmerchantConfig.DECIMAL_POINT_CALC ||
                 result != input )
            {
                btnSave.Enabled = true;
                btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnSave.Enabled = false;
                btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
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


    }
}