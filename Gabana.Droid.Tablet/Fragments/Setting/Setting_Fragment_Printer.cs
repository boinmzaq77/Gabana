using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Setting;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.ClassStructure;
using Gabana.ShareSource.Manage;
using Java.Security;
using Java.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;
using static LinqToDB.Reflection.Methods.LinqToDB.Insert;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_Printer : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Setting_Fragment_Printer NewInstance()
        {
            Setting_Fragment_Printer frag = new Setting_Fragment_Printer();
            return frag;
        }
        public static Setting_Fragment_Printer fragment_printer;
        View view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_printer, container, false);
            try
            {
                CheckJwt();
                CombineUI();
                SetEventUI();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : Setting_Fragment_Printer");
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

                SetValue();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());   
            }
        }

        private void SetValue()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }

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

                SetShowDataConnect();

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at Setting_Fragment_Printer");
            }
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
                _ = TinyInsights.TrackPageViewAsync("CheckJwt at Setting_Fragment_Printer");
            }
        }

        LinearLayout lnBack, lnWifi, lnBluetooth;
        Spinner spinTypeConnect, spinTypePaper, spinTypePrint, spinTypeCommand;
        FrameLayout lnShowSave;
        EditText textIP, textPort;
        RecyclerView rcvlistBluetooth;
        Button btnSave;
        ImageButton ImgbtnTestPrint;
        string TypeConnect = "", TypePaper = string.Empty, TypePrint = string.Empty, TypeCommand = string.Empty;

        bool first = true;
        private bool firstSpinerPaper = true, firstSpinerPrint = true, firstSpinerTypePrint = true, firstSpinerTypeCommand = true;
        FrameLayout lnTypeConnect, lntypePaper, lntypeTypeCommand, lntypePrint;
        ImageButton btnTypeConnect, btnTypePaper, btnTypeCommand, btnTypePrint;

        private void CombineUI()
        {
            lnShowSave = view.FindViewById<FrameLayout>(Resource.Id.lnShowSave);
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            spinTypeConnect = view.FindViewById<Spinner>(Resource.Id.spinTypeConnect);
            lnTypeConnect = view.FindViewById<FrameLayout>(Resource.Id.lnTypeConnect);
            spinTypePaper = view.FindViewById<Spinner>(Resource.Id.spinTypePaper);
            lntypePaper = view.FindViewById<FrameLayout>(Resource.Id.lntypePaper);
            lnWifi = view.FindViewById<LinearLayout>(Resource.Id.lnWifi);
            textIP = view.FindViewById<EditText>(Resource.Id.textIP);
            textPort = view.FindViewById<EditText>(Resource.Id.textPort);
            lnBluetooth = view.FindViewById<LinearLayout>(Resource.Id.lnBluetooth);
            rcvlistBluetooth = view.FindViewById<RecyclerView>(Resource.Id.rcvlistBluetooth);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
            spinTypePrint = view.FindViewById<Spinner>(Resource.Id.spinTypePrint);
            spinTypeCommand = view.FindViewById<Spinner>(Resource.Id.spinTypeCommand);
            lnTypeConnect = view.FindViewById<FrameLayout>(Resource.Id.lnTypeConnect);
            btnTypeConnect = view.FindViewById<ImageButton>(Resource.Id.btnTypeConnect); 
            btnTypePaper = view.FindViewById<ImageButton>(Resource.Id.btnTypePaper);
            lntypePaper = view.FindViewById<FrameLayout>(Resource.Id.lntypePaper);       
            btnTypeCommand = view.FindViewById<ImageButton>(Resource.Id.btnTypeCommand);
            lntypeTypeCommand = view.FindViewById<FrameLayout>(Resource.Id.lntypeTypeCommand);
            btnTypePrint = view.FindViewById<ImageButton>(Resource.Id.btnTypePrint);
            lntypePrint = view.FindViewById<FrameLayout>(Resource.Id.lntypePrint);
            ImgbtnTestPrint = view.FindViewById<ImageButton>(Resource.Id.imgbtnTestPrint);
        }

        private void SetEventUI()
        {
            lnBack.Click += LnBack_Click;
            btnSave.Click += LnSave_Click;
            lnTypeConnect.Click += LnTypeConnect_Click;
            btnTypeConnect.Click += LnTypeConnect_Click;
            btnTypePaper.Click += BtnTypePaper_Click;
            lntypePaper.Click += BtnTypePaper_Click;
            btnTypeCommand.Click += BtnTypeCommand_Click;
            lntypeTypeCommand.Click += BtnTypeCommand_Click;
            btnTypePrint.Click += BtnTypePrint_Click;
            lntypePrint.Click += BtnTypePrint_Click;
            textIP.TextChanged += TextIP_TextChanged;
            textPort.TextChanged += TextPort_TextChanged;
            ImgbtnTestPrint.Click += ImgbtnTestPrint_Click;

            spinTypeConnect.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinTypeConnect_ItemSelected);
            var adapterConnect = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.spinConnect, Resource.Layout.spinner_item);
            adapterConnect.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinTypeConnect.Adapter = adapterConnect;

            spinTypePaper.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinTypePaper_ItemSelected);
            var adapterPaper = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.spinPaper, Resource.Layout.spinner_item);
            adapterPaper.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinTypePaper.Adapter = adapterPaper;

            spinTypePrint.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinTypePrint_ItemSelected);
            var adapterPrint = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.spinPrintType, Resource.Layout.spinner_item);
            adapterPrint.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinTypePrint.Adapter = adapterPrint;

            spinTypeCommand.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinTypeCommand_ItemSelected);
            var adapterTypeCommand = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.spinPrinterCommand, Resource.Layout.spinner_item);
            adapterTypeCommand.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinTypeCommand.Adapter = adapterTypeCommand;
        }

        private void ImgbtnTestPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.TestPrint(this.Activity.Assets,this.Activity);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        

        private void TextPort_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (DataCashingAll.setting.PORTNUMBER == textPort.Text)
            {
                btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else
            {
                btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
        }

        private void TextIP_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (DataCashingAll.setting.IPADDRESS == textIP.Text)
            {
                btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else
            {
                btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
        }

        private static string ipaddress, port;
        private void LnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textIP.Text))
                {
                    textIP.RequestFocus();
                    Toast.MakeText(this.Activity, "กรุณากรอกข้อมูล IP Adress", ToastLength.Short).Show();
                    return;
                }
                if (string.IsNullOrEmpty(textPort.Text))
                {
                    textPort.RequestFocus();
                    Toast.MakeText(this.Activity, "กรุณากรอกข้อมูลการเชื่อมต่อ", ToastLength.Short).Show();
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

                Toast.MakeText(this.Activity, GetString(Resource.String.savesucess), ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnTypePrint_Click(object sender, EventArgs e)
        {
            spinTypePrint.PerformClick();
        }
        private void BtnTypeCommand_Click(object sender, EventArgs e)
        {
            spinTypeCommand.PerformClick();
        }
        private void BtnTypePaper_Click(object sender, EventArgs e)
        {
            spinTypePaper.PerformClick();
        }
        private void LnTypeConnect_Click(object sender, EventArgs e)
        {
            spinTypeConnect.PerformClick();
        }

        private async void LnBack_Click(object sender, EventArgs e)
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
        }

        private void SpinTypeConnect_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SpinTypeCommand_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SpinTypePrint_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SpinTypePaper_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
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
                    btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                    btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                }
                else
                {
                    btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                    btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primary, null));
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("at SetShowDataConnect");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }

        }
        List<Bluetooth> Bluetooths = new List<Bluetooth>();

        private void SetBluetooth()
        {
            try
            {
                Bluetooths = new List<Bluetooth>();
                BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
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


                GridLayoutManager gridLayout = new GridLayoutManager(this.Activity, 1, 1, false);
                Setting_Adapter_Printer adapterBluetooth = new Setting_Adapter_Printer(Bluetooths);

                rcvlistBluetooth.SetLayoutManager(gridLayout);
                rcvlistBluetooth.HasFixedSize = true;
                rcvlistBluetooth.SetItemViewCacheSize(20);
                rcvlistBluetooth.SetAdapter(adapterBluetooth);
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

    }
}