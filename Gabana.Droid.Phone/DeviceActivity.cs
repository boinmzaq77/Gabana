using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Newtonsoft.Json;
using System;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class DeviceActivity : AppCompatActivity
    {
        public static DeviceActivity activity;
        EditText textDeviceNo, textUdid, textComment;
        internal Button btnSave;
        int count = 0;
        string comments = "";
        bool flagdatachange = false;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.device_activity);

                activity = this;
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;

                textDeviceNo = FindViewById<EditText>(Resource.Id.textDeviceNo);
                textUdid = FindViewById<EditText>(Resource.Id.textUdid);
                textComment = FindViewById<EditText>(Resource.Id.textComment);
                textDeviceNo.Enabled = false;
                textUdid.Enabled = false;
                textComment.TextChanged += TextComment_TextChanged;
                btnSave = FindViewById<Button>(Resource.Id.btnSave);
                btnSave.Click += BtnSave_Click;
                btnSave.LongClick += BtnSave_LongClick;

                CheckJwt();

                textUdid.Text = (DataCashingAll.DeviceUDID).ToUpper();
                textDeviceNo.Text = DataCashingAll.Merchant?.Device?.DeviceNo.ToString() == null ? DataCashingAll.DeviceNo.ToString() : DataCashingAll.Merchant?.Device?.DeviceNo.ToString();
                btnSave.SetBackgroundResource(Resource.Drawable.btnblue);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnSave.Enabled = false;
                btnSave.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                if (DataCashingAll.Device != null)
                {
                    textComment.Text = DataCashingAll.Device.Comments;
                }

                _ = TinyInsights.TrackPageViewAsync("OnCreate : DeviceActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Device");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }

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
                btnSave.SetBackgroundResource(Resource.Drawable.btnblue);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnSave.Enabled = false;
                btnSave.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            CheckDataChange();
        }
        private void BtnSave_LongClick(object sender, Android.Views.View.LongClickEventArgs e)
        {
            count++;
            if (count == 2)
            {
                Utils.CopyDatabase();
            }
        }
        private async void BtnSave_Click(object sender, EventArgs e)
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

                if (DataCashingAll.Device == null)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }

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
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }

                DataCashingAll.Device.Comments = device.Comments;
                DataCashingAll.Device.DateLastActive = Utils.GetTranDate(device.DateLastActive);
                Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();

                var DeviceDetail = JsonConvert.SerializeObject(device);
                Preferences.Set("Device", DeviceDetail);
                var setDevice = Preferences.Get("Device", "");
                if (setDevice != "")
                {
                    var Config = JsonConvert.DeserializeObject<Device>(setDevice);
                    DataCashingAll.Device = Config;
                }
                this.Finish();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnSave_Click at Device");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        public override void OnBackPressed()
        {
            try
            {
                if (!flagdatachange)
                {
                    DialogCheckBack(); return;
                }

                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.edit_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("fromPage", "device");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }
        private void CheckDataChange()
        {
            if (textComment.Text != comments)
            {
                flagdatachange = true;
            }
        }
        public void DialogCheckBack()
        {
            base.OnBackPressed();
            flagdatachange = false;
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

