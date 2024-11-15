using Foundation;
using System;
using UIKit;

namespace Gabana.ios.Phone
{
    public partial class DummyItemController : UIViewController
    {
        public DummyItemController (IntPtr handle) : base (handle)
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
            //this.NavigationController.NavigationItem.BackButtonDisplayMode = UINavigationItemBackButtonDisplayMode.Default;

            txtAddDummy.BecomeFirstResponder();
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            txtAddDummy.BecomeFirstResponder();
        }

        partial void UIButton107770_TouchUpInside(UIButton sender)
        {
            throw new NotImplementedException();
        }

        partial void BtnAddDrescription_TouchUpInside(UIButton sender)
        {
            // add drescrption
           // throw new NotImplementedException();
        }
    }
}