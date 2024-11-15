using Foundation;
using Gabana.iOS.ITEMS;
using System;
using UIKit;

namespace Gabana.iOS
{
    public partial class LoginNavigationController : UINavigationController
    {
        public LoginNavigationController()
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            LoginController main = new LoginController();
            //MainController main = new MainController();
           // BranchController main = new BranchController();
            this.PushViewController(main, true);
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

        }
    }
}