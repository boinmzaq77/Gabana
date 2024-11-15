using Foundation;
using System;
using UIKit;

namespace Gabana.ios.Phone
{
    public partial class CartResultController : UIViewController
    {
        public CartResultController (IntPtr handle) : base (handle)
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

            UICollectionViewFlowLayout itemViewPOS = itemPOScollection.CollectionViewLayout as UICollectionViewFlowLayout;
            itemViewPOS.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            itemViewPOS.ItemSize = new CoreGraphics.CGSize((View.Frame.Width), (View.Frame.Height) / 9);
            itemPOScollection.ShowsHorizontalScrollIndicator = false;

            itemPOScollection.ReloadData();
            if (POSController.cart.Count != 0)
            {
                btnGoSumResult.SetTitle(POSController.countItems.ToString() + "item , " + POSController.sumItem.ToString() + "ß", UIControlState.Normal);
                btnGoSumResult.Enabled = true;
            }
            itemPOSCollectionDelegate ItemPOSCollectionDelegate = new itemPOSCollectionDelegate();
            ItemPOSCollectionDelegate.OnItemPOSSelected += (indexPathOfMyCoupon) =>
            {

            };
            itemPOScollection.Delegate = ItemPOSCollectionDelegate;
            itemPOScollection.DataSource = new ItemPOSViewDataSource(POSController.cart, null, 1);

            lblsumPrice.Text = "ß "+POSController.sumItem.ToString("N2");
        }

        partial void BtnGoSumResult_TouchUpInside(UIButton sender)
        {
            PaymentController sum = this.Storyboard?.InstantiateViewController("PaymentController") as PaymentController;
            this.NavigationController.PushViewController(sum, true);
        }

        partial void BtnOptions_TouchUpInside(UIButton sender)
        {
            //btnOptions show bottom navbar up

        }
    }
}