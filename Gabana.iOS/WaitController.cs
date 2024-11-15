using CoreFoundation;
using Foundation;
using Gabana.iOS;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using UIKit;

namespace Gabana
{
    
    public class WaitController : UIViewController
    {
        public WaitController()
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            Thread.Sleep(5000);
            SplashLoadingController con = new SplashLoadingController();
            PresentViewControllerAsync(con, true);
            // Perform any additional setup after loading the view
        }
    }
}