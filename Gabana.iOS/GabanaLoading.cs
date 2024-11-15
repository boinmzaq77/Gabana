using CoreGraphics;
using FFImageLoading;
using System;
using UIKit;

namespace Gabana
{
    class GabanaLoading
    {
        public static GabanaLoading SharedInstance = new GabanaLoading();

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

            // ImageService.Instance.LoadCompiledResource("FINAL-EX-G.gif")
            
            ImageService.Instance.LoadCompiledResource("gabana-loading-final-512x512.gif")
             .WithCache(FFImageLoading.Cache.CacheType.Memory)
            .Into(imageViewLoading);
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