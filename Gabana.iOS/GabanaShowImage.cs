using CoreGraphics;
using FFImageLoading;
using Foundation;
using Gabana.iOS;
using System;
using UIKit;

namespace Gabana
{
    class GabanaShowImage
    {
        public static GabanaShowImage SharedInstance = new GabanaShowImage();

        nfloat SCREEN_WIDTH = UIScreen.MainScreen.Bounds.Size.Width;
        nfloat SCREEN_HEIGHT = UIScreen.MainScreen.Bounds.Size.Height;

        UIView container = null;
        UIImageView imageViewLoading = null;

        public GabanaShowImage()
        {
            container = new UIView(frame: new CGRect(x: 0, y: 0, width: SCREEN_WIDTH, height: SCREEN_HEIGHT));

            imageViewLoading = new UIImageView();
            imageViewLoading.ContentMode = UIViewContentMode.ScaleAspectFit; 
            imageViewLoading.Frame = new CGRect(new CGPoint(0, 0), new CGSize(80, 80));
            imageViewLoading.Center = container.Center;

            // ImageService.Instance.LoadCompiledResource("FINAL-EX-G.gif")
            
            ImageService.Instance.LoadCompiledResource("gabana-loading-final-512x512.gif")
             .WithCache(FFImageLoading.Cache.CacheType.Memory)

            .Into(imageViewLoading);
            container.BackgroundColor = UIColor.Clear;
            
        }

        public void Show(UIViewController viewController,string path, string Placeholderpath)
        {
            container.BackgroundColor = UIColor.Black.ColorWithAlpha((nfloat)0.2);

            imageViewLoading.Frame = new CGRect(new CGPoint(25, 0), new CGSize(viewController.View.Frame.Width - 50 , viewController.View.Frame.Height - 50));
            ImageService.Instance.LoadUrl(path)
                    .LoadingPlaceholder(Placeholderpath, FFImageLoading.Work.ImageSource.CompiledResource)
                    .WithCache(FFImageLoading.Cache.CacheType.Disk)
                    .Into(imageViewLoading);
            
            container.AddSubview(imageViewLoading);
            viewController.NavigationController.View.AddSubview(container);
            container.UserInteractionEnabled = true;
            var tapGesture5 = new UITapGestureRecognizer(viewController,
                    new ObjCRuntime.Selector("showimage:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            container.AddGestureRecognizer(tapGesture5);

        }
        [Export("showimage:")]
        public void Close2(UIGestureRecognizer sender)
        {
            Hide();
        }
        public void Hide()
        {
            imageViewLoading.RemoveFromSuperview();
            container.RemoveFromSuperview();
        }

        internal void Show(UIViewController viewController, UIImage editedImage, string v)
        {
            container.BackgroundColor = UIColor.Black.ColorWithAlpha((nfloat)0.2);

            imageViewLoading.Frame = new CGRect(new CGPoint(25, 0), new CGSize(viewController.View.Frame.Width - 50, viewController.View.Frame.Height - 50));
            
            imageViewLoading.Image = editedImage;

            container.AddSubview(imageViewLoading);
            viewController.NavigationController.View.AddSubview(container);
            container.UserInteractionEnabled = true;
            var tapGesture5 = new UITapGestureRecognizer(viewController,
                    new ObjCRuntime.Selector("showimage:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            container.AddGestureRecognizer(tapGesture5);
        }
    }
}