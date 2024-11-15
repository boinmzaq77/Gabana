using Foundation;
using System;
using UIKit;

namespace Gabana.ios.Phone
{
    public partial class AddNewItemPOSController : UIViewController
    {
        public AddNewItemPOSController(IntPtr handle) : base (handle)
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
            this.NavigationItem.Title = "New item";

            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.NavigationBar.TintColor = new UIColor(red: 0f, green: 149f / 255f, blue: 218f / 255f, alpha: 1f);
            viewProduct.Hidden = false;
        }
        //viewProduct
        partial void SegmenteControl_ValueChanged(UISegmentedControl sender)
        {
            var index = segSelectNewCreate.SelectedSegment;
            if (index == 0)
            {
                viewProduct.Hidden = false;
                viewStock.Hidden = true;
            }
            else if (index == 1)
            {
                viewProduct.Hidden = true;
                viewStock.Hidden = false;
            }
        }
    }
}