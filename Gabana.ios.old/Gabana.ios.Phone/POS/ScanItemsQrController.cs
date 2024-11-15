using CoreGraphics;
using Foundation;
using Gabana.Model;
using System;
using System.Collections.Generic;
using UIKit;
using ZXing.Mobile;

namespace Gabana.ios.Phone
{
    public partial class ScanItemsQrController : UIViewController
    {
        public static List<itemPOS> cart = new List<itemPOS>();
        ZXingScannerView scannerView;
        public ScanItemsQrController(IntPtr handle) : base(handle)
        {
        }
        public async override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

        }
        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationItem.Title = "";
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.NavigationBar.TintColor = new UIColor(red: 0f, green: 149f / 255f, blue: 218f / 255f, alpha: 1f);
            btnSumCart.Enabled = false;
            UICollectionViewFlowLayout itemViewPOS = itemPOScollection.CollectionViewLayout as UICollectionViewFlowLayout;
            itemViewPOS.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            itemViewPOS.ItemSize = new CoreGraphics.CGSize((View.Frame.Width), (View.Frame.Height-20)/9);
            itemPOScollection.ShowsHorizontalScrollIndicator = false;

            itemPOScollection.ReloadData();

            itemPOSCollectionDelegate ItemPOSCollectionDelegate = new itemPOSCollectionDelegate();
            ItemPOSCollectionDelegate.OnItemPOSSelected += (indexPathOfMyCoupon) =>
            {
              
            };
            itemPOScollection.Delegate = ItemPOSCollectionDelegate;
            itemPOScollection.DataSource = new ItemPOSViewDataSource(POSController.cart, null,1);

            if (POSController.countItems!= 0)
            {
                btnSumCart.SetTitle(POSController.countItems.ToString() + "item , " + POSController.sumItem.ToString() + "฿", UIControlState.Normal);
                btnSumCart.Enabled = true;
            }
           
            try
            {
            //    UIView camView = new UIView(new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height / 4)) { BackgroundColor = UIColor.Clear };
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                scanner.UseCustomOverlay = false;
             //   scanner.CustomOverlay = camView;

                var result = await scanner.Scan();
                HandleScanResult(result);
            }
            catch (Exception ex)
            {
                ShowAlert(this, "Error !", "10001");
            }
        }
        async void HandleScanResult(ZXing.Result result)
        {
            try
            {
                if (result != null && !string.IsNullOrEmpty(result.Text))
                {
                    ShowAlert(this, "OK !", result.Text);
                }
            }
            catch (Exception ex)
            {
                ShowAlert(this, "ไม่สำเร็จ !", "ไม่พบสินค้า");
            }

        }
        internal static void ShowAlert(UIViewController uIViewController, string title, string detail)
        {
            var alert = UIAlertController.Create(title, detail, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
            uIViewController.PresentViewController(alert, animated: true, completionHandler: null);
        }

        partial void BtnSumCart_TouchUpInside(UIButton sender)
        {
            CartResultController sum = this.Storyboard?.InstantiateViewController("CartResultController") as CartResultController;
            this.NavigationController.PushViewController(sum, true);
        }
    }
}