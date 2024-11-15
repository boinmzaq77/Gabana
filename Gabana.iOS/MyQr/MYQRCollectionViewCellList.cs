using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using FFImageLoading;

namespace Gabana.iOS
{
    public class MYQRCollectionViewCellList : UICollectionViewCell
    {
        UILabel lblQRCode, lblBranch;
        UIView viewHead, viewBody;
        UIImageView imageView;

        public MYQRCollectionViewCellList(IntPtr handle) : base(handle)
        {
            ContentView.BackgroundColor = UIColor.Black;
            viewHead = new UIView();
            viewHead.TranslatesAutoresizingMaskIntoConstraints = false;
            viewHead.BackgroundColor = UIColor.FromRGB(241, 250, 255);
            ContentView.AddSubview(viewHead);

            lblQRCode = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblQRCode.TextColor = UIColor.FromRGB(64, 64, 64);
            lblQRCode.TextAlignment = UITextAlignment.Left;
            lblQRCode.Font = lblQRCode.Font.WithSize(15);
            viewHead.AddSubview(lblQRCode);

            lblBranch = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblBranch.TextColor = UIColor.FromRGB(112, 112, 112);
            lblBranch.TextAlignment = UITextAlignment.Left;
            lblBranch.Font = lblBranch.Font.WithSize(15);
            viewHead.AddSubview(lblBranch);

            viewBody = new UIView();
            viewBody.TranslatesAutoresizingMaskIntoConstraints = false;
            viewBody.BackgroundColor = UIColor.FromRGB(255,255,255);
            ContentView.AddSubview(viewBody);

            imageView = new UIImageView();
            imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            imageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            viewBody.AddSubview(imageView);

            //ContentView.BackgroundColor = UIColor.FromRGB(200,200,200);
            ContentView.Layer.BorderColor = UIColor.FromRGB(64,64,64).CGColor;
            ContentView.ClipsToBounds = true;
            ContentView.Layer.CornerRadius = 5;
            ContentView.Layer.ShadowColor = UIColor.FromRGB(226, 226, 226).CGColor;
            ContentView.Layer.ShadowOpacity = 1;
            ContentView.Layer.ShadowRadius = 10;
            setupListView();
        }

        private void setupListView()
        {
            viewHead.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            viewHead.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            viewHead.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewHead.HeightAnchor.ConstraintEqualTo(65).Active = true;

            lblQRCode.TopAnchor.ConstraintEqualTo(viewHead.TopAnchor, 12).Active = true;
            lblQRCode.LeftAnchor.ConstraintEqualTo(viewHead.LeftAnchor, 15).Active = true;
            lblQRCode.RightAnchor.ConstraintEqualTo(viewHead.RightAnchor, -15).Active = true;
            lblQRCode.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblBranch.TopAnchor.ConstraintEqualTo(lblQRCode.BottomAnchor, 2).Active = true;
            lblBranch.LeftAnchor.ConstraintEqualTo(viewHead.LeftAnchor, 15).Active = true;
            lblBranch.RightAnchor.ConstraintEqualTo(viewHead.RightAnchor, -15).Active = true;
            lblBranch.HeightAnchor.ConstraintEqualTo(18).Active = true;

            viewBody.TopAnchor.ConstraintEqualTo(viewHead.SafeAreaLayoutGuide.BottomAnchor, 0.5f).Active = true;
            viewBody.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            viewBody.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewBody.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            imageView.TopAnchor.ConstraintEqualTo(viewBody.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            imageView.LeftAnchor.ConstraintEqualTo(viewBody.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            imageView.RightAnchor.ConstraintEqualTo(viewBody.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            imageView.BottomAnchor.ConstraintEqualTo(viewBody.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;

        }
        public string Code
        {
            get { return lblQRCode.Text; }
            set
            {
                lblQRCode.Text = value;
            }
        }
        public string BranchName
        {
            get { return lblBranch.Text; }
            set { lblBranch.Text = value; }
        }
        public string Image
        {
            get { return imageView.Image.ToString(); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    ImageService.Instance.LoadUrl(value)
                   .LoadingPlaceholder("DefaultMyQR.png", FFImageLoading.Work.ImageSource.CompiledResource)
                   .WithCache(FFImageLoading.Cache.CacheType.Disk)
                   .Into(imageView);
                }
                else
                {
                    imageView.Image = UIImage.FromBundle("DefaultMyQR");
                }
               
                //Utils.SetImageURL(imageView, value);
            }
        }
        public static void SetImage(UIImageView ImageView, string value)
        {
            if (value != null && value != "")
            {
                ImageView.Image = UIImage.FromBundle(value);
            }
        }

    }
}