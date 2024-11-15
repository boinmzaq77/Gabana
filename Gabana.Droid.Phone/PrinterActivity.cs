using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Theme = "@style/AppTheme.Main", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@mipmap/GabanaLogIn", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class PrinterActivity : AppCompatActivity
    {
        Spinner spinTypeConnect, spinTypePaper, spinTypePrint, spinTypeCommand;
        string TypeConnect = "", TypePaper = string.Empty, TypePrint = string.Empty, TypeCommand = string.Empty;
        LinearLayout lnBluetooth, lnWifi, lnSave, lnBack;
        FrameLayout lnShowSave;
        PrinterActivity activity;
        List<Bluetooth> Bluetooths;
        EditText textIP, textPort, textPrintMode, textCodePage;
        private static string ipaddress, port;
        Button buttonSave;
        bool first = true;
        private bool firstSpinerPaper = true,firstSpinerPrint = true , firstSpinerTypePrint = true, firstSpinerTypeCommand = true;
        FrameLayout lnTypeConnect, lntypePaper, lntypeTypeCommand, lntypePrint;
        ImageButton btnTypeConnect, btnTypePaper , btnTypeCommand , btnTypePrint, imgbtnTestPrint;


        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.printer_activity_main);
                activity = this;
                spinTypeConnect = FindViewById<Spinner>(Resource.Id.spinTypeConnect);
                spinTypePaper = FindViewById<Spinner>(Resource.Id.spinTypePaper);
                spinTypePrint = FindViewById<Spinner>(Resource.Id.spinTypePrint);
                spinTypeCommand = FindViewById<Spinner>(Resource.Id.spinTypePrinterName);
                lnBluetooth = FindViewById<LinearLayout>(Resource.Id.lnBluetooth);
                lnWifi = FindViewById<LinearLayout>(Resource.Id.lnWifi);
                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                lnShowSave = FindViewById<FrameLayout>(Resource.Id.lnShowSave);

                lnSave = FindViewById<LinearLayout>(Resource.Id.lnSave);
                buttonSave = FindViewById<Button>(Resource.Id.buttonSave);
                textIP = FindViewById<EditText>(Resource.Id.textIP);
                textPort = FindViewById<EditText>(Resource.Id.textPort);

                textPrintMode = FindViewById<EditText>(Resource.Id.textPrintMode);
                textCodePage = FindViewById<EditText>(Resource.Id.textCodePage);

                spinTypeConnect.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinTypeConnect_ItemSelected);
                var adapterConnect = ArrayAdapter.CreateFromResource(this, Resource.Array.spinConnect, Resource.Layout.spinner_item);
                adapterConnect.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinTypeConnect.Adapter = adapterConnect;

                spinTypePaper.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinTypePaper_ItemSelected);
                var adapterPaper = ArrayAdapter.CreateFromResource(this, Resource.Array.spinPaper, Resource.Layout.spinner_item);
                adapterPaper.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinTypePaper.Adapter = adapterPaper;

                spinTypePrint.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinTypePrint_ItemSelected);
                var adapterPrint = ArrayAdapter.CreateFromResource(this, Resource.Array.spinPrintType, Resource.Layout.spinner_item);
                adapterPrint.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinTypePrint.Adapter = adapterPrint;

                spinTypeCommand.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinTypeCommand_ItemSelected);
                var adapterTypeCommand = ArrayAdapter.CreateFromResource(this, Resource.Array.spinPrinterCommand, Resource.Layout.spinner_item);
                adapterTypeCommand.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinTypeCommand.Adapter = adapterTypeCommand;

                CheckJwt();

                lnTypeConnect = FindViewById<FrameLayout>(Resource.Id.lnTypeConnect);
                btnTypeConnect = FindViewById<ImageButton>(Resource.Id.btnTypeConnect);
                lnTypeConnect.Click += LnTypeConnect_Click;
                btnTypeConnect.Click += LnTypeConnect_Click;

                btnTypePaper = FindViewById<ImageButton>(Resource.Id.btnTypePaper);
                lntypePaper = FindViewById<FrameLayout>(Resource.Id.lntypePaper);
                btnTypePaper.Click += BtnTypePaper_Click;
                lntypePaper.Click += BtnTypePaper_Click;
                 
                btnTypeCommand = FindViewById<ImageButton>(Resource.Id.btnTypePrinterName);
                lntypeTypeCommand = FindViewById<FrameLayout>(Resource.Id.lntypePrinterName);
                btnTypeCommand.Click += BtnTypePrinterName_Click;
                lntypeTypeCommand.Click += BtnTypePrinterName_Click;

                btnTypePrint = FindViewById<ImageButton>(Resource.Id.btnTypePrint);
                lntypePrint = FindViewById<FrameLayout>(Resource.Id.lntypePrint);
                btnTypePrint.Click += BtnTypePrint_Click;
                lntypePrint.Click += BtnTypePrint_Click;

                imgbtnTestPrint = FindViewById<ImageButton>(Resource.Id.btnTypePrint);
                imgbtnTestPrint.Click += ImgbtnTestPrint_Click;    

                var perSetting = Preferences.Get("Setting", "");  
                if (!string.IsNullOrEmpty(perSetting))
                {
                    var settingPrinter = JsonConvert.DeserializeObject<SettingPrinter>(perSetting);
                    DataCashingAll.setting = settingPrinter;

                    if (DataCashingAll.setting.TYPE == "Wifi")
                    {
                        spinTypeConnect.SetSelection(0);
                    }
                    else
                    {
                        spinTypeConnect.SetSelection(1);
                    }

                    if (DataCashingAll.setting.TYPEPAGE == "58mm" || DataCashingAll.setting.TYPEPAGE == "58 มม.")
                    {
                        spinTypePaper.SetSelection(0);
                    }
                    else
                    {
                        spinTypePaper.SetSelection(1);
                    }
                    textIP.Text = DataCashingAll.setting.IPADDRESS?.ToString();
                    textPort.Text = DataCashingAll.setting.PORTNUMBER?.ToString();
                    TypeConnect = DataCashingAll.setting.TYPE;
                    TypePrint = DataCashingAll.setting.PRINTTYPE;
                    TypeCommand = DataCashingAll.setting.COMMAND;

                    if (DataCashingAll.setting.PRINTTYPE == "Image")
                    {
                        spinTypePrint.SetSelection(0);
                    }
                    else
                    {
                        spinTypePrint.SetSelection(1);
                    }

                    if (DataCashingAll.setting.COMMAND == "Epson Command")
                    {
                        spinTypeCommand.SetSelection(0);
                    }
                    else
                    {
                        spinTypeCommand.SetSelection(1);
                    }
                }
                else
                {
                    DataCashingAll.setting = new SettingPrinter()
                    {
                        TYPE = "Wifi",
                        PORTNUMBER = "9100",
                        IPADDRESS = "192.168.200.182",
                        USE = "Wifi",
                        TYPEPAGE = "58mm",
                        PRINTTYPE = "Image",
                        COMMAND = "Epson Command",
                    };
                }


                textIP.TextChanged += TextIP_TextChanged;
                textPort.TextChanged += TextPort_TextChanged;

                SetShowDataConnect();
                lnSave.Click += LnSave_Click;
                buttonSave.Click += LnSave_Click;
                lnShowSave.Click += LnSave_Click;

                _ = TinyInsights.TrackPageViewAsync("OnCreate : PrinterActivity");


                if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                {
                    CheckPermissionAdriod12();
                }
                else
                {
                    CheckPermission();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Printer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void ImgbtnTestPrint_Click(object sender, EventArgs e)
        {
            try
            {
                var perSetting = Preferences.Get("Setting", "");
                if (string.IsNullOrEmpty(perSetting))
                {
                    Toast.MakeText(this, GetString(Resource.String.settingprinter), ToastLength.Short).Show();
                    return;
                }

                var settingPrinter = JsonConvert.DeserializeObject<SettingPrinter>(perSetting);
                DataCashingAll.setting = settingPrinter;
                Utils.TestPrint(this.Assets, this);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnTypePrint_Click(object sender, EventArgs e)
        {
            spinTypePrint.PerformClick();
        }

        private void BtnTypePrinterName_Click(object sender, EventArgs e)
        {
            spinTypeCommand.PerformClick();
        }

        private void SpinTypeCommand_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            string select = spinTypeCommand.SelectedItem.ToString();
            if (select == "Epson Command")
            {
                TypeCommand = "Epson Command";
            }
            if (select == "Winpal Command")
            {
                TypeCommand = "Winpal Command";
            }

            if (!firstSpinerTypeCommand)
            {
                DataCashingAll.setting.COMMAND = TypeCommand;
                var setting = JsonConvert.SerializeObject(DataCashingAll.setting);
                Preferences.Set("Setting", setting);
            }
            firstSpinerTypeCommand = false;

            SetShowDataConnect();
        }

        public bool CheckPermission()
        {
            Permission permissionCamera = CheckSelfPermission(Manifest.Permission.Camera);
            Permission permissionRead = CheckSelfPermission(Manifest.Permission.ReadExternalStorage);
            Permission permissionWrite = CheckSelfPermission(Manifest.Permission.WriteExternalStorage);
            Permission permissionBluetooth = CheckSelfPermission(Manifest.Permission.Bluetooth);
            Permission permissionBluetoothC = CheckSelfPermission(Manifest.Permission.BluetoothConnect);

            string[] PERMISSIONS;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                PERMISSIONS = new string[]
                {
                    "android.permission.READ_EXTERNAL_STORAGE",
                    "android.permission.WRITE_EXTERNAL_STORAGE",
                    "android.permission.CAMERA",
                    "android.permission.BLUETOOTH",
                    "android.permission.BLUETOOTH_CONNECT"
                };
                if (permissionCamera != Permission.Granted
                    || permissionRead != Permission.Granted
                    || permissionWrite != Permission.Granted
                    || permissionBluetooth != Permission.Granted
                    || permissionBluetoothC != Permission.Granted)
                {
                    RequestPermissions(PERMISSIONS, 1);
                    return false;
                }
                return true;
            }
            else
            {
                PERMISSIONS = new string[]
               {
                    "android.permission.READ_EXTERNAL_STORAGE",
                    "android.permission.WRITE_EXTERNAL_STORAGE",
                    "android.permission.CAMERA",
                    "android.permission.BLUETOOTH"
                };
                if (permissionCamera != Permission.Granted || permissionRead != Permission.Granted || permissionWrite != Permission.Granted || permissionBluetooth != Permission.Granted)
                {
                    RequestPermissions(PERMISSIONS, 1);
                    return false;
                }
                return true;
            }
        }


        public bool CheckPermissionAdriod12()
        {
            try
            {
                Permission permissionCamera = CheckSelfPermission(Manifest.Permission.Camera);
                Permission permissionRead = CheckSelfPermission(Manifest.Permission.ReadExternalStorage);
                Permission permissionWrite = CheckSelfPermission(Manifest.Permission.WriteExternalStorage);
                Permission permissionBluetooth = CheckSelfPermission(Manifest.Permission.Bluetooth);
                Permission permissionBluetoothC = CheckSelfPermission(Manifest.Permission.BluetoothConnect);


                string[] PERMISSIONS =
                    {
                    "android.permission.READ_EXTERNAL_STORAGE",
                    "android.permission.WRITE_EXTERNAL_STORAGE",
                    "android.permission.CAMERA",
                    "android.permission.BLUETOOTH",
                    "android.permission.BLUETOOTH_CONNECT"

                };
                if (
                    permissionCamera != Permission.Granted ||
                    permissionRead != Permission.Granted ||
                    permissionWrite != Permission.Granted ||
                    permissionBluetooth != Permission.Granted ||
                    permissionBluetoothC != Permission.Granted
                    )
                {
                    RequestPermissions(PERMISSIONS, 1);

                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckPermission at Printer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        private void BtnTypePaper_Click(object sender, EventArgs e)
        {
            spinTypePaper.PerformClick();
        }

        private void LnTypeConnect_Click(object sender, EventArgs e)
        {
            spinTypeConnect.PerformClick();
        }

        private void TextPort_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (DataCashingAll.setting.PORTNUMBER == textPort.Text)
            {
                buttonSave.SetBackgroundResource(Resource.Drawable.btnborderblue);
                buttonSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            else
            {
                buttonSave.SetBackgroundResource(Resource.Drawable.btnblue);
                buttonSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
        }

        private void TextIP_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {

            if (DataCashingAll.setting.IPADDRESS == textIP.Text)
            {
                buttonSave.SetBackgroundResource(Resource.Drawable.btnborderblue);
                buttonSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            else
            {
                buttonSave.SetBackgroundResource(Resource.Drawable.btnblue);
                buttonSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        private void SetShowDataConnect()
        {
            try
            {
                if (string.IsNullOrEmpty(TypeConnect))
                {
                    return;
                }
                if (TypeConnect == "Wifi")
                {
                    lnBluetooth.Visibility = ViewStates.Gone;
                    lnWifi.Visibility = ViewStates.Visible;
                    lnShowSave.Visibility = ViewStates.Visible;
                }
                if (TypeConnect == "Bluetooth" || TypeConnect == "บลูทูท")
                {
                    SetBluetooth();

                    lnBluetooth.Visibility = ViewStates.Visible;
                    lnWifi.Visibility = ViewStates.Gone;
                    lnShowSave.Visibility = ViewStates.Gone;
                }

                textIP.Text = DataCashingAll.setting.IPADDRESS;
                textPort.Text = DataCashingAll.setting.PORTNUMBER;
                TypePrint = DataCashingAll.setting.PRINTTYPE;
                TypeCommand = DataCashingAll.setting.COMMAND;

                if (DataCashingAll.setting.TYPE == TypeConnect &&
                    DataCashingAll.setting.IPADDRESS == textIP.Text &&
                    DataCashingAll.setting.PORTNUMBER == textPort.Text &&
                    DataCashingAll.setting.PRINTTYPE == TypePrint &&
                    DataCashingAll.setting.COMMAND == TypeCommand)
                {
                    buttonSave.SetBackgroundResource(Resource.Drawable.btnborderblue);
                    buttonSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                }
                else
                {
                    buttonSave.SetBackgroundResource(Resource.Drawable.btnblue);
                    buttonSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("at SetShowDataConnect");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }

        }

        private void LnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textIP.Text))
                {
                    textIP.RequestFocus();
                    Toast.MakeText(this, "กรุณากรอกข้อมูล IP Adress", ToastLength.Short).Show();
                    return;
                }
                if (string.IsNullOrEmpty(textPort.Text))
                {
                    textPort.RequestFocus();
                    Toast.MakeText(this, "กรุณากรอกข้อมูลการเชื่อมต่อ", ToastLength.Short).Show();
                    return;
                }

                ipaddress = textIP.Text?.ToString();
                port = textPort.Text?.ToString();
                //DataCashingAll.setting.IPADDRESS = ipaddress;
                //DataCashingAll.setting.PORTNUMBER = port;
                //DataCashingAll.setting.PRINTTYPE = TypePrint;
                //DataCashingAll.setting.COMMAND = TypeCommand ;

                var settingPrinter = new SettingPrinter()
                {
                    TYPE = TypeConnect,
                    PORTNUMBER = port,
                    IPADDRESS = ipaddress,
                    USE = TypeConnect,
                    TYPEPAGE = TypePaper,
                    PRINTTYPE = TypePrint,
                    COMMAND = TypeCommand,
                };

                DataCashingAll.setting.TYPE = settingPrinter.TYPE;
                DataCashingAll.setting.PORTNUMBER = settingPrinter.PORTNUMBER;
                DataCashingAll.setting.IPADDRESS = settingPrinter.IPADDRESS;
                DataCashingAll.setting.USE = settingPrinter.USE;
                DataCashingAll.setting.TYPEPAGE = settingPrinter.TYPEPAGE;
                DataCashingAll.setting.PRINTTYPE = settingPrinter.PRINTTYPE;
                DataCashingAll.setting.COMMAND = settingPrinter.COMMAND;
                var setting = JsonConvert.SerializeObject(DataCashingAll.setting);
                Preferences.Set("Setting", setting);

                Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                this.Finish();
            }
            catch (Exception)
            {
                return;
            }
        }

        private void SetBluetooth()
        {
            try
            {
                Bluetooths = new List<Bluetooth>();
#pragma warning disable CS0618 // Type or member is obsolete
                BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
#pragma warning restore CS0618 // Type or member is obsolete
                if (adapter == null)
                {
                    return;
                }
                List<BluetoothDevice> listdevices = adapter.BondedDevices.ToList();
                var list = listdevices.Select(x => x.Name).ToList();

                foreach (var item in listdevices)
                {
                    var name = item.Name.ToString();
                    if (item.Address == "00:11:22:33:44:55")
                    {
                        Bluetooths.Add(new Bluetooth() { BluetoothName = name, id = "00001101-0000-1000-8000-00805F9B34FB", Address = item.Address });
                    }
                    else
                    {
                        Android.OS.ParcelUuid uuid;
                        if (item.GetUuids() == null)
                        {
                            Bluetooths.Add(new Bluetooth() { BluetoothName = name, id = "00001101-0000-1000-8000-00805F9B34FB", Address = item.Address });
                        }
                        else
                        {
                            uuid = item.GetUuids().ElementAt(0);
                            Bluetooths.Add(new Bluetooth() { BluetoothName = name, id = uuid.Uuid?.ToString(), Address = item.Address });
                        }
                    }
                };

                RecyclerView recyclerview_listbluetooth = FindViewById<RecyclerView>(Resource.Id.recyclerview_listBluetooth);

                GridLayoutManager gridLayout = new GridLayoutManager(this, 1, 1, false);
                Printer_Adapter_Main adapterBluetooth = new Printer_Adapter_Main(Bluetooths);

                recyclerview_listbluetooth.SetLayoutManager(gridLayout);
                recyclerview_listbluetooth.HasFixedSize = true;
                recyclerview_listbluetooth.SetItemViewCacheSize(20);
                recyclerview_listbluetooth.SetAdapter(adapterBluetooth);
                adapterBluetooth.ItemClick += AdapterBluetooth_ItemClick;

            }
            catch (Exception)
            {
                return;
            }

        }

        private void AdapterBluetooth_ItemClick(object sender, int e)
        {
            try
            {
                var name = Bluetooths[e].BluetoothName;
                if (name == DataCashingAll.setting.BLUETOOTH1 && DataCashingAll.setting.IPADDRESS == Bluetooths[e].Address)
                {
                    DataCashingAll.setting.BLUETOOTH1 = "";
                    DataCashingAll.setting.IPADDRESS = "";
                }
                else
                {
                    DataCashingAll.setting.BLUETOOTH1 = name;
                    DataCashingAll.setting.IPADDRESS = Bluetooths[e].Address;
                }
                DataCashingAll.setting.TYPE = TypeConnect;
                DataCashingAll.setting.USE = TypeConnect;

                var setting = JsonConvert.SerializeObject(DataCashingAll.setting);
                Preferences.Set("Setting", setting);
                SetBluetooth();
            }
            catch (Exception)
            {
                return;
            }

        }

        private void SpinTypePrint_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            string select = spinTypePrint.SelectedItem.ToString();
            if (select == "Image")
            {
                TypePrint = "Image";
            }
            if (select == "Text")
            {
                TypePrint = "Text";
            }

            if (!firstSpinerTypePrint)
            {
                DataCashingAll.setting.PRINTTYPE = TypePrint;
                var setting = JsonConvert.SerializeObject(DataCashingAll.setting);
                Preferences.Set("Setting", setting);
            }
            firstSpinerTypePrint = false;

            SetShowDataConnect();

        }
        private void SpinTypePaper_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            string select = spinTypePaper.SelectedItem.ToString();
            if (select == "58mm" || select == "58 มม.")
            {
                TypePaper = "58mm";
            }
            if (select == "80mm" || select == "80 มม.")
            {
                TypePaper = "80mm";
            }

            if (!firstSpinerPaper)
            {
                DataCashingAll.setting.TYPEPAGE = TypePaper;
                var setting = JsonConvert.SerializeObject(DataCashingAll.setting);
                Preferences.Set("Setting", setting);
            }
            firstSpinerPaper = false;

            SetShowDataConnect();

        }

        private void SpinTypeConnect_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            string select = spinTypeConnect.SelectedItem.ToString();
            if (select == "Wifi")
            {
                TypeConnect = "Wifi";

                var perSetting = Preferences.Get("Setting", "");
                if (!string.IsNullOrEmpty(perSetting))
                {
                    var settingPrinter = JsonConvert.DeserializeObject<SettingPrinter>(perSetting);
                    DataCashingAll.setting = settingPrinter;
                }
                else
                {
                    DataCashingAll.setting = new SettingPrinter()
                    {
                        TYPE = "Wifi",
                        PORTNUMBER = "9100",
                        IPADDRESS = "192.168.200.182",
                        USE = "Wifi",
                        TYPEPAGE = "58mm",
                        PRINTTYPE = "Image",
                        COMMAND = "Epson Command",
                    };
                }
                textIP.Text = DataCashingAll.setting.IPADDRESS?.ToString();
                textPort.Text = DataCashingAll.setting.PORTNUMBER?.ToString();
                TypeConnect = "Wifi";
                TypePrint = DataCashingAll.setting.PRINTTYPE;
                TypeCommand = DataCashingAll.setting.COMMAND;
                TypePaper = DataCashingAll.setting.TYPEPAGE;
            }
            else
            {
                TypeConnect = "Bluetooth";
            }

            if (!first)
            {
                DataCashingAll.setting.USE = TypeConnect;
                DataCashingAll.setting.TYPE = TypeConnect;
                //DataCashingAll.setting.IPADDRESS = "";
                //DataCashingAll.setting.PORTNUMBER = "";

                var setting = JsonConvert.SerializeObject(DataCashingAll.setting);
                Preferences.Set("Setting", setting);
            }
            first = false;

            SetShowDataConnect();

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

            CheckJwt();
        }
    }

}

