using Foundation;
using System;
using UIKit;

namespace Gabana.ios.Phone
{
    public partial class loginNavigationController : UINavigationController
    {
        public loginNavigationController (IntPtr handle) : base (handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
          
            //this.NavigationBar.BarTintColor = UIColor.Orange;
        }
    }
}