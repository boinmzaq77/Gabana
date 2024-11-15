using CoreFoundation;
using Foundation;
using Gabana.iOS;
using Newtonsoft.Json;
using Plugin.BLE;
using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Drawing;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{

    public class BluetoothController : UIViewController
    {
        UILabel labelSettingBluetooth; 
        UICollectionView uICollectionView;
        UIButton btnSearch;
        public static List<Plugin.BLE.Abstractions.Contracts.IDevice> devices = new List<Plugin.BLE.Abstractions.Contracts.IDevice>();

        public BluetoothController()
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {

            base.ViewDidLoad();
            try
            {
                 View.BackgroundColor = UIColor.White;
                labelSettingBluetooth = new UILabel
                {
                    Text = "ตั้งค่า Bluetooth",
                    TextColor = UIColor.FromRGB(255, 120, 46),
                TranslatesAutoresizingMaskIntoConstraints = false,
                    TextAlignment = UITextAlignment.Left
                };
                labelSettingBluetooth.Font = labelSettingBluetooth.Font.WithSize(25);
                View.AddSubview(labelSettingBluetooth);

                UICollectionViewFlowLayout flowLayout = new UICollectionViewFlowLayout();
                flowLayout.SectionInset = new UIEdgeInsets(top: 5, left:5, bottom: 5, right: 5);
                flowLayout.ItemSize = new CoreGraphics.CGSize(width: View.Frame.Width - 50 , height: 60);
                uICollectionView = new UICollectionView(frame: View.Frame, layout: flowLayout);
                uICollectionView.BackgroundColor = UIColor.White;
                uICollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
                uICollectionView.RegisterClassForCell(cellType: typeof(BluetoothViewCell), reuseIdentifier: "MyCell");
                var filterdatasource = new BluetoothDataSource(devices);
                uICollectionView.DataSource = filterdatasource;
                var cardCollectionDelegate = new BluetoothCollectionDelegate();
                var delegatefilter = new UICollectionViewDelegate();

                
                
                cardCollectionDelegate.OnCardSelected += (indexPath) =>
                {
                    var id = devices[indexPath.Row].Id;
                    DataCaching.setting.BLUETOOTH1 = id.ToString();
                    var name = devices[indexPath.Row].Name;
                    DataCaching.setting.BLUETOOTH2 = name;
                    var setting = JsonConvert.SerializeObject(DataCaching.setting);
                    Preferences.Set("Setting", setting);
                    this.NavigationController.PopViewController(false);
                };


                uICollectionView.Delegate = cardCollectionDelegate;
                View.AddSubview(uICollectionView);

                btnSearch = new UIButton();
                btnSearch.SetTitle("Search", UIControlState.Normal);
                btnSearch.Layer.CornerRadius = 20f;
                btnSearch.BackgroundColor = UIColor.FromRGB(255, 120, 46);
                btnSearch.TranslatesAutoresizingMaskIntoConstraints = false;
                btnSearch.TouchUpInside  += async (sender, e)  =>  {
                     btnSearch_TouchUpInside();
                };

                View.AddSubview(btnSearch);
                // Perform any additional setup after loading the view
                setupAutoLayout();
                UIBarButtonItem selectCustomer = new UIBarButtonItem();
                UIImageView uIImage = new UIImageView(new CoreGraphics.CGRect(0, 0, 10, 10));
                uIImage.Image = UIImage.FromBundle("SeniorSoftLogo");
                uIImage.TranslatesAutoresizingMaskIntoConstraints = false;
                //uIImage.WidthAnchor.ConstraintEqualTo(80).Active = true;
                uIImage.HeightAnchor.ConstraintEqualTo(30).Active = true;
                uIImage.ContentMode = UIViewContentMode.ScaleAspectFit;
                this.NavigationItem.TitleView = uIImage;
                this.NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), default);
                this.NavigationController.NavigationBar.ShadowImage = new UIImage();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private async void btnSearch_TouchUpInside()
        {
            try
            {
                //CrossBleAdapter.Current.Scan().Subscribe(scanResult => { });

                GabanaLoading.SharedInstance.Show(this.NavigationController);
                var ble = CrossBluetoothLE.Current;
                var adapter = CrossBluetoothLE.Current.Adapter;
                adapter.ScanTimeout = 10000;
                adapter.ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode.LowPower;
                adapter.DeviceDiscovered += (obj, a) =>
                {
                    Guid aa = a.Device.Id;
                    if (!BluetoothController.devices.Contains(a.Device) && !String.IsNullOrEmpty(a.Device.Name))
                        BluetoothController.devices.Add(a.Device);
                };
                if (!ble.Adapter.IsScanning)
                {
                    await adapter.StartScanningForDevicesAsync();

                }

                ((BluetoothDataSource)uICollectionView.DataSource).ReloadData(devices);
                uICollectionView.ReloadData();
                GabanaLoading.SharedInstance.Hide();
            }
            catch (Exception ex )
            {
                
            }
            

        }

        private void setupAutoLayout()
        {
            labelSettingBluetooth.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor, 0).Active = true;
            labelSettingBluetooth.TopAnchor.ConstraintEqualTo(View.TopAnchor, 120).Active = true;
            labelSettingBluetooth.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 10).Active = true;
            labelSettingBluetooth.RightAnchor.ConstraintEqualTo(View.RightAnchor, -10).Active = true;
            labelSettingBluetooth.HeightAnchor.ConstraintEqualTo(50).Active = true;

            uICollectionView.TopAnchor.ConstraintEqualTo(labelSettingBluetooth.BottomAnchor, 30).Active = true;
            uICollectionView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 20).Active = true;
            uICollectionView.RightAnchor.ConstraintEqualTo(View.RightAnchor, -20).Active = true;
            uICollectionView.BottomAnchor.ConstraintEqualTo(btnSearch.TopAnchor, -20).Active = true;

            btnSearch.TopAnchor.ConstraintEqualTo(uICollectionView.BottomAnchor, 10).Active = true;
            btnSearch.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 20).Active = true;
            btnSearch.RightAnchor.ConstraintEqualTo(View.RightAnchor, -20).Active = true;
            btnSearch.HeightAnchor.ConstraintEqualTo(50).Active = true;
            //btnSearch.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -20).Active = true;

            UIImageView viewfooter = new UIImageView(new CoreGraphics.CGRect(0, 0, 50, 50));
            viewfooter.Image = UIImage.FromBundle("mymaxxFooter");
            viewfooter.LargeContentImageInsets = new UIEdgeInsets(0, 0, 0, 0);
            viewfooter.ContentMode = UIViewContentMode.ScaleAspectFit;
            viewfooter.TranslatesAutoresizingMaskIntoConstraints = false;
            //viewfooter.BackgroundColor = UIColor.Black;
            View.AddSubview(viewfooter);

            viewfooter.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            viewfooter.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;
            viewfooter.HeightAnchor.ConstraintEqualTo(40).Active = true;
            viewfooter.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            UIView line = new UIView();
            line.BackgroundColor = UIColor.Gray;
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(line);

            line.TopAnchor.ConstraintEqualTo(btnSearch.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            line.BottomAnchor.ConstraintEqualTo(viewfooter.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            line.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            line.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;
            line.HeightAnchor.ConstraintEqualTo(1).Active = true;

        }
    }
}