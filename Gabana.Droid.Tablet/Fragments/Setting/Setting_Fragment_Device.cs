using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.ShareSource;
using Newtonsoft.Json;
using Org.W3c.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_Device : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Setting_Fragment_Device NewInstance()
        {
            Setting_Fragment_Device frag = new Setting_Fragment_Device();
            return frag;
        }
        View view;
        public static Setting_Fragment_Device fragment_device;
        bool flagdatachange = false;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_device, container, false);
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
            base.OnResume();

            //if (!IsVisible)
            //{
            //    return;
            //}

            textUdid.Text = (DataCashingAll.DeviceUDID).ToUpper();
            textDeviceNo.Text = DataCashingAll.Merchant?.Device?.DeviceNo.ToString() == null ? DataCashingAll.DeviceNo.ToString() : DataCashingAll.Merchant?.Device?.DeviceNo.ToString();
            btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
            btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            btnSave.Enabled = false;
            btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
            btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            if (DataCashingAll.Device != null)
            {
                textComment.Text = DataCashingAll.Device.Comments;
            }

        }

        LinearLayout lnBack;
        EditText textDeviceNo, textUdid, textComment;
        internal Button btnSave;
        string comments = "";
        private void CombineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            textDeviceNo = view.FindViewById<EditText>(Resource.Id.textDeviceNo);
            textUdid = view.FindViewById<EditText>(Resource.Id.textUdid);
            textComment = view.FindViewById<EditText>(Resource.Id.textComment);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);

            textDeviceNo.Enabled = false;
            textUdid.Enabled = false;
            textComment.TextChanged += TextComment_TextChanged;
            btnSave.Click += BtnSave_Click;
            lnBack.Click += LnBack_Click;
        }
        private void TextComment_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (DataCashingAll.Device.Comments != null)
            {
                comments = DataCashingAll.Device.Comments;
            }

            if (textComment.Text != comments)
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

                if (DataCashingAll.Device != null)
                {
                    btnSave.Enabled = false;
                    ORM.Master.Device device = new ORM.Master.Device();
                    device.MerchantID = DataCashingAll.Device.MerchantID;
                    device.Comments = textComment.Text;
                    device.DateCreated = Utils.GetTranDate(DataCashingAll.Device.DateCreated);
                    device.DateLastActive = Utils.GetTranDate(DataCashingAll.Device.DateLastActive);
                    device.DeviceInfo = DataCashingAll.Device.DeviceInfo;
                    device.DeviceNo = DataCashingAll.Device.DeviceNo;
                    device.Platform = DataCashingAll.Device.Platform;
                    device.UDID = DataCashingAll.Device.UDID;

                    var updateDevice = await GabanaAPI.PutDataDevice(device);
                    if (!updateDevice.Status)
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    }
                    DataCashingAll.Device.Comments = device.Comments;
                    DataCashingAll.Device.DateLastActive = Utils.GetTranDate(device.DateLastActive);
                    Toast.MakeText(this.Activity, GetString(Resource.String.savesucess), ToastLength.Short).Show();

                    var DeviceDetail = JsonConvert.SerializeObject(device);
                    Preferences.Set("Device", DeviceDetail);
                    var setDevice = Preferences.Get("Device", "");
                    if (setDevice != "")
                    {
                        var Config = JsonConvert.DeserializeObject<Device>(setDevice);
                        DataCashingAll.Device = Config;
                    }
                }
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                btnSave.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnSave_Click at Device");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {            
            if (!flagdatachange)
            {
                SetClearData(); return;
            }

            Bundle bundle = new Bundle();
            String myMessage = Resource.Layout.edit_dialog_back.ToString();
            bundle.PutString("message", myMessage);
            Edit_Dialog_Back.SetPage("device");
            Edit_Dialog_Back edit_Dialog = Edit_Dialog_Back.NewInstance();
            edit_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
            return;
        }

        public async void SetClearData()
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
        }

        private void CheckDataChange()
        {
            if (textComment.Text != comments)
            {
                flagdatachange = true;
            }
        }
    }
}