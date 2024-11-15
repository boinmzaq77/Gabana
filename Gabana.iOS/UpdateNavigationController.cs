using Foundation;
using Gabana.iOS.ITEMS;
using System;
using UIKit;

namespace Gabana.iOS
{
    public partial class UpdateNavigationController : UINavigationController
    {
        public UpdateNavigationController()
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // LoginController main = new LoginController();
            UpdatProfileController update = new UpdatProfileController();



            //AddItemController main = new AddItemController();
            this.PushViewController(update, true);
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }
    }
}