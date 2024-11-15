using AutoMapper;
using CoreBluetooth;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using Plugin.BLE;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class PrinterSettingController : UIViewController
    {
        UIScrollView _scrollView;
        UIView _contentView;
        public UIView LinkTypeView, PaperTypeView , BluetoothView , WifiView , Viewline;
        UILabel lblLinkType, lblPaperType , lblWifi , lblBluetooth , lblip , lblport;
        UIImageView btnLinkType, btnPaperType;
        UITextField txtLinkType, txtPaperType , txtip , txtport;
        UIButton btnAddDummy, btnTestPrint;  
        List<Bluetooth2> Bluetooths = new List<Bluetooth2>();
        public static List<Plugin.BLE.Abstractions.Contracts.IDevice> devices = new List<Plugin.BLE.Abstractions.Contracts.IDevice>();
        UICollectionView BluethoothListCollection;
        private UINavigationItem naviitem;
        private UIBarButtonItem selectCustomer;
        private UIView TypePrintView;
        private UILabel lblTypePrint;
        private UITextField txtTypePrint;
        private UIImageView btnTypePrint;

        public PrinterSettingController() 
        {

        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            try
            {
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                this.NavigationController.SetNavigationBarHidden(false, false);

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
        }
        public override async void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            base.ViewDidLoad();
            try
            {
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

                naviitem = this.NavigationItem;
                initAttribute();
                Textboxfocus(View);
                SetupAutoLayout();
                SetupPicker();
                var ble = CrossBluetoothLE.Current;
                var adapter = CrossBluetoothLE.Current.Adapter;
                adapter.ScanTimeout = 10000;
                adapter.ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode.LowPower;
                Bluetooths = new List<Bluetooth2>();
                adapter.DeviceDiscovered += (obj, a) =>
                {
                    //Bluetooths = new List<Bluetooth2>();
                    //GabanaLoading.SharedInstance.Show(this.NavigationController);
                    Guid aa = a.Device.Id;
                    var x = new Bluetooth2() { BluetoothName = a.Device.Name, BluetoothStatus = "off", id = a.Device.Id };
                    if (!Bluetooths.Contains(x) && !String.IsNullOrEmpty(a.Device.Name))
                    {
                        Bluetooths.Add(x);
                    }
                    if (DataCashingAll.setting.BLUETOOTH1 != null)
                    {
                        var index = Bluetooths.FindIndex(x => x.id == Guid.Parse(DataCashingAll.setting.BLUETOOTH1));
                        if (index != -1)
                        {
                            Bluetooths[index].BluetoothStatus = "on";
                        }
                    }

                    ((PrinterDataSource)BluethoothListCollection.DataSource).ReloadData(Bluetooths);
                    BluethoothListCollection.ReloadData();
                    //GabanaLoading.SharedInstance.Hide();
                };
                selectCustomer = new UIBarButtonItem();
                selectCustomer.Image = UIImage.FromBundle("Search");
                selectCustomer.Clicked += async (sender, e) => {
                    Bluetooths = new List<Bluetooth2>();
                    await adapter.StartScanningForDevicesAsync();
                };
                //naviitem.RightBarButtonItem = selectCustomer;
                naviitem.SetRightBarButtonItem(selectCustomer, false);
                //
                //naviitem.RightBarButtonItem.Hidden = true;

                if (DataCashingAll.setting.TYPE == "Wifi")
                {
                    
                    WifiView.Hidden = false;
                    BluetoothView.Hidden = true;
                    txtLinkType.Text = "Wifi";
                    ((UIPickerView)txtLinkType.InputView).Select(1,0,false);
                }
                else
                {
                    //Bluetooths = new List<Bluetooth2>();
                    //await adapter.StartScanningForDevicesAsync();
                    WifiView.Hidden = true;
                    BluetoothView.Hidden = false;
                    txtLinkType.Text = "Bluetooth";
                    ((UIPickerView)txtLinkType.InputView).Select(0, 0, false);

                }
                if (DataCashingAll.PrintType == "Image")
                {
                    txtTypePrint.Text = "Image";
                    ((UIPickerView)txtTypePrint.InputView).Select(0, 0, false);
                }
                else
                {
                    txtTypePrint.Text = "Text";
                    ((UIPickerView)txtTypePrint.InputView).Select(1, 0, false);
                }
                if (DataCashingAll.setting.TYPEPAGE == "58mm")
                {
                    txtPaperType.Text = "58 mm.";
                    ((UIPickerView)txtPaperType.InputView).Select(0, 0, false);
                }
                else
                {
                    txtPaperType.Text = "80 mm.";
                    ((UIPickerView)txtPaperType.InputView).Select(1, 0, false);
                }

                txtip.Text = DataCashingAll.setting.IPADDRESS;
                txtport.Text = DataCashingAll.setting.PORTNUMBER;

                if (DataCashingAll.setting.TYPE != "Wifi")
                {
                    Bluetooths = new List<Bluetooth2>();
                    await adapter.StartScanningForDevicesAsync();
                }

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        void initAttribute()
        {
            //_scrollView = new UIScrollView();
            //_scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            //_scrollView.BackgroundColor = UIColor.FromRGB(248,248,248);

            //_contentView = new UIView();
            //_contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            //_contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region LinkTypeView
            LinkTypeView = new UIView();
            LinkTypeView.BackgroundColor = UIColor.White;
            LinkTypeView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblLinkType = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblLinkType.Font = lblLinkType.Font.WithSize(15);
            lblLinkType.Text = Utils.TextBundle("connecttype", "Connect type");
            LinkTypeView.AddSubview(lblLinkType);

            txtLinkType = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtLinkType.ReturnKeyType = UIReturnKeyType.Next;
            txtLinkType.ShouldReturn = (tf) =>
            {
                txtPaperType.BecomeFirstResponder();
                return true;
            };
            txtLinkType.EditingChanged += (object sender, EventArgs e) =>
            {

            };
            txtLinkType.AttributedPlaceholder = new NSAttributedString("Bluetooth", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtLinkType.Font = txtLinkType.Font.WithSize(15);
            LinkTypeView.AddSubview(txtLinkType);

            btnLinkType = new UIImageView();
            btnLinkType.Image = UIImage.FromBundle("Next");
            btnLinkType.TranslatesAutoresizingMaskIntoConstraints = false;
            LinkTypeView.AddSubview(btnLinkType);

            btnLinkType.UserInteractionEnabled = true;
            var tapGestureLinkType = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("LinkType:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnLinkType.AddGestureRecognizer(tapGestureLinkType);
            #endregion

            #region PaperTypeView
            PaperTypeView = new UIView();
            PaperTypeView.BackgroundColor = UIColor.White;
            PaperTypeView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblPaperType = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblPaperType.Font = lblPaperType.Font.WithSize(15);
            lblPaperType.Text = Utils.TextBundle("pagetype", "Page type");
            PaperTypeView.AddSubview(lblPaperType);

            txtPaperType = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtPaperType.ReturnKeyType = UIReturnKeyType.Next;
            txtPaperType.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtPaperType.EditingChanged += (object sender, EventArgs e) =>
            {

            };
            txtPaperType.AttributedPlaceholder = new NSAttributedString("58 mm.", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtPaperType.Font = txtPaperType.Font.WithSize(15);
            PaperTypeView.AddSubview(txtPaperType);

            btnPaperType = new UIImageView();
            btnPaperType.Image = UIImage.FromBundle("Next");
            btnPaperType.TranslatesAutoresizingMaskIntoConstraints = false;
            PaperTypeView.AddSubview(btnPaperType);

            btnPaperType.UserInteractionEnabled = true;
            var tapGesturePaperType = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("PaperType:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnPaperType.AddGestureRecognizer(tapGesturePaperType);
            #endregion

            #region TypePrintView
            TypePrintView = new UIView();
            TypePrintView.BackgroundColor = UIColor.White;
            TypePrintView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblTypePrint = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblTypePrint.Font = lblTypePrint.Font.WithSize(15);
            lblTypePrint.Text = Utils.TextBundle("pagetype", "Page type");
            TypePrintView.AddSubview(lblTypePrint);

            txtTypePrint = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtTypePrint.ReturnKeyType = UIReturnKeyType.Next;
            txtTypePrint.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtTypePrint.EditingChanged += (object sender, EventArgs e) =>
            {

            };
            txtTypePrint.AttributedPlaceholder = new NSAttributedString("58 mm.", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtTypePrint.Font = txtTypePrint.Font.WithSize(15);
            TypePrintView.AddSubview(txtTypePrint);

            btnTypePrint = new UIImageView();
            btnTypePrint.Image = UIImage.FromBundle("Next");
            btnTypePrint.TranslatesAutoresizingMaskIntoConstraints = false;
            TypePrintView.AddSubview(btnTypePrint);

            btnTypePrint.UserInteractionEnabled = true;
            var tapGestureTypePrint = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("TypePrint:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnTypePrint.AddGestureRecognizer(tapGestureTypePrint);
            #endregion
            #region BluetoothView
            BluetoothView = new UIView();
            BluetoothView.BackgroundColor = UIColor.White;
            BluetoothView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblBluetooth = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblBluetooth.Font = lblLinkType.Font.WithSize(15);
            lblBluetooth.Text = "Bluetooth List";
            BluetoothView.AddSubview(lblBluetooth);

            #endregion

            #region WifiView
            WifiView = new UIView();
            WifiView.BackgroundColor = UIColor.White;
            WifiView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblWifi = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblWifi.Font = lblLinkType.Font.WithSize(15);
            lblWifi.Text = "Wifi";
            WifiView.AddSubview(lblWifi);
            lblip = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblip.Font = lblLinkType.Font.WithSize(15);
            lblip.Text = "IP Address";
            lblWifi.AddSubview(lblip);

            txtip = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtip.ReturnKeyType = UIReturnKeyType.Next;
            txtip.ShouldReturn = (tf) =>
            {
                txtPaperType.BecomeFirstResponder();
                return true;
            };

            txtip.AttributedPlaceholder = new NSAttributedString("192.168.200.1", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtip.Font = txtip.Font.WithSize(15);
            WifiView.AddSubview(txtip);

            lblport = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblport.Font = lblLinkType.Font.WithSize(15);
            lblport.Text = "Port Number";
            WifiView.AddSubview(lblport);
            

            txtport = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtport.ReturnKeyType = UIReturnKeyType.Next;
            txtport.ShouldReturn = (tf) =>
            {
                txtPaperType.BecomeFirstResponder();
                return true;
            };

            txtport.AttributedPlaceholder = new NSAttributedString("9100", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtport.Font = txtport.Font.WithSize(15);
            WifiView.AddSubview(txtport);
            Viewline = new UIView();

            Viewline.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            Viewline.TranslatesAutoresizingMaskIntoConstraints = false;
            WifiView.AddSubview(Viewline);


            #endregion

            #region btnTestPrint
            btnTestPrint = new UIButton();
            btnTestPrint.Layer.CornerRadius = 5;
            btnTestPrint.Layer.BorderWidth = 0.4f;
            btnTestPrint.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
            btnTestPrint.BackgroundColor = UIColor.FromRGB(51, 170, 225);
            btnTestPrint.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnTestPrint.TranslatesAutoresizingMaskIntoConstraints = false;
            btnTestPrint.TouchUpInside += async (sender, e) => {
                PrintTest();
            };
            #endregion


            // collectionview

            #region BluethoothListCollection
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 60);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            BluethoothListCollection = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            BluethoothListCollection.BackgroundColor = UIColor.White;
            BluethoothListCollection.ShowsVerticalScrollIndicator = false;
            BluethoothListCollection.TranslatesAutoresizingMaskIntoConstraints = false;
            BluethoothListCollection.RegisterClassForCell(cellType: typeof(BluetoothViewCell), reuseIdentifier: "BluetoothViewCell");


            PrinterDataSource BranchDataList = new PrinterDataSource(Bluetooths); // ส่ง list ไป
            BluethoothListCollection.DataSource = BranchDataList;
            PrinterCollectionDelegate BlueToothCollectionDelegate = new PrinterCollectionDelegate();
            BlueToothCollectionDelegate.OnItemSelected += async (indexPath) => {
                var id = Bluetooths[indexPath.Row].id;
                DataCashingAll.setting.BLUETOOTH1 = id.ToString();
                var name = Bluetooths[indexPath.Row].BluetoothName;
                DataCashingAll.setting.BLUETOOTH2 = name;
                var setting = JsonConvert.SerializeObject(DataCashingAll.setting);
                Preferences.Set("Setting", setting);
                Bluetooths.ConvertAll(x => x.BluetoothStatus = "off");
                Bluetooths[indexPath.Row].BluetoothStatus = "on";
                ((PrinterDataSource)BluethoothListCollection.DataSource).ReloadData(Bluetooths);
                BluethoothListCollection.ReloadData();


                #region Newprint
                //Plugin.BLE.Abstractions.ConnectParameters connectParameters = new Plugin.BLE.Abstractions.ConnectParameters(false, false);
                //var adapter = CrossBluetoothLE.Current.Adapter;
                //adapter.ScanTimeout = 1000;
                //adapter.ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode.Balanced;
                //if (DataCashingAll.setting.BLUETOOTH1 is null)
                //{
                //    Utils.ShowMessage(Utils.TextBundle("pleasesetprinter", "Please set the printer."));
                //    GabanaLoading.SharedInstance.Hide();
                //    return;
                //}
                //bool print = true;
                //var device = await adapter.ConnectToKnownDeviceAsync(Guid.Parse(DataCashingAll.setting.BLUETOOTH1), connectParameters);

                //var t = await device.RequestMtuAsync(500);
                //device.UpdateConnectionInterval(Plugin.BLE.Abstractions.ConnectionInterval.High);
                ////await device.UpdateRssiAsync();
                ////var t2 = await device.RequestMtuAsync(244);

                //var Services = await device.GetServicesAsync();

                //foreach (var ser in Services)
                //{
                //    //if (ser.Name == "Device Information")
                //    //{
                //    var Servicethis = await device.GetServiceAsync(ser.Id);
                //    var characteristics = await Servicethis.GetCharacteristicsAsync();

                //    foreach (var item in characteristics)
                //    {
                //        if (item.CanWrite && print)
                //        {
                //            try
                //            {

                //                print = false;
                //                item.WriteType = Plugin.BLE.Abstractions.CharacteristicWriteType.WithoutResponse;
                //                List<byte> bytelist = new List<byte>();
                //                bytelist.Add((byte)27);
                //                bytelist.Add((byte)97);
                //                bytelist.Add((byte)49);

                //                bytelist.Add((byte)29);
                //                bytelist.Add((byte)33);
                //                bytelist.Add((byte)0);
                //                await item.WriteAsync(bytelist.ToArray());
                //                var typesub = "Window-874q";
                //                switch (typesub)

                //                {
                //                    case "Window-874":
                //                        var enc = Encoding.GetEncoding("windows-874");
                //                        byte[] bytes = enc.GetBytes("Connected Successfully !! \n\n\n\n ");
                //                        //var x = txt1.Length;
                //                        await item.WriteAsync(bytes);
                //                        break;
                //                    default:

                //                        await item.WriteAsync(System.Text.Encoding.UTF8.GetBytes("Connected Successfully !! \n\n\n\n "));
                //                                                                break;
                //                }
                //                await item.WriteAsync(new byte[] { (byte)10 });
                //                Utils.ShowMessage("Print Sucess !");
                //                print = false;
                //            }
                //            catch (Exception ex)
                //            {
                //                //GabanaLoading.SharedInstance.Hide();
                //                //throw new Exception("ไม่สามารถเชื่อมต่อกับเครื่องปริ้นได้");
                //            }
                //        }
                //    }
                //}
                //device.Dispose(); 
                #endregion
            };
            BluethoothListCollection.Delegate = BlueToothCollectionDelegate;
            BluetoothView.AddSubview(BluethoothListCollection);
            #endregion

            btnAddDummy = new UIButton();
            btnAddDummy.Layer.CornerRadius = 5;
            
            btnAddDummy.Layer.BorderWidth = 0.4f;
            btnAddDummy.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
            btnAddDummy.BackgroundColor = UIColor.FromRGB(51, 170, 225);
            btnAddDummy.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnAddDummy.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddDummy.TouchUpInside += async (sender, e) => {
                UpdatewifiConfig();
            };
            WifiView.AddSubview(btnAddDummy);

            View.AddSubview(LinkTypeView);
            View.AddSubview(PaperTypeView);
            View.AddSubview(TypePrintView);

            //  _contentView.AddSubview(BluethoothListCollection);

            //_scrollView.AddSubview(_contentView);

            //View.AddSubview(_scrollView);
            View.AddSubview(WifiView);
            View.AddSubview(BluetoothView); 
        }

        private async void PrintTest()
        {
            try
            {
                await Utils.TestPrint(this);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void UpdatewifiConfig()
        {
            if (string.IsNullOrEmpty(txtip.Text))
            {
                txtip.BecomeFirstResponder();
                Utils.ShowMessage(Utils.TextBundle("plaseenterip", "Please enter IP Address information."));
                //Toast.MakeText(this, "กรุณากรอกข้อมูล IP Adress", ToastLength.Short).Show();
                return;
            }
            if (string.IsNullOrEmpty(txtport.Text))
            {
                txtport.BecomeFirstResponder();
                Utils.ShowMessage("กรุณากรอกข้อมูลการเชื่อมต่อ");
                
                return;
            }
            DataCashingAll.setting.IPADDRESS = txtip.Text;
            DataCashingAll.setting.PORTNUMBER = txtport.Text;
            var setting = JsonConvert.SerializeObject(DataCashingAll.setting);
            Preferences.Set("Setting", setting);
            Utils.ShowMessage(Utils.TextBundle("savedatasuccessfully", "Save data successfully"));
        }
        #region SelectType
        [Export("PaperType:")]
        public void PaperType(UIGestureRecognizer sender)
        {
            txtPaperType.BecomeFirstResponder();
        }
        [Export("TypePrint:")]
        public void TypePrint(UIGestureRecognizer sender)
        {
            txtTypePrint.BecomeFirstResponder();
        }
        [Export("LinkType:")]
        public void LinkType(UIGestureRecognizer sender)
        {
            txtLinkType.BecomeFirstResponder();
        }
        #endregion
        private readonly List<string> LinkTypeTxt = new List<string>
        {
        "Bluetooth",
        "Wifi"
        };

        private readonly List<string> PaperTypeTxt = new List<string>
        {
        "58 mm.",
        "80 mm."
        };
        private readonly List<string> TypePrintTxt= new List<string>
        {
        "Image",
        "Text"
        };
        public class PickerModel : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;

            private UILabel personLabel;


            private readonly IList<string> values;
            public PickerModel(IList<string> values)
            {
                this.values = values;
            }

            public override nint GetComponentCount(UIPickerView v)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return values.Count;
            }

            public override string GetTitle(UIPickerView picker, nint row, nint component)
            {
                return values[Convert.ToInt32(row)];
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (this.PickerChanged != null)
                {
                    this.PickerChanged(this, new PickerChangedEventArgs { SelectedValue = values[Convert.ToInt32(row)] });
                }
            }



            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }


        }
        public void SetupPicker()
        {
            #region LinkType 
            PickerModel modelLinkType = new PickerModel(LinkTypeTxt);
            txtLinkType.Text = DataCashingAll.setting.TYPE;
            modelLinkType.PickerChanged += (sender, e) => {
                txtLinkType.Text = e.SelectedValue;
                if (e.SelectedValue == "Bluetooth")
                {
                    
                    //naviitem.RightBarButtonItem.Hidden = false;
                    DataCashingAll.setting.TYPE = "Bluetooth";
                    WifiView.Hidden = true;
                    BluetoothView.Hidden = false;
                }
                else
                {
                    
                    //naviitem.RightBarButtonItem.Hidden = true;
                    DataCashingAll.setting.TYPE = "Wifi";
                    WifiView.Hidden = false;
                    BluetoothView.Hidden = true;
                }
                var setting = JsonConvert.SerializeObject(DataCashingAll.setting);
                Preferences.Set("Setting", setting);
            };
            UIToolbar toolbar = new UIToolbar();
            toolbar.Translucent = true;
            toolbar.SizeToFit();
            var flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                View.EndEditing(true);
            });
            toolbar.SetItems(new UIBarButtonItem[] { flexible, doneButton }, true);
            var pickertypeprint = new UIPickerView() { Model = modelLinkType, ShowSelectionIndicator = true };
            txtLinkType.InputView = pickertypeprint;
            if (DataCashingAll.setting.TYPE == "Bluetooth")
            {
                pickertypeprint.Select(0, 0, false);
            }
            else
            {
                pickertypeprint.Select(1, 0, false);
            }
            
            txtLinkType.InputAccessoryView = toolbar;

            #endregion
            #region PaperType
            PickerModel modelPaperType = new PickerModel(PaperTypeTxt);
            txtPaperType.Text = PaperTypeTxt[0];
            modelPaperType.PickerChanged += (sender, e) => {
                txtPaperType.Text = e.SelectedValue;
                if (e.SelectedValue == "58 mm.")
                {
                    DataCashingAll.setting.TYPEPAGE = "58mm";
                }
                else
                {
                    DataCashingAll.setting.TYPEPAGE = "80mm";
                }
                var setting = JsonConvert.SerializeObject(DataCashingAll.setting);
                Preferences.Set("Setting", setting);
            };

            txtPaperType.InputView = new UIPickerView() { Model = modelPaperType, ShowSelectionIndicator = true };
            txtPaperType.InputAccessoryView = toolbar;


            #endregion
            #region TypePrint
            PickerModel modelTypePrint = new PickerModel(TypePrintTxt);
            txtTypePrint.Text = TypePrintTxt[0];
            modelTypePrint.PickerChanged += (sender, e) => {
                txtTypePrint.Text = e.SelectedValue;
                if (e.SelectedValue == "Image")
                {
                    DataCashingAll.PrintType = "Image";
                }
                else
                {
                    DataCashingAll.PrintType = "Text";
                }
                Preferences.Set("PrintType", DataCashingAll.PrintType);
            };

            txtTypePrint.InputView = new UIPickerView() { Model = modelTypePrint, ShowSelectionIndicator = true };
            txtTypePrint.InputAccessoryView = toolbar;


            #endregion
        }
        void SetupAutoLayout()
        {
            //UIScrollView can be any size 
            //_scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            //_scrollView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            //_scrollView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            //_scrollView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            ////Inner UIView has to be attached to all UIScrollView constraints
            //_contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            //_contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            //_contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            //_contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            //_contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region LinkTypeView
            LinkTypeView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            LinkTypeView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            LinkTypeView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            LinkTypeView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblLinkType.TopAnchor.ConstraintEqualTo(LinkTypeView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            lblLinkType.LeftAnchor.ConstraintEqualTo(LinkTypeView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblLinkType.HeightAnchor.ConstraintEqualTo(20).Active = true;

            txtLinkType.TopAnchor.ConstraintEqualTo(lblLinkType.SafeAreaLayoutGuide.BottomAnchor,2).Active = true;
            txtLinkType.LeftAnchor.ConstraintEqualTo(LinkTypeView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtLinkType.HeightAnchor.ConstraintEqualTo(18).Active = true;


            btnLinkType.CenterYAnchor.ConstraintEqualTo(LinkTypeView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnLinkType.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnLinkType.RightAnchor.ConstraintEqualTo(LinkTypeView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnLinkType.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            #region PaperTypeView
            PaperTypeView.TopAnchor.ConstraintEqualTo(LinkTypeView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            PaperTypeView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            //PaperTypeView.BottomAnchor.ConstraintEqualTo(PaperTypeView.Superview.BottomAnchor,0).Active=true;
            PaperTypeView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            PaperTypeView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

           

            lblPaperType.TopAnchor.ConstraintEqualTo(PaperTypeView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            lblPaperType.LeftAnchor.ConstraintEqualTo(PaperTypeView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblPaperType.HeightAnchor.ConstraintEqualTo(20).Active = true;

            txtPaperType.TopAnchor.ConstraintEqualTo(lblPaperType.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtPaperType.LeftAnchor.ConstraintEqualTo(PaperTypeView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtPaperType.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnPaperType.CenterYAnchor.ConstraintEqualTo(PaperTypeView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnPaperType.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnPaperType.RightAnchor.ConstraintEqualTo(PaperTypeView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnPaperType.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            #region TypePrintView
            TypePrintView.TopAnchor.ConstraintEqualTo(PaperTypeView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            TypePrintView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            //PaperTypeView.BottomAnchor.ConstraintEqualTo(PaperTypeView.Superview.BottomAnchor,0).Active=true;
            TypePrintView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            TypePrintView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;



            lblTypePrint.TopAnchor.ConstraintEqualTo(TypePrintView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            lblTypePrint.LeftAnchor.ConstraintEqualTo(TypePrintView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblTypePrint.HeightAnchor.ConstraintEqualTo(20).Active = true;

            txtTypePrint.TopAnchor.ConstraintEqualTo(lblTypePrint.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtTypePrint.LeftAnchor.ConstraintEqualTo(TypePrintView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtTypePrint.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnTypePrint.CenterYAnchor.ConstraintEqualTo(TypePrintView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnTypePrint.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnTypePrint.RightAnchor.ConstraintEqualTo(TypePrintView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnTypePrint.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            #region Blue
            BluetoothView.TopAnchor.ConstraintEqualTo(TypePrintView.SafeAreaLayoutGuide.BottomAnchor,5).Active = true;
            //BluetoothView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            BluetoothView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BluetoothView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BluetoothView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            //BluetoothView.BackgroundColor = UIColor.Red;


            lblBluetooth.TopAnchor.ConstraintEqualTo(BluetoothView.SafeAreaLayoutGuide.TopAnchor, 5).Active = true;
            //lblBluetooth.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            lblBluetooth.LeftAnchor.ConstraintEqualTo(BluetoothView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblBluetooth.RightAnchor.ConstraintEqualTo(BluetoothView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblBluetooth.HeightAnchor.ConstraintEqualTo(18).Active = true;

            BluethoothListCollection.TopAnchor.ConstraintEqualTo(lblBluetooth.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            //lblBluetooth.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            BluethoothListCollection.LeftAnchor.ConstraintEqualTo(BluetoothView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            BluethoothListCollection.RightAnchor.ConstraintEqualTo(BluetoothView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            BluethoothListCollection.BottomAnchor.ConstraintEqualTo(BluetoothView.SafeAreaLayoutGuide.BottomAnchor, -15).Active = true;


            WifiView.TopAnchor.ConstraintEqualTo(PaperTypeView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            WifiView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            WifiView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            WifiView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblWifi.TopAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.TopAnchor, 5).Active = true;
            lblWifi.LeftAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblWifi.RightAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblWifi.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblip.TopAnchor.ConstraintEqualTo(lblWifi.SafeAreaLayoutGuide.BottomAnchor, 11).Active = true;
            lblip.LeftAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblip.RightAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblip.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtip.TopAnchor.ConstraintEqualTo(lblip.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtip.LeftAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtip.RightAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            txtip.HeightAnchor.ConstraintEqualTo(18).Active = true;

            Viewline.TopAnchor.ConstraintEqualTo(txtip.SafeAreaLayoutGuide.BottomAnchor, 11).Active = true;
            Viewline.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            Viewline.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            Viewline.HeightAnchor.ConstraintEqualTo(1).Active = true;

            lblport.TopAnchor.ConstraintEqualTo(Viewline.SafeAreaLayoutGuide.BottomAnchor, 11).Active = true;
            lblport.LeftAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblport.RightAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblport.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtport.TopAnchor.ConstraintEqualTo(lblport.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtport.LeftAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtport.RightAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            txtport.HeightAnchor.ConstraintEqualTo(18).Active = true;


            btnAddDummy.HeightAnchor.ConstraintEqualTo(45).Active = true;
            //btnAddDummy.TopAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAddDummy.LeftAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddDummy.RightAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnAddDummy.BottomAnchor.ConstraintEqualTo(WifiView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;

            #endregion

            // collectionview
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
    }
   
}