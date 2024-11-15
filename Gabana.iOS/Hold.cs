using CoreFoundation;
using Foundation;
using System;
using System.Drawing;
using System.Threading;
using UIKit;

namespace Gabana
{
    [Register("UniversalView")]
    
    public class Hold : UIViewController
    {
        public Hold()
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //await Thread.Sleep(5000);
            // Perform any additional setup after loading the view
        }
    }
}