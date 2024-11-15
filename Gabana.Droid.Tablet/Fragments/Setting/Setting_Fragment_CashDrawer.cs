using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using Org.W3c.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_CashDrawer : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Setting_Fragment_CashDrawer NewInstance()
        {
            Setting_Fragment_CashDrawer frag = new Setting_Fragment_CashDrawer();
            return frag;
        }
        View view;
        public static Setting_Fragment_CashDrawer fragment_device;
        bool flagdatachange = false;
        string flagCashdrawer = string.Empty, switchcashdrawersetting = string.Empty;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_cashdrawer, container, false);
            try
            {
                fragment_device= this;
                CombineUI();
                return view;
            }
            catch (Exception)
            {
                return view;
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                var CashdrawerConfig = string.IsNullOrEmpty(DataCashingAll.setmerchantConfig?.CASHDRAWER) ? "0": DataCashingAll.setmerchantConfig?.CASHDRAWER;
                if (CashdrawerConfig == "0")
                {
                    //0 = ไม่ Auto ,
                    flagCashdrawer = "0";
                    switchCashdrawer.Checked = false;
                }
                else
                {
                    //1 = Auto
                    flagCashdrawer = "1";
                    switchCashdrawer.Checked = true;
                }


                btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnSave.Enabled = false;
                btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        LinearLayout lnBack;
        public Button btnSave, btnOpenCashdawer;
        Switch switchCashdrawer;

        private void CombineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
            btnOpenCashdawer = view.FindViewById<Button>(Resource.Id.btnOpenCashdawer);
            switchCashdrawer = view.FindViewById<Switch>(Resource.Id.switchCashdrawer);
            lnBack.Click += LnBack_Click;
            btnSave.Click += BtnSave_Click;
            btnOpenCashdawer.Click += BtnOpenCashdawer_Click;
            switchCashdrawer.CheckedChange += SwitchCashdrawer_CheckedChange;
        }

        private async void BtnOpenCashdawer_Click(object sender, EventArgs e)
        {
            try
            {
                string typeLogin = Preferences.Get("LoginType", "");
                //string typeLogin = "officer";
                bool check = UtilsAll.CheckPermissionRoleUser(typeLogin, "insert", "cashdrawer");
                if (!check)
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.setting_dialog_cashdrawerrole.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                    return;
                }


				await Utils.Kick();
                //Kick Cashdrawer ที่นี่
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void SwitchCashdrawer_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                var CashdrawerConfig = string.IsNullOrEmpty(DataCashingAll.setmerchantConfig.CASHDRAWER) ? "0" : DataCashingAll.setmerchantConfig.CASHDRAWER;
                switchcashdrawersetting = switchCashdrawer.Checked == true ? "1" : "0";
                if (CashdrawerConfig != switchcashdrawersetting)
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
                CheckDataChange();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave.Enabled = false;
                if (!await GabanaAPI.CheckNetWork())
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    btnSave.Enabled = true;
                    return;
                }

                var checkNet = await GabanaAPI.CheckNetWork();
                if (checkNet)
                {
                    UpdateMerchantConfig();
                }
                else
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                }
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                btnSave.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnSave_Click at Cashdrawer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!flagdatachange)
                {
                    SetClearData(); return;
                }

                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.edit_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                Edit_Dialog_Back.SetPage("cashdrawer");
                Edit_Dialog_Back edit_Dialog = Edit_Dialog_Back.NewInstance();
                edit_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                return;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public async void SetClearData()
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
        }

        private void CheckDataChange()
        {
            try
            {
                //flagCashdrawer = oldsetting
                //switchsetting = newsetting
                switchcashdrawersetting = switchCashdrawer.Checked == true ? "1": "0" ;

                if (switchcashdrawersetting != flagCashdrawer)
                {
                    flagdatachange = true;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void UpdateMerchantConfig()
        {
            try
            {
                switchcashdrawersetting = switchCashdrawer.Checked == true ? "1" : "0";

                var lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();

                //CASHDRAWER
                var merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "CASHDRAWER",
                    CfgInteger = Convert.ToInt32(switchcashdrawersetting),
                };
                lstmerchantConfig.Add(merchantConfig);

                var update = await GabanaAPI.PutDataMerchantConfig(lstmerchantConfig, DataCashingAll.DeviceNo);
                if (!update.Status)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }

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
                    MerchantConfigManage configManage = new MerchantConfigManage();
                    var local = await configManage.InsertorReplacrMerchantConfig(localConfig);
                }

                DataCashingAll.setmerchantConfig.CASHDRAWER = switchcashdrawersetting;
                flagdatachange = false;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateMerchantConfig at Decimal");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
    }
}