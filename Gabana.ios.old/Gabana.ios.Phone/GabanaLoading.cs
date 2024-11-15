using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using FFImageLoading;
using Foundation;
using UIKit;

namespace Gabana.ios.Phone
{
    public class GabanaLoading
    {
        public  static GabanaLoading SharedInstance = new GabanaLoading();

        nfloat SCREEN_WIDTH = UIScreen.MainScreen.Bounds.Size.Width;
        nfloat SCREEN_HEIGHT = UIScreen.MainScreen.Bounds.Size.Height;

        UIView container = null;
        UIImageView imageViewLoading = null;

        public GabanaLoading()
        {
            container = new UIView(frame: new CGRect(x: 0, y: 0, width: SCREEN_WIDTH, height: SCREEN_HEIGHT));

            imageViewLoading = new UIImageView();
            imageViewLoading.Frame = new CGRect(new CGPoint(0, 0), new CGSize(80, 80));
            imageViewLoading.Center = container.Center;

            container.BackgroundColor = UIColor.Clear;
        }

        public void Show(UIViewController viewController)
        {
            container.BackgroundColor = UIColor.Black.ColorWithAlpha((nfloat)0.1);
            container.AddSubview(imageViewLoading);
            viewController.View.AddSubview(container);
        }

        public void Hide()
        {
            imageViewLoading.RemoveFromSuperview();
            container.RemoveFromSuperview();
        }
    }
}