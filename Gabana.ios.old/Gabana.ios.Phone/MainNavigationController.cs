using Foundation;
using System;
using UIKit;

namespace Gabana.ios.Phone
{
    public partial class MainNavigationController : UINavigationController
    {
        public MainNavigationController(IntPtr handle) : base (handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var g = new UITapGestureRecognizer(() => View.EndEditing(true));
            g.CancelsTouchesInView = false;
            View.AddGestureRecognizer(g);
            //MainController main = this.Storyboard.InstantiateViewController("main") as MainController;
            //this.SetViewControllers(new UIViewController[] { main }, false);
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);


        }
    }
}